#define UNICODE 1
#include <windows.h>
#include <stdio.h>
#include <inttypes.h>
#include <string.h>
#include <time.h>       /* clock_t, clock, CLOCKS_PER_SEC */
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



// hack: unused attributes bits used by us to signal hardlinks are present
#define FILE_ATTRIBUTE_HAS_MULTIPLE_SITES  0x80000000U
#define FILE_ATTRIBUTE_HARDLINK            0x40000000U


#define PRIME_MODULUS    16769023

typedef struct
{
    uint32_t hash;
    WCHAR* path;
} HashtableEntry;



//
// Globals
//
clock_t ticks;
FILE* output = NULL;
CPINFOEXW CPInfo = { 0 };
int cvtErrors = 0;
int conciseOutput = 0;
ULONG FilesMatched = 0;
ULONG DotsPrinted = 0;
BOOLEAN PrintDirectoryOpenErrors = FALSE;
HashtableEntry UniqueFilePaths[PRIME_MODULUS];
HashtableEntry OutputFilePaths[PRIME_MODULUS];

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
    fwprintf(stderr, L"\r%s\n", errMsg);
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
    fwprintf(stderr, L"\r%s\n", lpMsgBuf);
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
        fwprintf(stderr, L"Missing attributes mask parameter value.\n");
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

        case 'X':
            attrs |= FILE_ATTRIBUTE_HARDLINK;
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
                | FILE_ATTRIBUTE_HARDLINK
                );
            break;

        default:
            fwprintf(stderr, L"\rError reading attributes mask: unknown attribute %C.\n", *mask);
            exit(1);
        }
        mask++;
    }

    if (negate)
        attrs = ~attrs;

    return attrs;
}



// Produce a hash 1..PRIME (NOTE the 1-based number: this makes it easy and fast to detect empty (hash=0) slots!)
unsigned int CalculateHash(const WCHAR* str)
{
    uint64_t hash = 5381;

    while (*str)
    {
        uint64_t c = (*str++) & 0xFFFF;

        hash = ((hash << 5) + hash) + c; /* hash * 33 + c */
    }

    return (unsigned int)(hash % PRIME_MODULUS) + 1;
}



// Make sure all path separators are windows standard: '\'.
// Also reduce duplicate path separators, e.g. '//' and '///' into single ones: '\'
// Rewrites string IN-PLACE.
void NormalizePathSeparators(WCHAR* str)
{
    WCHAR* start = str;
    WCHAR* dst = str;
    while (*str)
    {
        WCHAR c = *str++;
        if (c == '/')
            c = '\\';
        // bunch consecutive '\' separators EXCEPT at the start, where we may have '\\?\'!
        if (c == '\\' && str - start >= 2)
        {
            WCHAR c2;
            do
            {
                c2 = *str++;
                if (c2 == '/')
                    c2 = '\\';
            } while (c2 == '\\');
            str--;
        }
        *dst++ = c;
    }
    *dst = 0;
}



void CvtUTF16ToUTF8(char* dst, size_t dstlen, const WCHAR* src)
{
    // https://docs.microsoft.com/en-us/windows/win32/api/stringapiset/nf-stringapiset-widechartomultibyte
    int len = WideCharToMultiByte(CP_UTF8, WC_ERR_INVALID_CHARS | WC_NO_BEST_FIT_CHARS,src, -1, NULL, 0, NULL, NULL);
    if (len == 0)
    {
        fwprintf(stderr, L"\rWARNING: Error while converting string to UTF8 for output to file. The data will be sanitized!\n    Offending string: \"%s\"\n", src);
        PrintWin32Error(GetLastError());
        len = WideCharToMultiByte(CP_UTF8, WC_NO_BEST_FIT_CHARS, src, -1, NULL, 0, NULL, NULL);
        cvtErrors++;
    }
    if (len >= (int)dstlen)
    {
        fwprintf(stderr, L"\rERROR: The UTF8 encoded string is too large: %d doesn't fit into the %zu byte buffer. The data will be truncated!\n    Offending string: \"%s\"\n", len, dstlen, src);
        cvtErrors++;
    }
    
    dst[0] = 0;
    int rv = WideCharToMultiByte(CP_UTF8, WC_NO_BEST_FIT_CHARS, src, -1, dst, dstlen, NULL, NULL);
    dst[dstlen - 1] = 0;
    if (rv == 0)
    {
        fwprintf(stderr, L"\rERROR: Error while converting string to UTF8 for output to file.\n    Offending string: \"%s\"\n", src);
        PrintWin32Error(GetLastError());
        exit(3);
    }
}



void CloseOutput(void)
{
    if (output && output != stdout)
    {
        // Dump the hash table content as UTF8 to file.
        // And clear the hash table too! It may be re-used in another round.
        for (int i = 0; i < PRIME_MODULUS; i++)
        {
            HashtableEntry *slot = &OutputFilePaths[i];
            if (!slot->hash)
                continue;
            ASSERT(slot->path != NULL);

            // write filename as UTF8 and check it for sanity while we do:
            char fname[MAX_PATH * 5];
            CvtUTF16ToUTF8(fname, sizeof(fname), slot->path);
            fprintf(output, "%s\n", fname);

            free(slot->path);
        }

        fclose(output);

        memset(OutputFilePaths, 0, sizeof(OutputFilePaths));
    }
    output = NULL;
}


int TestAndAddInHashtable(const WCHAR* str, HashtableEntry *UniqueFilePaths)
{
    unsigned int hash = CalculateHash(str);
    unsigned int idx = hash - 1;
    HashtableEntry* slot = &UniqueFilePaths[idx];

    while (slot->path)
    {
        if (!wcscmp(slot->path, str))
            return 1;                   // 1: exists already
        do {
            // jump and test next viable slot
            idx += 43;                  // mutual prime with PRIME_MODULUS
            idx = idx % PRIME_MODULUS;
            slot = &UniqueFilePaths[idx];
        } while (slot->hash && slot->hash != hash);
    }

    assert(!slot->hash);
    slot->hash = hash;
    slot->path = _wcsdup(str);
    return 0;                   // 0: not present before, ADDED now!
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



void ClearProgress(void)
{
    if (!conciseOutput)
    {
        fwprintf(stderr, L"\r     \r");
        DotsPrinted = 0;
    }
}


void ShowProgress(void)
{
    if (!conciseOutput)
    {
        clock_t t2 = clock();

        if (t2 - ticks >= CLOCKS_PER_SEC / 2)
        {
            ticks = t2;

            if (DotsPrinted == 3)
            {
                ClearProgress();
            }
            else
            {
                DotsPrinted++;
                fwprintf(stderr, L".");
            }
            fflush(stdout);
        }
    }
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
        fwprintf(stderr, L"\rError reading attributes of %s:\n", FileName);
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

        int mandatoryHardLink = !!(mandatoryAttribs & FILE_ATTRIBUTE_HARDLINK);
        int wantedHardLink = !!(wantedAnyAttribs & FILE_ATTRIBUTE_HARDLINK);
        int rejectedHardLink = !!(rejectedAttribs & FILE_ATTRIBUTE_HARDLINK);

        mandatoryAttribs &= ~(FILE_ATTRIBUTE_HAS_MULTIPLE_SITES | FILE_ATTRIBUTE_HARDLINK);
        wantedAnyAttribs &= ~(FILE_ATTRIBUTE_HAS_MULTIPLE_SITES | FILE_ATTRIBUTE_HARDLINK);
        rejectedAttribs &= ~(FILE_ATTRIBUTE_HAS_MULTIPLE_SITES | FILE_ATTRIBUTE_HARDLINK);

        if ((attrs & mandatoryAttribs) != mandatoryAttribs)
            return;
        if (wantedAnyAttribs && (attrs & wantedAnyAttribs) == 0)
            return;
        if (attrs & rejectedAttribs)
            return;

        BOOL hasLinks = FileHasMultipleInstances(FileName);

        if (mandatoryLinks && !hasLinks)
            return;
        // when 'has multiple sites' or 'is a hardlink' is the only thing we *want*, it's kinda mandatory, eh:
        if (wantedLinks && !wantedAnyAttribs && !hasLinks)
            return;
        if (wantedHardLink && !wantedAnyAttribs && !hasLinks)
            return;
        if (rejectedLinks && hasLinks)
            return;

        // register all links in a hash table, so next time we test, we'll hit 
        // one of those entries and declare that one a "hardlink", UNIX Style.
        int isHardlink = 0;
        if (hasLinks)
        {
            if (!TestAndAddInHashtable(FileName, UniqueFilePaths))
            {
                WCHAR linkPath[MAX_PATH];
                WCHAR fullPath[MAX_PATH];
                DWORD slen = nelem(linkPath);
                HANDLE fnameHandle = FindFirstFileNameW(FileName, 0, &slen, linkPath);
                if (fnameHandle != INVALID_HANDLE_VALUE)
                {
                    if (wcscmp(linkPath, FileName + 6 /* skip \\?\X: long filename prefix plus drive part as that is not present in the link path */))
                    {
                        wcsncpy_s(fullPath, FileName, 6);
                        wcsncat_s(fullPath, linkPath, nelem(fullPath));
                        TestAndAddInHashtable(fullPath, UniqueFilePaths);
                    }

                    slen = nelem(linkPath);
                    while (FindNextFileNameW(fnameHandle, &slen, linkPath))
                    {
                        if (wcscmp(linkPath, FileName + 6 /* skip \\?\X: long filename prefix plus drive part as that is not present in the link path */))
                        {
                            wcsncpy_s(fullPath, FileName, 6);
                            wcsncat_s(fullPath, linkPath, nelem(fullPath));
                            TestAndAddInHashtable(fullPath, UniqueFilePaths);
                        }
                        slen = nelem(linkPath);
                    }
                    FindClose(fnameHandle);
                }
            }
            else
            {
                isHardlink = 1;
            }
        }

        if (mandatoryHardLink && !isHardlink)
            return;
        // when 'is hardlink' is the only thing we *want*, it's kinda mandatory, eh:
        if (wantedHardLink && !wantedAnyAttribs && !wantedLinks && !isHardlink)
            return;
        if (rejectedHardLink && isHardlink)
            return;

        FilesMatched++;

        if (!conciseOutput)
        {
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
                attr_str[22] = isHardlink ? 'L' : '*';
            }
            if (attrs)
            {
                attr_str[23] = '?';
                attrs &= ~FILE_ATTRIBUTE_STRICTLY_SEQUENTIAL;
            }
            attr_str[24] = 0;

            fwprintf(stderr, L"\r");
            fwprintf(stdout, L"%s %s\n", attr_str, FileName + 4 /* skip \\?\ prefix */);
        }
        else if (!output)
        {
            // only dump the file paths to STDOUT in concise mode when NO output file has been specified.
            fwprintf(stderr, L"\r");
            fwprintf(stdout, L"%s\n", FileName + 4 /* skip \\?\ prefix */);
        }

        if (output)
        {
            // register filename in the OUTPUT hash table when we're going to output it 'unordered' to output file.
            TestAndAddInHashtable(FileName + 4 /* skip \\?\ prefix */, OutputFilePaths);
        }
    }

    if (showLinks)
    {
        WCHAR linkPath[MAX_PATH];
        int linkCount = 0;
        DWORD slen = nelem(linkPath);
        HANDLE fnameHandle = FindFirstFileNameW(FileName, 0, &slen, linkPath);
        if (fnameHandle == INVALID_HANDLE_VALUE)
        {
            fwprintf(stderr, L"\rError reading link names for %s:\n", FileName);
            PrintWin32Error(GetLastError());
        }
        else
        {
            if (wcscmp(linkPath, FileName + 6 /* skip \\?\X: long filename prefix plus drive part as that is not present in the link path */))
            {
                fwprintf(stdout, L"\r#--Link: %2.2s%s\n", FileName + 4, linkPath);
            }
            linkCount++;

            slen = nelem(linkPath);
            while (FindNextFileNameW(fnameHandle, &slen, linkPath))
            {
                if (wcscmp(linkPath, FileName + 6 /* skip \\?\X: long filename prefix plus drive part as that is not present in the link path */))
                {
                    fwprintf(stdout, L"\r#--Link: %2.2s%s\n", FileName + 4, linkPath);
                }
                slen = nelem(linkPath);
                linkCount++;
            }
            // EVERY file has ONE "hardlink" at least. UNIX-like "hardlinked files" have MULTIPLE sites:
            if (linkCount > 1)
            {
                fwprintf(stdout, L"\r#--Number of sites: %d\n", linkCount);
            }

            if (GetLastError() != ERROR_HANDLE_EOF)
            {
                fwprintf(stderr, L"\rError reading link names for %s:\n", FileName);
                PrintWin32Error(GetLastError());
            }
            FindClose(fnameHandle);
        }
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
    BOOLEAN	        firstCall = (SearchPattern[0] == 0);
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
                ShowProgress();

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
        ShowProgress();

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
                ShowProgress();

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

    fwprintf(stderr, L"\nDirScanner v1.1 - List directory contents including NTFS hardlinks\n");
    fwprintf(stderr, L"Copyright (C) 2021 Ger Hobbelt\n");
    fwprintf(stderr, L"Some parts Copyright (C) 1999-2005 Mark Russinovich\n");

    fwprintf(stderr, L"usage: %s [-s] [-m mask] [-r mask] [-w mask] [-o file] <file or directory> ...\n", baseName);
    fwprintf(stderr, L"-c     Concise output, i.e. do NOT print the attributes\n");
    fwprintf(stderr, L"-s     Recurse subdirectories\n");
    fwprintf(stderr, L"-m     mask of attributes which are Mandatory (MUST HAVE)\n");
    fwprintf(stderr, L"-w     mask of attributes which are Wanted (MAY HAVE)\n");
    fwprintf(stderr, L"-r     mask of attributes which are Rejected (HAS NOT)\n\n");
    fwprintf(stderr, L"-l     list all hardlink sites for every file which has multiple sites (hardlinks)\n");
    fwprintf(stderr, L"-o     write the collected list of paths to the specified file (SEMI-RANDOM HASH-based order)\n");
    fwprintf(stderr, L"\n");
    fwprintf(stderr, L"The M,W,R masks are processed as follows:\n"
            L"  mask & MUST(Mandatory) == MUST\n"
            L"  mask & MAY(Wanted) != 0          (if '-w' was specified)\n"
            L"  mask & NOT(Rejected) == 0\n"
            L"only files which pass all three checks will be listed.\n\n");
    fwprintf(stderr, L"Mask/Attributes:\n");
    fwprintf(stderr, L"       R : READONLY\n");
    fwprintf(stderr, L"       H : HIDDEN\n");
    fwprintf(stderr, L"       S : SYSTEM\n");
    fwprintf(stderr, L"       D : DIRECTORY\n");
    fwprintf(stderr, L"       A : ARCHIVE\n");
    fwprintf(stderr, L"       d : DEVICE\n");
    fwprintf(stderr, L"       N : NORMAL\n");
    fwprintf(stderr, L"       T : TEMPORARY\n");
    fwprintf(stderr, L"       s : SPARSE_FILE\n");
    fwprintf(stderr, L"       h : REPARSE_POINT\n");
    fwprintf(stderr, L"       C : COMPRESSED\n");
    fwprintf(stderr, L"       O : OFFLINE\n");
    fwprintf(stderr, L"       i : NOT_CONTENT_INDEXED\n");
    fwprintf(stderr, L"       E : ENCRYPTED\n");
    fwprintf(stderr, L"       t : INTEGRITY_STREAM\n");
    fwprintf(stderr, L"       V : VIRTUAL\n");
    fwprintf(stderr, L"       b : NO_SCRUB_DATA\n");
    fwprintf(stderr, L"       a : EA\n");
    fwprintf(stderr, L"       P : PINNED\n");
    fwprintf(stderr, L"       u : UNPINNED\n");
    fwprintf(stderr, L"       c : RECALL_ON_DATA_ACCESS\n");
    fwprintf(stderr, L"       o : RECALL_ON_OPEN\n");
    fwprintf(stderr, L"       l : STRICTLY_SEQUENTIAL\n");
    fwprintf(stderr, L"       L : MULTIPLE_SITES (i.e. file has hardlinks on the drive)\n");
    fwprintf(stderr, L"       X : HARDLINK (i.e. file is a 'hardlink'.)\n");
    fwprintf(stderr, L"       M : misc. (unknown)\n");
    fwprintf(stderr, L"       ? : misc. (unknown)\n");
    fwprintf(stderr, L"       ~ : *NEGATE* the entire specified mask\n");
    fwprintf(stderr, L"       ! : *NEGATE* the entire specified mask\n");
    fwprintf(stderr, L"\n");
    fwprintf(stderr, L"NOTE: we consider a file a 'hardlink' in the UNIX sense when it's the *second or later*\n");
    fwprintf(stderr, L"      file path we encounter for the given file during the scan.\n");
    fwprintf(stderr, L"      Hence you can filter with '/w ~X', i.e. not wanting to see hardlinks, to get a\n");
    fwprintf(stderr, L"      list of unique files in the given search path (there may be links to these files elsewhere).\n");
    fwprintf(stderr, L"      You can filter with '/w L' to see all file paths for 'hardlinked' files.\n");
    fwprintf(stderr, L"      You can filter with '/m L /r X' to see the *first occurrence* of each 'hardlinked' file\n");
    fwprintf(stderr, L"      in the given search path.\n");
    fwprintf(stderr, L"\n");

    return -1;
}


int wmain(int argc, WCHAR* argv[])
{
    BOOLEAN     recurse = FALSE;
    BOOLEAN     regular_only = FALSE;
    DWORD		fsFlags;
    BOOLEAN     showLinks = FALSE;
    DWORD       mandatoryAttribs = 0;
    DWORD       wantedAnyAttribs = 0;
    DWORD       rejectedAttribs = 0;
    WCHAR       searchPattern[MAX_PATH];
    WCHAR		searchPath[MAX_PATH];
    WCHAR		listOutputPath[MAX_PATH];
    int         i;

    ticks = clock();

    if (argc <= 1)
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
        fwprintf(stderr, L"\nCould not find NtQueryInformationFile entry point in NTDLL.DLL\n");
        exit(1);
    }
    if (!(RtlNtStatusToDosError = (RtlNtStatusToDosError_f)GetProcAddress(GetModuleHandle(L"ntdll.dll"), "RtlNtStatusToDosError")))
    {
        fwprintf(stderr, L"\nCould not find RtlNtStatusToDosError entry point in NTDLL.DLL\n");
        exit(1);
    }

    {
        if (!GetCPInfoExW(CP_UTF8, 0, &CPInfo))
        {
            memset(&CPInfo, 0, sizeof(CPInfo));
            CPInfo.DefaultChar[0] = '?';
        }
    }

    // Now go through the search paths sequentially, while we parse the commandline parameters alongside.
    // Order of appearancee is important, hence you can specify different attribute mask filters
    // for different search paths!

    if (argv[argc - 1][0] == L'/' || argv[argc - 1][0] == L'-')
    {
        fwprintf(stderr, L"Unused commandline parameters at the end of your commandline. Please clean up: %s\n", argv[argc - 1]);
        return Usage(argv[0]);
    }

    memset(UniqueFilePaths, 0, sizeof(UniqueFilePaths));
    memset(OutputFilePaths, 0, sizeof(OutputFilePaths));

    output = NULL;
    atexit(CloseOutput);

    for (i = 1; i < argc; i++)
    {
        if (argv[i][0] != L'/' && argv[i][0] != L'-')
        {
            // https://docs.microsoft.com/en-us/windows/win32/fileio/maximum-file-path-limitation
            wcsncpy_s(searchPath, L"\\\\?\\", 4);
            PWCHAR filePart;
            GetFullPathName(argv[argc - 1], MAX_PATH - 4, searchPath + 4, &filePart);
            NormalizePathSeparators(searchPath);

            //
            // Check that it's a NTFS volume and report limited abilities when it's not
            //
            if (searchPath[1] == L':')
            {
                fsFlags = 0;
                WCHAR volume[] = L"C:\\";
                volume[0] = searchPath[0];
                GetVolumeInformation(volume, NULL, 0, NULL, NULL, &fsFlags, NULL, 0);
                if (!(fsFlags & FILE_SUPPORTS_HARD_LINKS))
                {
                    fwprintf(stderr, L"\nWARNING: The specified volume %s does not support Windows/NTFS hardlinks. We won't be able to find any of those then!\n\n", volume);
                    // ignore this inability, so we can scan network drives, etc. anyway.
                }
            }
            else if (searchPath[4 + 1] == L':')
            {
                // User very probably specified a '\\?\D:\...' UNC path. Check the drive letter in there.
                fsFlags = 0;
                WCHAR volume[] = L"C:\\";
                volume[0] = searchPath[4];
                GetVolumeInformation(volume, NULL, 0, NULL, NULL, &fsFlags, NULL, 0);
                if (!(fsFlags & FILE_SUPPORTS_HARD_LINKS))
                {
                    fwprintf(stderr, L"\nWARNING: The specified volume %s does not support Windows/NTFS hardlinks. We won't be able to find any of those then!\n\n", volume);
                    // ignore this inability, so we can scan network drives, etc. anyway.
                }
            }

            //
            // Now go and process directories
            //
            searchPattern[0] = 0;           // signal initial call of this recursive function
            ProcessDirectory(searchPath, searchPattern, nelem(searchPattern), recurse, mandatoryAttribs, wantedAnyAttribs, rejectedAttribs, showLinks);
        }
        else if (argv[i][1] == L'c' || argv[i][1] == L'C')
        {
            conciseOutput = TRUE;
        }
        else if (argv[i][1] == L's' || argv[i][1] == L'S')
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
        else if (argv[i][1] == L'o' || argv[i][1] == 'O')
        {
            CloseOutput();

            i++;
            wcscpy_s(listOutputPath, argv[i]);
            NormalizePathSeparators(listOutputPath);

            char fname[MAX_PATH * 5];
            CvtUTF16ToUTF8(fname, sizeof(fname), listOutputPath);
            errno_t err = fopen_s(&output, fname, "w");
            if (!output || err)
            {
                char msg[1024];
                strerror_s(msg, errno);
                fwprintf(stderr, L"Unable to open file '%s' for writing: ", listOutputPath);
                fprintf(stderr, "%s.\n\n", msg);
                return EXIT_FAILURE;
            }
        }
        else
        {
            fwprintf(stderr, L"Unrecognized commandline argument: %s\n\n", argv[i]);
            return Usage(argv[0]);
        }
    }

    CloseOutput();

    // reset progress dots to empty line before we exit.
    ClearProgress();

    if (!FilesMatched)
        fwprintf(stderr, L"\rNo matching files found.\n\n");
    return 0;
}
