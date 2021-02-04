<!doctype html>
<html lang="en">
  <head>
    <meta charset="utf-8">
    <meta name="viewport" content="width=device-width, initial-scale=1.0">
    
    <p>There are several technology chunks in Qiqqa which are in need of updating, a couple of which translate to: company or library we use is defunct, what do we use that might be solid for the next couple o’ years?</p>
<h1>Technology areas and their <em>function</em> in Qiqqa:</h1>
<h2>Pdf rendering = pdf viewer/reader built into Qiqqa UI</h2>
<p>Uses Sorax (defunct) and has serious trouble with several pdfs - including quite a few tech datasheets in my lib. Try OnSemi / Fairchild ones and chances are you get blank pages only.</p>
<h2>PDF text extraction</h2>
<p>PDF text extraction = PDF OCR (text recognition) <em>and</em> text+position coordinates (!) extraction from pdf docs which have no or crummy embedded text: many pdfs are produces as pure image-only (“shapes&quot; or bitmaps) - this is done so you can text search &amp; drag mouse over pdf pages to “select text” for copy/paste.</p>
<h2>Metadata extraction</h2>
<p>Metadata extraction - from both pdfs itself and from (often less cooperative) websites such as google scholar. (that one is a pain for everybody as scholar is engineered with flesh humans as end reception point in mind and vehemently aggressive in kicking any robot/machine off their front porch. This “tug of war” is happening in the Qiqqa Sniffer which currently is still stuck with a mozilla/firefox v33 engine. (microsoft academic search doesn’t render in that one, for example)</p>
<h2>FTS aka Full Text Search</h2>
<p>FTS aka Full Text Search or “googling your stuff locally” 😉 is done using an old <a href="http://lucene.Net">lucene.Net</a> v3 engine. Not the biggest issue, as that one is not failing yet, but would be great if we could upgrade. Meanwhile <a href="http://lucene.net">lucene.net</a> as a project is still in v4 while tge world around has moved fast: lucene (java) is at v8.x while there’s plenty layered in top that’s the buzz of the town these days: solr, elasticsearch, etc. Again, not a must-have like the previous items, but has been investigated.</p>
<h2>Application performance</h2>
<p>Application performance: the code is pretty intertwined and some times either UI or background processes suffer. I’ve done a lot of tweaking there as that was a major problem for me combined with the bad error handling in the processes back when it was just open sourced, but not all that work/change is useful for everybody as at least one person filed a github issue. Anyway, this screams “refactoring” and then the craziness begins for real. 😃</p>
<h2>UI/UX performance and modernization</h2>
<p>Qiqqa doesn’t look “hip” and as you may know, the look of a thing is half the sale. 😃 So we got work to do there. That’s currently done in WPF / C#. NET and I am not a happy user if that tech, though I’ve done C# work back in the day. So, with me, you’ll encounter a mix of want-to-improve &amp; loath-to-dive-in emotion on this subject. Other folks have already taken a look (or more, @mahfiaz !) and hopefully will assist some more. I’m also looking at “migrating” this layer to something like Electron (that’s nodeJS based, BTW) so I don’t have to deal with WPF and can work in a HTML5/CSS stack for the UI like I’ve done the last couple of years.</p>
<h2>Cross Platform</h2>
<p>Cross Platform, i. e. have Qiqqa run on ubiquitous linux rigs. Quantile also had a mobile app, but I’ve never seen or used it, nor was it open sourced, so I don’t know the breath of functionality of that one, but “mobile” is another target platform to consider.</p>
<p>Incidentally, that item moved us from “update/upgrade” to &quot;new features“ section.</p>
<p>Qiqqa has never been published for Linux and WPF for the UI is a showstopper for that one. As I don’t want to leave that part of the (student + academia + less moneyed? ) market out of scope for it is simply too large and growing, we gotta do <em>something</em>. 😉</p>

  </head>
  <body>

    <p>There are several technology chunks in Qiqqa which are in need of updating, a couple of which translate to: company or library we use is defunct, what do we use that might be solid for the next couple o’ years?</p>
<h1>Technology areas and their <em>function</em> in Qiqqa:</h1>
<h2>Pdf rendering = pdf viewer/reader built into Qiqqa UI</h2>
<p>Uses Sorax (defunct) and has serious trouble with several pdfs - including quite a few tech datasheets in my lib. Try OnSemi / Fairchild ones and chances are you get blank pages only.</p>
<h2>PDF text extraction</h2>
<p>PDF text extraction = PDF OCR (text recognition) <em>and</em> text+position coordinates (!) extraction from pdf docs which have no or crummy embedded text: many pdfs are produces as pure image-only (“shapes&quot; or bitmaps) - this is done so you can text search &amp; drag mouse over pdf pages to “select text” for copy/paste.</p>
<h2>Metadata extraction</h2>
<p>Metadata extraction - from both pdfs itself and from (often less cooperative) websites such as google scholar. (that one is a pain for everybody as scholar is engineered with flesh humans as end reception point in mind and vehemently aggressive in kicking any robot/machine off their front porch. This “tug of war” is happening in the Qiqqa Sniffer which currently is still stuck with a mozilla/firefox v33 engine. (microsoft academic search doesn’t render in that one, for example)</p>
<h2>FTS aka Full Text Search</h2>
<p>FTS aka Full Text Search or “googling your stuff locally” 😉 is done using an old <a href="http://lucene.Net">lucene.Net</a> v3 engine. Not the biggest issue, as that one is not failing yet, but would be great if we could upgrade. Meanwhile <a href="http://lucene.net">lucene.net</a> as a project is still in v4 while tge world around has moved fast: lucene (java) is at v8.x while there’s plenty layered in top that’s the buzz of the town these days: solr, elasticsearch, etc. Again, not a must-have like the previous items, but has been investigated.</p>
<h2>Application performance</h2>
<p>Application performance: the code is pretty intertwined and some times either UI or background processes suffer. I’ve done a lot of tweaking there as that was a major problem for me combined with the bad error handling in the processes back when it was just open sourced, but not all that work/change is useful for everybody as at least one person filed a github issue. Anyway, this screams “refactoring” and then the craziness begins for real. 😃</p>
<h2>UI/UX performance and modernization</h2>
<p>Qiqqa doesn’t look “hip” and as you may know, the look of a thing is half the sale. 😃 So we got work to do there. That’s currently done in WPF / C#. NET and I am not a happy user if that tech, though I’ve done C# work back in the day. So, with me, you’ll encounter a mix of want-to-improve &amp; loath-to-dive-in emotion on this subject. Other folks have already taken a look (or more, @mahfiaz !) and hopefully will assist some more. I’m also looking at “migrating” this layer to something like Electron (that’s nodeJS based, BTW) so I don’t have to deal with WPF and can work in a HTML5/CSS stack for the UI like I’ve done the last couple of years.</p>
<h2>Cross Platform</h2>
<p>Cross Platform, i. e. have Qiqqa run on ubiquitous linux rigs. Quantile also had a mobile app, but I’ve never seen or used it, nor was it open sourced, so I don’t know the breath of functionality of that one, but “mobile” is another target platform to consider.</p>
<p>Incidentally, that item moved us from “update/upgrade” to &quot;new features“ section.</p>
<p>Qiqqa has never been published for Linux and WPF for the UI is a showstopper for that one. As I don’t want to leave that part of the (student + academia + less moneyed? ) market out of scope for it is simply too large and growing, we gotta do <em>something</em>. 😉</p>


    <footer>
      © 2020 Qiqqa Contributors ::
      <a href="https://github.com/GerHobbelt/qiqqa-open-source/blob/docs-src/Progress in Development/Qiqqa Functionalities & Technology Areas.md">Edit this page on GitHub</a>
    </footer>
  </body>
</html>
