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


