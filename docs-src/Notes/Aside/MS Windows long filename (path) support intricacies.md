# MS Windows long filename (path) support intricacies

An blog triggered by https://github.com/wxWidgets/wxWidgets/issues/25033#issuecomment-2591498688 /  https://groups.google.com/g/wx-dev/c/91UvFJLq_f0/m/2es1UogIAQAJ:

> I'm still in 2 minds about the volume for the `\\?\UNC` paths because, after thinking more about it, I realize that we may need to do it for other prefixes too (although I couldn't find any other ones right now, but it seems like an open system and even if they don't exist, they could be added later) and it may also be quite unexpected to have `\` in the volume.
> 
> And I have another related question: should the volume for `\\?\c:\...` path be `\\?\c:`, as it is now, or `c`, as for the normal paths? The latter seems preferable, but then we wouldn't be able to reconstruct the original path by string concatenation...
> 
> Or maybe I'm thinking about this completely wrong and we shouldn't use volumes at all for this kind of paths? But this would be backwards-incompatible with `\\?\Volume{GUID}` ones.
> 
> All in all I think that perhaps it's best to leave this as is, i.e. let it be as simple as possible because the notion of volumes just doesn't make much sense for this kind of paths anyhow.
> 
> \[....]
>                                     (Vadim Z)[https://github.com/vadz], maintainer of (wxWidgets)[https://github.com/wxWidgets/wxWidgets]

------------------------------------------------------------

Naming a Volume
https://learn.microsoft.com/en-us/windows/win32/fileio/naming-a-volume

You can't delete a file or a folder on an NTFS file system volume
https://learn.microsoft.com/en-us/troubleshoot/windows-server/backup-and-storage/cannot-delete-file-folder-on-ntfs-file-system
--> Cause 4: Files exist in paths that are deeper than MAX_PATH characters

https://learn.microsoft.com/en-us/troubleshoot/windows-server/backup-and-storage/disk-space-problems-on-ntfs-volumes
--> MFT, FRS, Alternate data streams


https://learn.microsoft.com/en-us/troubleshoot/windows-server/networking/slow-smb-file-transfer
Slow transfer when using small files


https://superuser.com/questions/1699190/which-unicode-characters-cannot-be-used-for-ntfs-file-names

https://stackoverflow.com/questions/19499257/opening-mft-file-causes-access-denied-even-if-run-as-administrator


https://learn.microsoft.com/en-us/windows/win32/api/fileapi/nf-fileapi-createfile2

The **CreateFile2** function can create a handle to console input (`CONIN$`). If the process has an open handle to it as a result of inheritance or duplication, it can also create a handle to the active screen buffer (`CONOUT$`). The calling process must be attached to an inherited console or one allocated by the [AllocConsole](https://learn.microsoft.com/en-us/windows/console/allocconsole) function. For console handles, set the **CreateFile2** parameters as follows.

Expand table

| Parameters              | Value                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                             |
| ----------------------- | ----------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------- |
| _lpFileName_            | Use the `CONIN$` value to specify console input.<br><br>Use the `CONOUT$` value to specify console output.<br><br>`CONIN$` gets a handle to the console input buffer, even if the [SetStdHandle](https://learn.microsoft.com/en-us/windows/console/setstdhandle) function redirects the standard input handle. To get the standard input handle, use the [GetStdHandle](https://learn.microsoft.com/en-us/windows/console/getstdhandle) function.<br><br>`CONOUT$` gets a handle to the active screen buffer, even if [SetStdHandle](https://learn.microsoft.com/en-us/windows/console/setstdhandle) redirects the standard output handle. To get the standard output handle, use [GetStdHandle](https://learn.microsoft.com/en-us/windows/console/getstdhandle). |
| _dwDesiredAccess_       | `GENERIC_READ \| GENERIC_WRITE` is preferred, but either one can limit access.                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                    |
| _dwShareMode_           | When opening `CONIN$`, specify **FILE_SHARE_READ**. When opening `CONOUT$`, specify **FILE_SHARE_WRITE**.<br><br>If the calling process inherits the console, or if a child process should be able to access the console, this parameter must be `FILE_SHARE_READ \| FILE_SHARE_WRITE`.                                                                                                                                                                                                                                                                                                                                                                                                                                                                           |
| _dwCreationDisposition_ | You should specify **OPEN_EXISTING** when using **CreateFile2** to open the console.                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                              |


https://learn.microsoft.com/en-us/windows/win32/api/fileapi/nf-fileapi-createfilew

By default, the name is limited to MAX_PATH characters. To extend this limit to 32,767 wide characters, prepend `"\\?\"` to the path. For more information, see [Naming Files, Paths, and Namespaces](https://learn.microsoft.com/en-us/windows/win32/fileio/naming-a-file).



https://learn.microsoft.com/en-us/windows/win32/api/fileapi/nf-fileapi-createfilea




Naming a Volume
https://learn.microsoft.com/en-us/windows/win32/fileio/naming-a-volume

You can't delete a file or a folder on an NTFS file system volume
https://learn.microsoft.com/en-us/troubleshoot/windows-server/backup-and-storage/cannot-delete-file-folder-on-ntfs-file-system
--> Cause 4: Files exist in paths that are deeper than MAX_PATH characters

https://learn.microsoft.com/en-us/troubleshoot/windows-server/backup-and-storage/disk-space-problems-on-ntfs-volumes
--> MFT, FRS, Alternate data streams


https://learn.microsoft.com/en-us/troubleshoot/windows-server/networking/slow-smb-file-transfer
Slow transfer when using small files


https://superuser.com/questions/1699190/which-unicode-characters-cannot-be-used-for-ntfs-file-names

https://stackoverflow.com/questions/19499257/opening-mft-file-causes-access-denied-even-if-run-as-administrator



https://devco.re/blog/2025/01/09/worstfit-unveiling-hidden-transformers-in-windows-ansi/
WorstFit: Unveiling Hidden Transformers in Windows ANSI!

https://worst.fit/


https://github.com/gulrak/filesystem
https://gulrak.net/posts/2019-std-filesystem-corner-cases/
https://github.com/gulrak/filesystem/wiki/Differences-to-Standard-Filesystem-Implementations



Naming a Volume
https://learn.microsoft.com/en-us/windows/win32/fileio/naming-a-volume

You can't delete a file or a folder on an NTFS file system volume
https://learn.microsoft.com/en-us/troubleshoot/windows-server/backup-and-storage/cannot-delete-file-folder-on-ntfs-file-system
--> Cause 4: Files exist in paths that are deeper than MAX_PATH characters

https://learn.microsoft.com/en-us/troubleshoot/windows-server/backup-and-storage/disk-space-problems-on-ntfs-volumes
--> MFT, FRS, Alternate data streams


https://learn.microsoft.com/en-us/troubleshoot/windows-server/networking/slow-smb-file-transfer
Slow transfer when using small files


https://superuser.com/questions/1699190/which-unicode-characters-cannot-be-used-for-ntfs-file-names

https://stackoverflow.com/questions/19499257/opening-mft-file-causes-access-denied-even-if-run-as-administrator



https://devco.re/blog/2025/01/09/worstfit-unveiling-hidden-transformers-in-windows-ansi/
WorstFit: Unveiling Hidden Transformers in Windows ANSI!

https://worst.fit/


https://github.com/gulrak/filesystem
https://gulrak.net/posts/2019-std-filesystem-corner-cases/
https://github.com/gulrak/filesystem/wiki/Differences-to-Standard-Filesystem-Implementations





I am trying to save a file containing UNICODE-characters like "𤽜" or "𤭢", both of which should be stored as surrogate pairs with two 16-bit-wide characters each, by setting the encoding to UCS-2 LE, but the characters are stored wrongly. Even on immediate "Reload from Disk", the then-displayed characters are messed up. After inspection of the generated file with a hex-editor, the storage representation generated by Notepad++ for those characters is too short (only two bytes instead of the required four per character).
If I enter the correct representation via the hex-editor (𤽜 --> 53 D8 5C DF, 𤭢 --> 52 D8 62 DF), Notepad++ displays six (!) bytes each instead of one character, namely

𤽜 --> xED xA1 x93 xED xBD x9C
𤭢 --> xED xA1 x92 xED xBD xA2

UTF-16 was redefined to be ill-formed if it contains unpaired surrogate 16-bit code units.




ADS: alternate data streams specials: quoting from https://learn.microsoft.com/en-us/windows/win32/fileio/naming-a-file
Use any character in the current code page for a name, including Unicode characters and characters in the extended character set (128–255), except for the following:

- The following reserved characters:
    
    - < (less than)
    - > (greater than)
    - : (colon)
    - " (double quote)
    - / (forward slash)
    - \ (backslash)
    - | (vertical bar or pipe)
    - ? (question mark)
    - * (asterisk)
    
- Integer value zero, sometimes referred to as the ASCII _NUL_ character.
    
- Characters whose integer representations are in the range from 1 through 31, except for alternate data streams where these characters are allowed. For more information about file streams, see [File Streams](https://learn.microsoft.com/en-us/windows/win32/fileio/file-streams).

And FileStreams https://learn.microsoft.com/en-us/windows/win32/fileio/file-streams then says:

When creating and working with files that have one-character names, prefix the file name with period followed by a backslash (`.\`) or use a fully qualified path name. The reason to do this is that Windows treats one-character file names as drive letters. When a drive letter is specified with a relative path, a colon separates the drive letter from the path. When there is an ambiguity about whether a one-character name is a drive letter or a file name, Windows assumes it is a drive letter if the string following the colon is a valid path, even if the drive letter is invalid.



UCS2 and filename parts:

I am trying to save a file containing UNICODE-characters like "𤽜" or "𤭢", both of which should be stored as surrogate pairs with two 16-bit-wide characters each, by setting the encoding to UCS-2 LE, but the characters are stored wrongly. Even on immediate "Reload from Disk", the then-displayed characters are messed up. After inspection of the generated file with a hex-editor, the storage representation generated by Notepad++ for those characters is too short (only two bytes instead of the required four per character).  
If I enter the correct representation via the hex-editor (𤽜 --> `53 D8 5C DF`, 𤭢 --> `52 D8 62 DF`), Notepad++ displays **six (!) bytes** each instead of **one character**, namely

- 𤽜 --> `xED xA1 x93 xED xBD x9C`
- 𤭢 --> `xED xA1 x92 xED xBD xA2`

https://unascribed.com/b/2019-08-02-the-tragedy-of-ucs2.html

https://simonsapin.github.io/wtf-8/

[UTF-16](https://simonsapin.github.io/wtf-8/#utf-16) was redefined to be [ill-formed](https://simonsapin.github.io/wtf-8/#ill-formed) if it contains [unpaired surrogate 16-bit code units](https://simonsapin.github.io/wtf-8/#unpaired-surrogate-16-bit-code-unit).



---------------------------------------------------------------

> 😊 Whoops, replied in the mailing the list first. Only then realized my mistake as this is an github issue. 😊 


Oh boy. Long filename/path support in MSWindows: everybody wants it. Anyhow...
(Apologies, joining the party rather late; saw it only yesterday as I lurk in the ML but don't actively use wxW ATM. 😔)


<br><br><br><br>


> ## TriggerWarning
> 
> Grab your support animal, hold him tight and keep one finger on the power switch for emergency screen blanking.
> _You have been warned._


<br><br><br><br>

Several things to note here; this message is a bit of a shot across the bow as writing this / arguing this will take probably another week before I've got any sort of a usable essay done: Win32 `fileapi.h` and UNC is _complex_ stuff. Right now, covering the major pain points:

The fast response to your question "**...I wonder if we should switch to using extended-length [UNC] paths automatically if we detect that the path length is greater than MAX_PATH?...**": tread very very carefully here. 
I know the whole bloody internet is filled with ******** taking the `\\?\` prefix route (including Microsoft's own documentation pages) and that's great for hacky throw-away tools when you need to do _something by yesterday_, but **you** are working on a framework employed by many, so simply: DO NOT. (RFC2116)  
Yes, I have used `\\?\` prefix myself. Extremely dangerous and you will get burned, not a question of IF but merely WHEN.
(_Edit:_ the fact that 7zip & other 'well knowns' are doing it (allegedly; I haven't checked their source tree recently), doesn't make it the Golden Egg. Sorry to disappoint.)



### References:

- Naming Files, Paths, and Namespaces: https://learn.microsoft.com/en-us/windows/win32/fileio/naming-a-file
- Maximum Path Length Limitation: https://learn.microsoft.com/en-us/windows/win32/fileio/maximum-file-path-limitation
- CreateFileW: https://learn.microsoft.com/en-us/windows/win32/api/fileapi/nf-fileapi-createfilew
- mind Cause Nr. 4: https://learn.microsoft.com/en-us/troubleshoot/windows-server/backup-and-storage/cannot-delete-file-folder-on-ntfs-file-system




Using `\\?\` as a sort of _path extender on occasion_ is _plain wrong at a fundamental level_. If you want quick & dirty, you MAY want to consider the `\\.\` prefix instead. The Microsoft documentation spends exactly one line addressing this in an entire article, but the major issue with `\\?\` is that it (paraphrasing here) SKIPS the MSWindows _path normalizer_ code (which executes in User Space) and is immediately routed, in RAW form, to the "designated" Kernel Space device driver which is responsible for your filesystem, e.g. NTFS. (Via the MUP router, it is called IIRC) 

When you read this and think "WTF is he saying?!?!" then read the docs above again and note the very subtle mention of the exact behaviour of `\\?\`, which is also known as the _Global Namespace_, sometimes referred to as _Global?_ or _Global??_ in some Microsoft docs (yes, two question marks) and it DOES mention that you MAY want to use `\\?\` _to access otherwise inaccessible_ directories and files. `\\.\` only gets mentioned by the time everybody is pottytrained to use `\\?\` instead. 🤦

Yes, `\\?\` allows creating/accessing _filenames_ called `.` or `..` (_those are not typos!_) as the `\\?\` UNC path is processed as-is, no `.` "current directory" or `..` "parent path element" operators applied, as those are part of the _path normalization_ applied in User Space, but you are already in Kernel Space, thanks to `\\?\` routing! 

An example everyone can test (no special access rights requires AFAICT): compare the output of these command lines in a `cmd` shell, assuming you have a local drive `C:` and `D:`, as I have on my dev machine:
<sup>(Bracketed numbers added for reference in the discussion below)</sup>

**Bonus points if you think about what you'ld expect to see, before executing each of these lovelies:**

(1)
```
fsutil fsinfo ntfsinfo \\.\c:\
fsutil fsinfo ntfsinfo \\?\c:\
```

(2)
```
fsutil fsinfo ntfsinfo \\.\d:\
fsutil fsinfo ntfsinfo \\?\d:\
```

(3)
```
fsutil fsinfo ntfsinfo \\.\c:\..\d:\
```

(4)
```
fsutil fsinfo ntfsinfo \\?\c:\..\d:\
```

(5)
```
fsutil fsinfo ntfsinfo \\.\UNC\c:\
fsutil fsinfo ntfsinfo \\?\UNC\c:\
fsutil fsinfo ntfsinfo \\.\UNC\d:\
fsutil fsinfo ntfsinfo \\?\UNC\d:\
fsutil fsinfo ntfsinfo \\.\UNC\c:\..\d:\
fsutil fsinfo ntfsinfo \\?\UNC\c:\..\d:\
```

Yes, the groupings were a hint: I expect each command line in a single group to behave exactly the same; at least they do around here. 
<sup>tested on 'American' English Win10; as I love my national language but not as part of a computer interface since the translations are mind boggling, so I run 'mostly American' systems where-ever I can have my way.</sup>

Re group 5: not going to mince words: DOA, the lot of them. Error message's all you're going to get. 
I don't know where you picked up the extra `\UNC\` part of that `\\?\UNC\` you mention (well, I do have an idea; the internet is riddled with it); the only mention of that one that's anywhere near possibly potentially 'official' is in the 'leaked' Windows XP source code of yesteryear that's floating around: there's some code lines in there which check for both `\\?\` and `\\?\UNC\` prefixes. My personal experience is: it doesn't fly in modern OS revisions (Win10, Win11). 
> Over the years/decades I've encountered some hair-raising changes of behaviour around `\\?\` et al after unscheduled reboots (thank you, Windows Update Service; my own stupidity causes a bluescreen, you replace my pine fragrance pendant hanging from the rearview mirror as part of the restart; much obliged. When you're debugging code, that can cause many a WTF?! Where did my bug just go?! Anyhow. _Next._

Group 1: gives you info about your drive `C:`
Group 2: ditto for that drive/volume/partition `D:` of ours: note the differences. If you have two same-sized partitions, there's still the different unique identifier to observe which is which. Remember that one, because...

Group 3: bit of an eyebrow raiser for me ("_huh? `\..\` parent operator works not just for path elements but also for the share/volume element? Wow!_") but this one, thanks to the `\\.\` UNC prefix the `C:\..\D:\` is _normalized_ (resolved) to `D:\`, so you don't get an error for "crazy path spec" but get to see the same output as Group 2. (nitpick by the way: everything that starts with `\\` is an UNC path; `\\?\` just happens to identify the "Global Namespace". `\\.\` is local, `\\domain.server.id\share\` is for SMB and other networked filesystem access.)
"_Great! Got it! We're done!_" you might think (I like lazy too, but here... no no no. Just wait...)

Group 4: WTF?! error message?
Yup. Because, as it says in the documentation (when you read very carefully) there's _no path normalization_ happening for this one, so the filesystem driver in the kernel gets fed `C:\..\D:\` and that one, very sensibly, barfs a hairball. *B0rk!*

"_Ok, great! So we use `\\.\` and we're good to go!_" would be obvious next brain eruption. Uh. Uh.

Here is where things start to *really* take off: Microsoft spends another line of text to mention that MANY file apis in Win32 have been reworked to accept UNC paths and thus offer long filename support.
The take-away of that shortlist is this: as long as you DO NOT use the ANSI Win32 file APIs, you run a reasonable chance.
However, when you test against the ANSI (the A suffix) APIs rather than the W (Unicode UTF16 supporting) APIs, it usually works anyway. The caveat? There's additional transformations and sanitization ("normalization") happening in the A=ANSI ones, allegedly.

😄 As you happen to have impeccable timing with this `wxFilename` work, only a few days ago some folks did a very nice write-up of some relevant CVEs on this subgenre of the Win32 API realm: 

https://devco.re/blog/2025/01/09/worstfit-unveiling-hidden-transformers-in-windows-ansi/ (WorstFit: Unveiling Hidden Transformers in Windows ANSI!)
https://worst.fit/

Also do note everybody's enthousiastic reaction to this, including the pro-active stance of the Redmond Win32 guys 'n' gals. _{sarcasm}_

Is this relevant to wxWidgets? I believe so: at least you will be very subtly (and possibly fatally for some) altering the behaviour of the wxFilename interface et al as you move away from "legacy" path specs for a particular subset of the disk tree, making precise behaviour dependent on actual path length.
If you do, please do use the `\\.\` prefix, use it throughout / whenever you can and stay away from `\\?\` and forget that `\UNC\` bit.

Given that the A variants of the Win32 file APIs are extra-kinky-cactus-in-butt-cheeks vs. the W variants, MAY I suggest considering moving towards using the new-ish `std::filesystem` / `ghc::filesystem` (https://github.com/gulrak/filesystem) software underneath as this thing would require a code review throughout anyway: `fopen()`, `mkdir()` et al are, of course, suspect and they will very probably call `CreateFileA`, `CreateDirectoryA`, etc. under the hood? Not that using `ghc::filesystem` / `std::filesystem` magically solves these woes, but one can reasonably expect the most attentive folks to be busy with making sure the Win32 Unicode versions are used throughout there -- meanwhile I use a `ghc::filesystem` copy in forced Unicode (W) build mode for applications which will be user-facing, though I do know I have occasional `fopen()`, `mkdir()` et al usage lurking in my codebases: it's an ongoing process, alas.

--------------

In a reply to your first question: "**...I don't even know if it really matters, but we should probably consider `\\?\UNC\host` as the volume, shouldn't we?...**", the too-short answer is _maybe, but are you sure you want the consequences_?
 
Technically, you are trying to shoehorn multiple path elements into a single one in your own (volume, directories, file) tuple; my choice would be to take the entire `\\server\share\` part from the UNC path spec as your 'volume', the argument being that that last `\` is the boundary between on-the-same-volume/share path elements which can be traversed using the `\..\` path operator. 

If `\..\` path operator working is the preferred argument instead, than your pick is the correct one, as the `\..\` operator can traverse _shares_ in the UNC path, as shown earlier in the `fsutil` commands (I haven't tested if it also works for the `\server\` element as I'm not near any large enough Windows network right now and won't be for a while. _Someone care to check if `\\.\serverA\..\serverB\share\` normalizes to `\\.\serverB\share\`, please?_ <sup>My bet today is: it does...</sup>)


Pick your poison; personally, I wouldn't want to allow anyone to easily traverse across shares, _assuming I have my own traversal/sanitizer code layered on top of yours/the Win32 API_, but that is a design choice. `fileapi.h` and friends are not holding you back.



-----------------------------------------------------

## Concluding:

- a code review to ensure one is explicitly using Unicode (W) variant file APIs, irrespective of whether the rest of the code is compiled in ANSI or Unicode (MSVC `/UNICODE`) mode, is most useful when considering codebase robustness.
- long filename support? UNC paths? Use `\\.\` prefix, if none has been provided already due to network or device access being attempted.
- _(not mentioned above)_ when using `std::filesystem` et al, which tend to convert to UTF8, keep in mind that it is *legal* on NTFS/Win32 to have file/path names containing _unpaired surrogate 16-bit code units_, which are _illegal_ Unicode codepoints. Compare UTF8 vs. WTF8 encoding: https://simonsapin.github.io/wtf-8/ which allows to transport such legal Windows/NTFS filepaths through a regular `std::string` and alike.




P.S.: while this Win32 stuff is called "_long filename support_" everywhere, I consider that rather a misnomer: it is "_long path support_". (max length: 32K)
 
As we've been bashing the Win32 API quite enough now, here's one from the Linux side of things: last year I had a nasty run-in with some Ubuntu servers, where, it turns out, the maximum _path element_ size is 256 bytes(?) -- I didn't check thoroughly whether it was 256 Unicode codepoints or 256 UTF8 bytes, though I suspect it's the latter. Either way, those filenames had a lot of Chinese characters in them as they were extracted straight from their content: PDF whitepaper titles, some of which had surprisingly long titles.
Once you have Win32 "_long path support_" going, each single element can carry somewhere around 1024-4096 bytes (still to be tested, but I do recall the 4K number, only doubt is the _type_); at least that limit was quite large enough to not create a problem for these Chinese PDFs, while Ubuntu b0rked. *\*sigh\**



HTH












--------------------------------------------------------


Oh boy. Long filename/path support in MSWindows: everybody wants it. Anyhow...

(Apologies, joining the party a little late; saw it only yesterday as I lurk in the ML but don't actively use WXW ATM. [![😔](https://fonts.gstatic.com/s/e/notoemoji/16.0/1f614/72.png)](https://en.wikipedia.org/w/index.php?title=%F0%9F%98%94&redirect=no "😔"))

  

The fast & short response to your question "**...I wonder if we should switch to using extended-length [UNC] paths automatically if we detect that the path length is greater than `MAX_PATH`?...**": ooooh, great minds think alike. Brilliant thought! Done that, been there, got the T-shirt. Only it had this rather peculiar perfume about it... which turned out to be kerosine, bit of a surprising "bzzt" noise and next thing I got is a free trip to the ER, while years down the road, I'm still in recovery today. Turns out I'm a recidivist: the skin-graft medics and me are on first-name basis now.

(_Edit_: the fact that 7zip & other 'well knowns' are doing it, like I have done and probably will do again (recidivist), is us taking the easiest, _laziest_ path for a programmer in a hurry, and is plain _wrong at a fundamental level_. If you want quick and don't give a ****, you MAY want to consider the `//./` prefix instead. The _Why_ should be obvious once you've plowed through this entire message + referenced material. `//?/` is great, but dangerous in its own very special way.

  

Lazy is _good_, but not today. You're providing a generic framework so I would advise to make this a programmer-is-fully-aware explicit choice rather than an automatic one. See below for more, including some fresh CVE.

Whatever you do, there'll be complaints and obscure issues down the road anyway, so I guess conservative and explicit (documentation) is called for in this case.

  

In the section below, I'll also provide a reply to your first question: "...**I don't even know if it really matters, but we should probably consider `\\?\UNC\host` as the volume, shouldn't we?...**", which is regrettably way more complicated than you are going to like. Sorry. 

The too-short answer is _maybe, but are you sure you want the consequences?_ Technically, you are trying to shoehorn multiple path elements into a single one in your (volume, directories, file) tuple; my choice would be to take the entire `\\server\share\` part from the UNC path spec as your 'volume', the argument being that that last '\' is the boundary between on-the-same-volume/share path elements which can be traversed using the `\..\` path operator. if '\..\' path operator working is the preferred argument instead, than your pick is the correct one, as the '\..\' operator can traverse shares in the UNC path (I haven't tested if it also works for the '\server\' element as I'm not near any large enough Windows network right now. On my dev machine `fsutil fsinfo ntfsinfo \\.\c:\..\d:\` does deliver the info for the 'D:' drive/volume. Pick your poison; personally, I wouldn't want to allow anyone to easily traverse across shares, assuming I have my own traversal/sanitizer code layered on top of yours, but that is a design choice. `fileapi.h` and friends are not holding you back.

  

  

  

**TriggerWarning**

Please, dear reader, call your pharmacist, order something stronger than your regular prescription and wait for delivery and the first dose to really hit, before commencing. No, not trying to be funny; you'll need it.

Grab your support animal, hold him tight and keep one finger on the power switch for emergency screen blanking.

_You have been warned._

  

  

  

Bland copycat working title: **What every serious programmer should know about Windows file paths.**

  

Let's start with some official documentation pages by Mrrs. Microsoft, a few blog articles with important & nifty details and if you haven't decided by then to rethink your career plans, welcome to the dungeon! There's some discussion with empirical data, battle scars (stuff going wrong, the infernal WTF and OMG) and, as _you_ (@Vadim) have _impeccable_ timing with this, somebody published a superb CVE just a few days ago that saves me from writing an additional chapter. Documentation first (@Vadim: I'm sure you've seen it before, but I write this also for others who might visit later, traveling along similar paths). Hair of the dog:

  

1. Naming Files, Paths, and Namespaces: https://learn.microsoft.com/en-us/windows/win32/fileio/naming-a-file

  

   advised: initial light reading; revisit and reread with utmost attention to detail after having read nr.2 below: every word here matters. \\?\UNC\ is definitely NOT the same as anything else, though you can reach those same files of 'ol this way too.

  

2. Maximum Path Length Limitation: https://learn.microsoft.com/en-us/windows/win32/fileio/maximum-file-path-limitation

  

   I assume you're awake by now; read carefully and note the little weasley words, e.g. "many" instead of "all" or "none", the latter two words liked and loved so much by any serious engineer. "many" as in "...MAX_PATH limitations have been removed from _many_ common Win32 file and directory functions..." (emphasis mine). Also register that last section titled "Functions without MAX_PATH restrictions", which are a _hint_ for when you're looking for a lazy-_ish_ ladder out the rabbit hole later.  
  
3.   

    

  

3. Project Zero: February 29, 2016, The Definitive Guide on Win32 to NT Path Conversion: https://googleprojectzero.blogspot.com/2016/02/the-definitive-guide-on-win32-to-nt.html

  

   Thank you very much and much respect to James Forshaw for writing this; while it, very sensibly, only addresses a very clearly delineated subset of the subject matter, this material beautifully hints at several troubled spots. As you friendly politician might say: Nothing to worry about, naturally, but you MAY note, f.e., his mention of CONIN$ and CONOUT$ (which were a good reminder for me too, so thank you again), which may seem irrelevant but the mighty $ is coming back, with a vengeance, later on.

  

  

  

  

  

  

  
-----------------------

Yea, I'll be honest dude, you could have kept the argument to a bit less than an diverging rant/essay

> I don't know where you picked up the extra \UNC\ part of that \?\UNC\ you mention (well, I do have an idea; the internet is riddled with it); the only mention of that one that's anywhere near possibly potentially 'official' is in the 'leaked' Windows XP source code of yesteryear that's floating around: there's some code lines in there which check for both \?\ and \?\UNC\ prefixes. My personal experience is: it doesn't fly in modern OS revisions (Win10, Win11).

Uh, `\\?\UNC` is required for server paths not local drive paths, not sure why you are denying this. A unc path to a remote server will not work with just `\\?\`, it requires the prefix `\\?\UNC`. I have tested and validated this a million times deploying this in KiCad. They are even documented by Microsoft.

[https://learn.microsoft.com/en-us/windows/win32/fileio/maximum-file-path-limitation?tabs=registry](https://learn.microsoft.com/en-us/windows/win32/fileio/maximum-file-path-limitation?tabs=registry)  
**The "\\?\" prefix can also be used with paths constructed according to the universal naming convention (UNC). To specify such a path using UNC, use the "\\?\UNC\" prefix. For example, "\\?\UNC\server\share", where "server" is the name of the computer and "share" is the name of the shared folder.**

> If you do, please do use the \.\ prefix, use it throughout / whenever you can and stay away from \?\ and forget that \UNC\ bit.

No, it will not work without \UNC\ for smb servers.

But yes, in the grander scheme of things, automatically switching on MAX_PATH is probably a little risky for some applications. I just wanted wxFilename to deal with the paths properly in the first place for concatenation and other uses more than anything. I can iron fist the other issues away in my applications.




-------------------------


> Yea, I'll be honest dude, you could have kept the argument to a bit less than an diverging rant/essay  
> 😄

> [https://learn.microsoft.com/en-us/windows/win32/fileio/maximum-file-path-limitation?tabs=registry](https://learn.microsoft.com/en-us/windows/win32/fileio/maximum-file-path-limitation?tabs=registry)  
> The "\?" prefix can also be used with paths constructed according to the universal naming convention (UNC). To specify such a path using UNC, use the "\?\UNC" prefix. For example, "\?\UNC\server\share", where "server" is the name of the computer and "share" is the name of the shared folder.

😊 I stand corrected! That'll teach me to triple-check what I write.

Given the specific `\\?\` behaviour, I _never_ use it for network access: there's more peeps than just me being "smart" by, on occasion, constructing abs paths from relative ones by simply pasting, f.e. if you know a relative path to be `..\..\alt-base\xyz` and now you wish to reach it from another machine, you can construct a path like `\\server\share\repos\data\..\..\alt-base\xyz` by simply concatenating a known base and that relative path. This works for these, thanks to Win32 internal "path normalization":

- `\\server\share\repos\data\..\..\alt-base\xyz`
- `\\.\D$\repos\data\..\..\alt-base\xyz`

the `\\?\` prefix makes this a fail as the `\..\` entries are to be taken literally.

I see & use this behaviour in shell scripts where string concat is the easiest to do and let the filesystem handle the cleanup, but it can happen in applications as well, where parts of the path are mixed from different sources (database, user (web) forms, cli).

> No, it will not work without \UNC\ for smb servers.

Maybe, but that is specific for the `\\?\` prefix; I'm addressing SMB servers using `\\domain.server\`, `\\server.dns.name\` (or `\\IPv4/6-address\` if I have to) without the need for that `\UNC\` additional prefix. Added benefit: path normalization, same as for `\\.\`.

The only places I see `\\?\` used where it makes sense is when one is trying to achieve something special, e.g. reading physical harddrive sectors or talking to a particular device driver. (Third option: using `\\?\` path to help rename directories/files that are illegal/troublesome on Windows, while placed there by other tools or when the drive was previously mounted on a rather lenient UNIX system; anyway, it's the same ballpark as the first place: drive/filesystem repair/recovery.)








--------------------------


  

  
@vadz:

> 1. wxWidgets always uses only Unicode API.

👍  (like I said: I'm lurking in the ML, haven't used wxWidgets in a while 😢; good to know, so you and yours are not subject to this CVE: https://devco.re/blog/2025/01/09/worstfit-unveiling-hidden-transformers-in-windows-ansi/ (WorstFit: Unveiling Hidden Transformers in Windows ANSI!) .)

> 2. Extended length paths are created by wxFileName only after normalizing the path, i.e. it's never going to contain any . or .. segments. Knowing this, do you still think it's worth changing the code to use `\\.\` instead of `\\?\` and, if so, why?

Absolutely. While you state you do your own normalization (that's what I read here, anyway), `\\?\` is just adding risk on top that brings little; given that you do your own normalization, specialty applications, such as disk repair/recovery software, should not travel trough wxFilename but use their own, dedicated, I/O interface layer on top of Win32 `fileapi.h` instead anyhow. 

To the *why*:

1. `\\.\x\y\z` path specs have, at the Win32 interface, a very similar (if not *identical* -- I haven't dug that deep, so no _guarantee_ here) behaviour to what a regular Windows user would observe from using Windows Explorer and other 'regular' applications, say Notepad and such.
2. `\\?\x\y\z` is RAW: anything you have in there will be skipping the regular (expected) Win32 API side "normalization" code that sits behind the Win32 File API 'W' (Wide/Unicode) interfaces: "normalization" such as discarding trailing `.` dots in filenames: try these in a `cmd` shell:

```
echo y > t....txt
rem ^^^^^^^ will produce `t....txt`
echo z > t....
rem ^^^^^^^ will produce `t`, NOT `t....`!
dir t*
more < t
rem now for something nasty; you are on Z:\, as I am:
echo nopety-nope > t
echo oh-boy-the-joy-is-great > \\?\Z:\t....
rem ^^^^^^^ will produce `t....`, NOT `t`!
rem HOWEVER, other Windows applications will have a bloody hard time to access this bozo!
rem Try opening it with the famous Notepad++: 👋 toodles! 👋 (it will open `t` instead)
dir t*
more < t
more < \\?\Z:\t....
more < \\?\Z:\t....
notepad  t
notepad  \\?\Z:\t....
notepad  \\?\Z:\t....
rem ... Should be obvious by now. 
rem You may try using Windows Explorer if you are still in doubt. Good luck.
rem ⚠️⚠️Did you notice the different behaviour of more vs. standard notepad?⚠️⚠️
rem ... The defense rests...
```

**The only reason** I can imagine you would pick `\\?\` over `\\.\` is when you want (design/choice) to facilitate UNIX boys to "not worry about anything": `\\?\` is the way to go if you want to accept directory and file names that are *acceptable* on most UNIX systems (I'm looking at Linux here, as that is the major player there these days, and most loudly), such as filenames with `:` colons in them, or other curious characters -- if you, like me, don't come from a UNIX environment, you already learned pretty darn quickly not to try that sort of shenanigans (I was more a VMS guy back in the day and liked that one over UNIX, if only VMS had had UNIX pipes 💌 \*sigh\*).
`\\?\` is also the poison to pick when you want to create/access files with 'reserved names', e.g. `aux.c`. (Yes, I know. 😅 Backwards compatibility to the MSDOS days combined with way too lazy Redford programmers, oh jeez. 🤦‍♀  --> https://learn.microsoft.com/en-us/troubleshoot/windows-server/backup-and-storage/cannot-delete-file-folder-on-ntfs-file-system#cause-5-the-file-name-includes-a-reserved-name-in-the-win32-name-space
, https://learn.microsoft.com/en-us/windows/win32/fileio/naming-a-file#naming-conventions

Quoting:

> Do not use the following reserved names for the name of a file:
>
> CON, PRN, AUX, NUL, COM0, COM1, COM2, COM3, COM4, COM5, COM6, COM7, COM8, COM9, COM¹, COM², COM³, LPT0, LPT1, LPT2, LPT3, LPT4, LPT5, LPT6, LPT7, LPT8, LPT9, LPT¹, LPT², and LPT³. Also avoid these names followed immediately by an extension; for example, NUL.txt and NUL.tar.gz are both equivalent to NUL. For more information, see [Namespaces](https://learn.microsoft.com/en-us/windows/win32/fileio/naming-a-file#win32-file-namespaces).

The argument *against* `\\?\` here is that, while the last couple may read like complete and utter insanity at the OS level (_I cannot agree more!!1! 😆🤮_ :: why didn't they spend the minimal extra effort to check for filename extensions or like, and let those pass, eh?), it is the sort of insanity that you will collide with anyway when using other Windows applications. 

FYI: as a headache/side-project, I'm currently looking into a git repo from someone who has clearly grown up in Linux Land: his code is non-portable thanks to a bunch of directories filled with various files named `aux.c`, `aux.s`, `aux.txt`, `nul.d`, and so on: `git reset --hard` on my dev machine takes 20 minutes (!sic!) to finish, with some fine errors about not being able to recreate those files. 
The question therefor is: do you want to be special and depend heavily on your normalization code, or do you want to follow the usual application path on Windows? Either way, there's suffering to be when folks from other systems come in and expect their behaviour to copy over 1:1 to the new system.

Example nr. 2: run these few commands in a `cmd` or `ps` (PowerShell) terminal on Windows and see what happens -- meanwhile, imagine what these would be doing on UNIX boxes:

```
echo x
echo x > aux.txt
more < aux.txt
type aux.txt
```

(`type` is Windoweze for UNIX' `cat` command)

> To get your sanity to return at least halfway, try the above commands with another filename, say `foo` instead of `aux`.

You (most probably) will have to abort the `type aux.txt` command with Ctrl-C, as it will wait _forever_ on an AUX port that doesn't go nowhere - at least not on my current dev machine. Maybe you have some wicked embedded hardware interface or similar device driver ware installed: then your behaviour may be different of course. 😉 


Also, lastly, please DO note the error reports printed by the `cmd` terminal (the PowerShell is _way_ more verbose):

```
Z:\> more aux.txt
Cannot access file \\.\aux
```

🤔 Hmmm. Maybe Microsoft "best practices" (*cough*) are to use `\\.\`, so maybe wxWindows, pardon, wxWidgets 😉, as a end-user facing application development framework, should do the same? You're the man, so it's up to you. 🙇 


I'm ATM creating a demo git repo with most of the nastiness I have seen and recall / jotted down in my notes, but that'll take a wee bit more time. I know what I want it to do, but coding it isn't a 15' job, plus there non-computing chores to do elsewhere. I'll post a link here when it's delivering legible results.


-------------

While you can argue that most of this stuff doesn't concern you as you have your own sanitizer on top of the Win32 File API, the **fundamental question** is: why skip/ignore Microsoft's own *additional* sanitization/normalization layer, when almost nobody else who's writing Windows end user applications doesn't ignore that one? What are your fundamental design goals: bring as much UNIXy path behaviour as possible to the realm of Windows filesystem I/O, or behave as much as the natives as possible, orrrr some  third option I haven't considered here (e.g. support _both_ modes by switch/config: filtered-normalized `\\.\` & raw-unaldulterated-nasty `\\?\` ? 
















