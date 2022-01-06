# Testing :: Nasty URLs for PDFs

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

  Renders as PDF in browser, but 'Save As' produces the HTML. Only clicking on the content and then hitting right-click meenu -> Save As will prduce the PDF.
  
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
-  
  
