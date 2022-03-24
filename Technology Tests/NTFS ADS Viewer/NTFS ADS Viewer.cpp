#define UNICODE 1
#include <windows.h>
#include <stdio.h>
#include <string.h>
#include <assert.h>

#include "ntfs_ads_io.h"

// Make sure we support Long Paths:
#undef MAX_PATH
#define MAX_PATH 1500

#define ASSERT(t)                       \
    if (!(t))                           \
    {                                   \
        fprintf(stderr, "assertion failed: %s\n", #t); \
        exit(666);                      \
    }      

static NtQueryInformationFile_f NtQueryInformationFile;
static RtlNtStatusToDosError_f RtlNtStatusToDosError;


//
// Globals
//
ULONG FilesWithStreams = 0;
ULONG FilesProcessed = 0;
ULONG DotsPrinted = 0;
BOOLEAN PrintDirectoryOpenErrors = FALSE;

//----------------------------------------------------------------------
//
// PrintNtError
//
// Formats an error message for the last native error.
//
//----------------------------------------------------------------------
void PrintNtError(NTSTATUS status)
{
    WCHAR* errMsg;

    FormatMessage(FORMAT_MESSAGE_ALLOCATE_BUFFER | FORMAT_MESSAGE_FROM_SYSTEM,
        NULL, RtlNtStatusToDosError(status),
        MAKELANGID(LANG_NEUTRAL, SUBLANG_DEFAULT),
        (LPTSTR)&errMsg, 0, NULL);
    wprintf(L"%s\n", errMsg);
    LocalFree(errMsg);
}

//--------------------------------------------------------------------
//
// PrintWin32Error
// 
// Translates a Win32 error into a text equivalent
//
//--------------------------------------------------------------------
void PrintWin32Error(DWORD ErrorCode)
{
    LPTSTR lpMsgBuf;

    FormatMessage(FORMAT_MESSAGE_ALLOCATE_BUFFER | FORMAT_MESSAGE_FROM_SYSTEM,
        NULL, ErrorCode,
        MAKELANGID(LANG_NEUTRAL, SUBLANG_DEFAULT),
        (LPTSTR)&lpMsgBuf, 0, NULL);
    wprintf(L"%s\n", lpMsgBuf);
    LocalFree(lpMsgBuf);
}

//----------------------------------------------------------------------
//
// EnableTokenPrivilege
//
// Enables the load driver privilege
//
//----------------------------------------------------------------------
BOOL EnableTokenPrivilege(LPCTSTR PrivilegeName)
{
    TOKEN_PRIVILEGES tp;
    LUID luid;
    HANDLE	hToken;
    TOKEN_PRIVILEGES tpPrevious;
    DWORD cbPrevious = sizeof(TOKEN_PRIVILEGES);

    //
    // Get debug privilege
    //
    if (!OpenProcessToken(GetCurrentProcess(),
        TOKEN_ADJUST_PRIVILEGES | TOKEN_QUERY,
        &hToken))
    {
        return FALSE;
    }

    if (!LookupPrivilegeValue(NULL, PrivilegeName, &luid))
        return FALSE;

    //
    // first pass.  get current privilege setting
    //
    tp.PrivilegeCount = 1;
    tp.Privileges[0].Luid = luid;
    tp.Privileges[0].Attributes = 0;

    AdjustTokenPrivileges(
        hToken,
        FALSE,
        &tp,
        sizeof(TOKEN_PRIVILEGES),
        &tpPrevious,
        &cbPrevious
    );

    if (GetLastError() != ERROR_SUCCESS)
        return FALSE;

    //
    // second pass.  set privilege based on previous setting
    //
    tpPrevious.PrivilegeCount = 1;
    tpPrevious.Privileges[0].Luid = luid;
    tpPrevious.Privileges[0].Attributes |= (SE_PRIVILEGE_ENABLED);
    AdjustTokenPrivileges(
        hToken,
        FALSE,
        &tpPrevious,
        cbPrevious,
        NULL,
        NULL
    );

    return GetLastError() == ERROR_SUCCESS;
}


//--------------------------------------------------------------------
//
// ProcessFile
//
// Queries a file to obtain stream information.
//
//--------------------------------------------------------------------
VOID ProcessFile(WCHAR* FileName, BOOLEAN IsDirectory, BOOLEAN Delete)
{
    PFILE_STREAM_INFORMATION streamInfo;
    PFILE_STREAM_INFORMATION streamInfoPtr;
    ULONG    streamInfoSize = 0;
    BOOLEAN  printedFile = FALSE;
    NTSTATUS status;
    HANDLE   fileHandle;
    WCHAR    streamName[MAX_PATH];
    WCHAR    fullStreamName[MAX_PATH];
    IO_STATUS_BLOCK ioStatus = { 0 };

    //
    // Open the file
    //
    fileHandle = CreateFile(FileName, GENERIC_READ,
        FILE_SHARE_READ | FILE_SHARE_WRITE, NULL,
        OPEN_EXISTING,
        FILE_FLAG_BACKUP_SEMANTICS, 0);
    if (fileHandle == INVALID_HANDLE_VALUE)
    {
        if (!IsDirectory || PrintDirectoryOpenErrors)
        {
            wprintf(L"\rError opening %s:\n", FileName);
            PrintWin32Error(GetLastError());
        }
        return;
    }

    if (!(++FilesProcessed % 500))
    {
        if (DotsPrinted == 3)
        {
            wprintf(L"\r     \r");
            DotsPrinted = 0;
        }
        else
        {
            DotsPrinted++;
            wprintf(L".");
        }
        fflush(stdout);
    }

    streamInfoSize = 16384;
    streamInfo = (PFILE_STREAM_INFORMATION)calloc(1, streamInfoSize);
    status = STATUS_BUFFER_OVERFLOW;
    while (status == STATUS_BUFFER_OVERFLOW)
    {
        memset(&ioStatus, 0, sizeof(ioStatus));
        status = NtQueryInformationFile(fileHandle, &ioStatus,
            streamInfo, streamInfoSize,
            FileStreamInformation);
        if (status == STATUS_BUFFER_OVERFLOW)
        {
            free(streamInfo);
            streamInfoSize += 16384;
            streamInfo = (PFILE_STREAM_INFORMATION)calloc(1, streamInfoSize);
        }
        else
        {
            break;
        }
    }

    //
    // If success, dump the contents
    //
    if (NT_SUCCESS(status) && ioStatus.Information)
    {
        streamInfoPtr = streamInfo;
        while (1)
        {
            memcpy(streamName,
                streamInfoPtr->StreamName,
                streamInfoPtr->StreamNameLength);
            streamName[streamInfoPtr->StreamNameLength / 2] = 0;

            //
            // Skip the standard Data stream
            //
            if (_wcsicmp(streamName, L"::$DATA"))
            {
                if (!printedFile)
                {
                    wprintf(L"\r%s:\n", FileName);
                    printedFile = TRUE;
                }

                if (Delete)
                {
                    swprintf(fullStreamName, nelem(fullStreamName), L"%s%s", FileName, streamName);
                    if (!DeleteFile(fullStreamName))
                    {
                        wprintf(L"   Error deleting %s:\n", streamName);
                        PrintWin32Error(GetLastError());
                    }
                    else
                    {
                        wprintf(L"   Deleted %s\n", streamName);
                    }
                }
                else
                {
                    wprintf(L"   %20s\t%I64d\n", streamName, streamInfoPtr->StreamSize.QuadPart);

                    swprintf(fullStreamName, nelem(fullStreamName), L"%s%s", FileName, streamName);
                    HANDLE adsFileHandle = CreateFile(fullStreamName, GENERIC_READ,
                            FILE_SHARE_READ | FILE_SHARE_WRITE, NULL,
                            OPEN_EXISTING,
                            FILE_FLAG_BACKUP_SEMANTICS, 0);
                    if (adsFileHandle == INVALID_HANDLE_VALUE)
                    {
                        wprintf(L"\rError opening %s:\n", fullStreamName);
                        PrintWin32Error(GetLastError());
                        return;
                    }

                    size_t content_len = streamInfoPtr->StreamSize.QuadPart;
                    BYTE* content = (BYTE *)malloc(content_len + 8);
                    DWORD actual_len = 0;

                    BOOL rv = ReadFile(adsFileHandle, content, (DWORD)content_len + 4, &actual_len, NULL);
                    if (!rv)
                    {
                        wprintf(L"   Error reading %s:\n", fullStreamName);
                        PrintWin32Error(GetLastError());
                        free(content);
                        content = NULL;
                    }

                    CloseHandle(adsFileHandle);

                    if (content && actual_len > 0)
                    {
                        if (actual_len != content_len)
                        {
                            wprintf(L"   Warning: actual length %zu != expected length %zu for %s.\n", (size_t)actual_len, content_len, fullStreamName);
                        }
                        memset(content + actual_len, 0, 4);

                        // check if content is plain text or binary: use some simple heuristics for that:
                        bool is_plain_text = true;
                        for (size_t i = 0; i < actual_len; i++)
                        {
                            int c = content[i];
                            if (c < 32 && c != '\r' && c != '\n' && c != '\t')
                            {
                                is_plain_text = false;
                                break;
                            }
                            if (c == 127)
                            {
                                is_plain_text = false;
                                break;
                            }
                        }

                        if (is_plain_text)
                        {
                            // trim the trailing newlines:
                            BYTE* end = content + content_len - 1;
                            for (; end >= content; end--)
                            {
                                if (*end == '\r' || *end == '\n')
                                    *end = 0;
                                else
                                    break;
                            }

                            wprintf(L"   text content:\n------------------------------------------\n%S\n------------------------------------------\n", content);
                        }
                        else
                        {
                            for (size_t i = 0; i < actual_len; i += 16)
                            {
                                wprintf(L"   %08zu: \n", i);
                                for (size_t j = 0; j < 16; j++)
                                {
                                    if (j + i < actual_len)
                                        wprintf(L"%02x ", content[i + j]);
                                    else
                                        wprintf(L"   ");
                                }
                                wprintf(L" | ");
                                for (size_t j = 0; j < 16; j++)
                                {
                                    if (j + i < actual_len)
                                    {
                                        int c = content[i + j];
                                        if (isprint(c))
                                            wprintf(L"%c ", c);
                                        else
                                            wprintf(L". ");
                                    }
                                    else
                                        wprintf(L"  ");
                                }
                                wprintf(L"\n");
                            }
                        }
                    }
                }
            }

            if (!streamInfoPtr->NextEntryOffset)
                break;

            FilesWithStreams++;
            streamInfoPtr = (PFILE_STREAM_INFORMATION)((char*)streamInfoPtr + streamInfoPtr->NextEntryOffset);
        }
    }
    else if (!NT_SUCCESS(status))
    {
        wprintf(L"\rError on %s: ", FileName);
        PrintNtError(status);
    }
    free(streamInfo);
    CloseHandle(fileHandle);
}



//--------------------------------------------------------------------
//
// ProcessDirectory
// 
// Recursive routine that passes files to the stream analyzing 
// function.
//
//--------------------------------------------------------------------
void ProcessDirectory(WCHAR* PathName, WCHAR* SearchPattern, size_t SearchPatternSize,
    BOOLEAN Recurse, BOOLEAN Delete)
{
    WCHAR			subName[MAX_PATH];
    WCHAR			fileSearchName[MAX_PATH];
    WCHAR			searchName[MAX_PATH];
    HANDLE			dirHandle = INVALID_HANDLE_VALUE;
    HANDLE			patternHandle;
    static BOOLEAN	firstCall = TRUE;
    WIN32_FIND_DATA foundFile;

    //
    // Scan the files and/or directories if this is a directory
    //
    if (firstCall)
    {
        if (PathName[wcslen(PathName) - 1] == L'\\')
        {
            PathName[wcslen(PathName) - 1] = 0;
        }

        if (wcsrchr(PathName, '*'))
        {
            LPTSTR fns = wcsrchr(PathName, '\\');
            if (fns)
            {
                swprintf(SearchPattern, SearchPatternSize, fns + 1);
                wcscpy_s(searchName, PathName);
                LPTSTR last = wcsrchr(searchName, '\\');
                ASSERT(last != NULL);
                wcscpy_s(last + 1, nelem(searchName) - (last + 1 - searchName), L"*.*");
            }
            else
            {
                swprintf(SearchPattern, SearchPatternSize, PathName);
                wcscpy_s(searchName, PathName);
            }
            swprintf(fileSearchName, nelem(fileSearchName), L"%s", PathName);
        }
        else
        {
            swprintf(SearchPattern, SearchPatternSize, L"*.*");
            if (Recurse)
            {
                swprintf(searchName, nelem(searchName), L"%s\\*.*", PathName);
                swprintf(fileSearchName, nelem(fileSearchName), L"%s\\*.*", PathName);
            }
            else
            {
                swprintf(searchName, nelem(searchName), L"%s", PathName);
                swprintf(fileSearchName, nelem(fileSearchName), L"%s", PathName);
            }
        }
    }
    else
    {
        swprintf(searchName, nelem(searchName), L"%s\\*.*", PathName);
        swprintf(fileSearchName, nelem(fileSearchName), L"%s\\%s", PathName, SearchPattern);
    }

    //
    // Process all the files, according to the search pattern
    //
    if ((patternHandle = FindFirstFile(fileSearchName, &foundFile)) != INVALID_HANDLE_VALUE)
    {
        do
        {
            if (firstCall || (wcscmp(foundFile.cFileName, L".") &&
                wcscmp(foundFile.cFileName, L"..")))
            {
                wcscpy_s(subName, searchName);
                LPTSTR fn = wcsrchr(subName, '\\');
                if (fn)
                    wcscpy_s(fn + 1, nelem(subName) - (fn + 1 - subName), foundFile.cFileName);
                else
                    wcscpy_s(subName, foundFile.cFileName);

                //
                // Do this file/directory
                //
                ProcessFile(subName,
                    (BOOLEAN)(foundFile.dwFileAttributes & FILE_ATTRIBUTE_DIRECTORY),
                    Delete);
            }
        } while (FindNextFile(patternHandle, &foundFile));
        FindClose(patternHandle);
    }

    //
    // Now recurse if we're supposed to
    //
    if (Recurse)
    {
        if (firstCall && !wcsrchr(searchName, L'\\'))
        {
            if (wcsrchr(searchName, L'*'))
            {
                if ((dirHandle = FindFirstFile(L"*.*", &foundFile)) == INVALID_HANDLE_VALUE)
                {
                    //
                    // Nothing to process
                    //
                    return;
                }
            }
            else
            {
                if ((dirHandle = FindFirstFile(searchName, &foundFile)) == INVALID_HANDLE_VALUE)
                {
                    //
                    // Nothing to process
                    //
                    return;
                }
            }
        }
        else
        {
            if ((dirHandle = FindFirstFile(searchName, &foundFile)) == INVALID_HANDLE_VALUE)
            {
                //
                // Nothing to process
                //
                return;
            }
        }
        firstCall = FALSE;

        do
        {
            if ((foundFile.dwFileAttributes & FILE_ATTRIBUTE_DIRECTORY) &&
                wcscmp(foundFile.cFileName, L".") &&
                wcscmp(foundFile.cFileName, L".."))
            {
                wcscpy_s(subName, searchName);
                LPTSTR fns = wcsrchr(subName, '\\');
                if (fns)
                    wcscpy_s(fns + 1, nelem(subName) - (fns + 1 - subName), foundFile.cFileName);
                else
                    wcscpy_s(subName, foundFile.cFileName);

                //
                // Go into this directory
                //
                ProcessDirectory(subName, SearchPattern, SearchPatternSize, Recurse, Delete);
            }
        } while (FindNextFile(dirHandle, &foundFile));
    }

    FindClose(dirHandle);
}


int Usage(WCHAR* ProgramName)
{
    wprintf(L"usage: %s [-s] [-d] <file or directory>\n", ProgramName);
    wprintf(L"-s     Recurse subdirectories\n");
    wprintf(L"-d     Delete streams\n\n");
    return -1;
}


int wmain(int argc, WCHAR* argv[])
{
    BOOLEAN     recurse = FALSE;
    BOOLEAN     _delete = FALSE;
    PWCHAR      filePart;
    WCHAR		volume[] = L"C:\\";
    DWORD		fsFlags;
    WCHAR       searchPattern[MAX_PATH];
    WCHAR		searchPath[MAX_PATH];
    int         i;

    //
    // Print banner and perform parameter check
    //
    wprintf(L"\nStreams v1.53 - Enumerate alternate NTFS data streams\n");
    wprintf(L"Copyright (C) 1999-2005 Mark Russinovich\n");
    wprintf(L"Sysinternals - www.sysinternals.com\n\n");

    if (argc > 1)
    {
        for (i = 1; i < argc; i++)
        {
            if (argv[i][0] != L'/' && argv[i][0] != L'-')
            {
                if (i != argc - 1)
                {
                    wprintf(argv[i]);
                    return Usage(argv[0]);
                }
                continue;
            }

            if (argv[i][1] == L's' || argv[i][1] == L'S')
            {
                recurse = TRUE;
            }
            else if (argv[i][1] == L'd' || argv[i][1] == 'D')
            {
                _delete = TRUE;
            }
            else
            {
                return Usage(argv[0]);
            }
        }
    }
    else
    {
        return Usage(argv[0]);
    }

    //
    // Enable backup privilege if we can
    //
    if (EnableTokenPrivilege(SE_BACKUP_NAME))
    {
        PrintDirectoryOpenErrors = TRUE;
    }

    //
    // Load the NTDLL entry point we need
    //
    if (!(NtQueryInformationFile = (NtQueryInformationFile_f)GetProcAddress(GetModuleHandle(L"ntdll.dll"), "NtQueryInformationFile")))
    {
        wprintf(L"\nCould not find NtQueryInformationFile entry point in NTDLL.DLL\n");
        exit(1);
    }
    if (!(RtlNtStatusToDosError = (RtlNtStatusToDosError_f)GetProcAddress(GetModuleHandle(L"ntdll.dll"), "RtlNtStatusToDosError")))
    {
        wprintf(L"\nCould not find RtlNtStatusToDosError entry point in NTDLL.DLL\n");
        exit(1);
    }
    GetFullPathName(argv[argc - 1], MAX_PATH, searchPath, &filePart);

    //
    // Make sure that its a FAT volume
    //
    if (searchPath[1] == L':')
    {
        fsFlags = 0;
        volume[0] = searchPath[0];
        GetVolumeInformation(volume, NULL, 0, NULL, NULL, &fsFlags, NULL, 0);
        if (!(fsFlags & FILE_NAMED_STREAMS))
        {
            wprintf(L"\nThe specified volume does not support streams.\n\n");
            exit(2);
        }
    }

    //
    // Now go and process directories
    //
    ProcessDirectory(searchPath, searchPattern, nelem(searchPattern), recurse, _delete);

    if (!FilesWithStreams)
        wprintf(L"No files with streams found.\n\n");
    return 0;
}
