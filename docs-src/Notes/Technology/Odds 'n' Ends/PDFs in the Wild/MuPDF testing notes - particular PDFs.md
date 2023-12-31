# MuPDF Testing Notes - Peculiar/Particular PDFs

Observations when bulk testing `mutool multipurp` (aspirant tool to provide metadata, such as PageCount, but also outlines, annotations and attachment info (what Qiqqa uses SynFusion stuff for today)

> Also note our `bulktest` batch test runs' log book notes here: [[../../../Progress in Development/Testing & Evaluating/PDF bulktest test run notes at the bleeding edge]], which contains additional (and some more recent) information about PDF processing troubles. 

---

Took ages to info:

ECHO T:2699h1

```
MUTOOL info -o "__mujstest/Sample-PDFs-for-format-testing/isartor-pdfa-2008-08-13/Isartor testsuite/PDFA-1b/6.1 File structure/6.1.12 Implementation Limits/isartor-6-1-12-t01-fail-a/%04d.info.txt" "Sample-PDFs-for-format-testing/isartor-pdfa-2008-08-13/Isartor testsuite/PDFA-1b/6.1 File structure/6.1.12 Implementation Limits/isartor-6-1-12-t01-fail-a.pdf"
```


T4446  

T4565

T1947













UTF16 BOM in T23, T52 (Chinese Creator?), T408 (little endian BOM!), 
(Note there's often a \u00FD (or equiv: \uFD00 ;-) ) at the end of the string. Or it includes a \x00 sentinel just before that.)


T5076 attempts a buffer overflow? print allocated the title string HHHHH...., then frees it after use.

T7481 bested that one, once buffer was set at 2K. Oddly enough, it's the outline Title, that's a whole paragraph of text...

```
% ---------------------------------------------------------------------------------------------------------------
% PDF: ../base/Guest1/documents/2/2835BEBED5F33DDAB8F9FAB252354746C745BC29.pdf
% dir: ../base/Guest1/documents/2
% name: 2835BEBED5F33DDAB8F9FAB252354746C745BC29.pdf
% base: 2835BEBED5F33DDAB8F9FAB252354746C745BC29

% CD G:/Qiqqa/evil-base
CD {SCRIPTDIR}

ECHO T:7481h1

MUTOOL multipurp -o "__mujstest/../base/Guest1/documents/2/2835BEBED5F33DDAB8F9FAB252354746C745BC29/%04d.info.txt" "../base/Guest1/documents/2/2835BEBED5F33DDAB8F9FAB252354746C745BC29.pdf"
```






CRASH (now fixed and reported over at Artifex site; tracking...):

```
% ---------------------------------------------------------------------------------------------------------------
% PDF: ../base/C9F0D079-EE4C-4A15-8547-72164A7A356D/documents/1/1F29DE4E5BF35D879ED46AED61892DE6FA1EFE.pdf
% dir: ../base/C9F0D079-EE4C-4A15-8547-72164A7A356D/documents/1
% name: 1F29DE4E5BF35D879ED46AED61892DE6FA1EFE.pdf
% base: 1F29DE4E5BF35D879ED46AED61892DE6FA1EFE

% CD G:/Qiqqa/evil-base
CD {SCRIPTDIR}

ECHO T:273h1

MUTOOL multipurp -o "__mujstest/../base/C9F0D079-EE4C-4A15-8547-72164A7A356D/documents/1/1F29DE4E5BF35D879ED46AED61892DE6FA1EFE/%04d.info.txt" "../base/C9F0D079-EE4C-4A15-8547-72164A7A356D/documents/1/1F29DE4E5BF35D879ED46AED61892DE6FA1EFE.pdf"
```



	
T5442: memory hog.

```
% ---------------------------------------------------------------------------------------------------------------
% PDF: ../base/Guest1/documents/1/18D155D36C587AEA67F26D7654C2398EDBA583.pdf
% dir: ../base/Guest1/documents/1
% name: 18D155D36C587AEA67F26D7654C2398EDBA583.pdf
% base: 18D155D36C587AEA67F26D7654C2398EDBA583

% CD G:/Qiqqa/evil-base
CD {SCRIPTDIR}

ECHO T:5442h1

MUTOOL multipurp -o "__mujstest/../base/Guest1/documents/1/18D155D36C587AEA67F26D7654C2398EDBA583/%04d.info.txt" "../base/Guest1/documents/1/18D155D36C587AEA67F26D7654C2398EDBA583.pdf"
```






T9277: triggers the UTF16 astral plane unicode decode logic by way of a chance encounter with an Indexed Image: the /ColorSpace /DeviceRGB value happens to have the magic value.

```
% ---------------------------------------------------------------------------------------------------------------
% PDF: ../base/Guest1/documents/3/36BA264A071B71E904D7F75D81BB7453676D5D2.pdf
% dir: ../base/Guest1/documents/3
% name: 36BA264A071B71E904D7F75D81BB7453676D5D2.pdf
% base: 36BA264A071B71E904D7F75D81BB7453676D5D2

% CD G:/Qiqqa/evil-base
CD {SCRIPTDIR}

ECHO T:9277h1

MUTOOL multipurp -o "__mujstest/../base/Guest1/documents/3/36BA264A071B71E904D7F75D81BB7453676D5D2/%04d.info.txt" "../base/Guest1/documents/3/36BA264A071B71E904D7F75D81BB7453676D5D2.pdf"
```




T13385: another seriously slow mother:

```
% ---------------------------------------------------------------------------------------------------------------
% PDF: ../base/Guest1/documents/5/56831E9293475B87B10CB53E84AAD11B8455397.pdf
% dir: ../base/Guest1/documents/5
% name: 56831E9293475B87B10CB53E84AAD11B8455397.pdf
% base: 56831E9293475B87B10CB53E84AAD11B8455397

% CD G:/Qiqqa/evil-base
CD {SCRIPTDIR}

ECHO T:13385h1

MUTOOL multipurp -o "__mujstest/../base/Guest1/documents/5/56831E9293475B87B10CB53E84AAD11B8455397/%04d.info.txt" "../base/Guest1/documents/5/56831E9293475B87B10CB53E84AAD11B8455397.pdf"
```





and one more T56256:

```
% ---------------------------------------------------------------------------------------------------------------
% PDF: ../base/INTRANET_EF52564A-831D-42F2-B956-815CF0418C08/documents/8/865DD2BB2769B17A83D8BB9EB1AE2D31FCC2A88.pdf
% dir: ../base/INTRANET_EF52564A-831D-42F2-B956-815CF0418C08/documents/8
% name: 865DD2BB2769B17A83D8BB9EB1AE2D31FCC2A88.pdf
% base: 865DD2BB2769B17A83D8BB9EB1AE2D31FCC2A88

% CD G:/Qiqqa/evil-base
CD {SCRIPTDIR}

ECHO T:56256h1

MUTOOL multipurp -o "__mujstest/../base/INTRANET_EF52564A-831D-42F2-B956-815CF0418C08/documents/8/865DD2BB2769B17A83D8BB9EB1AE2D31FCC2A88/%04d.info.txt" "../base/INTRANET_EF52564A-831D-42F2-B956-815CF0418C08/documents/8/865DD2BB2769B17A83D8BB9EB1AE2D31FCC2A88.pdf"
```






T16212: another DeviceRGB lucky UTF16 astral plane trigger

```
% ---------------------------------------------------------------------------------------------------------------
% PDF: ../base/Guest1/documents/C/C98FF828C3A8ECCF8942C41EE00F9DE7EC5663B.pdf
% dir: ../base/Guest1/documents/C
% name: C98FF828C3A8ECCF8942C41EE00F9DE7EC5663B.pdf
% base: C98FF828C3A8ECCF8942C41EE00F9DE7EC5663B

% CD G:/Qiqqa/evil-base
CD {SCRIPTDIR}

ECHO T:16212h1

MUTOOL multipurp -o "__mujstest/../base/Guest1/documents/C/C98FF828C3A8ECCF8942C41EE00F9DE7EC5663B/%04d.info.txt" "../base/Guest1/documents/C/C98FF828C3A8ECCF8942C41EE00F9DE7EC5663B.pdf"
```






T56552 *finally* is one that triggers the astral plane Unicode logic in the *regular* UTF8 string JSON serializer:

```
% ---------------------------------------------------------------------------------------------------------------
% PDF: ../base/INTRANET_EF52564A-831D-42F2-B956-815CF0418C08/documents/8/877F8F79B8F3ED5CA953934C2E6EA5F11AEAFA4D.pdf
% dir: ../base/INTRANET_EF52564A-831D-42F2-B956-815CF0418C08/documents/8
% name: 877F8F79B8F3ED5CA953934C2E6EA5F11AEAFA4D.pdf
% base: 877F8F79B8F3ED5CA953934C2E6EA5F11AEAFA4D

% CD G:/Qiqqa/evil-base
CD {SCRIPTDIR}

ECHO T:56552h1

MUTOOL multipurp -o "__mujstest/../base/INTRANET_EF52564A-831D-42F2-B956-815CF0418C08/documents/8/877F8F79B8F3ED5CA953934C2E6EA5F11AEAFA4D/%04d.info.txt" "../base/INTRANET_EF52564A-831D-42F2-B956-815CF0418C08/documents/8/877F8F79B8F3ED5CA953934C2E6EA5F11AEAFA4D.pdf"
```


and then this one did the same (but *many* times!):

```
% ---------------------------------------------------------------------------------------------------------------
% PDF: ../base/INTRANET_EF52564A-831D-42F2-B956-815CF0418C08/documents/9/9AB415BF1862508560CA3462BE61D7E4A5511ED.pdf
% dir: ../base/INTRANET_EF52564A-831D-42F2-B956-815CF0418C08/documents/9
% name: 9AB415BF1862508560CA3462BE61D7E4A5511ED.pdf
% base: 9AB415BF1862508560CA3462BE61D7E4A5511ED

% CD G:/Qiqqa/evil-base
CD {SCRIPTDIR}

ECHO T:61124h1

MUTOOL multipurp -o "__mujstest/../base/INTRANET_EF52564A-831D-42F2-B956-815CF0418C08/documents/9/9AB415BF1862508560CA3462BE61D7E4A5511ED/%04d.info.txt" "../base/INTRANET_EF52564A-831D-42F2-B956-815CF0418C08/documents/9/9AB415BF1862508560CA3462BE61D7E4A5511ED.pdf"
```

and one more:

```
% ---------------------------------------------------------------------------------------------------------------
% PDF: ../base/INTRANET_EF52564A-831D-42F2-B956-815CF0418C08/documents/A/AE6F35C13EBDAB1E6C4EA0DB37ADA8A3F68DDD.pdf
% dir: ../base/INTRANET_EF52564A-831D-42F2-B956-815CF0418C08/documents/A
% name: AE6F35C13EBDAB1E6C4EA0DB37ADA8A3F68DDD.pdf
% base: AE6F35C13EBDAB1E6C4EA0DB37ADA8A3F68DDD

% CD G:/Qiqqa/evil-base
CD {SCRIPTDIR}

ECHO T:64852h1

MUTOOL multipurp -o "__mujstest/../base/INTRANET_EF52564A-831D-42F2-B956-815CF0418C08/documents/A/AE6F35C13EBDAB1E6C4EA0DB37ADA8A3F68DDD/%04d.info.txt" "../base/INTRANET_EF52564A-831D-42F2-B956-815CF0418C08/documents/A/AE6F35C13EBDAB1E6C4EA0DB37ADA8A3F68DDD.pdf"
```





Took still longer to hit a PDF with the other Endianess encoding of UTF16 to hit the astral planes Unicode logic:

```
% ---------------------------------------------------------------------------------------------------------------
% PDF: ../base/INTRANET_EF52564A-831D-42F2-B956-815CF0418C08/documents/8/865DD2BB2769B17A83D8BB9EB1AE2D31FCC2A88.pdf
% dir: ../base/INTRANET_EF52564A-831D-42F2-B956-815CF0418C08/documents/8
% name: 865DD2BB2769B17A83D8BB9EB1AE2D31FCC2A88.pdf
% base: 865DD2BB2769B17A83D8BB9EB1AE2D31FCC2A88

% CD G:/Qiqqa/evil-base
CD {SCRIPTDIR}

ECHO T:56256h1

MUTOOL multipurp -o "__mujstest/../base/INTRANET_EF52564A-831D-42F2-B956-815CF0418C08/documents/8/865DD2BB2769B17A83D8BB9EB1AE2D31FCC2A88/%04d.info.txt" "../base/INTRANET_EF52564A-831D-42F2-B956-815CF0418C08/documents/8/865DD2BB2769B17A83D8BB9EB1AE2D31FCC2A88.pdf"
```

That one is sure odd stuff: here's what was generated from that so far:

```
{
  "/CreationDate": "D:20130118161422-06'00'",
  "/Subject": "HEX INVERTERS WITH SCHMITT TRIGGER INPUTS",
  "/Author": "Diodes Incorporated",
  "/Creator": "PScript5.dll Version 5.2.2",
  "/Keywords": "74HC14 HEX INVERTERS WITH SCHMITT TRIGGER INPUTS SO-14, TSSOP-14\r\n\u2022 Wide Supply Voltage Range from 2.0V to 6.0V\r\n\u2022 Sinks or Sources 4mA at VCC = 4.5V\r\n\u2022 CMOS Low Power Consumption\r\n\u2022 Schmitt Trigger Action at All Inputs\r\n\u2022 ESD Protection Exceeds JESD 22\r\n\u100083 200-V Machine Model (A115-A)\r\n\u100083 2000-V Human Body Model (A114-A)\r\n\u100083 Exceeds 1000-V Charged Device Model (C101C)\r\n\u2022 Range of Package Options SO-14 and TSSOP-14\r\n\u2022 Totally Lead-Free & Fully RoHS Compliant (Notes 1 & 2)\r\n\u2022 Halogen and Antimony Free. \u201CGreen\u201D Device\r\n\u00FD"
```

Note the \u100083 unicode codepoints in there... A very modern CodePoint for a very old chip!  :-D







Ha.

```
{
  "/Title": { HEX: "FE FF",
  RAW: "þÿ"
},
```

just another way of saying: empty string!




---



Took ages to info:

```
ECHO T:2699h1

MUTOOL info -o "__mujstest/Sample-PDFs-for-format-testing/isartor-pdfa-2008-08-13/Isartor testsuite/PDFA-1b/6.1 File structure/6.1.12 Implementation Limits/isartor-6-1-12-t01-fail-a/%04d.info.txt" "Sample-PDFs-for-format-testing/isartor-pdfa-2008-08-13/Isartor testsuite/PDFA-1b/6.1 File structure/6.1.12 Implementation Limits/isartor-6-1-12-t01-fail-a.pdf"
```


511 pages with some over 4000 image slots?! Anyway, talk about *slow*:

```
% ---------------------------------------------------------------------------------------------------------------
% PDF: Sample-PDFs-for-format-testing/Qiqqa Tools - test collection/865DD2BB2769B17A83D8BB9EB1AE2D31FCC2A88.pdf
% dir: Sample-PDFs-for-format-testing/Qiqqa Tools - test collection
% name: 865DD2BB2769B17A83D8BB9EB1AE2D31FCC2A88.pdf
% base: 865DD2BB2769B17A83D8BB9EB1AE2D31FCC2A88

% CD G:/Qiqqa/evil-base
CD {SCRIPTDIR}

ECHO T:4281h1

MUTOOL multipurp -o "__mujstest/Sample-PDFs-for-format-testing/Qiqqa Tools - test collection/865DD2BB2769B17A83D8BB9EB1AE2D31FCC2A88/%04d.info.json" "Sample-PDFs-for-format-testing/Qiqqa Tools - test collection/865DD2BB2769B17A83D8BB9EB1AE2D31FCC2A88.pdf"
```



```
% ---------------------------------------------------------------------------------------------------------------
% PDF: Sample-PDFs-for-format-testing/Qiqqa Tools - test collection/C98FF828C3A8ECCF8942C41EE00F9DE7EC5663B.pdf
% dir: Sample-PDFs-for-format-testing/Qiqqa Tools - test collection
% name: C98FF828C3A8ECCF8942C41EE00F9DE7EC5663B.pdf
% base: C98FF828C3A8ECCF8942C41EE00F9DE7EC5663B

% CD G:/Qiqqa/evil-base
CD {SCRIPTDIR}

ECHO T:4315h1

MUTOOL multipurp -o "__mujstest/Sample-PDFs-for-format-testing/Qiqqa Tools - test collection/C98FF828C3A8ECCF8942C41EE00F9DE7EC5663B/%04d.info.json" "Sample-PDFs-for-format-testing/Qiqqa Tools - test collection/C98FF828C3A8ECCF8942C41EE00F9DE7EC5663B.pdf"
```



```
% ---------------------------------------------------------------------------------------------------------------
% PDF: Sample-PDFs-for-format-testing/From.MuPDF.and.GhostScript.bugtracker/A4.3.1.pdf
% dir: Sample-PDFs-for-format-testing/From.MuPDF.and.GhostScript.bugtracker
% name: A4.3.1.pdf
% base: A4.3.1

% CD G:/Qiqqa/evil-base
CD {SCRIPTDIR}

ECHO T:1947h1

MUTOOL multipurp -o "__mujstest/Sample-PDFs-for-format-testing/From.MuPDF.and.GhostScript.bugtracker/A4.3.1/%04d.info.json" "Sample-PDFs-for-format-testing/From.MuPDF.and.GhostScript.bugtracker/A4.3.1.pdf"
```



```
% ---------------------------------------------------------------------------------------------------------------
% PDF: Sample-PDFs-for-format-testing/isartor-pdfa-2008-08-13/Isartor testsuite/PDFA-1b/6.1 File structure/6.1.12 Implementation Limits/isartor-6-1-12-t01-fail-a.pdf
% dir: Sample-PDFs-for-format-testing/isartor-pdfa-2008-08-13/Isartor testsuite/PDFA-1b/6.1 File structure/6.1.12 Implementation Limits
% name: isartor-6-1-12-t01-fail-a.pdf
% base: isartor-6-1-12-t01-fail-a

% CD G:/Qiqqa/evil-base
CD {SCRIPTDIR}

ECHO T:2699h1

MUTOOL multipurp -o "__mujstest/Sample-PDFs-for-format-testing/isartor-pdfa-2008-08-13/Isartor testsuite/PDFA-1b/6.1 File structure/6.1.12 Implementation Limits/isartor-6-1-12-t01-fail-a/%04d.info.json" "Sample-PDFs-for-format-testing/isartor-pdfa-2008-08-13/Isartor testsuite/PDFA-1b/6.1 File structure/6.1.12 Implementation Limits/isartor-6-1-12-t01-fail-a.pdf"
```



```
% ---------------------------------------------------------------------------------------------------------------
% PDF: Sample-PDFs-for-format-testing/Qiqqa Tools - test collection/36BA264A071B71E904D7F75D81BB7453676D5D2.pdf
% dir: Sample-PDFs-for-format-testing/Qiqqa Tools - test collection
% name: 36BA264A071B71E904D7F75D81BB7453676D5D2.pdf
% base: 36BA264A071B71E904D7F75D81BB7453676D5D2

% CD G:/Qiqqa/evil-base
CD {SCRIPTDIR}

ECHO T:4276h1

MUTOOL multipurp -o "__mujstest/Sample-PDFs-for-format-testing/Qiqqa Tools - test collection/36BA264A071B71E904D7F75D81BB7453676D5D2/%04d.info.json" "Sample-PDFs-for-format-testing/Qiqqa Tools - test collection/36BA264A071B71E904D7F75D81BB7453676D5D2.pdf"
```




lots of output on this one:

```
% ---------------------------------------------------------------------------------------------------------------
% PDF: Sample-PDFs-for-format-testing/Qiqqa Tools - test collection/1F29DE4E5BF35D879ED46AED61892DE6FA1EFE.pdf
% dir: Sample-PDFs-for-format-testing/Qiqqa Tools - test collection
% name: 1F29DE4E5BF35D879ED46AED61892DE6FA1EFE.pdf
% base: 1F29DE4E5BF35D879ED46AED61892DE6FA1EFE

% CD G:/Qiqqa/evil-base
CD {SCRIPTDIR}

ECHO T:4269h1

MUTOOL multipurp -o "__mujstest/Sample-PDFs-for-format-testing/Qiqqa Tools - test collection/1F29DE4E5BF35D879ED46AED61892DE6FA1EFE/%04d.info.json" "Sample-PDFs-for-format-testing/Qiqqa Tools - test collection/1F29DE4E5BF35D879ED46AED61892DE6FA1EFE.pdf"
```



slowest of them all?

```
% ---------------------------------------------------------------------------------------------------------------
% PDF: Sample-PDFs-for-format-testing/Qiqqa Tools - test collection/56831E9293475B87B10CB53E84AAD11B8455397.pdf
% dir: Sample-PDFs-for-format-testing/Qiqqa Tools - test collection
% name: 56831E9293475B87B10CB53E84AAD11B8455397.pdf
% base: 56831E9293475B87B10CB53E84AAD11B8455397

% CD G:/Qiqqa/evil-base
CD {SCRIPTDIR}

ECHO T:4279h1

MUTOOL multipurp -o "__mujstest/Sample-PDFs-for-format-testing/Qiqqa Tools - test collection/56831E9293475B87B10CB53E84AAD11B8455397/%04d.info.json" "Sample-PDFs-for-format-testing/Qiqqa Tools - test collection/56831E9293475B87B10CB53E84AAD11B8455397.pdf"
```



```
% ---------------------------------------------------------------------------------------------------------------
% PDF: Sample-PDFs-for-format-testing/Qiqqa Tools - test collection/865DD2BB2769B17A83D8BB9EB1AE2D31FCC2A88.pdf
% dir: Sample-PDFs-for-format-testing/Qiqqa Tools - test collection
% name: 865DD2BB2769B17A83D8BB9EB1AE2D31FCC2A88.pdf
% base: 865DD2BB2769B17A83D8BB9EB1AE2D31FCC2A88

% CD G:/Qiqqa/evil-base
CD {SCRIPTDIR}

ECHO T:4281h1

MUTOOL multipurp -o "__mujstest/Sample-PDFs-for-format-testing/Qiqqa Tools - test collection/865DD2BB2769B17A83D8BB9EB1AE2D31FCC2A88/%04d.info.json" "Sample-PDFs-for-format-testing/Qiqqa Tools - test collection/865DD2BB2769B17A83D8BB9EB1AE2D31FCC2A88.pdf"
```



```
% ---------------------------------------------------------------------------------------------------------------
% PDF: Sample-PDFs-for-format-testing/Qiqqa Tools - test collection/C98FF828C3A8ECCF8942C41EE00F9DE7EC5663B.pdf
% dir: Sample-PDFs-for-format-testing/Qiqqa Tools - test collection
% name: C98FF828C3A8ECCF8942C41EE00F9DE7EC5663B.pdf
% base: C98FF828C3A8ECCF8942C41EE00F9DE7EC5663B

% CD G:/Qiqqa/evil-base
CD {SCRIPTDIR}

ECHO T:4315h1

MUTOOL multipurp -o "__mujstest/Sample-PDFs-for-format-testing/Qiqqa Tools - test collection/C98FF828C3A8ECCF8942C41EE00F9DE7EC5663B/%04d.info.json" "Sample-PDFs-for-format-testing/Qiqqa Tools - test collection/C98FF828C3A8ECCF8942C41EE00F9DE7EC5663B.pdf"
```

	

huge output:

```
% ---------------------------------------------------------------------------------------------------------------
% PDF: Sample-PDFs-for-format-testing/Qiqqa Tools - test collection/TE2000_TC3_HMI_EN.pdf
% dir: Sample-PDFs-for-format-testing/Qiqqa Tools - test collection
% name: TE2000_TC3_HMI_EN.pdf
% base: TE2000_TC3_HMI_EN

% CD G:/Qiqqa/evil-base
CD {SCRIPTDIR}

ECHO T:4457h1

MUTOOL multipurp -o "__mujstest/Sample-PDFs-for-format-testing/Qiqqa Tools - test collection/TE2000_TC3_HMI_EN/%04d.info.json" "Sample-PDFs-for-format-testing/Qiqqa Tools - test collection/TE2000_TC3_HMI_EN.pdf"
```



slow:

```
% ---------------------------------------------------------------------------------------------------------------
% PDF: Sample-PDFs-for-format-testing/testset misc 2/global/ec00d5825f47b9d0faa953b1709163c3.pdf
% dir: Sample-PDFs-for-format-testing/testset misc 2/global
% name: ec00d5825f47b9d0faa953b1709163c3.pdf
% base: ec00d5825f47b9d0faa953b1709163c3

% CD G:/Qiqqa/evil-base
CD {SCRIPTDIR}

ECHO T:4576h1

MUTOOL multipurp -o "__mujstest/Sample-PDFs-for-format-testing/testset misc 2/global/ec00d5825f47b9d0faa953b1709163c3/%04d.info.json" "Sample-PDFs-for-format-testing/testset misc 2/global/ec00d5825f47b9d0faa953b1709163c3.pdf"
```

	
	
















UTF16 BOM in T23, T52 (Chinese Creator?), T408 (little endian BOM!), 
(Note there's often a `\u00FD` (or equiv: `\uFD00` ;-) ) at the end of the string. Or it includes a `\x00` sentinel just before that.)


T5076 attempts a buffer overflow? print allocated the title string HHHHH...., then frees it after use.

T7481 bested that one, once buffer was set at 2K. Oddly enough, it's the outline Title, that's a whole paragraph of text...

```
% ---------------------------------------------------------------------------------------------------------------
% PDF: ../base/Guest1/documents/2/2835BEBED5F33DDAB8F9FAB252354746C745BC29.pdf
% dir: ../base/Guest1/documents/2
% name: 2835BEBED5F33DDAB8F9FAB252354746C745BC29.pdf
% base: 2835BEBED5F33DDAB8F9FAB252354746C745BC29

% CD G:/Qiqqa/evil-base
CD {SCRIPTDIR}

ECHO T:7481h1

MUTOOL multipurp -o "__mujstest/../base/Guest1/documents/2/2835BEBED5F33DDAB8F9FAB252354746C745BC29/%04d.info.txt" "../base/Guest1/documents/2/2835BEBED5F33DDAB8F9FAB252354746C745BC29.pdf"
```






CRASH:

```
% ---------------------------------------------------------------------------------------------------------------
% PDF: ../base/C9F0D079-EE4C-4A15-8547-72164A7A356D/documents/1/1F29DE4E5BF35D879ED46AED61892DE6FA1EFE.pdf
% dir: ../base/C9F0D079-EE4C-4A15-8547-72164A7A356D/documents/1
% name: 1F29DE4E5BF35D879ED46AED61892DE6FA1EFE.pdf
% base: 1F29DE4E5BF35D879ED46AED61892DE6FA1EFE

% CD G:/Qiqqa/evil-base
CD {SCRIPTDIR}

ECHO T:273h1

MUTOOL multipurp -o "__mujstest/../base/C9F0D079-EE4C-4A15-8547-72164A7A356D/documents/1/1F29DE4E5BF35D879ED46AED61892DE6FA1EFE/%04d.info.txt" "../base/C9F0D079-EE4C-4A15-8547-72164A7A356D/documents/1/1F29DE4E5BF35D879ED46AED61892DE6FA1EFE.pdf"
```



	
T5442: memory hog.

```
% ---------------------------------------------------------------------------------------------------------------
% PDF: ../base/Guest1/documents/1/18D155D36C587AEA67F26D7654C2398EDBA583.pdf
% dir: ../base/Guest1/documents/1
% name: 18D155D36C587AEA67F26D7654C2398EDBA583.pdf
% base: 18D155D36C587AEA67F26D7654C2398EDBA583

% CD G:/Qiqqa/evil-base
CD {SCRIPTDIR}

ECHO T:5442h1

MUTOOL multipurp -o "__mujstest/../base/Guest1/documents/1/18D155D36C587AEA67F26D7654C2398EDBA583/%04d.info.txt" "../base/Guest1/documents/1/18D155D36C587AEA67F26D7654C2398EDBA583.pdf"
```






T9277: triggers the UTF16 astral plane unicode decode logic by way of a chance encounter with an Indexed Image: the `/ColorSpace /DeviceRGB` value happens to have the magic value.

```
% ---------------------------------------------------------------------------------------------------------------
% PDF: ../base/Guest1/documents/3/36BA264A071B71E904D7F75D81BB7453676D5D2.pdf
% dir: ../base/Guest1/documents/3
% name: 36BA264A071B71E904D7F75D81BB7453676D5D2.pdf
% base: 36BA264A071B71E904D7F75D81BB7453676D5D2

% CD G:/Qiqqa/evil-base
CD {SCRIPTDIR}

ECHO T:9277h1

MUTOOL multipurp -o "__mujstest/../base/Guest1/documents/3/36BA264A071B71E904D7F75D81BB7453676D5D2/%04d.info.txt" "../base/Guest1/documents/3/36BA264A071B71E904D7F75D81BB7453676D5D2.pdf"
```




T13385: another seriously slow mother:

```
% ---------------------------------------------------------------------------------------------------------------
% PDF: ../base/Guest1/documents/5/56831E9293475B87B10CB53E84AAD11B8455397.pdf
% dir: ../base/Guest1/documents/5
% name: 56831E9293475B87B10CB53E84AAD11B8455397.pdf
% base: 56831E9293475B87B10CB53E84AAD11B8455397

% CD G:/Qiqqa/evil-base
CD {SCRIPTDIR}

ECHO T:13385h1

MUTOOL multipurp -o "__mujstest/../base/Guest1/documents/5/56831E9293475B87B10CB53E84AAD11B8455397/%04d.info.txt" "../base/Guest1/documents/5/56831E9293475B87B10CB53E84AAD11B8455397.pdf"
```





and one more: T56256:

```
% ---------------------------------------------------------------------------------------------------------------
% PDF: ../base/INTRANET_EF52564A-831D-42F2-B956-815CF0418C08/documents/8/865DD2BB2769B17A83D8BB9EB1AE2D31FCC2A88.pdf
% dir: ../base/INTRANET_EF52564A-831D-42F2-B956-815CF0418C08/documents/8
% name: 865DD2BB2769B17A83D8BB9EB1AE2D31FCC2A88.pdf
% base: 865DD2BB2769B17A83D8BB9EB1AE2D31FCC2A88

% CD G:/Qiqqa/evil-base
CD {SCRIPTDIR}

ECHO T:56256h1

MUTOOL multipurp -o "__mujstest/../base/INTRANET_EF52564A-831D-42F2-B956-815CF0418C08/documents/8/865DD2BB2769B17A83D8BB9EB1AE2D31FCC2A88/%04d.info.txt" "../base/INTRANET_EF52564A-831D-42F2-B956-815CF0418C08/documents/8/865DD2BB2769B17A83D8BB9EB1AE2D31FCC2A88.pdf"
```






T16212: another DeviceRGB lucky UTF16 astral plane trigger

```
% ---------------------------------------------------------------------------------------------------------------
% PDF: ../base/Guest1/documents/C/C98FF828C3A8ECCF8942C41EE00F9DE7EC5663B.pdf
% dir: ../base/Guest1/documents/C
% name: C98FF828C3A8ECCF8942C41EE00F9DE7EC5663B.pdf
% base: C98FF828C3A8ECCF8942C41EE00F9DE7EC5663B

% CD G:/Qiqqa/evil-base
CD {SCRIPTDIR}

ECHO T:16212h1

MUTOOL multipurp -o "__mujstest/../base/Guest1/documents/C/C98FF828C3A8ECCF8942C41EE00F9DE7EC5663B/%04d.info.txt" "../base/Guest1/documents/C/C98FF828C3A8ECCF8942C41EE00F9DE7EC5663B.pdf"
```






T56552 *finally* is one that triggers the astral plane Unicode logic in the *regular* UTF8 string JSON serializer:

```
% ---------------------------------------------------------------------------------------------------------------
% PDF: ../base/INTRANET_EF52564A-831D-42F2-B956-815CF0418C08/documents/8/877F8F79B8F3ED5CA953934C2E6EA5F11AEAFA4D.pdf
% dir: ../base/INTRANET_EF52564A-831D-42F2-B956-815CF0418C08/documents/8
% name: 877F8F79B8F3ED5CA953934C2E6EA5F11AEAFA4D.pdf
% base: 877F8F79B8F3ED5CA953934C2E6EA5F11AEAFA4D

% CD G:/Qiqqa/evil-base
CD {SCRIPTDIR}

ECHO T:56552h1

MUTOOL multipurp -o "__mujstest/../base/INTRANET_EF52564A-831D-42F2-B956-815CF0418C08/documents/8/877F8F79B8F3ED5CA953934C2E6EA5F11AEAFA4D/%04d.info.txt" "../base/INTRANET_EF52564A-831D-42F2-B956-815CF0418C08/documents/8/877F8F79B8F3ED5CA953934C2E6EA5F11AEAFA4D.pdf"
```


and then this one did the same (but *many* times!):

```
% ---------------------------------------------------------------------------------------------------------------
% PDF: ../base/INTRANET_EF52564A-831D-42F2-B956-815CF0418C08/documents/9/9AB415BF1862508560CA3462BE61D7E4A5511ED.pdf
% dir: ../base/INTRANET_EF52564A-831D-42F2-B956-815CF0418C08/documents/9
% name: 9AB415BF1862508560CA3462BE61D7E4A5511ED.pdf
% base: 9AB415BF1862508560CA3462BE61D7E4A5511ED

% CD G:/Qiqqa/evil-base
CD {SCRIPTDIR}

ECHO T:61124h1

MUTOOL multipurp -o "__mujstest/../base/INTRANET_EF52564A-831D-42F2-B956-815CF0418C08/documents/9/9AB415BF1862508560CA3462BE61D7E4A5511ED/%04d.info.txt" "../base/INTRANET_EF52564A-831D-42F2-B956-815CF0418C08/documents/9/9AB415BF1862508560CA3462BE61D7E4A5511ED.pdf"
```

and one more:

```
% ---------------------------------------------------------------------------------------------------------------
% PDF: ../base/INTRANET_EF52564A-831D-42F2-B956-815CF0418C08/documents/A/AE6F35C13EBDAB1E6C4EA0DB37ADA8A3F68DDD.pdf
% dir: ../base/INTRANET_EF52564A-831D-42F2-B956-815CF0418C08/documents/A
% name: AE6F35C13EBDAB1E6C4EA0DB37ADA8A3F68DDD.pdf
% base: AE6F35C13EBDAB1E6C4EA0DB37ADA8A3F68DDD

% CD G:/Qiqqa/evil-base
CD {SCRIPTDIR}

ECHO T:64852h1

MUTOOL multipurp -o "__mujstest/../base/INTRANET_EF52564A-831D-42F2-B956-815CF0418C08/documents/A/AE6F35C13EBDAB1E6C4EA0DB37ADA8A3F68DDD/%04d.info.txt" "../base/INTRANET_EF52564A-831D-42F2-B956-815CF0418C08/documents/A/AE6F35C13EBDAB1E6C4EA0DB37ADA8A3F68DDD.pdf"
```





Took still longer to hit a PDF with the other Endianess encoding of UTF16 to hit the astral planes Unicode logic:

```
% ---------------------------------------------------------------------------------------------------------------
% PDF: ../base/INTRANET_EF52564A-831D-42F2-B956-815CF0418C08/documents/8/865DD2BB2769B17A83D8BB9EB1AE2D31FCC2A88.pdf
% dir: ../base/INTRANET_EF52564A-831D-42F2-B956-815CF0418C08/documents/8
% name: 865DD2BB2769B17A83D8BB9EB1AE2D31FCC2A88.pdf
% base: 865DD2BB2769B17A83D8BB9EB1AE2D31FCC2A88

% CD G:/Qiqqa/evil-base
CD {SCRIPTDIR}

ECHO T:56256h1

MUTOOL multipurp -o "__mujstest/../base/INTRANET_EF52564A-831D-42F2-B956-815CF0418C08/documents/8/865DD2BB2769B17A83D8BB9EB1AE2D31FCC2A88/%04d.info.txt" "../base/INTRANET_EF52564A-831D-42F2-B956-815CF0418C08/documents/8/865DD2BB2769B17A83D8BB9EB1AE2D31FCC2A88.pdf"
```

That one is sure odd stuff: here's what was generated from that so far:

```
{
  "/CreationDate": "D:20130118161422-06'00'",
  "/Subject": "HEX INVERTERS WITH SCHMITT TRIGGER INPUTS",
  "/Author": "Diodes Incorporated",
  "/Creator": "PScript5.dll Version 5.2.2",
  "/Keywords": "74HC14 HEX INVERTERS WITH SCHMITT TRIGGER INPUTS SO-14, TSSOP-14\r\n\u2022 Wide Supply Voltage Range from 2.0V to 6.0V\r\n\u2022 Sinks or Sources 4mA at VCC = 4.5V\r\n\u2022 CMOS Low Power Consumption\r\n\u2022 Schmitt Trigger Action at All Inputs\r\n\u2022 ESD Protection Exceeds JESD 22\r\n\u100083 200-V Machine Model (A115-A)\r\n\u100083 2000-V Human Body Model (A114-A)\r\n\u100083 Exceeds 1000-V Charged Device Model (C101C)\r\n\u2022 Range of Package Options SO-14 and TSSOP-14\r\n\u2022 Totally Lead-Free & Fully RoHS Compliant (Notes 1 & 2)\r\n\u2022 Halogen and Antimony Free. \u201CGreen\u201D Device\r\n\u00FD"
```

Note the `\u100083` unicode codepoints in there... A very modern CodePoint for a very old chip!  :-D







