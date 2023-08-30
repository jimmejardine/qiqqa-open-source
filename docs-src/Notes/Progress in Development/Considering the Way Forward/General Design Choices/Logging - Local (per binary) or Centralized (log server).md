# Logging :: Local (per binary) or Centralized (log server)

Pros and cons:

- Pro Centralized: from an Administrator perspective, I'ld like to have all my logging collected in a single place.
	- Added benefit: all logged events are nicely sorted by time, i.e. in general order of occurrence, as they land in the log file. This means I need little or no tooling to analyze processes which span multiple processes.
	- One Server To Collect Them All also implies I won't have to bother with sharing or duplicating log rotation and log backup configurations. It also means I can perform **ad hoc extractions and archivals** (i.e. file copies) without much fuss and no risk of having missed some bugger's log output somewhere.
* Pro Local: no working *localhost* socket connect and *listening server* required for the log lines to land on disk and persist beyond crash / termination.
	* This implies that the individual parts (applications / binaries / processes) can easily be (re-)used in an environment where the *listening log server* is not (yet) present, aiding testing and hackish custom activities involving only a few members of the entire crew.
	* No worries about potential intermittent errors where the *listening log server* is dropped/killed, even if only temporarily: as we DO NOT use a *log server*, we don't have the added (minimal) risk of a *permanently live* local socket and working log server code.
	* Ditto for risks about laggard or severely bottle-necked or throttled (*log server*) processes, while our application process code is running full tilt and swamping the CPU cores. Or other ways you can get at a severely overburdened system where important processes start to cause significant waits due to CPU core unavailability.
	
	We can manage thread priority *locally* (within our current process) and chances are that no matter the scheduler overload, wee'll get a decent *logging thread performance in step with our main, log-generating process(es)*.
	
	* Corollary of the above: Local Logging has (*theoretically*) better *performance characteristics* then any centralized system: 
		* **any** *gathering* method, be it localhost *socket*, *pipe*, *memory-mapped I/O* or anything else for that matter, will come at a non-zero cost in communication time / effort and will *almost always* involve one or more extra in-memory message copy actions, thus also having a non-zero *added storage cost* compared to the Local Log approach.
		 
		  This, of course, assumes the interleaved disk I/O and file system *fragmentation* for Local Log systems, due to multiple processes writing log messages to disk simultaneously, is a lesser cost than the added costs for the Centralized server log system discussed above.

		  > In actual practice, I believe this assumption will hold **iff** we agree to use *buffering* for our log output to file(s): once the multiple local process loggers start to put pressure on the disk / file system like that, we'ld be better off with a central system as that would *not* be hammering the disk like that. Or so the thought goes, at the moment.
		 > 
		 > Of course, this is just conjecture based on previous experience with other systems and should, ultimately, be verified or rejected through rigorous benchmarking of both approaches on various hardware / OS platforms.
		 > For now, however, I'm still leaning towards to Local Log approach as my primary criterium here is **least cost in CPU and memory pressure**.
		

Pro for one is Con for the other option above, hence no need to list Cons for those, unless not obvious from the above:

* Con for Centralized: as we realize this approach might be more costly to make *highly performant under load* in terms of picking and implementing properly selected IPC mechanisms to ensure the log messages will travel quickly at minimum CPU and memory cost -- while not forgetting the *up-time/stability requirement* for the central log server -- the *overall development cost* for the centralized system can be assumed to be *higher* as well: making a (slightly) more complex system work *smooth and fast* is never cheap.


## Any preference?
I'm leaning towards to Local Log approach as my primary criterium here is **least cost in CPU and memory pressure**.

The Central Log Server approach has a few benefits, which I really like, but the perceived cost of arriving at a fully operational and rock-solid log core is rather higher than I am okay with, right now. Which leaves me to consider the Cons for the Local Log solution such that we have a decent alternative solution for the (administrative / timeline diagnostic) user activities so the absence of a *centralized log server* is not felt as a lack of the overall system any more.

Walking through the "pros and cons" list above, we discuss the *Pros for Central* and how we might go about turning those around so that we can offer those benefits from a Local Log environment:

- "*all my logging collected in a single place*": tooling can provide this function by mixing the relevant log files from the various processes and mixing and sorting them into a single log timeline output.

  > Q: How is this cheaper than having a *Central Log Server* anyway?
  > 
  > A: These tools just need to work. They don't have to bee guaranteed rock solid in a 24/7 deployment setting like said *server*, so we can be a little more forgiving when it comes to obnoxious programming matters such as (minor) memory leaks, etc.
  > 
  > Lower run-time quality requirements translate to a lower *minimum required effort* to make these tools *ready for production use*.

  * "*all logged events are nicely sorted by time*": as wee are running all processes on a single machine anyway, we'll have easy access to timestamps which are *guaranteed to be in sync with one another*, so that reduces the problem to a *sub-second sequencing* one: when log messages are produced within the same second, having access to *milliseconds* WILL NOT bee enough to reconstruct a proper timeline, particularly when log levels are dialed up to *max* and the various processes start to churn out tons of messages per second.
    
    *The idea* here is to use the ubiquitous CPU clock cycle counter which is present in all modern hardware, easily and *cheaply* accessible to any user application code for benchmarking/execution-timing purposes, and SHOULD be usable by us in multiple processes to answer which log message came first on a peer-clock-tick time-base.
    
    Only trouble there is I've not seen anybody do this yet and do not know **for sure** that thee CPU tick counter is system-unique, i.e. **shared among all processes**: after all, I need a tick counter which is more or less smoothly moving *up*, irrespective which process happens to query its current value at any time.
   
    Added challenge there then is to relate those CPU performance counter values to real-time timestamps in such a way that we are, at least without our own bundle of processes/applications, very clear and unambiguous about wall-clock time versus CPU performance counter progress. Heat-throttled CPU cores, which auto-reduce/auto-throttle their core clocks, etc. do come to mind: this target is rather *fluid under load* and thus hard to test & verify in lightly loaded developer rigs. *Cave canem*.
   
  * "*bother with sharing or duplicating log rotation and log backup configurations*": alas. No way around that one, really. All applications can start by using the same general logging facility library code and shared log configuration file(s), while the archival & ad hoc snapshot actions can be assisted by added tooling, but fundamentally there's no way around this one. We'll have to live with this one.
   
    > Consequently, we also have to deal with those configurations becoming altered for various processes in our collective, when the user feels the need for such alterations. Alas. 
    > 
    > **Do note**: while the *Central Log Server* concept would require additional tooling to *extract* certain processes' log messages from the collective log, we need the opposite: wee need tooling to mix & mash individual processes' log messages.
    > This includes dealing with requested older log messages **not existing any more** when any log rotation / archival configuration anywhere has decided to ditch those messages previously. Hence our tooling should report such facts and warn users about such losses of completeness in the derived log outputs thus generated.
    


After writing this analysis, I *still* believe thee smarter route is to provide per-process Local Log facilities, preferably using a single log library or group of libraries and configuration, above having a (dedicated or OS-provided) Central Log Server. 

Yes, we need to provide various tools to re-process our log file collective, but that can be handled relatively easily as such tools would be "one off behaviours" thus we won't need to worry about slow memory leaks, etc. overmuch, making development of such tooling relatively easy.
It also means we can add or augment such tools with ease, without having to go through a rigorous stability testing round after changes have been made.

