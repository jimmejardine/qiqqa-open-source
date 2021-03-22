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



// hack: unused attributes bit used by us to signal hardlinks are present
#define FILE_ATTRIBUTE_HAS_MULTIPLE_SITES  0x80000000U



//
// Globals
//
ULONG FilesMatched = 0;
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
// Parse Mask
//
//--------------------------------------------------------------------
DWORD ParseMask(WCHAR* mask)
{
    DWORD attrs = 0;
    int negate = 0;

    if (!mask)
    {
        wprintf(L"Missing attributes mask parameter value.\n");
        exit(1);
    }

    while (*mask)
    {
        switch (*mask)
        {
        case '!':
        case '~':
            negate = 1;
            break;

        case 'R':
            attrs |= FILE_ATTRIBUTE_READONLY;
            break;

        case 'H':
            attrs |= FILE_ATTRIBUTE_HIDDEN;
            break;

        case 'S':
            attrs |= FILE_ATTRIBUTE_SYSTEM;
            break;

        case 'D':
            attrs |= FILE_ATTRIBUTE_DIRECTORY;
            break;

        case 'A':
            attrs |= FILE_ATTRIBUTE_ARCHIVE;
            break;

        case 'd':
            attrs |= FILE_ATTRIBUTE_DEVICE;
            break;

        case 'N':
            attrs |= FILE_ATTRIBUTE_NORMAL;
            break;

        case 'T':
            attrs |= FILE_ATTRIBUTE_TEMPORARY;
            break;

        case 's':
            attrs |= FILE_ATTRIBUTE_SPARSE_FILE;
            break;

        case 'h':
            attrs |= FILE_ATTRIBUTE_REPARSE_POINT;
            break;

        case 'C':
            attrs |= FILE_ATTRIBUTE_COMPRESSED;
            break;

        case 'O':
            attrs |= FILE_ATTRIBUTE_OFFLINE;
            break;

        case 'i':
            attrs |= FILE_ATTRIBUTE_NOT_CONTENT_INDEXED;
            break;

        case 'E':
            attrs |= FILE_ATTRIBUTE_ENCRYPTED;
            break;

        case 't':
            attrs |= FILE_ATTRIBUTE_INTEGRITY_STREAM;
            break;

        case 'V':
            attrs |= FILE_ATTRIBUTE_VIRTUAL;
            break;

        case 'b':
            attrs |= FILE_ATTRIBUTE_NO_SCRUB_DATA;
            break;

        case 'a':
            attrs |= FILE_ATTRIBUTE_EA;
            break;

        case 'P':
            attrs |= FILE_ATTRIBUTE_PINNED;
            break;

        case 'u':
            attrs |= FILE_ATTRIBUTE_UNPINNED;
            break;

        case 'c':
            attrs |= FILE_ATTRIBUTE_RECALL_ON_DATA_ACCESS;
            break;

        case 'o':
            attrs |= FILE_ATTRIBUTE_RECALL_ON_OPEN;
            break;

        case 'l':
            attrs |= FILE_ATTRIBUTE_STRICTLY_SEQUENTIAL;
            break;

        case 'L':
            attrs |= FILE_ATTRIBUTE_HAS_MULTIPLE_SITES;
            break;

        case 'M':
        case '?':
            attrs |= ~(0
                | FILE_ATTRIBUTE_READONLY
                | FILE_ATTRIBUTE_HIDDEN
                | FILE_ATTRIBUTE_SYSTEM
                | FILE_ATTRIBUTE_DIRECTORY
                | FILE_ATTRIBUTE_ARCHIVE
                | FILE_ATTRIBUTE_DEVICE
                | FILE_ATTRIBUTE_NORMAL
                | FILE_ATTRIBUTE_TEMPORARY
                | FILE_ATTRIBUTE_SPARSE_FILE
                | FILE_ATTRIBUTE_REPARSE_POINT
                | FILE_ATTRIBUTE_COMPRESSED
                | FILE_ATTRIBUTE_OFFLINE
                | FILE_ATTRIBUTE_NOT_CONTENT_INDEXED
                | FILE_ATTRIBUTE_ENCRYPTED
                | FILE_ATTRIBUTE_INTEGRITY_STREAM
                | FILE_ATTRIBUTE_VIRTUAL
                | FILE_ATTRIBUTE_NO_SCRUB_DATA
                | FILE_ATTRIBUTE_EA
                | FILE_ATTRIBUTE_PINNED
                | FILE_ATTRIBUTE_UNPINNED
                | FILE_ATTRIBUTE_RECALL_ON_DATA_ACCESS
                | FILE_ATTRIBUTE_RECALL_ON_OPEN
                | FILE_ATTRIBUTE_STRICTLY_SEQUENTIAL
                | FILE_ATTRIBUTE_HAS_MULTIPLE_SITES
                );
            break;

        default:
            wprintf(L"\rError reading attributes mask: unknown attribute %C.\n", *mask);
            exit(1);
        }
        mask++;
    }

    if (negate)
        attrs = ~attrs;

    return attrs;
}



// Return TRUE when file is hardlinked at least once, i.e. has two paths on the disk AT LEAST.
BOOL FileHasMultipleInstances(WCHAR* FileName)
{
    WCHAR linkPath[MAX_PATH];
    int linkCount = 0;
    DWORD slen = nelem(linkPath);
    HANDLE fnameHandle = FindFirstFileNameW(FileName, 0, &slen, linkPath);
    if (fnameHandle != INVALID_HANDLE_VALUE)
    {
        slen = nelem(linkPath);
        if (FindNextFileNameW(fnameHandle, &slen, linkPath))
        {
            linkCount++;
        }
        FindClose(fnameHandle);
    }
    return !!linkCount;
}


//--------------------------------------------------------------------
//
// ProcessFile
//
// Queries a file to obtain stream information.
//
//--------------------------------------------------------------------
VOID ProcessFile(WCHAR* FileName, BOOLEAN IsDirectory, DWORD mandatoryAttribs, DWORD wantedAnyAttribs, DWORD rejectedAttribs, BOOLEAN showLinks)
{
    DWORD attrs = GetFileAttributes(FileName);

    if (attrs == INVALID_FILE_ATTRIBUTES)
    {
        wprintf(L"\rError reading attributes of %s:\n", FileName);
        PrintWin32Error(GetLastError());
    }
    else
    {
        // First do the easy filter stuff. 
        // Then, after we've done that, we go in and scan the filesystem to see if the file has hardlinks
        // and we filter on THAT.

        int mandatoryLinks = !!(mandatoryAttribs & FILE_ATTRIBUTE_HAS_MULTIPLE_SITES);
        int wantedLinks = !!(wantedAnyAttribs & FILE_ATTRIBUTE_HAS_MULTIPLE_SITES);
        int rejectedLinks = !!(rejectedAttribs & FILE_ATTRIBUTE_HAS_MULTIPLE_SITES);

        mandatoryAttribs &= ~FILE_ATTRIBUTE_HAS_MULTIPLE_SITES;
        wantedAnyAttribs &= ~FILE_ATTRIBUTE_HAS_MULTIPLE_SITES;
        rejectedAttribs &= ~FILE_ATTRIBUTE_HAS_MULTIPLE_SITES;

        if ((attrs & mandatoryAttribs) != mandatoryAttribs)
            return;
        if (wantedAnyAttribs && (attrs & wantedAnyAttribs) == 0)
            return;
        if (attrs & rejectedAttribs)
            return;

        BOOL hasLinks = FileHasMultipleInstances(FileName);
        if (mandatoryLinks && !hasLinks)
            return;
        // when 'has links' is the only thing we *want*, it's kinda mandatory, eh:
        if (wantedLinks && !wantedAnyAttribs && !hasLinks)
            return;
        if (rejectedLinks && hasLinks)
            return;

        FilesMatched++;

        WCHAR    attr_str[32];

        for (int i = 0; i < 32; i++)
            attr_str[i] = '.';

        if (attrs & FILE_ATTRIBUTE_READONLY)
        {
            attr_str[0] = 'R';
            attrs &= ~FILE_ATTRIBUTE_READONLY;
        }
        if (attrs & FILE_ATTRIBUTE_HIDDEN)
        {
            attr_str[1] = 'H';
            attrs &= ~FILE_ATTRIBUTE_HIDDEN;
        }
        if (attrs & FILE_ATTRIBUTE_SYSTEM)
        {
            attr_str[2] = 'S';
            attrs &= ~FILE_ATTRIBUTE_SYSTEM;
        }
        if (attrs & FILE_ATTRIBUTE_DIRECTORY)
        {
            attr_str[3] = 'D';
            attrs &= ~FILE_ATTRIBUTE_DIRECTORY;
        }
        if (attrs & FILE_ATTRIBUTE_ARCHIVE)
        {
            attr_str[4] = 'A';
            attrs &= ~FILE_ATTRIBUTE_ARCHIVE;
        }
        if (attrs & FILE_ATTRIBUTE_DEVICE)
        {
            attr_str[5] = 'd';
            attrs &= ~FILE_ATTRIBUTE_DEVICE;
        }
        if (attrs & FILE_ATTRIBUTE_NORMAL)
        {
            attr_str[6] = 'N';
            attrs &= ~FILE_ATTRIBUTE_NORMAL;
        }
        if (attrs & FILE_ATTRIBUTE_TEMPORARY)
        {
            attr_str[7] = 'T';
            attrs &= ~FILE_ATTRIBUTE_TEMPORARY;
        }
        if (attrs & FILE_ATTRIBUTE_SPARSE_FILE)
        {
            attr_str[8] = 's';
            attrs &= ~FILE_ATTRIBUTE_SPARSE_FILE;
        }
        if (attrs & FILE_ATTRIBUTE_REPARSE_POINT)
        {
            attr_str[9] = 'h';
            attrs &= ~FILE_ATTRIBUTE_REPARSE_POINT;
        }
        if (attrs & FILE_ATTRIBUTE_COMPRESSED)
        {
            attr_str[10] = 'C';
            attrs &= ~FILE_ATTRIBUTE_COMPRESSED;
        }
        if (attrs & FILE_ATTRIBUTE_OFFLINE)
        {
            attr_str[11] = 'O';
            attrs &= ~FILE_ATTRIBUTE_OFFLINE;
        }
        if (attrs & FILE_ATTRIBUTE_NOT_CONTENT_INDEXED)
        {
            attr_str[12] = 'i';
            attrs &= ~FILE_ATTRIBUTE_NOT_CONTENT_INDEXED;
        }
        if (attrs & FILE_ATTRIBUTE_ENCRYPTED)
        {
            attr_str[13] = 'E';
            attrs &= ~FILE_ATTRIBUTE_ENCRYPTED;
        }
        if (attrs & FILE_ATTRIBUTE_INTEGRITY_STREAM)
        {
            attr_str[14] = 't';
            attrs &= ~FILE_ATTRIBUTE_INTEGRITY_STREAM;
        }
        if (attrs & FILE_ATTRIBUTE_VIRTUAL)
        {
            attr_str[15] = 'V';
            attrs &= ~FILE_ATTRIBUTE_VIRTUAL;
        }
        if (attrs & FILE_ATTRIBUTE_NO_SCRUB_DATA)
        {
            attr_str[16] = 'b';
            attrs &= ~FILE_ATTRIBUTE_NO_SCRUB_DATA;
        }
        if (attrs & FILE_ATTRIBUTE_EA)
        {
            attr_str[17] = 'a';
            attrs &= ~FILE_ATTRIBUTE_EA;
        }
        if (attrs & FILE_ATTRIBUTE_PINNED)
        {
            attr_str[18] = 'P';
            attrs &= ~FILE_ATTRIBUTE_PINNED;
        }
        if (attrs & FILE_ATTRIBUTE_UNPINNED)
        {
            attr_str[19] = 'u';
            attrs &= ~FILE_ATTRIBUTE_UNPINNED;
        }
        if (attrs & FILE_ATTRIBUTE_RECALL_ON_DATA_ACCESS)
        {
            attr_str[20] = 'c';
            attrs &= ~FILE_ATTRIBUTE_RECALL_ON_DATA_ACCESS;
        }
        if (attrs & FILE_ATTRIBUTE_RECALL_ON_OPEN)
        {
            attr_str[21] = 'o';
            attrs &= ~FILE_ATTRIBUTE_RECALL_ON_OPEN;
        }
        if (attrs & FILE_ATTRIBUTE_STRICTLY_SEQUENTIAL)
        {
            attr_str[22] = 'l';
            attrs &= ~FILE_ATTRIBUTE_STRICTLY_SEQUENTIAL;
        }
        if (hasLinks)
        {
            attr_str[22] = 'L';
        }
        if (attrs)
        {
            attr_str[23] = '?';
            attrs &= ~FILE_ATTRIBUTE_STRICTLY_SEQUENTIAL;
        }
        attr_str[24] = 0;

        wprintf(L"\r%s %s\n", attr_str, FileName + 4 /* skip \\?\ prefix */ );
    }

    if (showLinks)
    {
        WCHAR linkPath[MAX_PATH];
        int linkCount = 0;
        DWORD slen = nelem(linkPath);
        HANDLE fnameHandle = FindFirstFileNameW(FileName, 0, &slen, linkPath);
        if (fnameHandle == INVALID_HANDLE_VALUE)
        {
            wprintf(L"\rError reading link names for %s:\n", FileName);
            PrintWin32Error(GetLastError());
        }
        else
        {
            if (wcscmp(linkPath, FileName + 6 /* skip \\?\X: long filename prefix plus drive part as that is not present in the link path */))
            {
                wprintf(L"\r#--Link: %2.2s%s\n", FileName + 4, linkPath);
            }
            linkCount++;

            slen = nelem(linkPath);
            while (FindNextFileNameW(fnameHandle, &slen, linkPath))
            {
                if (wcscmp(linkPath, FileName + 6 /* skip \\?\X: long filename prefix plus drive part as that is not present in the link path */))
                {
                    wprintf(L"\r#--Link: %2.2s%s\n", FileName + 4, linkPath);
                }
                slen = nelem(linkPath);
                linkCount++;
            }
            // EVERY file has ONE "hardlink" at least. UNIX-like "hardlinked files" have MULTIPLE sites:
            if (linkCount > 1)
            {
                wprintf(L"\rNumber of sites: %d\n", linkCount);
            }

            if (GetLastError() != ERROR_HANDLE_EOF)
            {
                wprintf(L"\rError reading link names for %s:\n", FileName);
                PrintWin32Error(GetLastError());
            }
            FindClose(fnameHandle);
        }
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
    BOOLEAN Recurse, DWORD mandatoryAttribs, DWORD wantedAnyAttribs, DWORD rejectedAttribs, BOOLEAN showLinks)
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
            // check if the specified path is a file or directory:
            int is_dir = FALSE;

            if ((patternHandle = FindFirstFile(PathName, &foundFile)) != INVALID_HANDLE_VALUE)
            {
                is_dir = !!(foundFile.dwFileAttributes & FILE_ATTRIBUTE_DIRECTORY);
                FindClose(patternHandle);
            }

            if (is_dir)
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
            else
            {
                WCHAR *dirEnd = wcsrchr(PathName, '\\');
                if (!dirEnd)
                    dirEnd = wcsrchr(PathName, '/');
                if (!dirEnd)
                    dirEnd = wcsrchr(PathName, ':');
                WCHAR* basename;
                if (dirEnd)
                    basename = dirEnd + 1;
                else
                {
                    dirEnd = PathName;
                    basename = PathName;
                }

                swprintf(SearchPattern, SearchPatternSize, basename);

                if (Recurse)
                {
                    swprintf(searchName, nelem(searchName), L"%.*s\\*.*", (int)(dirEnd - PathName), PathName);
                    swprintf(fileSearchName, nelem(fileSearchName), L"%s", PathName);
                }
                else
                {
                    swprintf(searchName, nelem(searchName), L"%s", PathName);
                    swprintf(fileSearchName, nelem(fileSearchName), L"%s", PathName);
                }
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
            if (wcscmp(foundFile.cFileName, L".") &&
                wcscmp(foundFile.cFileName, L".."))
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
                    mandatoryAttribs, wantedAnyAttribs, rejectedAttribs, showLinks
                );
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
                ProcessDirectory(subName, SearchPattern, SearchPatternSize, Recurse, mandatoryAttribs, wantedAnyAttribs, rejectedAttribs, showLinks);
            }
        } while (FindNextFile(dirHandle, &foundFile));
    }

    FindClose(dirHandle);
}


int Usage(WCHAR* ProgramName)
{
    WCHAR* baseName = wcsrchr(ProgramName, '\\');
    if (!baseName)
        baseName = wcsrchr(ProgramName, '//');
    if (!baseName)
        baseName = ProgramName;
    else
        baseName++;
    wprintf(L"usage: %s [-s] [-m mask] [-r mask] <file or directory>\n", baseName);
    wprintf(L"-s     Recurse subdirectories\n");
    wprintf(L"-m     mask of attributes which are Mandatory (MUST HAVE)\n");
    wprintf(L"-w     mask of attributes which are Wanted (MAY HAVE)\n");
    wprintf(L"-r     mask of attributes which are Rejected (HAS NOT)\n\n");
    wprintf(L"-l     list all hardlink sites for every file which has multiple sites (hardlinks)\n");
    wprintf(L"\n");
    wprintf(L"The M,W,R masks are processed as follows:\n"
            L"  mask & MUST(Mandatory) == MUST\n"
            L"  mask & MAY(Wanted) != 0          (if '-w' was specified)\n"
            L"  mask & NOT(Rejected) == 0\n"
            L"only files which pass all three checks will be listed.\n\n");
    wprintf(L"Mask/Attributes:\n");
    wprintf(L"       R : READONLY\n");
    wprintf(L"       H : HIDDEN\n");
    wprintf(L"       S : SYSTEM\n");
    wprintf(L"       D : DIRECTORY\n");
    wprintf(L"       A : ARCHIVE\n");
    wprintf(L"       d : DEVICE\n");
    wprintf(L"       N : NORMAL\n");
    wprintf(L"       T : TEMPORARY\n");
    wprintf(L"       s : SPARSE_FILE\n");
    wprintf(L"       h : REPARSE_POINT\n");
    wprintf(L"       C : COMPRESSED\n");
    wprintf(L"       O : OFFLINE\n");
    wprintf(L"       i : NOT_CONTENT_INDEXED\n");
    wprintf(L"       E : ENCRYPTED\n");
    wprintf(L"       t : INTEGRITY_STREAM\n");
    wprintf(L"       V : VIRTUAL\n");
    wprintf(L"       b : NO_SCRUB_DATA\n");
    wprintf(L"       a : EA\n");
    wprintf(L"       P : PINNED\n");
    wprintf(L"       u : UNPINNED\n");
    wprintf(L"       c : RECALL_ON_DATA_ACCESS\n");
    wprintf(L"       o : RECALL_ON_OPEN\n");
    wprintf(L"       l : STRICTLY_SEQUENTIAL\n");
    wprintf(L"       L : MULTIPLE_SITES (i.e. file has hardlinks on the drive)\n");
    wprintf(L"       M : misc. (unknown)\n");
    wprintf(L"       ? : misc. (unknown)\n");
    wprintf(L"       ~ : *NEGATE* the entire specified mask\n");
    wprintf(L"       ! : *NEGATE* the entire specified mask\n");
    wprintf(L"\n");

    return -1;
}


int wmain(int argc, WCHAR* argv[])
{
    BOOLEAN     recurse = FALSE;
    BOOLEAN     regular_only = FALSE;
    PWCHAR      filePart;
    WCHAR		volume[] = L"C:\\";
    DWORD		fsFlags;
    BOOLEAN     showLinks = FALSE;
    DWORD       mandatoryAttribs = 0;
    DWORD       wantedAnyAttribs = 0;
    DWORD       rejectedAttribs = 0;
    WCHAR       searchPattern[MAX_PATH];
    WCHAR		searchPath[MAX_PATH];
    int         i;

    //
    // Print banner and perform parameter check
    //
    wprintf(L"\nNTFSlinks v1.0 - Enumerate NTFS hardlinks\n");
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
            else if (argv[i][1] == L'l' || argv[i][1] == L'L')
            {
                showLinks = TRUE;
            }
            else if (argv[i][1] == L'm' || argv[i][1] == 'M')
            {
                i++;
                mandatoryAttribs = ParseMask(argv[i]);
            }
            else if (argv[i][1] == L'w' || argv[i][1] == 'W')
            {
                i++;
                wantedAnyAttribs = ParseMask(argv[i]);
            }
            else if (argv[i][1] == L'r' || argv[i][1] == 'R')
            {
                i++;
                rejectedAttribs = ParseMask(argv[i]);
            }
            else
            {
                wprintf(L"Unrecognized commandline argument: %s\n\n", argv[i]);
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
    // https://docs.microsoft.com/en-us/windows/win32/fileio/maximum-file-path-limitation
    wcsncpy_s(searchPath, L"\\\\?\\", 4);
    GetFullPathName(argv[argc - 1], MAX_PATH - 4, searchPath + 4, &filePart);

    //
    // Make sure that it's a NTFS volume
    //
    if (searchPath[1] == L':')
    {
        fsFlags = 0;
        volume[0] = searchPath[0];
        GetVolumeInformation(volume, NULL, 0, NULL, NULL, &fsFlags, NULL, 0);
        if (!(fsFlags & FILE_SUPPORTS_HARD_LINKS))
        {
            wprintf(L"\nThe specified volume does not support hardlinks.\n\n");
            exit(2);
        }
    }

    //
    // Now go and process directories
    //
    ProcessDirectory(searchPath, searchPattern, nelem(searchPattern), recurse, mandatoryAttribs, wantedAnyAttribs, rejectedAttribs, showLinks);

    if (!FilesMatched)
        wprintf(L"No matching files found.\n\n");
    return 0;
}
