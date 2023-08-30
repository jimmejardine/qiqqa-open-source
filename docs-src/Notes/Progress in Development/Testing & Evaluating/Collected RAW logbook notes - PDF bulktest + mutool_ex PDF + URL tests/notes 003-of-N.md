# PDF bulktest + mutool_ex PDF + URL tests: logbook notes


# Test run notes at the bleeding edge

This is about the multiple test runs covering the `evil-base` PDF corpus: I've been collecting these notes over the years. **Big Caveat: these notes were valid at the time of writing, but MAY be obsolete or even counterdicting current behaviour at any later moment, sometimes even *seconds* away from the original event.**

This is about the things we observe when applying our tools at the bleeding edge of development to existing PDFs of all sorts, plus more or less wicked Internet URLs we checked out and the (grave) bugs that pop up most unexpectedly.

This is the lump sum notes (logbook) of these test runs' *odd observations*.

**The Table Of Contents / Overview Index is at [[PDF `bulktest` test run notes at the bleeding edge]].**

-------------------------------

(The logbook was started quite a while ago, back in 2020.)

*Here goes -- lower is later ==> updates are appended at the bottom.*

-------------------------------




## Extracts from the `bulktest` run logs: errors and curiosities








##### Item ♯00001


Stack overflow failure:

> Using previous log line for source info (as the actual logline below was broken due to buffered log I/O vs. hard crash:
> 
> `::SKIP-DUE-TO-RANDOM-SAMPLING: MUTOOL command: MUTOOL extract -o //?/J:/__bulktest-all/DATA/__bulktest/TextExtractFiles-T1/Y:\Qiqqa\Qiqqa-Alt3-old.D.drv\base\101B96DE-F964-44E7-ACD4-3EC8939BB83C\documents\2/2B43E0F081A99E8B15C7DA1783224E9BDCC5849/FULL-DOC.extract.dump -r Y:\Qiqqa\Qiqqa-Alt3-old.D.drv\base\101B96DE-F964-44E7-ACD4-3EC8939BB83C\documents\2\2B43E0F081A99E8B15C7DA1783224E9BDCC5849.pdf`


```
MUTOOL info -o //?/J:/__bulktest-all/DATA/__bulktest/TextExtractFiles-T1/Y:\Qiqqa\Qiqqa-Alt3-old.D.drv\base\101B96DE-F964-44E7-ACD4-3EC8939BB83C\documents\2/2B43E0F081A99E8B15C7DA1783224E9BDCC5849/FULL-DOC.info.txt -F -I -M -P -S -X Y:\Qiqqa\QiqqaException thrown at 0x00007FF7496EFBB4 in bulktest.exe: 0xC00000FD: Stack overflow (parameters: 0x0000000000000001, 0x000000124C2C3FF8).
Unhandled exception at 0x00007FF7496EFBB4 in bulktest.exe: 0xC00000FD: Stack overflow (parameters: 0x0000000000000001, 0x000000124C2C3FF8).
```










##### Item ♯00002


--> stack trace (partial from the top):


```
bulktest.exe!pdf_xref_len(fz_context * ctx, pdf_document * doc) Line 201
	at W:\Projects\sites\library.visyond.gov\80\lib\tooling\qiqqa\MuPDF\source\pdf\pdf-xref.c(201)
bulktest.exe!pdf_cache_object(fz_context * ctx, pdf_document * doc, int num) Line 2308
	at W:\Projects\sites\library.visyond.gov\80\lib\tooling\qiqqa\MuPDF\source\pdf\pdf-xref.c(2308)
bulktest.exe!pdf_resolve_indirect(fz_context * ctx, pdf_obj * ref) Line 2437
	at W:\Projects\sites\library.visyond.gov\80\lib\tooling\qiqqa\MuPDF\source\pdf\pdf-xref.c(2437)
bulktest.exe!pdf_resolve_indirect_chain(fz_context * ctx, pdf_obj * ref) Line 2464
	at W:\Projects\sites\library.visyond.gov\80\lib\tooling\qiqqa\MuPDF\source\pdf\pdf-xref.c(2464)
bulktest.exe!pdf_dict_get(fz_context * ctx, pdf_obj * obj, pdf_obj * key) Line 2048
	at W:\Projects\sites\library.visyond.gov\80\lib\tooling\qiqqa\MuPDF\source\pdf\pdf-object.c(2048)
bulktest.exe!pdf_dict_get_int(fz_context * ctx, pdf_obj * dict, pdf_obj * key) Line 4616
	at W:\Projects\sites\library.visyond.gov\80\lib\tooling\qiqqa\MuPDF\source\pdf\pdf-object.c(4616)
bulktest.exe!pdf_lookup_page_loc_imp(fz_context * ctx, pdf_document * doc, pdf_obj * node, int * skip, pdf_obj * * parentp, int * indexp) Line 285
	at W:\Projects\sites\library.visyond.gov\80\lib\tooling\qiqqa\MuPDF\source\pdf\pdf-page.c(285)
bulktest.exe!pdf_lookup_page_loc(fz_context * ctx, pdf_document * doc, int needle, pdf_obj * * parentp, int * indexp) Line 345
	at W:\Projects\sites\library.visyond.gov\80\lib\tooling\qiqqa\MuPDF\source\pdf\pdf-page.c(345)
bulktest.exe!pdf_lookup_page_obj(fz_context * ctx, pdf_document * doc, int needle) Line 361
	at W:\Projects\sites\library.visyond.gov\80\lib\tooling\qiqqa\MuPDF\source\pdf\pdf-page.c(361)
bulktest.exe!gatherresourceinfo(fz_context * ctx, pdf_mark_list * mark_list, globals * glo, int page, pdf_obj * rsrc, int show) Line 735
	at W:\Projects\sites\library.visyond.gov\80\lib\tooling\qiqqa\MuPDF\source\tools\pdfinfo.c(735)
bulktest.exe!gatherresourceinfo(fz_context * ctx, pdf_mark_list * mark_list, globals * glo, int page, pdf_obj * rsrc, int show) Line 779
	at W:\Projects\sites\library.visyond.gov\80\lib\tooling\qiqqa\MuPDF\source\tools\pdfinfo.c(779)
bulktest.exe!gatherresourceinfo(fz_context * ctx, pdf_mark_list * mark_list, globals * glo, int page, pdf_obj * rsrc, int show) Line 779
	at W:\Projects\sites\library.visyond.gov\80\lib\tooling\qiqqa\MuPDF\source\tools\pdfinfo.c(779)
bulktest.exe!gatherresourceinfo(fz_context * ctx, pdf_mark_list * mark_list, globals * glo, int page, pdf_obj * rsrc, int show) Line 779
	at W:\Projects\sites\library.visyond.gov\80\lib\tooling\qiqqa\MuPDF\source\tools\pdfinfo.c(779)
bulktest.exe!gatherresourceinfo(fz_context * ctx, pdf_mark_list * mark_list, globals * glo, int page, pdf_obj * rsrc, int show) Line 779
	at W:\Projects\sites\library.visyond.gov\80\lib\tooling\qiqqa\MuPDF\source\tools\pdfinfo.c(779)
bulktest.exe!gatherresourceinfo(fz_context * ctx, pdf_mark_list * mark_list, globals * glo, int page, pdf_obj * rsrc, int show) Line 779
	at W:\Projects\sites\library.visyond.gov\80\lib\tooling\qiqqa\MuPDF\source\tools\pdfinfo.c(779)
bulktest.exe!gatherresourceinfo(fz_context * ctx, pdf_mark_list * mark_list, globals * glo, int page, pdf_obj * rsrc, int show) Line 779
	at W:\Projects\sites\library.visyond.gov\80\lib\tooling\qiqqa\MuPDF\source\tools\pdfinfo.c(779)
bulktest.exe!gatherresourceinfo(fz_context * ctx, pdf_mark_list * mark_list, globals * glo, int page, pdf_obj * rsrc, int show) Line 779
	at W:\Projects\sites\library.visyond.gov\80\lib\tooling\qiqqa\MuPDF\source\tools\pdfinfo.c(779)
bulktest.exe!gatherresourceinfo(fz_context * ctx, pdf_mark_list * mark_list, globals * glo, int page, pdf_obj * rsrc, int show) Line 779
	at W:\Projects\sites\library.visyond.gov\80\lib\tooling\qiqqa\MuPDF\source\tools\pdfinfo.c(779)
[... ad nauseam ...]
```











##### Item ♯00003


lethargically slow?


```
T:01028:: Y:\Qiqqa\Qiqqa.Alt-2\evil-base\Meuk\ebooks.cleaned2\Cisco.Press.Cisco.OSPF.Command.and.Configuration.Handbook.CCIE.Professional.Development.pdf
```



> Also note that MSVC Debugger discovers a heap memory consumption at ~10GByte (!) after running through 1000 PDFs -- check the logfiles to find the memory consumers/leakers...


> > (**Postscript anno 2023**: this is possibly one of the occasions where I ran into those race conditions around JBIG2 image processing inside mupdf. Of course, the exact grounds for this event *are lost in the mysts of time*.)


(continued below, in the next item...)




##### Item ♯00004


Whoops, broke into the debugger after some time. Turns out the buffer is stuck on a lock (critical section): stacktrace:

```
ntdll.dll!00007ffd8f4bf9a4()
ntdll.dll!00007ffd8f427619()
ntdll.dll!00007ffd8f4274d2()
ntdll.dll!00007ffd8f4272fd()
ntdll.dll!00007ffd8f43b576()
ntdll.dll!00007ffd8f43b3c0()
bulktest.exe!boost::asio::detail::win_mutex::lock(void) Line 51
	at W:\Projects\sites\library.visyond.gov\80\lib\tooling\qiqqa\MuPDF\thirdparty\owemdjee\boost\libs\asio\include\boost\asio\detail\win_mutex.hpp(51)
bulktest.exe!fzoutput_lock(fz_output * out) Line 53
	at W:\Projects\sites\library.visyond.gov\80\lib\tooling\qiqqa\MuPDF\source\fitz\output.c(53)
bulktest.exe!fz_flush_output(fz_context * ctx, fz_output * out) Line 831
	at W:\Projects\sites\library.visyond.gov\80\lib\tooling\qiqqa\MuPDF\source\fitz\output.c(831)
bulktest.exe!fz_close_output(fz_context * ctx, fz_output * out) Line 711
	at W:\Projects\sites\library.visyond.gov\80\lib\tooling\qiqqa\MuPDF\source\fitz\output.c(711)
bulktest.exe!muraster_main(int argc, const char * * argv) Line 2315
	at W:\Projects\sites\library.visyond.gov\80\lib\tooling\qiqqa\MuPDF\source\tools\muraster.c(2315)
bulktest.exe!mutool_main(int argc, const char * * argv) Line 609
	at W:\Projects\sites\library.visyond.gov\80\lib\tooling\qiqqa\MuPDF\source\tools\mutool.c(609)
bulktest.exe!bulktest_main(int argc, const char * * argv) Line 2583
	at W:\Projects\sites\library.visyond.gov\80\lib\tooling\qiqqa\MuPDF\source\tools\bulktest.c(2583)
bulktest.exe!main(int argc, const char * * argv) Line 2833
	at W:\Projects\sites\library.visyond.gov\80\lib\tooling\qiqqa\MuPDF\source\tools\bulktest.c(2833)
bulktest.exe!invoke_main() Line 79
	at D:\a\_work\1\s\src\vctools\crt\vcstartup\src\startup\exe_common.inl(79)
bulktest.exe!__scrt_common_main_seh() Line 288
	at D:\a\_work\1\s\src\vctools\crt\vcstartup\src\startup\exe_common.inl(288)
bulktest.exe!__scrt_common_main() Line 331
	at D:\a\_work\1\s\src\vctools\crt\vcstartup\src\startup\exe_common.inl(331)
bulktest.exe!mainCRTStartup(void * __formal) Line 17
	at D:\a\_work\1\s\src\vctools\crt\vcstartup\src\startup\exe_main.cpp(17)
kernel32.dll!00007ffd8ded7bd4()
ntdll.dll!00007ffd8f48ce51()
```


> > (**Postscript anno 2023**: this is possibly one of the occasions where I ran into those race conditions around JBIG2 image processing inside mupdf. Of course, the exact grounds for this event *are lost in the mysts of time*.)









##### Item ♯00005


Offending command:


```
MUTOOL raster -F ppm -o //?/J:/__bulktest-all/DATA/__bulktest/TextExtractFiles-T1/Y:\Qiqqa\Qiqqa.Alt-2\evil-base\Meuk\ebooks.cleaned2/Cisco.Press.Cisco.OSPF.Command.and.Configuration.Handbook.CCIE.Professional.Development/%04d.raster-x150.ppm -s mt -r 150 -P Y:\Qiqqa\Qiqqa.Alt-2\evil-base\Meuk\ebooks.cleaned2\Cisco.Press.Cisco.OSPF.Command.and.Configuration.Handbook.CCIE.Professional.Development.pdf
```










##### Item ♯00006


Last bit of dbg output at the time of lockup:


```
Glyph Cache Evictions: 0 (0 bytes)
page Y:\Qiqqa\Qiqqa.Alt-2\evil-base\Meuk\ebooks.cleaned2\Cisco.Press.Cisco.OSPF.Command.and.Configuration.Handbook.CCIE.Professional.Development.pdf 680
 pagenum=680 :: 4ms (interpretation) 324ms (rendering) 328ms (total)
Glyph Cache Size: 339810
Glyph Cache Evictions: 0 (0 bytes)
page Y:\Qiqqa\Qiqqa.Alt-2\evil-base\Meuk\ebooks.cleaned2\Cisco.Press.Cisco.OSPF.Command.and.Configuration.Handbook.CCIE.Professional.Development.pdf 681
error: cannot fwrite: No space left on device (written 0 of 8192 bytes)
 pagenum=681 :: 4ms (interpretation) 168ms (rendering) 172ms (total)
Glyph Cache Size: 339810
Glyph Cache Evictions: 0 (0 bytes)
error: Failed to render page
error: cannot draw 'Y:\Qiqqa\Qiqqa.Alt-2\evil-base\Meuk\ebooks.cleaned2\Cisco.Press.Cisco.OSPF.Command.and.Configuration.Handbook.CCIE.Professional.Development.pdf': Failed to render page
total -2503ms (1ms layout) / 681 pages for an average of -3ms
fastest page 681: 4ms (interpretation) 168ms (rendering) 172ms(total)
slowest page 159: 6ms (interpretation) 414ms (rendering) 420ms(total)
Lock 0 held for 663.47726 seconds (303.12928%)
Lock 1 held for 17.749203 seconds (8.10925%)
Lock 2 held for 1680.3442 seconds (767.7152%)
Lock 3 held for 11.588128 seconds (5.29438%)
Lock 4 held for 0 seconds (0%)
Lock 5 held for 37.609975 seconds (1The thread 0x99cc has exited with code 0 (0x0).
7.183234%)
Total program time 218.876 seconds
The thread 0xab04 has exited with code 0 (0x0).
The thread 0x4d6c has exited with code 0 (0x0).
The thread 0xa5a8 has exited with code 0 (0x0).
The thread 0x6b6c has exited with code 0 (0x0).
The thread 0x63f0 has exited with code 0 (0x0).
The thread 0x32c4 has exited with code 0 (0x0).
The thread 0x260c has exited with code 0 (0x0).
The thread 0x6004 has exited with code 0 (0x0).
The thread 0xa668 has exited with code 0 (0x0).
The thread 0x7d30 has exited with code 0 (0x0).
The thread 0xb3d8 has exited with code 0 (0x0).
The thread 0x9a38 has exited with code 0 (0x0).
The thread 0x6e58 has exited with code 0 (0x0).
The thread 0xa9ec has exited with code 0 (0x0).
The thread 0xaf98 has exited with code 0 (0x0).
The thread 0x4c44 has exited with code 0 (0x0).
```



**_huh?_** "**cannot fwrite: No space left on device**"? There's still *299GB* space left on the output/scratch HDD and none of the other HDDs is full either. What is going on here?







##### Item ♯00007


Further investigation: the line "error: cannot fwrite: No space left on device" didn't end up in the logfile, while what came after DID land in there:


```
Glyph Cache Size: 339810
Glyph Cache Evictions: 0 (0 bytes)
page Y:\Qiqqa\Qiqqa.Alt-2\evil-base\Meuk\ebooks.cleaned2\Cisco.Press.Cisco.OSPF.Command.and.Configuration.Handbook.CCIE.Professional.Development.pdf 680
 pagenum=680 :: 4ms (interpretation) 324ms (rendering) 328ms (total)
Glyph Cache Size: 339810
Glyph Cache Evictions: 0 (0 bytes)
page Y:\Qiqqa\Qiqqa.Alt-2\evil-base\Meuk\ebooks.cleaned2\Cisco.Press.Cisco.OSPF.Command.and.Configuration.Handbook.CCIE.Professional.Development.pdf 681
error: cannot fwrite: No space left on device (written 0 of 8192 bytes)
 pagenum=681 :: 4ms (interpretation) 168ms (rendering) 172ms (total)
Glyph Cache Size: 339810
Glyph Cache Evictions: 0 (0 bytes)
error: Failed to render page
error: cannot draw 'Y:\Qiqqa\Qiqqa.Alt-2\evil-base\Meuk\ebooks.cleaned2\Cisco.Press.Cisco.OSPF.Command.and.Configuration.Handbook.CCIE.Professional.Development.pdf': Failed to render page
```



^^^^^^^ a later revision of `bulktest` and further analysis of this pesky 'no space left' error revealed a couple of things (one of which I had completely forgotten, but -- as a test -- is a nice touch in hindsight:

- the output target HDD is an old 'burner', i.e. it was picked for this job as I won't cry a rainbow if the HDD fails due to excessive writing or related mishap. For the `bulktest` it is deemed sufficiently large (~500GB free space) and fast enough to be 'representable' of average user equipment, adding a bit of a sense of cost to the numbers.
- that target HDD is NOT NTFS formatted but instead is still **FAT32** format -- which makes it impossible to write any file larger than 4GByte. While that is fine for a document / logging / image test disk, it so happens that....

(continued below)







##### Item ♯00008



```
MUTOOL raster -F ppm -o //?/J:/__bulktest-all/DATA/__bulktest/TextExtractFiles-T1/Y:\Qiqqa\Qiqqa.Alt-2\evil-base\Meuk\ebooks.cleaned2/Cisco.Press.Cisco.OSPF.Command.and.Configuration.Handbook.CCIE.Professional.Development/%04d.raster-x150.ppm -s mt -r 150 -P Y:\Qiqqa\Qiqqa.Alt-2\evil-base\Meuk\ebooks.cleaned2\Cisco.Press.Cisco.OSPF.Command.and.Configuration.Handbook.CCIE.Professional.Development.pdf
```


DOES surpass the 4GB mark for the PPM file once the renderer is very close to the last of the last page in this large document!

(last bit of log that made it to disk before we killed the bastard in the debugger)







##### Item ♯00009


```
page Y:\Qiqqa\Qiqqa.Alt-2\evil-base\Meuk\ebooks.cleaned2\Cisco.Press.Cisco.OSPF.Command.and.Configuration.Handbook.CCIE.Professional.Development.pdf 679
 pagenum=679 :: 4ms (interpretation) 325ms (rendering) 329ms (total)
Glyph Cache Size: 339810
Glyph Cache Evictions: 0 (0 bytes)
page Y:\Qiqqa\Qiqqa.Alt-2\evil-base\Meuk\ebooks.cleaned2\Cisco.Press.Cisco.OSPF.Command.and.Configuration.Handbook.CCIE.Professional.Development.pdf 680
 pagenum=680 :: 3ms (interpretation) 304ms (rendering) 307ms (total)
Glyph Cache Size: 339810
Glyph Cache Evictions: 0 (0 bytes)
page Y:\Qiqqa\Qiqqa.Alt-2\evil-base\Meuk\ebooks.cleaned2\Cisco.Press.
```


Meanwhile the system error message was obscuring this issue, so some analysis and error message augmentation code has been added to the core (`analyze_and_improve_fwrite_error()`) as this error may be rare, but it's buggers like these, combined with low quality error reporting, that eat your time like mad, so for maintenance I consider it cost-effective to add some hints to the error report, in case this happens again. Debugging this instance took (thanks in part to the time needed to write near 4BG PPM formatted data via `fz_output`: a few minutes for each run) several hours already so I'd rather see a very helpful reminder/hint at the root cause next time I run this test suite -- and chances are it'll be running many times from now on!







##### Item ♯00010


```
Y:\Qiqqa\Qiqqa.Alt-2\evil-base\Meuk\ebooks.technical\Journal of Pharmaceutical and Biomedical Analysis [1983 - 2012]\2012 (Vol 62)\140-148.pdf
```


--> `muraster` renders these pages incompletely?


