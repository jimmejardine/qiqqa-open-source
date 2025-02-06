# File last change + last access timestamps' woes

See https://github.com/git-for-windows/git/issues/1000#issuecomment-301611003:

Upon closer inspection using stat test.xls in Git Bash, it would appear that the change time is modified by Excel along with the bytes on disk, but not the modified time. I fear that the described problem is related to the fact that Git for Windows has to take a couple of shortcuts when trying to emulate Linux' semantics. In particular, when the so-called "stat" data (essentially, all the metadata for a give file) is emulated, we use the FindFirstFile()/FindNextFile() API which gives us the time of the last access, the time of the last modification and the creation time. Sadly, that differs slightly from the POSIX semantics that Git wants to see, where the first two times are identical, but the ctime does not refer to the creation time but the change time. But we do not have a fast way to get at the change time, only the access time, modified time and creation time. We could get the change time, via the ChangeTime field in the FILE_BASIC_INFO data structure initialized by GetFileAttributesByHandleEx() function, but that requires a HANDLE, which we can only obtain using CreateFile() (which is orders of magnitude slower than FindFirstFile()/FindNextFile(). So what Git for Windows does is rely on applications to update the modified time when changing any file contents. But that is not the case with Excel. I fear there is not really anything we can do here, not unless we want to slow down Git for Windows dramatically (in most cases, for no good reason)...

Just FWIW.  
It seem that it depend on the FS used (and the underlying OS drivers for that FS).  
My findings are that:

```
#--------------------------------------
# [a/c/m]time
#--------------------------------------
# On Windows (via Cygwin & Python3):
#   The creation time is:       aTime           .CreationTime === .LastAccessTime in Poweshell, but known as "access" time in Linux)
#   The modification time is:   mTime == cTime  .LastWriteTime in Poweshell
# 
# On Linux:
#   The creation time is:       cTime
#   The modification time is:   mTime
#   The access time is:         aTime           (normally not used)
# 
# ==> For seeing last modification time, use "cTime" on Windows FS's, and "mTime" on *linux FS's
#--------------------------------------
```

IDK why an _Excel_ file would behave different from any other "Windows" generated file, in this respect.  

... plus:

https://community.hpe.com/t5/operating-system-hp-ux/what-s-the-difference-of-ctime-and-mtime-in-find-command/td-p/3341256?nobounce

### Re: what's the difference of -ctime and -mtime in find command?

mtime refers to the modification time of the file, while ctime refers to a change in the status information of the file. For example, you could use the touch command to alter the date of the file (the status information), without actually changing the file itself.  
  
At least that's the way I interpret it.  
  
Pete

------

Pete's right. Even if you can change both change and modification with touch (-c or -m).  
  
You have 3 dates for a file :  
. ctime : change time. It gives you the last time a modification was done on the inode. For example chmod. You can see it with ls -lu file.  
. mtime : modification time. It gives the last time the file content was modified. For example with vi. It is the one normally displayed by ls -l.  
. atime : access time. It gives you the last time the file was accessed. Even cat modifies this date.


### https://nicolasbouliane.com/blog/knowing-difference-mtime-ctime-atime :: Knowing the difference between mtime, ctime and atime

If you are dealing with files, you might wonder what the difference is between `mtime`, `ctime` and `atime`.

`mtime`, or modification time, is when the file was last modified. When you change the _contents_ of a file, its mtime changes.

`ctime`, or change time, is when the file’s property changes. It will always be changed when the mtime changes, but also when you change the file’s permissions, name or location.

`atime`, or access time, is updated when the file’s contents are read by an application or a command such as `grep` or `cat`.

The easiest way to remember which is which is to read their alphabetical order:

- `Atime` can be updated alone
- `Ctime` will update `atime`
- `Mtime` will update both `atime` and `ctime`.
