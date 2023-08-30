# Why I consider OpenMP the *spawn of evil* and disable it in `tesseract`

This already showed up on my radar in 2022 when bulk-testing our `mupdf` + `tesseract` monolith Windows build: it didn't matter which sub-command of `bulktest` I was running, the fans would start spinning and CPU temperatures would rise quick as they could until a new *burn motherfucker burn* versus cooling solution equilibrium was reached at almost 80 degrees Celsius *core*. Which was, frankly, *insane*, for 'twas simple *single threaded* tasks on an *8/16 core* Ryzen mammoth.

Profiling in Visual Studio plus some debugging showed me some obscure OpenMP internals decided to run like mad, without having been told to do so by yours truly.

That analysis led to `tesseract` [where some OpenMP attributes are sprinkled around some hot paths when you run OCR](https://github.com/search?q=repo%3AGerHobbelt%2Ftesseract++pragma+omp&type=code). I didn't know how to look into it deeper, pulling apart OpenMP itself. (I knew about ETW *existing* but never had used it myself before, so it was, ah, "*not top of mind*" while I was facing this crap, which only overjoyed the energy companies.)

The preliminary conclusion then was that OpenMP *somehow*, stupidly, decided to start all the threads required for *when* and *if* all that parallel exec power was demanded inside the belly of `tesseract`, and, then, once started, **never stopped running, i.e. waiting for stuff to do and spinning like mad while doing the wait thing**. (*All* the cores were maxing out once this started.)

Key to the problem was it always occurred *after* some (minimal) `tesseract` activity, which was part of the randomized bulk test, which is set up to stress test several main components, feeding it a slew of PDFs to chew through. Several of those tasks are single-threaded per PDF and to help built-in diagnostics I don't parallelize multiple PDFs for simultaneous testing: while that *will* be a good idea at some point, it's *not exactly helping yourself* when you go and make a multi-threaded parallel execution test rig while you're still hunting obnoxious bugs that occur in single-thread runs, so having OpenMP in there was only there because of `tesseract` having it by default and me considering this as a potential *nice to have & use* system for when I would be ready to migrate my code-base to use it elsewhere in the code-base as well. 
However, once OpenMP would have run through one such an [`openmp`-enhanced code section](https://github.com/search?q=repo%3AGerHobbelt%2Ftesseract++pragma+omp&type=code), it was apparently "*Bijenkorf Dwaze Dagen*"[^1] at the CPU forever after.

The solution, I found, was getting rid of OpenMP entirely. Meanwhile I wondered why everyone is using this one while it clearly is coded like *utter shite*, at least from where I'm standing... And nobody complaining about this?[^2]



## Until today, that is...

Granted, I'm slow on the up-take, but I count myself lucky I finally ran into [the performance analysis work published by Alois Kraus](https://aloiskraus.wordpress.com/). 

One of his posts is this one: https://aloiskraus.wordpress.com/2022/12/11/war-story-how-antivirus-solutions-can-bring-a-server-down/ and one section in there has been an eye-opener for me! After the fact, alas.

To quote the relevant part (at least it was very relevant to me; marked emphasis in the quote is mine):

> Below is a picture of total CPU consumption over a period of two minutes. Green is Idle (means CPU is free) and red is the CPU consumed by the Windows Sleep API call which needs ca. 60% for all cores for more than one minute. Why is sleeping consuming so much CPU?
>
> ![](./assets/kraus1.webp)
>
> There is a specific workload which occurs only on many core servers: Spinlocks are locks which try to burn some CPU on all cores before all spinning (= trading CPU for decreased latency) threads enter the expensive OS lock. The usual implementation of a Spinlock uses several stages:
>
> 1. Check if lock can be acquired
> 2. If not
>   
>    - For(i..n)
>      - Burn some CPU cycles by calling [`pause`](https://aloiskraus.wordpress.com/2018/06/16/why-skylakex-cpus-are-sometimes-50-slower-how-intel-has-broken-existing-code/) instruction in a second loop
>      - Check if lock can be acquired and exit loop
>      - Call `Sleep(0)` to give another thread to do some useful work
>      - Check if lock can be acquired and exit loop
>    
>    - Enter OS lock
> 3. Lock is acquired
>
> **Some high performance threading libraries like the Intel OpenMP library use excessive amounts of CPU spinning for up to 200 ms to almost never take a lock to get, at insane power consumption costs, a few microseconds of faster algorithm execution.** This means if you let one empty task run every 200 ms you will get this CPU graph when using the Intel OpenMP library:
>
> ![](./assets/kraus-cpu.webp)
>
> Sure, your CPU looks super busy, but did you perform much useful work? Not at all. The Intel OpenMP library makes it look like you are doing heavy number crunching and you need a bigger processor to take that load.
>
> If you set the environment variable [KMP_BLOCKTIME=0](https://www.intel.com/content/www/us/en/developer/articles/technical/how-to-get-better-performance-on-chainer-with-intel-acceleration.html) then you get a flat CPU profile as it should have been always the case. The decision of Intel to waste 200 ms of CPU cycles by default is highly questionable, and no one, after profiling the CPU consumption, would leave it at the default value.
>

Crazy indeed. 200 **milliseconds** for *spinlocks*... the only causes I can think of there are:
1. ~~you don't know what you're doing.~~ (This is a library allegedly engineered by knowledgeable folks at Intel, who shed more multithreading/multiprocessing acumen in their toenail clippings than I expect to ever collect into my *brain*, so we scratch this one *with extreme prejudice*.)
2. you are totally bonkers. (Always an option, that one... 😊 )
3. you do vengeance like a Queen and don't give a hoot about "*alleged collateral*". *[[Apres nous le deluge - attribution|Après nous le déluge]].* (Makes me wonder who the intended target of your rectal massage with that cactus stub is. 🤔)
4. you are zealously ambitious and just *have to have* that number one spot in the benchmarks, *coûte que coûte*. Regular usage be damned!
5. ... 🤷 all out of viable alternative explanations here 🤷 ...

I'm willing to bet 5 dimes on horse number 4, until further background intel is forthcoming.

----

Anyway, thanks to Mr. Kraus, I now have a reasonable hint of what might have gone on in there: apparently OpenMP is way too happy to use spinlocks and kept the instantiated threads running (for shutting down and restarting new ones is relatively costly, if you think that spinlock-style way) while they waited for another [`[openmp]`-attributed code section](https://github.com/search?q=repo%3AGerHobbelt%2Ftesseract++pragma+omp&type=code) to come their way, thus resulting in continuous 100% CPU loads as those threads' spinlocks spin, and spin, and spin, ... and then very probably loop the still empty work queue, so it's back to spinning, ... spinning..., and more spinning. *Ad nauseam.*

*Caveat emptor:* 
I *DID NOT* verify the above reasoning by digging into the OpenMP codebase. Mr. Kraus' work is worthwhile enough for me as it is and as a reminder for when I run into OpenMP again in a generic application.

> Given where I intend to take this baby (Qiqqa backend), using OpenMP like that is expected to be detrimental (er, "sub-optimal") anyhow, as we plan to run PDF *batches* through those CPU-heavy code chunks. Given that several important parts of the codebase are NOT thread-safe, such batches are best done in a multi-processing style (rather than using a multi-threaded approach within a single application *instance*) and those will occupy those CPU cores just fine while the parallel processes chew through those PDF batches. Of course, there's also the scenario where'd want to process a single PDF *asap*, but that's supposed to be a *page render* or similar task, *not* `tesseract` OCR activity. So those `#pragma omp` in there will be pretty useless for us either way.
> 



## Preliminary conclusion to this 

Good to keep this in mind when we ever revisit this OpenMP stuff elsewhere -- I'm bound to run into it elsewhere as we'll be incorporating some "A.I." at some point in the future and those libs come loaded with OpenMP -- but for now I stick to my earlier decision: no OpenMP at all if I can help it. We'll do it another way, so we have better control and less risk of encountering crap like that under the hood.

*200msecs*. ... Somebody is gunning for some particular benchmark metrics there, no doubt. Bah!











[^1]: in the words of one of my acquaintances in The Hague, who had a rather different outlook on life and the human condition, compared to me: "*No! I don't go there to **buy** stuff! I go there to watch the abundance of Olympian bitch fights at the fire sale! And when you get bored watching this one, all you gotta do is turn around and sure as shit there's more great fighting to choose from! Oh! I can't wait!*" (ed.: *Bijenkorf* is/was a rather posh clothing store which had an annual *fire sale* a.k.a. "*Dwaze Dagen*" (Crazy Days) -- crazy indeed as some women would enter the store with rather more hair then when they would leave the premises. 🤦‍♂️ I wouldn't be surprised if the authors of OpenMP were riled and enthused similarly to this acquaintance of mine. So they coded their own *Crazy Days* into their library, to run ad nauseam. **Not my cup of tea, so OpenMP can go mess up somewhere else, but not here, not any more.**)

[^2]: Regrettably my cynical alter-ego is the only one with a ready and probably-on-the-money answer: OpenMP is generally used in academia and they're far removed from their electricity bill to not be bothered by such trivialities, so who gives a fuck? *Let'r rip!*




