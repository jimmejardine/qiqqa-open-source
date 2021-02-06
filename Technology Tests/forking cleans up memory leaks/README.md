Demo / test app to verify that fork() and the windows alternative CreateProcess() do deliver on the heap cleanup assumption.

Also serves as basic testing ground for this fork() vs. CreateProcess() work for producing a "monitor" parent and "worker" childs, which terminate after a while, where the "monitor" keeps / kicks N children alive to ensure round-the-clock service by these "workers".

------

Weirdness: had some trouble with CreateProcess() where it caused total run-away by creating zillions of children, despite parent not obviously looping or being re-invoked, even when the commandline would've been shot to hell. :-S

------

Extra: when an application crashes or simply calls the ExitProcess() Win32 API (or TerminateProcess() API), then we have no recourse for any last-minute cleanup code.

Hence my initial code with a named Mutex was severely flawed: the check must entirely happen at the start of the application and NOT depend on any cleanup code doing its job (or not).

so it comes down to inspecting how many processes are actually running and counting them, making the Named Semaphore solution unnecessary in the process.

The key design criterium here is that the entire check-and-allow-or-abort action MUST be completed at the start of the application, without any need for 'cleanup' later, like, say, a ReleaseSemaphore() call.




From an old Jeff Prosise article:

### Method 3: Named Mutexes

In another method of detecting existing Win32 application instances, the first application instance to be launched creates a shared thread-synchronization object or file-mapping object. Subsequent instances can then determine whether another instance is running by checking for the shared object's existence.

The WinMain function in Figure 3 uses a named mutex to detect previous application instances. A mutex (a contraction of the words mut ually and ex clusive) is a synchronization object that is usually used to coordinate the actions of two or more threads. The threads can belong to the same process, or they can belong to different processes; it doesn't matter, because mutexes are kernel objects and thus reach across process boundaries. Two processes that use the same mutex identify the mutex by name.

FIGURE 3: A named mutex can be shared among applications and provides a convenient means for an application to detect another instance of itself.
In Figure 3, the mutex isn't used for synchronization purposes. Instead, it functions as a flag for detecting previous instances. First WinMain calls CreateMutex to create a named mutex. Then it calls GetLastError to determine whether CreateMutex actually created a mutex or whether it simply returned the handle of an existing mutex with the same name. A return value of ERROR_ALREADY_EXISTS means another instance of the application has already created the mutex; that in turn causes WinMain to display an error message and shut down. Any other return value allows startup to proceed normally. Before terminating, WinMain calls CloseHandle to close the mutex handle. When the last instance of the application calls CloseHandle, the mutex's reference count drops to 0 and the system deletes it, freeing the resources allocated to it.

You can use a variation on the named-mutex method to limit the allowed number of instances of your application to a value other than 1. Suppose, for example, you'd like your users to be able to run up to five concurrent instances of your program. Just replace the named mutex with a named semaphore whose initial resource count is 5, then plug the following code into WinMain:

         static char szSemName[] = "MySemaphore";
         HANDLE hSem;
         .
         .
         .
         // At startup
         hSem = CreateSemaphore (NULL, 5, 5,szSemName);
         if (WaitForSingleObject (hSem, 0) == WAIT_TIMEOUT) 
         {
         MessageBox (NULL, "There are already five instances of this application
                                running", "Error", MB_OK | MB_ICONSTOP);
         CloseHandle (hSem);
         return -1;
         }
         .
         .
         .
         // Before shutdown
         ReleaseSemaphore (hSem, 1, NULL);
         CloseHandle (hSem);

Semaphores are similar to mutexes, but unlike mutexes, semaphores maintain resource counts that indicate when all the objects they guard are in use. The call to CreateSemaphore creates a named semaphore with an initial resource count of 5 if a semaphore of the same name doesn't already exist; otherwise, it returns the handle of the existing semaphore.

Calling WaitForSingleObject with the semaphore's handle decrements the resource count and returns WAIT_OBJECT_0 if the semaphore's resource count is nonzero. If the resource count is 0, WaitForSingleObject simply returns WAIT_TIMEOUT. Each instance of the application decrements the resource count with WaitForSingleObject when started and increments the resource count with ReleaseSemaphore before terminating. Thus, WaitForSingleObject returns WAIT_OBJECT_0 for the first five application instances. But if a sixth instance is started, WaitForSingleObject returns WAIT_TIMEOUT and the application quits. It's important to pass WaitForSingleObject a time-out value of 0 (parameter 2), or else calling it when the resource count is 0 will block the calling thread until either the time-out period expires or an existing instance of the application is terminated.

---

From MS forum:

Get the number of instances with:

      int count = Process.GetProcessesByName(Process.GetCurrentProcess().ProcessName).Length;

Of course, the returned count was only accurate a little while before you got the count.  Processes might have exited by the time you get to use the number.

---

so a Named Semaphore may be useless, but a Named Mutex might be handy to have: that way we can guarantee that the process counting (done inside the critical section) is on the money: all counted instances were still running at the time of the counting!

---

## State of Affairs / What's happening in the example code

\[Taken from previous commit message:]

adding Technology Test sample/test app: checking forking cleans up memory leaks.cpp() on Windows. Turns out that's a no-go (been away too long from Win32 API work; had forgotten that bit :-( ), so sample code checks behaviour what to do for a self-monitoring Windows executable. 

Turns out debugging is a bastard as you're hard up against the wall to debug the *child* that's invoked via CreateProcess. 
Besides, the initial sample code grabbed off the Microsoft site had the nasty "side effect" of causing infinite instantiations when patched that way, so the conclusion for the future is: better to have a separately named 'monitor' application, which keeps N child applications afloat.

Example code is still a mess, as all things that were looked at, include Counting Semaphores to help keep track of the number of child instances, etc. is all still in there.

### Conclusions:

- name monitor and child apps differently, so you can have the solution open in TWO MSVC instances and debug the monitor in one, while debugging the child in another.
- do NOT use Counting Semaphores (or any other 'obvious' stuff) for keeping track of the number of children alive: when they *terminate*, through Win32API ExitProcess, TerminateProcess or hard crash, ANYTHING that's not `exit(X)`, you're toast as the application WILL NOT invoke class destructors or any handlers you can hook into when that happens, resulting in the **inability to guarantee** that any semaphore signaling will be matched by appropriate release at end-of-child-life.

  Consequently, the only viable way to make sure you can count the children alive at point of decision to kick one more alive or not is to use a system which **completes entirely** at start of child app: the way this is done here is to use a Named Mutex (all we need is a critical section, no counting done at this level, so not a semaphore but a mutex instead), which protects a critical section in all parties involved, where the OS is queried for a processes snapshot (a la `ps -ax` if you're into UNIX) and then scan the executable names for a match. Once that scan is done, the critical section ends and the Mutex is released, so we CAN guarantee proper global mutex handling now (as long as our OS process scan code doesn't *crash* ;-) )

  Then, when the count of 'live children' is high enough, the creation of yet anotheer one is skipped.
- extra lesson: Named Mutexes and Named Semaphores on Win32/64 can have a 'Global\' prefix, if you read their API docs. DO NOT DO THAT. Turns out that the verbiage at the Microsoft site is not clear enough for *me*, at least, to grok that this prefix is ONLY legal when running Terminal Services, which is Windows Server stuff and I bet you're not running research UI applications on a Windows Server license, surely!  ;-P

  That bit took another couple of hours off my life. !@#$%^

- extra lesson: DO read ('empty') your child's STDOUT pipe CONTINUOUSLY and RIGOROUSLY, when you've redirected its stdio to you, the parent/monitor. If you don't (and the original Microsoft sample code didn't because it didn't have to, as it didn't have any "debugging printf statements" in there! !@#$%^) to child process will BLOCK, waiting indefinitely for the parent to finally do some ReadFile(handle) work and empty the buffer.

  In the current example code, this has been resolved by kicking an extra *thread* alive in the *monitor* (`ThreadProc`) which' sole purpose is continuously waiting for stdout data from the child. 

  I could have gone the whole hog and written asynchronous I/O code for that instead, but this was done faster, WORKS, and wasn't the main subject at time of writing.

- extra lesson: call the `FlushFileBuffers()` API on your parent/child pipes (redirected stdin + stdout): that *also* wasn't in the original sample code from MS, but is absolutely mandatory if you don't want to wonder why the Heck your collected log looks brutally truncated on app exit.

  The MS sample code got away with it, because it matched write and read buffer for buffer, so that really is an edge case of using redirected stdio.

- use OutputDebugStr to *really* printf-debug your code, because the child's printf() is, of course, *redirected*, and if you're debugging pipes it DOES NOT help when you're first trying to fill & choke the buggers only faster: you won't see nuttin', only a stuck bunch of applications. ;-)

  Use [SysInternals OutputDebug monitor UI app (Debug64.exe)](https://docs.microsoft.com/en-us/sysinternals/downloads/debugview) for this as MSVC *only* grabs `OutputDebugStr` output from the executable currently being debugged: that being only one half of the story underway, you will quickly understand that other means to read your debug output are mandatory here.

- extra lesson: sharing handles sounds *fun*, but that did not pan out for sharing Event/Mutex/Semaphore handles, at least not for me. (Never have done *exactly* that before in my life and some deep searching on the InterWebz, wading through the cruft out there, it **sounds** like that's only going to fly, *maybe*, when I set up custom ACLs (yech! been there. Done that. Never again. Thank *you*.) **or** I may be barking up the wrong tree there *utterly* and can *somehow* pass these handles I wish to share with the CreateProcess-ed child via the ProcessInfo block or there-abouts. That's just a very vague notion I have now, while writing this, but haven't seen ANYTHING on the InterNeddz suggesting this might fly for arbitrary handles *outside* the stdio set...

  Anyway, point being: use Named Pipes, Named Mutexes, etc. 

  Yes, those can be 'hacked' - or rather *hyjacked* by malicious apps, so tread carefully and don't attach the soul of your first-born to a Win32 API mutex or anything.

---

\[Edit]

fork() emulators on Win32:

- https://github.com/kaniini/win32-fork 

  On revisiting after the work doen above: FORGET IT. It's using undocumented WIN32 APIs and here's https://social.msdn.microsoft.com/Forums/en-US/4ade520c-0899-4600-9b80-6202d87c16ef/rtlcloneuserprocess-behavior-changed-after-build-14393
  to *appreciate* that. DIW.

- https://github.com/wrenchonline/win32_fork_

  Does some pretty hairy things, haven't tested it. After finding out there's no official `fork` in Win32 (after reading it, I *did* remember it. sigh. Webservers and how to implement them in classic UNIX vs classic Win: fork vs async...)
  I merely had this around as a copy to possibly look at in case of despair. The despair was there, but only after having gone through the ordeal above myself did I understand again what's going on here.
  
  Not what I'm looking for, and while my own code is flaky in a few spots, this one has hairs in other places where I do not want them, particularly after having tested my own code and the Win32 API behaviours on the dev box.


Note about my wondering about which handles to share and how:

See these repo's for sample code. The key bit for *sharing* is this for every handle:

	SECURITY_ATTRIBUTES sa;
	//
	// Create two inheritable events
	//
	sa.nLength = sizeof(sa);
	sa.lpSecurityDescriptor = 0;
	sa.bInheritHandle = TRUE;  <-- !!!
    ...
	__hforkchild = CreateEvent(&sa,TRUE,FALSE,NULL); <-- for example

and then there's some macro fancy fencing, but AFAICT the handles are passed across via `WriteProcessMemory()`. Takes some extra footwork with Events to make sure that pans out apparently. Not my fancy.

Particularly since I notice that *they* use Named Mutexes (okay, *Named Events*) as well, so why bother if that's the basic signal I need? 
I'm not planning on doing (or needing) a 100% fork() port, that having gone out the window as soon as I read & recalled the fork() trouble of old on Windows. 

Basically it's just another fancy `execvp()` or whatnot, so the cost is very probably comparable and that was what all this fuss was about initially: 
a cheaper way to kick off children/processes such that I can stop worrying about heap corruption and **heap leaks**. 

So far, the conclusion there is that I'm stuck with basic CreateProcess("path-to-exe") costs any which way, so it's back to the drawing board to reconsider how I'll do those children again: 
definitely NOT as a one-child-per-task, but probably more like a run-until-we-are-buggered single multithreaded one, which gets a monitor as parent to make sure baby is kicked alive as soon as it goes b0rk-b0rk-bork.
Then we can do extra-fancy stuff like monitor OS-level reported memory consumption by ourselves and kill ourselves (the child, that is) via ExitProcess() Win32 API (which is pretty brutal) so all socket handles and stuff get released by the OS on our demise and us not having to wait for some possibly horribly long 'cleanup' process in `exit()` phase. The monitor will be able to observe our demise (TODO: that is not yet coded into this technology example!) and kick us back alive with another CreateProcess() once we're found absent & demised.
(Hm, do that with a bit of a delay in between attempts, until our target child count is back up to max: that CreateProcess MAY start a fresh client, but *that* client MAY bug out if there's anything wrong with the important sockets and stuff it wants to bind to at startup... Just a thought...)

