# Filesystems: *caveat emptor*
(or: ... and then I thought NTFS was a little nasty... 🤕😫)



Most days I'm happy to learn something new every day. *Today is not such a day.*

🤔 Today I learned the Win/NTFS bashers, a.k.a. Linux lovers, have some pretty aggravating filesystem limitations of their own.
🤡 Here's a couple of pretty innocent filenames for y'all to copy across a filesystem boundary, onto a brand spankin' new 16TB BTRFS disk:

```
/media/ger/16TB2303A/6TB/Qiqqa/SopKonijn/!QIQQA-pdf-watch-dir/!5/docs/UNKNOWN - 增益值-信号极性以及更新速率的选择可用串行输入口由软件来配臵-该器件还包括自校准和系统校准选项-以消除器件本身或系统的增益和偏移误差- CMOS 结构确保器件具有极低功耗-掉电模式减少等待时的功耗至.pdf

/media/ger/16TB2303A/6TB/Qiqqa/SopKonijn/!QIQQA-pdf-watch-dir/2018-10-09/Phased Locked Loop - PLL/74HCT9046A Philips Semiconductor PDFs ����������� ������������ 74HCT9046A �������� ������� 74HCT9046A.pdf Philips Semiconductor PDFs datasheets datasheet data sheets 74HCT9046A Philips Semiconductor PDFs.html
```

Nothing that would faze a decent, latest Ubuntu, system, right?

**Wrong**!!!1! 
`ENAMETOOLONG`, a.k.a. `error: file name too long`.

WTF? These were *legal* on NTFS!

## File name length, file *path* length, *legal filename characters*, UCS-2 vs. UTF16 vs. UTF8 and the size of *characters*, case sensitivity and more of that jazz...

Ok, let's dial down the frustration and the snark and have a deeper look at this.

> TL;DR and only *one* problem of many: https://unix.stackexchange.com/questions/619625/to-what-extent-does-linux-support-file-names-longer-than-255-bytes

Here we ran into the general *filename limitation* on Linux systems: 256 *bytes* max. 
And, yes, the above file**names** are longer than 256 *bytes* when they are encoded in UTF8.
Meanwhile Win10/NTFS didn't barf a hairball on these as *there* the *usual* filename limit is 256 *characters* -- which the given filenames *do not surpass* as they will be encoded as a UCS-2/UTF16 string.

### How I encountered that one

I ran into this issue while further recovering an important drive from an otherwise completely  failed Win10 system: a severe (and very much *fatal*) hardware failure which on its way down, before it croaked, did quite successfully corrupt a good 50TB of affixed storage in all sorts of amazing new ways. *Ouch.* 🤕🚑 The new ordered hardware then refused to cooperate for any appreciable time, reducing the dev rig to shreds and handing me a strong conviction **never** to buy Gigabyte mobo hardware *ever again*.

Anyhow, some older hardware was revived and given a fresh coat of Ubuntu/Mint to serve as part of the recovery effort and here I was, copying the NTFS drives onto newly arrived spinning rust (by Toshiba), formatted as BTRFS. And there suddenly the copy command spit out these errors.

I was living in the dream where Linux filesystems could handle *anything*. That bubble has been popped, *violently*. https://en.wikipedia.org/wiki/Comparison_of_file_systems#Limits

So here's one limitation where NTFS is way more lenient.

Any others we should keep in mind?

### `PATH_MAX` / `MAX_PATH` (Windows: 260? 259? 255? 😖 Linux: 1024? 4096? BSD/OSX/Android: ???)

On Windows this is set to [260 == `MAX_PATH`](https://learn.microsoft.com/en-us/windows/win32/fileio/maximum-file-path-limitation?tabs=registry). *However*, this can be circumnavigated by specifying UNC file paths like this: `\\?\Drive:\ultra\long\path\to\file`. In which case the path limit becomes 32767. 
- https://it.cornell.edu/shared-file/windows-file-name-or-destination-path-you-specified-not-valid-or-too-long
- https://learn.microsoft.com/en-us/windows/win32/fileio/maximum-file-path-limitation?tabs=registry

Quoting the (Microsoft) source here[^1]:

[^1]: We quote the entire thing as Microsoft is one of those who enthusiastically embrace *the notion that any publication on the Internet has a (very) limited lifetime*: their technology info pages have been moved quite often, resulting in a slew of dead/zombie links over the links and I'm loath to loose this intel.

---

In the Windows API (with some exceptions discussed in the following paragraphs), the maximum length for a path is `MAX_PATH`, which is defined as **260 characters**. A local path is structured in the following order: drive letter, colon, backslash, name components separated by backslashes, and a terminating null character. For example, the maximum path on drive D is `"D:\_some 256-character path string_<NUL>"` where `"<NUL>"` represents the invisible terminating *null character* for the current system codepage. (The characters `< >` are used here for visual clarity and cannot be part of a valid path string.)

For example, you may hit this limitation if you are cloning a git repo that has long file names into a folder that itself has a long name.

> Note
>
> File I/O functions in the Windows API convert `"/"` to `"\"` as part of converting the name to an NT-style name, except when using the `"\\?\"` prefix as detailed in the following sections.

*The Windows API has many functions that also have Unicode versions to permit an extended-length path for a maximum total path length of 32,767 characters.* This type of path is composed of components separated by backslashes, each up to the value returned in the `lpMaximumComponentLength` parameter of the [`GetVolumeInformation`](https://learn.microsoft.com/en-us/windows/desktop/api/FileAPI/nf-fileapi-getvolumeinformationa) function (*this value is commonly 255 characters*). To specify an extended-length path, use the `"\\?\"` prefix. For example, `"\\?\D:\_very long path_"`.

> Note
>
> The maximum path of 32,767 characters is approximate, because the `"\\?\"` prefix may be expanded to a longer string by the system at run time, and this expansion applies to the total length.

The `"\\?\"` prefix can also be used with paths constructed according to the universal naming convention (UNC). To specify such a path using UNC, use the `"\\?\UNC\"` prefix. For example, `"\\?\UNC\server\share"`, where `"server"` is the name of the computer and `"share"` is the name of the shared folder. These prefixes are not used as part of the path itself. They indicate that the path should be passed to the system with minimal modification, which means that you cannot use forward slashes to represent path separators, or a period to represent the current directory, or double dots to represent the parent directory. Because you cannot use the `"\\?\"` prefix with a relative path, relative paths are always limited to a total of `MAX_PATH` characters.

There is no need to perform any Unicode normalization on path and file name strings for use by the Windows file I/O API functions because the file system treats path and file names as an opaque sequence of `WCHAR`s. Any normalization that your application requires should be performed with this in mind, external of any calls to related Windows file I/O API functions.

When using an API to create a directory, the specified path cannot be so long that you cannot append an 8.3 file name (that is, the directory name cannot exceed `MAX_PATH` minus 12).

The shell and the file system have different requirements. It is possible to create a path with the Windows API that the shell user interface is not able to interpret properly.



#### Enable Long Paths in Windows 10, Version 1607, and Later

Starting in Windows 10, version 1607, `MAX_PATH` limitations have been removed from common Win32 file and directory functions. However, you must opt-in to the new behavior.

To enable the new long path behavior, both of the following conditions must be met:

-   The registry key `Computer\HKEY_LOCAL_MACHINE\SYSTEM\CurrentControlSet\Control\FileSystem\LongPathsEnabled (Type: REG_DWORD)` must exist and be set to 1. The key's value will be cached by the system (per process) after the first call to an affected Win32 file or directory function (see below for the list of functions). The registry key will not be reloaded during the lifetime of the process. In order for all apps on the system to recognize the value of the key, a reboot might be required because some processes may have started before the key was set.

  You can also copy this code to a `.reg` file which can set this for you, or use the PowerShell command from a terminal window with elevated privileges.

  Console

```
Windows Registry Editor Version 5.00

[HKEY_LOCAL_MACHINE\SYSTEM\CurrentControlSet\Control\FileSystem]
"LongPathsEnabled"=dword:00000001
```

> Note
>
> This registry key can also be controlled via Group Policy at `Computer Configuration > Administrative Templates > System > Filesystem > Enable Win32 long paths`.

-  The [application manifest](https://learn.microsoft.com/en-us/windows/win32/sbscs/application-manifests) must also include the `longPathAware` element.
    
    XML
    
```
    <application xmlns="urn:schemas-microsoft-com:asm.v3">
        <windowsSettings xmlns:ws2="http://schemas.microsoft.com/SMI/2016/WindowsSettings">
            <ws2:longPathAware>true</ws2:longPathAware>
        </windowsSettings>
    </application>
```
    

These are the directory management functions that no longer have `MAX_PATH` restrictions if you opt-in to long path behavior: `CreateDirectoryW`, `CreateDirectoryExW`, `GetCurrentDirectoryW`, `RemoveDirectoryW`, `SetCurrentDirectoryW`.

These are the file management functions that no longer have `MAX_PATH` restrictions if you opt-in to long path behavior: `CopyFileW`, `CopyFile2`, `CopyFileExW`, `CreateFileW`, `CreateFile2`, `CreateHardLinkW`, `CreateSymbolicLinkW`, `DeleteFileW`, `FindFirstFileW`, `FindFirstFileExW`, `FindNextFileW`, `GetFileAttributesW`, `GetFileAttributesExW`, `SetFileAttributesW`, `GetFullPathNameW`, `GetLongPathNameW`, `MoveFileW`, `MoveFileExW`, `MoveFileWithProgressW`, `ReplaceFileW`, `SearchPathW`, `FindFirstFileNameW`, `FindNextFileNameW`, `FindFirstStreamW`, `FindNextStreamW`, `GetCompressedFileSizeW`, `GetFinalPathNameByHandleW`.

----

See also https://learn.microsoft.com/en-us/windows/win32/fileio/naming-a-file (which is mentioned again further below in another subtopic, BTW: (Un-)acceptable filename characters & system aliases you MUST NOT use for regular files).



### File name length vs. *path length*

Default Windows *path length* limit is 260 *characters*, while Linux has a compiled hard limit of 4096 *bytes* AFAICT. Meanwhile Linux also has a *file NAME length* limit, which is set at 255 *bytes* for all regularly used Linux filesystems: ext4, btrfs, etc.

Both OSs call their length limit unit "*characters*", but that's incorrect: MSWindows counts UCS-2 codes while Linux counts *bytes*, while the latter encodes filenames in UTF8.




### UCS-2 vs. UTF16 vs. UTF8 and the size of *characters*

See the section above.




### Case (in-)sensitivity

MSWindows and its major filesystem (NTFS) are case-insensitive: you CANNOT have `file.txt` and `file.TXT` in the same directory as both names would identify the very same file there.

I haven't explicitly checked for xFAT, vFAT, et al, but back in the day of MSDOS & Win95, it was already like this.

Linux filesystems et al are (almost always) case-sensitive, but when you mount an NTFS drive on Linux, it, of course, is still case-INsensitive as that's part of that filesystem's definition. DO however reckon with slightly different behaviour when use such "foreign" filesystems, as the filename sensitivity/INsensitivity kicks in at a different level, so you MAY observe slightly altered behaviour when addressing files. E.g. `find` and other UNIX tools are still very much case-*sensitive* in their filename matching, irrespective of the filesystem they happen to access. In the case of `find`, you might want to use `-iname '*.pdf'` rather than the usual `-name '*.pdf'`, for example, when looking for PDF files on a given mounted filesystem!








### (Un-)acceptable filename characters & system aliases you MUST NOT use for regular files

For Windows, see also: https://learn.microsoft.com/en-us/windows/win32/fileio/naming-a-file.

Bottom line: 
 - only use characters which are either:
   * ASCII code 32 or above *and* are `isprint()`-able *and* DO NOT have special meaning in the OS and/or the various shells (i.e. these are out: `<>:/\|"?*` and you're well advised to also forego on `!#$%^&{}[];` and use `~.,'+@=` and the "\`" *backtick* very judiciously, for any of those can be used to assist in undesirable shell hacking[^2]; rejecting all of those improves your file naming security)
   * Unicode codepoints 128 and above which have [the `Graphic` Unicode Basic Type Character Property](https://en.wikipedia.org/wiki/Unicode_character_property#General_Category). Hence DO NOT accept any Undefined, Reserved, Noncharacter, Private-use, Format and Control Unicode codepoints.
- DO NOT accept any of these file / directory *basenames*: 
  - CON, PRN, AUX, NUL, COM0, COM1, COM2, COM3, COM4, COM5, COM6, COM7, COM8, COM9, LPT0, LPT1, LPT2, LPT3, LPT4, LPT5, LPT6, LPT7, LPT8, and LPT9
  - while on UNIXes a directory named `dev` might be ill-advised. (Writing to a **device** such as `/dev/null` is perfectly okay there; we're interested and focusing on *regular files and directories* here though, so giving them names such as `dev`, `stdout`, `stdin`, `stderr` or `null` might be somewhat... *ill-advised*. 😉
  - of course, Mac/OSX has it's own set of reserved names and aliases, e.g. all filenames starting with `._` are "extended attributes": when you have filename `"xyz"`, OSX will create an additional file named `"._xyz"` when you set any *extended attributes*. There's also the `__MACOSX` directory that you never see -- unless you look at the same store by way of a MSWindows or Linux box.
- it's ill-advised to start (or end!) any file or directory name with `"."` or `"~"`: the former is used to declare the file/directory "hidden" in UNIX, while the latter MAY be treated as a shorthand for the user home directory or signal the file is a "backup/temporary file" when appended at the end.
- NTFS has a set of reserved names of its own, all of which start with `"$"`, while antique MSDOS reserved filenames *end* with a `"$"`, so you'ld do well to forego `"$"` anywhere it shows up in your naming. To put the scare into you, there's a MSWindows hack which will *crash and corrupt* the system by simply *addressing* a (non-existing) reserved filename `"$i30:$bitmap"` on any NTFS drive (CVE-2021-28312): https://www.bleepingcomputer.com/news/security/microsoft-fixes-windows-10-bug-that-can-corrupt-ntfs-drives/
- OneDrive also [mentions several reserved filenames](https://support.microsoft.com/en-us/office/restrictions-and-limitations-in-onedrive-and-sharepoint-64883a5d-228e-48f5-b3d2-eb39e07630fa#invalidcharacters): "*These names aren't allowed for files or folders: `.lock`, `CON`, `PRN`, `AUX`, `NUL`, `COM0` - `COM9`, `LPT0` - `LPT9`, `_vti_`, `desktop.ini`, any filename starting with `~$`.*" It also rejects `".ds_store"`, which is a hidden file on Mac/OSX. [Elsewhere](https://support.microsoft.com/en-us/office/onedrive-can-rename-files-with-invalid-characters-99333564-c2ed-4e78-8936-7c773e958881) it mentions these again, but with a broader scope: "*These other characters have special meanings when used in file names in OneDrive, SharePoint, Windows and macOS, such as `"*"` for wildcards, `"\"` in file name paths, **and names containing** `.lock`, `CON`, or `_vti_`.*" They also strongly discourage the use of `#%&` characters in filenames.
- Furthermore it's a **bad idea** to start any filename ith `"-"` or `"--"` as this clashes very badly with standard UNIX commandline options; only a few tools "fix" this by allowing a special `"--"` commandline option, e.g. 
- [GoogleDrive](https://developers.google.com/style/filenames) doesn't mention any explicit restrictions, but the published *guidelines* rather suggest limiting oneself to plain ASCII only, with all the other restrictions mentioned in this larger list. Google is (as usual for those buggers) *extremely vague* on this subject, but [some hints](https://www.googlecloudcommunity.com/gc/Workspace-Q-A/File-and-restrictions-for-Google-Drive/td-p/509595) have been discovered: path length *probably* limited to 32767, directory depth max @ 21 is mentioned (probably gleaned from [here](https://support.google.com/a/users/answer/7338880?hl=en#shared_drives_file_folder_limits), however I tested it on my own GoogleDrive account and at the time of this writing (May 2023) Google didn't object to creating a tree depth >= 32, nor a filename <= 500 characters.)
- While I'm not particularly fond of C++ Boost, they have this to offer: https://www.boost.org/doc/libs/1_81_0/libs/filesystem/doc/portability_guide.htm, where `name_check` functions are described. It would have been so much more useful when they'ld actually offered sanitization functions alongside these check-if-valid simpletons. *sigh*.
- 


  


[^2]: for example, having a `"$"` in your filename can result in inadvertent shell variable expansion when such a file is processed via a shell command or shell script. The other characters can have all sorts of surprising effects when used in Windows. Mac or Unix shell scripts or as arguments in shell commands. YMMV.

See also:
- https://superuser.com/questions/104500/what-is-macosx-folder
- https://en.wikipedia.org/wiki/Filename
- https://www.bleepingcomputer.com/news/security/windows-10-bug-corrupts-your-hard-drive-on-seeing-this-files-icon/ + https://www.bleepingcomputer.com/news/security/microsoft-fixes-windows-10-bug-that-can-corrupt-ntfs-drives/ + https://msrc.microsoft.com/update-guide/en-US/vulnerability/CVE-2021-28312 + https://www.cvedetails.com/cve/CVE-2021-28312/
- https://stackoverflow.com/questions/2679699/what-characters-allowed-in-file-names-on-android
- https://support.microsoft.com/en-us/office/restrictions-and-limitations-in-onedrive-and-sharepoint-64883a5d-228e-48f5-b3d2-eb39e07630fa#invalidcharacters
- https://support.microsoft.com/en-us/office/onedrive-can-rename-files-with-invalid-characters-99333564-c2ed-4e78-8936-7c773e958881
- https://techcommunity.microsoft.com/t5/microsoft-sharepoint-blog/new-support-for-and-in-sharepoint-online-and-onedrive-for/ba-p/60357
- https://developers.google.com/style/filenames
- https://www.googlecloudcommunity.com/gc/Workspace-Q-A/File-and-restrictions-for-Google-Drive/td-p/509595
- https://www.mtu.edu/umc/services/websites/writing/characters-avoid/
- https://kb.synology.com/en-au/DSM/tutorial/Why_are_dot_underscore_files_created
- https://www.boost.org/doc/libs/1_81_0/libs/filesystem/doc/portability_guide.htm
- https://github.com/microsoft/vscode/issues/60383
- https://help.workplace.datto.com/help/Content/1_GENERAL/File_Name_Constraints.htm
- https://www.sweetwater.com/sweetcare/articles/there-restrictions-what-name-files-computer/
- 

---

Google *sez*:

---

### Item cap

*A shared drive can contain a maximum of 400,000 items*, including files, folders, and shortcuts. **Note**: This limit is based on item count, not storage use. We recommend that you keep shared drives well below the strict limit. Shared drives with too many files might be difficult to organize and search, or members ignore much of the content.

If you're a Google Workspace administrator, you can see how close a shared drive is to the item limit in your Admin console. Search for "Manage shared drives" and review the **Item cap** column.


### File sharing limits

A file in a shared drive can be directly shared with a maximum of 100 groups.


### Folder nesting and moving

**A folder in a shared drive can have up to 20 levels of nested folders.** We recommend that you avoid creating many folders in one shared drive. Members might have difficulty organizing and finding content. Instead, organize content into multiple shared drives.

When using Drive for desktop, you can’t move a folder from My Drive to a shared drive.

---

UMC (Michian Tech) has this to say:

-----

### Characters to Avoid in Filenames and Directories

Your web files will be viewed by numerous users who use a wide variety of operating systems (Mac, PC, and Linux for instance) and devices (desktops, tablets, and smartphones are some examples). Therefore, it is essential to play it safe and **avoid common illegal filename and directory characters**.

Naming conventions are important in _web folders_ as well as for _downloadable_ files such as HTML files, images, PDFs, Word documents, and Excel spreadsheets.

#### Illegal Filename Characters

Do not use any of these common illegal characters or symbols in your filenames or folders:

\# pound

% percent

& ampersand

{ left curly bracket

} right curly bracket

\ back slash

< left angle bracket

\> right angle bracket

\* asterisk

? question mark

/ forward slash

  blank spaces

$ dollar sign

! exclamation point

' single quotes

" double quotes

: colon

@ at sign

\+ plus sign

\` backtick

| pipe

= equal sign



Also, keep these rules in mind.

-   Don’t start or end your filename with a space, period, hyphen, or underline.
-   Keep your filenames to a reasonable length and be sure they are under 31 characters.
-   Most operating systems are case sensitive; **always use lowercase**.
-   Avoid using spaces and underscores; use a hyphen instead. This will also improve your [search engine rankings](https://www.mtu.edu/umc/services/websites/seo/).


-----

DATTO adds:

-------

### File name constraints and best practices

Workplace does not have any character limits for files names, but operating systems **do** have limitations on the use of certain characters and file/directory path lengths.  

#### File path/directory length

We recommend that you keep directory lengths under 255 characters.

-   Maximum **directory path** length of 255 characters for Windows.
-   Maximum **file name** length of 255 characters for Mac.

#### Special characters

If you use characters in your naming conventions, please verify your file and folder names adhere to the limitations, including special character conventions, of **all** the operating systems used by your company.

If you use a special character when naming a file in Workplace Online, or if you use one of the special characters that Mac supports that Windows doesn’t, it will automatically be replaced **on the machine that doesn't support it** with an underscore.

EXAMPLE  For example, if you create `“1>2<3*special.png”` on a Mac, a file by the same name will be created on Workplace Online. This file **will sync** to a Windows machine, but it will be called `“1_2_3_special.png”`. The system will remember this exception, so it will continue to sync with `“1>2<3*special.png”` on the server.

A special character is a letter or symbol that cannot be used in file names because it is being utilized in another location or by the operating system.

[![Open](https://help.workplace.datto.com/help/Skins/Default/Stylesheets/Images/transparent.gif)Special characters for Macintosh OS X and later:](https://help.workplace.datto.com/help/Content/1_GENERAL/File_Name_Constraints.htm#)

-   : (colon)
-   . (dot/period) *hidden when a filename starts with “.” In all other instances, this character is acceptable.
-   / (forward slash)

[![Open](https://help.workplace.datto.com/help/Skins/Default/Stylesheets/Images/transparent.gif)Special characters for Windows:](https://help.workplace.datto.com/help/Content/1_GENERAL/File_Name_Constraints.htm#)

-   < (less than)
-   > (greater than)
-   : (colon)
-   " (quote)
-   / (forward slash)
-   \\ (backslash)
-   | (vertical bar)
-   ? (question mark)
-   * (asterisk)
-   ^ (caret) *FAT


**NOTE**  The safest approach is to completely avoid using special characters.

----------

Sweetwater Q&A he qothed:

------

### Are there restrictions on what I can name my files on my computer?

(Article #31650, published on May 4, 2007 at 12:00 AM)

The following list is fairly exhaustive and pulls together references from various sources. Although not mentioned explicitly, Unix seems to have few – if any – restrictions. Compliance with these conventions as assets are added to your library will allow widest use of the assets without subsequent manual intervention to re-path/name, etc. The rules take into account the use of assets on local & network hard drives, CD/DVD, removable drives and online (web/ftp) using Mac OS9/OSX and Windows OSs:

- Illegal filename characters, (e.g. `:` or `?`). (All OSs).  
- Deprecated filename characters (`;` and `,`). (All OSs).  
- \>31 filename characters including extension. (Mac Classic).  
- \>64 filename characters including extension. (Windows: ISO9660+Joliet CD or Hybrid CD partition).  
- No extension – extensions are mandatory for Windows and the only means for Portfolio to tell file type. (Windows, Mac OS X).  
- Filename has \>1 period – Portfolio may misinterpret extension. (Windows, Mac OS X).  
- Extension may be wrong, i.e. not 3 characters. (Windows, Mac OS X).  
- Illegal characters in path to file – same issue as #1 but for path. (All OSs).  
- Deprecated characters in path to file – same issue as #2 but for path. (All OSs).  
- Filename may not begin with a period. (Windows not allowed, Mac treats as a hidden file)  
- Filename may not end in a period. (Windows not allowed – OS ‘throws away’ the trailing period when naming/reading so incorrect matching vs. Mac name)  
- Names conflicting with some of Win OS’ old DOS functions (Not allowed in either upper or lowercase and with or without a file extension or as a file extension: COM1 to COM9 inclusive, LPT1 to LPT9 inclusive, CON, PRN, AUX, CLOCK$ and NUL)  
- Case sensitivity. Windows OSs (and IIS web servers) aren’t case sensitive. Most other OSs (and web servers) are.  

Notes:

Whilst the illegal characters are simply not supported for use by one or more of the OSs to be used, the deprecated characters include those deprecated for naming purposes plus those likely to cause use in a web/ftp environment.  

- \#1. Illegal filename characters: `\` (backslash), `/` (forward slash), `:` (colon), `*` (asterisk), `?` (question mark), `”` (double quotes), `<` (left angle bracket), `>` (right angle bracket), `|` (pipe). Most of these are Windows OS constraints; Mac allows all except a colon (though a forward slash, `/`, can cause issues for POSIX paths). The aim here is to allow problem-free cross-platform use. An all-Windows or all-Mac organisation may need to interact with others using different OSs, so the safe method is to observe both OS’ limitations, even if you mostly/always work on only one OS.  
- \#2. Avoid `%`, `#`, and `$` as these are commonly used as variable name prefixes, so it can get messy if automating anything with filenames that include these characters. If networking cross platform (e.g. Samba, SMB, CIFS) consider effects of `!+{}&[]` on path and filename translation.  
- \#2. Where possible avoid spaces in filenames (though not strictly necessary, they can complicate things, especially if scripting). You are best advised to stick to alphanumerics, underscores, hyphens, periods. **Do not use a hyphen or a period as the first or last character of a filename as this can have special meaning on some OSs** (e.g. a starting period often indicates the file is hidden or system file that is not displayed to non-admin user accounts).  
- \#2. File/folder delimiters: Mac Classic uses a colon, Mac OS X uses either forward slash (POSIX paths) or colon (‘Mac’ paths), Unix uses a forward slash and Windows a backslash (plus colon for drive letter).  
- \#2 & \#9. Deprecated filename & path characters: `,` (comma), `;` (semi-colon), ` ` (space), `–` (bullet = ASCII #149), `%` (percent), `&` (ampersand). The ‘bullet’ character has no special significance but does seem popular as a form of name punctuator amongst some Mac users but it can cause unreadable filenames in a cross-platform environment.  
- \#2 & \#9. Unix has few limitations. Filenames may be up to 256 characters. A forward slash (`/`) is a folder delimiter and a leading period (`.`) makes that file a system file.  
- \#3 & \#4. Windows, Unix and Mac OS X all support long filenames. References conflict as to whether exactly 256 or 255 characters are allowed (in Windows this includes the extension). However, as no current OS will allow you to create a filename exceeding this and all have the same top limit, this is one constraint you are unlikely to have to test for!  
- \#3. Consider the impact of long filenames on their display in OS and program dialogs, web pages etc. For instance Firefox won’t wrap long strings of alphanumeric characters (nor can you force it to do so via code) so that long filenames may ‘break’ web layouts not designed with this issue in mind.  
- \#4. The ‘Joliet’ standard for Windows CD supporting long filenames has a limit of 64 characters for the total path (folders & filename). If you need deep nesting, use short folder and file names! Also watch for limitations on the depth of folder nesting that can occur with strict observance of ISO9660 (see first bullet in next list). This is something that is rarely explained in CD/DVD burning software and is more likely to bite when using older systems/software. If path length limitations are a likely issue then make sure you test before starting any large body of work.  
- \#5. Whilst modern 32-bit Windows (post Win 95) will tolerate files without extensions it does not know how to handle them. In addition some apps use the OSs ‘knowledge’ of file types to help it when finding/opening files – i.e. if the OS is confused so too will be some apps. Bottom line, if Windows use is likely always have file extensions. OSX usage seems to be drifting towards the extension model now resource fork info is becoming a legacy issue.  
- \#6. Mac, making less deliberate use of extensions allows periods in names. Portfolio will assume the set of characters after the right-most period in the name is the Windows-compatible extension. So if you must have, for example, `“myname.txt.stuff”` for a text file, better to call it `“myname.stuff.txt”`; Portfolio will read the first as having extension `“.stuff”`, whilst the second will be read as `“.txt”`. If you intend to transmit files over the internet (especially as an email attachment) multiple periods such as `“filename.tif.zip”` will often get the file killed in transit. This has more to do with IT managers that see files with “multiple extensions” as a risk since the true nature of the file may be “masked” so that a worm/virus can inflict damage.  
- \#7. Windows convention is for three letter extensions but more or fewer characters are not unknown, e.g. `“logo.ai”` (Illustrator) `“page.html”` (some HTML editors’ output). The point of this rule is not to insist extensions of 3 characters but to flag up odd – normally Mac created names – that might have problems on Windows computers.  
- \#8. Illegal path characters: as above minus backslash and colon.  
- \#9. Periods, though allowed in filenames, are deprecated as they aren’t supported in stricter ISO9660 versions (without Joliet) and on some older systems such as VMS.  
- \#9. For web use, periods in filenames are deprecated as some web servers (especially IIS), when fully security patched, will not serve content if the URL has any folder names containing a period.  
- \#10/11. You are well advised not to use commas as a starting or finishing character – they are likely to get missed when reading by eye.  
- \#12. See also MSDN. `“Null.txt”` is allowed but `“nul.txt”`, `“NUL”` or `“nul”` are not.  
- \#13. Consider being case-insensitive when naming. In a large archive it can be tempting fate to rely on case to distinguish a file from another; thus ideally `“Filename.jpg”` and `“FILENAME.JPG”` should resolve to be the same file. In addition, `“Filename.jpg”` is arguably not unique. So regardless of whether your primary OS is case-sensitive, it is a good idea to (a) treat all case variations of a name as one for uniqueness of naming but (b) use only ever use one variant of the name (as if the OS were case-sensitive).  

#### So that’s it then?  

Not really, these rules are advisory – you will need to think how to apply them. For instance you may work only on Mac OS and thus don’t need to worry about folder names but you will need to ensure you files – or those you may need to send to users not on Macs – have extensions. Of course, the extension needs to be appropriate – don’t put `‘.jpg’` on the end of a TIFF, for example. If you’re unsure about extensions there are numerous online listing that will give these to you.

On the subject of extensions, this is one thing it’s not too easy to change in Portfolio except via ‘link to new file’ or scripting. As ‘link to new file’ require you so manually set each connection, scripting is the way to go – if you can define the changes to make.

Anyway, don’t feel compelled to apply all of the above rules. Rather, be aware of them and apply them as appropriate. If you are starting a new collection or re-ordering old content you’re at the right point to get some discipline into the naming. If some or all of your new content comes in from external sources and is poorly named, sit down with your partners make sure good naming rules are in the contract. Then, if it’s wrong, they can rename files at their time/expense – a good encouragement for them to adopt good practice from outset.

#### Some final thoughts  

You might also consider these additional rules where older OS variants are in use:

Strict ISO9660 Level 1. Files and folders max 8.3 characters; only uppercase letters, numeral, or underscore characters allowed. Folder may only nest 8 levels (i.e. 7 below the root) in the strictest interpretation of the standard. ISO level 2 extends filename length to 31 characters.  
8.3 filenames. Files must be 8.3 characters or less (`x.y` through `xxxxxxxx.yyy`) and folders 8 or 8.3. characters or less (`\x\` through `\xxxxxxxx\` and `\x.y\` through `\xxxxxxxx.yyy\` – noting folder names with extensions are deprecated in Windows OS usage).  
Non alphanumeric characters (i.e. only allow letters and numbers).  
What ‘special’ characters does Windows (95/98) allow? The following is from Microsoft. Note that many of the following are deprecated for cross-platform workflow use for the reasons already stated above:  

$ Dollar sign  
% Percent sign  
‘ Apostrophe  
\` Opening single quotation mark  
– Hyphen  
@ At sign  
{ Left brace  
} Right brace  
~ Tilde  
! Exclamation point  
\# Number sign  
( Opening parenthesis  
) Closing parenthesis  
& Ampersand  
_ Underscore  
^ Caret  
\+ Plus sign  
, Comma  
. Period  
= Equal sign  
\[ Opening bracket  
] Closing bracket

-----------




  
   * The following reserved characters:
    
    -   < (less than)
    -   > (greater than)
    -   : (colon)
    -   " (double quote)
    -   / (forward slash)
    -   \ (backslash)
    -   | (vertical bar or pipe)
    -   ? (question mark)
    -   * (asterisk)














