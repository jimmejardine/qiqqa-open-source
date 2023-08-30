

### Notes

  Of course there's StackOverflow and such, but when you get into trouble, just file a github issue and I might have an answer.

- I'll make sure (I hope, fingers crossed) to have every head commit that I push to github compile/build properly: I've been a nasty bastard sometimes by committing stuff that wouldn't build until one or two commits later. I'll refrain from that 'lone gun' practice. ;-)  (Or breakage should be clearly noted in the commit message.)

- Get yourself familiar with `git` and it might help to use [TortoiseGit](https://tortoisegit.org/) as a graphical UI tool on top in Windows as that gets you going immediately without bothering about git commandlines.

  If you haven't already, git + bash for Windows is available here: https://git-scm.com/downloads

- The Qiqqa *solution* (DevStudio terminology) contains many *projects*: the important ones are 'Qiqqa' and 'QiqqaOCR'.  Projects 'libs', 'icons' and 'Utilities' are required libraries, which is what Jimmy set up to use as a kind of 'common stuff for both' set back when Qiqqa was still Quantisle.

  The rest of it is organized in folders in DevStudio and is only for either testing or for future dev work while we check viability of upgrade paths for several Qiqqa functionalities. Basically, you can assume that when a project is in a folder there, it's not part of the Qiqqa executable or required DLLs right now.
  
  ![](assets/devstudio-solution-view.png)
 

- I am currently working (or trying to) on getting the Qiqqa developer documentation going in `/docs-src/`, though be aware that most of it is quickly written stream-of-conscience notes of mine about the various parts of Qiqqa and its upgrade process. I'll try to update and organize that stuff for you as we go, so sync the repo maybe every week? (`git pull --all`)

  If you run into stuff that's weird, that you don't grok or otherwise have trouble with, please give me feedback on that so it can be improved. I'm not a technical writer (e.g. got too flowery verbiage) and there's already a lot of gain when those pages end up as a useful and *readable* resource for you and others-to-come.

- Might be good to try using the DevStudio debugger on Qiqqa. Though it is a multi-threaded application and that can be confusing with multiple background task copies running in parallel when you debug, so there's a place in the code where you can dial the number of detected processor cores down to 1 and/or disable background tasks entirely -- I coded that but I have to look up the precise spot as it dropped from active memory. The useful bit here is being aware that you can dial down the mayhem when stepping through background or foreground code by limiting/killing the parallelism.

- Also be aware that Qiqqa now has a command-line argument to specify a different base directory for the Qiqqa libraries: that's quite handy when you want to debug/test Qiqqa on a test rig or observe how it behaves when it's a "fresh install", i.e. no user library yet. This can be set in DevStudio Project Debug section and is passed to Qiqqa when you start it from DevStudio to run [Ctrl+F5] or debug [F5]

  ![](assets/devstudio-project-debug-arguments-view.png)
 

For developers, it's a handy option to have as you can easily test Qiqqa on a library set that's not important to you, i.e. your own Qiqqa libraries. ;-)




## By the way, how did you get so involved in the code?

I found Qiqqa back when it was still a commercial product and closed-source. It was the one out there that got closest to what I really needed/wanted for my own document management. The PDFs were stacking up and I had hit the problem of knowing the info must be there but not being able to dig it out way too often.

Qiqqa turned out to do what was advertised and did it in a way I understood immediately, so I was happy with Qiqqa for a while. Then Qiqqa started to behave erratically as my libraries grew and the hair-pulling started: libraries which would not recover, no matter what you tried. 

This resulted in some long agonizing hours and a few bug reports filed. Meanwhile I had bought a license, so there was commitment from my side and then I started to notice activity dropping while my problems were increasing, so I started some reverse-engineering work, full well realizing that I would be recreating Qiqqa from scratch if I went ahead and that was just... crazy. Nevertheless, my frustration produced this analysis: https://github.com/GerHobbelt/qiqqa-revengin and I could use Qiqqa, still *quite unsatisfactory* but at least *usable*, for collecting PDFs and getting their metadata. I dumped the SQLite database and did blunt grep work on that (and the OCR directory if it was important enough, for that meant finding possible hits and having to dig out the PDF by hash, so I had a `grep`+`find`+`cp`=copy bash script that grepped the dumped lib and copied all mentioned PDFs to a scratch directory for manual scrutiny as the next stage. Talk about hassle.

Then Qiqqa suddenly became open-sourced, so I jumped at the source code  and now 'fixing Qiqqa' was not a 'build from scratch' proposal anymore but somehow looked like a *reasonable effort*.

Turns out you can still pour a lot of time into something 'reasonable', but alas. ;-))  Decided to mail Jimmy at some point on the off chance that he was around and got a reply. Bit back & forth and then I got push rights on the main repo, so I can publish software releases, manage the issue list on the github repo site and push/merge code. 

If you ever have a look at the git log, you'll see me working on fixing Qiqqa crashes and other bits and pieces that annoyed me no end (particularly the application crashes due to crap ending up in your downloaded PDFs and the PDF access code (mupdf, QiqqaOCR, tesseract, Sorax, Qiqqa) failing fatally. Then a few bells and whistles got added and I still didn't quit, so I guess I'm the maintainer now. ;-)

I still need (and will continue to need) Qiqqa for my document management and it's still not cooperating the way it should (as a lot of the trouble is due to bugs in outdated/obsolete libraries/binaries used in Qiqqa) so the Qiqqa story is far from over as 'upgrading' this baby is a lot of work.

As I asked myself a couple of times if sticking with (commercial) Qiqqa was a sane choice, I looked for greener pastures. The excursions I made to other citation & document management systems have left me quite dissatisfied: haven't seen something really good out there, not even commercially, at least not for a one-man or few-folks shop. (If you're a *big* hospital, Nuance might sell you on some of their valuable XYZ, but that's an expensive proposition in both ware and work, anyhow. The rest of the bunch is tailored to either relieve cash-strapped university folks from their precious dough or otherwise doesn't pay attention to those of us not in the business of [writing papers for either graduation or tenure](https://youtu.be/vtIzMaLkCaM?t=670). 
<sup>(Uh-oh, did I sound strongly opinionated just there? *Whoopsie.*)</sup>

So it's Qiqqa or bust for me, it seems.

