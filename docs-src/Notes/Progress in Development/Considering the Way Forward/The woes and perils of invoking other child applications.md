# The woes and perils of invoking other/child applications

* Windows doesn't do `fork()`. 

* *`execv()` et al do work... *kind of*. It's when you want to receive or send data via stdio (`stdout`, `stderr`, `stdin`) that things tend to go pear-shaped in a hurry.

  It's *then* that `execv()` & friends are sorely lacking.
  
* So we need a library to cover these eventualities, which are very much desired in the Qiqqa use case: sub-programs (such as `QiqqaOCR`) output their logging info to `stderr` (which we wish to receive and pipe into our main application's log channel(s)!), while important content data is sent back to the main app via `stdout`, hence we **need** a rig which can receive both these streams independently.

  As we expect to encounter this problem in multiple places, we feel a library would be good for our DRY: one lib to serve them all.
  
  > Current *old Qiqqa* calls `pdfdraw.exe`, `QiqqaOCR.exe` and a few others, all of which behave ever so slightly different when it comes to their stdio streams, probably because those executables have been coded in different languages + run-time libraries.
  > 
  > While we *envision* the *future Qiqqa* to be very much IPC/socket oriented -- and thus foregoing this entire problem area as we would then send and receive everything through `localhost` *local loopback* IP streams instead of stdio -- there's still a need for this while we transition. And very probably afterwards as well, when people start using the different Qiqqa components for their own customized purposes.
  > 
  
* `stdin` turned out to be another horror as several previous attempts at solving this ourselves in C#/.NET produced deadlocks and other serious crap. 

  The reason for this, it turned out, is the limited stdio pipe buffer depth on MSWindows and the dire need to fetch/flush that buffer **asynchronously** at speed, unless you want your (child or parent) application to deadlock on the data exchange across `stdin`. By the way: we only encountered it there first. The same goes for `stdout` and `stderr`, but those didn't show their wiles this early because it's the Qiqqa main application that has to manage a Windows message queue live cycle to keep things flowing, while the child applications are all of the brutal/basic console-app kind: less chance to *eff up* on that side. Or so it seemed.
  
  **Do note** the red remarks in the Python `Popen()` API documentation: https://docs.python.org/3.10/library/subprocess.html -- they have struggled with those same problems I did/do, e.g.:
  
  > **Warning**: Use `communicate()` rather than `.stdin.write`, `.stdout.read` or `.stderr.read` to avoid deadlocks due to any of the other OS pipe buffers filling up and blocking the child process.

  Of course that quote forgot to mention that the trouble is also highly dependent on what your child process does to *read* (`stdin`) and *write* (`stdout`, `stderr`) those streams: generally there will be a relationship between incoming `stdin` data and `stdout`/`stderr` responses and that'll give you some serious headaches if your side isn't prepared to grab those `stdout` and `stderr` responses in a fully *asynchronous* manner.
  
* Of course, trouble never travels alone: I also had some very obnoxious issues re getting hold of the child process' **exit code** and/or precise moment when the child process has indeed fully terminated: I know I still have bugs lurking deep down in the current C#/.NET code handling child process invocations.

  I've spent quite some time on those and the conclusion there is that things are too opaque to perform the very detailed problem analysis that these issues require: *that* is one of the reasons why I intend to migrate the whole caboodle to local-loopback IPC (sockets): that approach may be a tad slower, though from what I've gathered from the few benchmarks old and new floating around on the *IntrNetz* the performance is probably *on par*. Heck, as long as the trouble is **not** "on par", I'll be a happy camper!
  
* As far as I'm concerned, any stdio + child process invocating will, from now on, only be done in a precisely controlled environment that's -- at least in principle -- cross-platform: a C/C++ based library. And no P/Invoke, either. I've had it with those! 

  It's all cute, until you realize the costs involved when you start pumping significant amounts of data and/or large data chunks across that marshaling interface. Total revisit of my old COM/DCOM nightmares. Such corporate crap. 
  
  Hence: *sockets*. Portable. Fast. Ubiquitous. Well behaved.

* Another fun problem (which shows up with the new `subprocess` C++ library I'm testing now): `echo` and similar executables produce a run-time error: *application not found*.

  This stuff gets weirder by thee second as my standard `bash` console is also a little off: `where echo` doesn't deliver (and *be reminded* this is bash on **MSWindows**, so *no* `which` but `where` instead: my `bash` doesn't have a `which`!) Meanwhile, running a dosbox in that same `bash` console via invoking `cmd` and *then* typing `where echo` does deliver a proper full path to `echo.exe`... WTF?!
  
  Turns out my dev box is tickling a *limitation* in Windows re merging *System* PATH and *User* PATH: https://stackoverflow.com/questions/21269171/does-echo-path-expand-to-only-the-system-or-also-the-user-variables/21269570?noredirect=1#21269570 --> when your (*expanded*) PATH hits 1920 characters, the *User* part doesn't get mixed into the `PATH` environment variable.... yet somehow `cmd` + `where` happen to properly locate `echo.exe`, while `bash` + `where` cannot, because that particular search path *happens to sit in my **User PATH***!
  
  > Also note https://superuser.com/questions/133263/windows-7-user-specific-path#133459, which discusses what you might want to do when you're specifically looking for that *User PATH*: go straight for the registry and grab it there!
  > 
  > > Now I looked for the documented registry entry for the *System PATH*, but apparently my Google Fu fails me there... *oh, wait, here's a hint:* https://stackoverflow.com/questions/35246896/adding-a-directory-to-the-system-path-variable-through-registry#35248331

* Another problem, which is also described near the bottom at the CPython RTFC references: Windows child processes still not having sent all stdio stream data while they are already signaled as *terminated*. *Urgh!*

  > Windows accumulates the output in a single blocking read() call run on child threads, with the timeout being done in a join() on those threads. communicate() _after_ kill() is required to collect that and add it to the exception.



## Reference material

- https://docs.python.org/3.10/library/subprocess.html -- we may also take a look at CPython there: *RTFC* ;-) to see if we need any tweaks we might otherwise have missed.
- I picked a `subprocess` library project, but there are several of similar state and design. Unfortunately, none of them does everything right out of the box on my dev machine, so we'll need to mix & match. And **test & debug**. **A lot**.
	- x
- PATH issues on Windows: 
	- https://stackoverflow.com/questions/21270267/system-versus-user-path-environmental-variable-winmerge-works-only-if-i-add-th 
	- https://stackoverflow.com/questions/21269171/does-echo-path-expand-to-only-the-system-or-also-the-user-variables/21269570?noredirect=1#21269570
	- https://superuser.com/questions/367194/prevent-windows-system-path-from-being-prepended-to-user-path
	- https://superuser.com/questions/615209/access-modify-the-user-path-variable-not-system-path
		- https://stackoverflow.com/questions/3304463/how-do-i-modify-the-path-environment-variable-when-running-an-inno-setup-install
	- https://superuser.com/questions/133263/windows-7-user-specific-path#133459
	- https://stackoverflow.com/questions/35246896/adding-a-directory-to-the-system-path-variable-through-registry#35248331
	- https://www.opentechguides.com/how-to/article/windows-10/113/windows-10-set-path.html
	- https://docs.microsoft.com/en-us/windows-server/administration/windows-commands/setx
	- https://social.technet.microsoft.com/Forums/en-US/4e925dd2-6054-4eec-b32f-556b4bec84a1/admin-user-cannot-change-path-entries-on-windows-10 -- for those rare occasions where you need *administrator*-level access to *el reg*: 
	 
	  > To enable the administrator account, type the command `net user administrator /active:yes`
- https://ss64.com/nt/where.html
- See also commit SHA-1: b0ed55110591e3c0a8437a99eb0c8acfe2027312 in our `MuPDF/thirdparty/owemdjee/subprocess/` submodule: that's where we've been testing & dealing with this stuff in the *new setting* (C/C++ library instead of C#/.NET code for Qiqqa+QiqqaOCR).
- repositories:
	- https://github.com/benman64/subprocess (forked)
	- https://github.com/sheredom/subprocess.h
	- https://github.com/arun11299/cpp-subprocess
	- https://github.com/eerimoq/subprocess
	- https://github.com/eerimoq/subprocess
	- https://github.com/lbartnik/subprocess
	- https://github.com/rajatjain1997/subprocess
	- RTFC / Investigate also these: 
		- https://github.com/bgschiller/cpp-subprocess
		- https://github.com/tomtzook/subprocess-c
		- https://github.com/polysquare/cpp-subprocess
		- https://github.com/pnappa/subprocesscpp
		- CPython:
			- https://github.com/python/cpython/blob/f4c03484da59049eb62a9bf7777b963e2267d187/Modules/_posixsubprocess.c
			- https://github.com/python/cpython/blob/4141d94fa608cdf5c8cd3e62f7ea1c27fd41eb8d/PC/launcher.c -- also note the PostMessage()+GetMessage() hack in there, BTW.
			- https://github.com/python/cpython/blob/f4c03484da59049eb62a9bf7777b963e2267d187/Modules/_winapi.c
			- https://github.com/python/cpython/blob/f4c03484da59049eb62a9bf7777b963e2267d187/Modules/mmapmodule.c
			- https://github.com/python/cpython/blob/f4c03484da59049eb62a9bf7777b963e2267d187/Modules/errnomodule.c
			- https://github.com/python/cpython/blob/f4c03484da59049eb62a9bf7777b963e2267d187/PC/winreg.c
			- https://github.com/python/cpython/blob/f4c03484da59049eb62a9bf7777b963e2267d187/Modules/clinic/_winapi.c.h
			- https://github.com/python/cpython/blob/f4c03484da59049eb62a9bf7777b963e2267d187/Lib/test/test_popen.py
			- https://github.com/python/cpython/blob/f4c03484da59049eb62a9bf7777b963e2267d187/Lib/multiprocessing/popen_spawn_win32.py
			- https://github.com/python/cpython/blob/f4c03484da59049eb62a9bf7777b963e2267d187/Lib/multiprocessing/popen_forkserver.py
			- https://github.com/python/cpython/blob/f4c03484da59049eb62a9bf7777b963e2267d187/Lib/multiprocessing/popen_spawn_posix.py
			- https://github.com/python/cpython/blob/f4c03484da59049eb62a9bf7777b963e2267d187/Lib/test/test_subprocess.py
			- https://github.com/python/cpython/blob/f4c03484da59049eb62a9bf7777b963e2267d187/Lib/test/libregrtest/win_utils.py
			- https://github.com/python/cpython/blob/f4c03484da59049eb62a9bf7777b963e2267d187/Lib/asyncio/windows_utils.py
			- https://github.com/python/cpython/blob/f4c03484da59049eb62a9bf7777b963e2267d187/Lib/subprocess.py :: note the `communicate` after `kill` comment near line 508: that's another of the troubles we ran into ourselves: *sometimes*, some `stdout` / `stderr` data is still pending (as in: *not yet received*) when the child process has already been signaled as *terminated*. That further convoluted our C#/.NET code. _!@#$%^&U\*\*_
			- https://github.com/python/cpython/blob/f4c03484da59049eb62a9bf7777b963e2267d187/PC/frozen_dllmain.c
			- 
  
  
  
## Updates (further findings)

### 32-bit builds in a 64-bit OS environment

Turns out 32-bit vs. 64-bit is also relevant: https://stackoverflow.com/questions/2909527/i-cant-access-certain-subkeys-in-an-entry-in-the-registry

Ran into this while testing the subprocess C++ library and trying to find out what was going wrong with the search PATH. :-(((   Turns out the `Computer\HKEY_LOCAL_MACHINE\SOFTWARE\...` registry keys are largely hidden ("*sandboxed*") for 32bit API callers when the applications listed there are 64bit themselves. *Dang!* :bomb:

This is probably related to the fact that you can't invoke/start a 64bit application from a 32bit one: 
see also [[The Transitional Period#Tackling the transition from 32bit to 64bit]].






### `echo` and similar tools only have a binary (executable) in MSYS / GitForWindows

There's no `echo.exe` in a default Windows install. There *is* one in the GitForWindows `bash` environment though, but you won't ever reach that one in a plain vanilla console or other application, unless it somehow inherits the MSYS/bash environment settings.

Discovering the GFW 'root directory' path is another fat effin' problem: there's no documented way to get that path (so you can add `/bin`, `/usr/bin` or `/usr/local/bin` to it to help *discover* the executable you seek) so a wide Registry scan over here revealed the following registry key as a *probable* for this:

```
Computer\HKEY_LOCAL_MACHINE\SOFTWARE\GitForWindows (+ key: InstallPath)
```

but attempting to grab that one will get you an API error when you do that in a 32bit build.

> By the way: there's also this candidate (when you have that software installed, which would also provide `bash` & friends):
> 
> ```
> Computer\HKEY_USERS\.DEFAULT\Software\TortoiseGit (+ key: MSysGit)
> ```
> 

