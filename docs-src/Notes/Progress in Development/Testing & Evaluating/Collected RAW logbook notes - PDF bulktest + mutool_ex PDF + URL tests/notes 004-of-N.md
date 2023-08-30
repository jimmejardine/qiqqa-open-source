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










##### Item ♯00001


`mupdf` memory issue:

```
muconvert INTRANET_E57B9774-4712-430E-93E0-E67433F7DF07/documents/4/458B1F6296CFA32442F7CC47752231F74B89E54D.pdf
```















##### Item ♯00001


```
MUTOOL raster -F png -o //?/J:/__bulktest-all/DATA/__bulktest/TextExtractFiles-T1/Y:\Qiqqa\Qiqqa-Q-TestDrive\Qiqqa\base\C9F0D079-EE4C-4A15-8547-72164A7A356D\documents\A/AE4528C0F0CB2DEFE9CCF589FC78E736C68ECFDA/%04d-raster-x150.png -s mt -r 150 -w 3840 -h 3840 -P Y:\Qiqqa\Qiqqa-Q-TestDrive\Qiqqa\base\C9F0D079-EE4C-4A15-8547-72164A7A356D\documents\A\AE4528C0F0CB2DEFE9CCF589FC78E736C68ECFDA.pdf
```

--> this completely b0rks the PNG band write phase (assertion failure); debugging/analysis shows we're trying to dump a 1-plane pix immediately into a 3-plane (RBG) PNG format writer without conversion and that's where the negative `stride` value results: 

`stride = 1257` in `fz_write_band()`

then we get into `png_write_band()` -- where we DO NOT KNOW the originating `pix` -- where `writer->super.n == 3` and `writer->super.w == 1257`, resulting in `stride -= w*n;` screwing the pooch, thanks to the `n` mismatch with the original/incoming data.

KABOOM!

-->

```
MUTOOL raster -F png -o //?/J:/__bulktest-all/DATA/__bulktest/TextExtractFiles-T1/Y:\Qiqqa\Qiqqa-Q-TestDrive\Qiqqa\base\C9F0D079-EE4C-4A15-8547-72164A7A356D\documents\A/AE4528C0F0CB2DEFE9CCF589FC78E736C68ECFDA/%04d-raster-x150.png -s mt -r 150 -w 3840 -h 3840 -P Y:\Qiqqa\Qiqqa-Q-TestDrive\Qiqqa\base\C9F0D079-EE4C-4A15-8547-72164A7A356D\documents\A\AE4528C0F0CB2DEFE9CCF589FC78E736C68ECFDA.pdf
```

--> fails. nasty abort(). Looks like MSVC again overrode my custom assertions.h -- the assert.h header file doesn't have a load-once wrapper nor `#pragma once` which makes it a pure POS!

hrmmmm :-(((

`-P` multi-threaded mode in `muraster` is b0rked. Probably more b0rk in there than just that, as I've seen some damaged output previously...









##### Item ♯00001

???

```
:L#00229: MUTOOL clean -gggg -D -c -s -AA Y:\Qiqqa\SSD-0001\Qiqqa\backups\QiqqaBackup.20190815011657\Guest\documents\C\CB19C4413F474A7F345CA3EE99AF46B4AF16E93F.pdf //?/J:/__bulktest-all/DATA/__bulktest/TextExtractFiles-T1/Y:\Qiqqa\SSD-0001\Qiqqa\backups\QiqqaBackup.20190815011657\Guest\documents\C/CB19C4413F474A7F345CA3EE99AF46B4AF16E93F/FULL-DOC.clean.pdf
warning: openjpeg warning: Non conformant codestream TPsot==TNsot.
warning: ... repeated 58 times...
warning: deduplication cost pathological at O(146444498)?
```
















##### Item ♯00001

Another what-the-heck?!...

"Permission denied"?!  target HDD is still half empty (200GB+ free); there's currently ~13K files in that destination directory, so even FAT32 should be okay with us dumping a crazy PDF that apparently has its content stored as single-scanline images, resulting in a metric ton of 1KByte-a-piece PNG files.

Hm, forgot to check: did we possibly run out of file handles due so oddity I haven't noticed yet? error at 13K sounds like that...  :thinking:


```
extracting //?/J:/__bulktest-all/DATA/__bulktest/TextExtractFiles-T1/Y_/Qiqqa/Qiqqa-Q-TestDrive/Qiqqa/base/INTRANET_EF52564A-831D-42F2-B956-815CF0418C08/documents/6/68CC556AEB46AF68D067F9331AED462CB44C78B/FULL-DOC.extract.dumpimage-13689-13091.png
extracting //?/J:/__bulktest-all/DATA/__bulktest/TextExtractFiles-T1/Y_/Qiqqa/Qiqqa-Q-TestDrive/Qiqqa/base/INTRANET_EF52564A-831D-42F2-B956-815CF0418C08/documents/6/68CC556AEB46AF68D067F9331AED462CB44C78B/FULL-DOC.extract.dumpimage-13690-13092.png
extracting //?/J:/__bulktest-all/DATA/__bulktest/TextExtractFiles-T1/Y_/Qiqqa/Qiqqa-Q-TestDrive/Qiqqa/base/INTRANET_EF52564A-831D-42F2-B956-815CF0418C08/documents/6/68CC556AEB46AF68D067F9331AED462CB44C78B/FULL-DOC.extract.dumpimage-13691-13093.png
extracting //?/J:/__bulktest-all/DATA/__bulktest/TextExtractFiles-T1/Y_/Qiqqa/Qiqqa-Q-TestDrive/Qiqqa/base/INTRANET_EF52564A-831D-42F2-B956-815CF0418C08/documents/6/68CC556AEB46AF68D067F9331AED462CB44C78B/FULL-DOC.extract.dumpimage-13692-13094.png
extracting //?/J:/__bulktest-all/DATA/__bulktest/TextExtractFiles-T1/Y_/Qiqqa/Qiqqa-Q-TestDrive/Qiqqa/base/INTRANET_EF52564A-831D-42F2-B956-815CF0418C08/documents/6/68CC556AEB46AF68D067F9331AED462CB44C78B/FULL-DOC.extract.dumpimage-13693-13095.png
extracting //?/J:/__bulktest-all/DATA/__bulktest/TextExtractFiles-T1/Y_/Qiqqa/Qiqqa-Q-TestDrive/Qiqqa/base/INTRANET_EF52564A-831D-42F2-B956-815CF0418C08/documents/6/68CC556AEB46AF68D067F9331AED462CB44C78B/FULL-DOC.extract.dumpimage-13694-13096.png
extracting //?/J:/__bulktest-all/DATA/__bulktest/TextExtractFiles-T1/Y_/Qiqqa/Qiqqa-Q-TestDrive/Qiqqa/base/INTRANET_EF52564A-831D-42F2-B956-815CF0418C08/documents/6/68CC556AEB46AF68D067F9331AED462CB44C78B/FULL-DOC.extract.dumpimage-13695-13097.png
extracting //?/J:/__bulktest-all/DATA/__bulktest/TextExtractFiles-T1/Y_/Qiqqa/Qiqqa-Q-TestDrive/Qiqqa/base/INTRANET_EF52564A-831D-42F2-B956-815CF0418C08/documents/6/68CC556AEB46AF68D067F9331AED462CB44C78B/FULL-DOC.extract.dumpimage-13696-13098.png
extracting //?/J:/__bulktest-all/DATA/__bulktest/TextExtractFiles-T1/Y_/Qiqqa/Qiqqa-Q-TestDrive/Qiqqa/base/INTRANET_EF52564A-831D-42F2-B956-815CF0418C08/documents/6/68CC556AEB46AF68D067F9331AED462CB44C78B/FULL-DOC.extract.dumpimage-13697-13099.png
extracting //?/J:/__bulktest-all/DATA/__bulktest/TextExtractFiles-T1/Y_/Qiqqa/Qiqqa-Q-TestDrive/Qiqqa/base/INTRANET_EF52564A-831D-42F2-B956-815CF0418C08/documents/6/68CC556AEB46AF68D067F9331AED462CB44C78B/FULL-DOC.extract.dumpimage-13698-13100.png
extracting //?/J:/__bulktest-all/DATA/__bulktest/TextExtractFiles-T1/Y_/Qiqqa/Qiqqa-Q-TestDrive/Qiqqa/base/INTRANET_EF52564A-831D-42F2-B956-815CF0418C08/documents/6/68CC556AEB46AF68D067F9331AED462CB44C78B/FULL-DOC.extract.dumpimage-13699-13101.png
extracting //?/J:/__bulktest-all/DATA/__bulktest/TextExtractFiles-T1/Y_/Qiqqa/Qiqqa-Q-TestDrive/Qiqqa/base/INTRANET_EF52564A-831D-42F2-B956-815CF0418C08/documents/6/68CC556AEB46AF68D067F9331AED462CB44C78B/FULL-DOC.extract.dumpimage-13700-13102.png
extracting //?/J:/__bulktest-all/DATA/__bulktest/TextExtractFiles-T1/Y_/Qiqqa/Qiqqa-Q-TestDrive/Qiqqa/base/INTRANET_EF52564A-831D-42F2-B956-815CF0418C08/documents/6/68CC556AEB46AF68D067F9331AED462CB44C78B/FULL-DOC.extract.dumpimage-13701-13103.png
extracting //?/J:/__bulktest-all/DATA/__bulktest/TextExtractFiles-T1/Y_/Qiqqa/Qiqqa-Q-TestDrive/Qiqqa/base/INTRANET_EF52564A-831D-42F2-B956-815CF0418C08/documents/6/68CC556AEB46AF68D067F9331AED462CB44C78B/FULL-DOC.extract.dumpimage-13702-13104.png
extracting //?/J:/__bulktest-all/DATA/__bulktest/TextExtractFiles-T1/Y_/Qiqqa/Qiqqa-Q-TestDrive/Qiqqa/base/INTRANET_EF52564A-831D-42F2-B956-815CF0418C08/documents/6/68CC556AEB46AF68D067F9331AED462CB44C78B/FULL-DOC.extract.dumpimage-13703-13105.png
extracting //?/J:/__bulktest-all/DATA/__bulktest/TextExtractFiles-T1/Y_/Qiqqa/Qiqqa-Q-TestDrive/Qiqqa/base/INTRANET_EF52564A-831D-42F2-B956-815CF0418C08/documents/6/68CC556AEB46AF68D067F9331AED462CB44C78B/FULL-DOC.extract.dumpimage-13704-13106.png
extracting //?/J:/__bulktest-all/DATA/__bulktest/TextExtractFiles-T1/Y_/Qiqqa/Qiqqa-Q-TestDrive/Qiqqa/base/INTRANET_EF52564A-831D-42F2-B956-815CF0418C08/documents/6/68CC556AEB46AF68D067F9331AED462CB44C78B/FULL-DOC.extract.dumpimage-13705-13107.png
error: cannot open file '//?/J:/__bulktest-all/DATA/__bulktest/TextExtractFiles-T1/Y_/Qiqqa/Qiqqa-Q-TestDrive/Qiqqa/base/INTRANET_EF52564A-831D-42F2-B956-815CF0418C08/documents/6/68CC556AEB46AF68D067F9331AED462CB44C78B/FULL-DOC.extract.dumpimage-13705-13107.png': rtl error: Permission denied
warning: ignoring object 13705
extracting //?/J:/__bulktest-all/DATA/__bulktest/TextExtractFiles-T1/Y_/Qiqqa/Qiqqa-Q-TestDrive/Qiqqa/base/INTRANET_EF52564A-831D-42F2-B956-815CF0418C08/documents/6/68CC556AEB46AF68D067F9331AED462CB44C78B/FULL-DOC.extract.dumpimage-13706-13108.png
error: cannot open file '//?/J:/__bulktest-all/DATA/__bulktest/TextExtractFiles-T1/Y_/Qiqqa/Qiqqa-Q-TestDrive/Qiqqa/base/INTRANET_EF52564A-831D-42F2-B956-815CF0418C08/documents/6/68CC556AEB46AF68D067F9331AED462CB44C78B/FULL-DOC.extract.dumpimage-13706-13108.png': rtl error: Permission denied
warning: ignoring object 13706
extracting //?/J:/__bulktest-all/DATA/__bulktest/TextExtractFiles-T1/Y_/Qiqqa/Qiqqa-Q-TestDrive/Qiqqa/base/INTRANET_EF52564A-831D-42F2-B956-815CF0418C08/documents/6/68CC556AEB46AF68D067F9331AED462CB44C78B/FULL-DOC.extract.dumpimage-13707-13109.png
error: cannot open file '//?/J:/__bulktest-all/DATA/__bulktest/TextExtractFiles-T1/Y_/Qiqqa/Qiqqa-Q-TestDrive/Qiqqa/base/INTRANET_EF52564A-831D-42F2-B956-815CF0418C08/documents/6/68CC556AEB46AF68D067F9331AED462CB44C78B/FULL-DOC.extract.dumpimage-13707-13109.png': rtl error: Permission denied
warning: ignoring object 13707
extracting //?/J:/__bulktest-all/DATA/__bulktest/TextExtractFiles-T1/Y_/Qiqqa/Qiqqa-Q-TestDrive/Qiqqa/base/INTRANET_EF52564A-831D-42F2-B956-815CF0418C08/documents/6/68CC556AEB46AF68D067F9331AED462CB44C78B/FULL-DOC.extract.dumpimage-13708-13110.png
error: cannot open file '//?/J:/__bulktest-all/DATA/__bulktest/TextExtractFiles-T1/Y_/Qiqqa/Qiqqa-Q-TestDrive/Qiqqa/base/INTRANET_EF52564A-831D-42F2-B956-815CF0418C08/documents/6/68CC556AEB46AF68D067F9331AED462CB44C78B/FULL-DOC.extract.dumpimage-13708-13110.png': rtl error: Permission denied
warning: ignoring object 13708
extracting //?/J:/__bulktest-all/DATA/__bulktest/TextExtractFiles-T1/Y_/Qiqqa/Qiqqa-Q-TestDrive/Qiqqa/base/INTRANET_EF52564A-831D-42F2-B956-815CF0418C08/documents/6/68CC556AEB46AF68D067F9331AED462CB44C78B/FULL-DOC.extract.dumpimage-13709-13111.png
error: cannot open file '//?/J:/__bulktest-all/DATA/__bulktest/TextExtractFiles-T1/Y_/Qiqqa/Qiqqa-Q-TestDrive/Qiqqa/base/INTRANET_EF52564A-831D-42F2-B956-815CF0418C08/documents/6/68CC556AEB46AF68D067F9331AED462CB44C78B/FULL-DOC.extract.dumpimage-13709-13111.png': rtl error: Permission denied
warning: ignoring object 13709
extracting //?/J:/__bulktest-all/DATA/__bulktest/TextExtractFiles-T1/Y_/Qiqqa/Qiqqa-Q-TestDrive/Qiqqa/base/INTRANET_EF52564A-831D-42F2-B956-815CF0418C08/documents/6/68CC556AEB46AF68D067F9331AED462CB44C78B/FULL-DOC.extract.dumpimage-13710-13112.png
error: cannot open file '//?/J:/__bulktest-all/DATA/__bulktest/TextExtractFiles-T1/Y_/Qiqqa/Qiqqa-Q-TestDrive/Qiqqa/base/INTRANET_EF52564A-831D-42F2-B956-815CF0418C08/documents/6/68CC556AEB46AF68D067F9331AED462CB44C78B/FULL-DOC.extract.dumpimage-13710-13112.png': rtl error: Permission denied
warning: ignoring object 13710
extracting //?/J:/__bulktest-all/DATA/__bulktest/TextExtractFiles-T1/Y_/Qiqqa/Qiqqa-Q-TestDrive/Qiqqa/base/INTRANET_EF52564A-831D-42F2-B956-815CF0418C08/documents/6/68CC556AEB46AF68D067F9331AED462CB44C78B/FULL-DOC.extract.dumpimage-13711-13113.png
error: cannot open file '//?/J:/__bulktest-all/DATA/__bulktest/TextExtractFiles-T1/Y_/Qiqqa/Qiqqa-Q-TestDrive/Qiqqa/base/INTRANET_EF52564A-831D-42F2-B956-815CF0418C08/documents/6/68CC556AEB46AF68D067F9331AED462CB44C78B/FULL-DOC.extract.dumpimage-13711-13113.png': rtl error: Permission denied
warning: ignoring object 13711
extracting //?/J:/__bulktest-all/DATA/__bulktest/TextExtractFiles-T1/Y_/Qiqqa/Qiqqa-Q-TestDrive/Qiqqa/base/INTRANET_EF52564A-831D-42F2-B956-815CF0418C08/documents/6/68CC556AEB46AF68D067F9331AED462CB44C78B/FULL-DOC.extract.dumpimage-13712-13114.png
error: cannot open file '//?/J:/__bulktest-all/DATA/__bulktest/TextExtractFiles-T1/Y_/Qiqqa/Qiqqa-Q-TestDrive/Qiqqa/base/INTRANET_EF52564A-831D-42F2-B956-815CF0418C08/documents/6/68CC556AEB46AF68D067F9331AED462CB44C78B/FULL-DOC.extract.dumpimage-13712-13114.png': rtl error: Permission denied
warning: ignoring object 13712

[...]

warning: ignoring object 31601
extracting //?/J:/__bulktest-all/DATA/__bulktest/TextExtractFiles-T1/Y_/Qiqqa/Qiqqa-Q-TestDrive/Qiqqa/base/INTRANET_EF52564A-831D-42F2-B956-815CF0418C08/documents/6/68CC556AEB46AF68D067F9331AED462CB44C78B/FULL-DOC.extract.dumpimage-31602-31004.png
error: cannot open file '//?/J:/__bulktest-all/DATA/__bulktest/TextExtractFiles-T1/Y_/Qiqqa/Qiqqa-Q-TestDrive/Qiqqa/base/INTRANET_EF52564A-831D-42F2-B956-815CF0418C08/documents/6/68CC556AEB46AF68D067F9331AED462CB44C78B/FULL-DOC.extract.dumpimage-31602-31004.png': rtl error: Permission denied
warning: ignoring object 31602
extracting //?/J:/__bulktest-all/DATA/__bulktest/TextExtractFiles-T1/Y_/Qiqqa/Qiqqa-Q-TestDrive/Qiqqa/base/INTRANET_EF52564A-831D-42F2-B956-815CF0418C08/documents/6/68CC556AEB46AF68D067F9331AED462CB44C78B/FULL-DOC.extract.dumpimage-31603-31005.png
error: cannot open file '//?/J:/__bulktest-all/DATA/__bulktest/TextExtractFiles-T1/Y_/Qiqqa/Qiqqa-Q-TestDrive/Qiqqa/base/INTRANET_EF52564A-831D-42F2-B956-815CF0418C08/documents/6/68CC556AEB46AF68D067F9331AED462CB44C78B/FULL-DOC.extract.dumpimage-31603-31005.png': rtl error: Permission denied
warning: ignoring object 31603
extracting //?/J:/__bulktest-all/DATA/__bulktest/TextExtractFiles-T1/Y_/Qiqqa/Qiqqa-Q-TestDrive/Qiqqa/base/INTRANET_EF52564A-831D-42F2-B956-815CF0418C08/documents/6/68CC556AEB46AF68D067F9331AED462CB44C78B/FULL-DOC.extract.dumpimage-31604-31006.png
error: cannot open file '//?/J:/__bulktest-all/DATA/__bulktest/TextExtractFiles-T1/Y_/Qiqqa/Qiqqa-Q-TestDrive/Qiqqa/base/INTRANET_EF52564A-831D-42F2-B956-815CF0418C08/documents/6/68CC556AEB46AF68D067F9331AED462CB44C78B/FULL-DOC.extract.dumpimage-31604-31006.png': rtl error: Permission denied
warning: ignoring object 31604
extracting //?/J:/__bulktest-all/DATA/__bulktest/TextExtractFiles-T1/Y_/Qiqqa/Qiqqa-Q-TestDrive/Qiqqa/base/INTRANET_EF52564A-831D-42F2-B956-815CF0418C08/documents/6/68CC556AEB46AF68D067F9331AED462CB44C78B/FULL-DOC.extract.dumpimage-31605-31007.png
error: cannot open file '//?/J:/__bulktest-all/DATA/__bulktest/TextExtractFiles-T1/Y_/Qiqqa/Qiqqa-Q-TestDrive/Qiqqa/base/INTRANET_EF52564A-831D-42F2-B956-815CF0418C08/documents/6/68CC556AEB46AF68D067F9331AED462CB44C78B/FULL-DOC.extract.dumpimage-31605-31007.png': rtl error: Permission denied
warning: ignoring object 31605
extracting //?/J:/__bulktest-all/DATA/__bulktest/TextExtractFiles-T1/Y_/Qiqqa/Qiqqa-Q-TestDrive/Qiqqa/base/INTRANET_EF52564A-831D-42F2-B956-815CF0418C08/documents/6/68CC556AEB46AF68D067F9331AED462CB44C78B/FULL-DOC.extract.dumpimage-31606-31008.png
error: cannot open file '//?/J:/__bulktest-all/DATA/__bulktest/TextExtractFiles-T1/Y_/Qiqqa/Qiqqa-Q-TestDrive/Qiqqa/base/INTRANET_EF52564A-831D-42F2-B956-815CF0418C08/documents/6/68CC556AEB46AF68D067F9331AED462CB44C78B/FULL-DOC.extract.dumpimage-31606-31008.png': rtl error: Permission denied
warning: ignoring object 31606
extracting //?/J:/__bulktest-all/DATA/__bulktest/TextExtractFiles-T1/Y_/Qiqqa/Qiqqa-Q-TestDrive/Qiqqa/base/INTRANET_EF52564A-831D-42F2-B956-815CF0418C08/documents/6/68CC556AEB46AF68D067F9331AED462CB44C78B/FULL-DOC.extract.dumpimage-31607-31009.png
error: cannot open file '//?/J:/__bulktest-all/DATA/__bulktest/TextExtractFiles-T1/Y_/Qiqqa/Qiqqa-Q-TestDrive/Qiqqa/base/INTRANET_EF52564A-831D-42F2-B956-815CF0418C08/documents/6/68CC556AEB46AF68D067F9331AED462CB44C78B/FULL-DOC.extract.dumpimage-31607-31009.png': rtl error: Permission denied
warning: ignoring object 31607
extracting //?/J:/__bulktest-all/DATA/__bulktest/TextExtractFiles-T1/Y_/Qiqqa/Qiqqa-Q-TestDrive/Qiqqa/base/INTRANET_EF52564A-831D-42F2-B956-815CF0418C08/documents/6/68CC556AEB46AF68D067F9331AED462CB44C78B/FULL-DOC.extract.dumpimage-31608-31010.png
error: cannot open file '//?/J:/__bulktest-all/DATA/__bulktest/TextExtractFiles-T1/Y_/Qiqqa/Qiqqa-Q-TestDrive/Qiqqa/base/INTRANET_EF52564A-831D-42F2-B956-815CF0418C08/documents/6/68CC556AEB46AF68D067F9331AED462CB44C78B/FULL-DOC.extract.dumpimage-31608-31010.png': rtl error: Permission denied
warning: ignoring object 31608
extracting //?/J:/__bulktest-all/DATA/__bulktest/TextExtractFiles-T1/Y_/Qiqqa/Qiqqa-Q-TestDrive/Qiqqa/base/INTRANET_EF52564A-831D-42F2-B956-815CF0418C08/documents/6/68CC556AEB46AF68D067F9331AED462CB44C78B/FULL-DOC.extract.dumpimage-31609-31011.png
error: cannot open file '//?/J:/__bulktest-all/DATA/__bulktest/TextExtractFiles-T1/Y_/Qiqqa/Qiqqa-Q-TestDrive/Qiqqa/base/INTRANET_EF52564A-831D-42F2-B956-815CF0418C08/documents/6/68CC556AEB46AF68D067F9331AED462CB44C78B/FULL-DOC.extract.dumpimage-31609-31011.png': rtl error: Permission denied
warning: ignoring object 31609
extracting //?/J:/__bulktest-all/DATA/__bulktest/TextExtractFiles-T1/Y_/Qiqqa/Qiqqa-Q-TestDrive/Qiqqa/base/INTRANET_EF52564A-831D-42F2-B956-815CF0418C08/documents/6/68CC556AEB46AF68D067F9331AED462CB44C78B/FULL-DOC.extract.dumpimage-31610-31012.png
error: cannot open file '//?/J:/__bulktest-all/DATA/__bulktest/TextExtractFiles-T1/Y_/Qiqqa/Qiqqa-Q-TestDrive/Qiqqa/base/INTRANET_EF52564A-831D-42F2-B956-815CF0418C08/documents/6/68CC556AEB46AF68D067F9331AED462CB44C78B/FULL-DOC.extract.dumpimage-31610-31012.png': rtl error: Permission denied
warning: ignoring object 31610
extracting //?/J:/__bulktest-all/DATA/__bulktest/TextExtractFiles-T1/Y_/Qiqqa/Qiqqa-Q-TestDrive/Qiqqa/base/INTRANET_EF52564A-831D-42F2-B956-815CF0418C08/documents/6/68CC556AEB46AF68D067F9331AED462CB44C78B/FULL-DOC.extract.dumpimage-31611-31013.png
error: cannot open file '//?/J:/__bulktest-all/DATA/__bulktest/TextExtractFiles-T1/Y_/Qiqqa/Qiqqa-Q-TestDrive/Qiqqa/base/INTRANET_EF52564A-831D-42F2-B956-815CF0418C08/documents/6/68CC556AEB46AF68D067F9331AED462CB44C78B/FULL-DOC.extract.dumpimage-31611-31013.png': rtl error: Permission denied
warning: ignoring object 31611
extracting //?/J:/__bulktest-all/DATA/__bulktest/TextExtractFiles-T1/Y_/Qiqqa/Qiqqa-Q-TestDrive/Qiqqa/base/INTRANET_EF52564A-831D-42F2-B956-815CF0418C08/documents/6/68CC556AEB46AF68D067F9331AED462CB44C78B/FULL-DOC.extract.dumpimage-31612-31014.png
error: cannot open file '//?/J:/__bulktest-all/DATA/__bulktest/TextExtractFiles-T1/Y_/Qiqqa/Qiqqa-Q-TestDrive/Qiqqa/base/INTRANET_EF52564A-831D-42F2-B956-815CF0418C08/documents/6/68CC556AEB46AF68D067F9331AED462CB44C78B/FULL-DOC.extract.dumpimage-31612-31014.png': rtl error: Permission denied
warning: ignoring object 31612
extracting //?/J:/__bulktest-all/DATA/__bulktest/TextExtractFiles-T1/Y_/Qiqqa/Qiqqa-Q-TestDrive/Qiqqa/base/INTRANET_EF52564A-831D-42F2-B956-815CF0418C08/documents/6/68CC556AEB46AF68D067F9331AED462CB44C78B/FULL-DOC.extract.dumpimage-31613-31015.png
error: cannot open file '//?/J:/__bulktest-all/DATA/__bulktest/TextExtractFiles-T1/Y_/Qiqqa/Qiqqa-Q-TestDrive/Qiqqa/base/INTRANET_EF52564A-831D-42F2-B956-815CF0418C08/documents/6/68CC556AEB46AF68D067F9331AED462CB44C78B/FULL-DOC.extract.dumpimage-31613-31015.png': rtl error: Permission denied
warning: ignoring object 31613
extracting //?/J:/__bulktest-all/DATA/__bulktest/TextExtractFiles-T1/Y_/Qiqqa/Qiqqa-Q-TestDrive/Qiqqa/base/INTRANET_EF52564A-831D-42F2-B956-815CF0418C08/documents/6/68CC556AEB46AF68D067F9331AED462CB44C78B/FULL-DOC.extract.dumpimage-31614-31016.png
error: cannot open file '//?/J:/__bulktest-all/DATA/__bulktest/TextExtractFiles-T1/Y_/Qiqqa/Qiqqa-Q-TestDrive/Qiqqa/base/INTRANET_EF52564A-831D-42F2-B956-815CF0418C08/documents/6/68CC556AEB46AF68D067F9331AED462CB44C78B/FULL-DOC.extract.dumpimage-31614-31016.png': rtl error: Permission denied
warning: ignoring object 31614
extracting //?/J:/__bulktest-all/DATA/__bulktest/TextExtractFiles-T1/Y_/Qiqqa/Qiqqa-Q-TestDrive/Qiqqa/base/INTRANET_EF52564A-831D-42F2-B956-815CF0418C08/documents/6/68CC556AEB46AF68D067F9331AED462CB44C78B/FULL-DOC.extract.dumpimage-31615-31017.png
error: cannot open file '//?/J:/__bulktest-all/DATA/__bulktest/TextExtractFiles-T1/Y_/Qiqqa/Qiqqa-Q-TestDrive/Qiqqa/base/INTRANET_EF52564A-831D-42F2-B956-815CF0418C08/documents/6/68CC556AEB46AF68D067F9331AED462CB44C78B/FULL-DOC.extract.dumpimage-31615-31017.png': rtl error: Permission denied
warning: ignoring object 31615
OK: MUTOOL command: MUTOOL extract -o //?/J:/__bulktest-all/DATA/__bulktest/TextExtractFiles-T1/Y:\Qiqqa\Qiqqa-Q-TestDrive\Qiqqa\base\INTRANET_EF52564A-831D-42F2-B956-815CF0418C08\documents\6/68CC556AEB46AF68D067F9331AED462CB44C78B/FULL-DOC.extract.dump -r Y:\Qiqqa\Qiqqa-Q-TestDrive\Qiqqa\base\INTRANET_EF52564A-831D-42F2-B956-815CF0418C08\documents\6\68CC556AEB46AF68D067F9331AED462CB44C78B.pdf
bulktest: logfile rotated. Previous logging at "//?/J:/__bulktest-all/DATA.text_extract_pdf_files.bulktest.C-0039.log".
bulktest: using random_exec_percentage: 13.0%
Using a RESTRICTED DATA SET:
- ACCEPT: regex: -
          line numbers: 1029-1000000000
- IGNORE: regex: -
          line numbers: -
-----------------------------------------------------------------------------------


>L#00233> T:294417ms USED:153.41Mb OK MUTOOL extract -o //?/J:/__bulktest-all/DATA/__bulktest/TextExtractFiles-T1/Y:\Qiqqa\Qiqqa-Q-TestDrive\Qiqqa\base\INTRANET_EF52564A-831D-42F2-B956-815CF0418C08\documents\6/68CC556AEB46AF68D067F9331AED462CB44C78B/FULL-DOC.extract.dump -r Y:\Qiqqa\Qiqqa-Q-TestDrive\Qiqqa\base\INTRANET_EF52564A-831D-42F2-B956-815CF0418C08\documents\6\68CC556AEB46AF68D067F9331AED462CB44C78B.pdf
>L#00233> T:294417ms USED:153.41Mb **NOTICABLY SLOW COMMAND**:: OK MUTOOL extract -o //?/J:/__bulktest-all/DATA/__bulktest/TextExtractFiles-T1/Y:\Qiqqa\Qiqqa-Q-TestDrive\Qiqqa\base\INTRANET_EF52564A-831D-42F2-B956-815CF0418C08\documents\6/68CC556AEB46AF68D067F9331AED462CB44C78B/FULL-DOC.extract.dump -r Y:\Qiqqa\Qiqqa-Q-TestDrive\Qiqqa\base\INTRANET_EF52564A-831D-42F2-B956-815CF0418C08\documents\6\68CC556AEB46AF68D067F9331AED462CB44C78B.pdf
>L#00233> T:294417ms USED:153.41Mb **LETHARGICALLY SLOW COMMAND**:: OK MUTOOL extract -o //?/J:/__bulktest-all/DATA/__bulktest/TextExtractFiles-T1/Y:\Qiqqa\Qiqqa-Q-TestDrive\Qiqqa\base\INTRANET_EF52564A-831D-42F2-B956-815CF0418C08\documents\6/68CC556AEB46AF68D067F9331AED462CB44C78B/FULL-DOC.extract.dump -r Y:\Qiqqa\Qiqqa-Q-TestDrive\Qiqqa\base\INTRANET_EF52564A-831D-42F2-B956-815CF0418C08\documents\6\68CC556AEB46AF68D067F9331AED462CB44C78B.pdf
:L#00235: MUTOOL info -o //?/J:/__bulktest-all/DATA/__bulktest/TextExtractFiles-T1/Y:\Qiqqa\Qiqqa-Q-TestDrive\Qiqqa\base\INTRANET_EF52564A-831D-42F2-B956-815CF0418C08\documents\6/68CC556AEB46AF68D067F9331AED462CB44C78B/FULL-DOC.info.txt -F -I -M -P -S -X Y:\Qiqqa\Qiqqa-Q-TestDrive\Qiqqa\base\INTRANET_EF52564A-831D-42F2-B956-815CF0418C08\documents\6\68CC556AEB46AF68D067F9331AED462CB44C78B.pdf
::SKIP-DUE-TO-RANDOM-SAMPLING: MUTOOL command: MUTOOL info -o //?/J:/__bulktest-all/DATA/__bulktest/TextExtractFiles-T1/Y:\Qiqqa\Qiqqa-Q-TestDrive\Qiqqa\base\INTRANET_EF52564A-831D-42F2-B956-815CF0418C08\documents\6/68CC556AEB46AF68D067F9331AED462CB44C78B/FULL-DOC.info.txt -F -I -M -P -S -X Y:\Qiqqa\Qiqqa-Q-TestDrive\Qiqqa\base\INTRANET_EF52564A-831D-42F2-B956-815CF0418C08\documents\6\68CC556AEB46AF68D067F9331AED462CB44C78B.pdf
:L#00237: MUTOOL pages -o //?/J:/__bulktest-all/DATA/__bulktest/TextExtractFiles-T1/Y:\Qiqqa\Qiqqa-Q-TestDrive\Qiqqa\base\INTRANET_EF52564A-831D-42F2-B956-815CF0418C08\documents\6/68CC556AEB46AF68D067F9331AED462CB44C78B/FULL-DOC.pages.txt Y:\Qiqqa\Qiqqa-Q-TestDrive\Qiqqa\base\INTRANET_EF52564A-831D-42F2-B956-815CF0418C08\documents\6\68CC556AEB46AF68D067F9331AED462CB44C78B.pdf
::SKIP-DUE-TO-RANDOM-SAMPLING: MUTOOL command: MUTOOL pages -o //?/J:/__bulktest-all/DATA/__bulktest/TextExtractFiles-T1/Y:\Qiqqa\Qiqqa-Q-TestDrive\Qiqqa\base\INTRANET_EF52564A-831D-42F2-B956-815CF0418C08\documents\6/68CC556AEB46AF68D067F9331AED462CB44C78B/FULL-DOC.pages.txt Y:\Qiqqa\Qiqqa-Q-TestDrive\Qiqqa\base\INTRANET_EF52564A-831D-42F2-B956-815CF0418C08\documents\6\68CC556AEB46AF68D067F9331AED462CB44C78B.pdf
:L#00239: MUTOOL show -o //?/J:/__bulktest-all/DATA/__bulktest/TextExtractFiles-T1/Y:\Qiqqa\Qiqqa-Q-TestDrive\Qiqqa\base\INTRANET_EF52564A-831D-42F2-B956-815CF0418C08\documents\6/68CC556AEB46AF68D067F9331AED462CB44C78B/FULL-DOC.show.txt -b Y:\Qiqqa\Qiqqa-Q-TestDrive\Qiqqa\base\INTRANET_EF52564A-831D-42F2-B956-815CF0418C08\documents\6\68CC556AEB46AF68D067F9331AED462CB44C78B.pdf trailer xref pages grep outline js form trailer/* Root/* Root/Metadata 0/* 1/* 2/* 3/* 4/* 5/* 6/* 7/* 8/* 9/* 10/* 11/* 12/* 13/* trailer/Info trailer/Info/Author
error: cannot open file '//?/J:/__bulktest-all/DATA/__bulktest/TextExtractFiles-T1/Y_/Qiqqa/Qiqqa-Q-TestDrive/Qiqqa/base/INTRANET_EF52564A-831D-42F2-B956-815CF0418C08/documents/6/68CC556AEB46AF68D067F9331AED462CB44C78B/FULL-DOC.show.txt': rtl error: Permission denied
error: aborting process from uncaught error!
total 20141708ms / 5512 commands for an average of 3654ms in 5512 commands
fastest command line 233 (dataline: 1077): 6ms (MUTOOL extract -o //?/J:/__bulktest-all/DATA/__bulktest/TextExtractFiles-T1/Y:\Qiqqa\Qiqqa-Q-TestDrive\Qiqqa\evil\Guest\documents\A/ACDBEEFACFA07CBDAC9A2B291CF2B67B7514DCA2/FULL-DOC.extract.dump -r Y:\Qiqqa\Qiqqa-Q-TestDrive\Qiqqa\evil\Guest\documents\A\ACDBEEFACFA07CBDAC9A2B291CF2B67B7514DCA2.pdf)
slowest command line 158 (dataline: 1218): 583516ms (MUTOOL draw -o //?/J:/__bulktest-all/DATA/__bulktest/TextExtractFiles-T1/Y:\Qiqqa\SSD-0001\Qiqqa\backups\QiqqaBackup.20190815011657\Guest\documents\8/8D32A9979A3CE36EE9C5FA067752FE45CCA23/FULL-DOC-x300.png -s mtf -r 300 -w 3840 -h 3840 -y l -P -B 50 Y:\Qiqqa\SSD-0001\Qiqqa\backups\QiqqaBackup.20190815011657\Guest\documents\8\8D32A9979A3CE36EE9C5FA067752FE45CCA23.pdf)
Lock 0 held for 13291.955 seconds (65.88398%)
Lock 1 held for 229.85472 seconds (1.1393166%)
Lock 2 held for 9641.933 seconds (47.79199%)
Lock 3 held for 351.62644 seconds (1.7429001%)
Lock 4 held for 0 seconds (0%)
Lock 5 held for 53.455129 seconds (0.26496003%)
Total program time 20174.79 seconds
Memory use total=8883.63G peak=1.80M current=1.80M
Allocations total=1275422326
Soft Assertion failed: !ctx || !fz_has_global_context() || (ctx->error.top == ctx->error.stack_base) --> Z:\lib\tooling\qiqqa\MuPDF\source\tools\bulktest.c::1396
Soft Assertion failed: ctx->error.top == ctx->error.stack_base --> Z:\lib\tooling\qiqqa\MuPDF\source\fitz\context.c::196
```

