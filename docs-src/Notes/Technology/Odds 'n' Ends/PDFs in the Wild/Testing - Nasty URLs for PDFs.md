# Testing :: Nasty URLs for PDFs

> **Note**: Also check these for more PDF download/fetching woes:
>
> - [[../curl - command-line and notes|curl - command-line and notes]] (sections about *nasty PDFs*)
> - [[PDF cannot be Saved.As in browser (Microsoft Edge)|PDF cannot be 'Saved As' in browser (Microsoft Edge)]]
> - [[Testing - PDF URLs with problems]]
> - [[MuPDF testing notes - particular PDFs]]
> - [[../../../Progress in Development/Testing & Evaluating/PDF `bulktest` test run notes at the bleeding edge|PDF `bulktest` test run notes at the bleeding edge]]
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




## HTML pages with problems

* [Irreducible and Aperiodic Markov Chains (uni-ulm.de)](https://www.mathematik.uni-ulm.de/stochastik/lehre/ss06/markov/skript_engl/node12.html) -- expired certificate gets you blocked


## Download / fetch weirdnesses & miscellaneous oddities

- https://epubs.siam.org/doi/epdf/10.1137/1.9781611976472.5

  Linux/firefox b0rks on this one with 'popup blocked' warning. However, the 'popup' is what will allow you to get the PDF, so you'll have allow popups for the given siam.org website.

- Academia.com of course is a nuisance as it always wants you to log in with either google or facebook: one big tracking hazard.

* https://ietresearch.onlinelibrary.wiley.com/doi/epdf/10.1049/iet-epa.2016.0190 : Linux/Firefox report that it blocked popup(s) when you click on the download button in this page to get an actual copy of the PDF: you need to enable popups for the Wiley subdomain for the PDF file to be actually downloaded. See if we can circumvent this with cURL?



