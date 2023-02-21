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

`mupdf` memory issue:


```
muconvert INTRANET_E57B9774-4712-430E-93E0-E67433F7DF07/documents/4/458B1F6296CFA32442F7CC47752231F74B89E54D.pdf
```










##### Item ♯00002


stdout / stderr:


```
'% PDF: INTRANET_E57B9774-4712-430E-93E0-E67433F7DF07/documents/4/458B1F6296CFA32442F7CC47752231F74B89E54D.pdf'
'% dir: INTRANET_E57B9774-4712-430E-93E0-E67433F7DF07/documents/4'
'% name: 458B1F6296CFA32442F7CC47752231F74B89E54D.pdf'
'% base: 458B1F6296CFA32442F7CC47752231F74B89E54D'
'CD F:/Qiqqa/evil-base'
'MUTOOL mudraw -o "__mujstest/INTRANET_E57B9774-4712-430E-93E0-E67433F7DF07/documents/4/%04d-458B1F6296CFA32442F7CC47752231F74B89E54D.ocr.html" -s mtf5 -r 150 -x preserve-ligatures,preserve-whitespace,dehyphenate -y l "INTRANET_E57B9774-4712-430E-93E0-E67433F7DF07/documents/4/458B1F6296CFA32442F7CC47752231F74B89E54D.pdf"'
Layer configs:
page INTRANET_E57B9774-4712-430E-93E0-E67433F7DF07/documents/4/458B1F6296CFA32442F7CC47752231F74B89E54D.pdf 1 grayscaleError opening data file W:\Projects\sites\library.visyond.gov\80\lib\CS\MuPDF\platform\win32\Debug-64/tessdata/eng.traineddata
Please make sure the TESSDATA_PREFIX environment variable is set to your "tessdata" directory.
Failed loading language 'eng'
Tesseract couldn't load any languages!
error: Tesseract initialisation failed
warning: dropping unclosed device
error: cannot draw 'INTRANET_E57B9774-4712-430E-93E0-E67433F7DF07/documents/4/458B1F6296CFA32442F7CC47752231F74B89E54D.pdf': Tesseract initialisation failed
Allocations total=1807
error executing MUTOOL command: mudraw -o "__mujstest/INTRANET_E57B9774-4712-430E-93E0-E67433F7DF07/documents/4/%04d-458B1F6296CFA32442F7CC47752231F74B89E54D.ocr.html" -s mtf5 -r 150 -x preserve-ligatures,preserve-whitespace,dehyphenate -y l "INTRANET_E57B9774-4712-430E-93E0-E67433F7DF07/documents/4/458B1F6296CFA32442F7CC47752231F74B89E54D.pdf"
'MUTOOL muconvert -o "__mujstest/INTRANET_E57B9774-4712-430E-93E0-E67433F7DF07/documents/4/%04d-458B1F6296CFA32442F7CC47752231F74B89E54D.convert.pdf" -W 1200 -H 1800 "INTRANET_E57B9774-4712-430E-93E0-E67433F7DF07/documents/4/458B1F6296CFA32442F7CC47752231F74B89E54D.pdf"'
OK: MUTOOL command: muconvert -o "__mujstest/INTRANET_E57B9774-4712-430E-93E0-E67433F7DF07/documents/4/%04d-458B1F6296CFA32442F7CC47752231F74B89E54D.convert.pdf" -W 1200 -H 1800 "INTRANET_E57B9774-4712-430E-93E0-E67433F7DF07/documents/4/458B1F6296CFA32442F7CC47752231F74B89E54D.pdf"
'MUTOOL muraster -o "__mujstest/INTRANET_E57B9774-4712-430E-93E0-E67433F7DF07/documents/4/%04d-458B1F6296CFA32442F7CC47752231F74B89E54D.raster.ppm" -s mt -r 150 -P "INTRANET_E57B9774-4712-430E-93E0-E67433F7DF07/documents/4/458B1F6296CFA32442F7CC47752231F74B89E54D.pdf"'
page INTRANET_E57B9774-4712-430E-93E0-E67433F7DF07/documents/4/458B1F6296CFA32442F7CC47752231F74B89E54D.pdf 1warning: found duplicate fz_icc_link in the store
 19ms (interpretation) 467ms (rendering) 486ms (total)
Glyph Cache Size: 0
Glyph Cache Evictions: 0 (0 bytes)
page INTRANET_E57B9774-4712-430E-93E0-E67433F7DF07/documents/4/458B1F6296CFA32442F7CC47752231F74B89E54D.pdf 2 32ms (interpretation) 306ms (rendering) 338ms (total)
Glyph Cache Size: 0
Glyph Cache Evictions: 0 (0 bytes)
page INTRANET_E57B9774-4712-430E-93E0-E67433F7DF07/documents/4/458B1F6296CFA32442F7CC47752231F74B89E54D.pdf 3
```



**CRASH**







##### Item ♯00003





```
'MUTOOL muraster -o "__mujstest/INTRANET_E57B9774-4712-430E-93E0-E67433F7DF07/documents/7/%04d-71120B277417F57C6496F63EE705197262F7AA.raster.ppm" -s mt -r 150 -P "INTRANET_E57B9774-4712-430E-93E0-E67433F7DF07/documents/7/71120B277417F57C6496F63EE705197262F7AA.pdf"'
page INTRANET_E57B9774-4712-430E-93E0-E67433F7DF07/documents/7/71120B277417F57C6496F63EE705197262F7AA.pdf 1warning: found duplicate fz_icc_link in the store
 21ms (interpretation) 311ms (rendering) 332ms (total)
Glyph Cache Size: 0
Glyph Cache Evictions: 0 (0 bytes)
page INTRANET_E57B9774-4712-430E-93E0-E67433F7DF07/documents/7/71120B277417F57C6496F63EE705197262F7AA.pdf 2 8ms (interpretation) 35ms (rendering) 43ms (total)
Glyph Cache Size: 0
Glyph Cache Evictions: 0 (0 bytes)
page INTRANET_E57B9774-4712-430E-93E0-E67433F7DF07/documents/7/71120B277417F57C6496F63EE705197262F7AA.pdf 3 18ms (interpretation) 138ms (rendering) 156ms (total)
Glyph Cache Size: 0
Glyph Cache Evictions: 0 (0 bytes)
page INTRANET_E57B9774-4712-430E-93E0-E67433F7DF07/documents/7/71120B277417F57C6496F63EE705197262F7AA.pdf 4 15ms (interpretation) 143ms (rendering) 158ms (total)
Glyph Cache Size: 0
Glyph Cache Evictions: 0 (0 bytes)
page INTRANET_E57B9774-4712-430E-93E0-E67433F7DF07/documents/7/71120B277417F57C6496F63EE705197262F7AA.pdf 5 15ms (interpretation) 147ms (rendering) 162ms (total)
Glyph Cache Size: 0
Glyph Cache Evictions: 0 (0 bytes)
page INTRANET_E57B9774-4712-430E-93E0-E67433F7DF07/documents/7/71120B277417F57C6496F63EE705197262F7AA.pdf 6
```



**CRASH**







##### Item ♯00004





```
OK: MUTOOL command: muconvert -o "__mujstest/INTRANET_E57B9774-4712-430E-93E0-E67433F7DF07/documents/7/%04d-7C40AD1A6353764F741E9F8C56ECDDA68061BAF0.convert.pdf" -W 1200 -H 1800 "INTRANET_E57B9774-4712-430E-93E0-E67433F7DF07/documents/7/7C40AD1A6353764F741E9F8C56ECDDA68061BAF0.pdf"
'MUTOOL muraster -o "__mujstest/INTRANET_E57B9774-4712-430E-93E0-E67433F7DF07/documents/7/%04d-7C40AD1A6353764F741E9F8C56ECDDA68061BAF0.raster.ppm" -s mt -r 150 -P "INTRANET_E57B9774-4712-430E-93E0-E67433F7DF07/documents/7/7C40AD1A6353764F741E9F8C56ECDDA68061BAF0.pdf"'
page INTRANET_E57B9774-4712-430E-93E0-E67433F7DF07/documents/7/7C40AD1A6353764F741E9F8C56ECDDA68061BAF0.pdf 1 13ms (interpretation) 230ms (rendering) 243ms (total)
Glyph Cache Size: 0
Glyph Cache Evictions: 0 (0 bytes)
page INTRANET_E57B9774-4712-430E-93E0-E67433F7DF07/documents/7/7C40AD1A6353764F741E9F8C56ECDDA68061BAF0.pdf 2
```



**CRASH**







##### Item ♯00005





```
'MUTOOL muraster -o "__mujstest/INTRANET_E57B9774-4712-430E-93E0-E67433F7DF07/documents/7/%04d-7E70426EE6C56414553F313EE285F64734EBBF.raster.ppm" -s mt -r 150 -P "INTRANET_E57B9774-4712-430E-93E0-E67433F7DF07/documents/7/7E70426EE6C56414553F313EE285F64734EBBF.pdf"'
page INTRANET_E57B9774-4712-430E-93E0-E67433F7DF07/documents/7/7E70426EE6C56414553F313EE285F64734EBBF.pdf 1 1ms (interpretation) 182ms (rendering) 183ms (total)
Glyph Cache Size: 0
Glyph Cache Evictions: 0 (0 bytes)
page INTRANET_E57B9774-4712-430E-93E0-E67433F7DF07/documents/7/7E70426EE6C56414553F313EE285F64734EBBF.pdf 2 23ms (interpretation) 115ms (rendering) 138ms (total)
Glyph Cache Size: 0
Glyph Cache Evictions: 0 (0 bytes)
page INTRANET_E57B9774-4712-430E-93E0-E67433F7DF07/documents/7/7E70426EE6C56414553F313EE285F64734EBBF.pdf 3 29ms (interpretation) 84ms (rendering) 113ms (total)
Glyph Cache Size: 0
Glyph Cache Evictions: 0 (0 bytes)
page INTRANET_E57B9774-4712-430E-93E0-E67433F7DF07/documents/7/7E70426EE6C56414553F313EE285F64734EBBF.pdf 4 15ms (interpretation) 104ms (rendering) 119ms (total)
Glyph Cache Size: 0
Glyph Cache Evictions: 0 (0 bytes)
page INTRANET_E57B9774-4712-430E-93E0-E67433F7DF07/documents/7/7E70426EE6C56414553F313EE285F64734EBBF.pdf 5 7ms (interpretation) 91ms (rendering) 98ms (total)
Glyph Cache Size: 0
Glyph Cache Evictions: 0 (0 bytes)
page INTRANET_E57B9774-4712-430E-93E0-E67433F7DF07/documents/7/7E70426EE6C56414553F313EE285F64734EBBF.pdf 6 5ms (interpretation) 31ms (rendering) 36ms (total)
Glyph Cache Size: 0
Glyph Cache Evictions: 0 (0 bytes)
page INTRANET_E57B9774-4712-430E-93E0-E67433F7DF07/documents/7/7E70426EE6C56414553F313EE285F64734EBBF.pdf 7
```



**CRASH**







##### Item ♯00006





```
Glyph Cache Size: 1048431
Glyph Cache Evictions: 144013 (71527649 bytes)
page Sample-PDFs-for-format-testing/PDF.js-issue-PDFs/PDF32000_2008.pdf 756 6ms (interpretation) 103ms (rendering) 109ms (total)
Glyph Cache Size: 0
Glyph Cache Evictions: 144015 (71528593 bytes)
total 132018ms / 756 pages for an average of 174ms
fastest page 654: 36ms
slowest page 403: 7727ms
warning: ... repeated 2 times...
Memory use total=5508642498 peak=42052316 current=0
```



---NO CRASH, just huge memory usage and slow rendering!---







##### Item ♯00007





```
'MUTOOL trace -o "__mujstest/Sample-PDFs-for-format-testing/PDF.js-issue-PDFs/%04d-pdfmark_reference.trace.txt" -W 1200 -H 1800 "Sample-PDFs-for-format-testing/PDF.js-issue-PDFs/pdfmark_reference.pdf"'
OK: MUTOOL command: trace -o "__mujstest/Sample-PDFs-for-format-testing/PDF.js-issue-PDFs/%04d-pdfmark_reference.trace.txt" -W 1200 -H 1800 "Sample-PDFs-for-format-testing/PDF.js-issue-PDFs/pdfmark_reference.pdf"
T:1964ms D:94.662s OK MUTOOL trace -o "__mujstest/Sample-PDFs-for-format-testing/PDF.js-issue-PDFs/%04d-pdfmark_reference.trace.txt" -W 1200 -H 1800 "Sample-PDFs-for-format-testing/PDF.js-issue-PDFs/pdfmark_reference.pdf"
'MUTOOL clean -gggg -D -c -s -AA "Sample-PDFs-for-format-testing/PDF.js-issue-PDFs/pdfmark_reference.pdf" "__mujstest/Sample-PDFs-for-format-testing/PDF.js-issue-PDFs/%04d-pdfmark_reference.clean.pdf"'
warning: deduplication cost pathological at O(75104768)?
```



--slow clean without the timeout patch--







##### Item ♯00008





```
'MUTOOL clean -gggg -D -c -s -AA "Sample-PDFs-for-format-testing/PDFIO-format-corpus/govdocs1-error-pdfs/error_set_1/499039.pdf" "__mujstest/Sample-PDFs-for-format-testing/PDFIO-format-corpus/govdocs1-error-pdfs/error_set_1/%04d-499039.clean.pdf"'
```



warning: deduplication cost pathological at O(588485124)?







##### Item ♯00009





```
warning: cannot render glyph
warning: FT_Load_Glyph(ICKPOC+HelveticaNeue-Condensed,71,FT_LOAD_NO_HINTING): broken file
warning: FT_Load_Glyph(ICKPOC+HelveticaNeue-Condensed,71,FT_LOAD_NO_SCALE|FT_LOAD_IGNORE_TRANSFORM): broken file
warning: cannot render glyph
warning: FT_Load_Glyph(ICKPOC+HelveticaNeue-Condensed,68,FT_LOAD_NO_HINTING): broken file
warning: FT_Load_Glyph(ICKPOC+HelveticaNeue-Condensed,68,FT_LOAD_NO_SCALE|FT_LOAD_IGNORE_TRANSFORM): broken file
warning: cannot render glyph
 13ms (interpretation) 8538ms (rendering) 8551ms (total)
Glyph Cache Size: 1048520
Glyph Cache Evictions: 5101 (2140987 bytes)
page Sample-PDFs-for-format-testing/PDFIO-format-corpus/govdocs1-error-pdfs/error_set_2/178360.pdf 11error: zlib error: invalid distance too far back
warning: read error; treating as end of file
error: zlib error: invalid distance too far back
```



-- no crash, but LOTS of failures --







##### Item ♯00010





```
'MUTOOL clean -gggg -D -c -s -AA "Sample-PDFs-for-format-testing/PDFIO-format-corpus/govdocs1-error-pdfs/error_set_2/219789.pdf" "__mujstest/Sample-PDFs-for-format-testing/PDFIO-format-corpus/govdocs1-error-pdfs/error_set_2/%04d-219789.clean.pdf"'
```



-- causes infinite loop in the code: hits the 'else if (rune == 32)' branch inside 'static void walk_string(fz_context *ctx, int uni, int remove, editable_str *str)' and loops indefinitely at the same spot as str->pos is not updated.







##### Item ♯00011





```
'MUTOOL clean -gggg -D -c -s -AA "Sample-PDFs-for-format-testing/Qiqqa Tools - test collection/PEP20-01-01-006-508.pdf" "__mujstest/Sample-PDFs-for-format-testing/Qiqqa Tools - test collection/%04d-PEP20-01-01-006-508.clean.pdf"'
```



-- and another one for that infinite loop --







##### Item ♯00012





```
OK: MUTOOL command: muconvert -o "__mujstest/Sample-PDFs-for-format-testing/Qiqqa Tools - test collection/%04d-TE2000_TC3_HMI_EN.convert.pdf" -W 1200 -H 1800 "Sample-PDFs-for-format-testing/Qiqqa Tools - test collection/TE2000_TC3_HMI_EN.pdf"
T:122284ms D:4911.867s OK MUTOOL muconvert -o "__mujstest/Sample-PDFs-for-format-testing/Qiqqa Tools - test collection/%04d-TE2000_TC3_HMI_EN.convert.pdf" -W 1200 -H 1800 "Sample-PDFs-for-format-testing/Qiqqa Tools - test collection/TE2000_TC3_HMI_EN.pdf"
```



-- no errors but takes 120 seconds to complete --







##### Item ♯00013





```
page Sample-PDFs-for-format-testing/Qiqqa Tools - test collection/TE2000_TC3_HMI_EN.pdf 2286 13ms (interpretation) 1122ms (rendering) 1135ms (total)
Glyph Cache Size: 0
Glyph Cache Evictions: 63808 (33888924 bytes)
total 531963ms / 2286 pages for an average of 232ms
fastest page 2: 26ms
slowest page 545: 31173ms
warning: ... repeated 9 times...
Memory use total=17710823054 peak=395591797 current=0
OK: MUTOOL command: muraster -o "__mujstest/Sample-PDFs-for-format-testing/Qiqqa Tools - test collection/%04d-TE2000_TC3_HMI_EN.raster.ppm" -s mt -r 150 -P "Sample-PDFs-for-format-testing/Qiqqa Tools - test collection/TE2000_TC3_HMI_EN.pdf"
```



-- 395MB: that's some HUGE memory consumption! --







##### Item ♯00014





```
page Sample-PDFs-for-format-testing/testset European Unicode extract & OCR/AH_26-2018-2.pdf 310 34ms (interpretation) 37ms (rendering) 71ms (total)
Glyph Cache Size: 1048369
Glyph Cache Evictions: 40373 (18540939 bytes)
page Sample-PDFs-for-format-testing/testset European Unicode extract & OCR/AH_26-2018-2.pdf 311 3ms (interpretation) 66ms (rendering) 69ms (total)
Glyph Cache Size: 0
Glyph Cache Evictions: 40374 (18541369 bytes)
total 71802ms / 311 pages for an average of 230ms
fastest page 7: 22ms
slowest page 239: 30350ms
Memory use total=1709316691 peak=94789515 current=0
OK: MUTOOL command: muraster -o "__mujstest/Sample-PDFs-for-format-testing/testset European Unicode extract & OCR/%04d-AH_26-2018-2.raster.ppm" -s mt -r 150 -P "Sample-PDFs-for-format-testing/testset European Unicode extract & OCR/AH_26-2018-2.pdf"
```



-- page 239 takes about half the total render time of the entire PDF --







!!!BAD-1DC618F32245B5B4F143197B5249DCF83A68.extract.image-0004-0001.jpg ...
-- that PDF produced over 7000 jpg files on extract; apparently the pages are constructed from a zillion tiny images --







##### Item ♯00015





```
'MUTOOL clean -gggg -D -c -s -AA "Sample-PDFs-for-format-testing/Qiqqa Tools - test collection/9780735626690 (links in TOC seem b0rked in Acrobat DC at least).pdf" "__mujstest/Sample-PDFs-for-format-testing/Qiqqa Tools - test collection/%04d-9780735626690 (links in TOC seem b0rked in Acrobat DC at least).clean.pdf"'
warning: undefined link destination
warning: ... repeated 359 times...
warning: object out of range (0 0 R); xref size 21798
warning: undefined link destination
warning: deduplication cost pathological at O(237620000)?
```



-- another slow cleaner --







##### Item ♯00016





```
'MUTOOL clean -gggg -D -c -s -AA "Sample-PDFs-for-format-testing/Qiqqa Tools - test collection/Engineering-Data-Release_LandingGear.pdf" "__mujstest/Sample-PDFs-for-format-testing/Qiqqa Tools - test collection/%04d-Engineering-Data-Release_LandingGear.clean.pdf"'
error: cannot create appearance stream for 3D annotations
warning: cannot create appearance stream
warning: deduplication cost pathological at O(85844304)?
OK: MUTOOL command: clean -gggg -D -c -s -AA "Sample-PDFs-for-format-testing/Qiqqa Tools - test collection/Engineering-Data-Release_LandingGear.pdf" "__mujstest/Sample-PDFs-for-format-testing/Qiqqa Tools - test collection/%04d-Engineering-Data-Release_LandingGear.clean.pdf"
T:21172ms D:1469.051s OK MUTOOL clean -gggg -D -c -s -AA "Sample-PDFs-for-format-testing/Qiqqa Tools - test collection/Engineering-Data-Release_LandingGear.pdf" "__mujstest/Sample-PDFs-for-format-testing/Qiqqa Tools - test collection/%04d-Engineering-Data-Release_LandingGear.clean.pdf"
```



-- another slow cleaner, which is restricted by the new 15s time duration hack in the de-dup code --







##### Item ♯00017





```
page Sample-PDFs-for-format-testing/Qiqqa Tools - test collection/TE2000_TC3_HMI_EN.pdf 2286 12ms (interpretation) 1382ms (rendering) 1394ms (total)
Glyph Cache Size: 0
Glyph Cache Evictions: 63823 (33897130 bytes)
total 594473ms / 2286 pages for an average of 260ms
fastest page 1633: 22ms
slowest page 1806: 7878ms
warning: ... repeated 7 times...
warning: ... repeated 2 times...
Memory use total=17710831059 peak=395591764 current=0
OK: MUTOOL command: muraster -o "__mujstest/Sample-PDFs-for-format-testing/Qiqqa Tools - test collection/%04d-TE2000_TC3_HMI_EN.raster.ppm" -s mt -r 150 -P "Sample-PDFs-for-format-testing/Qiqqa Tools - test collection/TE2000_TC3_HMI_EN.pdf"
T:526693ms D:3100.355s OK MUTOOL muraster -o "__mujstest/Sample-PDFs-for-format-testing/Qiqqa Tools - test collection/%04d-TE2000_TC3_HMI_EN.raster.ppm" -s mt -r 150 -P "Sample-PDFs-for-format-testing/Qiqqa Tools - test collection/TE2000_TC3_HMI_EN.pdf"
'MUTOOL trace -o "__mujstest/Sample-PDFs-for-format-testing/Qiqqa Tools - test collection/%04d-TE2000_TC3_HMI_EN.trace.txt" -W 1200 -H 1800 "Sample-PDFs-for-format-testing/Qiqqa Tools - test collection/TE2000_TC3_HMI_EN.pdf"'
OK: MUTOOL command: trace -o "__mujstest/Sample-PDFs-for-format-testing/Qiqqa Tools - test collection/%04d-TE2000_TC3_HMI_EN.trace.txt" -W 1200 -H 1800 "Sample-PDFs-for-format-testing/Qiqqa Tools - test collection/TE2000_TC3_HMI_EN.pdf"
T:128212ms D:3228.570s OK MUTOOL trace -o "__mujstest/Sample-PDFs-for-format-testing/Qiqqa Tools - test collection/%04d-TE2000_TC3_HMI_EN.trace.txt" -W 1200 -H 1800 "Sample-PDFs-for-format-testing/Qiqqa Tools - test collection/TE2000_TC3_HMI_EN.pdf"
'MUTOOL clean -gggg -D -c -s -AA "Sample-PDFs-for-format-testing/Qiqqa Tools - test collection/TE2000_TC3_HMI_EN.pdf" "__mujstest/Sample-PDFs-for-format-testing/Qiqqa Tools - test collection/%04d-TE2000_TC3_HMI_EN.clean.pdf"'
warning: deduplication cost pathological at O(4069102472)?
```



-- slow going --







##### Item ♯00018





```
OK: MUTOOL command: trace -o "__mujstest/Sample-PDFs-for-format-testing/testset misc 2/global/%04d-1f5dd128c3757420a881a155f2f8ace3.trace.txt" -W 1200 -H 1800 "Sample-PDFs-for-format-testing/testset misc 2/global/1f5dd128c3757420a881a155f2f8ace3.pdf"
T:13695ms D:28397.400s OK MUTOOL trace -o "__mujstest/Sample-PDFs-for-format-testing/testset misc 2/global/%04d-1f5dd128c3757420a881a155f2f8ace3.trace.txt" -W 1200 -H 1800 "Sample-PDFs-for-format-testing/testset misc 2/global/1f5dd128c3757420a881a155f2f8ace3.pdf"
'MUTOOL clean -gggg -D -c -s -AA "Sample-PDFs-for-format-testing/testset misc 2/global/1f5dd128c3757420a881a155f2f8ace3.pdf" "__mujstest/Sample-PDFs-for-format-testing/testset misc 2/global/%04d-1f5dd128c3757420a881a155f2f8ace3.clean.pdf"'
warning: undefined link destination
```



warning: deduplication cost pathological at O(204606220)?







##### Item ♯00019





```
T:29060ms D:30885.487s OK MUTOOL trace -o "__mujstest/Sample-PDFs-for-format-testing/testset misc 2/global/%04d-ec00d5825f47b9d0faa953b1709163c3.trace.txt" -W 1200 -H 1800 "Sample-PDFs-for-format-testing/testset misc 2/global/ec00d5825f47b9d0faa953b1709163c3.pdf"
'MUTOOL clean -gggg -D -c -s -AA "Sample-PDFs-for-format-testing/testset misc 2/global/ec00d5825f47b9d0faa953b1709163c3.pdf" "__mujstest/Sample-PDFs-for-format-testing/testset misc 2/global/%04d-ec00d5825f47b9d0faa953b1709163c3.clean.pdf"'
warning: PDF stream Length incorrect
```



warning: deduplication cost pathological at O(200100012)?







##### Item ♯00020





```
'MUTOOL clean -gggg -D -c -s -AA "INTRANET-EVIL-PDF-SAMPLES-FOR-QIQQA/documents/2/2F7BEB491AD6C6E3539C95FB78E6CCF5E4BF772.pdf" "__mujstest/INTRANET-EVIL-PDF-SAMPLES-FOR-QIQQA/documents/2/%04d-2F7BEB491AD6C6E3539C95FB78E6CCF5E4BF772.clean.pdf"'
error: cannot find startxref
warning: trying to repair broken xref after encountering error: cannot find startxref
warning: repairing PDF document
warning: object missing 'endobj' token
warning: deduplication cost pathological at O(53809938)?
OK: MUTOOL command: clean -gggg -D -c -s -AA "INTRANET-EVIL-PDF-SAMPLES-FOR-QIQQA/documents/2/2F7BEB491AD6C6E3539C95FB78E6CCF5E4BF772.pdf" "__mujstest/INTRANET-EVIL-PDF-SAMPLES-FOR-QIQQA/documents/2/%04d-2F7BEB491AD6C6E3539C95FB78E6CCF5E4BF772.clean.pdf"
T:68374ms D:595.226s OK MUTOOL clean -gggg -D -c -s -AA "INTRANET-EVIL-PDF-SAMPLES-FOR-QIQQA/documents/2/2F7BEB491AD6C6E3539C95FB78E6CCF5E4BF772.pdf" "__mujstest/INTRANET-EVIL-PDF-SAMPLES-FOR-QIQQA/documents/2/%04d-2F7BEB491AD6C6E3539C95FB78E6CCF5E4BF772.clean.pdf"
'MUTOOL extract -o "__mujstest/INTRANET-EVIL-PDF-SAMPLES-FOR-QIQQA/documents/2/%04d-2F7BEB491AD6C6E3539C95FB78E6CCF5E4BF772.extract." -r "INTRANET-EVIL-PDF-SAMPLES-FOR-QIQQA/documents/2/2F7BEB491AD6C6E3539C95FB78E6CCF5E4BF772.pdf"'
error: cannot find startxref
warning: trying to repair broken xref after encountering error: cannot find startxref
warning: repairing PDF document
warning: object missing 'endobj' token
```



-- another 'pathological' ... --







##### Item ♯00021





```
6568483517080AD2E1EAB5DE0A9AEDD72B0F3AA.pdf"
'MUTOOL mudraw -o "__mujstest/INTRANET_E57B9774-4712-430E-93E0-E67433F7DF07/documents/1/%04d-16568483517080AD2E1EAB5DE0A9AEDD72B0F3AA.png" -s mtf5 -r 150 "INTRANET_E57B9774-4712-430E-93E0-E67433F7DF07/documents/1/16568483517080AD2E1EAB5DE0A9AEDD72B0F3AA.pdf"'
page INTRANET_E57B9774-4712-430E-93E0-E67433F7DF07/documents/1/16568483517080AD2E1EAB5DE0A9AEDD72B0F3AA.pdf 1 color a79b579f2de8e4bd40145b5d7f83db11 776ms
Glyph Cache Size: 0
Glyph Cache Evictions: 0 (0 bytes)
page INTRANET_E57B9774-4712-430E-93E0-E67433F7DF07/documents/1/16568483517080AD2E1EAB5DE0A9AEDD72B0F3AA.pdf 2 color 515b07f9f07ca9fcae9720cd0b1fa9f0 416ms
Glyph Cache Size: 0
Glyph Cache Evictions: 0 (0 bytes)
```



-- zero glyphs??? (no errors, just wondering what's going on in there...) ---







##### Item ♯00022





```
'MUTOOL muraster -o "__mujstest/INTRANET_E57B9774-4712-430E-93E0-E67433F7DF07/documents/4/%04d-458B1F6296CFA32442F7CC47752231F74B89E54D.raster.ppm" -s mt -r 150 -P "INTRANET_E57B9774-4712-430E-93E0-E67433F7DF07/documents/4/458B1F6296CFA32442F7CC47752231F74B89E54D.pdf"'
page INTRANET_E57B9774-4712-430E-93E0-E67433F7DF07/documents/4/458B1F6296CFA32442F7CC47752231F74B89E54D.pdf 1 15ms (interpretation) 408ms (rendering) 423ms (total)
Glyph Cache Size: 0
Glyph Cache Evictions: 0 (0 bytes)
page INTRANET_E57B9774-4712-430E-93E0-E67433F7DF07/documents/4/458B1F6296CFA32442F7CC47752231F74B89E54D.pdf 2 32ms (interpretation) 293ms (rendering) 325ms (total)
Glyph Cache Size: 0
Glyph Cache Evictions: 0 (0 bytes)
page INTRANET_E57B9774-4712-430E-93E0-E67433F7DF07/documents/4/458B1F6296CFA32442F7CC47752231F74B89E54D.pdf 3 37ms (interpretation) 279ms (rendering) 316ms (total)
Glyph Cache Size: 0
Glyph Cache Evictions: 0 (0 bytes)
page INTRANET_E57B9774-4712-430E-93E0-E67433F7DF07/documents/4/458B1F6296CFA32442F7CC47752231F74B89E54D.pdf 4
```



**CRASH** (double free???)







##### Item ♯00023





```
'MUTOOL muraster -o "__mujstest/INTRANET_E57B9774-4712-430E-93E0-E67433F7DF07/documents/6/%04d-6581B97C8F4A6CF712F84D81D0BA3155FFB5491.raster.ppm" -s mt -r 150 -P "INTRANET_E57B9774-4712-430E-93E0-E67433F7DF07/documents/6/6581B97C8F4A6CF712F84D81D0BA3155FFB5491.pdf"'
page INTRANET_E57B9774-4712-430E-93E0-E67433F7DF07/documents/6/6581B97C8F4A6CF712F84D81D0BA3155FFB5491.pdf 1warning: found duplicate fz_icc_link in the store
 13ms (interpretation) 232ms (rendering) 245ms (total)
Glyph Cache Size: 0
Glyph Cache Evictions: 0 (0 bytes)
page INTRANET_E57B9774-4712-430E-93E0-E67433F7DF07/documents/6/6581B97C8F4A6CF712F84D81D0BA3155FFB5491.pdf 2 4ms (interpretation) 298ms (rendering) 302ms (total)
Glyph Cache Size: 0
Glyph Cache Evictions: 0 (0 bytes)
page INTRANET_E57B9774-4712-430E-93E0-E67433F7DF07/documents/6/6581B97C8F4A6CF712F84D81D0BA3155FFB5491.pdf 3
```



**CRASH** (double free???)
-- Hmmmmm. Apparently only happens when other files have been processed previously as running this file as the first in the `mujstest` script does NOT trigger the 'double free' check! --







##### Item ♯00024





```
'MUTOOL muraster -o "__mujstest/INTRANET_E57B9774-4712-430E-93E0-E67433F7DF07/documents/7/%04d-71120B277417F57C6496F63EE705197262F7AA.raster.ppm" -s mt -r 150 -P "INTRANET_E57B9774-4712-430E-93E0-E67433F7DF07/documents/7/71120B277417F57C6496F63EE705197262F7AA.pdf"'
page INTRANET_E57B9774-4712-430E-93E0-E67433F7DF07/documents/7/71120B277417F57C6496F63EE705197262F7AA.pdf 1 19ms (interpretation) 309ms (rendering) 328ms (total)
Glyph Cache Size: 0
Glyph Cache Evictions: 0 (0 bytes)
page INTRANET_E57B9774-4712-430E-93E0-E67433F7DF07/documents/7/71120B277417F57C6496F63EE705197262F7AA.pdf 2 10ms (interpretation) 31ms (rendering) 41ms (total)
Glyph Cache Size: 0
Glyph Cache Evictions: 0 (0 bytes)
page INTRANET_E57B9774-4712-430E-93E0-E67433F7DF07/documents/7/71120B277417F57C6496F63EE705197262F7AA.pdf 3 17ms (interpretation) 134ms (rendering) 151ms (total)
Glyph Cache Size: 0
Glyph Cache Evictions: 0 (0 bytes)
page INTRANET_E57B9774-4712-430E-93E0-E67433F7DF07/documents/7/71120B277417F57C6496F63EE705197262F7AA.pdf 4 16ms (interpretation) 821ms (rendering) 837ms (total)
Glyph Cache Size: 0
Glyph Cache Evictions: 0 (0 bytes)
page INTRANET_E57B9774-4712-430E-93E0-E67433F7DF07/documents/7/71120B277417F57C6496F63EE705197262F7AA.pdf 5 15ms (interpretation) 148ms (rendering) 163ms (total)
Glyph Cache Size: 0
Glyph Cache Evictions: 0 (0 bytes)
page INTRANET_E57B9774-4712-430E-93E0-E67433F7DF07/documents/7/71120B277417F57C6496F63EE705197262F7AA.pdf 6 55ms (interpretation) 151ms (rendering) 206ms (total)
Glyph Cache Size: 0
Glyph Cache Evictions: 0 (0 bytes)
page INTRANET_E57B9774-4712-430E-93E0-E67433F7DF07/documents/7/71120B277417F57C6496F63EE705197262F7AA.pdf 7 61ms (interpretation) 177ms (rendering) 238ms (total)
Glyph Cache Size: 0
Glyph Cache Evictions: 0 (0 bytes)
page INTRANET_E57B9774-4712-430E-93E0-E67433F7DF07/documents/7/71120B277417F57C6496F63EE705197262F7AA.pdf 8 88ms (interpretation) 276ms (rendering) 364ms (total)
Glyph Cache Size: 0
Glyph Cache Evictions: 0 (0 bytes)
page INTRANET_E57B9774-4712-430E-93E0-E67433F7DF07/documents/7/71120B277417F57C6496F63EE705197262F7AA.pdf 9 69ms (interpretation) 170ms (rendering) 239ms (total)
Glyph Cache Size: 0
Glyph Cache Evictions: 0 (0 bytes)
page INTRANET_E57B9774-4712-430E-93E0-E67433F7DF07/documents/7/71120B277417F57C6496F63EE705197262F7AA.pdf 10
```



**CRASH**
-- attempt to use freed memory; pointer is `0xdddddddc` (`dddddddd - 1` as `early=1` in the `jbig2_decode_text_region()` function where this happens --

-- And that one happens at a **different page** in the process when run as the first file in the script, though the crash is at the same spot in the code! Here it that second run: --







##### Item ♯00025





```
'MUTOOL muraster -o "__mujstest/INTRANET_E57B9774-4712-430E-93E0-E67433F7DF07/documents/7/%04d-71120B277417F57C6496F63EE705197262F7AA.raster.ppm" -s mt -r 150 -P "INTRANET_E57B9774-4712-430E-93E0-E67433F7DF07/documents/7/71120B277417F57C6496F63EE705197262F7AA.pdf"'
page INTRANET_E57B9774-4712-430E-93E0-E67433F7DF07/documents/7/71120B277417F57C6496F63EE705197262F7AA.pdf 1 18ms (interpretation) 277ms (rendering) 295ms (total)
Glyph Cache Size: 0
Glyph Cache Evictions: 0 (0 bytes)
page INTRANET_E57B9774-4712-430E-93E0-E67433F7DF07/documents/7/71120B277417F57C6496F63EE705197262F7AA.pdf 2 10ms (interpretation) 37ms (rendering) 47ms (total)
Glyph Cache Size: 0
Glyph Cache Evictions: 0 (0 bytes)
page INTRANET_E57B9774-4712-430E-93E0-E67433F7DF07/documents/7/71120B277417F57C6496F63EE705197262F7AA.pdf 3 23ms (interpretation) 136ms (rendering) 159ms (total)
Glyph Cache Size: 0
Glyph Cache Evictions: 0 (0 bytes)
page INTRANET_E57B9774-4712-430E-93E0-E67433F7DF07/documents/7/71120B277417F57C6496F63EE705197262F7AA.pdf 4
```











##### Item ♯00026





```
'MUTOOL muraster -o "__mujstest/INTRANET_E57B9774-4712-430E-93E0-E67433F7DF07/documents/7/%04d-7C40AD1A6353764F741E9F8C56ECDDA68061BAF0.raster.ppm" -s mt -r 150 -P "INTRANET_E57B9774-4712-430E-93E0-E67433F7DF07/documents/7/7C40AD1A6353764F741E9F8C56ECDDA68061BAF0.pdf"'
page INTRANET_E57B9774-4712-430E-93E0-E67433F7DF07/documents/7/7C40AD1A6353764F741E9F8C56ECDDA68061BAF0.pdf 1
```



**CRASH** same error location as above, different file though.







##### Item ♯00027





```
'MUTOOL muraster -o "__mujstest/INTRANET_E57B9774-4712-430E-93E0-E67433F7DF07/documents/7/%04d-7E70426EE6C56414553F313EE285F64734EBBF.raster.ppm" -s mt -r 150 -P "INTRANET_E57B9774-4712-430E-93E0-E67433F7DF07/documents/7/7E70426EE6C56414553F313EE285F64734EBBF.pdf"'
page INTRANET_E57B9774-4712-430E-93E0-E67433F7DF07/documents/7/7E70426EE6C56414553F313EE285F64734EBBF.pdf 1 1ms (interpretation) 184ms (rendering) 185ms (total)
Glyph Cache Size: 0
Glyph Cache Evictions: 0 (0 bytes)
page INTRANET_E57B9774-4712-430E-93E0-E67433F7DF07/documents/7/7E70426EE6C56414553F313EE285F64734EBBF.pdf 2
```



**CRASH** one more of the same...







##### Item ♯00028





```
'MUTOOL muraster -o "__mujstest/INTRANET_E57B9774-4712-430E-93E0-E67433F7DF07/documents/8/%04d-808CA451AE74835B9BA0DA1DCA9DB4C2A057.raster.ppm" -s mt -r 150 -P "INTRANET_E57B9774-4712-430E-93E0-E67433F7DF07/documents/8/808CA451AE74835B9BA0DA1DCA9DB4C2A057.pdf"'
page INTRANET_E57B9774-4712-430E-93E0-E67433F7DF07/documents/8/808CA451AE74835B9BA0DA1DCA9DB4C2A057.pdf 1warning: found duplicate fz_image in the store
warning: found duplicate fz_image in the store
 84ms (interpretation) 2823ms (rendering) 2907ms (total)
Glyph Cache Size: 0
Glyph Cache Evictions: 0 (0 bytes)
page INTRANET_E57B9774-4712-430E-93E0-E67433F7DF07/documents/8/808CA451AE74835B9BA0DA1DCA9DB4C2A057.pdf 2warning: found duplicate fz_icc_link in the store
warning: found duplicate fz_icc_link in the store
 7ms (interpretation) 432ms (rendering) 439ms (total)
Glyph Cache Size: 0
Glyph Cache Evictions: 0 (0 bytes)
page INTRANET_E57B9774-4712-430E-93E0-E67433F7DF07/documents/8/808CA451AE74835B9BA0DA1DCA9DB4C2A057.pdf 3 13ms (interpretation) 435ms (rendering) 448ms (total)
Glyph Cache Size: 0
Glyph Cache Evictions: 0 (0 bytes)
page INTRANET_E57B9774-4712-430E-93E0-E67433F7DF07/documents/8/808CA451AE74835B9BA0DA1DCA9DB4C2A057.pdf 4 14ms (interpretation) 422ms (rendering) 436ms (total)
Glyph Cache Size: 0
Glyph Cache Evictions: 0 (0 bytes)
page INTRANET_E57B9774-4712-430E-93E0-E67433F7DF07/documents/8/808CA451AE74835B9BA0DA1DCA9DB4C2A057.pdf 5
```



**CRASH** (double free check triggered)







##### Item ♯00029


Ah, never checked before now:


```
'MUTOOL mudraw -o "__mujstest/INTRANET_E57B9774-4712-430E-93E0-E67433F7DF07/documents/8/%04d-808CA451AE74835B9BA0DA1DCA9DB4C2A057.ocr.html" -s mtf5 -r 150 -x preserve-ligatures,preserve-whitespace,dehyphenate -y l "INTRANET_E57B9774-4712-430E-93E0-E67433F7DF07/documents/8/808CA451AE74835B9BA0DA1DCA9DB4C2A057.pdf"'
Layer configs:
page INTRANET_E57B9774-4712-430E-93E0-E67433F7DF07/documents/8/808CA451AE74835B9BA0DA1DCA9DB4C2A057.pdf 1 colorError opening data file W:\Projects\sites\library.visyond.gov\80\lib\CS\MuPDF\platform\win32\Debug-64/tessdata/eng.traineddata
Please make sure the TESSDATA_PREFIX environment variable is set to your "tessdata" directory.
Failed loading language 'eng'
Tesseract couldn't load any languages!
error: Tesseract initialisation failed
warning: dropping unclosed device
error: cannot draw 'INTRANET_E57B9774-4712-430E-93E0-E67433F7DF07/documents/8/808CA451AE74835B9BA0DA1DCA9DB4C2A057.pdf': Tesseract initialisation failed
Allocations total=4570
```



-- Looks like `mudraw` doesn't get a chance to crash as it's `tesseract` throwing a tantrum causing it to fail early???







##### Item ♯00030





```
'MUTOOL muraster -o "__mujstest/INTRANET_E57B9774-4712-430E-93E0-E67433F7DF07/documents/8/%04d-844418D91B7D26A2EB20F0D19CE6F625F9E89BD.raster.ppm" -s mt -r 150 -P "INTRANET_E57B9774-4712-430E-93E0-E67433F7DF07/documents/8/844418D91B7D26A2EB20F0D19CE6F625F9E89BD.pdf"'
page INTRANET_E57B9774-4712-430E-93E0-E67433F7DF07/documents/8/844418D91B7D26A2EB20F0D19CE6F625F9E89BD.pdf 1warning: found duplicate fz_icc_link in the store
 352ms (interpretation) 142ms (rendering) 494ms (total)
Glyph Cache Size: 591342
Glyph Cache Evictions: 0 (0 bytes)
page INTRANET_E57B9774-4712-430E-93E0-E67433F7DF07/documents/8/844418D91B7D26A2EB20F0D19CE6F625F9E89BD.pdf 2 43ms (interpretation) 74ms (rendering) 117ms (total)
Glyph Cache Size: 888632
Glyph Cache Evictions: 0 (0 bytes)
page INTRANET_E57B9774-4712-430E-93E0-E67433F7DF07/documents/8/844418D91B7D26A2EB20F0D19CE6F625F9E89BD.pdf 3 44ms (interpretation) 85ms (rendering) 129ms (total)
Glyph Cache Size: 1048502
Glyph Cache Evictions: 431 (171134 bytes)
page INTRANET_E57B9774-4712-430E-93E0-E67433F7DF07/documents/8/844418D91B7D26A2EB20F0D19CE6F625F9E89BD.pdf 4 27ms (interpretation) 44ms (rendering) 71ms (total)
Glyph Cache Size: 1048055
Glyph Cache Evictions: 599 (252755 bytes)
page INTRANET_E57B9774-4712-430E-93E0-E67433F7DF07/documents/8/844418D91B7D26A2EB20F0D19CE6F625F9E89BD.pdf 5 35ms (interpretation) 74ms (rendering) 109ms (total)
Glyph Cache Size: 1048494
Glyph Cache Evictions: 661 (288178 bytes)
page INTRANET_E57B9774-4712-430E-93E0-E67433F7DF07/documents/8/844418D91B7D26A2EB20F0D19CE6F625F9E89BD.pdf 6 23ms (interpretation) 33ms (rendering) 56ms (total)
Glyph Cache Size: 1048569
Glyph Cache Evictions: 718 (319603 bytes)
page INTRANET_E57B9774-4712-430E-93E0-E67433F7DF07/documents/8/844418D91B7D26A2EB20F0D19CE6F625F9E89BD.pdf 7 52ms (interpretation) 105ms (rendering) 157ms (total)
Glyph Cache Size: 1048441
Glyph Cache Evictions: 1226 (571935 bytes)
page INTRANET_E57B9774-4712-430E-93E0-E67433F7DF07/documents/8/844418D91B7D26A2EB20F0D19CE6F625F9E89BD.pdf 8 69ms (interpretation) 108ms (rendering) 177ms (total)
Glyph Cache Size: 1048542
Glyph Cache Evictions: 1991 (889107 bytes)
page INTRANET_E57B9774-4712-430E-93E0-E67433F7DF07/documents/8/844418D91B7D26A2EB20F0D19CE6F625F9E89BD.pdf 9 63ms (interpretation) 96ms (rendering) 159ms (total)
Glyph Cache Size: 1048521
Glyph Cache Evictions: 2600 (1136154 bytes)
page INTRANET_E57B9774-4712-430E-93E0-E67433F7DF07/documents/8/844418D91B7D26A2EB20F0D19CE6F625F9E89BD.pdf 10 70ms (interpretation) 109ms (rendering) 179ms (total)
Glyph Cache Size: 1048381
Glyph Cache Evictions: 3333 (1414130 bytes)
page INTRANET_E57B9774-4712-430E-93E0-E67433F7DF07/documents/8/844418D91B7D26A2EB20F0D19CE6F625F9E89BD.pdf 11 27ms (interpretation) 58ms (rendering) 85ms (total)
Glyph Cache Size: 1048553
Glyph Cache Evictions: 3482 (1474532 bytes)
page INTRANET_E57B9774-4712-430E-93E0-E67433F7DF07/documents/8/844418D91B7D26A2EB20F0D19CE6F625F9E89BD.pdf 12 68ms (interpretation) 111ms (rendering) 179ms (total)
Glyph Cache Size: 1048489
Glyph Cache Evictions: 4306 (1786230 bytes)
page INTRANET_E57B9774-4712-430E-93E0-E67433F7DF07/documents/8/844418D91B7D26A2EB20F0D19CE6F625F9E89BD.pdf 13 59ms (interpretation) 109ms (rendering) 168ms (total)
Glyph Cache Size: 1048423
Glyph Cache Evictions: 4883 (2003378 bytes)
page INTRANET_E57B9774-4712-430E-93E0-E67433F7DF07/documents/8/844418D91B7D26A2EB20F0D19CE6F625F9E89BD.pdf 14 66ms (interpretation) 101ms (rendering) 167ms (total)
Glyph Cache Size: 1048526
Glyph Cache Evictions: 5568 (2258130 bytes)
page INTRANET_E57B9774-4712-430E-93E0-E67433F7DF07/documents/8/844418D91B7D26A2EB20F0D19CE6F625F9E89BD.pdf 15error: expected object number
warning: repairing PDF document
error: array not closed before end of file
error: cannot parse object (508 0 R)
warning: cannot load object (508 0 R) into cache
error: cannot find object in xref (508 0 R)
warning: cannot load object (508 0 R) into cache
error: invalid page object
error: cannot find object in xref (509 0 R)
warning: cannot load object (509 0 R) into cache
error: cannot find object in xref (510 0 R)
warning: cannot load object (510 0 R) into cache
error: cannot find object in xref (511 0 R)
warning: cannot load object (511 0 R) into cache
 59ms (interpretation) 76ms (rendering) 135ms (total)
Glyerror: cannot find object in xref (495 0 R)
ph Cache Size: 1048339
Glyph Cacwarning: cannot load object (495 0 R) into cache
he Evictions: 6error: cannot find object in xref (496 0 R)
090 (2457644 bytes)
warning: cannot load object (496 0 R) into cache
error: cannot find object in xref (497 0 R)
warning: cannot load object (497 0 R) into cache
error: cannot find object in xref (498 0 R)
warning: cannot load object (498 0 R) into cache
error: cannot find object in xref (499 0 R)
warning: cannot load object (499 0 R) into cache
error: cannot find object in xref (500 0 R)
warning: cannot load object (500 0 R) into cache
error: cannot find object in xref (501 0 R)
warning: cannot load object (501 0 R) into cache
error: cannot find object in xref (502 0 R)
warning: cannot load object (502 0 R) into cache
error: cannot find object in xref (503 0 R)
warning: cannot load object (503 0 R) into cache
error: cannot find object in xref (504 0 R)
warning: cannot load object (504 0 R) into cache
error: cannot find object in xref (505 0 R)
warning: cannot load object (505 0 R) into cache
error: cannot find object in xref (506 0 R)
warning: cannot load object (506 0 R) into cache
error: cannot find object in xref (507 0 R)
warning: cannot load object (507 0 R) into cache
error: cannot find object in xref (509 0 R)
warning: cannot load object (509 0 R) into cache
error: cannot find object in xref (510 0 R)
warning: cannot load object (510 0 R) into cache
error: cannot find object in xref (511 0 R)
warning: cannot load object (511 0 R) into cache
error: cannot find object in xref (153 0 R)
warning: cannot load object (153 0 R) into cache
error: cannot find object in xref (153 0 R)
warning: cannot load object (153 0 R) into cache
error: cannot find object in xref (153 0 R)
warning: cannot load object (153 0 R) into cache
error: cannot find object in xref (153 0 R)
warning: cannot load object (153 0 R) into cache
error: cannot find object in xref (153 0 R)
warning: cannot load object (153 0 R) into cache
error: cannot find object in xref (153 0 R)
warning: cannot load object (153 0 R) into cache
error: cannot find object in xref (153 0 R)
warning: cannot load object (153 0 R) into cache
error: cannot find object in xref (153 0 R)
warning: cannot load object (153 0 R) into cache
error: cannot find object in xref (153 0 R)
warning: cannot load object (153 0 R) into cache
error: cannot find object in xref (153 0 R)
warning: cannot load object (153 0 R) into cache
error: cannot find object in xref (153 0 R)
warning: cannot load object (153 0 R) into cache
error: cannot find object in xref (153 0 R)
warning: cannot load object (153 0 R) into cache
error: cannot find object in xref (153 0 R)
warning: cannot load object (153 0 R) into cache
error: cannot find object in xref (153 0 R)
warning: cannot load object (153 0 R) into cache
error: cannot find object in xref (153 0 R)
warning: cannot load object (153 0 R) into cache
error: cannot find object in xref (153 0 R)
warning: cannot load object (153 0 R) into cache
error: cannot find object in xref (153 0 R)
warning: cannot load object (153 0 R) into cache
error: cannot find object in xref (153 0 R)
warning: cannot load object (153 0 R) into cache
error: cannot find object in xref (153 0 R)
warning: cannot load object (153 0 R) into cache
error: cannot find object in xref (153 0 R)
warning: cannot load object (153 0 R) into cache
error: cannot find object in xref (153 0 R)
warning: cannot load object (153 0 R) into cache
error: cannot find object in xref (153 0 R)
warning: cannot load object (153 0 R) into cache
error: cannot find object in xref (939 0 R)
warning: cannot load object (939 0 R) into cache
error: cannot find object in xref (153 0 R)
warning: cannot load object (153 0 R) into cache
error: cannot find object in xref (153 0 R)
warning: cannot load object (153 0 R) into cache
error: cannot find object in xref (153 0 R)
warning: cannot load object (153 0 R) into cache
error: cannot find object in xref (153 0 R)
warning: cannot load object (153 0 R) into cache
error: cannot find object in xref (153 0 R)
warning: cannot load object (153 0 R) into cache
error: cannot find object in xref (153 0 R)
warning: cannot load object (153 0 R) into cache
error: cannot find object in xref (153 0 R)
warning: cannot load object (153 0 R) into cache
error: cannot find object in xref (153 0 R)
warning: cannot load object (153 0 R) into cache
error: cannot find object in xref (153 0 R)
warning: cannot load object (153 0 R) into cache
error: cannot find object in xref (153 0 R)
warning: cannot load object (153 0 R) into cache
error: cannot find object in xref (153 0 R)
warning: cannot load object (153 0 R) into cache
error: cannot find object in xref (153 0 R)
warning: cannot load object (153 0 R) into cache
error: cannot find object in xref (153 0 R)
warning: cannot load object (153 0 R) into cache
error: cannot find object in xref (153 0 R)
warning: cannot load object (153 0 R) into cache
error: cannot find object in xref (153 0 R)
warning: cannot load object (153 0 R) into cache
error: cannot find object in xref (153 0 R)
warning: cannot load object (153 0 R) into cache
error: cannot find object in xref (153 0 R)
warning: cannot load object (153 0 R) into cache
error: cannot find object in xref (153 0 R)
warning: cannot load object (153 0 R) into cache
page INTRANET_E57B9774-4712-430E-93E0-E67433F7DF07/documents/8/844418D91B7D26A2EB20F0D19CE6F625F9E89BD.pdf 16error: cannot find object in xref (939 0 R)
warning: cannot load object (939 0 R) into cache
error: Page not found: 17
warning: error: Page not found: 17
error: cannot draw 'INTRANET_E57B9774-4712-430E-93E0-E67433F7DF07/documents/8/844418D91B7D26A2EB20F0D19CE6F625F9E89BD.pdf'
total 2382ms / 15 pages for an average of 158ms
fastest page 6: 56ms
slowest page 1: 494ms
```



**CRASH** looks like another double free error, now discovered via `fz_drop_pixmap()`







##### Item ♯00031





```
L#78062: MUTOOL muraster -o "__mujstest/INTRANET_E57B9774-4712-430E-93E0-E67433F7DF07/documents/8/%04d-844418D91B7D26A2EB20F0D19CE6F625F9E89BD.raster.ppm" -s mt -r 150 -P "INTRANET_E57B9774-4712-430E-93E0-E67433F7DF07/documents/8/844418D91B7D26A2EB20F0D19CE6F625F9E89BD.pdf"
```



**CRASH** with the newer code at `num_workers == 1` !!! It's the workers `[idx]->pix` entry which appears to have been nuked prematurely...







##### Item ♯00032


---
-- added commit SHA-1: 0ae30f8b6a058c18acf8be3a793a48ebb2ed2953

* fix: *finally* got the crash mofo: turns out it always happens when shutting down `muraster` as the illegal freed memory accesses always happen past these DEBUG_THREAD lines:


```
Asking workers to shutdown, then destroy their resources
Worker 0 woken for band_start -1
Worker 0 completed band_start -1 (status=0)
Worker 0 shutting down
Worker 1 woken for band_start -1
Worker 1 completed band_start -1 (status=0)
Worker 1 shutting down
Worker 2 woken for band_start -1
Worker 2 completed band_start -1 (status=0)
Worker 2 shutting down
```



while the call stack at time of failure points to code executing in the `bgprint` thread, which was cleaned up AFTER the workers all got nuked. Thus the obvious fix is to FIRST stop the `bgprint` thread and only then shut down all workers.

**A short bulk test run (~ minutes, then aborting the `mujstest` test) DOES NOT pop up any more 'random' crashes/heap corruptions after this fix.**

---







##### Item ♯00033





```
L#1528: MUTOOL muraster -o "__mujstest/INTRANET_E57B9774-4712-430E-93E0-E67433F7DF07/documents/4/%04d-458B1F6296CFA32442F7CC47752231F74B89E54D.raster.ppm" -s mt -r 150 -P "INTRANET_E57B9774-4712-430E-93E0-E67433F7DF07/documents/4/458B1F6296CFA32442F7CC47752231F74B89E54D.pdf"
BGPrint waiting
Worker 0 waiting
Worker 1 waiting
Worker 2 waiting
page INTRANET_E57B9774-4712-430E-93E0-E67433F7DF07/documents/4/458B1F6296CFA32442F7CC47752231F74B89E54D.pdf 1
BGPrint woken for pagenum 1
Using 3 Bands
Worker 0, Pre-triggering band 0
Worker 0 woken for band_start 0
Worker 1, Pre-triggering band 1
Worker 1 woken for band_start 608
Worker 2, Pre-triggering band 2
Waiting for worker 0 to complete band 0
Worker 2 woken for band_start 1216
corrupted refcount -572662305?
corrupted refcount -572662305?
corrupted refcount -572662305?
corrupted refcount -572662305?
corrupted refcount -572662304?
...
```



**CRASH** after `muraster` threads fix was done. Looks like the jbig2 corruption is still there...







##### Item ♯00034





```
L#1527: MUTOOL muraster -o "__mujstest/INTRANET_E57B9774-4712-430E-93E0-E67433F7DF07/documents/4/%04d-458B1F6296CFA32442F7CC47752231F74B89E54D.raster.ppm" -s mt -r 150 -P "INTRANET_E57B9774-4712-430E-93E0-E67433F7DF07/documents/4/458B1F6296CFA32442F7CC47752231F74B89E54D.pdf"
BGPrint waiting
Worker 0 waiting
Worker 1 waiting
Worker 2 waiting
page INTRANET_E57B9774-4712-430E-93E0-E67433F7DF07/documents/4/458B1F6296CFA32442F7CC47752231F74B89E54D.pdf 1
BGPrint woken for pagenum 1
Using 3 Bands
Worker 0, Pre-triggering band 0
Worker 0 woken for band_start 0
Worker 1, Pre-triggering band 1
Worker 1 woken for band_start 608
Worker 2, Pre-triggering band 2
Waiting for worker 0 to complete band 0
Worker 2 woken for band_start 1216
Worker 2 completed band_start 1216 (status=0)
Worker 2 waiting
Worker 0 completed band_start 0 (status=0)
Worker 0 waiting
Waiting for worker 1 to complete band 1
Worker 1 completed band_start 608 (status=0)
Worker 1 waiting
Waiting for worker 2 to complete band 2
 18ms (interpretation) 418ms (rendering) 436ms (total)
Glyph Cache Size: 0
Glyph Cache Evictions: 0 (0 bytes)
BGPrint completed page 1
BGPrint waiting
page INTRANET_E57B9774-4712-430E-93E0-E67433F7DF07/documents/4/458B1F6296CFA32442F7CC47752231F74B89E54D.pdf 2
BGPrint woken for pagenum 2
Using 3 Bands
Worker 0, Pre-triggering band 0
Worker 0 woken for band_start 0
Worker 1, Pre-triggering band 1
Worker 1 woken for band_start 608
Worker 2, Pre-triggering band 2
Waiting for worker 0 to complete band 0
Worker 2 woken for band_start 1216
corrupted refcount -572662306?
corrupted refcount -572662305?
corrupted refcount -572662304?
corrupted refcount -572662305?
corrupted refcount -572662304?
corrupted refcount -572662305?
corrupted refcount -572662304?
corrupted refcount -572662305?
corrupted refcount -572662304?
corrupted refcount -572662305?
corrupted refcount -572662304?
corrupted refcount -572662305?
corrupted refcount -572662304?
corrupted refcount -572662305?
corrupted refcount -572662304?
corrupted refcount -572662305?
corrupted refcount -572662304?
corrupted refcount -572662305?
corrupted refcount -572662304?
corrupted refcount -572662305?
corrupted refcount -572662304?
corrupted refcount -572662305?
corrupted refcount -572662304?
```











##### Item ♯00035


--------------------------------------------------------------------------------------------------------------

## New `mujstest`. New tests. (`mutool metadump`)


```
6288  "digitalcorpora.org/govdocs1/025/025454.pdf"  "digitalcorpora.org/govdocs1/025"    "025454.pdf"      "025454"      "G:/Qiqqa/evil-base"
```



^ has to recover json depth for almost(?) every page. That's unusual in `write_level_guarantee_level()``.
--> turns out this PDF has type:'UNKNOWN' annotations!

^ has unsupported `FICLblablabla` annotations, which are reported as UNKNOWN (of course)







##### Item ♯00036





```
13715  "digitalcorpora.org/govdocs1/999/999686.pdf"  "digitalcorpora.org/govdocs1/999"    "999686.pdf"      "999686"      "G:/Qiqqa/evil-base"
```



^ has to recover json depth for almost(?) every page. That's unusual in `write_level_guarantee_level()`.







##### Item ♯00037





```
14835  "openpreserve-format-corpus/pdfCabinetOfHorrors/embedded_video_avi.pdf"  "openpreserve-format-corpus/pdfCabinetOfHorrors"    "embedded_video_avi.pdf"      "embedded_video_avi"      "G:/Qiqqa/evil-base"
```



^ ditto







##### Item ♯00038





```
15189  "pdfium_tests/fx/FRC_8.5_part2/FRC_8.5_PC_GoToE_T_R&P&A.pdf"  "pdfium_tests/fx/FRC_8.5_part2"    "FRC_8.5_PC_GoToE_T_R&P&A.pdf"      "FRC_8.5_PC_GoToE_T_R&P&A"      "G:/Qiqqa/evil-base"
```











##### Item ♯00039





```
15819  "Sample-PDFs-for-format-testing/doc - html to pdf - text runs off the right edge of the pages.pdf"  "Sample-PDFs-for-format-testing"    "doc - html to pdf - text runs off the right edge of the pages.pdf"      "doc - html to pdf - text runs off the right edge of the pages"      "G:/Qiqqa/evil-base"
```



^ superslow







##### Item ♯00040





```
16586  "Sample-PDFs-for-format-testing/isartor-pdfa-2008-08-13/Isartor testsuite/PDFA-1b/6.1 File structure/6.1.12 Implementation Limits/isartor-6-1-12-t01-fail-a.pdf"  "Sample-PDFs-for-format-testing/isartor-pdfa-2008-08-13/Isartor testsuite/PDFA-1b/6.1 File structure/6.1.12 Implementation Limits"    "isartor-6-1-12-t01-fail-a.pdf"      "isartor-6-1-12-t01-fail-a"      "G:/Qiqqa/evil-base"
```



^ superslow







##### Item ♯00041





```
18260  "Sample-PDFs-for-format-testing/Qiqqa Tools - test collection/insane-authors-list/Guidelines for the use and interpretation of assays for monitoring autophagy 3rd edition.pdf"  "Sample-PDFs-for-format-testing/Qiqqa Tools - test collection/insane-authors-list"    "Guidelines for the use and interpretation of assays for monitoring autophagy 3rd edition.pdf"      "Guidelines for the use and interpretation of assays for monitoring autophagy 3rd edition"      "G:/Qiqqa/evil-base"
```



^ memory corruption? json @ 500MBytes?! (ah, happened in x86/Win32 build. `realloc` of 1GB happened just before that in `fz_putc` handler)







##### Item ♯00042





```
952  "W:/Sopkonijn/!OmniPage-input-dir/__store/F571F789DCFBE5FCDEE03A6695F0285719708479.pdf"  "W/Sopkonijn/!OmniPage-input-dir/_store"    "F571F789DCFBE5FCDEE03A6695F0285719708479.pdf"      "F571F789DCFBE5FCDEE03A6695F0285719708479"      "G:/Qiqqa/evil-base"
```



^ insane output size; seems like the Attachedfiles item is going completely overboard. Weirdly enough with lots of Links?!?!







##### Item ♯00043





```
1897  "W:/Sopkonijn/!PaperPort-base-dir/B/B01BB061FFBF8A7873E55134575AB44C633042C.pdf"  "W/Sopkonijn/!PaperPort-base-dir/B"    "B01BB061FFBF8A7873E55134575AB44C633042C.pdf"      "B01BB061FFBF8A7873E55134575AB44C633042C"      "G:/Qiqqa/evil-base"
```



^ did not output anything for the Attachedfiles object while debugging?







##### Item ♯00044





```
1473  "W:/Sopkonijn/!PaperPort-base-dir/1/10822C58B7CE8328698EEB29F438183AB224482.pdf"  "W/Sopkonijn/!PaperPort-base-dir/1"    "10822C58B7CE8328698EEB29F438183AB224482.pdf"      "10822C58B7CE8328698EEB29F438183AB224482"      "G:/Qiqqa/evil-base"
```



^ a bit of nastiness around an empty stream, which results in the ludicrous "cannot parse as XML" error. Well, DUH!







##### Item ♯00045





```
1526  "W:/Sopkonijn/!PaperPort-base-dir/2/288765F82241BB31D94F35E58F41C0BCE4A44B0.pdf"  "W/Sopkonijn/!PaperPort-base-dir/2"    "288765F82241BB31D94F35E58F41C0BCE4A44B0.pdf"      "288765F82241BB31D94F35E58F41C0BCE4A44B0"      "G:/Qiqqa/evil-base"
```



^ another run-away that's kept in check by the "restricted mode"







##### Item ♯00046





```
3890  "W:/Sopkonijn/!QIQQA-pdf-buffer-dir/__nontext/67A9624514511BF3C88DACAA32618EDFFC6F1.pdf"  "W/Sopkonijn/!QIQQA-pdf-buffer-dir/__nontext"    "67A9624514511BF3C88DACAA32618EDFFC6F1.pdf"      "67A9624514511BF3C88DACAA32618EDFFC6F1"      "G:/Qiqqa/evil-base"
```



^ has UNKNOWN annotations.







##### Item ♯00047





```
7103  "W:/Sopkonijn/!QIQQA-pdf-buffer-dir/__possibly_erroneous/EBA9C2BAC1E301AEFA99296E0BF43CEBDA4D175.pdf"  "W/Sopkonijn/!QIQQA-pdf-buffer-dir/__possibly_erroneous"    "EBA9C2BAC1E301AEFA99296E0BF43CEBDA4D175.pdf"      "EBA9C2BAC1E301AEFA99296E0BF43CEBDA4D175"      "G:/Qiqqa/evil-base"
```



^ *extremely* long running metadump. (Long stream of dots on the console too.)







##### Item ♯00048





```
warning: cannot load object (1267 0 R) into cache
error: object out of range (1267 0 R); xref size 1249
warning: cannot load object (1267 0 R) into cache
error: object out of range (1267 0 R); xref size 1249
warning: cannot load object (1267 0 R) into cache
warning: non-page object in page tree ({NULL})
error: object out of range (1270 0 R); xref size 1249
```



etc.etc.etc.







##### Item ♯00049





```
5855  "W:/Sopkonijn/!QIQQA-pdf-buffer-dir/__nontext/DE242ACFFB486163801EDBACCB74244D287B8EF8.pdf"  "W/Sopkonijn/!QIQQA-pdf-buffer-dir/__nontext"    "DE242ACFFB486163801EDBACCB74244D287B8EF8.pdf"      "DE242ACFFB486163801EDBACCB74244D287B8EF8"      "G:/Qiqqa/evil-base"
```



^ long running.







##### Item ♯00050





```
metadump -o "__bulktest/W/Sopkonijn/!QIQQA-pdf-buffer-dir/__possibly_erroneous/EBA9C2BAC1E301AEFA99296E0BF43CEBDA4D175.info.json" "W:/Sopkonijn/!QIQQA-pdf-buffer-dir/__possibly_erroneous/EBA9C2BAC1E301AEFA99296E0BF43CEBDA4D175.pdf"
```



^ ### longest running b****d evar! ###







##### Item ♯00051





```
T:6356h1.[E]..
T:6360h1.[E]..
T:6361h1.[E]..
T:6366h1.[E]..
T:6368h1.[E]..
T:6371h1.[E]..
T:6375h1.[E]..
T:6376h1.[E]..
T:6381h1.[E]..
T:6385h1.[E]..
T:6387h1.[E]..
T:6392h1.[E]..
T:6395h1.[E]..
T:6401h1.[E]..
T:6407h1.[E]..
T:6410h1.[E]..
T:6411h1.[E]..
T:6425h1.[E]..
T:6427h1.[E]..
T:6433h1.[E]..
T:6434h1.[E]..
T:6436h1.[E]..
T:6440h1.[E]..
T:6443h1.[E]..
T:6445h1.[E]..
T:6446h1.[E]..
T:6457h1.[E]..
T:6460h1.[E]..
T:6466h1....[E]..
T:6470h1....[E]..
T:6485h1.[E]..
T:6487h1.[E]..
T:6488h1.[E]..
T:6489h1.[E]..
T:6491h1.[E]..
T:6494h1.[E]..
T:6498h1.[E]..
T:6500h1.[E]..
T:6503h1.[E]..
T:6504h1.[E]..
T:6512h1.[E]..
T:6513h1.[E]..
T:6517h1.[E]..
T:6518h1.[E]..
T:6521h1.[E]..
T:6525h1.[E]..
```



==>







##### Item ♯00052





```
6356  "W:/Sopkonijn/!QIQQA-pdf-buffer-dir/__possibly_erroneous/1019621951CA235A4035E9D61F56AF774174978E.pdf"  "W/Sopkonijn/!QIQQA-pdf-buffer-dir/__possibly_erroneous"    "1019621951CA235A4035E9D61F56AF774174978E.pdf"      "1019621951CA235A4035E9D61F56AF774174978E"      "G:/Qiqqa/evil-base"
6360  "W:/Sopkonijn/!QIQQA-pdf-buffer-dir/__possibly_erroneous/1132DFE1BCFFD2D62ED281A264E42A658E683.pdf"  "W/Sopkonijn/!QIQQA-pdf-buffer-dir/__possibly_erroneous"    "1132DFE1BCFFD2D62ED281A264E42A658E683.pdf"      "1132DFE1BCFFD2D62ED281A264E42A658E683"      "G:/Qiqqa/evil-base"
6361  "W:/Sopkonijn/!QIQQA-pdf-buffer-dir/__possibly_erroneous/11386151C2A734024FFBC369CFE2C46E52513EB.pdf"  "W/Sopkonijn/!QIQQA-pdf-buffer-dir/__possibly_erroneous"    "11386151C2A734024FFBC369CFE2C46E52513EB.pdf"      "11386151C2A734024FFBC369CFE2C46E52513EB"      "G:/Qiqqa/evil-base"
6366  "W:/Sopkonijn/!QIQQA-pdf-buffer-dir/__possibly_erroneous/11E0FA94F8FAD09297BA96AE4452EECF8ECCCCEB.pdf"  "W/Sopkonijn/!QIQQA-pdf-buffer-dir/__possibly_erroneous"    "11E0FA94F8FAD09297BA96AE4452EECF8ECCCCEB.pdf"      "11E0FA94F8FAD09297BA96AE4452EECF8ECCCCEB"      "G:/Qiqqa/evil-base"
6368  "W:/Sopkonijn/!QIQQA-pdf-buffer-dir/__possibly_erroneous/127447489B6D6EF9FFD58702657DA6456A42.pdf"  "W/Sopkonijn/!QIQQA-pdf-buffer-dir/__possibly_erroneous"    "127447489B6D6EF9FFD58702657DA6456A42.pdf"      "127447489B6D6EF9FFD58702657DA6456A42"      "G:/Qiqqa/evil-base"
6371  "W:/Sopkonijn/!QIQQA-pdf-buffer-dir/__possibly_erroneous/13E0C2542461EA9726BB125B5F89B16655C6DDF1.pdf"  "W/Sopkonijn/!QIQQA-pdf-buffer-dir/__possibly_erroneous"    "13E0C2542461EA9726BB125B5F89B16655C6DDF1.pdf"      "13E0C2542461EA9726BB125B5F89B16655C6DDF1"      "G:/Qiqqa/evil-base"
6375  "W:/Sopkonijn/!QIQQA-pdf-buffer-dir/__possibly_erroneous/148174D1E91C25CD3CF77CA84675C7184669EC4.pdf"  "W/Sopkonijn/!QIQQA-pdf-buffer-dir/__possibly_erroneous"    "148174D1E91C25CD3CF77CA84675C7184669EC4.pdf"      "148174D1E91C25CD3CF77CA84675C7184669EC4"      "G:/Qiqqa/evil-base"
6376  "W:/Sopkonijn/!QIQQA-pdf-buffer-dir/__possibly_erroneous/1498C0CCFD80E285507E9A587AF07B46AC78FBC.pdf"  "W/Sopkonijn/!QIQQA-pdf-buffer-dir/__possibly_erroneous"    "1498C0CCFD80E285507E9A587AF07B46AC78FBC.pdf"      "1498C0CCFD80E285507E9A587AF07B46AC78FBC"      "G:/Qiqqa/evil-base"
6381  "W:/Sopkonijn/!QIQQA-pdf-buffer-dir/__possibly_erroneous/159042E426B01E814DA8F6F993D3546B0C1C31B.pdf"  "W/Sopkonijn/!QIQQA-pdf-buffer-dir/__possibly_erroneous"    "159042E426B01E814DA8F6F993D3546B0C1C31B.pdf"      "159042E426B01E814DA8F6F993D3546B0C1C31B"      "G:/Qiqqa/evil-base"
6385  "W:/Sopkonijn/!QIQQA-pdf-buffer-dir/__possibly_erroneous/161277FC1F22EC6D2D3B6DA13FDCE9EE48DD76AD.pdf"  "W/Sopkonijn/!QIQQA-pdf-buffer-dir/__possibly_erroneous"    "161277FC1F22EC6D2D3B6DA13FDCE9EE48DD76AD.pdf"      "161277FC1F22EC6D2D3B6DA13FDCE9EE48DD76AD"      "G:/Qiqqa/evil-base"
6387  "W:/Sopkonijn/!QIQQA-pdf-buffer-dir/__possibly_erroneous/161E28C471A830D6A95FF67F21478A88F2287CF8.pdf"  "W/Sopkonijn/!QIQQA-pdf-buffer-dir/__possibly_erroneous"    "161E28C471A830D6A95FF67F21478A88F2287CF8.pdf"      "161E28C471A830D6A95FF67F21478A88F2287CF8"      "G:/Qiqqa/evil-base"
6392  "W:/Sopkonijn/!QIQQA-pdf-buffer-dir/__possibly_erroneous/168DA89E4205F98BB4ACC982C63564ABDD555E.pdf"  "W/Sopkonijn/!QIQQA-pdf-buffer-dir/__possibly_erroneous"    "168DA89E4205F98BB4ACC982C63564ABDD555E.pdf"      "168DA89E4205F98BB4ACC982C63564ABDD555E"      "G:/Qiqqa/evil-base"
6395  "W:/Sopkonijn/!QIQQA-pdf-buffer-dir/__possibly_erroneous/16E219C0B0F114DD391611786CE2647231474F1B.pdf"  "W/Sopkonijn/!QIQQA-pdf-buffer-dir/__possibly_erroneous"    "16E219C0B0F114DD391611786CE2647231474F1B.pdf"      "16E219C0B0F114DD391611786CE2647231474F1B"      "G:/Qiqqa/evil-base"
6401  "W:/Sopkonijn/!QIQQA-pdf-buffer-dir/__possibly_erroneous/184B5EB5E868FDBAABA8E2ED653B3E382221BB36.pdf"  "W/Sopkonijn/!QIQQA-pdf-buffer-dir/__possibly_erroneous"    "184B5EB5E868FDBAABA8E2ED653B3E382221BB36.pdf"      "184B5EB5E868FDBAABA8E2ED653B3E382221BB36"      "G:/Qiqqa/evil-base"
6407  "W:/Sopkonijn/!QIQQA-pdf-buffer-dir/__possibly_erroneous/19178EA63FEFDAED4D785B7DB88DAFD23ADDD59.pdf"  "W/Sopkonijn/!QIQQA-pdf-buffer-dir/__possibly_erroneous"    "19178EA63FEFDAED4D785B7DB88DAFD23ADDD59.pdf"      "19178EA63FEFDAED4D785B7DB88DAFD23ADDD59"      "G:/Qiqqa/evil-base"
6410  "W:/Sopkonijn/!QIQQA-pdf-buffer-dir/__possibly_erroneous/19BF65443B6A02C587826417CDE34BB8C56DAB.pdf"  "W/Sopkonijn/!QIQQA-pdf-buffer-dir/__possibly_erroneous"    "19BF65443B6A02C587826417CDE34BB8C56DAB.pdf"      "19BF65443B6A02C587826417CDE34BB8C56DAB"      "G:/Qiqqa/evil-base"
6411  "W:/Sopkonijn/!QIQQA-pdf-buffer-dir/__possibly_erroneous/19CFBE24F874A2C8EE891A21BF8CE3BE07CA1.pdf"  "W/Sopkonijn/!QIQQA-pdf-buffer-dir/__possibly_erroneous"    "19CFBE24F874A2C8EE891A21BF8CE3BE07CA1.pdf"      "19CFBE24F874A2C8EE891A21BF8CE3BE07CA1"      "G:/Qiqqa/evil-base"
6425  "W:/Sopkonijn/!QIQQA-pdf-buffer-dir/__possibly_erroneous/1ECE9A6BBD9C91EBD276C80B38B92401BE37966.pdf"  "W/Sopkonijn/!QIQQA-pdf-buffer-dir/__possibly_erroneous"    "1ECE9A6BBD9C91EBD276C80B38B92401BE37966.pdf"      "1ECE9A6BBD9C91EBD276C80B38B92401BE37966"      "G:/Qiqqa/evil-base"
6427  "W:/Sopkonijn/!QIQQA-pdf-buffer-dir/__possibly_erroneous/1FB1CC9ACAC28DBA64F74B3BABD554EB7E8A4E6B.pdf"  "W/Sopkonijn/!QIQQA-pdf-buffer-dir/__possibly_erroneous"    "1FB1CC9ACAC28DBA64F74B3BABD554EB7E8A4E6B.pdf"      "1FB1CC9ACAC28DBA64F74B3BABD554EB7E8A4E6B"      "G:/Qiqqa/evil-base"
6433  "W:/Sopkonijn/!QIQQA-pdf-buffer-dir/__possibly_erroneous/212E4EC7E4F887B761AB641A289B721A7A7784FB.pdf"  "W/Sopkonijn/!QIQQA-pdf-buffer-dir/__possibly_erroneous"    "212E4EC7E4F887B761AB641A289B721A7A7784FB.pdf"      "212E4EC7E4F887B761AB641A289B721A7A7784FB"      "G:/Qiqqa/evil-base"
6434  "W:/Sopkonijn/!QIQQA-pdf-buffer-dir/__possibly_erroneous/2179B74C965BF45B7FBC6A17752E622EDFDE125E.pdf"  "W/Sopkonijn/!QIQQA-pdf-buffer-dir/__possibly_erroneous"    "2179B74C965BF45B7FBC6A17752E622EDFDE125E.pdf"      "2179B74C965BF45B7FBC6A17752E622EDFDE125E"      "G:/Qiqqa/evil-base"
6436  "W:/Sopkonijn/!QIQQA-pdf-buffer-dir/__possibly_erroneous/21B9CF595F16363E1F7CD189BE2AF4E5A36489.pdf"  "W/Sopkonijn/!QIQQA-pdf-buffer-dir/__possibly_erroneous"    "21B9CF595F16363E1F7CD189BE2AF4E5A36489.pdf"      "21B9CF595F16363E1F7CD189BE2AF4E5A36489"      "G:/Qiqqa/evil-base"
6440  "W:/Sopkonijn/!QIQQA-pdf-buffer-dir/__possibly_erroneous/229915928C7DBA11BA758CE3DE31F9D583A1CFB8.pdf"  "W/Sopkonijn/!QIQQA-pdf-buffer-dir/__possibly_erroneous"    "229915928C7DBA11BA758CE3DE31F9D583A1CFB8.pdf"      "229915928C7DBA11BA758CE3DE31F9D583A1CFB8"      "G:/Qiqqa/evil-base"
6443  "W:/Sopkonijn/!QIQQA-pdf-buffer-dir/__possibly_erroneous/2338F0386B85B0895C051ED85E642FAF9CCF3.pdf"  "W/Sopkonijn/!QIQQA-pdf-buffer-dir/__possibly_erroneous"    "2338F0386B85B0895C051ED85E642FAF9CCF3.pdf"      "2338F0386B85B0895C051ED85E642FAF9CCF3"      "G:/Qiqqa/evil-base"
6445  "W:/Sopkonijn/!QIQQA-pdf-buffer-dir/__possibly_erroneous/236C55C520E42E1D5872ACD864C61D37E12DEA.pdf"  "W/Sopkonijn/!QIQQA-pdf-buffer-dir/__possibly_erroneous"    "236C55C520E42E1D5872ACD864C61D37E12DEA.pdf"      "236C55C520E42E1D5872ACD864C61D37E12DEA"      "G:/Qiqqa/evil-base"
6446  "W:/Sopkonijn/!QIQQA-pdf-buffer-dir/__possibly_erroneous/23BBF51B0A628C96471D126A2521688E797F0.pdf"  "W/Sopkonijn/!QIQQA-pdf-buffer-dir/__possibly_erroneous"    "23BBF51B0A628C96471D126A2521688E797F0.pdf"      "23BBF51B0A628C96471D126A2521688E797F0"      "G:/Qiqqa/evil-base"
6457  "W:/Sopkonijn/!QIQQA-pdf-buffer-dir/__possibly_erroneous/276338EB1B8E337F54BD66E1BCB78D8A2807578.pdf"  "W/Sopkonijn/!QIQQA-pdf-buffer-dir/__possibly_erroneous"    "276338EB1B8E337F54BD66E1BCB78D8A2807578.pdf"      "276338EB1B8E337F54BD66E1BCB78D8A2807578"      "G:/Qiqqa/evil-base"
6460  "W:/Sopkonijn/!QIQQA-pdf-buffer-dir/__possibly_erroneous/288D662EF0810AFCB856B49C31984A025F639F0.pdf"  "W/Sopkonijn/!QIQQA-pdf-buffer-dir/__possibly_erroneous"    "288D662EF0810AFCB856B49C31984A025F639F0.pdf"      "288D662EF0810AFCB856B49C31984A025F639F0"      "G:/Qiqqa/evil-base"
6466  "W:/Sopkonijn/!QIQQA-pdf-buffer-dir/__possibly_erroneous/2B5BAEDB61348943BE6A18621F3B9FBF299FB42A.pdf"  "W/Sopkonijn/!QIQQA-pdf-buffer-dir/__possibly_erroneous"    "2B5BAEDB61348943BE6A18621F3B9FBF299FB42A.pdf"      "2B5BAEDB61348943BE6A18621F3B9FBF299FB42A"      "G:/Qiqqa/evil-base"
6470  "W:/Sopkonijn/!QIQQA-pdf-buffer-dir/__possibly_erroneous/2CADAF131B0F1AB28226C4DD8AA37C3313BD4A.pdf"  "W/Sopkonijn/!QIQQA-pdf-buffer-dir/__possibly_erroneous"    "2CADAF131B0F1AB28226C4DD8AA37C3313BD4A.pdf"      "2CADAF131B0F1AB28226C4DD8AA37C3313BD4A"      "G:/Qiqqa/evil-base"
6485  "W:/Sopkonijn/!QIQQA-pdf-buffer-dir/__possibly_erroneous/32DF17A1FCEDEB7A5B086EBCD48CB24F5713D.pdf"  "W/Sopkonijn/!QIQQA-pdf-buffer-dir/__possibly_erroneous"    "32DF17A1FCEDEB7A5B086EBCD48CB24F5713D.pdf"      "32DF17A1FCEDEB7A5B086EBCD48CB24F5713D"      "G:/Qiqqa/evil-base"
6487  "W:/Sopkonijn/!QIQQA-pdf-buffer-dir/__possibly_erroneous/334AEB72B7C516C395A3417AB75EC9CF5B42F9.pdf"  "W/Sopkonijn/!QIQQA-pdf-buffer-dir/__possibly_erroneous"    "334AEB72B7C516C395A3417AB75EC9CF5B42F9.pdf"      "334AEB72B7C516C395A3417AB75EC9CF5B42F9"      "G:/Qiqqa/evil-base"
6488  "W:/Sopkonijn/!QIQQA-pdf-buffer-dir/__possibly_erroneous/3373E5F1EEE2B6E9A64A3FA1845BC846FDC4864.pdf"  "W/Sopkonijn/!QIQQA-pdf-buffer-dir/__possibly_erroneous"    "3373E5F1EEE2B6E9A64A3FA1845BC846FDC4864.pdf"      "3373E5F1EEE2B6E9A64A3FA1845BC846FDC4864"      "G:/Qiqqa/evil-base"
6489  "W:/Sopkonijn/!QIQQA-pdf-buffer-dir/__possibly_erroneous/33ADE01CB38A780FE1282259EF3AD14CAAD6.pdf"  "W/Sopkonijn/!QIQQA-pdf-buffer-dir/__possibly_erroneous"    "33ADE01CB38A780FE1282259EF3AD14CAAD6.pdf"      "33ADE01CB38A780FE1282259EF3AD14CAAD6"      "G:/Qiqqa/evil-base"
6491  "W:/Sopkonijn/!QIQQA-pdf-buffer-dir/__possibly_erroneous/3445BD41267AEC1673B6DE7B14FAF4F971CA356.pdf"  "W/Sopkonijn/!QIQQA-pdf-buffer-dir/__possibly_erroneous"    "3445BD41267AEC1673B6DE7B14FAF4F971CA356.pdf"      "3445BD41267AEC1673B6DE7B14FAF4F971CA356"      "G:/Qiqqa/evil-base"
6494  "W:/Sopkonijn/!QIQQA-pdf-buffer-dir/__possibly_erroneous/35C397CECA1057B0C886782BBCBEF8FB7A1CEA4.pdf"  "W/Sopkonijn/!QIQQA-pdf-buffer-dir/__possibly_erroneous"    "35C397CECA1057B0C886782BBCBEF8FB7A1CEA4.pdf"      "35C397CECA1057B0C886782BBCBEF8FB7A1CEA4"      "G:/Qiqqa/evil-base"
6498  "W:/Sopkonijn/!QIQQA-pdf-buffer-dir/__possibly_erroneous/3625ECD7944D9842925BD1EE09EB049F81C35A3.pdf"  "W/Sopkonijn/!QIQQA-pdf-buffer-dir/__possibly_erroneous"    "3625ECD7944D9842925BD1EE09EB049F81C35A3.pdf"      "3625ECD7944D9842925BD1EE09EB049F81C35A3"      "G:/Qiqqa/evil-base"
6500  "W:/Sopkonijn/!QIQQA-pdf-buffer-dir/__possibly_erroneous/369AE2C9D3E350CFEC56F81F7ED6BE41CC721671.pdf"  "W/Sopkonijn/!QIQQA-pdf-buffer-dir/__possibly_erroneous"    "369AE2C9D3E350CFEC56F81F7ED6BE41CC721671.pdf"      "369AE2C9D3E350CFEC56F81F7ED6BE41CC721671"      "G:/Qiqqa/evil-base"
6503  "W:/Sopkonijn/!QIQQA-pdf-buffer-dir/__possibly_erroneous/37AE1E9FE523848B1DF6DF106CEB3A612D1C12.pdf"  "W/Sopkonijn/!QIQQA-pdf-buffer-dir/__possibly_erroneous"    "37AE1E9FE523848B1DF6DF106CEB3A612D1C12.pdf"      "37AE1E9FE523848B1DF6DF106CEB3A612D1C12"      "G:/Qiqqa/evil-base"
6504  "W:/Sopkonijn/!QIQQA-pdf-buffer-dir/__possibly_erroneous/37B88B31E7858D8FB90CE5E6D3CD97485DDECB4.pdf"  "W/Sopkonijn/!QIQQA-pdf-buffer-dir/__possibly_erroneous"    "37B88B31E7858D8FB90CE5E6D3CD97485DDECB4.pdf"      "37B88B31E7858D8FB90CE5E6D3CD97485DDECB4"      "G:/Qiqqa/evil-base"
6512  "W:/Sopkonijn/!QIQQA-pdf-buffer-dir/__possibly_erroneous/3A1522B1EBE6B82FA7E5169EF5FBE365EBB48AE.pdf"  "W/Sopkonijn/!QIQQA-pdf-buffer-dir/__possibly_erroneous"    "3A1522B1EBE6B82FA7E5169EF5FBE365EBB48AE.pdf"      "3A1522B1EBE6B82FA7E5169EF5FBE365EBB48AE"      "G:/Qiqqa/evil-base"
6513  "W:/Sopkonijn/!QIQQA-pdf-buffer-dir/__possibly_erroneous/3AA49E37CE4E83140FBEFC349790D46DAF94E.pdf"  "W/Sopkonijn/!QIQQA-pdf-buffer-dir/__possibly_erroneous"    "3AA49E37CE4E83140FBEFC349790D46DAF94E.pdf"      "3AA49E37CE4E83140FBEFC349790D46DAF94E"      "G:/Qiqqa/evil-base"
6517  "W:/Sopkonijn/!QIQQA-pdf-buffer-dir/__possibly_erroneous/3B23FBFB3F28A2CA94E4118E108812E803016.pdf"  "W/Sopkonijn/!QIQQA-pdf-buffer-dir/__possibly_erroneous"    "3B23FBFB3F28A2CA94E4118E108812E803016.pdf"      "3B23FBFB3F28A2CA94E4118E108812E803016"      "G:/Qiqqa/evil-base"
6518  "W:/Sopkonijn/!QIQQA-pdf-buffer-dir/__possibly_erroneous/3B2B882D8E96476A1F7DFF293FAD43DED9FDC9A4.pdf"  "W/Sopkonijn/!QIQQA-pdf-buffer-dir/__possibly_erroneous"    "3B2B882D8E96476A1F7DFF293FAD43DED9FDC9A4.pdf"      "3B2B882D8E96476A1F7DFF293FAD43DED9FDC9A4"      "G:/Qiqqa/evil-base"
6521  "W:/Sopkonijn/!QIQQA-pdf-buffer-dir/__possibly_erroneous/3BC36190CD4A3F17A46D3819CBA613DCEDC834E.pdf"  "W/Sopkonijn/!QIQQA-pdf-buffer-dir/__possibly_erroneous"    "3BC36190CD4A3F17A46D3819CBA613DCEDC834E.pdf"      "3BC36190CD4A3F17A46D3819CBA613DCEDC834E"      "G:/Qiqqa/evil-base"
6525  "W:/Sopkonijn/!QIQQA-pdf-buffer-dir/__possibly_erroneous/3CF99317358F3EF2BB498A7A1CFE084AFDCAC.pdf"  "W/Sopkonijn/!QIQQA-pdf-buffer-dir/__possibly_erroneous"    "3CF99317358F3EF2BB498A7A1CFE084AFDCAC.pdf"      "3CF99317358F3EF2BB498A7A1CFE084AFDCAC"      "G:/Qiqqa/evil-base"
```











##### Item ♯00053





```
T:6467h1...........................................................................................................................6528
```



==>







##### Item ♯00054





```
6467  "W:/Sopkonijn/!QIQQA-pdf-buffer-dir/__possibly_erroneous/2C5E75BAEDF9155B4F86A1AAE4E014444D3B73C6.pdf"  "W/Sopkonijn/!QIQQA-pdf-buffer-dir/__possibly_erroneous"    "2C5E75BAEDF9155B4F86A1AAE4E014444D3B73C6.pdf"      "2C5E75BAEDF9155B4F86A1AAE4E014444D3B73C6"      "G:/Qiqqa/evil-base"
```



^ another one with b0rked page index list.







##### Item ♯00055





```
6528  "W:/Sopkonijn/!QIQQA-pdf-buffer-dir/__possibly_erroneous/3D8E405C60709DA3BCCCCA75B203FA37C441BEB.pdf"  "W/Sopkonijn/!QIQQA-pdf-buffer-dir/__possibly_erroneous"    "3D8E405C60709DA3BCCCCA75B203FA37C441BEB.pdf"      "3D8E405C60709DA3BCCCCA75B203FA37C441BEB"      "G:/Qiqqa/evil-base"
```



^ another one with b0rked page index list. But also has other issues beefore that: still long running after the O(N^2) fix.







##### Item ♯00056





```
6792  "W:/Sopkonijn/!QIQQA-pdf-buffer-dir/__possibly_erroneous/824EFA4775A1EC0C518DCD5EA89274ABBF594.pdf"  "W/Sopkonijn/!QIQQA-pdf-buffer-dir/__possibly_erroneous"    "824EFA4775A1EC0C518DCD5EA89274ABBF594.pdf"      "824EFA4775A1EC0C518DCD5EA89274ABBF594"      "G:/Qiqqa/evil-base"
```



^ long running, many dots. To be analyzed.







##### Item ♯00057





```
7103  "W:/Sopkonijn/!QIQQA-pdf-buffer-dir/__possibly_erroneous/EBA9C2BAC1E301AEFA99296E0BF43CEBDA4D175.pdf"  "W/Sopkonijn/!QIQQA-pdf-buffer-dir/__possibly_erroneous"    "EBA9C2BAC1E301AEFA99296E0BF43CEBDA4D175.pdf"      "EBA9C2BAC1E301AEFA99296E0BF43CEBDA4D175"      "G:/Qiqqa/evil-base"
```



^ one more for the b0rked page index list.







##### Item ♯00058





```
7164  "W:/Sopkonijn/!QIQQA-pdf-buffer-dir/__prot/095DFE3BB8DD7FEB83BBAADE6F06DE059A1AC26.pdf"  "W/Sopkonijn/!QIQQA-pdf-buffer-dir/__prot"    "095DFE3BB8DD7FEB83BBAADE6F06DE059A1AC26.pdf"      "095DFE3BB8DD7FEB83BBAADE6F06DE059A1AC26"      "G:/Qiqqa/evil-base"
```



^ long running







##### Item ♯00059





```
7631  "W:/Sopkonijn/!QIQQA-pdf-buffer-dir/__prot/468CC099AC8AF0C781E0FBFA7F909913BBAA576C.pdf"  "W/Sopkonijn/!QIQQA-pdf-buffer-dir/__prot"    "468CC099AC8AF0C781E0FBFA7F909913BBAA576C.pdf"      "468CC099AC8AF0C781E0FBFA7F909913BBAA576C"      "G:/Qiqqa/evil-base"
```



^ long running, many dots







##### Item ♯00060





```
8143  "W:/Sopkonijn/!QIQQA-pdf-buffer-dir/__prot/832EA3DF23EF67A5E589E65A46BC7380DEFB84E.pdf"  "W/Sopkonijn/!QIQQA-pdf-buffer-dir/__prot"    "832EA3DF23EF67A5E589E65A46BC7380DEFB84E.pdf"      "832EA3DF23EF67A5E589E65A46BC7380DEFB84E"      "G:/Qiqqa/evil-base"
```



^ much to report: many dots?







##### Item ♯00061


All with many dots:


```
8666
8822
8997
9235
9278
9702
10106
10214
10361
10737
10893
11068
11242
11812
13149
```











##### Item ♯00062


Then: slow but quiet:


```
9740
10686
10940
11456
11836
13207
13783
14513   <-- takes a very long time!
15201

20106   <-- takes an extremely long time! (and quiet)
```



-->







##### Item ♯00063





```
8666  "W:/Sopkonijn/!QIQQA-pdf-buffer-dir/__prot/BFD232C63E13E740D28CB1B31843FA3EE6967A.pdf"  "W/Sopkonijn/!QIQQA-pdf-buffer-dir/__prot"    "BFD232C63E13E740D28CB1B31843FA3EE6967A.pdf"      "BFD232C63E13E740D28CB1B31843FA3EE6967A"      "G:/Qiqqa/evil-base"
8822  "W:/Sopkonijn/!QIQQA-pdf-buffer-dir/__prot/CEDEE4FFED79289710C2C813C71C283EF76ECBC.pdf"  "W/Sopkonijn/!QIQQA-pdf-buffer-dir/__prot"    "CEDEE4FFED79289710C2C813C71C283EF76ECBC.pdf"      "CEDEE4FFED79289710C2C813C71C283EF76ECBC"      "G:/Qiqqa/evil-base"
8997  "W:/Sopkonijn/!QIQQA-pdf-buffer-dir/__prot/E465BF1C5FD1CEFF2F6D30514F517F381B28AC31.pdf"  "W/Sopkonijn/!QIQQA-pdf-buffer-dir/__prot"    "E465BF1C5FD1CEFF2F6D30514F517F381B28AC31.pdf"      "E465BF1C5FD1CEFF2F6D30514F517F381B28AC31"      "G:/Qiqqa/evil-base"
9235  "W:/Sopkonijn/!QIQQA-pdf-buffer-dir/__prot/__decrypted/095DFE3BB8DD7FEB83BBAADE6F06DE059A1AC26.pdf"  "W/Sopkonijn/!QIQQA-pdf-buffer-dir/__prot/__decrypted"    "095DFE3BB8DD7FEB83BBAADE6F06DE059A1AC26.pdf"      "095DFE3BB8DD7FEB83BBAADE6F06DE059A1AC26"      "G:/Qiqqa/evil-base"
9278  "W:/Sopkonijn/!QIQQA-pdf-buffer-dir/__prot/__decrypted/14FE638AC0D5C68DAB666BF2ACA4EEB66BB39B.pdf"  "W/Sopkonijn/!QIQQA-pdf-buffer-dir/__prot/__decrypted"    "14FE638AC0D5C68DAB666BF2ACA4EEB66BB39B.pdf"      "14FE638AC0D5C68DAB666BF2ACA4EEB66BB39B"      "G:/Qiqqa/evil-base"
9702  "W:/Sopkonijn/!QIQQA-pdf-buffer-dir/__prot/__decrypted/468CC099AC8AF0C781E0FBFA7F909913BBAA576C.pdf"  "W/Sopkonijn/!QIQQA-pdf-buffer-dir/__prot/__decrypted"    "468CC099AC8AF0C781E0FBFA7F909913BBAA576C.pdf"      "468CC099AC8AF0C781E0FBFA7F909913BBAA576C"      "G:/Qiqqa/evil-base"
10106  "W:/Sopkonijn/!QIQQA-pdf-buffer-dir/__prot/__decrypted/73BCDE7DA27138A2E2AB1E9F3C6EFF53F5F457.pdf"  "W/Sopkonijn/!QIQQA-pdf-buffer-dir/__prot/__decrypted"    "73BCDE7DA27138A2E2AB1E9F3C6EFF53F5F457.pdf"      "73BCDE7DA27138A2E2AB1E9F3C6EFF53F5F457"      "G:/Qiqqa/evil-base"
10214  "W:/Sopkonijn/!QIQQA-pdf-buffer-dir/__prot/__decrypted/832EA3DF23EF67A5E589E65A46BC7380DEFB84E.pdf"  "W/Sopkonijn/!QIQQA-pdf-buffer-dir/__prot/__decrypted"    "832EA3DF23EF67A5E589E65A46BC7380DEFB84E.pdf"      "832EA3DF23EF67A5E589E65A46BC7380DEFB84E"      "G:/Qiqqa/evil-base"
10361  "W:/Sopkonijn/!QIQQA-pdf-buffer-dir/__prot/__decrypted/95DA8A14E63FF9B6EF1275206742B2F5E6FDFB41.pdf"  "W/Sopkonijn/!QIQQA-pdf-buffer-dir/__prot/__decrypted"    "95DA8A14E63FF9B6EF1275206742B2F5E6FDFB41.pdf"      "95DA8A14E63FF9B6EF1275206742B2F5E6FDFB41"      "G:/Qiqqa/evil-base"
10737  "W:/Sopkonijn/!QIQQA-pdf-buffer-dir/__prot/__decrypted/BFD232C63E13E740D28CB1B31843FA3EE6967A.pdf"  "W/Sopkonijn/!QIQQA-pdf-buffer-dir/__prot/__decrypted"    "BFD232C63E13E740D28CB1B31843FA3EE6967A.pdf"      "BFD232C63E13E740D28CB1B31843FA3EE6967A"      "G:/Qiqqa/evil-base"
10893  "W:/Sopkonijn/!QIQQA-pdf-buffer-dir/__prot/__decrypted/CEDEE4FFED79289710C2C813C71C283EF76ECBC.pdf"  "W/Sopkonijn/!QIQQA-pdf-buffer-dir/__prot/__decrypted"    "CEDEE4FFED79289710C2C813C71C283EF76ECBC.pdf"      "CEDEE4FFED79289710C2C813C71C283EF76ECBC"      "G:/Qiqqa/evil-base"
11068  "W:/Sopkonijn/!QIQQA-pdf-buffer-dir/__prot/__decrypted/E465BF1C5FD1CEFF2F6D30514F517F381B28AC31.pdf"  "W/Sopkonijn/!QIQQA-pdf-buffer-dir/__prot/__decrypted"    "E465BF1C5FD1CEFF2F6D30514F517F381B28AC31.pdf"      "E465BF1C5FD1CEFF2F6D30514F517F381B28AC31"      "G:/Qiqqa/evil-base"
11242  "W:/Sopkonijn/!QIQQA-pdf-buffer-dir/__prot/__decrypted/F883D33E62AF8683A9437C9D654213E66D76CB0.pdf"  "W/Sopkonijn/!QIQQA-pdf-buffer-dir/__prot/__decrypted"    "F883D33E62AF8683A9437C9D654213E66D76CB0.pdf"      "F883D33E62AF8683A9437C9D654213E66D76CB0"      "G:/Qiqqa/evil-base"
11812  "W:/Sopkonijn/!QIQQA-pdf-buffer-dir/__store/5963BDCC1E23D4D6121E5E6B7D66180DF01555.pdf"  "W/Sopkonijn/!QIQQA-pdf-buffer-dir/__store"    "5963BDCC1E23D4D6121E5E6B7D66180DF01555.pdf"      "5963BDCC1E23D4D6121E5E6B7D66180DF01555"      "G:/Qiqqa/evil-base"
13149  "W:/Sopkonijn/!QIQQA-pdf-buffer-dir/__store/AC295877350E7F61E182DF991061BEE6EECE37.pdf"  "W/Sopkonijn/!QIQQA-pdf-buffer-dir/__store"    "AC295877350E7F61E182DF991061BEE6EECE37.pdf"      "AC295877350E7F61E182DF991061BEE6EECE37"      "G:/Qiqqa/evil-base"
9740  "W:/Sopkonijn/!QIQQA-pdf-buffer-dir/__prot/__decrypted/4BBC72EB2F387BF10C4C01044105ACA5D6844C.pdf"  "W/Sopkonijn/!QIQQA-pdf-buffer-dir/__prot/__decrypted"    "4BBC72EB2F387BF10C4C01044105ACA5D6844C.pdf"      "4BBC72EB2F387BF10C4C01044105ACA5D6844C"      "G:/Qiqqa/evil-base"
10686  "W:/Sopkonijn/!QIQQA-pdf-buffer-dir/__prot/__decrypted/BA9D5F2A69C7DE1A2A1E3C8EB5B4325EDC5DE3.pdf"  "W/Sopkonijn/!QIQQA-pdf-buffer-dir/__prot/__decrypted"    "BA9D5F2A69C7DE1A2A1E3C8EB5B4325EDC5DE3.pdf"      "BA9D5F2A69C7DE1A2A1E3C8EB5B4325EDC5DE3"      "G:/Qiqqa/evil-base"
10940  "W:/Sopkonijn/!QIQQA-pdf-buffer-dir/__prot/__decrypted/D477415B6C5A46639F1CA89CA128B8886AABBBC.pdf"  "W/Sopkonijn/!QIQQA-pdf-buffer-dir/__prot/__decrypted"    "D477415B6C5A46639F1CA89CA128B8886AABBBC.pdf"      "D477415B6C5A46639F1CA89CA128B8886AABBBC"      "G:/Qiqqa/evil-base"
11456  "W:/Sopkonijn/!QIQQA-pdf-buffer-dir/__store/9686D3A65FA5BFE595E8FE745D068204F983.pdf"  "W/Sopkonijn/!QIQQA-pdf-buffer-dir/__store"    "9686D3A65FA5BFE595E8FE745D068204F983.pdf"      "9686D3A65FA5BFE595E8FE745D068204F983"      "G:/Qiqqa/evil-base"
11836  "W:/Sopkonijn/!QIQQA-pdf-buffer-dir/__store/2DEF82859E4194D1D91541DF71C440FC54AD67E.pdf"  "W/Sopkonijn/!QIQQA-pdf-buffer-dir/__store"    "2DEF82859E4194D1D91541DF71C440FC54AD67E.pdf"      "2DEF82859E4194D1D91541DF71C440FC54AD67E"      "G:/Qiqqa/evil-base"
13207  "W:/Sopkonijn/!QIQQA-pdf-buffer-dir/__store/114B6EDE5750B9B4FB7B8D842B10CBE259F9C783.pdf"  "W/Sopkonijn/!QIQQA-pdf-buffer-dir/__store"    "114B6EDE5750B9B4FB7B8D842B10CBE259F9C783.pdf"      "114B6EDE5750B9B4FB7B8D842B10CBE259F9C783"      "G:/Qiqqa/evil-base"
13783  "W:/Sopkonijn/!QIQQA-pdf-buffer-dir/__store/25F7DE75CF43954B18717CFDFAE79D7B6F704C.pdf"  "W/Sopkonijn/!QIQQA-pdf-buffer-dir/__store"    "25F7DE75CF43954B18717CFDFAE79D7B6F704C.pdf"      "25F7DE75CF43954B18717CFDFAE79D7B6F704C"      "G:/Qiqqa/evil-base"
14513  "W:/Sopkonijn/!QIQQA-pdf-buffer-dir/__store/36BA264A071B71E904D7F75D81BB7453676D5D2.pdf"  "W/Sopkonijn/!QIQQA-pdf-buffer-dir/__store"    "36BA264A071B71E904D7F75D81BB7453676D5D2.pdf"      "36BA264A071B71E904D7F75D81BB7453676D5D2"      "G:/Qiqqa/evil-base"
15201  "W:/Sopkonijn/!QIQQA-pdf-buffer-dir/__store/40B7F6A375FEBBCF434966E53493C9E16EE5C443.pdf"  "W/Sopkonijn/!QIQQA-pdf-buffer-dir/__store"    "40B7F6A375FEBBCF434966E53493C9E16EE5C443.pdf"      "40B7F6A375FEBBCF434966E53493C9E16EE5C443"      "G:/Qiqqa/evil-base"
20106  "W:/Sopkonijn/!QIQQA-pdf-buffer-dir/__store/865DD2BB2769B17A83D8BB9EB1AE2D31FCC2A88.pdf"  "W/Sopkonijn/!QIQQA-pdf-buffer-dir/__store"    "865DD2BB2769B17A83D8BB9EB1AE2D31FCC2A88.pdf"      "865DD2BB2769B17A83D8BB9EB1AE2D31FCC2A88"      "G:/Qiqqa/evil-base"
```











##### Item ♯00064





```
22638 -> another b0rked page list
23450
24649 -> slow & quiet
24629
```



-->







##### Item ♯00065





```
22638  "W:/Sopkonijn/!QIQQA-pdf-buffer-dir/__store/EBA9C2BAC1E301AEFA99296E0BF43CEBDA4D175.pdf"  "W/Sopkonijn/!QIQQA-pdf-buffer-dir/__store"    "EBA9C2BAC1E301AEFA99296E0BF43CEBDA4D175.pdf"      "EBA9C2BAC1E301AEFA99296E0BF43CEBDA4D175"      "G:/Qiqqa/evil-base"
23450  "W:/Sopkonijn/!QIQQA-pdf-buffer-dir/__store/E3C5755DB55A9F878E6429C59F8E40D574429E3C.pdf"  "W/Sopkonijn/!QIQQA-pdf-buffer-dir/__store"    "E3C5755DB55A9F878E6429C59F8E40D574429E3C.pdf"      "E3C5755DB55A9F878E6429C59F8E40D574429E3C"      "G:/Qiqqa/evil-base"
24649  "W:/Sopkonijn/!QIQQA-pdf-buffer-dir/__store/DE242ACFFB486163801EDBACCB74244D287B8EF8.pdf"  "W/Sopkonijn/!QIQQA-pdf-buffer-dir/__store"    "DE242ACFFB486163801EDBACCB74244D287B8EF8.pdf"      "DE242ACFFB486163801EDBACCB74244D287B8EF8"      "G:/Qiqqa/evil-base"
24629  "W:/Sopkonijn/!QIQQA-pdf-buffer-dir/__store/D477415B6C5A46639F1CA89CA128B8886AABBBC.pdf"  "W/Sopkonijn/!QIQQA-pdf-buffer-dir/__store"    "D477415B6C5A46639F1CA89CA128B8886AABBBC.pdf"      "D477415B6C5A46639F1CA89CA128B8886AABBBC"      "G:/Qiqqa/evil-base"
```











##### Item ♯00066





```
35976 --> another b0rked page list
36220 --> slow & quiet
36437
38107
38216
38294
38818
43210
43354
```



`78842` --> long running, some *weird* I/O `MessageBox` (it's a commandline tool we're running here!?!?!) -- hmmm, whole system is deteriorated now. Rebooting Win10. Running the `bulktest` in MSVC debugger overnight is not a good idea?!

--> and then THIS happened too: https://github.com/GerHobbelt/qiqqa-open-source/blob/master/docs-src/Technology/Odds%20'n'%20Ends/git%20-%20recovering%20from%20b0rked%20repos%20and%20systems.md







##### Item ♯00067





```
35976  "W:/Sopkonijn/!QIQQA-pdf-buffer-dir/__store/F0875819EB7BFC459CCF4F80912D9EC66E8B092.pdf"  "W/Sopkonijn/!QIQQA-pdf-buffer-dir/__store"    "F0875819EB7BFC459CCF4F80912D9EC66E8B092.pdf"      "F0875819EB7BFC459CCF4F80912D9EC66E8B092"      "G:/Qiqqa/evil-base"
36220  "W:/Sopkonijn/!QIQQA-pdf-buffer-dir/__store/F41F239D58A72CF188B7B4C714AF5A77D9964E8A.pdf"  "W/Sopkonijn/!QIQQA-pdf-buffer-dir/__store"    "F41F239D58A72CF188B7B4C714AF5A77D9964E8A.pdf"      "F41F239D58A72CF188B7B4C714AF5A77D9964E8A"      "G:/Qiqqa/evil-base"
36437  "W:/Sopkonijn/!QIQQA-pdf-buffer-dir/__store/824EFA4775A1EC0C518DCD5EA89274ABBF594.pdf"  "W/Sopkonijn/!QIQQA-pdf-buffer-dir/__store"    "824EFA4775A1EC0C518DCD5EA89274ABBF594.pdf"      "824EFA4775A1EC0C518DCD5EA89274ABBF594"      "G:/Qiqqa/evil-base"
38107  "W:/Sopkonijn/!QIQQA-pdf-buffer-dir/__store/37C5C8FC564E38D8F35F86908888A2BBCEB6FCE.pdf"  "W/Sopkonijn/!QIQQA-pdf-buffer-dir/__store"    "37C5C8FC564E38D8F35F86908888A2BBCEB6FCE.pdf"      "37C5C8FC564E38D8F35F86908888A2BBCEB6FCE"      "G:/Qiqqa/evil-base"
38216  "W:/Sopkonijn/!QIQQA-pdf-buffer-dir/__store/4F81216F1ED595FEE3F4666D75F5B010A73F68.pdf"  "W/Sopkonijn/!QIQQA-pdf-buffer-dir/__store"    "4F81216F1ED595FEE3F4666D75F5B010A73F68.pdf"      "4F81216F1ED595FEE3F4666D75F5B010A73F68"      "G:/Qiqqa/evil-base"
38294  "W:/Sopkonijn/!QIQQA-pdf-buffer-dir/__store/8474931999DBDF9BBAA31CFBF4E9C4F74FBC2E7.pdf"  "W/Sopkonijn/!QIQQA-pdf-buffer-dir/__store"    "8474931999DBDF9BBAA31CFBF4E9C4F74FBC2E7.pdf"      "8474931999DBDF9BBAA31CFBF4E9C4F74FBC2E7"      "G:/Qiqqa/evil-base"
38818  "W:/Sopkonijn/!QIQQA-pdf-buffer-dir/__store/CEDEE4FFED79289710C2C813C71C283EF76ECBC.pdf"  "W/Sopkonijn/!QIQQA-pdf-buffer-dir/__store"    "CEDEE4FFED79289710C2C813C71C283EF76ECBC.pdf"      "CEDEE4FFED79289710C2C813C71C283EF76ECBC"      "G:/Qiqqa/evil-base"
43210  "W:/Sopkonijn/!QIQQA-pdf-buffer-dir/__store/CAC1DDB10D944721C9E8EA8D99C76917EDE8238.pdf"  "W/Sopkonijn/!QIQQA-pdf-buffer-dir/__store"    "CAC1DDB10D944721C9E8EA8D99C76917EDE8238.pdf"      "CAC1DDB10D944721C9E8EA8D99C76917EDE8238"      "G:/Qiqqa/evil-base"
43354  "W:/Sopkonijn/!QIQQA-pdf-buffer-dir/__store/E7575B93AB1DEEBEF155152AC989BF4F866645.pdf"  "W/Sopkonijn/!QIQQA-pdf-buffer-dir/__store"    "E7575B93AB1DEEBEF155152AC989BF4F866645.pdf"      "E7575B93AB1DEEBEF155152AC989BF4F866645"      "G:/Qiqqa/evil-base"
78842  "W:/Sopkonijn/!QIQQA-pdf-buffer-dir/__store/__decrypted/C99DBCFC1B7CFC2DCE3094D9887E790C4BA4293.pdf"  "W/Sopkonijn/!QIQQA-pdf-buffer-dir/__store/__decrypted"    "C99DBCFC1B7CFC2DCE3094D9887E790C4BA4293.pdf"      "C99DBCFC1B7CFC2DCE3094D9887E790C4BA4293"      "G:/Qiqqa/evil-base"
```













##### Item ♯00068





```
78841 --> long running
78909
78914
78984
79045
79095
79150
79308
79493
80050
```



-->







##### Item ♯00069





```
78841  "W:/Sopkonijn/!QIQQA-pdf-buffer-dir/__store/__decrypted/C98FF828C3A8ECCF8942C41EE00F9DE7EC5663B.pdf"  "W/Sopkonijn/!QIQQA-pdf-buffer-dir/__store/__decrypted"    "C98FF828C3A8ECCF8942C41EE00F9DE7EC5663B.pdf"      "C98FF828C3A8ECCF8942C41EE00F9DE7EC5663B"      "G:/Qiqqa/evil-base"
78909  "W:/Sopkonijn/!QIQQA-pdf-buffer-dir/__store/__decrypted/CEB76A97C04941386A4196E08DDCEBDD23815.pdf"  "W/Sopkonijn/!QIQQA-pdf-buffer-dir/__store/__decrypted"    "CEB76A97C04941386A4196E08DDCEBDD23815.pdf"      "CEB76A97C04941386A4196E08DDCEBDD23815"      "G:/Qiqqa/evil-base"
78914  "W:/Sopkonijn/!QIQQA-pdf-buffer-dir/__store/__decrypted/CEDEE4FFED79289710C2C813C71C283EF76ECBC.pdf"  "W/Sopkonijn/!QIQQA-pdf-buffer-dir/__store/__decrypted"    "CEDEE4FFED79289710C2C813C71C283EF76ECBC.pdf"      "CEDEE4FFED79289710C2C813C71C283EF76ECBC"      "G:/Qiqqa/evil-base"
78984  "W:/Sopkonijn/!QIQQA-pdf-buffer-dir/__store/__decrypted/D41F7F488641921FA3A6CA99077664A7CA453C.pdf"  "W/Sopkonijn/!QIQQA-pdf-buffer-dir/__store/__decrypted"    "D41F7F488641921FA3A6CA99077664A7CA453C.pdf"      "D41F7F488641921FA3A6CA99077664A7CA453C"      "G:/Qiqqa/evil-base"
79045  "W:/Sopkonijn/!QIQQA-pdf-buffer-dir/__store/__decrypted/D83010D867FD6E8053A8974B59AAB8B7FC24940.pdf"  "W/Sopkonijn/!QIQQA-pdf-buffer-dir/__store/__decrypted"    "D83010D867FD6E8053A8974B59AAB8B7FC24940.pdf"      "D83010D867FD6E8053A8974B59AAB8B7FC24940"      "G:/Qiqqa/evil-base"
79095  "W:/Sopkonijn/!QIQQA-pdf-buffer-dir/__store/__decrypted/DB1D61FC999EAC6750B7AA456116377035CFB612.pdf"  "W/Sopkonijn/!QIQQA-pdf-buffer-dir/__store/__decrypted"    "DB1D61FC999EAC6750B7AA456116377035CFB612.pdf"      "DB1D61FC999EAC6750B7AA456116377035CFB612"      "G:/Qiqqa/evil-base"
79150  "W:/Sopkonijn/!QIQQA-pdf-buffer-dir/__store/__decrypted/DE242ACFFB486163801EDBACCB74244D287B8EF8.pdf"  "W/Sopkonijn/!QIQQA-pdf-buffer-dir/__store/__decrypted"    "DE242ACFFB486163801EDBACCB74244D287B8EF8.pdf"      "DE242ACFFB486163801EDBACCB74244D287B8EF8"      "G:/Qiqqa/evil-base"
79308  "W:/Sopkonijn/!QIQQA-pdf-buffer-dir/__store/__decrypted/E37FB9822BB48F3542188ACE70ED56A01B5B7A4.pdf"  "W/Sopkonijn/!QIQQA-pdf-buffer-dir/__store/__decrypted"    "E37FB9822BB48F3542188ACE70ED56A01B5B7A4.pdf"      "E37FB9822BB48F3542188ACE70ED56A01B5B7A4"      "G:/Qiqqa/evil-base"
79493  "W:/Sopkonijn/!QIQQA-pdf-buffer-dir/__store/__decrypted/F5F86A559370767F7089CF6317E65255EAC1C99A.pdf"  "W/Sopkonijn/!QIQQA-pdf-buffer-dir/__store/__decrypted"    "F5F86A559370767F7089CF6317E65255EAC1C99A.pdf"      "F5F86A559370767F7089CF6317E65255EAC1C99A"      "G:/Qiqqa/evil-base"
80050  "W:/Sopkonijn/!QIQQA-pdf-buffer-dir/__store/__decrypted/095DFE3BB8DD7FEB83BBAADE6F06DE059A1AC26.pdf"  "W/Sopkonijn/!QIQQA-pdf-buffer-dir/__store/__decrypted"    "095DFE3BB8DD7FEB83BBAADE6F06DE059A1AC26.pdf"      "095DFE3BB8DD7FEB83BBAADE6F06DE059A1AC26"      "G:/Qiqqa/evil-base"
```











##### Item ♯00070


Now let's see what last night delivered in terms of totally ludicrous JSON file sizes: we now 'restrict' single subtree dumps to ~10K, so shouldn't expect to see 10M+ filesizes unless we missed a few spots (I bet we did!):


```
78841  "W:/Sopkonijn/!QIQQA-pdf-buffer-dir/__store/__decrypted/C98FF828C3A8ECCF8942C41EE00F9DE7EC5663B.pdf"  "W/Sopkonijn/!QIQQA-pdf-buffer-dir/__store/__decrypted"    "C98FF828C3A8ECCF8942C41EE00F9DE7EC5663B.pdf"
```



--> turns out to produce >600MB JSON

`85187` --> hits the doc repair 0x02 mode (while we were scanning the dump directories, so "off topic")







##### Item ♯00071





```
85187  "W:/Sopkonijn/!QIQQA-pdf-buffer-dir/__store/3D8E405C60709DA3BCCCCA75B203FA37C441BEB.pdf"  "W/Sopkonijn/!QIQQA-pdf-buffer-dir/__store"    "3D8E405C60709DA3BCCCCA75B203FA37C441BEB.pdf"      "3D8E405C60709DA3BCCCCA75B203FA37C441BEB"      "G:/Qiqqa/evil-base"
```











##### Item ♯00072





```
decrypted 865DD2BB2769B17A83D8BB9EB1AE2D31FCC2A88 @ 900MByte JSON
decrypted 56831E9293475B87B10CB53E84AAD11B8455397 --> ditto
store/buffer 865DD2BB2769B17A83D8BB9EB1AE2D31FCC2A88
decrypted 36BA264A071B71E904D7F75D81BB7453676D5D2 --> ~600MB
1A6FF143434A295D1D2B2C3CC0EFA41AE4DAC
28A75398587080FE7F293EB4D91795E05DDD27F5
2DEF82859E4194D1D91541DF71C440FC54AD67E
DE242ACFFB486163801EDBACCB74244D287B8EF8
AE67D091CDA8EEC1713814711543C58B0317A74
3CE72EB0DEB05339F712F44C8256F037EF4C8BC4
28CECE527DA34A239261953F6DC64ADDEAD48C1
85AEA5A933422550131CD76D37AF7F24C7E6027
A377F2BEFE9575BD1622375433BF34FF7A518  (two versions of this file exist apparently, different JSON output sizes!)
EA757B9EA4D9A54BEDF26FCA33DB195EE4A864F
61857B878C1E9E0867C8821FA52AD389560C1E3
79BA47CBA2BE573449449C8DB324C7846A8653
4F81216F1ED595FEE3F4666D75F5B010A73F68
```



^ this selection ranges from >900MB JSON output down to a "measly" 12MByte (while `mutool metadump` already restricts many subtree dumps to 10K only, so these need to be investigated to see where we're leaking more of that crazy)







##### Item ♯00073


`90249` --> long running


```
90249  "W:/Sopkonijn/!QIQQA-pdf-buffer-dir/__store/56831E9293475B87B10CB53E84AAD11B8455397.pdf"  "W/Sopkonijn/!QIQQA-pdf-buffer-dir/__store"    "56831E9293475B87B10CB53E84AAD11B8455397.pdf"      "56831E9293475B87B10CB53E84AAD11B8455397"      "G:/Qiqqa/evil-base"
```



`92999` --> long running







##### Item ♯00074


---------------------------

For working on the hash functions, I need a couple of PDFs which exhibit the b0rked Qiqqa hash, i.e. shortened SHA1:


```
./Guest1/documents/6/643ED1D1967E8A83F6B17A28BB5D2A150.pdf
./INTRANET_EF52564A-831D-42F2-B956-815CF0418C08/documents/6/643ED1D1967E8A83F6B17A28BB5D2A150.pdf
./INTRANET_EF52564A-831D-42F2-B956-815CF0418C08/documents/3/39C53A7B9B641F85921729234DBE64C4D.pdf
./Guest1/documents/6/643ED1D1967E8A83F6B17A28BB5D2A150.pdf
./INTRANET_EF52564A-831D-42F2-B956-815CF0418C08/documents/6/643ED1D1967E8A83F6B17A28BB5D2A150.pdf
./INTRANET_EF52564A-831D-42F2-B956-815CF0418C08/documents/8/89DC1ADAB9C58F791BECBE22F878AC0E9.pdf
./INTRANET_EF52564A-831D-42F2-B956-815CF0418C08/documents/D/DCF05C195EC32224343B4A425B35AB8AD.pdf
```



----------------------------------



`53331` -> long running
(`2139` in evil repo itself too: long running still, after the mutool updates, `2166`, `2144`, etc.)







##### Item ♯00075


-------------------------


```
43EA89C8D11BC2821EE1CEE0BD48F51F956E6
```



--> weird double text extracts: for SOME lines: same line produces two overlapping hOCR outputs which are slightly different too







##### Item ♯00076


Also note that SOME images are still in the hOCR output, despite this setting:


```
-x preserve-ligatures,preserve-whitespace,preserve-spans
```



so not request to `FZ_STEXT_PRESERVE_IMAGES` !

PLUS images are 1px high: these SHOULD ideally be consolidated into single (larger/higher) images.
(This 1px height images sequence construct has been observed in other PDFs as well.)







##### Item ♯00077


--------------------------------

Qiqqa fails/notes:


```
metadump -m 1 -o -  "G:\Qiqqa\evil\Guest\documents\2\2B5BAEDB61348943BE6A18621F3B9FBF299FB42A.pdf"
```













##### Item ♯00078





```
"Z:\lib\tooling\qiqqa\Qiqqa\bin\Debug\MuPDF\mutool.exe" metadump -m 1 -o -  "G:\Qiqqa\evil\Guest\documents\3\348417B141E4A3AEEDF8845E703A62BEFCB7B2DC.pdf"
```










##### Item ♯00079


DoS attack by logging a lot to stderr:


```
metadump -m 1 -o -  "G:\Qiqqa\evil\Guest\documents\3\3D8E405C60709DA3BCCCCA75B203FA37C441BEB.pdf"
```











##### Item ♯00080


Turns out metadump hangs when Qiqqa exists: the stdout feed does not signal the termination of the parent (Qiqqa) application and thus blocks forever on writing to stdout?
Or does it do something special? Since these took 1 CPU each:


```
"Z:\lib\tooling\qiqqa\Qiqqa\bin\Debug\MuPDF\mutool.exe" metadump -m 1 -o -  "G:\Qiqqa\evil\Guest\documents\6\6733C042A613BA992BA66A85AECB967A8CB1B9F0.pdf"

"Z:\lib\tooling\qiqqa\Qiqqa\bin\Debug\MuPDF\mutool.exe" metadump -m 1 -o -  "G:\Qiqqa\evil\Guest\documents\C\C8169F3C9BDFAD4B677D698F7096FED5AC4C413B.pdf"

"Z:\lib\tooling\qiqqa\Qiqqa\bin\Debug\MuPDF\mutool.exe" metadump -m 1 -o -  "G:\Qiqqa\evil\Guest\documents\C\C8169F3C9BDFAD4B677D698F7096FED5AC4C413B.pdf"
```











##### Item ♯00081


error 1 without further report ?!?!


```
metadump -m 1 -o -  "G:\Qiqqa\evil\Guest\documents\1\127447489B6D6EF9FFD58702657DA6456A42.pdf"
```











##### Item ♯00082





```
mudraw -o "G:/__bulktest/test.####.html" -r 100 -f -w 2400 -h 3600  -O preserve-ligatures,preserve-whitespace,preserve-spans,preserve-images  "W:/Sopkonijn/!OmniPage-input-dir/1C1C256136122FD38CA5D232DF3A2DB6BCAE7CBB.pdf"  2-12
```











##### Item ♯00083





```
draw -T0 -stmf -w 0 -h 0 -r 80 -o -  "G:\Qiqqa\evil\INTRANET_2B8E19DE-A7C5-4867-93E8-C3177957E09C\documents\4\4D14D229A3F84C891584332FF786BEBA8E25F1C.pdf" 7
```













##### Item ♯00084





```
--> C:\Users\Ger\AppData\Local\Temp/G%%Qiqqa%evil%INTRANET_2B8E19DE-A7C5-4867-93E8-C3177957E09C%documents%4%4D14D229A3F84C891584332FF786BEBA8E25F1C.pdf.accel
```













##### Item ♯00085





```
draw -T0 -stmf -w 0 -h 0 -r 80 -o -  "G:\Qiqqa\evil\INTRANET_2B8E19DE-A7C5-4867-93E8-C3177957E09C\documents\6\60835FB1D237D8F3ED73653CC9F935FDD7FA16B1.pdf" 1

draw -T0 -stmf -w 0 -h 0 -r 80 -o -  "G:\Qiqqa\evil\INTRANET_2B8E19DE-A7C5-4867-93E8-C3177957E09C\documents\4\4D14D229A3F84C891584332FF786BEBA8E25F1C.pdf" 7

draw -T0 -stmf -w 0 -h 0 -r 80 -o -  "G:\Qiqqa\evil\INTRANET_2B8E19DE-A7C5-4867-93E8-C3177957E09C\documents\6\60835FB1D237D8F3ED73653CC9F935FDD7FA16B1.pdf" 1

draw -T0 -stmf -w 0 -h 0 -r 80 -o -  "G:\Qiqqa\evil\INTRANET_2B8E19DE-A7C5-4867-93E8-C3177957E09C\documents\4\4D14D229A3F84C891584332FF786BEBA8E25F1C.pdf" 7

draw -T0 -stmf -w 0 -h 0 -r 80 -o -  "G:\Qiqqa\evil\INTRANET_2B8E19DE-A7C5-4867-93E8-C3177957E09C\documents\6\60835FB1D237D8F3ED73653CC9F935FDD7FA16B1.pdf" 1

draw -T0 -stmf -w 0 -h 0 -r 80 -o -  "G:\Qiqqa\evil\INTRANET_2B8E19DE-A7C5-4867-93E8-C3177957E09C\documents\4\4D14D229A3F84C891584332FF786BEBA8E25F1C.pdf" 8
```











##### Item ♯00086





```
mutool.exe draw -T0 -stmf -w 1432 -h 1853 -r 0 -o -  "G:\Qiqqa\evil\INTRANET_2B8E19DE-A7C5-4867-93E8-C3177957E09C\documents\6\6FA7489225ACDB9F802B3F1CD9FBB8FB9D1A76E3.pdf" 4
```











##### Item ♯00087





```
metadump -m 1 -o -  "G:\Qiqqa\evil\Guest\documents\1\121725C8CBB97A9F7C7B43336BA1DBC87EDA15C.pdf"
```











##### Item ♯00088





```
metadump -m 1 -o -  "G:\Qiqqa\evil\INTRANET_2B8E19DE-A7C5-4867-93E8-C3177957E09C\documents\E\EE10447C1B31E2BDFFCD36F2B3B7C2A5963C257.pdf"
```



-->
ZERO pages, but still exit code 0 instead of error report and exit 1+







##### Item ♯00089





```
metadump -m 1 -o -  "G:\Qiqqa\evil\INTRANET_2B8E19DE-A7C5-4867-93E8-C3177957E09C\documents\1\148174D1E91C25CD3CF77CA84675C7184669EC4.pdf"
```











##### Item ♯00090





```
metadump -m 1 -o -  "G:\Qiqqa\evil\INTRANET_2B8E19DE-A7C5-4867-93E8-C3177957E09C\documents\F\FEE4F241F6FA6397B723489A2B4B98C35B21B27.pdf"
```



-->







##### Item ♯00091





```
error: cannot recognize version marker: expected '%PDF-n.n', but reading '%PDF'
warning: trying to repair broken xref after encountering error: cannot recognize version marker: expected '%PDF-n.n', but reading '%PDF'
warning: repairing PDF document
error: invalid key in dict (tok = 9)
warning: ignoring object with invalid object number (0 0 R)
error: array not closed before end of file
--EXIT:1--
```











##### Item ♯00092





```
metadump -m 1 -o -  "G:\Qiqqa\evil\INTRANET_2B8E19DE-A7C5-4867-93E8-C3177957E09C\documents\6\6CED40AF808A4198B01D4A2959564EED92FD223C.pdf"
```



-->







##### Item ♯00093





```
warning: aes padding out of range
error: cannot authenticate password: G:\Qiqqa\evil\INTRANET_2B8E19DE-A7C5-4867-93E8-C3177957E09C\documents\6\6CED40AF808A4198B01D4A2959564EED92FD223C.pdf
--EXIT:1--
```











##### Item ♯00094





```
G:\Qiqqa\evil\INTRANET_2B8E19DE-A7C5-4867-93E8-C3177957E09C\documents\C\C7C55FA6CE5C263D46BB2D1C51476245BACE6BE7.pdf
```



--> no pagecount / corrupted doc







##### Item ♯00095





```
metadump -m 1 -o -  "G:\Qiqqa\evil\INTRANET_2B8E19DE-A7C5-4867-93E8-C3177957E09C\documents\B\BD23D4D2FC4C13CFAC627171C742BB384D71E9FE.pdf"
```



-->







##### Item ♯00096





```
error: object number out of range
warning: trying to repair broken xref after encountering error: object number out of range
warning: repairing PDF document
warning: ignoring object with invalid object number (0 0 R)
warning: expected 'endobj' or 'stream' keyword (47 0 R)
warning: ignoring zlib error: incorrect header check
error: corrupt object stream (48 0 R)
warning: ignoring broken object stream (48 0 R)
warning: ignoring zlib error: incorrect header check
error: corrupt object stream (49 0 R)
warning: ignoring broken object stream (49 0 R)
warning: ignoring zlib error: unknown compression method
error: corrupt object stream (50 0 R)
warning: ignoring broken object stream (50 0 R)
warning: ignoring zlib error: incorrect header check
error: corrupt object stream (51 0 R)
warning: ignoring broken object stream (51 0 R)
warning: ignoring zlib error: incorrect header check
error: corrupt object stream (52 0 R)
warning: ignoring broken object stream (52 0 R)
error: cannot authenticate password: G:\Qiqqa\evil\INTRANET_2B8E19DE-A7C5-4867-93E8-C3177957E09C\documents\B\BD23D4D2FC4C13CFAC627171C742BB384D71E9FE.pdf
--EXIT:1--
```











##### Item ♯00097





```
MUTOOL draw -o "J:/__bulktest/______/_/Y_/Qiqqa/Qiqqa-Test-DrvE/evil-base/__bulktest/TextExtractFiles-T1/Sample-PDFs-for-format-testing/testset misc 3 OCR/Theory of Thermionic Vacuum Tubes E. Chaffee (1933)/FULL-DOC-x300.png" -s mtf -r 300 -y l -T 3 -P -B 50 "Sample-PDFs-for-format-testing/testset misc 3 OCR/Theory of Thermionic Vacuum Tubes E. Chaffee (1933).pdf"
```



another crash thanks to heap corruption in JBig2Dec :-((

--> looks like JBig2Dec is NOT THREADSAFE as the other (single-threaded!) output format runs did NOT fail for this same file.







##### Item ♯00098





```
MUTOOL draw -o J:/__bulktest/______/_/Y_/Qiqqa/Qiqqa-Test-DrvE/evil-base/__bulktest/TextExtractFiles-T1/digitalcorpora.org/govdocs1/999/999891/FULL-DOC-x300.png -s mtf -r 300 -y l -T 3 -P -B 50 digitalcorpora.org/govdocs1/999/999891.pdf
```










##### Item ♯00099


CRASH: invalid heap access in LCMS2; have not observed JBIG2, so this looks, at first glance, to be an issue in conjuction with these warnings -- though DO NOTE that those warnings also showed up for other PDFs in the test before and those DID NOT cause a crash. Another conundrum, therefore:


```
warning: found duplicate fz_icc_link in the store
warning: found duplicate fz_icc_link in the store
warning: found duplicate fz_icc_link in the store
```











##### Item ♯00100





```
MUTOOL draw -o J:/__bulktest/______/_/Y_/Qiqqa/Qiqqa-Test-DrvE/evil-base/__bulktest/TextExtractFiles-T1/digitalcorpora.org/govdocs1/050/050600/FULL-DOC-x300.png -s mtf -r 300 -y l -T 3 -P -B 50 digitalcorpora.org/govdocs1/050/050600.pdf
```



- first file in the bulktest which uses JPX format. Nothing weird to report otherwise.







##### Item ♯00101


- second file found:


```
MUTOOL raster -F png -o J:/__bulktest/______/_/Y_/Qiqqa/Qiqqa-Test-DrvE/evil-base/__bulktest/TextExtractFiles-T1/digitalcorpora.org/govdocs1/039/039354/%04d.raster.png -s mt -r 150 -P digitalcorpora.org/govdocs1/039/039354.pdf
```











##### Item ♯00102





```
MUTOOL raster -F png -o J:/__bulktest/______/_/Y_/Qiqqa/Qiqqa-Test-DrvE/evil-base/__bulktest/TextExtractFiles-T1/artifex-mupdf-test-corpus/pdf/Jbig2_042_18/%04d.raster.png -s mt -r 150 -P artifex-mupdf-test-corpus/pdf/Jbig2_042_18.pdf
```



another JBIG2 using command, which triggers different debugger breakpoints in there. No crash yet, but DOES exercise different code paths inside JBIG2.
DID trigger a crash due to a bug in the tracking code I added to the refcounted allocations in there. (IBO pointer NULL access attempt)







##### Item ♯00103





```
MUTOOL draw -o J:/__bulktest/______/_/Y_/Qiqqa/Qiqqa-Test-DrvE/evil-base/__bulktest/TextExtractFiles-T1/digitalcorpora.org/govdocs1/031/031172/FULL-DOC-x300.png -s mtf -r 300 -y l -T 3 -P -B 50 digitalcorpora.org/govdocs1/031/031172.pdf
```



fails with a crash in fitz/colorspace after fz_find_icc_link() delivered an illegal pointer value.







##### Item ♯00104


Maybe relevant warnings? ::


```
page 1 file digitalcorpora.org/govdocs1/031/031172.pdf features:  color
warning: found duplicate fz_icc_link in the store
warning: found duplicate fz_icc_link in the store
warning: found duplicate fz_icc_link in the store
warning: ... repeated 28 times...
warning: found duplicate fz_image in the store
warning: ... repeated 27 times...
warning: found duplicate fz_image in the store
warning: found duplicate fz_icc_link in the store
```











##### Item ♯00105


Another crasher like that:


```
MUTOOL draw -o //?/J:/__bulktest/DATA/__bulktest/TextExtractFiles-T1/digitalcorpora.org/govdocs1/003/003737/FULL-DOC-x300.webp -s mtf -r 300 -y l -T 3 -P -B 50 digitalcorpora.org/govdocs1/003/003737.pdf
```











##### Item ♯00106





```
MUTOOL raster -F png -o //?/J:/__bulktest/DATA/__bulktest/TextExtractFiles-T1/artifex-mupdf-test-corpus/pdf/Jbig2_042_24/%04d.raster.png -s mt -r 150 -P artifex-mupdf-test-corpus/pdf/Jbig2_042_24.pdf
```



--> JBIG2 memleak; at least assertion failures at cleanup time.







##### Item ♯00107





```
MUTOOL extract -o "//?/J:/__bulktest/DATA/__bulktest/TextExtractFiles-T1/Sample-PDFs-for-format-testing/testset misc 4/E3C5755DB55A9F878E6429C59F8E40D574429E3C/FULL-DOC.extract.dump" -r "Sample-PDFs-for-format-testing/testset misc 4/E3C5755DB55A9F878E6429C59F8E40D574429E3C.pdf"
```



--> corrupted refcount in JBIG2 reported, while accessing already-freed memory @ dict->glyphs[i=50]







##### Item ♯00108





```
MUTOOL draw -o //?/J:/__bulktest/DATA/__bulktest/TextExtractFiles-T1/Sample-PDFs-for-format-testing/pdfium/testing/resources/goto_action/FULL-DOC-x300.png -s mtf -r 300 -y l -T 3 -P -B 50 Sample-PDFs-for-format-testing/pdfium/testing/resources/goto_action.pdf
```



--> hard crash in pdf_load_link thanks to uninitialized `uri` pointer.







##### Item ♯00109





```
MUTOOL draw -o //?/J:/__bulktest/DATA/__bulktest/TextExtractFiles-T1/digitalcorpora.org/govdocs1/999/999053/FULL-DOC-x300.png -s mtf -r 300 -y l -T 3 -P -B 50 digitalcorpora.org/govdocs1/999/999053.pdf
```



--> hard crash / assertion failure on Usee-After-Free assertion check in ICC code.

@!#$%^&*( BLAST! Slightest code edit and problem disappears, then never returns. Timing-related, hence another multi-threading issue. This time (since we fixes JBig2) I suspect fz_storable, as it's at that level that the 0xDDDDDDDD invalid memory pointer (`link` variable returned from fz_new_icc_link()) is produced: LCMS2MT also has its own internal ref counting and locking, but that is deeper into that struct, so it's fz_storable that's suspect: `link` is a non-NULL pointer, which happens to be FREED DATA. Occurs in a worker thread, so that corroborates the thread-unsafety analysis thus far. Shite. :-(((







##### Item ♯00110





```
"INTRANET_E57B9774-4712-430E-93E0-E67433F7DF07/documents/1/13D1943F462ACBAA5B1C9B883B4FD2742B996F1.pdf"  "INTRANET_E57B9774-4712-430E-93E0-E67433F7DF07/documents/1"    "13D1943F462ACBAA5B1C9B883B4FD2742B996F1.pdf"      "13D1943F462ACBAA5B1C9B883B4FD2742B996F1"      "./"
```



--> dumps 3 TTF files and a metric ton of images, which look like they're all small text chunks that make up the page in this PDF. No errors per see, but a weird way of doing this and certainly something to look into once we get to optimizing our PDF render output for HTML/etc. with a minimum number of (MERGED) images.

This is different from the single-pixel image stripes I've seen many times before and would therefor serve as a good testcase for any image bbox merging code that we may introduce to optimize mixed text+image outputs!







##### Item ♯00111





```
"INTRANET_E57B9774-4712-430E-93E0-E67433F7DF07/documents/1/17B556D9C5585C798D5C29922FCF03899E5221E.pdf"  "INTRANET_E57B9774-4712-430E-93E0-E67433F7DF07/documents/1"    "17B556D9C5585C798D5C29922FCF03899E5221E.pdf"      "17B556D9C5585C798D5C29922FCF03899E5221E"      "./"
```



--> No errors per se, but another good test sample for our purposes: this one has SCANNED IMAGES (one per page?) where you can see the scan/copier artifacts too. I expect this one doesn't come with a text overlay (TO BE CHECKED) and thus would serve well for testing our 'has proper text embedded' check code!







##### Item ♯00112


Another candidate like that (this is an otherwise clean electronics datasheet, no copier artifacts):


```
"INTRANET_E57B9774-4712-430E-93E0-E67433F7DF07/documents/3/396E7C1B38A2268EA1082453FFAC5D6778DEB6.pdf"  "INTRANET_E57B9774-4712-430E-93E0-E67433F7DF07/documents/3"    "396E7C1B38A2268EA1082453FFAC5D6778DEB6.pdf"      "396E7C1B38A2268EA1082453FFAC5D6778DEB6"      "./"
```











##### Item ♯00113


Another good one for testing: the old 405 "current dumping amplifier" instruction booklet:


```
"INTRANET_E57B9774-4712-430E-93E0-E67433F7DF07/documents/4/4CFF7BE82F8A4A6AC84DA26FD7DDBF815777DD4.pdf"  "INTRANET_E57B9774-4712-430E-93E0-E67433F7DF07/documents/4"    "4CFF7BE82F8A4A6AC84DA26FD7DDBF815777DD4.pdf"      "4CFF7BE82F8A4A6AC84DA26FD7DDBF815777DD4"      "./"
```











##### Item ♯00114


For detecting and processing extensive MATH blocks in the extract:


```
"INTRANET_E57B9774-4712-430E-93E0-E67433F7DF07/documents/4/47B017728D26B8D23891D2DC2184D1122C4369E.pdf"  "INTRANET_E57B9774-4712-430E-93E0-E67433F7DF07/documents/4"    "47B017728D26B8D23891D2DC2184D1122C4369E.pdf"      "47B017728D26B8D23891D2DC2184D1122C4369E"      "./"
```











##### Item ♯00115


Ueber-slow and memleaking? (Or is it due to bulktest collecting memleaks by running all the shoddy PDFs through the system all at once? ...):


```
MUTOOL draw -o //?/J:/__bulktest/DATA/__bulktest/TextExtractFiles-T1/Sample-PDFs-for-format-testing/ICJ-CIJ/57-9421/FULL-DOC-x300.webp -s mtf -r 300 -y l -T 3 -P -B 50 Sample-PDFs-for-format-testing/ICJ-CIJ/57-9421.pdf
```



--> at least page render takes a solid 10 seconds per page, so that needs cleaning up.

Oh, and this bugger is eating GIGABYTES (8 by now, halfway through) as seen in the deebugger system summary view!







##### Item ♯00116


---

Saw a lcms ICC failure zip by, which was followed up by a long-running, silent, work period (mutool trace or mutool trace, I believe). Anyway, this was looking at a running logfile, which had been going on for about 12 hours already. The idea is to `grep` the bulktest logfiles for error reports like that + long-running tasks, so grep mutool commands, long-run report lines and error lines, all in a single `grep`, thus producing a reduced report set that we can further analyze to extract the most interesting buggers.

NOT RELATED: check the mupdf fmt_obj() code, which employs the fmt_print functionality: there we can run in error-triggering situations (bad XML parses, etc.) which are 'caught' in the internal fmt() error handling code, producing invalid PDFs. HOWEVER, these errors fly by unreported to the outside world, so you have a few options for silent recursion.

Ideas:

1. communicate errors back through the `struct fmt`? Or can we propagate those exceptions? `fmt_obj()` is a static/local, so it always comes wrapped...

2. add mutool commands' options to have these errors reported or silently ignored -- in the latter case we'll have to doublecheck our generated PostScript and probably track the produced ( braces in certain parts of fmt/print...


```
MUTOOL clean -gggg -D -c -s -AA digitalcorpora.org/govdocs1/999/999576.pdf //?/J:/__bulktest/DATA/__bulktest/TextExtractFiles-T1/digitalcorpora.org/govdocs1/999/999576/FULL-DOC.clean.pdf
```



sounded like one of the candidates for this (slow exec + ICC failure stuff), but I'm not sure.







##### Item ♯00117





```
MUTOOL draw -o //?/J:/__bulktest/DATA/__bulktest/TextExtractFiles-T1/Sample-PDFs-for-format-testing/ICJ-CIJ/150-17490/FULL-DOC-x300.png -s mtf -r 300 -y l -T 3 -P -B 50 Sample-PDFs-for-format-testing/ICJ-CIJ/150-17490.pdf
```



at least is a slow renderer, taking ~6s or worse per page.







##### Item ♯00118





```
MUTOOL draw -o //?/J:/__bulktest/DATA/__bulktest/TextExtractFiles-T1/Sample-PDFs-for-format-testing/ICJ-CIJ/133-18550/FULL-DOC-x300.webp -s mtf -r 300 -y l -T 3 -P -B 50 Sample-PDFs-for-format-testing/ICJ-CIJ/133-18550.pdf
```



another odd lock-up (long-running page render, probably page 43 (remeber that we *buffer* our diagnostics logging, so another message may be still pending in the internal buffer! Might then be page 44 or upwards!)

rest of its pages go at ~6s (or worse) so certainly a performance cracker.


... meanwhile, the Summary View shows some PDF(s) have been able to require about 1GB of RAM to process during this run thus far. Do we report memory consumption for every PDF in bulktest runs? (I don't think so! At least not in a clean/usable fashion.)


BTW: performance on the Ryzen 3700X is about 7500 PDF files per 600 minutes (but be very aware that not all were rendered! this is a 11% sampling rate run!)




----

Make PDF writes (and probably other formats too) SANITY-SAFE a la WinRAR: only produce the output file when it's actually correct & complete: WinRAR does this by writing to a `.tmp` file in the same directory, then, when all is done and no errors occurred, RENAME that file to the target. The behaviour on error is to DELETE the `.tmp` file, but we might want to reconsider that last behaviour: some times it's very handy to have the *broken* output available for diagnostics...

----







##### Item ♯00119





```
Sample-PDFs-for-format-testing/testset misc 4/A377F2BEFE9575BD1622375433BF34FF7A518.pdf
```



--> no problems, but does force resize of the fz_store hash table at least twice, so this one will help exercising different code paths and execution timing mixes while we hunt for the elusive fz_store_item() -> existing != NULL path' crash due to invalidated (FREED) memory referenced by existing->val shortly after the lock is released.  :-S







##### Item ♯00120





```
MUTOOL metadump -o //?/J:/__bulktest/DATA/__bulktest/TextExtractFiles-T1/pdfium_tests/fx/FRC_8.2.2_part1/FRC_8_8.2.2_Parent_edit/FULL-DOC.extract.meta.json -m 2 -i p pdfium_tests/fx/FRC_8.2.2_part1/FRC_8_8.2.2_Parent_edit.pdf
```



error: syntax error in attributes







##### Item ♯00121


--> throws an exception in the XML parse. Should this be recoverable?

B0rks on this:


```
<?xpacket begin="" id="W5M0MpCehiHzreSzNTczkc9d"?>
<x:xmpmeta xmlns:x="adobe:ns:meta/">
   <rdf:RDF xmlns:rdf="http://www.w3.org/1999/02/22-rdf-syntax-ns#">
      <rdf:Description rdf:about=""
         xmlns:xmp="http://ns.adobe.com/xap/1.0/"
         xmlns:dc="http://purl.org/dc/elements/1.1/"
         xmlns:pdf="http://ns.adobe.com/pdf/1.3/"
         <dc:creator>
            <rdf:Seq>
               <rdf:li></rdf:li>
            </rdf:Seq>
         </dc:creator>
         <dc:title>
            <rdf:Alt>
               <rdf:li xml:lang="x-default"></rdf:li>
            </rdf:Alt>
         </dc:title>
         <dc:description>
           <rdf:Alt>
               <rdf:li xml:lang="x-default"></rdf:li>
           </rdf:Alt>
         </dc:description>
         <dc:subject>
            <rdf:Bag>
                <rdf:li></rdf:li>
            </rdf:Bag>
         </dc:subject>
         <xmp:CreatorTool></xmp:CreatorTool>
         <pdf:Producer>Foxit PhantomPDF Printer Version 7.1.0.112</pdf:Producer>
         <pdf:Keywords></pdf:Keywords>
         <xmp:CreateDate>2015-04-07T15:29:04+08:00</xmp:CreateDate>
         <xmp:ModifyDate>2015-04-27T15:41:07+08:00</xmp:ModifyDate>
      </rdf:Description>
   </rdf:RDF>
</x:xmpmeta>
<?xpacket end="w"?>
```



note the booger at the end of the `<rdf:Description` tag: no terminating '>'!  This type of error should be easily fixable...







##### Item ♯00122


... and another one:


```
MUTOOL metadump -o //?/J:/__bulktest/DATA/__bulktest/TextExtractFiles-T1/digitalcorpora.org/govdocs1/051/051192/FULL-DOC.extract.meta.json -m 2 -i p digitalcorpora.org/govdocs1/051/051192.pdf
```













##### Item ♯00123





```
error: syntax error after attribute name
```










##### Item ♯00124


B0rk b0rk b0rk (yes, lots of whitespace at the end there!):


```
<?xpacket begin='Ã¯Â»Â¿' id='W5M0MpCehiHzreSzNTczkc9d'?>
<?adobe-xap-filters esc="CRLF"?>
<x:xmpmeta xmlns:x='adobe:ns:meta/' x:xmptk='XMP toolkit 2.9.1-13, framework 1.6'>
<rdf:RDF xmlns:rdf='http://www.w3.org/1999/02/22-rdf-syntax-ns#' xmlns:iX='http://ns.adobe.com/iX/1.0/'>
<rdf:Description rdf:about='uuid:96099729-c0ba-47ce-92af-a84af3b2f73d' xmlns:pdf='http://ns.adobe.com/pdf/1.3/' pdf:Producer='Acrobat Distiller 6.0 (Windows)'></rdf:Description>
<rdf:Description rdf:about='uuid:96099729-c0ba-47ce-92af-a84af3b2f73d' xmlns:pdfx='http://ns.adobe.com/pdfx/1.3/' pdfx:Company='Economia' pdfx:LocalÃ¢â€ â€š0020Office='   ' pdfx:DocÃ¢â€ â€š0020No.='BSE Article 1128-LGG-JCT.doc' pdfx:OrigÃ¢â€ â€š0020DocÃ¢â€ â€š0020Path='   ' pdfx:DocÃ¢â€ â€š0020Name='BSE Article 1128-LGG-JCT.doc' pdfx:AddresseeÃ¢â€ â€š0028sÃ¢â€ â€š0029='   ' pdfx:SignerÃ¢â€ â€š0028sÃ¢â€ â€š0029='   ' pdfx:CauseÃ¢â€ â€š0020No.='   ' pdfx:Parties='   ' pdfx:ClientÃ¢â€ â€š0020No.='   ' pdfx:MatterÃ¢â€ â€š0020No.='   ' pdfx:ClientÃ¢â€ â€š0020Name='   ' pdfx:MatterÃ¢â€ â€š0020Name='   ' pdfx:CaptionÃ¢â€ â€š0020BankÃ¢â€ â€š0020Document='   ' pdfx:CaptionÃ¢â€ â€š0020ClientÃ¢â€ â€š0020Name='   ' pdfx:CaptionÃ¢â€ â€š0020OppÃ¢â€ â€š0020CounselÃ¢â€ â€š0020ClientÃ¢â€ â€š0020Name='   ' pdfx:CaptionÃ¢â€ â€š0020AttorneysÃ¢â€ â€š0020for='   ' pdfx:CaptionÃ¢â€ â€š0020OppÃ¢â€ â€š0020CounselÃ¢â€ â€š0020for='   ' pdfx:DocumentÃ¢â€ â€š0020ManagementÃ¢â€ â€š0020Library='   ' pdfx:RecipientÃ¢â€ â€š0020Array='   ' pdfx:DocType='   ' pdfx:SourceModified='D:20070301175722'><pdfx:DocÃ¢â€ â€š0020Path>C:\Documents and Settings\beckers\Local Settings\Temporary Internet Files\OLK2A</pdfx:DocÃ¢â€ â€š0020Path></rdf:Description>
<rdf:Description rdf:about='uuid:96099729-c0ba-47ce-92af-a84af3b2f73d' xmlns:photoshop='http://ns.adobe.com/photoshop/1.0/'><photoshop:headline><rdf:Seq><rdf:li></rdf:li></rdf:Seq></photoshop:headline></rdf:Description>
<rdf:Description rdf:about='uuid:96099729-c0ba-47ce-92af-a84af3b2f73d' xmlns:xap='http://ns.adobe.com/xap/1.0/' xap:CreatorTool='Acrobat PDFMaker 6.0 for Word' xap:ModifyDate='2007-03-01T13:47:53-06:00' xap:CreateDate='2007-03-01T12:00:31-06:00' xap:MetadataDate='2007-03-01T13:47:53-06:00'></rdf:Description>
<rdf:Description rdf:about='uuid:96099729-c0ba-47ce-92af-a84af3b2f73d' xmlns:xapMM='http://ns.adobe.com/xap/1.0/mm/' xapMM:DocumentID='uuid:94ee14f5-8b55-4e3a-af3f-497bc9863af8'><xapMM:VersionID><rdf:Seq><rdf:li>5</rdf:li></rdf:Seq></xapMM:VersionID></rdf:Description>
<rdf:Description rdf:about='uuid:96099729-c0ba-47ce-92af-a84af3b2f73d' xmlns:dc='http://purl.org/dc/elements/1.1/' dc:format='application/pdf'><dc:title><rdf:Alt><rdf:li xml:lang='x-default'>1128 Mexico</rdf:li></rdf:Alt></dc:title><dc:creator><rdf:Seq><rdf:li>LGG</rdf:li></rdf:Seq></dc:creator><dc:subject><rdf:Seq><rdf:li></rdf:li></rdf:Seq></dc:subject></rdf:Description>
</rdf:RDF>
</x:xmpmeta>





















<?xpacket end='w'?>
```



... which sounds like this one could use some `iconv` love at least, before we clean up those attribute names!







##### Item ♯00125


## THOUGHT:

After working an evening on some Metadata (XML) recovery code in `mupdf` core, I believe this is the WRONG TACTIC for this problem:

1) it takes a long time to get stuff done as the test/debug round trip is relatively slow, due to having to run the PDFs through the correct commands to trigger recovery, etc.
2) we limit ourselves to recovery code in C, using the xml parser (which fails) and the now-used gumbo html parser as a second attempt -- which turns out to be worthless as it expects HTML and nothing else, so it is pretty useless parsing semi-b0rked or fully-b0rked XMP records (XML).

Now we could persist and find ourselves the most lenient XML parser in the world and some very smart recovery codes, but here's a thought:

> Isn't this much more suitable to SCRIPTED processing? Where userland scripts can be created and used to fiddle to your heart's content?

**YES!**

Question is then: do we do this through an external process invoked through a callback, or via QuickJS and specifying user script to run or ...?

All those sound really nice, until you look at reality: https://jsonformatter.org/xml-formatter had no problem cleaning it up for us and that might not be batchable, but is surely very useful to any user anyway.

Which leads us to a meta-question: what can we do best to have the same tools suitable for batch and incident edits, including manual user operations like this copy&pasting to an external website?

--> while I was initially thinking in the direction of callbacks, i.e. staying in the current exec/run, now I believe the better and faster way forward is this:

allow the repair/recovery process to take arbitrary time and be executed at arbitrary times, hence:

- have the ability to at least detect these errors/issues in a batch or single run.
- use that signal (or do this as part of that signaling): ability to dump the raw data with sufficient markers so we can process it any way we want ...
- ... and then have a second tool (or batch run) import/replace those buggered bits with the corrected data provided by the external process (which might be anything, including manual actions such as copy/paste to/from websites, editors, ...)

Hence we must have tools with the ability to EXPORT these erroneous chunks ...
... and have the ability to IMPORT replacement data from files, preferably produced during previous EXPORT so users / external processes only need to bother with replacing the bad stuff for some good stuff.


^^^^^^ That means we can keep our tools relatively simple and don't need elaborate recovery schemes in the core tools themselves as everything is reduced to EXPORT + IMPORT/REPLACE (when available).

> Then the whole correction process becomes external activity that can be dealt with any way we like!






### Stack Overflow!

Wow! Never expected this to happen this late in the game! But we've got another **winner**:

- `INTRANET_E57B9774-4712-430E-93E0-E67433F7DF07/documents/7/7AD86BF80591DAD1DDE8E81EB6C55B41E5E4.pdf`


```
:L#00191: MUTOOL info -o //?/J:/__bulktest/DATA/__bulktest/TextExtractFiles-T1/INTRANET_E57B9774-4712-430E-93E0-E67433F7DF07/documents/7/7AD86BF80591DAD1DDE8E81EB6C5Exception thrown at 0x00007FFCF4528388 (MuPDFLib.dll) in bulktest.exe: 0xC00000FD: Stack overflow (parameters: 0x0000000000000001, 0x000000968CA03FE8).
Unhandled exception at 0x00007FFCF4528388 (MuPDFLib.dll) in bulktest.exe: 0xC00000FD: Stack overflow (parameters: 0x0000000000000001, 0x000000968CA03FE8).
```



**CRASH**







##### Item ♯00126


BTW, this is what came just before that, just in case...:


```
OK: MUTOOL command: MUTOOL extract -o //?/J:/__bulktest/DATA/__bulktest/TextExtractFiles-T1/INTRANET_E57B9774-4712-430E-93E0-E67433F7DF07/documents/7/7AD86BF80591DAD1DDE8E81EB6C55B41E5E4/FULL-DOC.extract.dump -r INTRANET_E57B9774-4712-430E-93E0-E67433F7DF07/documents/7/7AD86BF80591DAD1DDE8E81EB6C55B41E5E4.pdf
>L#00189> T:427ms USED:3.23Mb OK MUTOOL extract -o //?/J:/__bulktest/DATA/__bulktest/TextExtractFiles-T1/INTRANET_E57B9774-4712-430E-93E0-E67433F7DF07/documents/7/7AD86BF80591DAD1DDE8E81EB6C55B41E5E4/FULL-DOC.extract.dump -r INTRANET_E57B9774-4712-430E-93E0-E67433F7DF07/documents/7/7AD86BF80591DAD1DDE8E81EB6C55B41E5E4.pdf
```











##### Item ♯00127


Stackdump in the debugger at the moment of crash:


```
MuPDFLib.dll!pdf_cache_object(fz_context * ctx, pdf_document * doc, int num) Line 2306	C
 	MuPDFLib.dll!pdf_resolve_indirect(fz_context * ctx, pdf_obj * ref) Line 2437	C
 	MuPDFLib.dll!pdf_resolve_indirect_chain(fz_context * ctx, pdf_obj * ref) Line 2464	C
 	MuPDFLib.dll!pdf_dict_get(fz_context * ctx, pdf_obj * obj, pdf_obj * key) Line 2048	C
 	MuPDFLib.dll!pdf_lookup_page_loc_imp(fz_context * ctx, pdf_document * doc, pdf_obj * node, int * skip, pdf_obj * * parentp, int * indexp) Line 170	C
 	MuPDFLib.dll!pdf_lookup_page_loc(fz_context * ctx, pdf_document * doc, int needle, pdf_obj * * parentp, int * indexp) Line 345	C
 	MuPDFLib.dll!pdf_lookup_page_obj(fz_context * ctx, pdf_document * doc, int needle) Line 361	C
 	MuPDFLib.dll!gatherresourceinfo(fz_context * ctx, pdf_mark_list * mark_list, globals * glo, int page, pdf_obj * rsrc, int show) Line 735	C
 	MuPDFLib.dll!gatherresourceinfo(fz_context * ctx, pdf_mark_list * mark_list, globals * glo, int page, pdf_obj * rsrc, int show) Line 779	C
 	MuPDFLib.dll!gatherresourceinfo(fz_context * ctx, pdf_mark_list * mark_list, globals * glo, int page, pdf_obj * rsrc, int show) Line 779	C
 	MuPDFLib.dll!gatherresourceinfo(fz_context * ctx, pdf_mark_list * mark_list, globals * glo, int page, pdf_obj * rsrc, int show) Line 779	C
 	MuPDFLib.dll!gatherresourceinfo(fz_context * ctx, pdf_mark_list * mark_list, globals * glo, int page, pdf_obj * rsrc, int show) Line 779	C
 	MuPDFLib.dll!gatherresourceinfo(fz_context * ctx, pdf_mark_list * mark_list, globals * glo, int page, pdf_obj * rsrc, int show) Line 779	C
 	MuPDFLib.dll!gatherresourceinfo(fz_context * ctx, pdf_mark_list * mark_list, globals * glo, int page, pdf_obj * rsrc, int show) Line 779	C
 	MuPDFLib.dll!gatherresourceinfo(fz_context * ctx, pdf_mark_list * mark_list, globals * glo, int page, pdf_obj * rsrc, int show) Line 779	C
 	MuPDFLib.dll!gatherresourceinfo(fz_context * ctx, pdf_mark_list * mark_list, globals * glo, int page, pdf_obj * rsrc, int show) Line 779	C

[... repeats 4980 times!! ...]

	MuPDFLib.dll!gatherresourceinfo(fz_context * ctx, pdf_mark_list * mark_list, globals * glo, int page, pdf_obj * rsrc, int show) Line 779	C
	MuPDFLib.dll!gatherresourceinfo(fz_context * ctx, pdf_mark_list * mark_list, globals * glo, int page, pdf_obj * rsrc, int show) Line 779	C
 	MuPDFLib.dll!gatherresourceinfo(fz_context * ctx, pdf_mark_list * mark_list, globals * glo, int page, pdf_obj * rsrc, int show) Line 779	C
 	The maximum number of stack frames supported by Visual Studio has been exceeded.
```










##### Item ♯00128


and it occurs in this bit of code:


```
if (show & (IMAGES|XOBJS) && xobj)
	{
		int n;

		if (show & IMAGES)
			gatherimages(ctx, glo, page, pageref, xobj);
		if (show & XOBJS)
		{
			gatherforms(ctx, glo, page, pageref, xobj);
			gatherpsobjs(ctx, glo, page, pageref, xobj);
		}
		n = pdf_dict_len(ctx, xobj);
		for (i = 0; i < n; i++)
		{
			pdf_obj *obj = pdf_dict_get_val(ctx, xobj, i);
			subrsrc = pdf_dict_get(ctx, obj, PDF_NAME(Resources));
			if (subrsrc && pdf_objcmp(ctx, rsrc, subrsrc))
				gatherresourceinfo(ctx, mark_list, glo, page, subrsrc, show);
		}
	}
```



where loop counter `i` is reported as being `1` in `pdfinfo.c` ~ line 778 (at the time of writing: Sep 2022).







##### Item ♯00129


... Well, at least *this bug* is reproducible (after we've reduced the test set data file):


```
:L#00191: MUTOOL info -o //?/J:/__bulktest/DATA/__bulktest/TextExtractFiles-T1/INTRANET_E57B9774-4712-430E-93E0-E67433F7DF07/documents/7/7AD86BF80591DAD1DDE8E81EB6C5Exception thrown at 0x00007FFCF44AC724 (MuPDFLib.dll) in bulktest.exe: 0xC00000FD: Stack overflow (parameters: 0x0000000000000001, 0x000000A247A03FF0).
Unhandled exception at 0x00007FFCF44AC724 (MuPDFLib.dll) in bulktest.exe: 0xC00000FD: Stack overflow (parameters: 0x0000000000000001, 0x000000A247A03FF0).

The program '[26524] bulktest.exe' has exited with code 0 (0x0).
```











##### Item ♯00130





```
"digitalcorpora.org/govdocs1/011/011591.pdf"  "digitalcorpora.org/govdocs1/011"    "011591.pdf"      "011591"      "./"
```













##### Item ♯00131





```
MUTOOL draw -o //?/J:/__bulktest/DATA/__bulktest/TextExtractFiles-T1/digitalcorpora.org/govdocs1/011/011591/FULL-DOC-x300.png -s mtf -r 300 -y l -P -B 50 digitalcorpora.org/govdocs1/011/011591.pdf
Output format: png (DeviceRGB)
```



--> superslow render?!  Is already stuck for a while and no progress visible on screen   :-S







##### Item ♯00132





```
:L#00160: MUTOOL draw -o //?/J:/__bulktest/DATA/__bulktest/TextExtractFiles-T1/artifex-mupdf-test-corpus/pdf/Jbig2_042_14/FULL-DOC-x300.webp -s mtf -r 300 -y l -P -B 50 artifex-mupdf-test-corpus/pdf/Jbig2_042_14.pdf
Output format: png (DeviceRGB)
warning: expected object number (tok = 21)
page 1 file artifex-mupdf-test-corpus/pdf/Jbig2_042_14.pdf features:  grayscale
error: jbig2dec error: invalid number of symbols in aggregate glyph (segment 0x03)
error: jbig2dec error: invalid number of symbols in aggregate glyph (segment 0x03)
warning: jbig2dec warning: failed to decode; treating as end of file (segment 0x03)
error: jbig2dec error: invalid number of symbols in aggregate glyph (segment 0x03)
warning: jbig2dec warning: failed to decode; treating as end of file (segment 0x03)
error: jbig2dec error: invalid number of symbols in aggregate glyph (segment 0x03)
error: cannot decode jbig2 image
error: jbig2dec error: invalid number of symbols in aggregate glyph (segment 0x03)
error: cannot decode jbig2 image
warning: read error; treating as end of file
warning: read error; treating as end of file
warning: padding truncated image
error: jbig2dec error: invalid number of symbols in aggregate glyph (segment 0x03)
warning: padding truncated image
warning: jbig2dec warning: failed to decode; treating as end of file (segment 0x03)
error: cannot decode jbig2 image
warning: read error; treating as end of file
warning: padding truncated image
warning: jbig2dec warning: failed to decode; treating as end of file (segment 0x03)
error: cannot decode jbig2 image
error: jbig2dec error: invalid number of symbols in aggregate glyph (segment 0x03)
warning: read error; treating as end of file
error: jbig2dec error: invalid number of symbols in aggregate glyph (segment 0x03)
error: jbig2dec error: invalid number of symbols in aggregate glyph (segment 0x03)
error: jbig2dec error: invalid number of symbols in aggregate glyph (segment 0x03)
error: jbig2dec error: invalid number of symbols in aggregate glyph (segment 0x03)
warning: padding truncated image
error: jbig2dec error: invalid number of symbols in aggregate glyph (segment 0x03)
warning: jbig2dec warning: failed to decode; treating as end of file (segment 0x03)
warning: jbig2dec warning: failed to decode; treating as end of file (segment 0x03)
error: jbig2dec error: invalid number of symbols in aggregate glyph (segment 0x03)
error: cannot decode jbig2 image
error: cannot decode jbig2 image
warning: read error; treating as end of file
error: jbig2dec error: invalid number of symbols in aggregate glyph (segment 0x03)
warning: read error; treating as end of file
warning: padding truncated image
error: jbig2dec error: invalid number of symbols in aggregate glyph (segment 0x03)
warning: padding truncated imagSoft Assertion failed: i < ctx->current_page --> Z:\lib\tooling\qiqqa\MuPDF\thirdparty\jbig2dec\jbig2.c::519
Soft Assertion failed: i < ctx->current_page --> Z:\lib\tooling\qiqqa\MuPDF\thirdparty\jbig2dec\jbig2.c::519
```



--> nothing major, but here we hit an assertion that checks the OLD assumption of that loop, which we now have coded to span the entire allocated array, so no harm done anymore!







##### Item ♯00133


Crap! Another serious stack overflow!


```
:L#00160: MUTOOL draw -o //?/J:/__bulktest/DATA/__bulktest/TextExtractFiles-T1/pdfium_tests/fx/layer/4_36/FULL-DOC-x300.webp -s mtf -r 300 -y l -P -B 50 pdfium_tests/fx/layer/4_36.pdf
Output format: png (DeviceRGB)
error: cannot find startxref
Exception thrown at 0x00007FFCF45287C6 (MuPDFLib.dll) in bulktest.exe: 0xC00000FD: Stack overflow (parameters: 0x0000000000000001, 0x0000003373503FA0).
Unhandled exception at 0x00007FFCF45287C6 (MuPDFLib.dll) in bulktest.exe: 0xC00000FD: Stack overflow (parameters: 0x0000000000000001, 0x0000003373503FA0).
```










##### Item ♯00134


--> stacktrace at the moment of B0RK:


```
MuPDFLib.dll!pdf_cache_object(fz_context * ctx, pdf_document * doc, int num) Line 2302
	at Z:\lib\tooling\qiqqa\MuPDF\source\pdf\pdf-xref.c(2302)
MuPDFLib.dll!pdf_resolve_indirect(fz_context * ctx, pdf_obj * ref) Line 2437
	at Z:\lib\tooling\qiqqa\MuPDF\source\pdf\pdf-xref.c(2437)
MuPDFLib.dll!pdf_resolve_indirect_chain(fz_context * ctx, pdf_obj * ref) Line 2464
	at Z:\lib\tooling\qiqqa\MuPDF\source\pdf\pdf-xref.c(2464)
MuPDFLib.dll!pdf_is_name(fz_context * ctx, pdf_obj * obj) Line 336
	at Z:\lib\tooling\qiqqa\MuPDF\source\pdf\pdf-object.c(336)
MuPDFLib.dll!pdf_is_ocg_hidden_imp(fz_context * ctx, pdf_document * doc, pdf_obj * rdb, const char * usage, pdf_obj * ocg, pdf_cycle_list * cycle_up) Line 641
	at Z:\lib\tooling\qiqqa\MuPDF\source\pdf\pdf-layer.c(641)
MuPDFLib.dll!pdf_is_ocg_hidden(fz_context * ctx, pdf_document * doc, pdf_obj * rdb, const char * usage, pdf_obj * ocg) Line 797
	at Z:\lib\tooling\qiqqa\MuPDF\source\pdf\pdf-layer.c(797)
MuPDFLib.dll!calc_ve_state(fz_context * ctx, pdf_document * doc, pdf_obj * rdb, const char * usage, pdf_obj * ve) Line 580
	at Z:\lib\tooling\qiqqa\MuPDF\source\pdf\pdf-layer.c(580)
MuPDFLib.dll!calc_ve_state(fz_context * ctx, pdf_document * doc, pdf_obj * rdb, const char * usage, pdf_obj * ve) Line 602
	at Z:\lib\tooling\qiqqa\MuPDF\source\pdf\pdf-layer.c(602)
MuPDFLib.dll!pdf_is_ocg_hidden_imp(fz_context * ctx, pdf_document * doc, pdf_obj * rdb, const char * usage, pdf_obj * ocg, pdf_cycle_list * cycle_up) Line 742
	at Z:\lib\tooling\qiqqa\MuPDF\source\pdf\pdf-layer.c(742)
MuPDFLib.dll!pdf_is_ocg_hidden(fz_context * ctx, pdf_document * doc, pdf_obj * rdb, const char * usage, pdf_obj * ocg) Line 797
	at Z:\lib\tooling\qiqqa\MuPDF\source\pdf\pdf-layer.c(797)
MuPDFLib.dll!calc_ve_state(fz_context * ctx, pdf_document * doc, pdf_obj * rdb, const char * usage, pdf_obj * ve) Line 580
	at Z:\lib\tooling\qiqqa\MuPDF\source\pdf\pdf-layer.c(580)
MuPDFLib.dll!calc_ve_state(fz_context * ctx, pdf_document * doc, pdf_obj * rdb, const char * usage, pdf_obj * ve) Line 602
	at Z:\lib\tooling\qiqqa\MuPDF\source\pdf\pdf-layer.c(602)
MuPDFLib.dll!pdf_is_ocg_hidden_imp(fz_context * ctx, pdf_document * doc, pdf_obj * rdb, const char * usage, pdf_obj * ocg, pdf_cycle_list * cycle_up) Line 742
	at Z:\lib\tooling\qiqqa\MuPDF\source\pdf\pdf-layer.c(742)
MuPDFLib.dll!pdf_is_ocg_hidden(fz_context * ctx, pdf_document * doc, pdf_obj * rdb, const char * usage, pdf_obj * ocg) Line 797
	at Z:\lib\tooling\qiqqa\MuPDF\source\pdf\pdf-layer.c(797)
MuPDFLib.dll!calc_ve_state(fz_context * ctx, pdf_document * doc, pdf_obj * rdb, const char * usage, pdf_obj * ve) Line 580
	at Z:\lib\tooling\qiqqa\MuPDF\source\pdf\pdf-layer.c(580)
MuPDFLib.dll!calc_ve_state(fz_context * ctx, pdf_document * doc, pdf_obj * rdb, const char * usage, pdf_obj * ve) Line 602
	at Z:\lib\tooling\qiqqa\MuPDF\source\pdf\pdf-layer.c(602)
[...rinse & repeat for ~ 10000 lines...]
MuPDFLib.dll!pdf_is_ocg_hidden(fz_context * ctx, pdf_document * doc, pdf_obj * rdb, const char * usage, pdf_obj * ocg) Line 797
	at Z:\lib\tooling\qiqqa\MuPDF\source\pdf\pdf-layer.c(797)
MuPDFLib.dll!calc_ve_state(fz_context * ctx, pdf_document * doc, pdf_obj * rdb, const char * usage, pdf_obj * ve) Line 580
	at Z:\lib\tooling\qiqqa\MuPDF\source\pdf\pdf-layer.c(580)
MuPDFLib.dll!calc_ve_state(fz_context * ctx, pdf_document * doc, pdf_obj * rdb, const char * usage, pdf_obj * ve) Line 602
	at Z:\lib\tooling\qiqqa\MuPDF\source\pdf\pdf-layer.c(602)
The maximum number of stack frames supported by Visual Studio has been exceeded.
```











##### Item ♯00135


Yay! `bulktest` finished! (OK, done in chunks at sampling rates varying between 11% and 31%, most of the time <= 15%)

Last chunk did not trigger the debugger any more:


```
total 3665160ms / 1658 commands for an average of 2210ms in 1658 commands
fastest command line 189 (dataline: 945): 5ms (MUTOOL extract -o //?/J:/__bulktest/DATA/__bulktest/TextExtractFiles-T1/TestData/data/fixtures/PDF/qpdf/split-exp-02/FULL-DOC.extract.dump -r TestData/data/fixtures/PDF/qpdf/split-exp-02.Pdf)
slowest command line 160 (dataline: 865): 347885ms (MUTOOL draw -o //?/J:/__bulktest/DATA/__bulktest/TextExtractFiles-T1/Sample-PDFs-for-format-testing/ICJ-CIJ/big/150-17582/FULL-DOC-x300.webp -s mtf -r 300 -y l -P -B 50 Sample-PDFs-for-format-testing/ICJ-CIJ/big/150-17582.pdf)
```



