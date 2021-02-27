#ifndef __NTFS_ADS_IO_H__
#define __NTFS_ADS_IO_H__

#define UNICODE 1
#include <windows.h>

// return number of elements
#define nelem(a)     (sizeof(a) / sizeof((a)[0]))

//
// Check for success
//
#define NT_SUCCESS(Status) ((NTSTATUS)(Status) >= 0)

//
// The NT return codes we care about
//
#define STATUS_BUFFER_OVERFLOW           ((NTSTATUS)0x80000005L)

//--------------------------------------------------------------------
//     N T F S C O N T R O L F I L E   D E F I N I T I O N S
//--------------------------------------------------------------------

//
// Prototype for NtFsControlFile and data structures
// used in its definition
//

//
// Io Status block (see NTDDK.H)
//
typedef struct _IO_STATUS_BLOCK {
    NTSTATUS Status;
    ULONG Information;
} IO_STATUS_BLOCK, * PIO_STATUS_BLOCK;


//
// Apc Routine (see NTDDK.H)
//
typedef VOID(*PIO_APC_ROUTINE) (
    PVOID ApcContext,
    PIO_STATUS_BLOCK IoStatusBlock,
    ULONG Reserved
    );

//
// File information classes (see NTDDK.H)
//
typedef enum _FILE_INFORMATION_CLASS {
    // end_wdm
    FileDirectoryInformation = 1,
    FileFullDirectoryInformation, // 2
    FileBothDirectoryInformation, // 3
    FileBasicInformation,         // 4  wdm
    FileStandardInformation,      // 5  wdm
    FileInternalInformation,      // 6
    FileEaInformation,            // 7
    FileAccessInformation,        // 8
    FileNameInformation,          // 9
    FileRenameInformation,        // 10
    FileLinkInformation,          // 11
    FileNamesInformation,         // 12
    FileDispositionInformation,   // 13
    FilePositionInformation,      // 14 wdm
    FileFullEaInformation,        // 15
    FileModeInformation,          // 16
    FileAlignmentInformation,     // 17
    FileAllInformation,           // 18
    FileAllocationInformation,    // 19
    FileEndOfFileInformation,     // 20 wdm
    FileAlternateNameInformation, // 21
    FileStreamInformation,        // 22
    FilePipeInformation,          // 23
    FilePipeLocalInformation,     // 24
    FilePipeRemoteInformation,    // 25
    FileMailslotQueryInformation, // 26
    FileMailslotSetInformation,   // 27
    FileCompressionInformation,   // 28
    FileObjectIdInformation,      // 29
    FileCompletionInformation,    // 30
    FileMoveClusterInformation,   // 31
    FileQuotaInformation,         // 32
    FileReparsePointInformation,  // 33
    FileNetworkOpenInformation,   // 34
    FileAttributeTagInformation,  // 35
    FileTrackingInformation,      // 36
    FileMaximumInformation
    // begin_wdm
} FILE_INFORMATION_CLASS, * PFILE_INFORMATION_CLASS;


//
// Streams information
//
#pragma pack(4)
typedef struct {
    ULONG    	        NextEntry;
    ULONG    	        NameLength;
    LARGE_INTEGER    	Size;
    LARGE_INTEGER    	AllocationSize;
    USHORT    	        Name[1];
} FILE_STREAM_INFORMATION, * PFILE_STREAM_INFORMATION;
#pragma pack()


//
// Native functions we use
//
typedef NTSTATUS(__stdcall* NtQueryInformationFile_f)(
    IN HANDLE FileHandle,
    OUT PIO_STATUS_BLOCK IoStatusBlock,
    OUT PVOID FileInformation,
    IN ULONG Length,
    IN FILE_INFORMATION_CLASS FileInformationClass
    );

typedef ULONG(__stdcall* RtlNtStatusToDosError_f) (
    IN NTSTATUS Status
    );



#endif

