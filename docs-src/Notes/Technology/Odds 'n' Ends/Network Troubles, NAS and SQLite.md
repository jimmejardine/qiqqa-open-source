# Network Troubles, NAS and SQLite

## Whiskey and network bugs: 12 year old, cask strength. Still capped.

Old SMB had a nasty network bug around file locking.
NFS, much older still, also had a “known issue” re network file locking.
Bottom line: it doesn’t work.

Empirical data / personal experience: nobody has fixed that issue. 
That is discounting _specific network **monocultures**_ where the software involved is kept up to date and, *more importantly*, kept in sync with all the other network nodes. 
Any **multicultural** network to date has failed to provide trustworthy network file locking. That is: any mix of Windows, UNIX boxen of any denomination, not even bothering with Macs.

Database servers you say? Redundant / fail-over servers you say?
Did you ever take a **real close look** there? 
We’re talking **network file locking** here (as SQLite is an ISAM database, for starters) and none of the systems employed successfully in the wild employ **network file locking**: all of them use other technologies to distribute and *synchronize* tasks. All use network *communications* of some kind, mostly TCP/IP based, but *none* bet their bottom on the premise of **file locking**.
Meanwhile, the enterprise solutions that **do work** are not available to me here as Qiqqa will run on an unknown gamut of machines and operating system versions (even while it currently only supports MS Windows, there’s the question of version, service packs and patch level there) while the “*Sync Directories*”, i.e. the file system slots where Qiqqa will attempt to *synchronize its database and all other relevant data* to, MAY reside on **anything** that’s able to connect to a Windows platform and offer some kind of filesystem interaction. This is includes cloud-based systems such as DropBox, OneDrive and Google Drive. This also includes any other efforts, such as hardware NAS systems (which often run trimmed-down, older, Linux OSes), “software NAS” (anyone who repurposed their older machines as a “free” NAS, really) and god knows what else out there.
Across-the-network locking is simply **not available** when you happen to interact with such a wide gamut of devices.

So we need to come up with “something else”. Or at least *something that might work across the board and not suffer from the current “oddities”*.

## Aside / Reminders

- Currently Qiqqa doesn't support network & UNC mapped paths, e.g. `//share/path/...` (see also [#354](https://github.com/jimmejardine/qiqqa-open-source/issues/354)).
- SQLite (**NOT** the .NET version) has experimental code for WAL2 and other 'modes': those are also relevant for locking and network behaviour, e.g. [#354](https://github.com/jimmejardine/qiqqa-open-source/issues/354)).
- 




# References

- [Determining Whether a Directory Is a Mounted Folder](https://docs.microsoft.com/en-us/windows/win32/fileio/determining-whether-a-directory-is-a-volume-mount-point?redirectedfrom=MSDN)
- [MvsSln :: parsing and processing MSVC solution files](https://github.com/3F/MvsSln)
- [SQLite :: Modern C++](https://github.com/SqliteModernCpp/sqlite_modern_cpp)
- [This is how you do CMake](https://pabloariasal.github.io/2018/02/19/its-time-to-do-cmake-right/)
- [.NET preprocessor directives and defines](https://docs.microsoft.com/en-us/dotnet/csharp/language-reference/preprocessor-directives)
- File locking
  - [MS Windows Oplocks and Caching Controls](https://www.samba.org/samba/docs/old/Samba3-HOWTO/locking.html#id2617411)
  - [Opportunistic Locks](https://docs.microsoft.com/en-us/windows/win32/fileio/opportunistic-locks)
  - [How to Request an Opportunistic Lock](https://docs.microsoft.com/en-us/windows/win32/fileio/how-to-request-an-opportunistic-lock)
  - [Breaking Opportunistic Locks](https://docs.microsoft.com/en-us/windows/win32/fileio/breaking-opportunistic-locks)
  - [Data Coherency](https://docs.microsoft.com/en-us/windows/win32/fileio/data-coherency)
  - [Opportunistic Lock Operations](https://docs.microsoft.com/en-us/windows/win32/fileio/opportunistic-lock-operations)
  - [DeviceIoControl function (ioapiset.h)](https://docs.microsoft.com/en-us/windows/win32/api/ioapiset/nf-ioapiset-deviceiocontrol)
  - [To Disable SMB2 and OPLOCK (Network Locking Errors)](https://infusionsoftware.zendesk.com/hc/en-us/articles/115002293593-To-Disable-SMB2-and-OPLOCK-Network-Locking-Errors-)
  - [SMB3 Leasing Mode](https://infusionsoftware.zendesk.com/hc/en-us/articles/360001439295-SMB3-Leasing-Mode)
  - [Disable Opportunistic Locking and SMB2](https://support.trumpetinc.com/index.php?pg=kb.page&id=2025)
        
    > **This is for server Operating Systems that do not support SMB3(i.e. Windows Server 2008 R2 or earlier), where SMB2 is enabled.**
    >
    > Opportunistic locking and SMB2 are known to cause some really nasty file locking and data loss issues with Worldox and many other applications.  As mentioned above, this usually applies to Windows Server 2008 R2 and earlier.  If you are using SMB3 on a newer server O/S, this is not an issue.
  
  - [Enabling or disabling oplocks and lease oplocks on the storage system](https://library.netapp.com/ecmdocs/ECMP1401220/html/GUID-526B365B-219B-4CA3-AD67-3E1E17B0DB0A.html)
  - [Improving client performance with opportunistic and lease oplocks](https://library.netapp.com/ecmdocs/ECMP1401220/html/GUID-31C39B53-077C-4ED2-8E86-BABBC0495BF7.html)
  - [Opportunistic Locking and Read Caching on Microsoft Windows Networks](https://www.cardbox.com/v2/oplocks.htm)
  - [How to verify and disable SMB oplocks and caching in FoxPro application startup](https://stackoverflow.com/questions/58044466/how-to-verify-and-disable-smb-oplocks-and-caching-in-foxpro-application-startup)
  - [Disabling oplock/SMB2 vs FileInfoCacheLifetime](https://social.technet.microsoft.com/forums/windowsserver/en-US/67baa9fd-5eaf-438e-9cc4-dc1a531b9e19/disabling-oplocksmb2-vs-fileinfocachelifetime)
  - [Opportunistic Locking Explained](https://www.superbase.com/support/opportunistic-locking-explained/)
  - [Performance tuning for file servers](https://docs.microsoft.com/en-us/windows-server/administration/performance-tuning/role/file-server/)
  - [How to Fix Slow Access to Network Shares](https://www.zubairalexander.com/blog/how-to-fix-slow-access-to-network-shares/)
  - [Microsoft Access Database Corruption](https://answers.microsoft.com/en-us/msoffice/forum/msoffice_access-mso_win10-mso_2016/microsoft-access-database-corruption/e933f760-f7f2-4b3e-9bc3-0061d5073219)
  - [Microsoft Access Database corruption #934](https://github.com/MicrosoftDocs/windows-itpro-docs/issues/934)
  - [Network share performance of Windows 10 1803](https://community.spiceworks.com/topic/2135698-network-share-performance-of-windows-10-1803)
  - [File existence checks failing with SMB 2.0 even though file exists](https://microsoft.public.win32.programmer.networks.narkive.com/wDwqoQAn/file-existence-checks-failing-with-smb-2-0-even-though-file-exists)
  - [Mapped drive and files not showing up for a while.](https://community.spiceworks.com/topic/1326173-mapped-drive-and-files-not-showing-up-for-a-while)
  - [Disable SMBv1 Server with Group Policy](https://docs.microsoft.com/en-us/windows-server/storage/file-server/troubleshoot/detect-enable-and-disable-smbv1-v2-v3)
  - [How to configure Samba to use SMBv2 and disable SMBv1 on Linux or Unix](https://www.cyberciti.biz/faq/how-to-configure-samba-to-use-smbv2-and-disable-smbv1-on-linux-or-unix/)
  - [How to check SMB version on Windows 10](https://www.thewindowsclub.com/check-smb-version-windows)
      
    In PowerShell:
         
    ``` 
    Get-SmbServerConfiguration
    ``` 
                
  - [How to enable/disable SMBv1, SMBv2, and SMBv3 in Windows and Windows Server](https://www.alibabacloud.com/help/faq-detail/57499.htm)      
  - [File locks on an NFS?](https://serverfault.com/questions/66919/file-locks-on-an-nfs)
  - [Sqlite over a network share \[closed\]](https://stackoverflow.com/questions/788517/sqlite-over-a-network-share)
  - [SQLite Over a Network,  Caveats and Considerations](https://www.sqlite.org/useovernet.html)
  - [SQLite :: How To Corrupt Your Database Files](https://www.sqlite.org/lockingv3.html#how_to_corrupt)
  - [SQLite on Network Share #1886](https://github.com/Sonarr/Sonarr/issues/1886)
  - [Wikipedia :: File locking](https://en.wikipedia.org/wiki/File_locking#Lock_files)
  - [Linux: Mailbox locking over NFS](https://www.spinnaker.de/linux/nfs-locking.html)
  - [flock(2) versus fcntl(2) over a NFS](https://unix.stackexchange.com/questions/1777/flock2-versus-fcntl2-over-a-nfs)
  - [On the Brokenness of File Locking](http://0pointer.net/blog/projects/locking.html)
  - [Addendum on the Brokenness of File Locking](http://0pointer.net/blog/projects/locking2.html)
  - [Samba :: A Tale of Two Standards](https://www.samba.org/samba/news/articles/low_point/tale_two_stds_os2.html)
  - [File Locking](https://www.ict.griffith.edu.au/teaching/2501ICT/archive/guide/ipc/flock.html)
  - [File Locking And Concurrency In SQLite Version 3](https://sqlite.org/lockingv3.html)
       
    The document only describes locking for the older rollback-mode transaction mechanism. Locking for the newer [write-ahead log](https://sqlite.org/wal.html)or[WAL mode](https://sqlite.org/wal.html) is described separately.
       
  - [SQLite :: Things That Can Go Wrong](https://sqlite.org/atomiccommit.html#sect_9_0)
  - [2 Types of Linux File Locking (Advisory, Mandatory Lock Examples)](https://www.thegeekstuff.com/2012/04/linux-file-locking-types/)
  - [flock fails on shared NFS for exclusive blocking locks from 2 different machines](https://serverfault.com/questions/1001700/flock-fails-on-shared-nfs-for-exclusive-blocking-locks-from-2-different-maschine)
  - [OSError: Unable to open file (unable to lock file, errno = 37, error message = 'No locks available') #1101](https://github.com/h5py/h5py/issues/1101)
  - [How To Unlock Access ldb/laccdb File?](http://www.accessrepairnrecovery.com/blog/access-ldb-or-laccdb-file-unlock)





- [# Obtaining Directory Change Notifications](https://docs.microsoft.com/en-us/windows/win32/fileio/obtaining-directory-change-notifications?redirectedfrom=MSDN)


