# Stability։ back end (local server component)

A few thoughts that drive the Qiqqa toolkit design re run-time stability:

## 1. Having a *long-lived*, always-available server while server-side process memory leaks are *expected*.

### *What problem*? 

The problem is how to architecture your server, which is meant to perform all the tough, *complex* and hard work in the background, to behave as "_always **on**_" when said server will be using some pretty complex third-party libraries, which *will*, *surely*, one day exhibit *heap memory leaks*, if they don't already today.[^1]

[^1]: we are doing our best to both select clean libraries and hunt down memory leaks, both large & small, with extreme prejudice. Despite that effort we *know* some library parts leak a bit of heap, even if it's only a small bunch of bytes at a time (e.g. 24 bytes). And that will ultimately swamp our machine, both in RAM consumption (as this also risks increasing fragmentation) and CPU (the heap manager slows down, even a little, for increased numbers of allocated chunks, so the heap allocation & release activities will slowly increase cost, slowly dragging our machine down).

### Q: Why don't the easy answer everyone else uses work here too?

### A: because...

... there are a couple of technically contradictory design requirements in play. 

Here's the design criteria brief:

- *preferably* don't use stand-alone executables (applications) or *processes* for the individual steps in a workflow: 
  **Rationale**: many such executables (wrapping the requisite libraries) have *severe*  start-up/initialization costs. Those costs ($C^{\text{init}}$) will impact processing performance, so the large costs should be *amortized* across multiple actions. Hence the use of a server-like approach, where a component is kept alive, waiting and swiftly responding to similar processing requests.

  - it is only *okay* to use stand-alone executables for some part of a process, when that process is user-scripted and the user script uses its own customized/user-provided tools to use & invoke. I.e.: we want a *streamlined* system for the default case / product, while we should anticipate some users going for their own thing entirely instead.
  
- *preferably* provide a single point of *initial* contact, using regular communication means (sockets, HTTP+REST, that sort of thing) available and *usual* on most/all platforms (Linux, Windows, Mac)[^2]  we intend to supply.

- design the server architecture in such a way that we *safely tolerate* relatively large amounts of heap memory leakage in the server components: we and/or third party library producers MAY cause heap leakage to occur for some or all usage scenarios and our server architecture MUST be able to cope, while showing a *stable*, *single point of contact* to the user scripts / applications. 

- MUST run on moderate consumer hardware.
- MUST NOT require medium or advanced IT / administrator knowledges and/or capabilities user-side.
- MUST accept and anticipate both *batch mode* and *single item* processing requests. The *single items* are for when the user uses the GUI to browse or otherwise look at individual documents, possibly adding selected items to the library as they occur and pass vetting. Thee *batch mode* is necessary to properly cope with the background update processes, where, f.e., a user imports an entire library, or drops a large number of documents in the watched disk directory to have them imported (added) to the library, or requests to re-index or otherwise bulk-reprocess the entire library after, perhaps, adjusting some indexing preferences or similar.

- We want *everybody & their grampa* to be able to use all the provided back-end services! We're not expecting to talk to the Qiqqa GUI front-end!

In other words: we want to provide a back-end *processing centre* which is addressable and *usable* by anyone who has just *minimal* experience with web technology for that. I'm thinking: fiddling with `cUrl`, a couple of lines of script to do your regular web requests & responses, and **most importantly**: *no nasty surprises* when you (the user) are used to using "web APIs".

Hence the server must be *continually available* on a *fixed local port* (*no surprises, remember!*) and be able to talk HTTP GET/POST. REST. JSON. That kinda jazz.

And when your script involves *multiple request/responses*, then you SHOULD be able to keep & track some *session context* across the requests you fire at us. Think "*cookies*": *session cookies* and such.

#### Stuff we've considered

##### The Classic Brutal Approach

*multi-processing*: a nice mouth-full for the ugly but doable approach where you package every risky component into its own executable ("*process*") and start/stop it for every item and  step in the data processing workflow.

*risky component* as in: every component that's sufficiently large & complex that it has a non-zero chance of containing heap leakage some day. 
 
Then you run a 'server' by having some **simple** software run an invocation chain script, where the individual *executable* components are invoked, one after another. Or possibly *piped*: think UNIX pipes, bash, ... 
While this *works*, it has a *significant overhead per processed item* that doesn't scale very well when you have a large/huge number of items to process. Starting large(ish) executables and/or loading large setup data (needed by the OCR component, f.e.) isn't *very costly*, but it isn't exactly *cheap* either. Not in *bulk* anyway.
 
In my personal case, we're talking 70K+ PDF documents (and counting) that the system SHOULD be able to analyze, text-extract and *index* in *reasonable time*. 
Given the assumption this has to run on moderate consumer hardware, these overhead costs become rather a bit too much: a *faster* system is called for, where we don't *start again* for every little fart we make, but a system where our items smoothly & swiftly stream* as data through our ductwork.

##### Ditto, but with *Memory Mapping Magyckk*

Running each step in a multi-step process ("workflow") as a separate executable/process incurs the *added cost* of a lot of cross-process data transfer between the executables. Rendered page images (OCR!), etc.

Iff we wish to cut down on the *data copying* part of the costs at least we MAY consider using Memory Mapped I/O for communication between processes: the *boon* here is reduced memory pressure and copying costs across processes. All initially targeted platforms (Linux, Windows. Mac as well, though that one is left as an exercise for the reader / Mac Owner) have thee technology built into their OSes, so that should work out fine, even when we need to use some portability layer library code layer to keep things manageable at the Qiqqa application level.

While I made some initial attempts, I must say this approach comes with a couple of drawbacks that I don't like as a *developer* and since that particular kind of resource is rather scarce ATM, it matters. Quite a lot, in fact, which is why we don't go this route. The drawbacks:

- **Hard to Deep-Debug[^3] a multi-stage workflow**. You can hook your debugger into *one* executable. Wizzards (Hello, Mr. Rincewind!) may be able to hook into *two*, Mrs. Weatherwax could possibly even do *three*, but when things get *real hairy* down in the rabbit hole, you quickly discover why this mechanism isn't widespread.
- **Less Memory Pressure. *But*!** It's easy between two *instances*, only. Beyond that, it get a little hairy: unless you're willing to deal with system-level semaphores or equiv tech to keep everyone in line re what-is-where in that *memory map*. Ditto when you use *multiple maps*, *plural*. Very cool to design and code, but still the diagnostics-ease-at-production-time are rather less than stellar.
  Oh, and I almost forgot: there's actual *limits* on the *number of memory mapped areas* your OS is willing to support. So, while you have some *smart thoughts*, those won't do and you'll have come up with some system or another to use one or more memory maps for *multiple 'messages'* going in various directions. Semaphores, yay!
 
  > I also considered a sensible TCP/socket messaging system (ZeroMQ-based) where those **small** messages are driving & guiding the system, while extra memory-mapped *zones* could be used by the various executables to share larger data among themselves. E.g. page images, text dumps.
  > While technically viable, *every which way* I look at this, it's inter-step communication costs *gone up* as we, by design and necessity, use OS-provided means which, while *fast*, still imply *additional overhead* vs. just pushing a couple of memory pointers/references around in a single application process, when the bugger would *just handle everything*.

- **Harder To Write**. Writing a single *multi-threaded* application isn't rated *easy*, writing such a  multi-processing + comms system isn't *easy* either. I tend to think it's actually a bit *harder* iff you do mind about production stability and ~ risks: for instance, how *do* you cope with a crashed component? Being multi-process, the rest is still alive, so you'll have to *deal*. 
  Meanwhile, your single-process, when at the same crash-risk (~ code quality), will *crash in its entirety*, thus leaving you with a much smaller what-to-do-in-case-of-severe-failure surface: sure, you must *deal* with recovery & command re-issue no matter what you choose, but with multi-process the number of checks grow with the number of processes you have.

Still, this was the way I was aiming for The Qiqqa Back End, until recently. Sometimes it takes a long while to realize the obvious:

A single process/executable appeals for multiple reasons: Deep Debugging, code sharing, easy and *cheapest* inter-part communications, also when you go the *multi-threaded* way, shared init data instances, hence reduced total system memory pressure. And you might even share some (setup/init/cache) data instances among multiple items while processing them, further reducing run-time memory pressure on the system.
A single process comes with one giant drawback, however, and that is: susceptibility to performance decay due to heap memory leakage, which is something we MUST reckon with in a non-garbage-collected codebase (C/C++). While everyone may be touting *garbage collecting* as pure *diamond horse*, I have found with Qiqqa (C#/.NET) and other projects that the blargh (say "garbage collection" and you are God) doesn't match *reality*: Qiqqa, for example, is *very bad* in that aspect, as there's a *lot* of fragmentation, zombied and otherwise *eternal data* due to the various components exercising their prerogative to *pin* some fragments or otherwise, e.g. by coding objects such that they effectively *cache their entire costly content ad infinitum*, thus beating the garbage collector silly by just doing what every programmer does: *not* think about ever *releasing* what they're currently holding onto. When ou observe Qiqqa at work (32-bit .NET), you'll note that it quickly grows to a rather large consumption as it acquires all the library metadata -- some of the bits due to intent, others due to the side-effects of simply inspecting the entire library as a necessary part of sorting lists, etc. -- so that the garbage collector gets to do its job in an environment where, after a while, the total memory pressure is huge (.NET allows about 1.5GByte RAM allocated before it starts to seriously b0rk; Qiqqa achieves that number easily when you feed it 70K+ documents + metadata and do a couple of searches and other actions that 'hit' the library item objects in sufficient numbers). When observing this in a debugger, you'll note the garbage collector is unable to release the bulk of the data (thanks to the way Qiqqa is coded: it was never designed for large libraries and suffers severely when used with such) and consequently slows the machine down to a proverbial crawl in its oft-repeated attempts at clearing up some cruft: while very smart, it's not able to sweep all the noise aside and system CPU costs go up a tad while your heap won't ever again bee *pristine*, or even *soft of clean*. It's a mess, that grew and never let down. 
And that's how most "we use a garbage collected language so we're golden!" applications are written: super-smart garbage collectors compete with increasingly... retarded? coding practices: you can now produce something that *works* much swifter that I ever will, but we'll be *on par* once you hit the numbers like I do, as coding for low memory pressures is a *special game*: the over-simplified brutal approach here would bee to code with a focus on minimizing data lifetimes: the shorter the lives of *everything*, the better off you probably are. And note I say *everything*: when you can re-use data, and thus decide to keep it around, that keeping-around  SHOULD be kept at bay by discarding it *quickly* if demand doesn't show up soon enough, ergo you might be interested in using in aggressively-releasing caches. When you don't you quickly run into plenty usage scenarios where you find out in production that you "must have bigger hardware to cope" because your code simply does the same as the Qiqqa codebase: not bothering and minding specially about "how soon can we wee ditch this?" and 'thus' keeping shit around for-effin'-ever, not because you meant to, but because large-application dependency chains in the objects / etc. drives the sys(correctly!) conclude that *someone in there* is still interested in your shit and 'the shit' is, therefore, still 'hot' and should be kept around just a little while longer. Seen it will Java. Seen it with .NET. Haven't seen a platform where this *doesn't* happen. Because it makes you faster (correct!) but the hidden dagger is that you won't be bothered about memory usage as much as you would have if you were in a C/C++ you-gotta-mind-every-darn-thing system. And thus your application gets done faster, performs about the same (because C++ can bloat like nobody's business, too!) and doesn't reckon with the peculiarities of living with a garbage collector, because, hey, not having to mind that sort was the selling point. 
That little rant doesn't make C/C++ your language of choice and *better* than anything else, it just is a viable alternative, still, *some times*. There's no big no vs. yes decision making criteria re programming language here. C/C++ can be bad in its own beautiful ways too, so it's exchanging one scary horror for most (C/C++ programming) for something else, that's at least *less dangerous* when it is failing.
I like C# and would have loved using it for Qiqqa, but the cross-platform thing is, well, *Microsoft*. Two decades and counting and all you can decently get is cross-platform *as long as you don't mind having a UI. Oh dear me, not a GUI! Nay! Nay!*  And this 'GUI' thing is a horror *anywhere*: Java, not a favorite of mine already, does offer cross-platform GUI dev, but don't ask me to code it up, please. And the looks are horrible, unless you spend inordinate amounts of time on it. Then there's Avalonia for .NET, you say. Great. Keep going, guys, but given that I consider YAML a design failure in its own right, building on that, while understandable somehow, is *not my thang*. (Sure, separation of concerns is a beautiful thought. Data binding: gets me all flushed and hormone-imbalanced too. However, in a way I can't exactly pin down, in daily *life* getting these while they're nice and perky are rather more as *academic* as every IT male ending up with a *trophy wife*: get real. Chances aren't *slim*, they are *infinitesimal*. And given a small glass of some very choice elixirs for my palate, starting somewhere around *peated* and lazily dancing to the tune of *single malt*, you may find me willing to argue those *chances* are even *negative*, an otherwise mathematical *impossibility*. But *consistent* *principled* separation of concerns is, when you hit the chaotic plasma that's called (Graphical) User Interface, rather... *quantum*. So I postulate that your chances achieving it *and keeping it intact while your application moves into production and some actual, serious, user interfacing* is *potentially negative*. Math notes: way I read it, it says here you'll have more luck acquiring *and up-keeping* a glorious trophy wife. Lots of glory and luck till the day you croak. Meanwhile, separation of concerns is a very nice, very useful and above all *ethical* ideal*: it's like having religion: when applied correctly, you'll probably turn out a more socially worthy animal than you would have otherwise. YAML, therefor, is a cute idea, but it's rather SteveBallmerish in its execution: let's fix the importantly *wrong bits* of HTML. Now make it *beautiful*: pure XML, yowza! Now, because we *can*, let's do the browser itself too, so we can have data binding and all the goodies your pipe dream is whispering to you. Voila. YAML + WPF. 





Okay, *off the rails*, definitely. To be done at a later time, when I'm less fuzzy.



Blarg blarg blergh! Tooooooooooooooooooot!!!1! 

*Hm*, bloody Elephant. ... 

Or to quote the brothers from Ice Age: 
- "Dude! This is getting **really** **weird**!" 
- "Yeah, I know!"


-----------------

@#\$%^&*()^(%*@&$^#@!





That means rolling all / most sub-processes into a single process, possibly using *multi-threading* to use the advantage of modern multi-core CPU hardware, while we try to keep the amount of (memory **and** disk) I/O to a minimum. And that last bit prevents us from using separate executables (processes) per step! (While we MIGHT have used "memory mapped" communication mechanisms to keep the memory & disk comms costs to a minimum even then...)


----------------------------------

Latest design:

- use a multi-processing approach *anyway* to deal with the risk of heap memory leaks due to code complexity: allow large chunks of the work be done in *temporary servers*: servers which have a *limited lifetime by design*. I'll keep my options open, but here's a few criteria & comms notes:

  **process termination criteria**

  - when not having had a command to process for some time: *timeout on idle*. This ensures the server process will kill itself even under adverse conditions, e.g. when the controlling process/application has crashed.  That's why I'm thinking about a timeout of 5 -- 30 seconds only!
  - when having reached a certain level of memory consumption. While this MUST NOT be hard terminating condition (any work under way SHOULD be allowed to complete, ideally, for some works WILL trigger huge memory consumption and we don't want to create a scenario where such work would forever halt/block the system by starting up, meeting the memory limit criteria, hard-failing as a consequence, then being *re-issued* due to outside *retry-on-failure* mechanisms that can trigger due to the hard-failure and too little context info: was this (or was it not) a hard-fail due to surprise circumstances, which would validate a retry, before declaring the job 'non-executable' perhaps?
  - when a certain number of jobs have been done: wipe the slate clean after a while so we are assured any pollution gathered during the process lifetime DOES NOT propagate into the result of many jobs which follow. **Note** that this is not a *hard criterion*: see the note about *sessions** below.

  **Implementation Notes**

  - work processes should be able track 'sessions' so that the process(es) issuing the jobs can set up *relation/dependency chains*, which are easy and simple as they'll execute on a single node in their entirety: giving us the minimum of administration fuss.
   
  - job issuers SHOULD be able to 'manage' the resulting sessions by (a) helping the job process by declaring a session *completed*, which would be a nice moment to terminate the job process for system cleanup reasons. 
    Given that a job process should be independent and be able to decide when to quit on its own, that 'end of session' message can only serve as a *hint*. Another approach to get the same result is to send a 'stay a while longer, please, for I have more work for you to do' request message from the issuer. Which adds a risk when thee issuer crashes next or is otherwise prematurely terminated itself: the *idle timeout* is there to save us from that, after all, but we need to reconsider how to set this up *precisely* and look if we can devise a system where we remove or at least severely limit the need for that *timeout on idle* capability: it SHOULD always be present, but the comms protocol (job issuer commands, etc.) SHOULD be such that it itself DOEES NOT depend on this particular bit of job process behaviour: it's a stop-gap, last ditch counter-measure any way.
    
  - database I/O and closely related work should be done in a single job process, while the others are reserved for heavy CPU load actions, e.g. PDF text extraction, analysis, etc., where we then have a bunch of data to fetch from / store into the database: here we agree with the additional data transfer costs at the interface layer due to our choice for multi-process. 
    SQLite behaves best when accessed from a single process, so that has our preference (no need for system-wide multi-process semaphores, etc., for coordinates database access). The consequence is that we then need to communicate (send/receive) the data we need across process boundaries. The risk to the database I/O process should be kept minimal, i.e. only particularly curated components will be allowed to run in the same process space as the database, as that process is not, by design, a mission critical component with a required *long life*.
    
    > Either that, or we come up with a system where it's somehow tolerable & *obvious* to the user that the database server will be unavailable for a short time as it's busy restarting right now...
    
 - The multi process design with limited lifetimes is viable as a *outwardly stable system* when we set up our comms using HTTP 301/302 redirects, for example. The idea is this:

   - set up a simple routing/monitor/manager server, which accepts incoming queries and, instead of answering them directly or through proxy, responds with a 301/302 temporary redirect: cUrl and other tools can deal with such a response most easily and invisible to the user, while the result is every request of ours being automatically redirected to the active *server du jour*: thee router/monitor/manager keeps track of which job processes are currently alive, can start fresh instances where needed (including delaying the 302 response until the new server is up & running, when none was before, when the user query came in)

   - the job processes then get the request (as it's one of them that is pointed to by the 302/302 redirect) and execute it. Iff they need other work done, which they can't handle themself, e.g. database I/O, then they go through the same 'protocol': all questions are to be sent to the router/monitor, which will either deliver a direct response or a redirect to one of the other job proceesses.
   











[^2]:  Android isn't in the list because Qiqqa is a pretty hefty librarian<sup>+</sup>  machine and Android is generally available on low(er) powered hardware. While *someone* out there might like the idea of running the Qiqqa Core (servers) on their PI or similar rig, I don't intend to support right out of the gate: OCR is pretty power-hungry and when you have a decently large library (say 10K+ publications) re-indexing and such will be pretty costly, so we'll take all the power we can grab, thank you very much. Meanwhile, I *don't say it's impossible* , while I have such hardware lying around, I don't *compile/build* for ARM. At least not *initially*.

[^3]: **Deep Debugging**: allow me to coin that title for stepping through a complex system using your debugger of choice. You *descend*, perhaps *ascend* through the call stack and then *descend* quite some more, while you hunt the current WTF undesirable a.k.a. The Bug. Deep Debugging implies you have the source code of the library components you use *and are fully in accord with diving in, walking those sources and possibly patching where you deem necessary*. All in the name of Bug Hunting: *undesirables have nowhere to hide*. When you're of a more cynical inclination, you might also call this a *totalitarian source* state. 
