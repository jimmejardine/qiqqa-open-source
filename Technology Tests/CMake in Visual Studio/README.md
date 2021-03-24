# CMake in Visual Studio

## Purpose

See if we can use CMake easily inside Visual Studio for our C/C++ work. Quite a few libraries we intend to use already offer CMake-based setups.




---

## Motto

This here is part of the technical storyboarding side of a UI & UX overhaul of Qiqqa.

Before we put it to Qiqqa, it will be tested here.



---

## Update

Okay, that was quick. **And a bloody nightmare!**

Turns out that:

- Visual Studio 2019 (latest!) instantly forgets any CMake project you have added in a previous session.
- Visual Studio 2019 (latest!) lets the CMake completely take over. 

  This means you don't have a chance of mixing CMake-based projects with regular MSVC ones in a single solution this way. 

  > The only way out that I see -- and know works -- is to have CMake generate a bunch of MSVS projects plus solution file and then go and include/reference those generated project files in your solution file which is located *elsewhere*.
  
The fact that MSVS yaks about the 'global' `CMakeLists.txt` already existing when you try to add *another* CMake project to a solution, is another hint that this is *utter crap*.

So it's back to MSVS projects as usual and then another build system, whatever it is, CMake or otherwise (`autoconf`, ...) for other platforms. 
Honk this for a dollar. :-((



