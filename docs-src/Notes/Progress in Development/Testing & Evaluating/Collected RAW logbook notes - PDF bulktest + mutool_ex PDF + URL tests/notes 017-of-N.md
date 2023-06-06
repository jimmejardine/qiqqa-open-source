# vcopy + sqlite + sync + rclone + rsync + ... tests: logbook notes


## Test run notes at the bleeding edge

**Big Caveat: these notes were valid at the time of writing, but MAY be obsolete or even contradicting current behaviour at any later moment, sometimes even *seconds* away from the original event.**

This is about the things we observe when applying our tools at the bleeding edge of development. This is the lump sum notes (logbook) of these test runs' *odd observations*.

**The Table Of Contents / Overview Index is at [[PDF `bulktest` test run notes at the bleeding edge]].**

-------------------------------

(This logbook section was started in 2023.)

*Here goes -- lower is later ==> updates are appended at the bottom.*

-------------------------------

### Item ♯00001 - Linux `cp` and Unicode / long filename surprises

When copying NTFS to BTRFS on Linux Mint, I got these error messages from `cp`:  DID NOT expect these. Apparently he Linux FUSE NTFS driver also suffers from MS Windows' long filename/path syndrome, but here I don't know how to fix that. On MS Windows you can get around it by using universal paths, like `\\.\drive\absolute-path\filename`, but there's no such thing on Linuxes. 🤔

```
cp: cannot stat '/media/ger/16TB-linux001/USB_H1/Sopkonijn/!QIQQA-pdf-watch-dir/!5/docs/UNKNOWN - 增益值-信号极性以及更新速率的选择可用串行输入口由软件来配臵-该器件还包括自校准和系统校准选项-以消除器件本身或系统的增益和偏移误差- CMOS 结构确保器件具有极低功耗-掉电模式减少等待时的功耗至.pdf': File name too long
cp: cannot stat '/media/ger/16TB-linux001/USB_H1/Sopkonijn/!QIQQA-pdf-watch-dir/!5/docs_original/UNKNOWN - 增益值-信号极性以及更新速率的选择可用串行输入口由软件来配臵-该器件还包括自校准和系统校准选项-以消除器件本身或系统的增益和偏移误差- CMOS 结构确保器件具有极低功耗-掉电模式减少等待时的功耗至.pdf': File name too long
cp: cannot stat '/media/ger/16TB-linux001/USB_H1/Sopkonijn/!QIQQA-pdf-watch-dir/2018-10-09/Phased Locked Loop - PLL/74HCT9046A Philips Semiconductor PDFs ����������� ������������ 74HCT9046A �������� ������� 74HCT9046A.pdf Philips Semiconductor PDFs datasheets datasheet data sheets 74HCT9046A Philips Semiconductor PDFs.html': File name too long
cp: cannot stat '/media/ger/16TB-linux001/USB_H1/Sopkonijn/Qiqqa-exports-dir/docs/UNKNOWN - 增益值-信号极性以及更新速率的选择可用串行输入口由软件来配臵-该器件还包括自校准和系统校准选项-以消除器件本身或系统的增益和偏移误差- CMOS 结构确保器件具有极低功耗-掉电模式减少等待时的功耗至.pdf': File name too long
cp: cannot stat '/media/ger/16TB-linux001/USB_H1/Sopkonijn/Qiqqa-exports-dir/docs_original/UNKNOWN - 增益值-信号极性以及更新速率的选择可用串行输入口由软件来配臵-该器件还包括自校准和系统校准选项-以消除器件本身或系统的增益和偏移误差- CMOS 结构确保器件具有极低功耗-掉电模式减少等待时的功耗至.pdf': File name too long
```

No idea how we're going to resolve this in our sync tool (named `vcopy`).



### Item ♯00002


#### Tests to set up & run (performance checks):

- SQLite performance test with 100K+ documents:
  1. column storage via multiple tables and JOINs: how does this fare as a metadata *variable column count* storage approach vs. ...
  2. *one* metatable which stores all (label, value) pairs for all metadata item labels for all documents, and ...
  3. one table which has columns for the most usual / regular metadata labels (title, author, ...) *mixed* via JOIN (+ flatten) of the above metadata table which now stores all the other, remaining, (label, value) metadata items for all documents, and ...
  4. is there any kind of noticeable *optimum* there, with respect to which and *how many* columns we designate as "*regular*" and thus park in the first, *wide*, table, vs. the second *gathering* table?

  And then the question also involves the question whether we'ld benefit from different \[types of\] *indexes* on the different tables. Plus: can we otherwise *optimize* such metadata gathering SQL queries, when, say, we want the metadata record to be gathered at query level, i.e. reported as a *very wide set of columns*, vs. leaving such *pivoting* to the requesting software?
  > 
  > Initially I thought it might be handy such a "*flattened*" SQL output for third-party users, but anyone can easily gather & merge multiple rows *by themselves* when they want this type of output, while folks who are only interested in certain *columns* for feeding into follow-up queries, he *flattening* is only detrimental to *lump sum consolidated* performance of the Qiqqa database query system... 🤔 so everyone might be better off with sticking with the *mostly RAW* output, which might be a bit chaotic when we land on database design/layout *numeros* 2 or 3 rom the list above.
  
  




#### Tooling to create:

- `vcopy` = tool to copy & hardlink Qiqqa library components (database, document files, etc.) for the purposes of team/remote sync and *library export*.

- `ingest` = tool to import external Qiqqa libraries, *import*/*merge* Qiqqa libraries, backups and (possibly incomplete or broken) Qiqqa metadata files + *import*/*merge* other metadata database dumps that could help us complete/improve our metadata collection, e.g. the SQL database dump for [sci-hub.ru](https://sci-hub.ru/), which carries millions of documents' metadata and file hashes (*by the look of it*). PLUS import my own rough database *dumps* and *exports*, which is another way to recover older/b0rked databases into a new Qiqqa library.
  Also understand how to unpack and *ingest* `.qiqqa_backup` classical Qiqqa backup files.

- `text-extractor-analyzer` = tool which extracts text from PDFs (like the old Qiqqa extract tool would've done, but *better*!) & analyzes possible applicable heuristics which would improve the exported text, e.g. 
 
  - **word separation** for those PDFs which produce a stream of (English) text without the proper spacing, making keyword search very hard,
  - **whitespace cleanup** for the near-opposite behaviour we've also observed: some PDFs spitting out slews of consecutive spaces and/or TAB characters as *variable-width* word separators: these lines could simply have been cleaned up to produce single spaces between words, as the old Qiqqa text extract (`doc` directory tree) *cache* has a format which includes position and size value arguments for each extracted "word"; this way we'ld get a more useful *and cleaner* text output stream.
  - **ditto**, but for the more usual and *curious & strange* choices for whitespace separators for higher up the Unicode codepoint range: We've observed plenty PDFs which spit out '?' question marks for these, which would obviously have been whitespace characters from a human perspective.
  - **recognize content written in non-WS languages (such as Chinese or Japanese)**, which does not mandate word separators & possibly suggest appropriate word separations. There's various libraries for that out there, but remember: here we enter the heuristics/statistics arena at full tilt: it's not about being 100% accurate 100% of the time, but getting near (or even surpass) 99% accurate 99% of the time. 😉
  - **recognize content that's gobbledygook**, i.e. *possibly **obfuscated** by way of font codepoint reshuffling*: we've seen several PDFs exhibit this and here we MAY WANT to turn to our next level of text extraction tooling: the OCR engine. 
    Ideally this one could then be cross-checked against the gobbledygook to produce a statistically max-probability codepoint mapping, as that obfuscation mapping would be consistent across the document. When it's just rubbish and there's no discernable relationship between the gobbledygook output and the OCR result, we MAY also want to consider this is perhaps and extract for a chunk of *mathematics*. May/should we treat such chunks as images for our text + context analysis + export action?
  * *possibly* **script** extraction / text-postprocessing to improve text extraction output quality, e.g. apply automatic / *suggested?* spell-checking --> this would be an advanced feature as we MAY want to keep both original and *edited="corrected"* text in the output.
    
    > First idea was to markup or otherwise keep both original and edited text in the same file, but when we want to be able to *easily* use *external* professional-quality tools for comparisons & evaluation (such as Scooter Software's Beyond Compare), **the simplest way to get usable results is to output *two* text files**: one "raw" and one "post-processed", so that we can always see whether the "autocorrect" was actually correct or had just fubarred something rare/unknown to the spell-checker/corrector.
    > **This would then more easily blend in with any subsequent human-user vetted editorial edits to the extracted text**: an ability that is currently NOT available, but which I've been desiring for a *long time* as this allows us to Mechanical Turk any 99% OCR result and lift it up into 100% correct (*vetted*) content: not a requirement for all of us, but something I need as this makes straight citing / quoting from the actual content far easier and thus much more usable: I'm personally not invested or interested in the plagiarism scare at some academia; *value* (in my case) is increased when I can directly quote relevant chunks of original reference content so readers don't have to bother with reading / scanning through the references: that's increasing efficiency in a *business research setting*, where verification of references is only important when you don't trust the information collector / writer of the report that you got from me: *efficiency* requires both *trusting the bearer of the news (**me**)* (& thus citing references and having them available on request (*Qiqqa library*!) is beneficial at that secondary level) and *fast perusal*, i.e. NOT having to wade though tens to thousands of extra referenced papers' pages in order to check I'm not pulling the citations out o my arse. Thus *efficiency* in *business reading* actually *benefits* from quoting chunks of text, which in a student/academia setting would be automatically as "plagiarism".
    >  
    ...

- `hog` = a tool for fetching *files* from the internet, specifically PDFs. Intended to be used where you browse the 'Net and decide you want to download a given PDF from any site: this can be done through the browser itself, but is sometimes convoluted or neigh impossible (ftp links require another tool, PDFs stored at servers which report as having their SSL certificates *expired* are a hassle to get through for the user-in-a-hurry, etc. etc.) and `hog` is meant to cope with all these by providing:
  - ability to detect the listed PDF url and *download* it, i.e. has knowledge about several approaches used by various sites (publishers, etc.) which offer on-line PDFs for viewing & downloading,
  - *possibly* (= future feature) allow scripting these seek&fetch behaviours so users can create their own, custom, procedures re DOM parsing, cookies, etc. as the sites change or other sites' access is desired, when not yet part of the "how to download" knowledge base of `hog` out of the box,
  - fundamentally a tool that sits somewhere between `cUrl` and `wget`, with some `cUrl-impersonation` abilities thrown in as well. This includes:
    - desirable behaviour with minimal commandline arguments, i.e. "good defaults",
    - auto-naming the local filename when downloading,
    - auto-sanitizing the local filename when downloading (255 max length, only ASCII or letter/number Unicode chars in the filename, etc.),
    - create a *second* local file describing the metadata thus far, in particular the source URL, for it will be loaded by Qiqqa as part of the file's metadata later on,
    - ability to detect and follow HTTP 301, 302 responses, *but also* HTTP pages which come back as 200, yet have "timers" (sourceforge, f.e.) or other wait-before-you-get-it or go-here-instead "redirection" embedded in their HTML output,
    - `hog` being able to either fetch a *file* (PDF, image, ...) or a *full HTML page*: some whitepapers are published as blog articles or other HTML-only formats, and we want those too *as local files*. This implies that we should be able to tell `hog` to grab the page itself (auto-detect such a scenario?), plus all its relevant assets (CSS, images, ...) and bundle that into a HTMLZ or similar "HTML file archive".
      > Here I'm not too enthousiastic about the classical MHTML (Mime-HTML) format as it expands images using Base64, while we have *zero* need to raw-copy this into email or would otherwise benefit from the convoluted MIME encoding in our storage format -- better would be having it all in an accessible single file, e.g. a ZIP-style wrapper, hence my preference for [HTMLZ](https://wiki.mobileread.com/wiki/HTMLZ) (or the more modern [WARC](https://en.wikipedia.org/wiki/Web_ARChive) format, which seems to be preferred by internet libraries out there, e.g. archive.org)
       
- `snarfl` = companion tool to `ingest` for gathering metadata "from abroad", e.g.:
  - processing large metadata databases (and their backup/export/import files), such as the sci-hub.ru metadata dump, and produce a "reference library" from those. To be used as search sources where we try to match DOI or title/author with every document in our actual *library* where we are still looking to improve our own metadata set (or rather: get a "reasonable" initial suggest for the metadata for our document X, so that we have a simplified and *faster* job of vetting the metadata gathered ("*snarfled*") through this and other automated processes.
  - roaming the Internet on the prowl for additional metadata opportunities, e.g. automated Google / Bing / DuckDuckGo searches for the title/author and automated decoding/extraction of "initial metadata suggestions": here *versioning*, *version control* and *ranking* / *marking* metadata for each document become important as we MUST be able to tell automated suggestions, automated "finds" in foreign metadata databases and *owner vetted* metadata apart, so that we can report a *reliable* grading/ranking of the same. Given the lousy Google Scholar metadata (my long-time experience with using that) and sometimes dubious quality of other sources' metadata "files", let alone the statistical *uncertainty* inherent in machine-derived/extracted metadata from web + search-engine *scrapes*, a clear and human-grokkable **_ranking per line item_** for any metadata is a **requirement**! Hence we need to equip our tooling (`snarfl`) with the ability to report *source* and *estimated/indicated ranking of said source*, so that the human owner/editor can jump in later and correct/complete/augment any entries she doesn't seem of sufficient quality.
    > *Abstracts* are of particular interest here and prone to the severest modes of "crapping up" when left to automated tools. Here we see a need for both:
    > - *scripting* of particular scraping / extraction processes within `snarfl`.  
    > - *versioning* and *version control*, once again, as we ant to be able to monitor, inspect and report on the (gradual) metadata quality improvement process itself. (😄😇 *meta* of the *meta*)
    > - later on in this process *human intervention* a.k.a. *editing* will be involved, but that's out of scope for the `snarfl` tool itself as far as I am concerned: what `snarfl` SHOULD provide is the "*departure point*" for this metadata editorial quality improvement process, such that it can commence with the least amount of fuss.

**Thought**: `text-extractor-analyzer`, `hog` and `snarfl` MAY become one and the same tool as these processes, while in different *arenas*[^arenas], probably involve quite a bit of the same *technology* and *type of processes*, so it makes sense to roll these into one "*multitool*".

[^arenas]: The two different *arenas*: text **content** "*snarfing*" & publication **metadata** "*snarfling*"[^snarf]. 

[^snarf]: 🤡 Which one will we refer to as *snarfing* (*sans* *elle*)? And which one will be referred to as *snarfling* (*avec du elle*)? The 'l' for "*level up*" (= meta) or the 'l' for *mostly local*? 🤔 Doubting, but when I did *spontaneously write about it just now*, it's apparently the former that's preferred by this individual. 😆

- `pappy` = Qiqqa library and *library collective* backup + restore tool. The one you depend on when things have gotten too harsh and out of control. 😉
  Classical Qiqqa has these choices available at the start of the app itself, but I want everything *functional* to be situated in a *cross-platform portable*, preferably commandline driven, *back-end* set of executables, so we can carry the functionality across OS boundaries (Windows vs. Linux vs. Mac) and peruse and *test* hose with the least possible *developer cost*; the GUI then can become just "*one of many ways to access the libraries you have available*" (both at the *individual*'s and team*'s level).

* `bezoar` = Qiqqa OCR *diagnostics* tool: in order to provide an improved OCR experience, we need:
  1. to improve `tesseract` diagnostic output so as to produce a more easily understandable reporting / visualization of the OCR process itself and the detailed steps taken to arrive at the final result, e.g. better control over and display of *binarization*, *page segmentation* and *raw recognized text stream post-processing*.
  2. to enhance / augment the interface between `mupdf`-based PDF page image output and the `tesseract` engine itself: currently `mupdf`/Artifex doesn't offer a way to configure/tweak the `tesseract` engine they have integrated into their codebase.
  
  and, IFF possible:
  
  3. *hopefully* add the ability to manipulate these behaviours through user-provided *scripting* for customized behaviours for individual input files.




----


Cool names for tools:

- `bezoar`  (Wikipedia: Bezoarsteine (DE))
- `auerhuhn` (Wikipedia: bird (DE))
- `jackdaw` (NL: "kauwtje" (family of blackbird))  (inspired by music: Blackbird & Crow)
- `troglodytes` (birds living in caves, e.g. Dutch "winterkoninkje") -- for extra fun compare `troglodytae` ("[cave dwellers](https://en.wikipedia.org/wiki/Troglodytae)") to `trogodytae` ([people living in Ethiopia and environs](https://oxfordre.com/classics/display/10.1093/acrefore/9780199381135.001.0001/acrefore-9780199381135-e-6581;jsessionid=1D13817E2D77A1285548CDFE2476E972)) --> `spelunking` --> `speleologica`
- `mela` ([Pomponius Mela](https://en.wikipedia.org/wiki/Pomponius_Mela) was the earliest *cartographer* whose name and output has persisted through history; many have preceded him, e.g. [Strabo](https://en.wikipedia.org/wiki/Strabo), and Roman legions, when traveling, measured their routes, thus producing the actual "graphing" of the world as they knew it, but none of those predecessors are referred as *cartographers*, rather *geographers*, *historians*, etc.)
* `statio`, `stationarii` (originally army barracks and their inhabitants, later the transportation management stations spread throughout the Roman Empire. See also [Stationarius (Roman military)](https://en.wikipedia.org/wiki/Stationarius_(Roman_military)), "[The Milites Stationarii considered in relation to the Hundred and Tithing of England](https://sci-hub.ru/10.1017/S0261340900006767)", ...)
* 






### Item ♯00003

