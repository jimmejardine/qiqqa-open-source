// monitor our memory usage and die when passing threshold.cpp : This file contains the 'main' function. Program execution begins and ends there.
//


// use the portability work of cUrl: faster than writing our own header files...
#include "curl_setup.h"

#include <stdio.h>
#include <stdlib.h>
#include <sys/types.h>


#if !defined(WIN32) && !defined(WIN64)

int main(void) {

    return EXIT_SUCCESS;
}

#else

#if !defined(UNICODE)
#error "Compile in UNICODE mode!"
#endif

#define PSAPI_VERSION 2

#include <tlhelp32.h>
#include <psapi.h>
#include <tchar.h>
#include <strsafe.h>
#include <eh.h>
#include <stdarg.h>

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


// Format a readable error message, display a message box,
// and exit from the application.
void ErrorExit(const wchar_t * lpszFunction)
{
    LPTSTR lpMsgBuf;
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

void printError(const wchar_t * msg)
{
    DWORD eNum;
    WCHAR sysMsg[256];

    eNum = GetLastError();
    FormatMessage(FORMAT_MESSAGE_FROM_SYSTEM | FORMAT_MESSAGE_IGNORE_INSERTS,
        NULL, eNum,
        MAKELANGID(LANG_NEUTRAL, SUBLANG_DEFAULT), // Default language
        sysMsg, 256, NULL);

    // Display the message
    WCHAR m[2000];
    wsprintf(m, TEXT("WARNING: %s failed with error %d (%s)\n"), msg, eNum, sysMsg);
    OutputDebugStringW(m);

    _tprintf(TEXT("\n  WARNING: %s failed with error %d (%s)"), msg, eNum, sysMsg);
}

void debugPrint(const wchar_t* msg, ...)
{
    va_list argptr;
    va_start(argptr, msg);

    wchar_t buf[4000];
    _vsntprintf(buf, sizeof(buf) / sizeof(buf[0]), msg, argptr);
    va_end(argptr);

    OutputDebugStringW(buf);
    _tprintf(TEXT("%s"), buf);
}

static void showMemoryConsumption()
{
    HANDLE hProcess;
    PROCESS_MEMORY_COUNTERS_EX pmc = { sizeof(pmc), 0 };

    hProcess = GetCurrentProcess();

    if (GetProcessMemoryInfo(hProcess, (PPROCESS_MEMORY_COUNTERS)&pmc, sizeof(pmc)))
    {
        debugPrint(L"\tPageFaultCount:             %8zu\n", (SIZE_T)pmc.PageFaultCount);
        debugPrint(L"\tPeakWorkingSetSize:         %8zu\n", pmc.PeakWorkingSetSize);
        debugPrint(L"\tWorkingSetSize:             %8zu\n", pmc.WorkingSetSize);
        debugPrint(L"\tQuotaPeakPagedPoolUsage:    %8zu\n", pmc.QuotaPeakPagedPoolUsage);
        debugPrint(L"\tQuotaPagedPoolUsage:        %8zu\n", pmc.QuotaPagedPoolUsage);
        debugPrint(L"\tQuotaPeakNonPagedPoolUsage: %8zu\n", pmc.QuotaPeakNonPagedPoolUsage);
        debugPrint(L"\tQuotaPeakNonPagedPoolUsage: %8zu\n", pmc.QuotaNonPagedPoolUsage);
        debugPrint(L"\tPagefileUsage:              %8zu\n", pmc.PagefileUsage);
        debugPrint(L"\tPeakPagefileUsage:          %8zu\n", pmc.PeakPagefileUsage);
        debugPrint(L"\tPrivateUsage:               %8zu\n", pmc.PrivateUsage);
    }
    else
    {
        ErrorExit(L"GetProcessMemoryInfo");
    }

    MEMORYSTATUSEX statex = { sizeof(statex), 0 };

    if (GlobalMemoryStatusEx(&statex))
    {
        const int DIV = 1024;
        const float MDIV = DIV * DIV;
        const float GDIV = DIV * MDIV;
        debugPrint(L"There is  % 8d percent of memory in use.\n", (int)statex.dwMemoryLoad);
        debugPrint(L"There are %8.1f total MB of physical memory.\n", (SIZE_T)statex.ullTotalPhys / MDIV);
        debugPrint(L"There are %8.1f free  MB of physical memory.\n", (SIZE_T)statex.ullAvailPhys / MDIV);
        debugPrint(L"There are %8.1f total MB of paging file.\n", (SIZE_T)statex.ullTotalPageFile / MDIV);
        debugPrint(L"There are %8.1f free  MB of paging file.\n", (SIZE_T)statex.ullAvailPageFile / MDIV);
        debugPrint(L"There are %8.1f total GB of virtual memory.\n", statex.ullTotalVirtual / GDIV);
        debugPrint(L"There are %8.1f free  GB of virtual memory.\n", statex.ullAvailVirtual / GDIV);
        // Show the amount of extended memory available.
        debugPrint(L"There are %8.1f free  MB of extended memory.\n", (SIZE_T)statex.ullAvailExtendedVirtual / MDIV);
    }
    else
    {
        ErrorExit(L"GlobalMemoryStatusEx");
    }

    // perform some heuristics on the memory consumption to decide when to abort:
    if (statex.dwMemoryLoad > 90)
    {
        debugPrint(L"Aborting @ 90%% memory loading threshold.\n");
        ExitProcess(0);
    }
    if (pmc.WorkingSetSize >= 0.75 * statex.ullTotalPageFile)
    {
        debugPrint(L"Aborting @ 75%% of total page file consumed threshold.\n");
        ExitProcess(0);
    }
    if (pmc.WorkingSetSize >= 0.75 * statex.ullTotalPhys)
    {
        debugPrint(L"Aborting @ 75%% of total physical memory consumed threshold.\n");
        ExitProcess(0);
    }
    if (pmc.WorkingSetSize >= 0.75 * statex.ullTotalVirtual)
    {
        debugPrint(L"Aborting @ 75%% of total virtual memory space consumed threshold.\n");
        ExitProcess(0);
    }
}

static void growHeapAndWatch(int n)
{
    const int CHUNKSIZE = (int)25e6;
    void* p = malloc(CHUNKSIZE);
    ++n;
    debugPrint(L"+ chunk %d\n", n);
    showMemoryConsumption();
    if (!p)
    {
        debugPrint(L"# chunk alloc FAILED\n", n);
        ExitProcess(1);
    }

    // do a bit more alloc + free:
    if (n % 10 == 3)
    {
        void* m = malloc(4 * CHUNKSIZE);
        if (!m)
        {
            debugPrint(L"# special chunk alloc FAILED\n", n);
            ExitProcess(1);
        }
        free(p);
        free(m);
        debugPrint(L"+ special block %d\n", n);
        showMemoryConsumption();
    }

    // recurse to grow
    growHeapAndWatch(n);
}

int _tmain(int argc, TCHAR* argv[])
{
    debugPrint(TEXT("start exec...\n"));

    // https://docs.microsoft.com/en-us/windows/win32/psapi/collecting-memory-usage-information-for-a-process
    // https://docs.microsoft.com/en-us/previous-versions/windows/desktop/legacy/aa965225(v=vs.85)
    //
    showMemoryConsumption();

    growHeapAndWatch(0);
}

#endif

// Run program: Ctrl + F5 or Debug > Start Without Debugging menu
// Debug program: F5 or Debug > Start Debugging menu

// Tips for Getting Started:
//   1. Use the Solution Explorer window to add/manage files
//   2. Use the Team Explorer window to connect to source control
//   3. Use the Output window to see build output and other messages
//   4. Use the Error List window to view errors
//   5. Go to Project > Add New Item to create new code files, or Project > Add Existing Item to add existing code files to the project
//   6. In the future, to open this project again, go to File > Open > Project and select the .sln file
