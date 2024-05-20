# Testing :: Nasty URLs for PDFs

> **Note**: Also check these for more PDF download/fetching woes:
>
> - [[../curl - command-line and notes|curl - command-line and notes]] (sections about *nasty PDFs*)
> - [[PDF cannot be Saved.As in browser (Microsoft Edge)|PDF cannot be 'Saved As' in browser (Microsoft Edge)]]
> - [[Testing - PDF URLs with problems]]
> - [[MuPDF testing notes - particular PDFs]]
> - [[../../../Progress in Development/Testing & Evaluating/PDF bulktest test run notes at the bleeding edge|PDF bulktest test run notes at the bleeding edge]]
> 
 
- http://dspace.bracu.ac.bd/xmlui/bitstream/handle/10361/7620/12201001%20&%2013101230_CSE.pdf?sequence=1

  This link downloads/views a PDF fine in MS Edge / Chrome, but produces a HTML/404 crash page when using `curl` as in `curl -v -L http://dspace.bracu.ac.bd/xmlui/bitstream/handle/10361/7620/12201001%20&%2013101230_CSE.pdf?sequence=1 -o test.pdf`

- https://www.aaai.org/Papers/Workshops/2006/WS-06-06/WS06-06-006.pdf

  Downloads OK in the browser, but `curl -v -L` outputs this:

  ```
  < HTTP/1.1 406 Not Acceptable
  < Date: Thu, 01 Oct 2020 21:00:39 GMT
  < Server: Apache
  < Content-Length: 373
  < Content-Type: text/html; charset=iso-8859-1
  ```

  + and another one: https://www.aaai.org/Papers/Workshops/2006/WS-06-06/WS06-06-003.pdf
  + https://www.aaai.org/Papers/Workshops/2006/WS-06-09/WS06-09-006.pdf
  
- https://aaaipress.org/Papers/Workshops/1998/WS-98-07/WS98-07-015.pdf

  Connection Not Secure (not tested with `curl` though; this is a browser report)

- http://en.saif.sjtu.edu.cn/junpan/Peso_RFS.pdf

  Weird DNS shit happening. Bing and Google know this one, browser cannot reach.
  
- https://onlinelibrary.wiley.com/doi/pdf/10.1002/humu.22848

  Renders as PDF in browser, but 'Save As' produces the HTML. Only clicking on the content and then hitting right-click menu -> Save As will produce the PDF.

  https://asistdl.onlinelibrary.wiley.com/doi/full/10.1002/asi.24082 : ditto
  
  https://onlinelibrary.wiley.com/doi/pdf/10.1002/acp.2995 : ditto
  
  https://onlinelibrary.wiley.com/doi/pdf/10.1002/ejsp.2331 : ditto. Hm, must be something smelly in that HTML...
  
- http://www.insightsociety.org/ojaseit/index.php/ijaseit/article/view/6566

  The small link to the PDF is not the PDF itself but delivers a page, which embeds a PDF in its HTML surroundings.
  
- https://pure.tugraz.at/ws/portalfiles/portal/1705957/Text%2520preprocessing%2520for%2520Opinion%2520Mining.pdf

  clearly has an incorrect mimetype as the PDF is loaded in the browser view as if it were plaintext. `%PDF-1.6
%¡³Å×
1 0 obj`...

  And another one: https://pure.tugraz.at/ws/portalfiles/portal/1333710/Opinion_Mining_Noisy_Text_Information%20Processing%20and%20Management.pdf
  
- and, of course, there's always the b0rked/expired SSL certificate abound, e.g.: https://www.cse.unsw.com/~lxue/paper/sigmod10demo.pdf

  This one: http://www.cedar.buffalo.edu/~srihari/papers/SPIE-2007-lineSeg.pdf doesn't want to cooperate, unless fed through `wget`. (Via https://www.semanticscholar.org/paper/A-statistical-approach-to-line-segmentation-in-Arivazhagan-Srinivasan/48fc23665dfecdd4ecef6eb7f24b24076f194d6a)
  
  + https://web-info8.informatik.rwth-aachen.de/media/papers/MDLSTM_final.pdf
  + https://mainline.brynmawr.edu/Courses/cs380/fall2006/intro_to_LSA.pdf
  + 

- https://ieeexplore.ieee.org/stamp/stamp.jsp?arnumber=9091126

  shows "Open" button for PDF download.
  
- oddly enough some semantic scholar pages don't seem to properly download their referenced PDFs, e.g.: https://www.semanticscholar.org/paper/Line-And-Word-Segmentation-of-Handwritten-Documents-Louloudis-Gatos/75c3c85480be7781b2a99d7d586870394281fbc7
	- https://www.semanticscholar.org/paper/Transition-pixel%3A-A-concept-for-binarization-based-Ram%C3%ADrez-Orteg%C3%B3n-Tapia/1e8e7e992cc1a930623d7aac1c46fc23df2cae1f
	- https://www.semanticscholar.org/paper/Adaptive-degraded-document-image-binarization-Gatos-Pratikakis/aa26d72a007aa6e55db7db8f23a5861f49b729ad
	- https://www.semanticscholar.org/paper/Separating-text-and-background-in-degraded-document-Leedham-Varma/e933ad6f822a46fb5f92a2a33915dec9896f98a0
	- https://www.semanticscholar.org/paper/Binarising-camera-images-for-OCR-Seeger-Dance/66a1177c31caf2bcc00af7dc451fd9de1310235a
	-


- FTP URI which must be surrounded by quotes or `bash` will nuke it due to the braces: ftp://nozdr.ru/biblio/kolxoz/M/MV/MVsa/Fink%20G.A.%20Markov%20models%20for%20pattern%20recognition%20(Springer,%202008)(ISBN%209783540717669)(256s)_MVsa_.pdf
- [On Prediction Using Variable Order Markov Models (aaai.org)](https://www.aaai.org/Papers/JAIR/Vol22/JAIR-2212.pdf) - `wget` spits out error 403 (Forbidden) while Chrome/MSEdge does show the PDF on screen.
- [citeseerx.ist.psu.edu/viewdoc/download?doi=10.1.1.589.5772&rep=rep1&type=pdf](http://citeseerx.ist.psu.edu/viewdoc/download?doi=10.1.1.589.5772&rep=rep1&type=pdf) -- `wget` locks up?? MSEdge does ok, however...
- https://www.microsoft.com/en-us/research/wp-content/uploads/2013/12/IEEE-ASRU-2013.pdf -- Error 403 in `wget`, view okay on MSEdge.
- https://www.aaai.org/ocs/index.php/AAAI/AAAI15/paper/viewFile/9387/9219 -- `wget` says Error 403 Forbidden, MSEdge is okay though.
- https://www.microsoft.com/en-us/research/wp-content/uploads/2012/01/tricks-2012.pdf -- ditto.
- https://docs.lib.purdue.edu/cgi/viewcontent.cgi?article=2782&context=cstech -- ditto.
- https://www.ncbi.nlm.nih.gov/pmc/articles/PMC8253764/pdf/41598_2021_Article_92992.pdf -- ditto.
- 


- https://repository.upenn.edu/cgi/viewcontent.cgi?article=3656&context=edissertations -- Error 400 Bad Request in `wget` but OK with MSEdge.
- https://www.cell.com/action/showPdf?pii=S0002-9297%2807%2963913-5 -- `wget` goes bonkers and MSEdge just gives us the PDF
- https://journals.plos.org/plosone/article/file?id=10.1371/journal.pone.0009506&type=printable -- 404 vs MSEdge okay
- [Back‐propagation neural network on Markov chains from system call sequences: a new approach for detecting Android malware with system call sequences (wiley.com)](https://ietresearch.onlinelibrary.wiley.com/doi/epdf/10.1049/iet-ifs.2015.0211)
- [MCMC-ODPR: Primer design optimization using Markov Chain Monte Carlo sampling (nih.gov)](https://www.ncbi.nlm.nih.gov/pmc/articles/PMC3561117/pdf/1471-2105-13-287.pdf/?tool=EBI) -- 403 vs. MSEdge ok.
- https://www.aaai.org/Papers/AAAI/2002/AAAI02-081.pdf
- [Probabilistic Tree-Edit Models with Structured Latent Variables for Textual Entailment and Question Answering (psu.edu)](https://citeseerx.ist.psu.edu/viewdoc/download?doi=10.1.1.231.7244&rep=rep1&type=pdf)
- [A Distance Measure for Bounding Probabilistic Belief Change (aaai.org)](https://www.aaai.org/Papers/AAAI/2002/AAAI02-081.pdf)
- 



* [Geigel (2013) [MTD, Barreno] Neural Network Trojan .pdf | DocDroid](https://www.docdroid.net/POLhybl/geigel-2013-mtd-barreno-neural-network-trojan-pdf) -- shows okay in MSEdge but is a HTML page wrapper when you hit CTRL+S key combo. MUST use the download button to get the PDF.
* [Back‐propagation neural network on Markov chains from system call sequences: a new approach for detecting Android malware with system call sequences (wiley.com)](https://ietresearch.onlinelibrary.wiley.com/doi/epdf/10.1049/iet-ifs.2015.0211) -- ditto.
* [Simulating a Skilled Typist: A Study of Skilled Cognitive‐Motor Performance (wiley.com)](https://onlinelibrary.wiley.com/doi/epdf/10.1207/s15516709cog0601_1)
* [Probabilistic Metric Spaces for Privacy by Design Machine Learning Algorithms : Modeling Database Changes - CORE Reader](https://core.ac.uk/reader/189883540)
* [Markov Chain‐Based Stochastic Modeling of Deep Signal Fading: Availability Assessment of Dual‐Frequency GNSS‐Based Aviation Under Ionospheric Scintillation (wiley.com)](https://agupubs.onlinelibrary.wiley.com/doi/epdf/10.1029/2020SW002655)
* [IEEE Xplore Full-Text PDF:](https://ieeexplore.ieee.org/stamp/stamp.jsp?arnumber=9245495)
* 


- [Efficient model similarity estimation with robust hashing (springer.com)](https://link.springer.com/epdf/10.1007/s10270-021-00915-9?sharing_token=v3msqOoVG1eMivJhmkW9VPe4RwlQNchNByi7wbcMAY4Tdobx2s0Z3m0cfSzm-zi7V-o745n34P_BRUDph0bTNJWQE04bVuKG3zolOy31qWgO0ZvnwjiGiAl-UBau7-o9FMZmp_jk4DGk3gcNPtoKofvpVUQnr--LEwcVrlteemc%3D) -- quacks like a PDF but IS NOT. Here, there's NO PDF TO BE HAD.
- 


- [Enhanced K-means Clustering Technique based Copy-Move Image Forgery Detection - ProQuest](https://www.proquest.com/openview/3da7e5d590079e3b095d17c8634e756b/1.pdf?pq-origsite=gscholar&cbl=2045096) - clicking the big DOWNLOAD button fails with 403 error, while clicking the tiny disk icon in the viewer part of the page does produce the PDF
- [Does semantic knowledge influence event segmentation and recall of text? - ProQuest](https://www.proquest.com/openview/d46f3888619a081c134c7efca9cb31ec/1?pq-origsite=gscholar&cbl=976351) -- ditto.
- 

- [IEEE Xplore Full-Text PDF:](https://ieeexplore.ieee.org/stamp/stamp.jsp?arnumber=8673945) - another Ctrl+S --> HTML while click on disk icon gives PDF page.
- https://www.google.com/url?sa=t&rct=j&q=&esrc=s&source=web&cd=&cad=rja&uact=8&ved=2ahUKEwj-qOSQwOb1AhWFPOwKHQodAMMQFnoFCOQBEAE&url=https%3A%2F%2Fwww.mdpi.com%2F2073-8994%2F10%2F12%2F706%2Fpdf&usg=AOvVaw0zLUBDx6gTxMc_kTNoPLr0 - google links like these download fine in MSEdge but b0rk with Bad Request in `wget`.
- https://www.google.com/url?sa=t&rct=j&q=&esrc=s&source=web&cd=&cad=rja&uact=8&ved=2ahUKEwj-qOSQwOb1AhWFPOwKHQodAMMQFnoFCOEBEAE&url=https%3A%2F%2Fopen.library.ubc.ca%2Fmedia%2Fdownload%2Fpdf%2F24%2F1.0073807%2F1&usg=AOvVaw23nPov3IR-hHu24w6dH3tS -- ditto
- https://www.google.com/url?sa=t&rct=j&q=&esrc=s&source=web&cd=&cad=rja&uact=8&ved=2ahUKEwj-qOSQwOb1AhWFPOwKHQodAMMQFnoECHkQAQ&url=https%3A%2F%2Fopus4.kobv.de%2Fopus4-fau%2Ffiles%2F10952%2FVincentChristleinDissertation.pdf&usg=AOvVaw233MDZbxiltScqmyCrnwJe -- ditto.
- https://www.google.com/url?sa=t&rct=j&q=&esrc=s&source=web&cd=&cad=rja&uact=8&ved=2ahUKEwj_06vzz-b1AhWUt6QKHay8BoQQFnoFCMkBEAE&url=https%3A%2F%2Fwww.atlantis-press.com%2Farticle%2F25868364.pdf&usg=AOvVaw2x6MeSaYlfAUgahnNmlSfD -- ditto.
- https://www.google.com/url?sa=t&rct=j&q=&esrc=s&source=web&cd=&cad=rja&uact=8&ved=2ahUKEwjWjufs1ub1AhUWt6QKHQXHC9wQFnoFCIwBEAE&url=https%3A%2F%2Fwww.mdpi.com%2F2504-2289%2F3%2F2%2F32%2Fpdf&usg=AOvVaw14thc9G_D9Vc96Wio7G_R7 -- ditto.
- [IEEE Xplore Full-Text PDF:](https://ieeexplore.ieee.org/stamp/stamp.jsp?arnumber=8999509) -- ditto.
- [IEEE Xplore Full-Text PDF:](https://ieeexplore.ieee.org/stamp/stamp.jsp?arnumber=9449333) -- ditto.
- [XNDDF: Towards a Framework for Flexible Near-Duplicate Document Detection Using Supervised and Unsupervised Learning (cyberleninka.org)](https://cyberleninka.org/article/n/1376468/viewer) -- ditto.
- 
- https://www.google.com/url?sa=t&rct=j&q=&esrc=s&source=web&cd=&cad=rja&uact=8&ved=2ahUKEwj_06vzz-b1AhWUt6QKHay8BoQQFnoECBkQAQ&url=http%3A%2F%2Ftheinternationaljournal.org%2Fojs%2Findex.php%3Fjournal%3Drjitsm%26op%3Ddownload%26page%3Darticle%26path%255B%255D%3D4982%26path%255B%255D%3Dpdf&usg=AOvVaw2H_wz7xoIlr0J5PP9_CZb0 -- ditto, *but with an additional feature:* this one is downloaded by MSEdge but marked as **unsecure**, requiring the user to pick "Keep" as the option of choice in the top-right context menu. Hence this one isn't easily obtained.
- 


- http://infoscience.epfl.ch/record/99373/files/Henzinger06.pdf -- an example of a PDF URL which results in a 302-Redirect before we obtain the actual data. Nothing fancy, works in both `wget` and MSEdge.

- [PeerJ - 432 Error:](https://peerj.com/articles/cs-389.pdf) - interesting link: produced as-is by a google search, but gives custom 432 Error in MSEdge; after clicking on the **Article** link in there, we get at the page where the PDF may be downloaded. Not tested in `wget`. This is another example of web site behaviour I've suspected for quite a while: they have special access channels for the google bot to get at the PDF content, while others have to go "through channels". This one, at least, isn't hiding behind a paywall.

- [Europe PMC](https://europepmc.org/articles/pmc8490428/bin/41598_2021_98933_moesm1_esm.pdf) -- the web page itself acts **instable** (keeps reloading) in MSEdge. No further problems downloading the PDF via the download icon, but click timing is critical as the HTML page keeps reloading.

- [pdf.aspx (igi-global.com)](https://www.igi-global.com/pdf.aspx?tid=289612&ptid=278101&ctid=4&oa=true&isxn=9781683182085) -- gives a b0rked filename as download target in MSEdge. Not tested with `wget`

- [ir.nuk.edu.tw](https://ir.nuk.edu.tw/ir/bitstream/310360000Q/14907/2/published%20paper.pdf) -- web browser reports this a site with an "unsupported protocol". Regular `wget` is taking ages, but ultimately succeeds in downloading. Here's the verbose output from `curl`:

```
* SSL connection using TLSv1.1 / ECDHE-RSA-AES256-SHA
* ALPN, server did not agree to a protocol
* Server certificate:
*  subject: CN=ir.nuk.edu.tw
*  start date: Dec 22 00:00:00 2020 GMT
*  expire date: Mar 22 23:59:59 2021 GMT
*  issuer: C=AT; O=ZeroSSL; CN=ZeroSSL RSA Domain Secure Site CA
*  SSL certificate verify result: unable to get local issuer certificate (20), continuing anyway.
  0     0    0     0    0     0      0      0 --:--:--  0:00:02 --:--:--     0} [5 bytes data]
> GET /ir/bitstream/310360000Q/14907/2/published%20paper.pdf HTTP/1.1
> Host: ir.nuk.edu.tw
> User-Agent: curl/7.81.0
```

- https://www.dizinot.com/upload/files/2016/12/AOS-AO4606.pdf : this one at least dumps the raw PDF binary content to screen in any browser due to incorrect(?) mimetype setup server-side. Only produces the PDF when done via "save as" popup menu entry in your web browser. Hence we can expect trouble when downloading this one using other tools, such as `curl`.
- https://www.pnas.org/doi/epdf/10.1073/pnas.1708279115 : linux firefox requires popups to be enabled for the PDF to be downloaded.
- https://opengrey.eu/

 * https://www.proceedings.aaai.org/Papers/ICML/2003/ICML03-102.pdf : browser opens this one, after you explicitly accept to visit the site due to expired/wrong SSL certificate, but `wget` barfs with a HTTP 403 (access denied) error! Very strange indeed.
   BTW: `cUrl` spits back HTTP ERROR 406 Not Acceptable.
 
   PDFs with this problem, all from the same site:
   - https://www.proceedings.aaai.org/Papers/ICML/2003/ICML03-000.pdf
   - https://www.proceedings.aaai.org/Papers/ICML/2003/ICML03-001.pdf
   - https://www.proceedings.aaai.org/Papers/ICML/2003/ICML03-002.pdf
   - https://www.proceedings.aaai.org/Papers/ICML/2003/ICML03-011.pdf
   - https://www.proceedings.aaai.org/Papers/ICML/2003/ICML03-102.pdf


- https://www.semanticscholar.org/reader/7dfac524a55f599786d64b9b2725d63e2679f2bf : need to click download in online PDF viewer
- https://www.semanticscholar.org/reader/33eeab272c06c30c6e550e312d0817c2681b5dd8


- https://www.macrothink.org/journal/index.php/ber/article/view/13666 : opens PDF in online viewer, plus separate download link next to the viewer

- https://www.cicling.org/2006/Proceedings/RCS-18-Page151.pdf : b0rks in browser due to certificate issues.

- https://www.tandfonline.com/doi/full/10.1080/17538947.2017.1371253

- https://www.mdpi.com/1099-4300/23/1/31 : Download PDF, etc. are available via dropdown.

- https://www.sciencedirect.com/science/article/pii/S1875389212001435?ref=pdf_download&fr=RR-2&rr=8241e1a149400be4 : the resulting PDF file link apparently has a time-out (discovered when binging on PDFs in a web browser download session) > https://www.sciencedirect.com/science/article/pii/S1875389212001435/pdf?md5=625bed8107bcf34e6adc0f3371811772&pid=1-s2.0-S1875389212001435-main.pdf
- https://ieeexplore.ieee.org/stamp/stamp.jsp?arnumber=8746164 : Ctrl-S -> HTML file but view is PDF?
- https://www.cns.nyu.edu/pub/lcv/portilla03-preprint.pdf- : note the dash at the end: file extension is `.pdf-` instead of `.pdf`, probably a data entry / fat finger mistake server-side.
- https://www.tandfonline.com/doi/epdf/10.1080/09540091.2023.2202363?needAccess=true : Ctrl-S --> HTML, click download button to get PDF
- https://citeseerx.ist.psu.edu/doc/10.1.1.329.6283 : a HTML based PDF viewer wraps the PDF.
- 

- https://link.springer.com/chapter/10.1007/978-3-642-15696-0_46 : **WARNING**: this is one example where this site provides a *preview* PDF for download, at the same location in the page where the regular PDF would be available otherwise. So when automating this stuff, we SHOULD add heuristics to identify potential previews and jump to sci-hub instead, after extracting the DOI from the same page!
- https://link.springer.com/chapter/10.1007/978-3-642-37456-2_14 : ditto!
- 





## HTML pages with problems

* [Irreducible and Aperiodic Markov Chains (uni-ulm.de)](https://www.mathematik.uni-ulm.de/stochastik/lehre/ss06/markov/skript_engl/node12.html) -- expired certificate gets you blocked
* https://136.199.55.186/rec/conf/icassp/SouideneBA06.html
* https://iuks.informatik.tu-muenchen.de/_media/members/steger/publications/2002/isprs-comm-iii-ulrich-steger.pdf : another one that has serious trouble today in Chrome: `curl -o ~/Downloads/isprs-comm-iii-ulrich-steger.pdf https://iuks.informatik.tu-muenchen.de/_media/members/steger/publications/2002/isprs-comm-iii-ulrich-steger.pdf --ssl-no-revoke --insecure` -- didn't work without the `--insecure` there.
* 




## Download / fetch weirdnesses & miscellaneous oddities

- https://epubs.siam.org/doi/epdf/10.1137/1.9781611976472.5

  Linux/firefox b0rks on this one with 'popup blocked' warning. However, the 'popup' is what will allow you to get the PDF, so you'll have allow popups for the given siam.org website.

- Academia.com of course is a nuisance as it always wants you to log in with either google or facebook: one big tracking hazard.

* https://ietresearch.onlinelibrary.wiley.com/doi/epdf/10.1049/iet-epa.2016.0190 : Linux/Firefox report that it blocked popup(s) when you click on the download button in this page to get an actual copy of the PDF: you need to enable popups for the Wiley subdomain for the PDF file to be actually downloaded. See if we can circumvent this with cURL?

- https://link.springer.com/chapter/10.1007/3-540-44888-8_9 : Springer puts a 2-page sample PDF online inside this page so our automaton may stumble over that one and consequently *forget* to o to sci-hub.ru with the DOI listed elsewhere on this page --> we need additional heuristics to detect such 'sample pages' throwing a spanner in the works.
- https://link.springer.com/chapter/10.1007/3-540-12689-9_129?error=cookies_not_supported&code=eb5bf9b4-1766-41db-b982-2b1c591c26bd

- https://www.ncbi.nlm.nih.gov/pmc/articles/PMC7939936/ : some pages are only published as HTML, not PDF. This is one. "Printer Friendly" link gives the cleaned-up HTML paper.

- https://www.nature.com/articles/s41551-018-0304-0.epdf?author_access_token=vSPt7ryUfdSCv4qcyeEuCdRgN0jAjWel9jnR3ZoTv0PdqacSN9qNY_fC0jWkIQUd0L2zaj3bbIQEdrTqCczGWv2brU5rTJPxyss1N4yTIHpnSv5_nBVJoUbvejyvvjrGTb2odwWKT2Bfvl0ExQKhZw%3D%3D : this one has a link to the publisher, where we need to grab the DOI in order to obtain the actual PDF via sci-hub.ru. *sigh*. 
  (publisher's page, by the way, is: https://www.nature.com/articles/s41551-018-0304-0)
  
  💕🔥So this is a **two levels deep** reachability issue right here! We haven't seen that very much, not since I've started sporadically noting the "odd ones" in these Obsidian notes...




## DOI's on the page --> [sci-hub.ru](https://sci-hub.ru/)

- https://www.computer.org/csdl/proceedings-article/csse/2008/3336a718/12OmNyuy9XT : "DOI Bookmark:", not just "DOI:" like most of the others do...
- https://epubs.siam.org/doi/10.1137/S0097539794264810
- https://onlinelibrary.wiley.com/doi/10.1002/asi.21063
- 

- https://doi.org/10.1007/978-1-4471-1597-7_33 : this one is a little extra nasty because the doi can be fed to sci-hub.ru but it won't be available directly; instead you'll get a https://library.lol/ URL (https://library.lol/main/438C9D590BAE8FB922F4C968425D3DC4) where you can grab the PDF through Cloudflare or otherwise.








## A-a-a-and another bunch, just for testing

These are not nasty *per sé*, but came about thanks to Google Scholar: these are the ones from a binge where all the others did download without a hitch and most of these links ended up as HTML pages. Do note that I filtered the set only very roughly, so there will be some "sane" Google Scholar overview/list pages among these...


ResInNet: A Novel Deep Neural Network With Feature Reuse for Internet of Things | IEEE Journals & Magazine | IEEE Xplore
https://ieeexplore.ieee.org/abstract/document/8405574

Feature Reuse Residual Networks for Insect Pest Recognition | IEEE Journals & Magazine | IEEE Xplore
https://ieeexplore.ieee.org/document/8819933?denied=

Applied Sciences | Free Full-Text | Enhanced Reinforcement Learning Method Combining One-Hot Encoding-Based Vectors for CNN-Based Alternative High-Level Decisions
https://www.mdpi.com/2076-3417/11/3/1291/htm

Rodríguez: Beyond one-hot encoding: Lower dimensional... - Google Scholar
https://scholar.google.com/scholar?q=related:AdWMSqokBi8J:scholar.google.com/&scioq=one-hot+encoding+alternative&hl=nl&as_sdt=0,5

Rodríguez: Beyond one-hot encoding: Lower dimensional... - Google Scholar
https://scholar.google.com/scholar?cites=3388436083456660737&as_sdt=2005&sciodt=0,5&hl=nl

Seger: An investigation of categorical variable encoding... - Google Scholar
https://scholar.google.com/scholar?q=related:FTDm1et6al0J:scholar.google.com/&scioq=one-hot+encoding+alternative&hl=nl&as_sdt=0,5

Seger: An investigation of categorical variable encoding... - Google Scholar
https://scholar.google.com/scholar?cites=6731327746383163413&as_sdt=2005&sciodt=0,5&hl=nl

Performance of Domain-Wall Encoding for Quantum Annealing | IEEE Journals & Magazine | IEEE Xplore
https://ieeexplore.ieee.org/document/9485068?denied=

Cerda: Encoding high-cardinality string categorical... - Google Scholar
https://scholar.google.com/scholar?cites=17592796031510814290&as_sdt=2005&sciodt=0,5&hl=nl

Cerda: Encoding high-cardinality string categorical... - Google Scholar
https://scholar.google.com/scholar?q=related:Up4asMcqJvQJ:scholar.google.com/&scioq=one-hot+encoding+alternative&hl=nl&as_sdt=0,5

Ranković: Bayesian optimisation for additive screening... - Google Scholar
https://scholar.google.com/scholar?q=related:XikXeS9g_8YJ:scholar.google.com/&scioq=one-hot+encoding+alternative&hl=nl&as_sdt=0,5

Impact of Encoding of High Cardinality Categorical Data to Solve ...: Ingenta Connect
https://www.ingentaconnect.com/contentone/asp/jctn/2020/00000017/f0020009/art00068

Li: Effective multi-hot encoding and classifier for... - Google Scholar
https://scholar.google.com/scholar?q=related:QuzVQeVK4p4J:scholar.google.com/&scioq=one-hot+encoding+alternative&hl=nl&as_sdt=0,5

Chen: Learning k-way d-dimensional discrete codes... - Google Scholar
https://scholar.google.com/scholar?q=related:uxv_nHJDr1EJ:scholar.google.com/&scioq=one-hot+encoding+alternative&hl=nl&as_sdt=0,5

Chen: Learning k-way d-dimensional discrete codes... - Google Scholar
https://scholar.google.com/scholar?cites=5885997397535824827&as_sdt=2005&sciodt=0,5&hl=nl

Regularized target encoding outperforms traditional methods in supervised machine learning with high cardinality features | Computational Statistics
https://link.springer.com/article/10.1007/s00180-022-01207-6

Andrew Wheeler | Crime Analysis and Crime Mapping | Page 12
https://andrewpwheeler.com/page/12/

Lenz: Representing missing values through polar encoding - Google Scholar
https://scholar.google.com/scholar?q=related:wdylaxwn17wJ:scholar.google.com/&scioq=one-hot+encoding+alternative&hl=nl&as_sdt=0,5

Response to Comment on “Predicting reaction performance in C–N cross-coupling using machine learning” | Science
https://www.science.org/doi/full/10.1126/science.aat8763

Serrà: Getting deep recommenders fit: Bloom embeddings... - Google Scholar
https://scholar.google.com/scholar?q=related:o1pRA-6rRGUJ:scholar.google.com/&scioq=one-hot+encoding+alternative&hl=nl&as_sdt=0,5

Serrà: Getting deep recommenders fit: Bloom embeddings... - Google Scholar
https://scholar.google.com/scholar?cites=7297146334993275555&as_sdt=2005&sciodt=0,5&hl=nl

Hadamard’s Defense Against Adversarial Examples | IEEE Journals & Magazine | IEEE Xplore
https://ieeexplore.ieee.org/document/9520401?denied=

Hoyos: Hadamard’s defense against adversarial examples - Google Scholar
https://scholar.google.com/scholar?cites=3277040636672608408&as_sdt=2005&sciodt=0,5&hl=nl

Electronics | Free Full-Text | Fault Diagnosis Method of Smart Meters Based on DBN-CapsNet
https://www.mdpi.com/2079-9292/11/10/1603

Garrido-Merchán: Dealing with categorical and integer-va... - Google Scholar
https://scholar.google.com/scholar?cites=16557151492841928343&as_sdt=2005&sciodt=0,5&hl=nl

Garrido-Merchán: Dealing with categorical and integer-va... - Google Scholar
https://scholar.google.com/scholar?q=related:l_YrJ4_RxuUJ:scholar.google.com/&scioq=one-hot+encoding+alternative&hl=nl&as_sdt=0,5

Encoding Web-based Data for Efficient Storage in Machine Learning Applications | IEEE Conference Publication | IEEE Xplore
https://ieeexplore.ieee.org/abstract/document/9092264

Aich: Encoding web-based data for efficient storage... - Google Scholar
https://scholar.google.com/scholar?cites=12080723765964382809&as_sdt=2005&sciodt=0,5&hl=nl

Wasserstein_Loss_With_Alternative_Reinforcement_Learning_for_Severity-Aware_Semantic_Segmentation.pdf - Google Drive
https://drive.google.com/file/d/1oJGGQgp7Rknk_Trt0AhgPmBg2r9u2h4H/view

An alternative approach to dimension reduction for pareto distributed data: a case study | Journal of Big Data | Full Text
https://journalofbigdata.springeropen.com/articles/10.1186/s40537-021-00428-8

Approximate Bayesian neural networks in genomic prediction | Genetics Selection Evolution
https://link.springer.com/article/10.1186/s12711-018-0439-1

Bayesian Nonparametric Dimensionality Reduction of Categorical Data for Predicting Severity of COVID-19 in Pregnant Women - PMC
https://www.ncbi.nlm.nih.gov/pmc/articles/PMC8920026/

Sensors | Free Full-Text | An Imbalanced Generative Adversarial Network-Based Approach for Network Intrusion Detection in an Imbalanced Dataset
https://www.mdpi.com/1424-8220/23/1/550

direct.mit.edu/isal/proceedings-pdf/alife2018/30/234/1904922/isal_a_00049.pdf
https://direct.mit.edu/isal/proceedings-pdf/alife2018/30/234/1904922/isal_a_00049.pdf

Improving Low-Resource Speech Recognition Based on Improved NN-HMM Structures | IEEE Journals & Magazine | IEEE Xplore
https://ieeexplore.ieee.org/document/9069188?denied=

Combining discrete choice models and neural networks through embeddings: Formulation, interpretability and performance - ScienceDirect
https://www.sciencedirect.com/science/article/pii/S019126152300108X

The Categorical Data Conundrum: Heuristics for Classification Problems—A Case Study on Domestic Fire Injuries | IEEE Journals & Magazine | IEEE Xplore
https://ieeexplore.ieee.org/document/9810246?denied=

ScienceDirect.com | Science, health and medical journals, full text articles and books.
https://www.sciencedirect.com/science/article/pii/S1877050920310619/pdf?crasolve=1&r=8247cc9f0f3d5c48&ts=1699718963084&rtype=https&vrr=UKN&redir=UKN&redir_fr=UKN&redir_arc=UKN&vhash=UKN&host=d3d3LnNjaWVuY2VkaXJlY3QuY29t&tsoh=d3d3LnNjaWVuY2VkaXJlY3QuY29t&rh=d3d3LnNjaWVuY2VkaXJlY3QuY29t&re=X2JsYW5rXw%3D%3D&ns_h=d3d3LnNjaWVuY2VkaXJlY3QuY29t&ns_e=X2JsYW5rXw%3D%3D&rh_fd=rrr)n%5Ed%60i%5E%60_dm%60%5Eo)%5Ejh&tsoh_fd=rrr)n%5Ed%60i%5E%60_dm%60%5Eo)%5Ejh&iv=41dc97a9fe8b49f6d91209672263fa6f&token=34663139623564386462333363663237353065396535353863373535646433333866313534626163623861623531303835393166316232623363383163303664393761373737376438323963303862653963663363313237383064653a386336333066366365643266613862383232633430373663&text=fca706c0c39771b402398c2d4cddc5f09dd341bcbc577707ae282dc9e11ce0af4be8fe838b9ba170516e4762b63b3ab30c2a22be805da749ee9bcb51990747c4ba987699657c7e147432fcc420fc9592b585edabea8ee5defa571c0ab5a05df15f082e3654c1d1d8d4ca1405de576afad7142ff7eaf9d93abb2217f1f6be187378d3d30de59cbdb10a24e23f5e9f503dff7708572d9f27d9984461ecf5c2c1a515fb52f0cde07854ed9a64a353a559ced9de1f167f926a83eafeaddc72c692137f5b80d74a5492863430572824b2492de9e53ada0416d7faf80f8f9c4a5184f22ce556847d32fcb2d62855f6d04d81edcb326244d61a39952cd5f5a48b060aa42147ca89afd4484ec04d7771af250fc6ae03629f57f1a1facd2739404ffe93632a395c118b0406187d8254410accebe7&original=3f6d64353d3033326665376334343730643961313364646563303636323439653033346464267069643d312d73322e302d53313837373035303932303331303631392d6d61696e2e706466

Nonlife Insurance Risk Classification Using Categorical Embedding | Published in CAS E-Forum
https://eforum.casact.org/article/74931

Document Forgery Detection in the Context of Double JPEG Compression | SpringerLink
https://link.springer.com/chapter/10.1007/978-3-031-37745-7_5

Menon: Why distillation helps: a statistical perspective - Google Scholar
https://scholar.google.com/scholar?cites=4817636428241334634&as_sdt=2005&sciodt=0,5&hl=nl

Hierarchy-based semantic embeddings for single-valued & multi-valued categorical variables | Journal of Intelligent Information Systems
https://link.springer.com/article/10.1007/s10844-021-00693-2

ScienceDirect.com | Science, health and medical journals, full text articles and books.
https://www.sciencedirect.com/science/article/pii/S1877050922012431/pdf?crasolve=1&r=8247d7361ad20b6c&ts=1699719396826&rtype=https&vrr=UKN&redir=UKN&redir_fr=UKN&redir_arc=UKN&vhash=UKN&host=d3d3LnNjaWVuY2VkaXJlY3QuY29t&tsoh=d3d3LnNjaWVuY2VkaXJlY3QuY29t&rh=d3d3LnNjaWVuY2VkaXJlY3QuY29t&re=X2JsYW5rXw%3D%3D&ns_h=d3d3LnNjaWVuY2VkaXJlY3QuY29t&ns_e=X2JsYW5rXw%3D%3D&rh_fd=rrr)n%5Ed%60i%5E%60_dm%60%5Eo)%5Ejh&tsoh_fd=rrr)n%5Ed%60i%5E%60_dm%60%5Eo)%5Ejh&iv=e0cface17bb7aa313ba0da476be7d739&token=646230346332343764636461623135353039396530303665353237643066346362313333326666316262343263666165663136613637313966646333366231393931323230656361393834303335613930333831343937353a343762616564333736623437633361623138626565326333&text=06d058212b40edb10e962b22c648e1b1d52c9cef1cfae171ef61817e7f2bb525c59eac6dd724f017a550dd02226580a5169ff133d29e576a02d8093d5645598b23e04892c30352e984ae9e84bfb4156cc30d029c49c6b61563fad674395a48f13d341e66c6eae1b75906d45f26e6b5950a803cdefbbd5fabd77abaabbf2572108473bb4b0a2113380ba6a6102900fda9a3414bdba1f9703fbc7ab04e8bdfd496e7573a72555041010f24d6be69c333ff10416877130397327749099477a0edf7f668d621d17a8deb0cb4098dd59e12f97e3ed17a0a67c666a560ead3bc1e660d9266f96519513451cc659b3f7e022df1cd05cbf2bccb08ef7a04ebe099ed34541b4bab81d1936e38c8989662397842454e6f57d4da496a0c0b171f666bbd22f3ca4ad4200715c0a794ee18649af149f4&original=3f6d64353d3265373466623961343765613533393563316236323232313462646535613762267069643d312d73322e302d53313837373035303932323031323433312d6d61696e2e706466

Predicting Default Risk on Peer-to-Peer Lending Imbalanced Datasets | IEEE Journals & Magazine | IEEE Xplore
https://ieeexplore.ieee.org/document/9429248?denied=

Gene2vec: gene subsequence embedding for prediction of mammalian N6-methyladenosine sites from mRNA
https://rnajournal.cshlp.org/content/25/2/205.full.html

Text Representation using Convolutional Networks - ProQuest
https://www.proquest.com/openview/ecbd779b15e0d56412e65ae274e0f905/1?pq-origsite=gscholar&cbl=18750&diss=y

Efficient Error-correcting Output Codes for Adversarial Learning Robustness | IEEE Conference Publication | IEEE Xplore
https://ieeexplore.ieee.org/abstract/document/9839178

Wan: Efficient error-correcting output codes for... - Google Scholar
https://scholar.google.com/scholar?cites=12858932987065476252&as_sdt=2005&sciodt=0,5&hl=nl

Latent Personality Traits Assessment From Social Network Activity Using Contextual Language Embedding | IEEE Journals & Magazine | IEEE Xplore
https://ieeexplore.ieee.org/document/9531972?denied=

Manifold Modeling in Embedded Space: An Interpretable Alternative to Deep Image Prior | IEEE Journals & Magazine | IEEE Xplore
https://ieeexplore.ieee.org/document/9281370?denied=

Optimization of Tree-Based Machine Learning Models to Predict the Length of Hospital Stay Using Genetic Algorithm
https://www.hindawi.com/journals/jhe/2023/9673395/

IEEE Xplore Full-Text PDF:
https://ieeexplore.ieee.org/stamp/stamp.jsp?arnumber=10107490

Web2Vec: Phishing Webpage Detection Method Based on Multidimensional Features Driven by Deep Learning | IEEE Journals & Magazine | IEEE Xplore
https://ieeexplore.ieee.org/document/9288677?denied=

A Hybrid Document Feature Extraction Method Using Latent Dirichlet Allocation and Word2Vec | IEEE Conference Publication | IEEE Xplore
https://ieeexplore.ieee.org/abstract/document/7866114

Wang: A hybrid document feature extraction method... - Google Scholar
https://scholar.google.com/scholar?cites=13652393100533729878&as_sdt=2005&sciodt=0,5&hl=nl

Wang: A hybrid document feature extraction method... - Google Scholar
https://scholar.google.com/scholar?cluster=13652393100533729878&hl=nl&as_sdt=0,5

Wang: A hybrid document feature extraction method... - Google Scholar
https://scholar.google.com/scholar?q=related:VopOAzELd70J:scholar.google.com/&scioq=one-hot+encoding+alternative&hl=nl&as_sdt=0,5

Buy & Sell Trends Analysis Using Decision Trees | IEEE Conference Publication | IEEE Xplore
https://ieeexplore.ieee.org/abstract/document/9247907

[1611.01462] Tying Word Vectors and Word Classifiers: A Loss Framework for Language Modeling
https://arxiv.org/abs/1611.01462

Symmetry | Free Full-Text | Clickbait Convolutional Neural Network
https://www.mdpi.com/2073-8994/10/5/138

Convolutional neural networks are not invariant to translation, but they can learn to be | The Journal of Machine Learning Research
https://dl.acm.org/doi/abs/10.5555/3546258.3546487

Durall: Watch your up-convolution: Cnn based generative... - Google Scholar
https://scholar.google.com/scholar?cites=18044048085015580055&as_sdt=2005&sciodt=0,5&hl=nl

Durall: Watch your up-convolution: Cnn based generative... - Google Scholar
https://scholar.google.com/scholar?q=related:l81rwSFWafoJ:scholar.google.com/&scioq=convolutional+neural+network+fail&hl=nl&as_sdt=0,5

Self-Distillation: Towards Efficient and Compact Neural Networks | IEEE Journals & Magazine | IEEE Xplore
https://ieeexplore.ieee.org/document/9381661?denied=

Head Network Distillation: Splitting Distilled Deep Neural Networks for Resource-Constrained Edge Computing Systems | IEEE Journals & Magazine | IEEE Xplore
https://ieeexplore.ieee.org/document/9265295?denied=

Effective training of convolutional neural networks for age estimation based on knowledge distillation | Neural Computing and Applications
https://link.springer.com/article/10.1007/s00521-021-05981-0

IEEE Xplore Full-Text PDF:
https://ieeexplore.ieee.org/stamp/stamp.jsp?arnumber=10220213

Subspace distillation for continual learning - ScienceDirect
https://www.sciencedirect.com/science/article/pii/S0893608023004057

Moghaddam: AdOtsu: An adaptive and parameterless... - Google Scholar
https://scholar.google.com/scholar?cites=7150396333454933302&as_sdt=2005&sciodt=0,5&hl=nl

Shafait: Efficient implementation of local adaptive... - Google Scholar
https://scholar.google.com/scholar?cites=2289746146619769012&as_sdt=2005&sciodt=0,5&hl=nl

Liu: Scene text detection and recognition with advances... - Google Scholar
https://scholar.google.com/scholar?cites=7859756468599902414&as_sdt=2005&sciodt=0,5&hl=nl

Tian: Using latent dirichlet allocation for automatic... - Google Scholar
https://scholar.google.com/scholar?cites=13597617382243920940&as_sdt=2005&sciodt=0,5&hl=nl

Xiong: An enhanced binarization framework for degraded... - Google Scholar
https://scholar.google.com/scholar?cluster=10788474601520992553&hl=nl&as_sdt=2005&sciodt=0,5

Xiong: An enhanced binarization framework for degraded... - Google Scholar
https://scholar.google.com/scholar?cites=10788474601520992553&as_sdt=2005&sciodt=0,5&hl=nl

Chaki: A comprehensive survey on image binarization... - Google Scholar
https://scholar.google.com/scholar?cluster=11338959589554478736&hl=nl&as_sdt=2005&sciodt=0,5

Chaki: A comprehensive survey on image binarization... - Google Scholar
https://scholar.google.com/scholar?cites=11338959589554478736&as_sdt=2005&sciodt=0,5&hl=nl

New trends on digitisation of complex engineering drawings | Neural Computing and Applications
https://link.springer.com/article/10.1007/s00521-018-3583-1

Moreno-García: New trends on digitisation of complex... - Google Scholar
https://scholar.google.com/scholar?cluster=13549619672030963393&hl=nl&as_sdt=2005&sciodt=0,5

Moreno-García: New trends on digitisation of complex... - Google Scholar
https://scholar.google.com/scholar?cites=13549619672030963393&as_sdt=2005&sciodt=0,5&hl=nl

Artificial intelligence based writer identification generates new evidence for the unknown scribes of the Dead Sea Scrolls exemplified by the Great Isaiah Scroll (1QIsaa) | PLOS ONE
https://journals.plos.org/plosone/article?id=10.1371/journal.pone.0249769&fbclid=IwAR2xP7G7yG-mHYH9yrokgATSPHlfmbJkdh3llN-UZEZoVsjn4w0tqoObd1Q

Popović: Artificial intelligence based writer identifica... - Google Scholar
https://scholar.google.com/scholar?cluster=10337017256701884065&hl=nl&as_sdt=2005&sciodt=0,5

Popović: Artificial intelligence based writer identifica... - Google Scholar
https://scholar.google.com/scholar?cites=10337017256701884065&as_sdt=2005&sciodt=0,5&hl=nl

Chen: A double-threshold image binarization method... - Google Scholar
https://scholar.google.com/scholar?cites=2057388095066247466&as_sdt=2005&sciodt=0,5&hl=nl

Chen: A double-threshold image binarization method... - Google Scholar
https://scholar.google.com/scholar?cluster=2057388095066247466&hl=nl&as_sdt=2005&sciodt=0,5

Huang: An improved image segmentation algorithm based... - Google Scholar
https://scholar.google.com/scholar?cluster=4105831725031549600&hl=nl&as_sdt=2005&sciodt=0,5

Huang: An improved image segmentation algorithm based... - Google Scholar
https://scholar.google.com/scholar?cites=4105831725031549600&as_sdt=2005&sciodt=0,5&hl=nl

Stathis: An Evaluation Technique for Binarization... - Google Scholar
https://scholar.google.com/scholar?cites=452124334981249785&as_sdt=2005&sciodt=0,5&hl=nl

Bardozzo: Sugeno integral generalization applied... - Google Scholar
https://scholar.google.com/scholar?cites=12601339676908616344&as_sdt=2005&sciodt=0,5&hl=nl

Bardozzo: Sugeno integral generalization applied... - Google Scholar
https://scholar.google.com/scholar?cluster=12601339676908616344&hl=nl&as_sdt=2005&sciodt=0,5

Zhu: A fast 2d otsu thresholding algorithm based... - Google Scholar
https://scholar.google.com/scholar?cluster=7455085599189691427&hl=nl&as_sdt=2005&sciodt=0,5

Zhu: A fast 2d otsu thresholding algorithm based... - Google Scholar
https://scholar.google.com/scholar?cites=7455085599189691427&as_sdt=2005&sciodt=0,5&hl=nl

Feature-extraction methods for historical manuscript dating based on writing style development - ScienceDirect
https://www.sciencedirect.com/science/article/pii/S0167865520300386

Dhali: Feature-extraction methods for historical... - Google Scholar
https://scholar.google.com/scholar?cluster=3840792240169960406&hl=nl&as_sdt=2005&sciodt=0,5

Dhali: Feature-extraction methods for historical... - Google Scholar
https://scholar.google.com/scholar?cites=3840792240169960406&as_sdt=2005&sciodt=0,5&hl=nl

Xiong: Degraded historical document image binarization... - Google Scholar
https://scholar.google.com/scholar?cluster=8344646218602678048&hl=nl&as_sdt=2005&sciodt=0,5

Xiong: Degraded historical document image binarization... - Google Scholar
https://scholar.google.com/scholar?cites=8344646218602678048&as_sdt=2005&sciodt=0,5&hl=nl

Bera: A non-parametric binarization method based... - Google Scholar
https://scholar.google.com/scholar?cites=3153868689449380835&as_sdt=2005&sciodt=0,5&hl=nl

Bera: A non-parametric binarization method based... - Google Scholar
https://scholar.google.com/scholar?cluster=3153868689449380835&hl=nl&as_sdt=2005&sciodt=0,5

Castellanos: Unsupervised neural domain adaptation... - Google Scholar
https://scholar.google.com/scholar?cluster=12572079185611235588&hl=nl&as_sdt=2005&sciodt=0,5

Castellanos: Unsupervised neural domain adaptation... - Google Scholar
https://scholar.google.com/scholar?cites=12572079185611235588&as_sdt=2005&sciodt=0,5&hl=nl

U-Net-bin: hacking the document image binarization contest – тема научной статьи по компьютерным и информационным наукам читайте бесплатно текст научно-исследовательской работы в электронной библиотеке КиберЛенинка
https://cyberleninka.ru/article/n/u-net-bin-hacking-the-document-image-binarization-contest

Bezmaternykh: U-Net-bin: hacking the document image... - Google Scholar
https://scholar.google.com/scholar?cluster=1201007585086316565&hl=nl&as_sdt=2005&sciodt=0,5

Bezmaternykh: U-Net-bin: hacking the document image... - Google Scholar
https://scholar.google.com/scholar?cites=1201007585086316565&as_sdt=2005&sciodt=0,5&hl=nl

A Survey on Breaking Technique of Text-Based CAPTCHA
https://www.hindawi.com/journals/scn/2017/6898617/

Simistira: Icdar2017 competition on layout analysis... - Google Scholar
https://scholar.google.com/scholar?cites=15952945818097872095&as_sdt=2005&sciodt=0,5&hl=nl

Khan: Automatic ink mismatch detection for forensic... - Google Scholar
https://scholar.google.com/scholar?cites=8060544934622872632&as_sdt=2005&sciodt=0,5&hl=nl

Tran: Table detection from document image using vertical... - Google Scholar
https://scholar.google.com/scholar?cites=3787761044487141930&as_sdt=2005&sciodt=0,5&hl=nl

Saxena: Niblack’s binarization method and its modificat... - Google Scholar
https://scholar.google.com/scholar?cites=10763796780487772861&as_sdt=2005&sciodt=0,5&hl=nl

Saxena: Niblack’s binarization method and its modificat... - Google Scholar
https://scholar.google.com/scholar?cluster=10763796780487772861&hl=nl&as_sdt=2005&sciodt=0,5

Feng: Contrast adaptive binarization of low quality... - Google Scholar
https://scholar.google.com/scholar?cites=8676153132691851427&as_sdt=2005&sciodt=0,5&hl=nl

Sensors | Free Full-Text | Combining the YOLOv4 Deep Learning Model with UAV Imagery Processing Technology in the Extraction and Quantization of Cracks in Bridges
https://www.mdpi.com/1424-8220/23/5/2572

Kao: Combining the YOLOv4 deep learning model with... - Google Scholar
https://scholar.google.com/scholar?cluster=14704569747460100613&hl=nl&as_sdt=2005&sciodt=0,5

Efficient and effective OCR engine training | International Journal on Document Analysis and Recognition (IJDAR)
https://link.springer.com/article/10.1007/s10032-019-00347-8

Barron: A generalization of Otsu’s method and minimum... - Google Scholar
https://scholar.google.com/scholar?cites=17503498248549422833&as_sdt=2005&sciodt=0,5&hl=nl

Nina: A recursive Otsu thresholding method for scanned... - Google Scholar
https://scholar.google.com/scholar?cluster=13686907608080975307&hl=nl&as_sdt=2005&sciodt=0,5

Nina: A recursive Otsu thresholding method for scanned... - Google Scholar
https://scholar.google.com/scholar?cites=13686907608080975307&as_sdt=2005&sciodt=0,5&hl=nl

Bolelli: Toward reliable experiments on the performance... - Google Scholar
https://scholar.google.com/scholar?cites=12315627088603150782&as_sdt=2005&sciodt=0,5&hl=nl

Afzal: Document image binarization using lstm: A... - Google Scholar
https://scholar.google.com/scholar?cites=8098448744792634736&as_sdt=2005&sciodt=0,5&hl=nl

Bako: Removing shadows from images of documents - Google Scholar
https://scholar.google.com/scholar?cites=5136840677231035445&as_sdt=2005&sciodt=0,5&hl=nl

Hedjam: A spatially adaptive statistical method for... - Google Scholar
https://scholar.google.com/scholar?cluster=501149854233444050&hl=nl&as_sdt=2005&sciodt=0,5

Hedjam: A spatially adaptive statistical method for... - Google Scholar
https://scholar.google.com/scholar?cites=501149854233444050&as_sdt=2005&sciodt=0,5&hl=nl

Kasar: Font and background color independent text... - Google Scholar
https://scholar.google.com/scholar?cites=9376484994271790146&as_sdt=2005&sciodt=0,5&hl=nl

Tribelhorn: Photo-document segmentation method and system - Google Scholar
https://scholar.google.com/scholar?cites=6192029047716595856&as_sdt=2005&sciodt=0,5&hl=nl

Mathematics | Free Full-Text | Lung X-ray Segmentation using Deep Convolutional Neural Networks on Contrast-Enhanced Binarized Images
https://www.mdpi.com/2227-7390/8/4/545/htm

J. Imaging | Free Full-Text | Adaptive Digital Hologram Binarization Method Based on Local Thresholding, Block Division and Error Diffusion
https://www.mdpi.com/2313-433X/8/2/15

Cheremkhin: Adaptive Digital Hologram Binarization... - Google Scholar
https://scholar.google.com/scholar?cluster=12642731123662336898&hl=nl&as_sdt=2005&sciodt=0,5

Wen: A new binarization method for non-uniform illuminate... - Google Scholar
https://scholar.google.com/scholar?cluster=12900974706966451519&hl=nl&as_sdt=2005&sciodt=0,5

Wen: A new binarization method for non-uniform illuminate... - Google Scholar
https://scholar.google.com/scholar?cites=12900974706966451519&as_sdt=2005&sciodt=0,5&hl=nl

Binarization of Degraded Document Images Using Convolutional Neural Networks and Wavelet-Based Multichannel Images | IEEE Journals & Magazine | IEEE Xplore
https://ieeexplore.ieee.org/document/9171243?denied=

Mustafa: Binarization of document image using optimum... - Google Scholar
https://scholar.google.com/scholar?cites=1256397357623439010&as_sdt=2005&sciodt=0,5&hl=nl

ScienceDirect.com | Science, health and medical journals, full text articles and books.
https://www.sciencedirect.com/science/article/am/pii/S0031320318301389

Vo: Robust regression for image binarization under... - Google Scholar
https://scholar.google.com/scholar?cites=13884842102900689454&as_sdt=2005&sciodt=0,5&hl=nl

Lu: Binarization of degraded document images based... - Google Scholar
https://scholar.google.com/scholar?cites=6275176260480022185&as_sdt=2005&sciodt=0,5&hl=nl

Lu: Binarization of degraded document images based... - Google Scholar
https://scholar.google.com/scholar?cluster=6275176260480022185&hl=nl&as_sdt=2005&sciodt=0,5

Kavallieratou: Improving the quality of degraded... - Google Scholar
https://scholar.google.com/scholar?cites=2644572286198015048&as_sdt=2005&sciodt=0,5&hl=nl

Binarization techniques for degraded document images — A review | IEEE Conference Publication | IEEE Xplore
https://ieeexplore.ieee.org/document/7784945?denied=

Chauhan: Binarization techniques for degraded document... - Google Scholar
https://scholar.google.com/scholar?cites=843970595977049850&as_sdt=2005&sciodt=0,5&hl=nl

Kaur: Modified Sauvola binarization for degraded... - Google Scholar
https://scholar.google.com/scholar?cluster=7328006225358559633&hl=nl&as_sdt=2005&sciodt=0,5

Kaur: Modified Sauvola binarization for degraded... - Google Scholar
https://scholar.google.com/scholar?cites=7328006225358559633&as_sdt=2005&sciodt=0,5&hl=nl

Lund: Combining multiple thresholding binarization... - Google Scholar
https://scholar.google.com/scholar?cites=9453788323613275605&as_sdt=2005&sciodt=0,5&hl=nl

Lund: Combining multiple thresholding binarization... - Google Scholar
https://scholar.google.com/scholar?cluster=9453788323613275605&hl=nl&as_sdt=2005&sciodt=0,5

IEEE Xplore Full-Text PDF:
https://ieeexplore.ieee.org/stamp/stamp.jsp?arnumber=8584426

ScienceDirect.com | Science, health and medical journals, full text articles and books.
https://www.sciencedirect.com/science/article/am/pii/S0031320319304455

ScienceDirect.com | Science, health and medical journals, full text articles and books.
https://www.sciencedirect.com/science/article/am/pii/S0001457518301751

Bhowmik: BINYAS: a complex document layout analysis system - Google Scholar
https://scholar.google.com/scholar?cluster=13208950577705010261&hl=nl&as_sdt=2005&sciodt=0,5

Bhowmik: BINYAS: a complex document layout analysis system - Google Scholar
https://scholar.google.com/scholar?cites=13208950577705010261&as_sdt=2005&sciodt=0,5&hl=nl

Yimit: 2-D direction histogram based entropic thresholding - Google Scholar
https://scholar.google.com/scholar?cluster=8692609532732300494&hl=nl&as_sdt=2005&sciodt=0,5

Yimit: 2-D direction histogram based entropic thresholding - Google Scholar
https://scholar.google.com/scholar?cites=8692609532732300494&as_sdt=2005&sciodt=0,5&hl=nl

DSpace
https://helda.helsinki.fi/bitstream/handle/10138/310105/ecp17131038.pdf?sequence=1

Koistinen: Improving optical character recognition... - Google Scholar
https://scholar.google.com/scholar?cites=12422171117038534124&as_sdt=2005&sciodt=0,5&hl=nl

Peng: Document binarization via multi-resolutional... - Google Scholar
https://scholar.google.com/scholar?cluster=18117382236184731442&hl=nl&as_sdt=2005&sciodt=0,5

Peng: Document binarization via multi-resolutional... - Google Scholar
https://scholar.google.com/scholar?cites=18117382236184731442&as_sdt=2005&sciodt=0,5&hl=nl

Chest X-ray segmentation using Sauvola thresholding and Gaussian derivatives responses | SpringerLink
https://link.springer.com/article/10.1007/s12652-019-01281-7

Kiran: Chest X-ray segmentation using Sauvola thresholdin... - Google Scholar
https://scholar.google.com/scholar?cites=1254217958761314765&as_sdt=2005&sciodt=0,5&hl=nl

ProQuest - ProQuest
https://www.proquest.com/info/openurldocerror;jsessionid=E19220673BDF9E33F86A1661628C319B.i-0edd47c499984ce68

Kaur: A review on various methods of image thresholding - Google Scholar
https://scholar.google.com/scholar?cites=9157504981372036802&as_sdt=2005&sciodt=0,5&hl=nl

Parallel Nonparametric Binarization for Degraded Document Images.pdf - Google Drive
https://drive.google.com/file/d/13D3_d8i75ZSz6rY3ouU_Yh8VSilXIgqX/view

Document Image Binarization With Stroke Boundary Feature Guided Network | IEEE Journals & Magazine | IEEE Xplore
https://ieeexplore.ieee.org/document/9366499?denied=

Lelore: Super-resolved binarization of text based... - Google Scholar
https://scholar.google.com/scholar?cites=10840415963942003910&as_sdt=2005&sciodt=0,5&hl=nl

A non-stationary density model to separate overlapped texts in degraded documents | Signal, Image and Video Processing
https://link.springer.com/article/10.1007/s11760-014-0735-3

Tonazzini: A non-stationary density model to separate... - Google Scholar
https://scholar.google.com/scholar?cluster=13364098428799968499&hl=nl&as_sdt=2005&sciodt=0,5

Tonazzini: A non-stationary density model to separate... - Google Scholar
https://scholar.google.com/scholar?cites=13364098428799968499&as_sdt=2005&sciodt=0,5&hl=nl

Multilevel Thresholding for Image Segmentation Using Mean Gradient
https://www.hindawi.com/journals/jece/2022/1254852/

Ashir: Multilevel thresholding for image segmentation... - Google Scholar
https://scholar.google.com/scholar?cluster=6269192388629488491&hl=nl&as_sdt=2005&sciodt=0,5

Villegas: On the modification of binarization algorithms... - Google Scholar
https://scholar.google.com/scholar?cites=11347019352762881665&as_sdt=2005&sciodt=0,5&hl=nl

Villegas: On the modification of binarization algorithms... - Google Scholar
https://scholar.google.com/scholar?q=related:gRbqdPu1eJ0J:scholar.google.com/&scioq=&hl=nl&as_sdt=2005&sciodt=0,5

Paixao: Fast (er) reconstruction of shredded text... - Google Scholar
https://scholar.google.com/scholar?cites=3473786441252369837&as_sdt=2005&sciodt=0,5&hl=nl

Fung: A review of evaluation of optimal binarization... - Google Scholar
https://scholar.google.com/scholar?cites=6070490695828019041&as_sdt=2005&sciodt=0,5&hl=nl

Jacobs: A novel approach to text binarization via... - Google Scholar
https://scholar.google.com/scholar?cluster=10744526816284385798&hl=nl&as_sdt=2005&sciodt=0,5

Jacobs: A novel approach to text binarization via... - Google Scholar
https://scholar.google.com/scholar?cites=10744526816284385798&as_sdt=2005&sciodt=0,5&hl=nl

Lamiroy: Computing precision and recall with missing... - Google Scholar
https://scholar.google.com/scholar?cites=7754703995047718958&as_sdt=2005&sciodt=0,5&hl=nl

Lamiroy: Computing precision and recall with missing... - Google Scholar
https://scholar.google.com/scholar?cluster=7754703995047718958&hl=nl&as_sdt=2005&sciodt=0,5

Milyaev: Fast and accurate scene text understanding... - Google Scholar
https://scholar.google.com/scholar?cluster=8409937517920519434&hl=nl&as_sdt=2005&sciodt=0,5

AGORA: the interactive document image analysis tool of the BVH project | IEEE Conference Publication | IEEE Xplore
https://ieeexplore.ieee.org/abstract/document/1612957

Image binarization method for markers tracking in extreme light conditions - IOS Press
https://content.iospress.com/articles/integrated-computer-aided-engineering/ica210674

Bai: A seed-based segmentation method for scene text... - Google Scholar
https://scholar.google.com/scholar?cluster=12614374815433395014&hl=nl&as_sdt=2005&sciodt=0,5

Dynamic Downscaling Segmentation for Noisy, Low-Contrast in Situ Underwater Plankton Images | IEEE Journals & Magazine | IEEE Xplore
https://ieeexplore.ieee.org/document/9115007?denied=

J. Imaging | Free Full-Text | Comparative Study of Data Matrix Codes Localization and Recognition Methods
https://www.mdpi.com/2313-433X/7/9/163/htm

Text Extraction in Complex Color Document Images for Enhanced Readability
https://www.scirp.org/html/1409.html

Nagabhushan: Text extraction in complex color document... - Google Scholar
https://scholar.google.com/scholar?cluster=16387403616812039983&hl=nl&as_sdt=2005&sciodt=0,5

Best Combination of Binarization Methods for License Plate Character Segmentation - Yoon - 2013 - ETRI Journal - Wiley Online Library
https://onlinelibrary.wiley.com/doi/pdf/10.4218/etrij.13.0112.0545

Image thresholding techniques for localization of sub‐resolution fluorescent biomarkers - Ghaye - 2013 - Cytometry Part A - Wiley Online Library
https://onlinelibrary.wiley.com/doi/full/10.1002/cyto.a.22345

Image thresholding techniques for localization of sub‐resolution fluorescent biomarkers - Ghaye - 2013 - Cytometry Part A - Wiley Online Library
https://onlinelibrary.wiley.com/doi/pdf/10.1002/cyto.a.22345

A survey on camera-captured scene text detection and extraction: towards Gurmukhi script | SpringerLink
https://link.springer.com/article/10.1007/s13735-016-0116-5

ICDAR2017.pdf - Google Drive
https://drive.google.com/file/d/1ZCd2UxhHJRkBE1nDb9fLIjNfo8HATm73/view

Binarization of images with variable lighting using adaptive windows | SpringerLink
https://link.springer.com/article/10.1007/s11760-022-02150-1

Calderon: Binarization of images with variable lighting... - Google Scholar
https://scholar.google.com/scholar?cites=17234027976438129434&as_sdt=2005&sciodt=0,5&hl=nl

Chiu: Parameter-free based two-stage method for binarizin... - Google Scholar
https://scholar.google.com/scholar?cites=2089767194149450511&as_sdt=2005&sciodt=0,5&hl=nl

Transition thresholds and transition operators for binarization and edge detection - ScienceDirect
https://www.sciencedirect.com/science/article/abs/pii/S0031320310002542

Fast and efficient document image clean up and binarization based on retinex theory | IEEE Conference Publication | IEEE Xplore
https://ieeexplore.ieee.org/abstract/document/6530014

Wagdy: Fast and efficient document image clean up... - Google Scholar
https://scholar.google.com/scholar?cites=5447894088095252032&as_sdt=2005&sciodt=0,5&hl=nl

A Review of Arabic Document Analysis Methods | IEEE Conference Publication | IEEE Xplore
https://ieeexplore.ieee.org/document/9946919?denied=

Toward a Binarization Framework resolving the Maghrebian Font Database challenges | IEEE Conference Publication | IEEE Xplore
https://ieeexplore.ieee.org/abstract/document/9364086

International Journal of Pattern Recognition and Artificial Intelligence
https://www.worldscientific.com/doi/abs/10.1142/S0218001421540136

ScienceDirect.com | Science, health and medical journals, full text articles and books.
https://www.sciencedirect.com/science/article/pii/S1877050915003877/pdf?crasolve=1&r=8246b11c9a73b94e&ts=1699707350509&rtype=https&vrr=UKN&redir=UKN&redir_fr=UKN&redir_arc=UKN&vhash=UKN&host=d3d3LnNjaWVuY2VkaXJlY3QuY29t&tsoh=d3d3LnNjaWVuY2VkaXJlY3QuY29t&rh=d3d3LnNjaWVuY2VkaXJlY3QuY29t&re=X2JsYW5rXw%3D%3D&ns_h=d3d3LnNjaWVuY2VkaXJlY3QuY29t&ns_e=X2JsYW5rXw%3D%3D&rh_fd=rrr)n%5Ed%60i%5E%60_dm%60%5Eo)%5Ejh&tsoh_fd=rrr)n%5Ed%60i%5E%60_dm%60%5Eo)%5Ejh&iv=6440070c7a040952f966fd2c0700815a&token=623232666439303862373064663964643662386234323632306438383535623465373839663330363336306166396261376262396566396165663464346236613763623764396565306166396563653061373438636566323a306638623466393066616436333237623331346531383230&text=30633da6ebb6fc2bf395c30fdbf9e00c19d55a0d84db0f0f8d3068fde5d6019e7536b0046f5c670e4557ab89713238333223675548bc66f85c8a81e215a304fcccb8a61546da47fe69694b4703dbc5002418090cc56d4f12ea2525c4e09523f7a4ca78688a560d18815fc30c554398cab6190277488cda625e383294f1af47df7fecc1b8c8af1d06f08baf4eba6a38ecb5a952a9732957a56896561cfd1896ad12b546fef6164fffd989b0cb5c28a5f2cd17427295e4ac3391ea0db1778f3d5f254c8232ed82d474a41c6495ff19eca5e54128858a8c295dab68cf03b841b2cfcfa0590a7430490b058b1d72cac846dcf4d833c9f46e337014dcc37b06085ca0a4af72d448d74c41dcc1eef7e2c2fda7898e8d43d3ed044553ae14f5c29f88bc1afa2e84efd2d901dfbce1583071343a&original=3f6d64353d6265333733353531346461376536393631306661396535623935616364303736267069643d312d73322e302d53313837373035303931353030333837372d6d61696e2e706466265f76616c636b3d31

Classification of incunable glyphs and out-of-distribution detection with joint energy-based models | International Journal on Document Analysis and Recognition (IJDAR)
https://link.springer.com/article/10.1007/s10032-023-00442-x

Electronics | Free Full-Text | Analysis of Image Preprocessing and Binarization Methods for OCR-Based Detection and Classification of Electronic Integrated Circuit Labeling
https://www.mdpi.com/2079-9292/12/11/2449

Morphological preprocessing method to thresholding degraded word images - ScienceDirect
https://www.sciencedirect.com/science/article/abs/pii/S016786550900049X

A Fuzzy C-Means Based Approach Towards Efficient Document Image Binarization | IEEE Conference Publication | IEEE Xplore
https://ieeexplore.ieee.org/abstract/document/8592936

Binarization of music score with complex background by deep convolutional neural networks | Multimedia Tools and Applications
https://link.springer.com/article/10.1007/s11042-020-10272-2

Applied Sciences | Free Full-Text | A Review of Document Image Enhancement Based on Document Degradation Problem
https://www.mdpi.com/2076-3417/13/13/7855

Text Extraction from Historical Document Images by the Combination of Several Thresholding Techniques
https://www.hindawi.com/journals/am/2014/934656/

Boiangiu: Methods of bitonal image conversion for... - Google Scholar
https://scholar.google.com/scholar?cites=4054754854685501570&as_sdt=2005&sciodt=0,5&hl=nl

Textline detection in degraded historical document images | EURASIP Journal on Image and Video Processing
https://link.springer.com/article/10.1186/s13640-017-0229-7

Image binarization using iterative partitioning: A global thresholding approach | IEEE Conference Publication | IEEE Xplore
https://ieeexplore.ieee.org/abstract/document/6146882

Full article: Image Thresholding Improved by Global Optimization Methods
https://www.tandfonline.com/doi/full/10.1080/08839514.2017.1300050

Oztan: Removal of artifacts from JPEG compressed... - Google Scholar
https://scholar.google.com/scholar?cites=10964784569913302753&as_sdt=2005&sciodt=0,5&hl=nl

Oztan: Removal of artifacts from JPEG compressed... - Google Scholar
https://scholar.google.com/scholar?cluster=10964784569913302753&hl=nl&as_sdt=2005&sciodt=0,5

Oja: Kohonen maps - Google Scholar
https://scholar.google.com/scholar?cites=6515284431613113944&as_sdt=2005&sciodt=0,5&hl=nl

Performance evaluation of the self‐organizing map for feature extraction - Liu - 2006 - Journal of Geophysical Research: Oceans - Wiley Online Library
https://agupubs.onlinelibrary.wiley.com/doi/full/10.1029/2005JC003117

Sensors | Free Full-Text | Neural Network-Based Self-Tuning PID Control for Underwater Vehicles
https://www.mdpi.com/1424-8220/16/9/1429/htm

Omatu: Self-tuning neuro-PID control and applications - Google Scholar
https://scholar.google.com/scholar?q=related:b5YQENwoqsAJ:scholar.google.com/&scioq=neural+pid+controller+tuning&hl=nl&as_sdt=0,5

Self-Tuning Neural Network PID With Dynamic Response Control | IEEE Journals & Magazine | IEEE Xplore
https://ieeexplore.ieee.org/document/9415738?denied=

Rivas-Echeverria: Neural network-based auto-tuning... - Google Scholar
https://scholar.google.com/scholar?q=related:-lqxGJ9_QcMJ:scholar.google.com/&scioq=neural+pid+controller+tuning&hl=nl&as_sdt=0,5

Sensors | Free Full-Text | Neural Network-Based Self-Tuning PID Control for Underwater Vehicles
https://www.mdpi.com/1424-8220/16/9/1429/htm

Self-Tuning Neural Network PID With Dynamic Response Control | IEEE Journals & Magazine | IEEE Xplore
https://ieeexplore.ieee.org/document/9415738?denied=

Rodríguez-Abreo: Self-tuning neural network PID... - Google Scholar
https://scholar.google.com/scholar?cites=3074129662556345400&as_sdt=2005&sciodt=0,5&hl=nl

PID controller tuning by using extremum seeking algorithm based on annealing recurrent neural network | IEEE Conference Publication | IEEE Xplore
https://ieeexplore.ieee.org/abstract/document/5646302

Auto-tuning of PID controller according to fractional-order reference model approximation for DC rotor control - ScienceDirect
https://www.sciencedirect.com/science/article/abs/pii/S0957415813000901

Research on self-tuning PID control strategy based on BP neural network | IEEE Conference Publication | IEEE Xplore
https://ieeexplore.ieee.org/abstract/document/6013163

Nonlinear neural controller with neural Smith predictor | SpringerLink
https://link.springer.com/article/10.1007/BF02310939

Artificial neural network and PID based control system for DC motor drives | IEEE Conference Publication | IEEE Xplore
https://ieeexplore.ieee.org/abstract/document/4602474

PID Parameters Auto-Tuning Method for Industrial Temperature Adjustment | IEEE Conference Publication | IEEE Xplore
https://ieeexplore.ieee.org/abstract/document/9486652

ScienceDirect.com | Science, health and medical journals, full text articles and books.
https://www.sciencedirect.com/science/article/pii/S1877050915013472/pdf?crasolve=1&r=8247374d5ab10a69&ts=1699712846955&rtype=https&vrr=UKN&redir=UKN&redir_fr=UKN&redir_arc=UKN&vhash=UKN&host=d3d3LnNjaWVuY2VkaXJlY3QuY29t&tsoh=d3d3LnNjaWVuY2VkaXJlY3QuY29t&rh=d3d3LnNjaWVuY2VkaXJlY3QuY29t&re=X2JsYW5rXw%3D%3D&ns_h=d3d3LnNjaWVuY2VkaXJlY3QuY29t&ns_e=X2JsYW5rXw%3D%3D&rh_fd=rrr)n%5Ed%60i%5E%60_dm%60%5Eo)%5Ejh&tsoh_fd=rrr)n%5Ed%60i%5E%60_dm%60%5Eo)%5Ejh&iv=f9150612d23b5b76be6a854e4829f73d&token=363662383365356563346465623263646335656136303838643831646136376639653836363739373234386132353039616234363931336631363131623531626564323661336432656638386634306662626437343065383a616663633231613261653963383739623938666237653962&text=7699ea4afede1512df25006e25150c86a04a9cb3637403794e62210caf38a2fcca817fcb26afdce04130c3952a4359a02663730d9a3257e4f61dd1b2b58fb671ad5792e3d9e7e7a48158d0efdb7baf63e3035615229b52352ff1cd78a4d5cf6f8b338a374bd31d2df1dc022672c3659ac21d814c1d03527540d6a6dcccbac20f0d99672ba9d70f66ff7d29ecb6d27ece92be5a2cc2b9883f9cfb56aae76ed209c4f6215f8972022d74c6adbb989e214683c894725bfa350ec3642a6b3c0b861eef4aef8de2e08fdd1e0d1ff54d8018f6996ddf7334f95912843dd99ee4070aaf1fe985c16cd2091bb8639d40e82870286c7e1c6e86d697647d3e28e4f4352b0451f6e5763a9ca7746a115977b41900a64f01c207592d23b1f7cf3cbc25d23a173c7616f2a47fb36154063baa9568a8c5&original=3f6d64353d3033386163653365393839613963646133646631323432323537383563646334267069643d312d73322e302d53313837373035303931353031333437322d6d61696e2e706466

Comparative Analysis of Neural-Network and Fuzzy Auto-Tuning Sliding Mode Controls for Overhead Cranes under Payload and Cable Variations
https://www.hindawi.com/journals/jcse/2019/1480732/

Frontiers | Online Tuning of PID Controller Using a Multilayer Fuzzy Neural Network Design for Quadcopter Attitude Tracking Control
https://www.frontiersin.org/articles/10.3389/fnbot.2020.619350/full

www.matlabi.ir
https://www.matlabi.ir/wp-content/uploads/bank_papers/g_paper/g186_Matlabi.ir_Development%20of%20Self-Tuning%20Intelligent%20PID%20Controller%20Based%20on%20BPNN%20for%20Indoor%20Air%20Quality%20Control.pdf

The application of the self-tuning neural network PID controller on the ship roll reduction in random waves - ScienceDirect
https://www.sciencedirect.com/science/article/abs/pii/S0029801810000582

Ortiz: Low-precision floating-point schemes for neural... - Google Scholar
https://scholar.google.com/scholar?cites=557672679283717467&as_sdt=2005&sciodt=0,5&hl=nl

Ortiz: Low-precision floating-point schemes for neural... - Google Scholar
https://scholar.google.com/scholar?q=related:W3Wl3VhAvQcJ:scholar.google.com/&scioq=neural+net+without+floating+point&hl=nl&as_sdt=0,5

Wang: Training deep neural networks with 8-bit floating... - Google Scholar
https://scholar.google.com/scholar?q=related:-fJTnZiot-8J:scholar.google.com/&scioq=neural+net+without+floating+point&hl=nl&as_sdt=0,5

Wang: Training deep neural networks with 8-bit floating... - Google Scholar
https://scholar.google.com/scholar?cites=17273460269230846713&as_sdt=2005&sciodt=0,5&hl=nl

Kim: Bitwise neural networks - Google Scholar
https://scholar.google.com/scholar?cites=10418356286723963315&as_sdt=2005&sciodt=0,5&hl=nl

Kim: Bitwise neural networks - Google Scholar
https://scholar.google.com/scholar?q=related:s7mjDdhvlZAJ:scholar.google.com/&scioq=neural+net+without+floating+point&hl=nl&as_sdt=0,5

Nissen: Implementation of a fast artificial neural... - Google Scholar
https://scholar.google.com/scholar?cites=14619671890808380989&as_sdt=2005&sciodt=0,5&hl=nl

LogNet: Energy-efficient neural networks using logarithmic computation | IEEE Conference Publication | IEEE Xplore
https://ieeexplore.ieee.org/abstract/document/7953288

Lee: Lognet: Energy-efficient neural networks using... - Google Scholar
https://scholar.google.com/scholar?cites=11669823869029147025&as_sdt=2005&sciodt=0,5&hl=nl

Privacy error
https://mpedram.com/~massoud/Papers/research_projects_papers/Mahdi/NullaNet.pdf

Logarithm-approximate floating-point multiplier is applicable to power-efficient neural network training - ScienceDirect
https://www.sciencedirect.com/science/article/abs/pii/S0167926019305826

Helfrich: Local Phansalkar threshold implementation... - Google Scholar
https://scholar.google.com/scholar_lookup?journal=GitHub+repository&title=Local+Phansalkar+threshold+implementation+in+ImageJ&author=S+Helfrich&author=C+Rueden&author=C+Dietz&author=L+Yang&publication_year=2016&

imagej-ops/src/main/java/net/imagej/ops/threshold/localPhansalkar/LocalPhansalkarThreshold.java at master · imagej/imagej-ops
https://github.com/imagej/imagej-ops/blob/master/src/main/java/net/imagej/ops/threshold/localPhansalkar/LocalPhansalkarThreshold.java

[PDF] Adaptive document image binarization | Semantic Scholar
https://www.semanticscholar.org/paper/Adaptive-document-image-binarization-Sauvola-Pietik%C3%A4inen/be97923dbcdaf8b1496b637ed156656d8874f552

Sensors | Free Full-Text | Robust Combined Binarization Method of Non-Uniformly Illuminated Document Images for Alphanumerical Character Recognition
https://www.mdpi.com/1424-8220/20/10/2914

Binarization Algorithm Based on Side Window Multidimensional Convolution Classification - PMC
https://www.ncbi.nlm.nih.gov/pmc/articles/PMC9371195/


https://academic.oup.com/comjnl/article/28/3/330/405605?login=false :: pdf link generates a new link with a timeout token that has a relatively short timeout; when your download process is slow to proceed to the next step, the timeout may kick in (observed when downloading multiple pdfs in parallel). Don't know how a curl-based automated downloader will cope with this...

https://pdf.sciencedirectassets.com/271538/1-s2.0-S0304397512X00112/1-s2.0-S0304397512000023/main.pdf?X-Amz-Security-Token=IQoJb3JpZ2luX2VjEMj%2F%2F%2F%2F%2F%2F%2F%2F%2F%2FwEaCXVzLWVhc3QtMSJHMEUCIQDuGkHn8qD3RTOq%2FcX0tRYdwntDVryhi5ZNORdRlplpJwIgPfVtsuVNg%2FrXsugeNKmJ9mySePRWIyYCmcPGivWox9kqsgUIcBAFGgwwNTkwMDM1NDY4NjUiDELAU8O24zTU%2BXeNyyqPBb9Vcy%2FqiEvTV7SS96TWqPRcLZzRzzaJ7p3FnlCozUeXVFxD4XVdreovAX6AkWkhbaYuh1EmVyx6zXcawxp7RYVH1P23bq7r6v%2Bio723yd9eV1gjpUo64KXxdTiWhfaw6coYpQtT89wopiX2zeGdRuiPTXPSJP6lN%2FmHR7Hsp5E5SirXSZhejO%2Bgq29tZP7sy03Js4N6P%2BhgEwctKqPVHChkSI5SaIH%2FSK9%2F2BrL9VAWRGlC%2BeRM4tDVxBJmWyyXMjr%2B0H3h0f%2FxtmJ8Xq7lSH4%2FS658oajF9KZZlQDWY1i611Tuum5ZuhswT1vnoxigzlemDDIFXl2ujOZrzupw5GOqnoeiySy%2BycbcoYMf5dYHcYmbRPtwulMzKiX4zsYhwm3IMgAKmqOl6ANY%2BDh5QQCLR55xtqpcdnw7hiTs358xYkvxUhzsVgVTkzQvxJ42%2FLxdOaQ7c3N7CTmgJQsUmQJIU9fpzIj2Yf7%2BfCsJf%2FTaPmj0l4nTSazbBINmSBkXK%2FVFyrkLLxPg6d4O%2BOfYgcv5UiwvVvhCFUwn8wUDl7%2BsmlT1csbVVRgr0fL7khV2fjbwS%2FQ4EbMkMWoY409Wuxmd2%2Fsj%2F7od7Vzrp0POMRuLmGn4kCwsIvzY4DtzobUyLE6M80qSrm2GR7Awigo5aFl8EkW8TWz3RmFzJ2B7tg9XjFXp332tS9UsoUuacdnK8T%2BEtmdTEFd5cQ02eeeoC1byjE5ykHs%2FNHj5mWequBk8G0WsXZSIZNjCD7%2BRdta3vvbLnR1Y7Tkl%2F0TBfd%2BsJNlnnwYl%2BZh9yXTIYSgnDrpCdwQKYl%2BJTt9oWjoSPT5MCXJGPUxjixbX5Rd77bq67h%2BbUewFT%2FPXvLseNmcB8I4wt5OOrQY6sQG6XwPLMZvB5OfwRMsFEXa8%2Fdu9AVBzmywedqj7kND8q%2FNO6uL1PNTiTDjDf78A6jWL0HovIYHg1d3GeMuViiKBBrX1AJEdoZFHnNcTtQPhWE6s17JuxWaKoe8JcCizUnzbF0jfYgvACs%2F6r2X110%2BtLMrA8GtzSN37FTXJuCfosCtP%2BYcTWtN%2BAlsvfVbWt1FtMuRxEQbqZGvqUz9c7nSlP6Hect9AxxpZTfX6N2zzu68%3D&X-Amz-Algorithm=AWS4-HMAC-SHA256&X-Amz-Date=20240114T075836Z&X-Amz-SignedHeaders=host&X-Amz-Expires=300&X-Amz-Credential=ASIAQ3PHCVTY3XBDDF4F%2F20240114%2Fus-east-1%2Fs3%2Faws4_request&X-Amz-Signature=dd17eb1114d645e6039c2a9b84e653e88e58044bdfe06cebaf30c4b7dd44a521&hash=690c21051e82ef2156dc987910afc23fb709953c073baad2a2dae2a5571802f6&host=68042c943591013ac2b2430a89b270f6af2c76d8dfd086a07176afe7c76c2c61&pii=S0304397512000023&tid=spdf-3350a5b4-05bf-496f-b8be-23095f86cf1c&sid=b047f5dd9a393241739bc306d9ad86427a5dgxrqb&type=client&tsoh=d3d3LnNjaWVuY2VkaXJlY3QuY29t&ua=18055a56535351540254&rr=845455b55a330b42&cc=be :: same kind of behaviour: timeout on the pdf thus produces the original start page (HTML), from where you can get the pdf again via "View PDF" button, which generates a new token for this link...





## Dokumen.pub, of course, has its own challenges

Though, granted, this one is more about *captcha breaking* and should simply be delegated, i.e. *turked*. As in: *no can do automaticallo!*


Numerical Methods of Scientific Computing [2&nbsp;ed.] 9065623639 - DOKUMEN.PUB
https://dokumen.pub/numerical-methods-of-scientific-computing-2nbsped-9065623639.html

Queue | Concise Guide to Numerical Algorithmics. The Foundations and Spirit of Scientific Computing 9783031217616, 9783031217623 - DOKUMEN.PUB
https://dokumen.pub/qdownload/concise-guide-to-numerical-algorithmics-the-foundations-and-spirit-of-scientific-computing-9783031217616-9783031217623.html

Queue | Isogeometric methods for numerical simulation 9783709118429, 9783709118436 - DOKUMEN.PUB
https://dokumen.pub/qdownload/isogeometric-methods-for-numerical-simulation-9783709118429-9783709118436.html



April 2024: SemanticScholar PDF browser view (HTML page) has a non-working 'Download' top menu item for some PDFs, e.g.:
- https://browse.arxiv.org/pdf/2102.10346v1.pdf by way of https://www.semanticscholar.org/paper/Convergence-Rates-of-Stochastic-Gradient-Descent-Wang-Gurbuzbalaban/65505eb6bb0ef9b5990a694da6697e1a9114d4e7 --> https://www.semanticscholar.org/reader/65505eb6bb0ef9b5990a694da6697e1a9114d4e7
Fun note: this happens in latest Chrome, but when visiting the semanticscholar link above using WaterFox, the 'Download' menu works: while it doesn't exactly *download* but shows the fetched PDF in the WaterFox PDF native viewer, it results in us being able to fetch that PDF. 
- ditto: https://www.semanticscholar.org/paper/Global-Convergence-and-Stability-of-Stochastic-Patel-Tian/ac4a78a38713450fe9ba1419ca07f8561e13e3d5
- ditto: https://proceedings.mlr.press/v125/woodworth20a.html --> "Download PDF" at bottom of page --> http://proceedings.mlr.press/v125/woodworth20a/woodworth20a.pdf works in WaterFox, silently fails in Chrome.
- ditto: https://paperswithcode.com/paper/stochastic-variance-reduced-ensemble-1 : both the PDF and CVPR2022 PDF buttons...
- ditto https://paperswithcode.com/paper/towards-sample-efficient-overparameterized-1 : both PDF buttons; works in MSEdge though. (Haven't tested the previous URLs with MSEdge so don't read this as yet another anomaly; I started to dump these bothersome URLs in different browsers just now...)
- ditto: https://www.scirp.org/journal/paperinformation?paperid=115011 (PDF link works in WaterFox and MSEdge; NOTE: seems this is a very slow download as Chrome seems to have fetched it after several minutes, while the others were only a little faster but showed much better UX: visual feedback of the download progress.)
- ditto: https://www.semanticscholar.org/paper/The-trade-off-between-long-term-memory-and-for-Ribeiro-Tiels/18c823a3763f88f9a6f4a3ceefdcc4a74dd10ab1 :: follow the link fails in Chrome, succeeds in  WaterFox; also succeeds in MSEdge, but that one then DOES NOT APPEND THE `.pdf` FILE EXTENSION to the 'Save As' target, so there's probably something rotten with the response headers / MIME type, I suppose. To be debugged... ([1906.08482 (arxiv.org)](https://export.arxiv.org/pdf/1906.08482))
- ditto: https://aclanthology.org/L12-1475/
   This is why the PDF download fails for this one, as per console window in Chrome: `Mixed Content: The site at 'https://aclanthology.org/' was loaded over a secure connection, but the file at 'http://www.lrec-conf.org/proceedings/lrec2012/pdf/806_Paper.pdf' was loaded over an insecure connection. This file should be served over HTTPS. See https://blog.chromium.org/2020/02/protecting-users-from-insecure.html for more details.` 
   FireFox has no problem with this.
- 

- https://www.sciencedirect.com/science/article/pii/S0377042717303497 :: View PDF shows a 'verify you're human' page a la Cloudflare, where you need to click before the PDF will be fetched.

- sci-hub.ru shows empty page in FireFox / WaterFox / etc. and basic 404-not-found page in Chrome / MSEdge for this DOI: [https://doi.org/10.1002/acs.3382](https://doi.org/10.1002/acs.3382)
- ditto: https://sci-hub.ru/https://doi.org/10.1016/j.acha.2021.12.009
- 


- https://core.ac.uk/reader/147907050 shows the PDF reader provided by core (website), but Chrome doesn't render the controls, thus making it unusable. None of the browsers I use did render the control icons, by the way, so this is not a Chrome issue. A quick check of the debug messages in the browser reveal several 404 reports under the hood.




- https://link.springer.com/chapter/10.1007/978-3-540-46332-0_8 :: provides a PREVIEW PDF download, which isn't the real thing. Follow the DOI instead! Springer has more pages like this one, where you get a preview = few pages PDF, so it's generally wise to follow the DOI there and at least compare file content / size, I suppose. *To Be Researched.*
- 

- https://cubs.cedar.buffalo.edu/images/pdf/pub/Binarization-and-Cleanup-of-Handwritten-Text-from-Carbon-Copy-Medical-Form-Images.pdf :: curl --> error curl: (60) schannel: SNI or certificate check failed: SEC_E_WRONG_PRINCIPAL (0x80090322) - The target principal name is incorrect.


