
# Technology Test

Example to test Windows APIs (and at some later point also Linux and OSX) to obtain information about the global memory usage in system *and* the memory usage of this example specifically.

The goal is to simulate an application which leaks horrible amounts of heap memory and observe the internal monitoring code (at the end of `showMemoryConsumption()`) decide when we've surpassed the *heuristic* thresholds and it's therefor time to quit the application swiftly and brutally.

This is another tech test in conjunction with the fork-me technology test: the higher level goal is to simulate the scenario where a long-living process, divided in a monitor application and one or more "worker childs", can survive and exist without trouble for long hours, when the workers childs suffer from such trouble as (heap) memory leakage, which would be a very tough thing to **guarantee** cleanliness about, particularly while our end product will use significant amounts of foreign and complicated codebases. (Not to speak of our own bug risk: after all, the end product will not have a low LOC, so memory bugs can be assumed to be present. Better to have memory leaks that we can cope with, then.

---

## Observations

- in Win32 build mode, the MSVC debugger shows crazy inaccurate values for the `MEMORYSTATUSEX statex` struct on stack. This behaviour persisted while I changed several compile/build settings re optimizations, etc.
  Printed (`debugPrint(...)`) values did agree with observed external reality, while the MSVC debugger watch and tooltips consistently showed different numbers for this struct's members throughout the execution of the monitor function.

  I have no possible explanation why this is; everything else seemed in order in the MSVC debugger.

  In Win64 build mode (which is our intended target after all), there was no such problem.
