# PDF bulktest + mutool_ex PDF + URL tests: logbook notes


# Test run notes at the bleeding edge

This is about the multiple test runs covering the `evil-base` PDF corpus: I've been collecting these notes over the years. **Big Caveat: these notes were valid at the time of writing, but MAY be obsolete or even contradicting current behaviour at any later moment, sometimes even *seconds* away from the original event.**

This is about the things we observe when applying our tools at the bleeding edge of development to existing PDFs of all sorts, plus more or less wicked Internet URLs we checked out and the (grave) bugs that pop up most unexpectedly.

This is the lump sum notes (logbook) of these test runs' *odd observations*.

**The Table Of Contents / Overview Index is at [[../PDF bulktest test run notes at the bleeding edge]].**

-------------------------------

(The logbook was started quite a while ago, back in 2020.)

*Here goes -- lower is later ==> updates are appended at the bottom.*

-------------------------------




## Debugging mutool_ex command lines, etc.







##### Item ♯00001





```
mutool test/debug command lines:

bulktest -v -smt -T G:\Qiqqa\evil-base\text_extract_pdf_files.bulktest    G:\Qiqqa\evil-base\text_extract_pdf_files.info-test.tsv
--> run bulktest script 


draw -smtf -F stext -r 300 -j content    "Z:\lib\tooling\qiqqa\Qiqqa\bin\blub-blob\Guest\documents\3\3752B65FCBD0A227B0A19673F03D3D7D52B58A.pdf" 41,42,43,44,45,46,47,48,49,50,51,52,53,54 -o tmp.tmp
--> text extraction (debugging qiqqa issue)

crow_mustache
--> crash

cwebp
--> unicode error



draw -smtf -F html  -j content -x reference-images=yes,mediabox-clip=no,preserve-spans=yes,reuse-images=yes,preserve-images=yes,preserve-whitespace=yes,preserve-ligatures=yes, -o tmp.html   "Z:\lib\tooling\qiqqa\Qiqqa\bin\blub-blob\Guest\documents\3\3752B65FCBD0A227B0A19673F03D3D7D52B58A.pdf"  



mediabox-clip=no,preserve-spans=yes,reuse-images=yes,preserve-images=yes,preserve-whitespace=yes,preserve-ligatures=yes,

fz_new_output_with_path()
fz_new_output()
```











##### Item ♯00002





```
---------------------------------------------------------------------------------------

curl:
--output-path-mimics-url --sanitize-with-extreme-prejudice --create-dirs --ftp-create-dirs --verbose --no-progress-bar  --insecure --no-clobber --remote-name-all --remote-header-name --output-dir ~/Downloads/pdfs_test2     https://www.ijsr.net/archive/v2i5/IJSRON2013934.pdf

curl::
--create-dirs --ftp-create-dirs --verbose --no-progress-bar  --insecure --no-clobber --remote-name-all --remote-header-name --output-dir ~/Downloads/pdfs_test                       https://www.bankofengland.co.uk/-/media/boe/files/inflation-report/2019/may/inflation-report-may-2019.pdf  https://www.bankofengland.co.uk/-/media/boe/files/inflation-report/2019/may/inflation-report-may-2019.pdf "https://webcms.pima.gov/UserFiles/Servers/Server_6/File/Community/CDNC/Outside%20Agency/How%20to%20Use%20Excel%20to%20Analyze%20Survey%20Data%20-%2011-1-18.pdf" "https://www.mckinsey.com/~/media/mckinsey/featured%20insights/diversity%20and%20inclusion/diversity%20wins%20how%20inclusion%20matters/diversity-wins-how-inclusion-matters-vf.pdf" "https://pubs.usgs.gov/tm/04/a11/tm4a11.pdf https://files.nc.gov/ncosc/documents/files/filteringdatainwebintelligencereports.pdf https://www.rwu.edu/sites/default/files/downloads/fcas/mns/calculating_and_displaying_regression_statistics_in_excel.pdf https://supervision.bamentorship.net/wp-content/uploads/sites/2/2019/06/Reversal-Design-Comprehensive-Tutorial-1.pdf" "https://upcommons.upc.edu/bitstream/handle/2117/79915/Design+and+development+of+an+app+for+statistical+data+analysis+learning.pdf?sequence=1" "https://training.cochrane.org/sites/training.cochrane.org/files/public/uploads/resources/downloadable_resources/English/RevMan_5.3_User_Guide.pdf"


../../platform/win32/bin/Debug-Unicode-64bit-x64/curl.exe --sanitize-with-extreme-prejudice --location --create-dirs --ftp-create-dirs --verbose --no-progress-bar  --insecure --no-clobber --remote-name-all --remote-header-name --output-dir ~/Downloads/pdfs_test2     https://dl.acm.org/doi/abs/10.1145/3532342.3532351

../../platform/win32/bin/Debug-Unicode-64bit-x64/curl.exe --verbose --output-dir ~/Downloads/pdfs_test2 --create-dirs --output-path-mimics-url --sanitize-with-extreme-prejudice -L --remote-name --remote-header-name  --url  "https://groups.google.com/g/tesseract-dev/c/oEHTjhpdRmo"
```











##### Item ♯00003





```
curl: archive.org page which should produce a PDF:
- https://web.archive.org/web/20060918190436/http://www.stern.nyu.edu/fin/workpapers/papers2002/pdf/wpa02041.pdf
```











##### Item ♯00004





```
JavaScript URL from google search engine:
https://www.google.be/url?sa=t&rct=j&q=&esrc=s&source=web&cd=&cad=rja&uact=8&ved=2ahUKEwisjvfXv9_8AhUrgv0HHSCFD7k4HhAWegQIARAB&url=https%3A%2F%2Fresearch-repository.griffith.edu.au%2Fbitstream%2Fhandle%2F10072%2F368179%2FMandal_2017_01Thesis.pdf%3FisAllowed%3Dy%26sequence%3D1&usg=AOvVaw0Rf6ulPFX8BueVLbgtNrVm
-->
'https://research-repository.griffith.edu.au/bitstream/handle/10072/368179/Mandal_2017_01Thesis.pdf?isAllowed=y&sequence=1'

but cURL doesn't yet extract that kind of info...
```











##### Item ♯00005





```
https://www.ahajournals.org/doi/epdf/10.1161/CIRCULATIONAHA.105.594929
--> epub+pdf viewer in HTML
```











##### Item ♯00006





```
not handled:
https://www.ncbi.nlm.nih.gov/pmc/articles/PMC6640864/
--> when you click on the grey pdf icon in top/left, you get a PDF url, but gUrl cannot resolve that URL to a PDF. Probably some JS indirection again in there, but we'll have to check...


ditto:
https://www.ncbi.nlm.nih.gov/pmc/articles/PMC8719728/
```











##### Item ♯00007





```
403 Forbidden for cURL???
https://www.irjet.net/archives/V7/i3/IRJET-V7I31065.pdf
```











##### Item ♯00008





```
URL which delivers an empty filename, but COULD be used to produce a decent filename if we pick the query part:
https://di.ku.dk/Ansatte/?pure=files/38552211/00761342.pdf
```











##### Item ♯00009





```
troublesome for cURL:
https://ieeexplore.ieee.org/stamp/stamp.jsp?tp=&arnumber=4310076

https://web.archive.org/web/20200901065104/https://arxiv.org/ftp/arxiv/papers/1512/1512.03706.pdf
(^^^^ correcte URL! werkt in Chrome, niet in cURL! ^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^)
```











##### Item ♯00010





```
https://apps.dtic.mil/sti/pdfs/ADA464347.pdf
```











##### Item ♯00011





```
curl --sanitize-with-extreme-prejudice --location --create-dirs --ftp-create-dirs --verbose --verbose --no-progress-bar  --insecure --no-clobber --remote-name-all --remote-header-name --output-dir ~/Downloads/pdfs_test2   --url  "http://www.eea-journal.ro/files/0/art%2005_3-2022-p047.pdf"
```











##### Item ♯00012





```
https://uom.lk/sites/default/files/elect/files/HV_Chap5.pdf
^^^ browser chokes on this one; curl does fine?
```











##### Item ♯00013





```
https://www.bu.edu.eg/portal/uploads/Engineering,%20Shoubra/Electrical%20Engineering/3103/crs-8705/Files/BREAKDOWN%20in%20SOLID%20DIELECTRICS1.pdf
^^^ sloooow site?
```











##### Item ♯00014





```
-------------------------------

mudraw SLOW:

 for f in D:/*.pdf ; do bin/Debug-Unicode-64bit-x64/mutool.exe draw -o "S:/OCR-test-corpus/d/$( basename "$f" ).###.webp" -v -s mtf -r 300 -J 9 "$f" '*' ; done
 ^^^^^^^ for the Microbiology for Cooks pdf in there. 100-400 SECONDS per page!
 (looks like the upscaling of the images is doing us in....)
 
 + if we can find a way to threadpool *page renders* across multi-core, i.e. parallellizing the individual pages 'render' action, NOT banding as currently offered by mudraw, 
   then we might get to the end that much faster as that kind of parallellizing would also work for internally-single-threaded image formats, such as WEBP.
```











##### Item ♯00015





```
page which provides PDF but on Ctrl+S save in browser spits out an HTML:
https://link.springer.com/epdf/10.1134/S1995080219050056?author_access_token=OoI6fs-VkXzk3NdCb2kiOkckSORA_DxfnEvY7GoQybalGXB5wga1vJjm8SZxhVhKdewKNQwizToP1i_OHXgaymY9AkWBd4N53vskP6rqKrdUTALWJR3VaeFW4nLRgfUQk908r05gJNmJrdzpoN6gRQ%3D%3D
```











##### Item ♯00016





```
another page (archive.org) which should deliver a PDF:
https://web.archive.org/web/20190226172352/http://pdfs.semanticscholar.org/708f/8e0a95ba5977072651c0681f3c7b8f09eca3.pdf
```











##### Item ♯00017





```
-----------------------------------------------------------------------------------------------------------------

dubbele punt in path: Unicode:  ：
```











##### Item ♯00018





```
RFC4122
```











##### Item ♯00019





```
https://github.com/clovaai/deep-text-recognition-benchmark/issues/13 ::

@klarajanouskova Hello,

No, I did not train with this model on multi-language dataset.

The character that you mentioned is '몲'
Firstly, I used [UNK] token for the unknown character, which is commented out here
(since we didn't use [UNK] token in the paper experiments).

Secondly, I just replace [UNK] token with '몲', because [UNK] is counted as 5 characters.
In other words, '몲' is just the result of simple post-processing to count [UNK] as 1 character.
Thus, instead of '몲', the other characters such as '1' or 'a' or 'b' would be also possible,
but for the strict evaluation, I wanted the character which is not in opt.character, so I used '몲'.
('몲' is Korean, I shortened the '모르겠음' = 'don't know' as '몲')

Best
```











##### Item ♯00020





```
MUTOOL info -o "J:/__bulktest/______/_/Y_/Qiqqa/Qiqqa-Test-DrvE/evil-base/__bulktest/TextExtractFiles-T1/Sample-PDFs-for-format-testing/Qiqqa Tools - test collection/56831E9293475B87B10CB53E84AAD11B8455397/FULL-DOC.info.txt" -F -I -M -P -S -X "Sample-PDFs-for-format-testing/Qiqqa Tools - test collection/56831E9293475B87B10CB53E84AAD11B8455397.pdf"
MUTOOL draw -o J:/__bulktest/______/_/Y_/Qiqqa/Qiqqa-Test-DrvE/evil-base/__bulktest/TextExtractFiles-T1/digitalcorpora.org/govdocs1/004/004121/FULL-DOC-x300-lwdclip.html -s mtf -r 300 -x preserve-ligatures,preserve-spans,preserve-whitespace,preserve-images,reference-images,reuse-images,mediabox-clip=yes -j everything digitalcorpora.org/govdocs1/004/004121.pdf
MUTOOL draw -o J:/__bulktest/______/_/Y_/Qiqqa/Qiqqa-Test-DrvE/evil-base/__bulktest/TextExtractFiles-T1/digitalcorpora.org/govdocs1/027/027344/FULL-DOC-x300.ps -s mtf -r 300 -y l digitalcorpora.org/govdocs1/027/027344.pdf
MUTOOL draw -o "J:/__bulktest/______/_/Y_/Qiqqa/Qiqqa-Test-DrvE/evil-base/__bulktest/TextExtractFiles-T1/Sample-PDFs-for-format-testing/testset misc 4/BA9D5F2A69C7DE1A2A1E3C8EB5B4325EDC5DE3/FULL-DOC-x300-lwdclip.xhtml" -s mtf -r 300 -x preserve-ligatures,preserve-spans,preserve-whitespace,preserve-images,reference-images,reuse-images,mediabox-clip=yes -j everything "Sample-PDFs-for-format-testing/testset misc 4/BA9D5F2A69C7DE1A2A1E3C8EB5B4325EDC5DE3.pdf"
MUTOOL draw -o "J:/__bulktest/______/_/Y_/Qiqqa/Qiqqa-Test-DrvE/evil-base/__bulktest/TextExtractFiles-T1/Sample-PDFs-for-format-testing/testset misc 4/BA9D5F2A69C7DE1A2A1E3C8EB5B4325EDC5DE3/FULL-DOC-x300.png" -s mtf -r 300 -y l -T 3 -P -B 50 "Sample-PDFs-for-format-testing/testset misc 4/BA9D5F2A69C7DE1A2A1E3C8EB5B4325EDC5DE3.pdf"
```











##### Item ♯00021





```
gitlab-rjotwani
Fetching gitlab-robinwatts
Fetching gitlab-skal65535
Fetching gitlab_angelmixu
Fetching gitlab_anilkoyuncu
Fetching gitlab_attilaolah
Fetching gitlab_boxerab
fatal: Cannot prompt because user interactivity has been disabled.
remote: HTTP Basic: Access denied
fatal: Authentication failed for 'https://gitlab.com/boxerab/libtiff.git/'
error: could not fetch gitlab_boxerab
Fetching gitlab_coolbugcheckers
Fetching gitlab_cygwin
Fetching gitlab_datalogics
Fetching gitlab_datalogics_robb
Fetching gitlab_derdakon
Fetching gitlab_er_akon
```











##### Item ♯00022





```
→
U+2192
Black Right-Pointing Triangle
U+25B6
➔
U+2794
➜
U+279C
➝
U+279D
➞
U+279E
🠖
U+1F816
🠒
U+1F812
☛
U+261B
Black Rightwards Arrow
U+27A1
➟
U+279F
➠
U+27A0
➢
U+27A2
➣
U+27A3
➥
U+27A5
➦
U+27A6
↛
U+219B
↝
U+219D
➤
U+27A4
↠
U+21A0
↣
U+21A3
↦
U+21A6
↬
U+21AC
⇀
U+21C0
⇁
U+21C1
⇒
U+21D2
⇏
U+21CF
⇛
U+21DB
⇝
U+21DD
⇢
U+21E2
⇥
U+21E5
⇨
U+21E8
⇰
U+21F0
⇴
U+21F4
⇉
U+21C9
⇶
U+21F6
⇸
U+21F8
⇻
U+21FB
⇾
U+21FE
➧
U+27A7
➨
U+27A8
➩
U+27A9
➪
U+27AA
➫
U+27AB
➬
U+27AC
➭
U+27AD
➮
U+27AE
➯
U+27AF
➱
U+27B1
➲
U+27B2
➳
U+27B3
➵
U+27B5
➸
U+27B8
➺
U+27BA
➻
U+27BB
➼
U+27BC
➽
U+27BD
➾
U+27BE
⍄
U+2344
⍈
U+2348
⍆
U+2346
➙
U+2799
➛
U+279B
>
U+003E
˃
U+02C3
ᐅ
U+1405
ᐉ
U+1409
ᐳ
U+1433
ᗒ
U+15D2
⃕
U+20D5
⃗
U+20D7
»
U+00BB
›
U+203A
❯
U+276F
❱
U+2771
˲
U+02F2
Left Arrows
←
U+2190
Black Left-Pointing Triangle
U+25C0
Leftwards Black Arrow
U+2B05
🠔
U+1F814
↚
U+219A
🠐
U+1F810
☚
U+261A
↜
U+219C
↞
U+219E
↢
U+21A2
↤
U+21A4
↫
U+21AB
↼
U+21BC
↽
U+21BD
⇐
U+21D0
⇍
U+21CD
⇚
U+21DA
⇇
U+21C7
⇜
U+21DC
⇠
U+21E0
⇤
U+21E4
⇦
U+21E6
⇷
U+21F7
⇺
U+21FA
⇽
U+21FD
<
U+003C
˂
U+02C2
ᐊ
U+140A
ᐸ
U+1438
ᑉ
U+1449
ᗕ
U+15D5
⃖
U+20D6
«
U+00AB
‹
U+2039
❮
U+276E
❰
U+2770
˱
U+02F1
Up Arrows
↑
U+2191
▲
U+25B2
Upwards Black Arrow
U+2B06
🠕
U+1F815
🠑
U+1F811
White Up Pointing Index
U+261D
↟
U+219F
↥
U+21A5
↾
U+21BE
↿
U+21BF
⇑
U+21D1
⇞
U+21DE
⇡
U+21E1
⇪
U+21EA
⇧
U+21E7
⇫
U+21EB
⇬
U+21EC
⇭
U+21ED
⇮
U+21EE
⇯
U+21EF
⇈
U+21C8
^
U+005E
˄
U+02C4
ˆ
U+02C6
˰
U+02F0
̑
U+0311
̭
U+032D
ᐃ
U+1403
ᐞ
U+141E
ᐱ
U+1431
ᗑ
U+15D1
ᛣ
U+16E3
Down Arrows
↓
U+2193
▼
U+25BC
Downwards Black Arrow
U+2B07
🠗
U+1F817
🠓
U+1F813
☟
U+261F
↡
U+21A1
↧
U+21A7
⇂
U+21C2
⇃
U+21C3
⇓
U+21D3
⇟
U+21DF
⇣
U+21E3
⇩
U+21E9
⇊
U+21CA
⍗
U+2357
⍌
U+234C
⍔
U+2354
⍖
U+2356
˅
U+02C5
ˇ
U+02C7
ˬ
U+02EC
̬
U+032C
ᐁ
U+1401
ᐯ
U+142F
ᗐ
U+15D0
Left Right Arrows
Left Right Arrow
U+2194
↭
U+21AD
↮
U+21AE
↹
U+21B9
⇄
U+21C4
⇆
U+21C6
⇿
U+21FF
⬌
U+2B0C
⇔
U+21D4
⇌
U+21CC
⇹
U+21F9
⇋
U+21CB
⇼
U+21FC
Up Down Arrows
Up Down Arrow
U+2195
↨
U+21A8
⇅
U+21C5
⇕
U+21D5
⇳
U+21F3
⬍
U+2B0D
⇵
U+21F5
Diagonal Arrows
North West Arrow
U+2196
North East Arrow
U+2197
South East Arrow
U+2198
South West Arrow
U+2199
⇖
U+21D6
⇗
U+21D7
⇘
U+21D8
⇙
U+21D9
⇱
U+21F1
⇲
U+21F2
➶
U+27B6
➴
U+27B4
➷
U+27B7
➹
U+27B9
➘
U+2798
➚
U+279A
⬈
U+2B08
⬉
U+2B09
⬊
U+2B0A
⬋
U+2B0B
Heavy Arrows
🠸
U+1F838
🠺
U+1F83A
🠻
U+1F83B
🠹
U+1F839
🡄
U+1F844
🡆
U+1F846
🡅
U+1F845
🡇
U+1F847
🡸
U+1F878
🡺
U+1F87A
🡹
U+1F879
🡻
U+1F87B
🡼
U+1F87C
🡽
U+1F87D
🡾
U+1F87E
🡿
U+1F87F
🢀
U+1F880
🢂
U+1F882
🢁
U+1F881
🢃
U+1F883
🢄
U+1F884
🢅
U+1F885
🢆
U+1F886
🢇
U+1F887
➨
U+27A8
➜
U+279C
➽
U+27BD
❰
U+2770
❱
U+2771
Heavy Compressed Arrows
🠼
U+1F83C
🠾
U+1F83E
🠽
U+1F83D
🠿
U+1F83F
🡀
U+1F840
🡂
U+1F842
🡁
U+1F841
🡃
U+1F843
Curved Arrows
⮨
U+2BA8
⮩
U+2BA9
⮪
U+2BAA
⮫
U+2BAB
➥
U+27A5
➦
U+27A6
⮬
U+2BAC
⮭
U+2BAD
⮮
U+2BAE
⮯
U+2BAF
Shadowed Arrows
🢠
U+1F8A0
🢡
U+1F8A1
🢢
U+1F8A2
🢣
U+1F8A3
🢤
U+1F8A4
🢥
U+1F8A5
🢦
U+1F8A6
🢧
U+1F8A7
🢨
U+1F8A8
🢩
U+1F8A9
🢪
U+1F8AA
🢫
U+1F8AB
➩
U+27A9
➪
U+27AA
➫
U+27AB
➬
U+27AC
➭
U+27AD
➮
U+27AE
➯
U+27AF
➱
U+27B1
Arrow to/from Bar
⭰
U+2B70
⭲
U+2B72
⭱
U+2B71
⭳
U+2B73
⭶
U+2B76
⭷
U+2B77
⭸
U+2B78
⭹
U+2B79
⇤
U+21E4
⇥
U+21E5
⤒
U+2912
⤓
U+2913
↨
U+21A8
⤝
U+291D
⤞
U+291E
⤟
U+291F
⤠
U+2920
↤
U+21A4
↦
U+21A6
↥
U+21A5
↧
U+21A7
⬶
U+2B36
⤅
U+2905
⟻
U+27FB
⟼
U+27FC
↸
U+21B8
⇱
U+21F1
⇲
U+21F2
Navigation Arrows
🡠
U+1F860
🡢
U+1F862
🡡
U+1F861
🡣
U+1F863
🡤
U+1F864
🡥
U+1F865
🡦
U+1F866
🡧
U+1F867
🡨
U+1F868
🡪
U+1F86A
🡩
U+1F869
🡫
U+1F86B
🡬
U+1F86C
🡭
U+1F86D
🡮
U+1F86E
🡯
U+1F86F
🡰
U+1F870
🡲
U+1F872
🡱
U+1F871
🡳
U+1F873
🡴
U+1F874
🡵
U+1F875
🡶
U+1F876
🡷
U+1F877
🡸
U+1F878
🡺
U+1F87A
🡹
U+1F879
🡻
U+1F87B
🡼
U+1F87C
🡽
U+1F87D
🡾
U+1F87E
🡿
U+1F87F
🢀
U+1F880
🢂
U+1F882
🢁
U+1F881
🢃
U+1F883
🢄
U+1F884
🢅
U+1F885
🢆
U+1F886
🢇
U+1F887
Hand Pointing Index
☚
U+261A
☞
U+261E
White Up Pointing Index
U+261D
☟
U+261F
☛
U+261B
90 Degree Arrows
↳
U+21B3
↲
U+21B2
↰
U+21B0
↱
U+21B1
↵
U+21B5
↴
U+21B4
☇
U+2607
Circle Circular Arrows
↺
U+21BA
↻
U+21BB
⟲
U+27F2
⟳
U+27F3
⭯
U+2B6F
⭮
U+2B6E
↺
U+21BA
↻
U+21BB
⥀
U+2940
⥁
U+2941
↶
U+21B6
↷
U+21B7
⮌
U+2B8C
⮍
U+2B8D
⮎
U+2B8E
⮏
U+2B8F
⤻
U+293B
⤸
U+2938
⤾
U+293E
⤿
U+293F
⤺
U+293A
⤼
U+293C
⤽
U+293D
⤹
U+2939
🗘
U+1F5D8
⮔
U+2B94
Clockwise Rightwards and Leftwards Open Circle Arrows
U+1F501
Clockwise Rightwards and Leftwards Open Circle Arrows with Circled One Overlay
U+1F502
Clockwise Downwards and Upwards Open Circle Arrows
U+1F503
Anticlockwise Downwards and Upwards Open Circle Arrows
U+1F504
Arrow Pointing Rightwards Then Curving Upwards
U+2934
Arrow Pointing Rightwards Then Curving Downwards
U+2935
⤶
U+2936
⤷
U+2937
Rightwards Arrow with Hook
U+21AA
Leftwards Arrow with Hook
U+21A9
⃕
U+20D5
Circled Arrows
⮈
U+2B88
⮊
U+2B8A
⮉
U+2B89
⮋
U+2B8B
➲
U+27B2
Ribbon Arrows
⮰
U+2BB0
⮱
U+2BB1
⮲
U+2BB2
⮳
U+2BB3
⮴
U+2BB4
⮵
U+2BB5
⮶
U+2BB6
⮷
U+2BB7
Paired Twin Two Arrows
⮄
U+2B84
⮆
U+2B86
⮅
U+2B85
⮇
U+2B87
⇈
U+21C8
⇊
U+21CA
⇇
U+21C7
⇉
U+21C9
⇆
U+21C6
⇄
U+21C4
⇅
U+21C5
⇵
U+21F5
⮀
U+2B80
⮂
U+2B82
⮁
U+2B81
⮃
U+2B83
Triple Three Arrows
⇶
U+21F6
⇚
U+21DA
⇛
U+21DB
⤊
U+290A
⟱
U+27F1
Keyboard Arrows
←
U+2190
→
U+2192
↑
U+2191
↓
U+2193
Tab Key
⭾
U+2B7E
⭿
U+2B7F
Bow and Arrows
➶
U+27B6
➴
U+27B4
➷
U+27B7
➹
U+27B9
Waved Arrows
⬿
U+2B3F
⤳
U+2933
↜
U+219C
↝
U+219D
⇜
U+21DC
⇝
U+21DD
⬳
U+2B33
⟿
U+27FF
Harpoon Arrows
↼
U+21BC
⇀
U+21C0
↽
U+21BD
⇁
U+21C1
↿
U+21BF
↾
U+21BE
⇃
U+21C3
⇂
U+21C2
⇋
U+21CB
⇌
U+21CC
Stroked Arrows
⇷
U+21F7
⇸
U+21F8
⇹
U+21F9
⇺
U+21FA
⇻
U+21FB
⇞
U+21DE
⇟
U+21DF
⇼
U+21FC
⭺
U+2B7A
⭼
U+2B7C
⭻
U+2B7B
⭽
U+2B7D
Double Head Arrows
↞
U+219E
↠
U+21A0
↟
U+219F
↡
U+21A1
⯬
U+2BEC
⯭
U+2BED
⯮
U+2BEE
⯯
U+2BEF
Miscellaneous Arrows
↯
U+21AF
☈
U+2608
⥼
U+297C
⥽
U+297D
⥾
U+297E
⥿
U+297F
Arrows Within Triangle Arrowhead
🢔
U+1F894
🢖
U+1F896
🢕
U+1F895
🢗
U+1F897
Arrow Heads
⮜
U+2B9C
⮞
U+2B9E
⮝
U+2B9D
⮟
U+2B9F
➤
U+27A4
⮘
U+2B98
⮚
U+2B9A
⮙
U+2B99
⮛
U+2B9B
➢
U+27A2
➣
U+27A3
Black Right-Pointing Triangle
U+25B6
➤
U+27A4
▲
U+25B2
🢐
U+1F890
🢒
U+1F892
🢑
U+1F891
🢓
U+1F893
⌃
U+2303
⌄
U+2304
Arrow Shafts
■
U+25A0
□
U+25A1
🞑
U+1F791
🞒
U+1F792
🞓
U+1F793
▦
U+25A6
▤
U+25A4
⧈
U+29C8
▨
U+25A8
▧
U+25A7
🢜
U+1F89C
🢝
U+1F89D
🢞
U+1F89E
🢟
U+1F89F
🢬
U+1F8AC
🢭
U+1F8AD
Fedex Logo Arrow
🡆
U+1F846
```











##### Item ♯00023





```
----------------------------------------------------------------------------------------------------

Triggered via pull request 2 minutes ago
@GerHobbeltGerHobbelt
synchronize
#6
GerHobbelt:master
Status
Failure
Total duration
53s
Artifacts
–


 
Annotations
1 error and 2 warnings
build
Process completed with exit code 1.

build
Node.js 12 actions are deprecated. For more information see: https://github.blog/changelog/2022-09-22-github-actions-all-actions-will-begin-running-on-node16-instead-of-node12/. Please update the following actions to use Node.js 16: microsoft/setup-msbuild

build
The `set-output` command is deprecated and will be disabled soon. Please upgrade to using Environment Files. For more information see: https://github.blog/changelog/2022-10-11-github-actions-deprecating-save-state-and-set-output-commands/
```











##### Item ♯00024





```
------------------------------------------------------------------------------------------------------------

b0rk:

mutool:

metadump -m 1 -o -  "Y:\Qiqqa\ZZZ (new)\base\INTRANET_42CD1F03-5AAA-4A28-9B03-768D6FE15EFA\documents\B\B7F56D71C2AA8980553236FA236CAB17EF537B.pdf"

--> b0rked JSON that results in a parse error about CCITTwhatever.
```











##### Item ♯00025





```
mutool:
metadump -m 1 -o -  "Y:\Qiqqa\ZZZ (new)\base\INTRANET_42CD1F03-5AAA-4A28-9B03-768D6FE15EFA\documents\1\181B47B0831E9935C678BB2F37CBF43DB4F5A570.pdf"

-->

Newtonsoft.Json.JsonSerializationException
  HResult=0x80131500
  Message=Error converting value "CCITTFax" to type 'Utilities.PDF.MuPDF.MultiPurpImageFilter'. Path '[0].PageInfo[88].Images[0].ImageFilter', line 8246, position 27.
  Source=Newtonsoft.Json
  StackTrace:
   at Newtonsoft.Json.Serialization.JsonSerializerInternalReader.EnsureType(JsonReader reader, Object value, CultureInfo culture, JsonContract contract, Type targetType)
   at Newtonsoft.Json.Serialization.JsonSerializerInternalReader.CreateValueInternal(JsonReader reader, Type objectType, JsonContract contract, JsonProperty member, JsonContainerContract containerContract, JsonProperty containerMember, Object existingValue)
   at Newtonsoft.Json.Serialization.JsonSerializerInternalReader.SetPropertyValue(JsonProperty property, JsonConverter propertyConverter, JsonContainerContract containerContract, JsonProperty containerProperty, JsonReader reader, Object target)
   at Newtonsoft.Json.Serialization.JsonSerializerInternalReader.PopulateObject(Object newObject, JsonReader reader, JsonObjectContract contract, JsonProperty member, String id)

  This exception was originally thrown at this call stack:
    Newtonsoft.Json.Utilities.ConvertUtils.EnsureTypeAssignable(object, System.Type, System.Type)
    Newtonsoft.Json.Utilities.ConvertUtils.ConvertOrCast(object, System.Globalization.CultureInfo, System.Type)
    Newtonsoft.Json.Serialization.JsonSerializerInternalReader.EnsureType(Newtonsoft.Json.JsonReader, object, System.Globalization.CultureInfo, Newtonsoft.Json.Serialization.JsonContract, System.Type)

Inner Exception 1:
ArgumentException: Could not cast or convert from System.String to Utilities.PDF.MuPDF.MultiPurpImageFilter.
```


