//
// # forking cleans up memory leaks
//
// Well, not exactly, but the key behaviour this test executable MUST exhibit (on Windows and elsewhere)
// is that any memory allocations *done in the forked child* will be cleaned up by the OS oncee that child
// has been terminated.
//
// The goal is to show / prove that a production-quality "forever-running" application can be created
// when we fork() off the work in batches and terminatee those children once they tend to consume
// too much memory.
// Think (web)server style applications which perform heavy processing, e.g. PDF rendering, OCR and
// text extraction.
//

// use the portability work of cUrl: faster than writing our own header files...
#include "curl_setup.h" 

#include <stdio.h>
#include <stdlib.h>
#include <sys/types.h>


#if !defined(WIN32) && !defined(WIN64)

int main(void) {
    pid_t pid = fork();

    if (pid == 0) {
        printf("Child => PPID: %d PID: %d\n", getppid(), getpid());
        exit(EXIT_SUCCESS);
    }
    else if (pid > 0) {
        printf("Parent => PID: %d\n", getpid());
        printf("Waiting for child process to finish.\n");
        wait(NULL);
        printf("Child process finished.\n");
    }
    else {
        printf("Unable to create child process.\n");
    }

    return EXIT_SUCCESS;
}

#else

// https://docs.microsoft.com/en-us/windows/win32/procthread/creating-a-child-process-with-redirected-input-and-output

#if !defined(UNICODE)
#error "Compile in UNICODE mode!"
#endif

#define PSAPI_VERSION 2

#include <tlhelp32.h>
#include <psapi.h>
#include <tchar.h>
#include <strsafe.h>
#include <eh.h>

#define ASSERT(check)           \
if (!(check))                   \
{                               \
    ErrorExit(TEXT(#check));    \
}

#define VERIFY(check)           \
if (!(check))                   \
{                               \
    ErrorExit(TEXT(#check));    \
}


#define BUFSIZE 4096 

HANDLE g_hChildStd_IN_Rd = NULL;
HANDLE g_hChildStd_IN_Wr = NULL;
HANDLE g_hChildStd_OUT_Rd = NULL;
HANDLE g_hChildStd_OUT_Wr = NULL;

HANDLE g_hInputFile = NULL;

LPCWSTR szChildArg = TEXT("--child");
LPCWSTR szParentArg = TEXT("--parent");

// Holy crap. https://stackoverflow.com/questions/19536697/unauthorizedaccessexception-when-trying-to-open-a-mutex#comments-19717341
// >  In the documention for .NET 4.7.2 it says that mutexes starting with "Global" are available on "On a server that is running Terminal Services". Do you run on Windows Server?
//
// That took me a couple of hours to discover!  :-(
// With 'Global\\' prefix in the name, the CreateMutex() will still succeed, but it's the
// WaitForSingleObject() that will produce the ACCESS DENIED error! (Windows 10 Pro)
//
//LPCWSTR mutexName = TEXT("Global\\ForkTestAppMutex");
LPCWSTR mutexName = TEXT("ForkTestAppMutex");
LPCWSTR childEventName = TEXT("ForkTestAppChildEvent");

// Format a readable error message, display a message box, 
// and exit from the application.
static void ErrorExit(PCTSTR lpszFunction)
{
    LPVOID lpMsgBuf;
    LPVOID lpDisplayBuf;
    DWORD dw = GetLastError();

    FormatMessage(
        FORMAT_MESSAGE_ALLOCATE_BUFFER |
        FORMAT_MESSAGE_FROM_SYSTEM |
        FORMAT_MESSAGE_IGNORE_INSERTS,
        NULL,
        dw,
        MAKELANGID(LANG_NEUTRAL, SUBLANG_DEFAULT),
        (LPTSTR)&lpMsgBuf,
        0, NULL);

    lpDisplayBuf = (LPVOID)LocalAlloc(LMEM_ZEROINIT,
        (lstrlen((LPCTSTR)lpMsgBuf) + lstrlen((LPCTSTR)lpszFunction) + 40) * sizeof(TCHAR));
    StringCchPrintf((STRSAFE_LPWSTR)lpDisplayBuf,
        LocalSize(lpDisplayBuf) / sizeof(TCHAR),
        TEXT("%s failed with error %d: %s\n"),
        lpszFunction, dw, lpMsgBuf);
    OutputDebugStringW((LPCTSTR)lpDisplayBuf);
    MessageBox(NULL, (LPCTSTR)lpDisplayBuf, TEXT("Error"), MB_OK);

    LocalFree(lpMsgBuf);
    LocalFree(lpDisplayBuf);
    //ExitProcess(1); <-- do NOT call ExitProcess but exit() instead so that our atexit handler will be called!
    exit(1);
}

static void printError(const WCHAR* msg)
{
    DWORD eNum;
    WCHAR sysMsg[256];
    WCHAR* p;

    eNum = GetLastError();
    FormatMessage(FORMAT_MESSAGE_FROM_SYSTEM | FORMAT_MESSAGE_IGNORE_INSERTS,
        NULL, eNum,
        MAKELANGID(LANG_NEUTRAL, SUBLANG_DEFAULT), // Default language
        sysMsg, 256, NULL);

    // Trim the end of the line and terminate it with a null
    p = sysMsg;
    while ((*p > 31) || (*p == 9))
        ++p;
    do { *p-- = 0; } while ((p >= sysMsg) &&
        ((*p == '.') || (*p < 33)));

    // Display the message
    WCHAR m[800];
    wsprintf(m, TEXT("WARNING: %s failed with error %d (%s)\n"), msg, eNum, sysMsg);
    OutputDebugStringW(m);

    _tprintf(TEXT("\n  WARNING: %s failed with error %d (%s)"), msg, eNum, sysMsg);
}

// Read from a file and write its contents to the pipe for the child's STDIN.
// Stop when there is no more data. 
static void WriteToPipe(void)
{
    DWORD dwRead, dwWritten;
    CHAR chBuf[BUFSIZE];
    BOOL bSuccess = FALSE;

    for (;;)
    {
        OutputDebugStringA("WriteToPipe::ReadFile...\n");
        bSuccess = ReadFile(g_hInputFile, chBuf, BUFSIZE, &dwRead, NULL);
        if (!bSuccess || dwRead == 0) break;

        OutputDebugStringA("WriteToPipe::WriteFile(g_hChildStd_IN_Wr)...\n");
        bSuccess = WriteFile(g_hChildStd_IN_Wr, chBuf, dwRead, &dwWritten, NULL);
        if (!bSuccess) break;
    }

    VERIFY(FlushFileBuffers(g_hChildStd_IN_Wr));

    // Close the pipe handle so the child process stops reading. 

    OutputDebugStringA("CloseHandle(g_hChildStd_IN_Wr)\n");
    if (!CloseHandle(g_hChildStd_IN_Wr))
    {
        ErrorExit(TEXT("StdInWr CloseHandle"));
    }
}

// Read output from the child process's pipe for STDOUT
// and write to the parent process's pipe for STDOUT. 
// Stop when there is no more data. 
static void ReadFromPipe(void)
{
    DWORD dwRead, dwWritten;
    CHAR chBuf[BUFSIZE];
    BOOL bSuccess = FALSE;
    HANDLE hParentStdOut = GetStdHandle(STD_OUTPUT_HANDLE);

    OutputDebugStringA("ReadFromPipe start...\n");
    for (;;)
    {
        OutputDebugStringA("ReadFromPipe::ReadFile...\n");
        bSuccess = ReadFile(g_hChildStd_OUT_Rd, chBuf, BUFSIZE, &dwRead, NULL);
        {
            wchar_t msg[200];
            wsprintf(msg, TEXT("ReadFile(g_hChildStd_OUT_Rd) -> dwRead:%d, success:%d\n"), (int)dwRead, (int)bSuccess);
            OutputDebugStringW(msg);
        }
        if (!bSuccess || dwRead == 0) break;

        OutputDebugStringA("ReadFromPipe::WriteFile...\n");
        bSuccess = WriteFile(hParentStdOut, chBuf, dwRead, &dwWritten, NULL);
        {
            wchar_t msg[200];
            wsprintf(msg, TEXT("WriteFile(hParentStdOut) -> dwWritten:%d, success:%d\n"), (int)dwWritten, (int)bSuccess);
            OutputDebugStringW(msg);
        }
        if (!bSuccess) break;
    }
    VERIFY(FlushFileBuffers(hParentStdOut));
    OutputDebugStringA("ReadFromPipe END.\n");
}

static DWORD WINAPI ThreadProc(LPVOID lpParameter)
{
    OutputDebugString(TEXT("Making sure the child stdout does not block on buffer full: copy it to parent stdout in separate thread.\n"));

    ReadFromPipe();

    OutputDebugString(TEXT("End of async ThreadProc.\n"));
    return 0;
}

static void CreateChildProcess()
// Create a child process that uses the previously created pipes for STDIN and STDOUT.
{
    TCHAR szAppPath[MAX_PATH];
    TCHAR szCmdline[max(2 * MAX_PATH, 1000)];
    PROCESS_INFORMATION piProcInfo;
    STARTUPINFO siStartInfo;
    BOOL bSuccess = FALSE;

    if (!GetModuleFileName(NULL, szAppPath, sizeof(szAppPath) / sizeof(TCHAR)))
    {
        ErrorExit(TEXT("GetModuleFileName"));
    }
    else
    {
        LPCWSTR args = GetCommandLine();
        // skip argv[0]:
        if (args[0] == L'"')
        {
            args = wcschr(args + 1, L'"') + 1;
            if (*args == L' ')
            {
                ++args;
            }
        }
        else
        {
            args = wcschr(args, L' ');
            if (!args)
            {
                // no args!
                args = TEXT("");
            }
        }
        args = TEXT("");
        StringCchPrintf(szCmdline,
            sizeof(szCmdline) / sizeof(TCHAR),
            TEXT("\"%s\" %s %s"), szAppPath, szChildArg, args);

        // Set up members of the PROCESS_INFORMATION structure. 

        ZeroMemory(&piProcInfo, sizeof(PROCESS_INFORMATION));

        // Set up members of the STARTUPINFO structure. 
        // This structure specifies the STDIN and STDOUT handles for redirection.

        ZeroMemory(&siStartInfo, sizeof(STARTUPINFO));
        siStartInfo.cb = sizeof(STARTUPINFO);
        siStartInfo.hStdError = g_hChildStd_OUT_Wr;
        siStartInfo.hStdOutput = g_hChildStd_OUT_Wr;
        siStartInfo.hStdInput = g_hChildStd_IN_Rd;
        siStartInfo.dwFlags |= STARTF_USESTDHANDLES;

        HANDLE childEventHandle = CreateEvent(NULL, FALSE, FALSE, childEventName);
        if (childEventHandle == NULL)
        {
            printError(TEXT("CreateEvent"));
        }

        OutputDebugStringW(TEXT("CreateProcess..."));

        // Create the child process. 

        bSuccess = TRUE;
#if true
        bSuccess = CreateProcess(szAppPath,
            szCmdline,     // command line argument(s) 
            NULL,          // process security attributes 
            NULL,          // primary thread security attributes 
            TRUE,          // handles are inherited 
            CREATE_UNICODE_ENVIRONMENT,             // creation flags 
            NULL,          // use parent's environment 
            NULL,          // use parent's current directory 
            &siStartInfo,  // STARTUPINFO pointer 
            &piProcInfo);  // receives PROCESS_INFORMATION 
#endif

        // If an error occurs, exit the application. 
        if (!bSuccess)
        {
            ErrorExit(TEXT("CreateProcess"));
        }
        else
        {
#if false   // only for GUI apps: this expects a message pump
            DWORD rv = WaitForInputIdle(piProcInfo.hProcess, INFINITE);
            if (rv != 0)
            {
                printError(TEXT("WaitForInputIdle"));
            }
#endif

            CreateThread(NULL, 0, ThreadProc, NULL, 0, NULL);

            OutputDebugStringA("waiting for child to start...\n");
            printf("waiting for child to start...\n");
            DWORD rv = WaitForSingleObject(childEventHandle, INFINITE);
            if (rv != WAIT_OBJECT_0)
            {
                printError(TEXT("WaitForSingleObject::ChildEvent"));
            }

            VERIFY(CloseHandle(childEventHandle));
            OutputDebugStringA("child has started...\n");
            printf("child has started...\n");

            // Close handles to the child process and its primary thread.
            // Some applications might keep these handles to monitor the status
            // of the child process, for example. 

            CloseHandle(piProcInfo.hProcess);
            CloseHandle(piProcInfo.hThread);

            // Close handles to the stdin and stdout pipes no longer needed by the child process.
            // If they are not explicitly closed, there is no way to recognize that the child process has ended.

            CloseHandle(g_hChildStd_OUT_Wr);
            CloseHandle(g_hChildStd_IN_Rd);
        }
    }
}

class CMutexWorkers
{
protected:
    HANDLE        m_semWorkers;
    int        m_nWorkers;
    int        m_nMaxCount;
public:
    CMutexWorkers(int maxCount = 1) :
        m_semWorkers(NULL),
        m_nWorkers(0),
        m_nMaxCount(maxCount)
    {
        // initialize the Workers variables
        m_semWorkers = ::CreateSemaphore(NULL, m_nMaxCount, m_nMaxCount, TEXT("Global\\ForkTestAppS"));

        if (m_semWorkers == NULL)
        {
            ErrorExit(TEXT("CreateSemaphore"));
        }
    };

    virtual ~CMutexWorkers()
    {
        if (m_semWorkers)
        {
            VERIFY(::CloseHandle(m_semWorkers));
        }
        m_semWorkers = NULL;
    }

    void Check() {
        DWORD dwEvent = WAIT_TIMEOUT;

        dwEvent = ::WaitForSingleObject(m_semWorkers, INFINITE);
        ASSERT(dwEvent == WAIT_OBJECT_0);

        m_nWorkers++;
    };

    void Release() {
        LONG prevCount = 0;
        char msg[80];

        VERIFY(::ReleaseSemaphore(m_semWorkers, 1, &prevCount));
        snprintf(msg, sizeof(msg), "Semaphore count before release: %ld\n", prevCount);
        OutputDebugStringA(msg);
    };
};

static int child_main(int argc, TCHAR* argv[])
{
    CHAR chBuf[BUFSIZE];
    DWORD dwRead, dwWritten;
    HANDLE hStdin, hStdout;
    BOOL bSuccess;

    OutputDebugStringA("child_main @ 386\n");

    // signal monitor (parent) that we've started...
    //HANDLE childEventHandle = OpenEventW(STANDARD_RIGHTS_ALL, FALSE, childEventName);
    HANDLE childEventHandle = CreateEvent(NULL, FALSE, FALSE, childEventName);
    if (childEventHandle == NULL)
    {
        printError(TEXT("CreateEvent"));
        OutputDebugStringA("child_main @ CreateEvent FAIL\n");
    }
    else
    {
        OutputDebugStringA("child_main @ SetEvent\n");
        VERIFY(SetEvent(childEventHandle));
        OutputDebugStringA("child_main @ CloseEvent\n");
        VERIFY(CloseHandle(childEventHandle));
    }

    OutputDebugStringA("child_main @ 403\n");

    hStdout = GetStdHandle(STD_OUTPUT_HANDLE);
    hStdin = GetStdHandle(STD_INPUT_HANDLE);
    if (
        (hStdout == INVALID_HANDLE_VALUE) ||
        (hStdin == INVALID_HANDLE_VALUE)
        )
    {
        printf("\n ** Failed to obtain stdin + stdout handles. ** \n");
        exit(1);
    }
    else
    {
        OutputDebugStringA("child_main @ 417\n");
        //MessageBox(NULL, TEXT("Child Exec!"), TEXT("Info"), MB_OK);

        OutputDebugStringA("child_main @ 420\n");
        // Send something to this process's stdout using printf.
        printf("\n ** This is a message from the child process. ** \n");
        int i;
        for (i = 0; i < argc; i++)
        {
            printf("\n ** ARGV[%d] = \"%S\"\n", i, argv[i]);
        }

        // This simple algorithm uses the existence of the pipes to control execution.
        // It relies on the pipe buffers to ensure that no data is lost.
        // Larger applications would use more advanced process control.

        OutputDebugStringA("child_main @ 432\n");
        for (;;)
        {
            // Read from standard input and stop on error or no data.
            bSuccess = ReadFile(hStdin, chBuf, BUFSIZE, &dwRead, NULL);

            OutputDebugStringA("child_main @ 439 after ReadFile\n");
            if (!bSuccess || dwRead == 0)
                break;

            OutputDebugStringA("child_main @ 443 before WriteFile\n");
            // Write to standard output and stop on error.
            bSuccess = WriteFile(hStdout, chBuf, dwRead, &dwWritten, NULL);
            OutputDebugStringA("child_main @ 446 after WriteFile\n");

            if (!bSuccess)
                break;
        }
        OutputDebugStringA("child_main @ 451 End Of Loop\n");
        //exit(0);
        return EXIT_SUCCESS;
    }
}

//static CMutexWorkers appnumber = CMutexWorkers(2);

static void terminate_on_unhandled_cpp_exception() 
{
    OutputDebugStringA("terminate_on_unhandled_cpp_exception was called by terminate.\n");
    exit(2);
}

static BOOL WINAPI TerminationHandlerRoutine(DWORD dwCtrlType)
{
    OutputDebugStringA("TerminationHandlerRoutine\n");
    return FALSE;
}




static BOOL ListProcessModules(DWORD dwPID)
{
    HANDLE hModuleSnap = INVALID_HANDLE_VALUE;
    MODULEENTRY32 me32;

    // Take a snapshot of all modules in the specified process.
    hModuleSnap = CreateToolhelp32Snapshot(TH32CS_SNAPMODULE, dwPID);
    if (hModuleSnap == INVALID_HANDLE_VALUE)
    {
        printError(TEXT("CreateToolhelp32Snapshot (of modules)"));
        return(FALSE);
    }

    // Set the size of the structure before using it.
    me32.dwSize = sizeof(MODULEENTRY32);

    // Retrieve information about the first module,
    // and exit if unsuccessful
    if (!Module32First(hModuleSnap, &me32))
    {
        printError(TEXT("Module32First"));  // show cause of failure
        CloseHandle(hModuleSnap);           // clean the snapshot object
        return(FALSE);
    }

    // Now walk the module list of the process,
    // and display information about each module
    do
    {
        _tprintf(TEXT("\n\n     MODULE NAME:     %s"), me32.szModule);
        _tprintf(TEXT("\n     Executable     = %s"), me32.szExePath);
        _tprintf(TEXT("\n     Process ID     = 0x%08X"), me32.th32ProcessID);
        _tprintf(TEXT("\n     Ref count (g)  = 0x%04X"), me32.GlblcntUsage);
        _tprintf(TEXT("\n     Ref count (p)  = 0x%04X"), me32.ProccntUsage);
        _tprintf(TEXT("\n     Base address   = 0x%08p"), me32.modBaseAddr);
        _tprintf(TEXT("\n     Base size      = %d"), me32.modBaseSize);

    } while (Module32Next(hModuleSnap, &me32));

    CloseHandle(hModuleSnap);
    return(TRUE);
}

static BOOL ListProcessThreads(DWORD dwOwnerPID)
{
    HANDLE hThreadSnap = INVALID_HANDLE_VALUE;
    THREADENTRY32 te32;

    // Take a snapshot of all running threads  
    hThreadSnap = CreateToolhelp32Snapshot(TH32CS_SNAPTHREAD, 0);
    if (hThreadSnap == INVALID_HANDLE_VALUE)
        return(FALSE);

    // Fill in the size of the structure before using it. 
    te32.dwSize = sizeof(THREADENTRY32);

    // Retrieve information about the first thread,
    // and exit if unsuccessful
    if (!Thread32First(hThreadSnap, &te32))
    {
        printError(TEXT("Thread32First")); // show cause of failure
        CloseHandle(hThreadSnap);          // clean the snapshot object
        return(FALSE);
    }

    // Now walk the thread list of the system,
    // and display information about each thread
    // associated with the specified process
    do
    {
        if (te32.th32OwnerProcessID == dwOwnerPID)
        {
            _tprintf(TEXT("\n\n     THREAD ID      = 0x%08X"), te32.th32ThreadID);
            _tprintf(TEXT("\n     Base priority  = %d"), te32.tpBasePri);
            _tprintf(TEXT("\n     Delta priority = %d"), te32.tpDeltaPri);
            _tprintf(TEXT("\n"));
        }
    } while (Thread32Next(hThreadSnap, &te32));

    CloseHandle(hThreadSnap);
    return(TRUE);
}

static BOOL GetProcessList()
{
    HANDLE hProcessSnap;
    HANDLE hProcess;
    PROCESSENTRY32 pe32;
    DWORD dwPriorityClass;

    // Take a snapshot of all processes in the system.
    hProcessSnap = CreateToolhelp32Snapshot(TH32CS_SNAPPROCESS, 0);
    if (hProcessSnap == INVALID_HANDLE_VALUE)
    {
        printError(TEXT("CreateToolhelp32Snapshot (of processes)"));
        return(FALSE);
    }

    // Set the size of the structure before using it.
    pe32.dwSize = sizeof(PROCESSENTRY32);

    // Retrieve information about the first process,
    // and exit if unsuccessful
    if (!Process32First(hProcessSnap, &pe32))
    {
        printError(TEXT("Process32First")); // show cause of failure
        CloseHandle(hProcessSnap);          // clean the snapshot object
        return(FALSE);
    }

    // Now walk the snapshot of processes, and
    // display information about each process in turn
    do
    {
        _tprintf(TEXT("\n\n====================================================="));
        _tprintf(TEXT("\nPROCESS NAME:  %s"), pe32.szExeFile);
        _tprintf(TEXT("\n-------------------------------------------------------"));

        // Retrieve the priority class.
        dwPriorityClass = 0;
        hProcess = OpenProcess(PROCESS_ALL_ACCESS, FALSE, pe32.th32ProcessID);
        if (hProcess == NULL)
        {
            if (GetLastError() == ERROR_INVALID_PARAMETER && pe32.th32ProcessID == 0)
            {
                _tprintf(TEXT("\n  Process           = System Idle"));
            }
            else
            {
                printError(TEXT("OpenProcess"));
            }
        }
        else
        {
            dwPriorityClass = GetPriorityClass(hProcess);
            if (!dwPriorityClass)
                printError(TEXT("GetPriorityClass"));
            CloseHandle(hProcess);
        }

        _tprintf(TEXT("\n  Process ID        = 0x%08X"), pe32.th32ProcessID);
        _tprintf(TEXT("\n  Thread count      = %d"), pe32.cntThreads);
        _tprintf(TEXT("\n  Parent process ID = 0x%08X"), pe32.th32ParentProcessID);
        _tprintf(TEXT("\n  Priority base     = %d"), pe32.pcPriClassBase);
        if (dwPriorityClass)
            _tprintf(TEXT("\n  Priority class    = %d"), dwPriorityClass);

        // List the modules and threads associated with this process
        ListProcessModules(pe32.th32ProcessID);
#if false
        ListProcessThreads(pe32.th32ProcessID);
#endif

    } while (Process32Next(hProcessSnap, &pe32));

    CloseHandle(hProcessSnap);
    return(TRUE);
}

static int CountNumberOfSameProcess(LPCWSTR exePath)
{
    HANDLE hProcessSnap;
    PROCESSENTRY32 pe32;
    int count = 0;

    // Take a snapshot of all processes in the system.
    hProcessSnap = CreateToolhelp32Snapshot(TH32CS_SNAPPROCESS, 0);
    if (hProcessSnap == INVALID_HANDLE_VALUE)
    {
        printError(TEXT("CreateToolhelp32Snapshot (of processes)"));
        return -1;
    }

    // Set the size of the structure before using it.
    pe32.dwSize = sizeof(PROCESSENTRY32);

    // Retrieve information about the first process,
    // and exit if unsuccessful
    if (!Process32First(hProcessSnap, &pe32))
    {
        printError(TEXT("Process32First")); // show cause of failure
        CloseHandle(hProcessSnap);          // clean the snapshot object
        return -1;
    }

    // Now walk the snapshot of processes, and
    // display information about each process in turn
    do
    {
        DWORD dwPID = pe32.th32ProcessID;
        HANDLE hModuleSnap = INVALID_HANDLE_VALUE;
        MODULEENTRY32 me32;

        // Take a snapshot of all modules in the specified process.
        hModuleSnap = CreateToolhelp32Snapshot(TH32CS_SNAPMODULE, dwPID);
        if (hModuleSnap == INVALID_HANDLE_VALUE)
        {
            {
                wchar_t msg[2000];
                wsprintf(msg, TEXT("CreateToolhelp32Snapshot::INVALID_HANDLE_VALUE for entry [%s] handle %d, parent %d\n"), pe32.szExeFile, (int)pe32.th32ProcessID, (int)pe32.th32ParentProcessID);
                OutputDebugStringW(msg);
            }

            printError(TEXT("CreateToolhelp32Snapshot (of modules)"));
            //return -1;  -- ignore
        }
        else
        {
            {
                wchar_t msg[2000];
                wsprintf(msg, TEXT("CreateToolhelp32Snapshot::scanModules for entry [%s] handle %d, parent %d\n"), pe32.szExeFile, (int)pe32.th32ProcessID, (int)pe32.th32ParentProcessID);
                OutputDebugStringW(msg);
            }

            // ignore HANDLE=0,PARTENT=0: that's us ourselves and we will be listed *again* later in the snapshot.
            // If we didn't ignore/skip this entry, we'ld be counting ourselves *twice*.
            if (pe32.th32ProcessID == 0 && pe32.th32ParentProcessID == 0)
            {
                OutputDebugStringW(TEXT("SKIP the 'self' entry.\n"));
            }
            else
            {
                // Set the size of the structure before using it.
                me32.dwSize = sizeof(MODULEENTRY32);

                // Retrieve information about the first module,
                // and exit if unsuccessful
                if (!Module32First(hModuleSnap, &me32))
                {
                    printError(TEXT("Module32First"));  // show cause of failure
                    CloseHandle(hModuleSnap);           // clean the snapshot object
                    return -1;
                }
                else
                {
                    if (wcscmp(exePath, me32.szExePath) == 0)
                    {
                        ListProcessModules(pe32.th32ProcessID);
                        count++;
                    }
                    {
                        wchar_t msg[2000];
                        wsprintf(msg, TEXT("comparing: %d <-- %s <--> %s\n"), count, exePath, me32.szExePath);
                        OutputDebugStringW(msg);
                    }
                }
                CloseHandle(hModuleSnap);
            }
        }
    } while (Process32Next(hProcessSnap, &pe32));

    CloseHandle(hProcessSnap);
    {
        wchar_t msg[2000];
        wsprintf(msg, TEXT("INSTANCE COUNT: %d\n"), count);
        OutputDebugStringW(msg);
    }
    return count;
}

// PSAPI
#if false

// To ensure correct resolution of symbols, add Psapi.lib to TARGETLIBS
// and compile with -DPSAPI_VERSION=1
static void PrintProcessNameAndID(DWORD processID)
{
    WCHAR szProcessName[MAX_PATH] = TEXT("<unknown>");

    // Get a handle to the process.
    HANDLE hProcess = OpenProcess(PROCESS_QUERY_INFORMATION | PROCESS_VM_READ, FALSE, processID);

    // Get the process name.
    if (NULL != hProcess)
    {
        HMODULE hMod;
        DWORD cbNeeded;

        if (EnumProcessModules(hProcess, &hMod, sizeof(hMod), &cbNeeded))
        {
            VERIFY(GetModuleBaseName(hProcess, hMod, szProcessName, sizeof(szProcessName) / sizeof(WCHAR)));
        }
    }

    // Print the process name and identifier.
    _tprintf(TEXT("%s  (PID: %u)\n"), szProcessName, processID);

    // Release the handle to the process.
    CloseHandle(hProcess);
}

static int PSGetProcessList()
{
    // Get the list of process identifiers.
    DWORD aProcesses[1024], cbNeeded, cProcesses;
    unsigned int i;

    if (!EnumProcesses(aProcesses, sizeof(aProcesses), &cbNeeded))
    {
        return 1;
    }

    // Calculate how many process identifiers were returned.
    cProcesses = cbNeeded / sizeof(DWORD);

    // Print the name and process identifier for each process.
    for (i = 0; i < cProcesses; i++)
    {
        if (aProcesses[i] != 0)
        {
            PrintProcessNameAndID(aProcesses[i]);
        }
    }

    return 0;
}

#endif


static int CPP_main(int argc, TCHAR* argv[])
{
    try
    {
        OutputDebugString(TEXT("CPP_main start...\n"));

        // Despite some differing noise on SO, this callback will NOT be invoked for regular console apps
        // as this API will already return FALSE: failure to set up callback.
        //
        // https://docs.microsoft.com/en-us/windows/win32/api/processthreadsapi/nf-processthreadsapi-setprocessshutdownparameters?redirectedfrom=MSDN#remarks
        //VERIFY(SetConsoleCtrlHandler(TerminationHandlerRoutine, TRUE));

        set_terminate(terminate_on_unhandled_cpp_exception);

        //appnumber.Check();

        {
            WCHAR szProcessName[MAX_PATH] = TEXT("<unknown>");

#if false
            // this produces some UNC path that does not match the executable paths from the scan. Alas.
            if (GetProcessImageFileName(GetCurrentProcess(), szProcessName, sizeof(szProcessName) / sizeof(WCHAR)))
            {
                _tprintf(TEXT("\nBASE NAME:  %s"), szProcessName);
            }
#endif

#if false
            ListProcessModules(GetCurrentProcessId());
#endif

            // get the name of the current binary. Do it in such a way that it will match the executable names 
            // in the system-wide process scan *exactly*:
            {
                DWORD dwPID = GetCurrentProcessId();
                HANDLE hModuleSnap = INVALID_HANDLE_VALUE;
                MODULEENTRY32 me32;

                // Take a snapshot of all modules in the specified process.
                hModuleSnap = CreateToolhelp32Snapshot(TH32CS_SNAPMODULE, dwPID);
                if (hModuleSnap == INVALID_HANDLE_VALUE)
                {
                    printError(TEXT("CreateToolhelp32Snapshot (of modules)"));
                    return EXIT_FAILURE;
                }

                // Set the size of the structure before using it.
                me32.dwSize = sizeof(MODULEENTRY32);

                // Retrieve information about the first module,
                // and exit if unsuccessful
                if (!Module32First(hModuleSnap, &me32))
                {
                    printError(TEXT("Module32First"));  // show cause of failure
                    CloseHandle(hModuleSnap);           // clean the snapshot object
                    return EXIT_FAILURE;
                }

                {
                    wchar_t msg[2000];
                    wsprintf(msg, TEXT("getting exe path: %s\n"), me32.szExePath);
                    OutputDebugStringW(msg);
                }
                wcscpy_s(szProcessName, sizeof(szProcessName) / sizeof(WCHAR), me32.szExePath);

                VERIFY(CloseHandle(hModuleSnap));
            }



            OutputDebugString(TEXT("CPP_main create mutex...\n"));

            //mutexSecurity.AddAccessRule(new MutexAccessRule(new SecurityIdentifier(WellKnownSidType.WorldSid, null),
            //    MutexRights.Synchronize | MutexRights.Modify, AccessControlType.Allow));
            HANDLE m = CreateMutex(NULL /* not inheriable by child */, FALSE, mutexName);
            if (m == NULL)
            {
                ErrorExit(TEXT("CreateMutexEx::Global\\ForkTestAppM"));
            }

            DWORD s = WaitForSingleObject(m, INFINITE);
            if (s == WAIT_FAILED)
            {
                //CloseHandle(m);
                //m = OpenMutexW(SYNCHRONIZE, FALSE, TEXT("Global\\ForkTestAppM"));
                //if (m == NULL)
                //{
                //    ErrorExit(TEXT("OpenMutexW"));
                //}
                ErrorExit(TEXT("WaitForSingleObject on MUTEX Global\\ForkTestAppM"));
            }
            if (s != WAIT_OBJECT_0)
            {
                ErrorExit(TEXT("WaitForSingleObject on MUTEX Global\\ForkTestAppM is not signaled"));
            }
            // inside the 'critical section' protected by the mutex now...

            OutputDebugString(TEXT("CPP_main inside mutex...\n"));

            // https://stackoverflow.com/questions/865152/how-can-i-get-a-process-handle-by-its-name-in-c
            // https://docs.microsoft.com/en-us/windows/win32/toolhelp/taking-a-snapshot-and-viewing-processes
            // https://docs.microsoft.com/en-us/windows/win32/psapi/enumerating-all-processes?redirectedfrom=MSDN
#if false
            GetProcessList();
#endif

#if false
            PSGetProcessList();
#endif

            int cnt = CountNumberOfSameProcess(szProcessName);

            OutputDebugString(TEXT("CPP_main counting done inside mutex...\n"));

            SetLastError(0);

            // now release the mutex: we have our count!
            // 
            // For some weird reason the ReleaseMutex() call returns ZERO, flagging an erro, but not setting any.
            // We *did* a Wait above, so it shouldn't fail, right?
            VERIFY(ReleaseMutex(m));

            OutputDebugString(TEXT("CPP_main past mutex...\n"));

            // and close the handle, so everything is cleaned up immediately
            VERIFY(CloseHandle(m));

            {
                wchar_t msg[200];
                wsprintf(msg, TEXT("\n  NOTE: EXE COUNT is %d\n"), cnt);
                OutputDebugStringW(msg);
            }
            _tprintf(TEXT("\n  NOTE: EXE COUNT is %d\n"), cnt);
            if (cnt > 4)
            {
                ErrorExit(TEXT("Way too many instances alive. Aborting this one!"));
                ExitProcess(2);
            }
        }

        OutputDebugString(TEXT("CPP_main before argv check...\n"));
        {
            wchar_t msg[500];
            wsprintf(msg, TEXT("ARGV[1]: [%s]\n"), argv[1]);
            OutputDebugStringW(msg);
        }

        if (argc >= 2 && lstrcmp(argv[1], szChildArg) == 0)
        {
            OutputDebugString(TEXT("CPP_main -> child...\n"));

            return child_main(argc, argv);
        }
        if (argc >= 2 && lstrcmp(argv[1], szParentArg) == 0)
        {
            OutputDebugString(TEXT("CPP_main -> parent...\n"));

            SECURITY_ATTRIBUTES saAttr;

            printf("\n->Start of parent execution.\n");

            // Set the bInheritHandle flag so pipe handles are inherited. 

            saAttr.nLength = sizeof(SECURITY_ATTRIBUTES);
            saAttr.bInheritHandle = TRUE;
            saAttr.lpSecurityDescriptor = NULL;

            // Create a pipe for the child process's STDOUT. 

            if (!CreatePipe(&g_hChildStd_OUT_Rd, &g_hChildStd_OUT_Wr, &saAttr, 0))
                ErrorExit(TEXT("StdoutRd CreatePipe"));

            // Ensure the read handle to the pipe for STDOUT is not inherited.

            if (!SetHandleInformation(g_hChildStd_OUT_Rd, HANDLE_FLAG_INHERIT, 0))
                ErrorExit(TEXT("Stdout SetHandleInformation"));

            // Create a pipe for the child process's STDIN. 

            if (!CreatePipe(&g_hChildStd_IN_Rd, &g_hChildStd_IN_Wr, &saAttr, 0))
                ErrorExit(TEXT("Stdin CreatePipe"));

            // Ensure the write handle to the pipe for STDIN is not inherited. 

            if (!SetHandleInformation(g_hChildStd_IN_Wr, HANDLE_FLAG_INHERIT, 0))
                ErrorExit(TEXT("Stdin SetHandleInformation"));

            // Create the child process. 

            CreateChildProcess();

            // Get a handle to an input file for the parent. 
            // This example assumes a plain text file and uses string output to verify data flow. 

            if (argc == 1)
                ErrorExit(TEXT("Please specify an input file.\n"));

            g_hInputFile = CreateFile(
                argv[2] ? argv[2] : TEXT("README.md"),
                GENERIC_READ,
                0,
                NULL,
                OPEN_EXISTING,
                FILE_ATTRIBUTE_READONLY,
                NULL);

            if (g_hInputFile == INVALID_HANDLE_VALUE)
            {
                ErrorExit(TEXT("CreateFile"));
            }
            else
            {
                // Write to the pipe that is the standard input for a child process. 
                // Data is written to the pipe's buffers, so it is not necessary to wait
                // until the child process is running before writing data.

                WriteToPipe();
                printf("\n->Contents of %S written to child STDIN pipe.\n", argv[1]);

                // Read from pipe that is the standard output for child process. 

                printf("\n->Contents of child process STDOUT:\n\n");
                //ReadFromPipe();  <-- this is already done in another thread!

                printf("\n->End of parent execution.\n");

                // The remaining open handles are cleaned up when this process terminates. 
                // To avoid resource leaks in a larger application, close handles explicitly. 
            }
        }
        else
        {
            OutputDebugString(TEXT("CPP_main -> not a child, not a parent?!?!...\n"));

            printf("\nNot a parent, nor a child...\n");
        }
        return 0;
    }
    catch (...)
    {
        OutputDebugStringA("CPP_main::catch caught a C++ application exception.\n");
        exit(2);
    }
}

static int SEH_main(int argc, TCHAR* argv[])
{
    int rv = 0;
    __try
    {
        rv = CPP_main(argc, argv);
    }
    __finally
    {
        BOOL ab = AbnormalTermination();

        if (ab)
        {
            OutputDebugStringA("Abnormal termination!\n");
            rv = 3;
        }
        //appnumber.Release();
    }
    return rv;
}

static void atexit_semaphore_handler()
{
    OutputDebugStringA("atexit_semaphore_handler\n");

    //appnumber.Release();
}

int _tmain(int argc, TCHAR* argv[])
{
    OutputDebugString(TEXT("start exec...\n"));
    OutputDebugStringW(GetCommandLine());

    atexit(atexit_semaphore_handler);

    // error C2712: Cannot use __try in functions that require object unwinding
    // hence we wrap that stuff for we need __try/__finally for the counting Semaphore work
    // to always work properly, even when the application instance is aborted!
    return SEH_main(argc, argv);
}

#endif
