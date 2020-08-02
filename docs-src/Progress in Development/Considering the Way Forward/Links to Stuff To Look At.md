
# Full Text Search Engines

index.rst
https://xapian.org/docs/

.net - Indexing .PDF, .XLS, .DOC, .PPT using Lucene.NET - Stack Overflow
https://stackoverflow.com/questions/4905271/indexing-pdf-xls-doc-ppt-using-lucene-net

Welcome to the Lucene.Net website! | Apache Lucene.NET 4.8.0
https://lucenenet.apache.org/

How to compile and use Xapian on Windows with C# - CodeProject
https://www.codeproject.com/Articles/71593/How-to-compile-and-use-Xapian-on-Windows-with-C






# Google Scholar

You want BibTeX data, citations, etc. from Google.

Let's not be squeamish about it: in exchange, Google wants your *soul*. Or your money, but in the case of Google Scholar, only your *soul* will do: lots of effort has been expended by Google in the last 1-2 years to **block everyone that's not provably a human using a modern browser *manually***. And even than, you're limited to only so much use of Scholar before you're supposed to go back to basic Search, which at least can soak you in ads.

Sounds opinionated? Well, it's "*follow the money*" as usual: if you *assume* Google is interested in **human activity only** -- or more specifically stated: Google is **only interested in humans who can click on ads** -- then *all* behavioural changes of the Google Scholar site over the recent months/years are *reasonable*: 

- kill/thwart any robot activity on the site (heck, they could only be crawling, and *we, the Google* are the only ones who should be doing *that*!)
- kill/thwart any old browsers: that's humans with browsers/machines which will probably break on all those fancy modern commercial websites anyway, while using an outdated browser also clearly marks you as *having no purchase mandate or purchase budget*: you're either a worker bee deep inside corporate firewalls or you're not, ah, sufficiently pecuniary advanced to purchase something modern, with a recent operating system and ditto browser. That's near zero buyer's potential right there. Hence, we're not interested. Bye.
- kill/thwart any human that's potentially wiping ads using filtering proxies (e.g. [privoxy](https://www.privoxy.org/)) and such: you might be tech savvy, but we are *savvier*: if your HTTP request signature doesn't look like it's coming from a vanilla Chrome/Safari/FireFox/Edge browser, you're probably not worth our while either. You get a few answers to show you we mean well, but nothing to generous.
  - when you 'log in' into your Google account while using Scholar, emperical data hints that you get more responses before you're hit with a "not-a-robot?" query by your friends at Google.
- don't give out many responses per unit of time: that sounds like crawling or heavy use: that should be monetized and while it isn't, we're going to send you through captcha hoops.

The nett effect is that **any embedded browser** is dead in the water unless you make some serious effort to make it not appear so. The corrolary of this, by the way, is that a Google Scholar session inside an `\<iframe>` is a **not-happening** situation either: I have yet to find out exactly what metrics Google is inspecting to detect embedded browsers, but it certainly looks at the `User-Agent` and possibly at traffic, for it's rather *uncanny* how good Google has become at detecting these things...

This translates to the Qiqqa Sniffer being a, ah, '*challenge*' to make it work again in 2020 A.D.: one must make it look like it's the genuine Chrome browser and no hanky panky happenin', or you get toasted with CAPTCHA/robot checks and 503/429 HTTP error reponses, i.e. *no results what-so-ever* until you return the next day and try again.

Meanwhile, Google also seems to have added a VPN exit node blacklist of some sort as using a VPN to perform Google Scholar searches doesn't seem to help: it was rather *worse* than Scholar-ing from the private node. Having had a look at `scholarly` (below) which uses the `tor` network as a randomizing VPN/proxy rig might fly better for a while, but I don't know how long Google will keep up appearances regarding `tor` and disadvantaged web users from beyond the Great Firewall and elsewhere: in the end, they're in it for the money. My opinion here: when / as-long-as the `tor` road works, heck, let's do it!



 

## Analysis Notes

- at least *some* of the Python repositories concerning themselves with Google Scholar crawling are using the python module [`scholarly`](https://pypi.org/project/scholarly/) under the hood.
  
  Upon closer inspection of [the source code](https://github.com/scholarly-python-package/scholarly), that one appears to circumvent the Google Scholar restrictions by using a few tricks -- see also [`scholarly/_navigator.py`](https://github.com/scholarly-python-package/scholarly/blob/master/scholarly/_navigator.py#L82):

  + using the `tor` **proxy** as a randomizing proxy, if available locally.
  
    When a CAPTCHA from Google is detected, [the tor proxy is refreshed](https://github.com/scholarly-python-package/scholarly/blob/master/scholarly/_navigator.py#L113) so our next (re)try would exit on a different tor network node.

  + sending [a randomized `User-Agent`](https://github.com/scholarly-python-package/scholarly/blob/master/scholarly/_navigator.py#L90) to thwart Google's same-user detection heuristics. The UserAgent is randomly picked from a sane set using [the Python module `fake_useragent`](https://pypi.org/project/fake-useragent/), which uses real-world User-Agent strings from [useragentstring.com] -- see also [its source code at github](https://github.com/hellysmile/fake-useragent).

  + [randomizing the GoogleID in the cookie that is sent along](https://github.com/scholarly-python-package/scholarly/blob/master/scholarly/_navigator.py#L91) with the search query.

---

  

## jkeirstead/scholar: Analyse citation data from Google Scholar
https://github.com/jkeirstead/scholar

The scholar R package provides functions to extract citation data from Google Scholar. In addition to retrieving basic information about a single scholar, the package also allows you to compare multiple scholars and predict future h-index values.

### Basic features

Individual scholars are referenced by a unique character string, which can be found by searching for an author and inspecting the resulting scholar homepage. For example, the profile of physicist Richard Feynman is located at http://scholar.google.com/citations?user=B7vSqZsAAAAJ and so his unique id is B7vSqZsAAAAJ.

### Analysis Notes

- Does some work to carry google cookies -- see [`/R/utils.R`](https://github.com/jkeirstead/scholar/blob/master/R/utils.R) and [`/R/scholar.r`](https://github.com/jkeirstead/scholar/blob/master/R/scholar.r)

- Does not do any randomization or other Google Scholar thwarting.

- However, it *does* detect Google HTTP response code 429:

  ```R
  else if (httr::status_code(resp) == 429) {
    stop("Response code 429. Google is rate limiting you for making too many requests too quickly.")
  }
  ```





## venthur/gscholar: Query Google Scholar with Python
https://github.com/venthur/gscholar

### Using `gscholar` as a command line tool

`gscholar` provides a command line tool, to use it, just call `gscholar` like:

```sh
$ gscholar "albert einstein"
```

or

```sh
$ python3 -m gscholar "albert einstein"
```

Getting more results:

```sh
$ gscholar --all "some author or title"
```

Same as above but returns up to 10 bibtex items. (Use with caution Google will assume you're a bot an ban you're IP temporarily)

Querying using a pdf:

```sh
$ gscholar /path/to/pdf
```

Will read the pdf to generate a Google Scholar query. It uses this query to show the first bibtex result as above.

Renaming a pdf:

```sh
$ gscholar --rename /path/to/pdf
```

Will do the same as above but asks you if it should rename the file according to the bibtex result. You have to answer with "y", default answer is no.


### Analysis Notes

Nothing fancy. Does use [Google Scholar cookie](https://github.com/venthur/gscholar/blob/master/gscholar/gscholar.py#L64) to request BibTeX ([or other format](https://github.com/venthur/gscholar/blob/master/gscholar/gscholar.py#L34)).




## ckreibich/scholar.py: A parser for Google Scholar, written in Python
https://github.com/ckreibich/scholar.py

scholar.py is a Python module that implements a querier and parser for Google Scholar's output. Its classes can be used independently, but it can also be invoked as a command-line tool.

The script used to live at http://icir.org/christian/scholar.html, and I've moved it here so I can more easily manage the various patches and suggestions I'm receiving for scholar.py.

### Analysis Notes

No fancy stuff. Is aware of Scholar quirks but has no smart circumventions.

Does have [a nice list of query options](https://github.com/ckreibich/scholar.py/blob/master/scholar.py) for Scholar though.

Has 61 open issues at the time of writing and 44 pull requests. For example, there's the dreaded [HTTP Error 503: Service Unavailable](https://github.com/ckreibich/scholar.py/issues/106) we've encountered so often ourselves.

Might be worth to merge several of those and mix in the `scholarly` library's smart circumvention tactics to make this a viable tool in 2020 A.D.





## WittmannF/sort-google-scholar: Sorting Google Scholar search results based on the number of citations
https://github.com/WittmannF/sort-google-scholar

This Python code ranks publications data from Google Scholar by the number of citations. It is useful for finding relevant papers in a specific field.

The data acquired from Google Scholar is Title, Citations, Links and Rank. A new columns with the number of citations per year is also included. The example of the code will look for the top 100 papers related to the keyword, and rank them by the number of citations. This keyword can eiter be included in the command line terminal ($python sortgs.py --kw 'my keyword') or edited in the original file. As output, a .csv file will be returned with the name of the chosen keyword ranked by the number of citations.

**GOOGLE COLAB**: Try running the code using Google Colab! No install requirements! Limitations: Can't handle robot checking, so use it carefully.

Handling robot checking with Selenium and Chrome browser: You might be asked to manually solve the first captcha for retrieving the content of the pages.


### Analysis Notes

Nice, but like most of the others: no circumvention techniques. Uses Selenium to kickstart your Chrome browser so you can answer the Google CAPTCHAs, but that's it.



## beloglazov/zotero-scholar-citations: Zotero plugin for auto-fetching numbers of citations from Google Scholar
https://github.com/beloglazov/zotero-scholar-citations

This is an add-on for Zotero, a research source management tool. The add-on automatically fetches numbers of citations of your Zotero items from Google Scholar and makes it possible to sort your items by the citations. Moreover, it allows batch updating the citations, as they may change over time.

When updating multiple citations in a batch, it may happen that citation queries are blocked by Google Scholar for multiple automated requests. If a blockage happens, the add-on opens a browser window and directs it to http://scholar.google.com/, where you should see a Captcha displayed by Google Scholar, which you need to enter to get unblocked and then re-try updating the citations. It may happen that Google Scholar displays a message like the following "We're sorry... but your computer or network may be sending automated queries. To protect our users, we can't process your request right now." In that case, the only solution is to wait for a while until Google unblocks you.


### Analysis Notes

That stuff [happens here](https://github.com/beloglazov/zotero-scholar-citations/blob/master/chrome/content/scripts/zoteroscholarcitations.js#L173).



## lintool/scholar-scraper: Scrapes citation statistics from Google Scholar
https://github.com/lintool/scholar-scraper

I wrote this simple utility to scrape citation statistics of researcher profiles on Google Scholar, using it as an opportunity to learn node.js. I began with a list of information retrieval researchers, but have since expanded to include a separate list of researchers in human-computer interaction. The results are here.

**Editorial note**: This list contains only researchers who have a Google Scholar profile; names were identified by snowball sampling and various other ad hoc techniques. If you wish to see a name added, please email me or send a pull request. I will endeavor to periodically run the crawl to gather updated statistics. Of course, scholarly achievement is only partially measured by citation counts, which are known to be flawed in many ways. Evaluations of scholars should include comprehensive examination of their research contributions.

### Analysis Notes

Basic Scholar access, nothing fancy. Done [here](https://github.com/lintool/scholar-scraper/blob/master/scrape.js#L12).




## jimmytidey/bibnet-google-scholar-scraper
https://github.com/jimmytidey/bibnet-google-scholar-scraper

### Google and rate limiting

This software should be used in compliance with Google's rules. Much as Zotero uses Google Scholar's results pages to populate it's metadata fields, this seems like a reasonable use of their service.

There is a keys.js where you can provide cookie details, so that you are querying Google as a logged in user. I don't think this adds any particular advantage.



## dnlcrl/PyScholar: A 'supervised' parser for Google Scholar
https://github.com/dnlcrl/PyScholar

A "supervised" parser for Google Scholar, written in Python.

PyScholar is a command line tool written in python that implements a querier and parser for Google Scholar's output. This project is inspired by scholar.py, in fact there is a lot of code from that project, the main difference is that scholar.py makes use of the urllib modules, thus, so no javascript, and given that people at big G don't like you to scrape their search results, when the server responses the "I'm not a robot" page, you simply get no output from scholar.py, for a long time. Instead PyScholar makes use of selenium webdriver giving the ability to see what's going on and in case the "I'm not a robot" shows up you can simply pass the challenge manually and let the scraper continue his job.

Also there are some other new features I inclulded from my scholar.py fork, that are: json exporting of the reults, "starting result" option, and the potential ability to get an unlimited number results, even if it seems that results are limited on server-side to approximately one thousand.

### Analysis Notes

Hmmmm... Last work was done 5 years ago, but at least this sounds like something. It doesn't have `scholarly`'s twists, but it does have the 'pop up Chrome via Selenium so user can do the captcha thing' approach at least.

Interesting. Should be tested...



## edsu/etudier: Extract a citation network from Google Scholar
https://github.com/edsu/etudier

étudier is a small Python program that uses Selenium and requests-html to drive a non-headless browser to collect a citation graph around a particular Google Scholar citation or set of search results. The resulting network is written out as a Gephi file and a D3 visualization using networkx. The D3 visualization could use some work, so if you add style to it please submit a pull request.

If you are wondering why it uses a non-headless browser it's because [Google is quite protective of this data](https://www.quora.com/Are-there-technological-or-logistical-challenges-that-explain-why-Google-does-not-have-an-official-API-for-Google-Scholar) and routinely will ask you to solve a captcha (identifying street signs, cars, etc in photos). étudier will allow you to complete these tasks when they occur and then will continue on its way collecting data.


### Analysis notes

**Follow the money**: following the link in the README above, Quora says:


> Aaron Tay, academic librarian who has studied, blogged and presented on Google scholar
>
> Updated August 11, 2016
>
> I’ve read or heard someone say that Google Scholar is given privileged access to crawl Publisher,aggregator (often enhanced with subject heading and controlled vocab) and none-free abstract and indexing sites like Elsevier and Thomson Reuters’s Scopus and Web of Science respectively.
>
> Obviously the latter two wouldn’t be so wild about Google Scholar offering a API that would expose all their content to anyone since they sell access to such metadata.
>
> Currently you only get such content (relatively rare) from GS if you are in the specific institution IP range that has subscriptions. (Also If your institution is already a subscriber to such services such as Web of Science or Scopus, you library could usually with some work allow you access directly via the specific resource API!.)
>
> Even publishers like Wiley that are usually happy for their metadata to be freely available might not like the idea of a Google Scholar API. The reason is unlike Google, Google Scholar actually has access to the full text as well (which they sell)…. If the API exposes that…..
>
> There are of course technical solutions if Google wants the API enough, but why would they make the effort?
>
> As already mentioned Libraries do pay for things like Web of Science and Scopus and these services do provide APIs, so do consult a librarian if you have such access.
>
> Also Web Scale discovery services that libraries pay for such as Summon, Ebsco discovery service, Primo etc do have APIs and they come closest to duplicating a (less comprehensive version) Google Scholar API
>
> Another poor substitute to a Google Scholar API, is the Crossref Metadata Search. It’s not as comprehensive as Google Scholar but most major publishers do deposit their metadata.
> 
> ---
> 
> Tom Griffin, works at IEEE
> 
> Answered July 15, 2013
>
> Google doesn't have an API for Scholar likely for the same reason they don't have an API for web search - it would get overwhelmed by applications creating aggregation platforms (and running continuous queries) versus applications that just run on-demand, user-initiated lookups (like Mendeley linking out to Google Scholar).
>
> Couple this with the fact that Scholar is a philanthropic and they make no money off of it - there certainly isn't the pressing need for an API.
>
> There are, however, some openly available scrapers that work as an API. Of course, they only work well if they're tuned to the current structure of Scholar search results. One such example
>
> [A Parser for Google Scholar](https://www.icir.org/christian/scholar.html)
>
> The other thing to note is that Microsoft Academic Search does offer an API. You need to request a key, but other than that, it provides full programatic access to what the application returns using the web interface.
>
> [Microsoft Academic Search API](https://www.microsoft.com/en-us/research/project/academic/articles/sign-academic-knowledge-api/)





## VT-CHCI/google-scholar: nodejs module for searching google scholar
https://github.com/VT-CHCI/google-scholar

nodejs module for searching google scholar

### Analysis Notes

Has [request rate limiting and some HTTP error response detection logic](https://github.com/VT-CHCI/google-scholar/blob/master/index.js#L7), but no circumventions.

Done in JS, so might be handier to start with than some Python code if we want to roll it into Qiqqa.




## linhung0319/google-scholar-crawler: A crawler to crawl google scholar search page
https://github.com/linhung0319/google-scholar-crawler



這是我練習寫的一個爬取Google Scholar Search Page的網路爬蟲程式  

zhè shì wǒ liànxí xiě de yīgè pá qǔ Google Scholar Search Page de wǎng lù páchóng chéngshì  

This is a web crawler program that I practice to write to crawl Google Scholar Search Page 


How to Use 

1. Go to Google Scholar Search and enter the keywords you want to find, arrive at the first page of the Search Page, and copy the URL of this page 
2. Enter google_crawler.py and put the copied URL into start_url, start_url ='URL' 
3. p_key puts the keywords you want to find, n_key puts the keywords you don’t want to find in the Search Page, the title and content of each cell, if it contains the words in p_key, Crawler will tend to grab this paper. 

   In the page, if the title and content of each cell contain the words in n_key, Crawler will tend to ignore this paper. 

   In the above picture, because I want to find the paper related to sound, not to find the paper related to optics, so p_key Put sound-related words in the n_key and optical-related words in the n_key 

4. Set page = number, you can set the Google Search Page to be crawled. Setting too many pages will induce Google’s robot check 
5. Start execution The program runs google_crawler.py in Terminal. 

  `$ python google_crawler.py` will save the captured data ('title','year','url',...) in result.pickle and then run csvNdownload in terminal.py 

  `$ python csvNdownload.py` Convert the data to a CSV file, save it in a CSV folder, download links with Tag (PDF, HTML) to PDF files, and save them separately into PDF, HTML folder 

### Google Robot Check 

Google Search Page will be anti-crawling Detection. If the speed of downloading or viewing web pages is too fast, he will suspect that you are a robot and ban your IP address. 
The simplest method currently in mind is to use a VPN. When it is considered a robot, use it. 

VPN immediately changes its IP address. Now there are many free VPNs on the Internet that can be downloaded. 

If a message appears when running Crawler 

__findPages - Can not find the pages link in the start URL!! 

__findPages - 1. Google robot check might ban you from crawling!! 

__findPages - 2. You might not crawl the page of google scholar 

The solution is to use VPN and change IP




## cute-jumper/gscholar-bibtex: Retrieve BibTeX entries from Google Scholar, ACM Digital Library, IEEE Xplore and DBLP
https://github.com/cute-jumper/gscholar-bibtex

## alberto-martin/googlescholar: Code to extract bibliographic data from Google Scholar
https://github.com/alberto-martin/googlescholar

## leventsagun/scholar-bib-scraper: Get bibtex from saved Google Scholar articles
https://github.com/leventsagun/scholar-bib-scraper

## supasorn/GoogleScholarCopyBibTeX: Copy BibTeX on Google Scholar Search page with a single click
https://github.com/supasorn/GoogleScholarCopyBibTeX

## maikelronnau/google_scholar_paper_finder: A engine that searches for papers on Google Scholar based on keywords extracted from a text.
https://github.com/maikelronnau/google_scholar_paper_finder

## ag-gipp/grespa: A tool to obtain and analyze data from Google Scholar
https://github.com/ag-gipp/grespa

## yufree/scifetch: webpage crawling tools for pubmed, google scholar and rss
https://github.com/yufree/scifetch

## pykong/PyperGrabber: Fetches PubMed article IDs (PMIDs) from email inbox, then crawls PubMed, Google Scholar and Sci-Hub for respective PDF files.
https://github.com/pykong/PyperGrabber

## pentas1150/google-scholar-keyword-crwaler: 구글 스칼라에서 논문 제목을 단어 단위로 쪼개어 단어 카운팅해주는 크롤러
https://github.com/pentas1150/google-scholar-keyword-crwaler

## janosh/gatsby-source-google-scholar: Gatsby source plugin that pulls metadata for scientific publications from Google Scholar
https://github.com/janosh/gatsby-source-google-scholar

## Nicozheng/GoogleScholarCrawler: search, format, and download paper form google scholar
https://github.com/Nicozheng/GoogleScholarCrawler

## zjuiuczy/Google-Scholar: cs411 project
https://github.com/zjuiuczy/Google-Scholar

## fholstege/GoogleScholar_Research
https://github.com/fholstege/GoogleScholar_Research

## mattkearl/AdvancedSearchforGoogleScholar: Advanced Search for Google Scholar
https://github.com/mattkearl/AdvancedSearchforGoogleScholar

## hazelnutsgz/NaiveScholarMap: Interactive Visualization Of Google Scholar connection all past 20 years
https://github.com/hazelnutsgz/NaiveScholarMap

## Robinlovelace/scholarsearch: Tiny R package that makes it easy to search for academic publications on Google Scholar from the command line
https://github.com/Robinlovelace/scholarsearch

## TWRogers/google-scholar-export: Takes a Google Scholar profile URL and outputs an html snippet to add to your website.
https://github.com/TWRogers/google-scholar-export

## Ir1d/PKUScholar: A naive Google Scholar for PKU CS
https://github.com/Ir1d/PKUScholar

## leyiwang/nlp_research: The Research Spider for Anthology. This toolkit can automatically grab the papers information by given keywords in title. You can set the search params: the conference, publication date and the keywords in title . Then it will automatically save these papers' title, authors, download link, Google Scholar cited number and abstracts information in a Excel file.
https://github.com/leyiwang/nlp_research

## tyiannak/pyScholar: Python Library to Analyse and Visualise Google Scholar Metadata
https://github.com/tyiannak/pyScholar

## lyn716/CitationsGenerator: for generating cite relation of articles in google scholar
https://github.com/lyn716/CitationsGenerator

## alvinwan/webfscholar: Generate publications webpage from a google scholar
https://github.com/alvinwan/webfscholar

## troore/scholar-spider: A spider to get researcher information, citations, etc. from academic libraries like IEEE digital library, google scholar, microsoft academic, etc.
https://github.com/troore/scholar-spider

## miostudio/DawnScholar: A scholar search for The People who cannot access google scholar freely.
https://github.com/miostudio/DawnScholar

## chrokh/lit: Command line tool for systematic literature reviews using Google Scholar
https://github.com/chrokh/lit

## HTian1997/getarticle: getarticle is a package based on SciHub and Google Scholar that can download articles given DOI, website address or keywords.
https://github.com/HTian1997/getarticle

## fagnersutel/Google_Scholar: Google Scholar
https://github.com/fagnersutel/Google_Scholar

## tugcelmaci/GoogleScholarWebScraping: Google Scholar Web Scraping
https://github.com/tugcelmaci/GoogleScholarWebScraping

## guzmonne/scholar: Google Scholar web scrapper
https://github.com/guzmonne/scholar

## arunhpatil/GoogleScholar: Revised GoogleScholar-API from fredrike
https://github.com/arunhpatil/GoogleScholar

## ChenZhongPu/GoogleScholar: This is an extension for PopClip to search in a China's google scholar mirror site
https://github.com/ChenZhongPu/GoogleScholar

## jiamings/scholar-bibtex-keys: Convert bibtex keys to Google scholar style: [first-author-last-name][year][title-first-word]
https://github.com/jiamings/scholar-bibtex-keys

## lovit/google_scholar_citation_keywords: Google scholar citation keyword
https://github.com/lovit/google_scholar_citation_keywords

## dtczhl/dtc-google-scholar-helper: Google Scholar Helper
https://github.com/dtczhl/dtc-google-scholar-helper

## Andorreta/ScholarCrawler: Google Scholar Crawler
https://github.com/Andorreta/ScholarCrawler

## jjsantanna/google_scholar_crawler: Google Scholar Crawler
https://github.com/jjsantanna/google_scholar_crawler

## calebchiam/GoogleScholarScraper: Uses Python scholarly package to scrape relevant articles with given search terms, and then filters by title
https://github.com/calebchiam/GoogleScholarScraper

## MJHutchinson/GoogleScholarWebscraping: Utility to scrape google scholar for citation data
https://github.com/MJHutchinson/GoogleScholarWebscraping

## HilaryTraut/GoogleScholar_MetaAnalysis: A collection of simple scripts for retrieving citation information from a Google Scholar search.
https://github.com/HilaryTraut/GoogleScholar_MetaAnalysis

## idchuem/googleScholar-ParsingBot
https://github.com/idchuem/googleScholar-ParsingBot

## gursidak/web_scrapping_googleScholar
https://github.com/gursidak/web_scrapping_googleScholar

## juicecwc/GoogleScholarCrawler
https://github.com/juicecwc/GoogleScholarCrawler

## foool/GoogleScholarBibTex: Batch get BibTeXs of papers collected in your Google Scholar library.
https://github.com/foool/GoogleScholarBibTex

## shaileshkeskar/GoogleScholarSortCit
https://github.com/shaileshkeskar/GoogleScholarSortCit

## michaelvdow/GoogleScholarScript
https://github.com/michaelvdow/GoogleScholarScript

## Sharmelen/google_scholar: Extract data from google scholar
https://github.com/Sharmelen/google_scholar

## maximusKarlson/google-scholar: Sort google scholar results by citations.
https://github.com/maximusKarlson/google-scholar

## Xotic-Knight/Google_Scholar
https://github.com/Xotic-Knight/Google_Scholar

## smakonin/ScholarHacks: Scripts for hacking Google Scholar HTML pages.
https://github.com/smakonin/ScholarHacks

## nivgold/SCholar: Nice tool to get useful authors information from google scholar. wraps the scholarly package.
https://github.com/nivgold/SCholar

## mehdi-user/ScholarChart: ScholarChart: a charting userscript for Google Scholar
https://github.com/mehdi-user/ScholarChart

## nivgold/SCholar: Nice tool to get useful authors information from google scholar. wraps the scholarly package.
https://github.com/nivgold/SCholar

## mehdi-user/ScholarChart: ScholarChart: a charting userscript for Google Scholar
https://github.com/mehdi-user/ScholarChart

## lucasmun09/Bibimbap
https://github.com/lucasmun09/Bibimbap

## meander-why/konbini: crawler for google scholar
https://github.com/meander-why/konbini

## ViGeng/gs_spider: Google Scholar Spider
https://github.com/ViGeng/gs_spider

## huanhuanBOY/scholarsearch
https://github.com/huanhuanBOY/scholarsearch

## aiB0ld/CS411-Database-System: Google Scholar Visualization
https://github.com/aiB0ld/CS411-Database-System

## sraashis/scholary: Scrap google scholar
https://github.com/sraashis/scholary

## devteamepic/scholar-parser: Parser for google scholar
https://github.com/devteamepic/scholar-parser

## esther730/Google-Scholar-Web-Crawling
https://github.com/esther730/Google-Scholar-Web-Crawling

## charlesduan/scholar2tex: Convert Google Scholar cases to LaTeX
https://github.com/charlesduan/scholar2tex

## Bearzilasaur/ScholarScraper: Repository for a Google Scholar scraper for literature reviews.
https://github.com/Bearzilasaur/ScholarScraper

## dilumb/scholarlyPull: Pull author details and papers from Google Scholar
https://github.com/dilumb/scholarlyPull

## toolbuddy/ScholarJS: A parser for Google Scholar, written in JavaScript.
https://github.com/toolbuddy/ScholarJS

## alegione/ScholarPlot: Shiny web tool for visualising and exporting google scholar data
https://github.com/alegione/ScholarPlot

## tihbe/google_scholar_trend: A simple scientific paper based trend viewer using the number of results in Google Scholar.
https://github.com/tihbe/google_scholar_trend

## brunojus/google-scholar-crawler
https://github.com/brunojus/google-scholar-crawler

## siwalan/google-scholar-citation-scrapper: Simple scrapper for Google Scholar Data
https://github.com/siwalan/google-scholar-citation-scrapper

## lucgerrits/google-scholar-scraper: Basic Google Scholar scraper written in python.
https://github.com/lucgerrits/google-scholar-scraper

## madhawadias/google-scholar-profile-scraper: Scrape profiles of google scholar authors.
https://github.com/madhawadias/google-scholar-profile-scraper

## gerryreilly/google_scholar_scrape: Sample code for scraping list of publication for a group of authors and saving in a csv file
https://github.com/gerryreilly/google_scholar_scrape

## SakuraX99/Crawler-For-Google-Scholar
https://github.com/SakuraX99/Crawler-For-Google-Scholar

## WebGuruBoy/Python-google-scholar-scraping
https://github.com/WebGuruBoy/Python-google-scholar-scraping

## amychan331/google-scholar-scraper: Scraps scientific article data from Google Scholar and either create or update a node.
https://github.com/amychan331/google-scholar-scraper

## zhivou/google-scholar-helper: A simple gem to show information for 1 user from google scholar page
https://github.com/zhivou/google-scholar-helper

## liusida/Google-Scholar-Trends: Plot the trends of terminologies in Google Scholar over 20 years
https://github.com/liusida/Google-Scholar-Trends

## erkamozturk/Python-Google-Scholar: Publications of faculty members at a university are usually published on university web pages. As an example, publications of SEHIR’s CS faculty members are published at their profile pages (e.g., see Ahmet Bulut’s publications at http://cs.sehir.edu.tr/en/profile/6/Ahmet-Bulut/). However, searching for particular publications is not usually possible on such web pages. In this project, you are going to develop a publication search engine (similar to Google Scholar) that will allow searching for publications of CS faculty members with some optional filters. Details regarding the requirements are as follows:
https://github.com/erkamozturk/Python-Google-Scholar

## mibot/Google-Scholar-Crawler
https://github.com/mibot/Google-Scholar-Crawler

## chponte/google-scholar-search-engine
https://github.com/chponte/google-scholar-search-engine

## msstoci/google-scholar-scraper
https://github.com/msstoci/google-scholar-scraper

## DaiJunyan/RutgersGoogleScholar
https://github.com/DaiJunyan/RutgersGoogleScholar

## JackXuRepo/Google-Scholar-Data-Scrapper: Developed a program, which reads and analyzes the contents of the Google Scholar Page of an author
https://github.com/JackXuRepo/Google-Scholar-Data-Scrapper

## JohnZhang-source/download_google_scholar_alert: download papers in google scholar alert
https://github.com/JohnZhang-source/download_google_scholar_alert

## qiisziilbash/Google-Scholar-Crawler: This is a bot that gets the name of article and returns information about that article in google scholar
https://github.com/qiisziilbash/Google-Scholar-Crawler

## yokotatsuya/ExtractGoogleScholarCitations: The matlab code to extract the list of publications of a personal author from Google Scholar Citations from only url (user id). Text Analytics toolbox is necessary.
https://github.com/yokotatsuya/ExtractGoogleScholarCitations

## paulazoo/google-scholar-h-index: Some authors don't have a Google Scholar stats page. This calculates an author's h index based on citation and publication data from Google Scholar.
https://github.com/paulazoo/google-scholar-h-index

## BAEM1N/google-scholar-crawler: with HYU or without HYU
https://github.com/BAEM1N/google-scholar-crawler

## Ngogabill/About-Google-Scholar
https://github.com/Ngogabill/About-Google-Scholar

## lorenzibex/Scrape-Google-Scholar: In this short python script you will see, how to extract/scrape these two parameters in Python.
https://github.com/lorenzibex/Scrape-Google-Scholar

## VikramTiwari/google-scholarr-metadata-extraction
https://github.com/VikramTiwari/google-scholarr-metadata-extraction

## utkarshsingh341/web-scraping-google-scholar: Web Scraping and Data Aquisition from Google Scholar
https://github.com/utkarshsingh341/web-scraping-google-scholar

## pranjaljo/Google-Scholar-Network-Analysis
https://github.com/pranjaljo/Google-Scholar-Network-Analysis

## istex/istex-google-scholar: A module for generating ISTEX holding description for Google Scholar Library Links
https://github.com/istex/istex-google-scholar

## Coldflyer/Google-Scholar-Breadcrumbs-Builder
https://github.com/Coldflyer/Google-Scholar-Breadcrumbs-Builder

## wl8837/Google-Scholar-API
https://github.com/wl8837/Google-Scholar-API

## vignif/crawler-google-scholar: Download automatically statistics and pictures from google scholar's researchers.
https://github.com/vignif/crawler-google-scholar

## sutlxwhx/scholar-parser: This is a PHP example of how you can use Phantom.js to extract links from the first page of Google Scholar SERP in one page web application.
https://github.com/sutlxwhx/scholar-parser

## xzk-seu/SpiderForGoogleScholar: 鱼塘
https://github.com/xzk-seu/SpiderForGoogleScholar

## wl8837/Google-Scholar-API
https://github.com/wl8837/Google-Scholar-API

## vignif/crawler-google-scholar: Download automatically statistics and pictures from google scholar's researchers.
https://github.com/vignif/crawler-google-scholar

## Coldflyer/Google-Scholar-Breadcrumbs-Builder
https://github.com/Coldflyer/Google-Scholar-Breadcrumbs-Builder

## madhawadias/google_scholar_scrapper: A scrapping tool to scrape article attributes from scholar.google.com
https://github.com/madhawadias/google_scholar_scrapper

## LMBertholdo/google_scholar_crawler_coop
https://github.com/LMBertholdo/google_scholar_crawler_coop

## aless80/Google-Scholar-scraper: Download information from Google Scholar for a number of author names
https://github.com/aless80/Google-Scholar-scraper

## LucasVadilho/nodeGoogleScholar
https://github.com/LucasVadilho/nodeGoogleScholar

## jakeelkins/google-scholar-analysis: Some code I'm writing for analyzing research areas using google scholar, NLP, topic modeling, clustering, etc.
https://github.com/jakeelkins/google-scholar-analysis

## sfhall/Google-Scholar-ID-Grabber: Python script that takes in an excel list and gets the Google Scholar ID for each name
https://github.com/sfhall/Google-Scholar-ID-Grabber

## profmike/Google-Scholar-Stats: Parses author citation and h-index statistics from Google Scholar
https://github.com/profmike/Google-Scholar-Stats

## mogekag/sci-stat: Automated google scholar statistics crawler based on Scholar.py
https://github.com/mogekag/sci-stat

## hrlblab/ISN_KI_PubMed_GoogleScholar
https://github.com/hrlblab/ISN_KI_PubMed_GoogleScholar

## kylermurphy/scholar_plot: Simple Plot for google scholar
https://github.com/kylermurphy/scholar_plot

Simple Plot for google scholar and scopus information

Requires [pybliometrics][1] for Scopus and [scholary][2] for Google Scholar.

pybliometrics was installed with pip, however it requires a license/network access and so publication numbers are hardcoded for now from [Scopus Author seach](https://www.scopus.com/freelookup/form/author.uri)

```
pip install pybliometrics
```

scholary was installed via GitHub

```
pip install -U git+https://github.com/OrganicIrradiation/scholarly.git
```

[1]:https://pybliometrics.readthedocs.io/en/stable/index.html
[2]:https://github.com/OrganicIrradiation/scholarly





## couetilc/selenium-web-scraping: scraping google scholar using selenium
https://github.com/couetilc/selenium-web-scraping

### Analysis Notes

Selenium driver, obviously...


## crmne/googlescholarscraper: A scraper for Google Scholar, written in Python
https://github.com/crmne/googlescholarscraper


GoogleScholarScraper is a [Scrapy][] project that implements a scraper for Google Scholar.

Features
--------

* Extracts Authors, Title, Year, Journal, and Url.
* Exports to CSV, JSON and BibTeX.
* Cookie and referer support for higher query volumes.
* Optimistically tries the next page in case of server errors.
* Supports the full Google Scholar query syntax for authors, title, exclusions, inclusions, etc. Check out those [search tips].


[Scrapy]: https://scrapy.org/
[search tips]: http://www.otago.ac.nz/library/pdf/Google_Scholar_Tips.pdf


### Analysis Notes

Uses Scrapy, which I have yet to look into.




## callumparker/Google-Scholar-PDF-Link-Scraper: Scrape Google Scholar for PDF links based on a keyword. Written in Python.
https://github.com/callumparker/Google-Scholar-PDF-Link-Scraper


Scrape multiple Google Scholar pages for PDF links based on a keyword(s).
- Automatically downloads PDF files to the directory of the script.
- Creates a text file with PDF links.

### Analysis Notes

Basic URL querying. Nothing fancy. Not even error handling.



## pradeepsen99/Google-Scholar-Web-App-DB: A web-app to visualize the different researchers on Google Scholar along with their relationships to other researchers.
https://github.com/pradeepsen99/Google-Scholar-Web-App-DB

### Analysis Notes

Doesn't seem to contain anything Google Scholar like.

Is a React website basic site AFAICT.


## ardirsaputra/Data-Mining-For-Google-Scholar: this repository used for educational purpose in the course data mining 2019 informatic engineer university of lampung
https://github.com/ardirsaputra/Data-Mining-For-Google-Scholar

### Analysis Notes

Looks like a website, **without any scraper**. There's a ZIP file in the repo that *maybe* contains GS data, but I see no way this code is scraping GS, at least not in this initial quick scan. Nothing obvious or prominent, like `scholar.py` or `scholarly`. It's PHP + JS code.



## janneliukkonen/google-scholar-results-counter-scraper: Small web scraper to quickly evaluate list of ML algorithms against whitepapers using Google Scholar.
https://github.com/janneliukkonen/google-scholar-results-counter-scraper

### Analysis Notes

basic scraper, nothing fancy. Uninteresting therefore.


## zhang-hz/zotero-autofetch: Zotero plugin for automatic download of PDFs from Scihub and Google Scholar
https://github.com/zhang-hz/zotero-autofetch

### Analysis Notes

Since this looks like a FireFox addon (.xul files...) we *might* want to look into it a little further at some point: same idea we had for the Chrome extension but now for firefox, maybe?



## Manikaran20/Better_metrics_for_google_Scholars: This work is for all the scholars who have a google scholar profile, associated with the idea of providing a better and fairer results to them.
https://github.com/Manikaran20/Better_metrics_for_google_Scholars

### Analysis Notes

Selenium driver...


## goose0058/scholarbot: A small script to pull down references from google scholar.
https://github.com/goose0058/scholarbot

## sarthak-patidar/scholar-crawler: A spider to crawl google scholar.
https://github.com/sarthak-patidar/scholar-crawler

### Analysis Notes

Hm. ~~Another Scrapy commercial Scraper API user!~~

#### *Update*

My bad! That's **not** the commercial Scraper API, but [Scrapy][] i.e. https://scrapy.org/

Haven't yet looked into that one to see what it does in terms of VPN hopping, UserAgent randomization, etc.



## theclementho/Scholar-Crawler: ECE496 Capstone - Crawler prototype for Google Scholar
https://github.com/theclementho/Scholar-Crawler

### Analysis Notes

Has its own copy of `scholarly` and runs on top of that one. To be inspected further at a later date.





## maze-runnar/my-scholarly: Use Scholarly to fetch google scholar data
https://github.com/maze-runnar/my-scholarly

Use Scholarly to fetch google scholar data  

###Project description

scholarly is a module that allows you to retrieve author and publication information from Google Scholar in a friendly, Pythonic way.

### Usage

Because scholarly does not use an official API, no key is required. 

### **Stand on the shoulders of giants**  

Google Scholar provides a simple way to broadly search for scholarly literature. From one place, you can search across many disciplines and sources: articles, theses, books, abstracts and court opinions, from academic publishers, professional societies, online repositories, universities and other web sites. Google Scholar helps you find relevant work across the world of scholarly research.

### Analysis Notes

Yada yada. See later if this adds any value on top of `scholarly`. It's the grabbing that's the hard part so I guess not, but I *could* be mistaken. (Yeah, I'm getting bitchy when the clock turns into the night and I've got a few more entries to go. Did that to myself, though, no-one else to blame...)



## akhilanto/GoogleScholar-WebScarping-Using-Free-VPN-in-Python: GoogleScholar Web Scraping using Free VPN In Python
https://github.com/akhilanto/GoogleScholar-WebScarping-Using-Free-VPN-in-Python

### GoogleScholar-WebScraping-Using-VPN

Since Google Scholar doesn’t provide any API  this Script can be used to Web scrap Google Scholar to get Citations and Authors for a Published paper. Here we are providing the papers from DBLP website. By using free VPN, this program overcomes the google captcha .The program works in the following way
1. Importing Packages 
2. Getting published papers from DBLP website for a provided link
3. Getting free VPN 
4. Google Scholar web search  for the given paper
5. Scraping the Google Scholar 
6. Converting the data into Data Frame and saving it as CSV output


### Analysis Notes

Now this sounds a lot like a basic `scholarly`. To be investigated further.



## jamespreed/scholar-crawler: Scrapes Google scholar to build a networks of co-authorship.
https://github.com/jamespreed/scholar-crawler

Because of captchas, this runs using selenium and Firefox, so you must have Firefox installed. This is currently designed for Windows, but the only Feel free to use the browser of your choice, you will need to roll your own session class.


## dgalaktionov/scholar.py: A parser for Google Scholar, written in Python
https://github.com/dgalaktionov/scholar.py

### scholar.py

**WARNING**: This repository is a derived work from two different forks, from @machaerus and @jessamynsmith on the original repository in https://github.com/ckreibich/scholar.py. This is the cleanest option I see for uploading the version I need, considering:
* The original project is obviously abandoned
* Forking only one of the authors would be inaccurate
* [Github can make you lose access on your forks](https://www.niels-ole.com/ownership/2018/03/16/github-forks.html).

`scholar.py` is a Python module that implements a querier and parser for Google Scholar's output. Its classes can be used independently, but it can also be invoked as a command-line tool.

The script used to live at http://icir.org/christian/scholar.html, and I've moved it here so I can more easily manage the various patches and suggestions I'm receiving for scholar.py. Thanks guys, for all your interest! If you'd like to get in touch, email me at christian@icir.org or ping me [on Twitter](http://twitter.com/ckreibich).

Cheers,<br>
Christian

### Features

* Extracts publication title, most relevant web link, PDF link, number of citations, number of online versions, link to Google Scholar's article cluster for the work, Google Scholar's cluster of all works referencing the publication, and excerpt of content.
* Extracts total number of hits as reported by Scholar (new in version 2.5)
* Supports the full range of advanced query options provided by Google Scholar, such as title-only search, publication date timeframes, and inclusion/exclusion of patents and citations.
* Supports article cluster IDs, i.e., information relating to the variants of an article already identified by Google Scholar
* Supports retrieval of citation details in standard external formats as provided by Google Scholar, including BibTeX and EndNote.
* Command-line tool prints entries in CSV format, simple plain text, or in the citation export format.
* Cookie support for higher query volume, including ability to persist cookies to disk across invocations.

### Note

I will always strive to add features that increase the power of this
API, but I will never add features that intentionally try to work
around the query limits imposed by Google Scholar. Please don't ask me
to add such features.


### Analysis Notes

Some folks can be too ethical. ;-) This is a `scholar.py` fork which isn't listed as one. Have a look at what he did when the time comes.




## ukalwa/scholarly: A Node.js module to fetch and parse academic articles from google scholar
https://github.com/ukalwa/scholarly

### Analysis Notes

TypeScript module. Certainly DOES NOT have the features of `scholarly` (the Python module) as this one uses straight URL querying via cheeerio.

From the README:

A Node.js module to fetch and parse academic articles from Google Scholar.

#### Acknowledgements

This project was inspired from other awesome projects ([scholar.py], [google-scholar], and [google-scholar-extended])

[scholar.py]: https://github.com/ckreibich/scholar.py
[google-scholar]: https://github.com/VT-CHCI/google-scholar
[google-scholar-extended]: https://github.com/martinchapman/google-scholar-extended



## Marcelobbr/web_crawler: Web crawler of Google Scholar profiles
https://github.com/Marcelobbr/web_crawler

For the web scraping to work with Google Chrome, you need to install chromedriver. 

### Analysis Notes

Ah well...



## lydianish/brag-gs: A Google Scholar API for BRAG
https://github.com/lydianish/brag-gs

### Analysis Notes

Uses `scholarly` to do the hard work...



## alexyashin/scholar-downloader: Chrome extension to download files from Google Scholar search result
https://github.com/alexyashin/scholar-downloader

### Analysis Notes

Chrome extension. Code is 6 years old now. Still useful for PDF fetching? I don't know. Not my first choice there.


## Rhaigtz/scholar-scraping: Creacion de funcion para scraping de google scholar.
https://github.com/Rhaigtz/scholar-scraping

### Analysis Notes

https://github.com/Rhaigtz/scholar-scraping/blob/master/src/utils/scholar.js :
Rate limit detection, request rate limiting in an attempt to ccircumvent Google Scholar locking up. JavaScript code, looks clean to me.




## zouzhenhong98/scholartobib: a toot to grab bib info from google scholar, and write it to a bib file
https://github.com/zouzhenhong98/scholartobib

### Analysis Notes

https://github.com/zouzhenhong98/scholartobib/blob/master/scholar.py : randomized GoogleID for the query, SOCKS5 proxy (tor!), CAPTCHA detection, ...

Comment from code: "Routes scholarly through a proxy (e.g. tor).        Requires pysocks.        Proxy must be running."

It's not `scholarly` but certainly using the same kind of mechanisms to thwart GS. 

Interesting!





## Rosyuku/gssearch: google-scholar検索効率化
https://github.com/Rosyuku/gssearch

Search efficiency improvement

### Analysis Notes

Looks a bit like an early(?) `scholarly`: UserAgent randomization is in there, but I don't see proxy/VPN hopping, but that be me, my deteriorating eyes and the late hour. (Yup, been updating this document in reverse order as a stupid keyboard miss had me jump to the end of the tabs earlier and I didn't want to screw up Chrome any more than I already had at that moment. Anyway, that's nothing to bother you with so why am I writing this line?! :deep-thought:)

Here's [the source code](https://github.com/Rosyuku/gssearch/blob/master/<google_scholar_search class="py"></google_scholar_search>)'s top comment:

> Created on Sun May 31 00:23:02 2020
>
> @author: Wakasugi Kazuyuki
>
> ### Works Cited
> - https://own-search-and-study.xyz/2019/06/09/python-scraping-icml2019-summary/
> - https://serpapi.com/google-scholar-api
> - https://qiita.com/kuto/items/9730037c282da45c1d2b
> - https://github.com/scholarly-python-package/scholarly
> 
> 



## Neo-101/etudier-improved: Extract a citation network from Google Scholar
https://github.com/Neo-101/etudier-improved


*étudier* is a small Python program that uses [Selenium] and [requests-html] to
drive a *non-headless* browser to collect a citation graph around a particular
[Google Scholar] citation or set of search results. The resulting network is
written out as a [Gephi] file and a [D3] visualization using [networkx].
Current D3 visualization is inspired by [eyaler]. *The D3 visualization could
use some work, so if you add style to it please submit a pull request.*

If you are wondering why it uses a non-headless browser it's because Google is
[quite protective] of this data and routinely will ask you to solve a captcha
(identifying street signs, cars, etc in photos).  *étudier* will allow you to
complete these tasks when they occur and then will continue on its way
collecting data.

### Install

You'll need to install [ChromeDriver] before doing anything else. If you use
Homebrew on OS X this is as easy as:

    brew install chromedriver

Then you'll want to install [Python 3] and:

    pip3 install etudier

### Run

To use it you first need to navigate to a page on Google Scholar that you are
interested in, for example here is the page of citations that reference Sherry
Ortner's [Theory in Anthropology since the Sixties]. Then you start *etudier* up
pointed at that page.

    % etudier 'https://scholar.google.com/scholar?start=0&hl=en&as_sdt=20000005&sciodt=0,21&cites=17950649785549691519&scipsc='

If you are interested in starting with keyword search results in Google Scholar
you can do that too. For example here is the url for searching for "cscw memory"
if I was interested in papers that talk about the CSCW conference and memory:

    % etudier 'https://scholar.google.com/scholar?hl=en&as_sdt=0%2C21&q=cscw+memory&btnG='

Note: it's important to quote the URL so that the shell doesn't interpret the
ampersands as an attempt to background the process.

### --pages

By default *étudier* will collect the 10 citations on that page and then look at
the top 10 citatations that reference each one. So you will end up with no more
than 100 citations being collected (10 on each page * 10 citations).

If you would like to get more than one page of results use the `--pages`. For
example this would result in no more than 400 (20 * 20) results being collected:

    % etudier --pages 2 'https://scholar.google.com/scholar?start=0&hl=en&as_sdt=20000005&sciodt=0,21&cites=17950649785549691519&scipsc=' 




## bsodhi/books_scraper: Books scraper for Google Scholar and Goodreads
https://github.com/bsodhi/books_scraper

### Prerequisites

1. Install Python for your operating. You can [download Python 3.8.2 from here](https://www.python.org/downloads/release/python-382/).
2. This program makes use of [Selenium WebDriver](https://www.selenium.dev/documentation/en/webdriver/driver_requirements/)
for fetching GoodReads book shelf data. You should have a driver installed for your
browser. Currently supported browsers are: Chrome, Firefox, Edge and Safari. We have tested with Firefox and Safari (on macOSX 10.14.6).



## coryjcombs/Scholar-search: Multi-page, multi-term scraper for Google Scholar results
https://github.com/coryjcombs/Scholar-search

### Scholar-Search

A multi-page, multi-term scraper for Google Scholar results (version 1.0.6).

### Background

This scraper was developed for use in a systematic review of scholarship on electricity generation, co-authored by Sarah M. Jordaan, Cory J. Combs, and Edeltraud Günther. The collected data served as the basis for an article being prepared for submission in early 2020.

### Details

* Designed to scrape all results of Google Scholar searches, up to Scholar's imposed maximum of 100 pages (1000 results) for each search.
* Developed using BeautifulSoup, Pandas, and the requests and time packages.

### Credits

* This code builds upon a single-page scraper for Google.com search results developed by Edmund Martin, whose original work is [available here](https://edmundmartin.com/scraping-google-with-python/). Many thanks and all due credit to Edmund Martin!
* Adaptation for Google Scholar, as well as iteration over pages, data extraction and manipulation, and export control were coded by Cory J. Combs.

### Scraper Components

1. A user agent, which provides identifying information to the server
2. A function to fetch results
3. A function to parse results
4. An function to execute fetching and parsing with error handlers
5. The main search script, which:
  * executes the search with the input parameters,
  * outputs the results in a pandas data frame,
  * extracts metadata elements not consistently identifiable through Google Scholar's html or xml alone,
  * cleans and formats the data, and
  * exports the fully formatted dataframe into Excel.

The results may be explored in the output Excel file or in Python using pandas. The final formatted pandas data frame is called "data_df_clean" by default.

### Ethical Scraping

Without appropriate constraints, web scraping can cause undue stress on a server. As such, special care was taken in implementing this scraper to ensure ethical use of server requests, first by scripting pauses between both page iterations and individual result collections, and second by separating execution of each search across the day, over multiple days, and avoiding peak usage times. When developing the script, a sample test was manually confirmed to yield a small number of results prior to execution to avoid unnecessary server burden.

For an interesting exploration of scraping Google Scholar results at a far larger scale, and through different means, see [Else 2018 in Nature](https://www.nature.com/articles/d41586-018-04190-5).


### Analysis Notes

Right. Right. Anyway, this one uses UserAgent spoofing and nothing else, just plain website requesting.

Now for the referenced articles:

#### https://edmundmartin.com/scraping-google-with-python/

That's basically `scholar.py` so no magic what-so-ever.

#### https://www.nature.com/articles/d41586-018-04190-5

Quoting a bit here (emphasis mine):

> ### How did you get around the fact that Google Scholar has no API?
>
> We spent three months scraping data from the website. I created a script to do so, but I had to be there to keep manually solving the CAPTCHAs that appeared regularly. 
> It was a boring summer! We used several computers to distribute the enquiries because Google Scholar asks you to solve a CAPTCHA if one computer is making too many requests. 
> **Sometimes the CAPTCHAs appear so frequently that it is not practical to get the data this way. We don’t think it is a reliable method of getting the data.**
> 
> ### How many CAPTCHAs did you solve over the course of the experiment?
> 
> I can’t tell you the exact number, but many hundreds!
> 
> ### How long would it have taken to extract the data if an API was available from Google Scholar?
> 
> One or two days.

And there you have it: that's basically what we're fighting: it's a tug of war with the Google engineers. Personally, I think `scholarly` is closest to a workable/usable scraper (there's the "Scraper API" mentioned elsewhere in this doc, which turns out to be a commercial SaaS version of that one AFAICT from their blurbs. Maybe with some added Mechanical Turk if you buy the more expensive licenses of theirs, but otherwise just `scholarly` and that's it. Note ny own findings / guestimates regarding VPN IP blocking: Google doesn't need to maintain a blacklist manually there: all they have to do is monitor the incoming IP addresses and do a DNS reverse lookup to see if an IP address that's frequenting their Scholar site belongs to a VPN company like NordVPN and then it's down to counting requests and 'downgrading' those IP numbers when you encounter them. At least that's what *I* would do if I were in their shoes: it's fast, doesn't need manual maintenance and all you have to do is be more strict enforcing your limits on these 'suspect' IP numbers. Since you have no published limits on Scholar anyway, you can get away with throttling the suspect ones and if someone complains the ball is in their court to proove that they are **not** using a VPN or proxy for their Scholar visits. Which leaves the rather more fluid(?) `tor` network... Which' exit nodes can be detected too, I suppose. Hm. I wonder what FSF's panopticon would say about the fingerprint of such a proxied visit: is it recognizable from a regular Chrome browser visit? I suppose so, as I seem to recall some older NSA/FBI work re 'cracking' darknet origins by monitoring tor exit nodes... Though I must say I don't recall how that one was done back in the day -- it's been a few years.)






## edwardmfho/ScholarScrape: Google Scholar strangely does not has its own API. I use a rather stupid way to scrape the title and authors' name from the scholar search query. Will expand the functionality of this repo in the future.
https://github.com/edwardmfho/ScholarScrape

### Analysis Notes

And another Selenium driver to kick off Chrome for browsing Google Scholar...


## aakashchandhoke/SpiderUnleashed: SpiderUnleashed is a webcrawler that retrieves the non-pdf results from educational websites because pdfs are easily available on Google Scholar. The application uses the inbuilt library of HTTrack that crawls the webpages of the provided topic to the application.
https://github.com/aakashchandhoke/SpiderUnleashed

### Analysis Notes

C# code. No magicks re Google Scholar quirks: https://github.com/aakashchandhoke/SpiderUnleashed/blob/master/LinksCrawler.cs

The mention of HTTrack in that description has me wondering, but only for a bit.



## erenkarabacak/scholar_app: Simple Google Scholar Web Application (Created by using scholarly and Flask)
https://github.com/erenkarabacak/scholar_app

## glamrock/hypnoscholar: A twitterbot, whose primary function is to post Google Scholar links.
https://github.com/glamrock/hypnoscholar

### Analysis Notes

Just a bit of *fun stuff*. Ruby code.



## maurice-schleussinger/SLR-Tools: Python scripts to perform a systematic literature review for Google Scholar and others
https://github.com/maurice-schleussinger/SLR-Tools

### Analysis Notes

Uses rate limiting in an attempt to prevent robot detection to trigger, but no other magicks are performed it seems: https://github.com/maurice-schleussinger/SLR-Tools/blob/master/crawl_googlescholar.py

Here's the latest relevant commit message on that subject, 9 months ago: https://github.com/maurice-schleussinger/SLR-Tools/commit/23cad9975dd0f7547cba393c85709ff48f5cd84b




## Ze1598/Scrape-Academic-Social-Networks: Scrape information from Google Scholar, ResearchGate and Academia.edu with Python and Selenium
https://github.com/Ze1598/Scrape-Academic-Social-Networks

For a college project, me and a classmate had to find out how many documents the authors of each school from Instituto Politécnico do Porto (IPP) had published, along with some other metrics, in academic social networks, specifically Google Scholar, ResearchGate and Academia.edu. Unfortunately, there's still no API support for none of them, and so I had to scrape this information with Python and Selenium.

These scripts are by no means API for the platforms, since they were tailored to our need, but I think these scripts offer a solid base for someone looking to start a project like that.

Due note that, because these scripts (Selenium) rely on a Google Chrome driver, you need to specify the path where it's located. For my case, executing the driver once and having it in the same folder as the scripts was enough to run the script successfully afterwards.

The code will probably be a bit messy as I was more worried about getting the results than making the code readable and/or maintainable in the long run, but I feel it's still clear enough as I wrote docstrings for every function and wrote comments for everything.

External sources:

* Selenium: https://www.seleniumhq.org/

* Google Chrome's driver download: https://chromedriver.storage.googleapis.com/index.html?path=73.0.3683.68/

* Google Scholar: https://scholar.google.com/

* ResearchGate: https://www.researchgate.net/

* Academia.edu: https://www.academia.edu/






## murfel/pauper-scholar: 🎓A Chrome extension that allows to differentiate between free-access and paid-access articles in search results on Google Scholar.
https://github.com/murfel/pauper-scholar

### Pauper Scholar — Get it in [Chrome Web Store]

Pauper Scholar is a Chrome extension for the Google Scholar website which adds an ability to differentiate between free-access and paid-access articles, in particular, it can hide all paid-access articles in search results.

An article is considered to be a free-access one if there's a link to a paper next to it.

### Analysis Notes

**Question**: what's keeping me from having that part of the Google Sniffer done as a Chrome extension instead? We can communicate across applications using websockets & localhost, after all, so we could take the idea of the next repo (ag-gipp/recvis-tiny-scholar-api) along with this one and mix that into a Chrome extension which talks to Qiqqa...



## ag-gipp/recvis-tiny-scholar-api: This is for creating single purposed unofficial temporary Google Scholar API that will serve RecVis project.
https://github.com/ag-gipp/recvis-tiny-scholar-api

This project is meant to be used as very small unofficial Google Scholar API for fetching bibliographic data based on input academic paper title. This API painfully slows down requests down to one request per 2 minute because Google Scholar is aggressively blocking the fetching process otherwise. Tiny Scholar API, whenever successfully fetching process happens, caches the request and doesn't count it towards API fetching limit of one document per 2 minutes.

### Analysis Notes

JavaScript code, which uses puppeteer, a headless browser. No special Google Scholar quirks circumvention sauce, apart from that slow rate of querying: one result per 2 minutes.




## aokabi/scholar-chrome-extension: Google Scholarの検索結果をリスト出力するChrome拡張
https://github.com/aokabi/scholar-chrome-extension

### Analysis Notes

Alas, the tough bit is done by the browser. Nothing useful in this code. https://github.com/aokabi/scholar-chrome-extension/blob/master/src/content_scripts.js


## driss14/googlescholarselenium: Simple script to download google scholar search to csv file using selenium web driver and beautiful soup
https://github.com/driss14/googlescholarselenium

Simple supervised script to parse google scholar search to csv file using selenium web driver and beautiful soup.


## adbrucker/scholar-kpi: A tool for analysing publication related key performance indicates (KPIs) based on the information available at the Google Scholar page of an author.
https://github.com/adbrucker/scholar-kpi

### Analysis Notes

[Scrapes Google Scholar using F#](https://github.com/adbrucker/scholar-kpi/blob/master/src/LogicalHacking.ScholarKpi/Scraper/GoogleScholar.fs) which is cool and everything, but there's no error checking for the usual Google Scholar quirks AFAICT.



## bcmechen/find-pmid: Chrome extension that finds and displays PMID on Google Scholar
https://github.com/bcmechen/find-pmid

Find PMID is a Google Chrome extension that uses Entrez Programming Utilities (E-utilities) to find PMID (unique identifier used in PubMed), displays PMID, and provides one click access to the article page in PubMed for Google Scholar's results.

### Analysis Notes

Accessed the PubMed website: https://pubmed.ncbi.nlm.nih.gov/




## lachrist/oghma: Oghma (responsibly) scrapes citation graphs from google-scholar
https://github.com/lachrist/oghma

Oghma is a command-line tool for scraping a citation graph from [google-scholar](https://scholar.google.com/) starting from some seminal works.
[google-scholar](https://scholar.google.com/) will prompt a captcha from time to time even though Oghma uses Selenium and only launches requests every 5 to 10 seconds.
In which case a notification will pop-up; once you resolved the capcha, you can press `<enter>` in the terminal to resume the scraping.
Using a Firefox profile can help to remain undercover; being logged in a [gmail](https://mail.google.com) account in this profile is even better.



## jessebrennan/citation_scraper: Pulls and formats citations for authors from Google Scholar
https://github.com/jessebrennan/citation_scraper


### Intro

This is software used to scrape Google Scholar for citations by a
particular author. It makes use of ckreibich's [scholar.py][1], with
a couple of [modifications][2].

### Features

#### Caching

Google blocking the program mid-run used to be a show stopper. All
of the citations already scraped would be lost and the program would
crash. Until... **CACHING!**

Every time all of the citations for a particular author are scraped
they are added to a cache file called `.pickle_cache.dat` which is 
created in the directory where the program is run. If the program
crashes due to a KeyboardInterrupt (^C) or from a 503 from Google's
servers, the progress so far is saved to this file so that on the next
run the scraping can resume from where it left off.

#### Refined Search

Sometimes you want to limit your search only to authors that are part
of a particular institute or university. By using the `--words` option
one can specify that so that it's reflected in the results. For example
`--words "UC Santa Cruz Genomics Institute"` will give only results
from authors within that institute.

#### Waiting

the `--wait` option can be used to wait for a specified number of
seconds between each query with the hopes that this won't upset Google.
The effectiveness of this solution has not been verified.

### Trouble shooting

Probably the only problem you will encounter is getting blocked by
Google Scholar's API. There is a workaround!

You need:

1. Mozilla Firefox

2. A Firefox extension that allows you to export cookies in the
   Netscape cookie file format such as [Cookie Exporter][4].

Then:

3. Navigate to one of the URLs that failed when requested (using
   Firefox)

4. Fill out the captcha

5. Export the cookies from the page (as `cookies.txt`)

6. Save the file and run again but specify the `-c` option. For example
   ```bash
   $ python3 citation_scraper zeppelin.txt output.txt -c cookies.txt
   ```


[1]: https://github.com/ckreibich/scholar.py
[2]: https://github.com/ckreibich/scholar.py/pull/96
[3]: https://virtualenv.pypa.io/en/stable/
[4]: https://addons.mozilla.org/en-US/firefox/addon/cookie-exporter/






## jjwallman/gscholartex: Parse saved Google Scholar webpage to extract citation data
https://github.com/jjwallman/gscholartex

The Google Scholar API is a little difficult and blocks repeated requests. I have not been able to find a way to load a profile with a specific number of citations showing, hence the manual steps.



## ShuDiamonds/twitterbot_googlescholar: the app aim is to send the infomation which is the result of thesis title analysis from google scholar
https://github.com/ShuDiamonds/twitterbot_googlescholar

### Analysis Notes

Looks rather unfinished or uses external services? https://github.com/ShuDiamonds/twitterbot_googlescholar/blob/master/posttweet.py



## lalit3370/scrapy-googlescholar: Scraping google scholar for user page and citations page using scrapy and creating an API with scrapyrt
https://github.com/lalit3370/scrapy-googlescholar

### Analysis Notes

Huh? README says:

> 13. Create a Scraper API account if you don't want to get banned from google
> 14. Copy your API key and paste it in topl_project/topl_project/spiders/1.py lines 22 and 52


Now I wonder what that Scraper API is... A-ha! 

https://www.scraperapi.com/

Proxy API for Web Scraping

Scraper API handles proxies, browsers, and CAPTCHAs, so you can get the HTML from any web page with a simple API call!

https://www.scraperapi.com/pricing

One of the most frustrating parts of automated web scraping is constantly dealing with IP blocks and CAPTCHAs. Scraper API rotates IP addresses with each request, from a pool of millions of proxies across over a dozen ISPs, and automatically retries failed requests, so you will never be blocked. Scraper API also handles CAPTCHAs for you, so you can concentrate on turning websites into actionable data.

---

So that looks like a commercial scholarly.py SaaS. :thinking:

Meanwhile, there's a API key apparently in an older commit, as there's this commit: https://github.com/lalit3370/scrapy-googlescholar/commit/0b332967a35e9132073ee0f7fb18dcd57947f2c9
which impacts https://github.com/lalit3370/scrapy-googlescholar/blob/master/topl_project/topl_project/spiders/1.py



## aptaheri/covid_response: Python scripts to extract text from Google Scholar docs
https://github.com/aptaheri/covid_response

## philnova/citationscraper: Short script to pull well-formatted citations from Google scholar
https://github.com/philnova/citationscraper

Uses selenium to scrape formatted citations from Google Scholar.

We need to control a real browser instance, rather than just making HTTP requests,
because the actual citation is hidden behind a modal window. To get it, we need
to interact with JavaScript on the Scholar page, so we need a zombie browser.

N.B. Google Scholar won't be happy about being scraped. Repeated use of this script
may lead to you being temporarily locked out.



## brinsonaml/PaperCrawler: Use Google scholar to search for papers and crawl content
https://github.com/brinsonaml/PaperCrawler

### Analysis Notes

Carries a copy (edited?) of scholar.py

Has code (`crawler.py`) which scrapes publications.
Journals include:

* IEEE proceedings
* Elsevier: Composite science and technology
* ACS 





## machetazo/sme: script de python que analiza resultados de google scholar
https://github.com/machetazo/sme

### Analysis Notes

scraper uses `scholarly`: https://github.com/machetazo/sme/blob/master/sme/scraper.py#L3

Translation: "we use these headers so that google believes we are firefox" at https://github.com/machetazo/sme/blob/master/sme/scraper.py#L20
so I guess they're not doing UserAgent randomization here?










# Microsoft Academic Search

## MicrosoftDocs/microsoft-academic-services
https://github.com/MicrosoftDocs/microsoft-academic-services

## Azure-Samples/academic-knowledge-analytics-visualization: Various examples to perform big data analytics over Microsoft Academic Graph and visualize the results.
https://github.com/Azure-Samples/academic-knowledge-analytics-visualization

## milindhg/Microsoft-Academic-Graph
https://github.com/milindhg/Microsoft-Academic-Graph

## Azure-Samples/microsoft-academic-graph-pyspark-samples: Sample PySpark code for interacting with the Microsoft Academic Graph
https://github.com/Azure-Samples/microsoft-academic-graph-pyspark-samples

## microsoft/mag-covid19-research-examples: Examples or utilizing Microsoft Academic for conducting covid-19 research
https://github.com/microsoft/mag-covid19-research-examples

### Official Microsoft Sample

<!-- 
Guidelines on README format: https://review.docs.microsoft.com/help/onboard/admin/samples/concepts/readme-template?branch=master

Guidance on onboarding samples to docs.microsoft.com/samples: https://review.docs.microsoft.com/help/onboard/admin/samples/process/onboarding?branch=master

Taxonomies for products and languages: https://review.docs.microsoft.com/new-hope/information-architecture/metadata/taxonomies?branch=master
-->

The code samples provided here provide WHO / PubMed ID -> MAG ID mapping data as well 
as code examples showing how to perform COVID-19 related analysis against the 
[MAG](https://www.microsoft.com/en-us/research/project/microsoft-academic-graph/) Dataset 
and [Project Academic Knowledge API](https://www.microsoft.com/en-us/research/project/academic-knowledge/) 
or [MAKES API](https://docs.microsoft.com/en-us/academic-services/knowledge-exploration-service/).



## ropensci/microdemic: microsoft academic client
https://github.com/ropensci/microdemic

## microsoft/academic-knowledge-exploration-services-utilities: Utility tools and scripts for interacting with Microsoft Academic Knowledge Exploration Service (MAKES)
https://github.com/microsoft/academic-knowledge-exploration-services-utilities

## jimbobbennett/MicrosoftAcademicContent: This repository lists content suitable for students/faculty to learn Azure and other Microsoft technologies
https://github.com/jimbobbennett/MicrosoftAcademicContent

## subhash-pujari/MicrosoftAcademicSearchCrawler: This is the crawler for querying the microsoft academic search APIs in BFS(breadth first search way starting from the root node). We get a JSON response which is parsed and saved to the database.
https://github.com/subhash-pujari/MicrosoftAcademicSearchCrawler

## andreas-wilm/awesome-academic-graph: Awesome list for Microsoft Academic Graph
https://github.com/andreas-wilm/awesome-academic-graph

Awesome list for Microsoft Academic Graph (MAG)

### About MAG: comparisons, coverage etc.

- [Two new kids on the block: How do Crossref and Dimensions compare with Google Scholar, Microsoft
  Academic, Scopus and the Web of Science? (May, 2019)](https://link.springer.com/article/10.1007%2Fs11192-019-03114-y)
- [Cost of tracking research trends and impacts with Microsoft Academic Graph (Feb 2019)](https://www.microsoft.com/en-us/research/project/academic/articles/cost-of-tracking-research-trends-and-impacts-with-microsoft-academic-graph/)
- [Publish or Perish version 6 (Nov 2017)](https://harzing.com/blog/2017/11/publish-or-perish-version-6)
- [Microsoft Academic: A multidisciplinary comparison of citation counts with Scopus and Mendeley for 29 journals (Nov, 2017)](https://arxiv.org/ftp/arxiv/papers/1711/1711.08767.pdf)
- [The coverage of Microsoft Academic: Analyzing the publication output of auniversity (Sep 2017)](https://arxiv.org/ftp/arxiv/papers/1703/1703.05539.pdf)
- [An Analysis of the Microsoft Academic Graph (Sept 2016)](http://www.dlib.org/dlib/september16/herrmannova/09herrmannova.html)
- [Eight Years of WSDM: Increasing Influence and Diversifying Heritage (Feb 2016)](http://cm.cecs.anu.edu.au/post/citation_analysis/)
<!-- - [Comparison of Microsoft Academic (Graph) with Web of Science, Scopus and Google Scholar](https://eprints.soton.ac.uk/408647/1/microsoft_academic_msc.pdf) -->
- [A Review of Microsoft Academic Services for Science of Science Studies (Dec 2019)](https://www.frontiersin.org/articles/10.3389/fdata.2019.00045/full)
- [CWTS: Mapping science using Microsoft Academic data](https://www.cwts.nl/blog?article=n-r2x284)


### Analyses using MAG

- [Analytics & Visualization Samples for Academic Graph](https://github.com/Azure-Samples/academic-knowledge-analytics-visualization)
- [China to Overtake US in AI Research (March, 2019)](https://medium.com/ai2-blog/china-to-overtake-us-in-ai-research-8b6b1fe30595)
- [Microsoft Academic: Is the Phoenix getting wings? (Nov 2017)](https://mdxminds.com/2016/11/17/microsoft-academic-is-the-phoenix-getting-wings/)
- [A Century of Science: Globalization of Scientific Collaborations,Citations, and Innovationsi (Aug, 2017)](https://arxiv.org/pdf/1704.05150.pdf)
- [PR-Index: Using the h-Index and PageRank for Determining True Impact (Sept, 2016)](https://www.ncbi.nlm.nih.gov/pmc/articles/PMC5023123/)
- [Visualizing Citation Patterns of Computer Science Conferences (Aug, 2016)](http://cm.cecs.anu.edu.au/post/citation_vis/)
- [Investigations on Rating Computer Sciences Conferences: An Experiment with the Microsoft Academic Graph Dataset (Apr, 2016)](https://dl.acm.org/citation.cfm?doid=2872518.2890525)
- [WSDM Cup 2016: Entity Ranking Challengei (Feb, 2016)](https://dl.acm.org/citation.cfm?doid=2835776.2855119)
- [An Overview of Microsoft Academic Service (MAS) and Applications (May, 2015)](http://www.www2015.it/documents/proceedings/companion/p243.pdf)
- [Citation recommendation of 80 Million papers using Graph DB(Neo-4J)](http://abhie19.github.io/MS_Academic_Graph/)








## lquan/MicrosoftAcademicSearch: review of Microsoft Academic Search
https://github.com/lquan/MicrosoftAcademicSearch

## DanielDugan/MicrosoftAcademicPython
https://github.com/DanielDugan/MicrosoftAcademicPython

## mcialini/MicrosoftAcademicSearch: Determine duplicate authors from Microsoft's massive research database
https://github.com/mcialini/MicrosoftAcademicSearch

The Microsoft Academic Search is a research database which covers more than 50 million publications and over 19 million authors across a variety of domains. One of the main challenges with providing this service is caused by author-name ambiguity. There are many authors in the database which have unique IDs, but are the same author in reality. Given several csv files (most importantly Author.csv and PaperAuthor.csv), the task of this project is to determine which authors are duplicates.

The data mining algorithm I used in this project was extensive, and involved searching for a series of name variations of each author to see if perhaps they were listed under a nickname, or their name was misspelled. For example, one of the heuristics was to check all possible abbreviations of a person's name and see if that was listed under a different ID. So for John Doe Smith, the code would search for John D Smith, J Doe Smith, J D Smith, and J Smith.

After applying several of these heuristics, I received a 97.816% accuracy rating.

### Analysis Notes

Code is 7 years old. Paper (PDF) is included with the Python code.




## coco11563/MicrosoftAcademicGraphDataMerge
https://github.com/coco11563/MicrosoftAcademicGraphDataMerge

## RapidSoftwareSolutions/Marketplace-MicrosoftAcademicSearch-Package: Discover more of what you need more quickly. Semantic search provides you with highly relevant search results from continually refreshed and extensive academic content from over 120 million publications.
https://github.com/RapidSoftwareSolutions/Marketplace-MicrosoftAcademicSearch-Package


### MicrosoftAcademicSearch Package

Discover more of what you need more quickly. Semantic search provides you with   highly relevant search results from continually refreshed and extensive academic content from over 120 million publications.

* Domain: [academic.research](http://academic.research.microsoft.com/)
* Credentials: key

### How to get credentials: 

1. Subscribe to the Microsoft Text Analytics API on the [Microsoft Azure portal](https://azure.microsoft.com/en-us/services/cognitive-services/).
2. Click create button.
3. In settings->credential section you will see apiKey (Ocp-Apim-Subscription-Key)

### Analysis Notes

Haven't scanned the source code (PHP), but might be useful.




## DarrinEide/microsoft-academic: Documentation for all Microsoft Academic projects
https://github.com/DarrinEide/microsoft-academic

## arnabsinha83/AcademicBot: Playing with Microsoft Academic API
https://github.com/arnabsinha83/AcademicBot

## vwoloszyn/mag2elasticsearch: Migrating more than 160GiB of research data from Microsoft Academic Graph into an Analytics engine - Elasticsearch!
https://github.com/vwoloszyn/mag2elasticsearch


### Using Elasticsearch on Microsoft Academic Graph MAG

Exploring more than 160 GiB of publications from Microsoft Academic Graph (MAG) using Elasticsearch!

### Download Microsoft Academic Graph

https://docs.microsoft.com/en-us/academic-services/graph/reference-data-schema

https://zenodo.org/record/2628216

```
     4564007 Affiliations.txt
 16528778635 Authors.txt
     2224843 ConferenceInstances.txt
      428103 ConferenceSeries.txt
    55188690 FieldsOfStudy.txt
     5689662 Journals.txt
 40976541540 PaperAuthorAffiliations.txt
 32446006785 PaperReferences.txt
     7763592 PaperResources.txt
 60213784152 Papers.txt
 23096534376 PaperUrls.txt
 ----------------------------------------
173337504385 (~161.4) GiB
```




## hugoTO/Microsoft-Academic-Graph: Microsoft Academic Graph Guide
https://github.com/hugoTO/Microsoft-Academic-Graph

The Microsoft Academic Graph (MAG) is a heterogeneous graph containing scientific publication records, citation relationships between those publications, as well as authors, institutions, journals, conferences, and fields of study. This graph is used to power experiences in Bing, Cortana, Word, and in Microsoft Academic. The graph is currently being updated on a weekly basis.

In this tutorial, you are able to create a organization insights with MAG and PowerBI like this.


## ankeshanand/Microsoft-academic-crawler: Scripts to crawl the Microsoft academic site to create a database of research papers for building a citation network, written primarily in PHP.
https://github.com/ankeshanand/Microsoft-academic-crawler

### Analysis Notes

7 years old, hence very probably obsolete.


## mattmarx/reliance_on_science: linkages from non-patent literature (NPL) references from USPTO patents since 1947 to academic papers since 1800 using Microsoft Academic Graph
https://github.com/mattmarx/reliance_on_science

The codes necessary to replicate Marx/Fuegi 2019 are contained in this directory. This code operates on, and assumes the presence of, a set of files from the Microsoft Academic Graph (MAG) and USPTO non-patent literature (NPL) references, described below.

### DISCLAIMERS

The code is unsupported and is largely undocumented. It is provided primarily for those interested in understanding how the NPL linkages to MAG were accomplished. Moreover, it is executable only in a Sun Grid Engine (or similar) Unix environment with STATA installed as well as several packages including ftools and gtools and the Perl module Text::LevenshteinXS. It assumes the directory structure described below and contains hardcoded, fully-qualified pathnames. Moreover, you will need at least 5 terabytes of disk space, perhaps as much as 10.

There are four general steps in executing the matches: First, preparing the MAG data. Second, preparing the NPL data. Third, generating a first-pass set of "loose" matches. Fourth, scoring those "loose" matches and picking the best match for each NPL. Each of these major steps includes a number of sub-steps; there is no "master" script to run the process from beginning to end.


## lhviet/microsoft-academic-crawler: Crawling Title and Abstract information of papers from Microsoft Academic
https://github.com/lhviet/microsoft-academic-crawler

## tczpl/BOP2016-Microsoft-Academic-Knowledge-API: Nodejs/ Microsoft Academic Knowledge API
https://github.com/tczpl/BOP2016-Microsoft-Academic-Knowledge-API

Microsoft Beauty of Programming 2016 semi-final project 

Use the given Microsoft Academic Search API to write a Restful API that returns all paths where the distance between two points (authors or papers) is <4. 

Because the ranking is based on response time, NodeJS asynchronous single thread is used. 
After discussing ideas with teammates, I wrote the code. At the top, I was ranked in the 30s. However, I didn't make the promotion.



## bethgelab/magapi-wrapper: Wrapper around Microsoft Academic Knowledge API to retrieve MAG data
https://github.com/bethgelab/magapi-wrapper

Microsoft Academic knowledge provides rich API's to retrieve information from Microsoft Academic Graph. MAG knowledge base is web-based heterogeneous entity graph which consists of entities such as Papers, Field of study, Authors, Affiliations, Citation Contexts, References etc.

This tool provides a wrapper around the Knowledge API to retrieve Authors, Field of Study and Papers data.



## Azure-Samples/microsoft-academic-knowledge-exploration-service-javascript-samples: Sample Javascript code for interacting with Microsoft Academic Knowledge Exploration Service (MAKES)
https://github.com/Azure-Samples/microsoft-academic-knowledge-exploration-service-javascript-samples

This sample creates a very simple webpage that leverages MAKES entity and semantic interpretation engines via javascript for searching academic papers.

### Microsoft Academic Knowledge Exploration Service (MAKES) Samples

#### [Academic semantic search website](samples/academic-semantic-search-website/get-started.md)

This sample creates a very simple webpage that leverages MAKES entity and semantic interpretation engines via javascript for searching academic papers.




## maysam/atn1
https://github.com/maysam/atn1

### Analysis Notes

C# code

Has Microsoft Academic Search support, but I don't see Google Scholar in there. (Done a very cursory source scan though.)



## supersambo/Author-Search-Msft-Academic-Search: A simple script to get publications of given authors from Microsoft Academic Search
https://github.com/supersambo/Author-Search-Msft-Academic-Search


### Publication search

This is a very simple single purpose script to query the Microsoft Academic Search Api for publications based on a list of given author names.

In order to use this:

* Download this repository
* Get an Api key from [here](https://www.microsoft.com/cognitive-services/en-US/sign-up?ReturnUrl=/cognitive-services/en-us/subscriptions)
* Edit access_key.edit by filling in your own key and change the filename to access_key
* Create a text file containing a list of authors (one per line) you want to query. Take `test.txt` as a reference if needed.
* Run the script with the path to your input file as a command line argument e.g. like this 
`python query_authors.py test.txt`
* The script will save the raw results as a single json file for each author to the `output/` directory

----

Note that this is everything but sophisticated and does not handle api errors.

If you run into API limits just stop the script and/or implement your own error handling. At least the console will tell you that there is something going wrong.






## wallyliu/PACInterview: Crawler of Microsoft Academic and Google Scholar implemented with python
https://github.com/wallyliu/PACInterview

### Analysis Notes

From the README: 

3. Google Scholar Crawler

Use python to build a crawler for the following website

** (NOTE): This function cannot crawl target page because it will be detected as ROBOT. It only implements a parser part. **

---

On the other hand, the code seems to include a recent Microsoft Academic Crawler but this code uses a Selenium driver to open a browser to access the site: https://github.com/wallyliu/PACInterview/blob/master/MSSpider.py#L2




## cfranck9/extract_article_abstract: Extract abstract of a journal article using Microsoft Academic API
https://github.com/cfranck9/extract_article_abstract

A simplistic Python code to extract abstract of a single journal article using Microsoft Academic API. User can enter either title or DOI of a paper for MS database query (provide at least one of the two). DOI is converted to title through Crossref API.

### Analysis Notes

Uses Chrome Driver to open up a browser.


## tranhungnghiep/CitationAnalysis: Citation Analysis on the Microsoft Academic Graph Dataset
https://github.com/tranhungnghiep/CitationAnalysis

Citation Analysis on the Microsoft Academic Graph Dataset

This contains some old code fragments for the citation analysis experiments in the past. The results were not reported formally but these codes are open sourced in case someone may find them useful. Note that the codes are old and some practices may have become outdated.

The codes concern the analysis and prediction of citation count of papers in a scholarly dataset, and may demonstrate two techniques:

- Using pandas for feature engineering on graph data, demonstrating basic pandas' operations as well as some tricky computations on graph data.

- Using sklearn for simple machine learning pipeline setup, with feature transforming, some simple modeling, model selection by grid search CV.


## deepakmunjal15/Research-Paper-Extractor-Tool: Created a Research Paper Data Extractor Tool using Microsoft Academic API. Skills Used: Python
https://github.com/deepakmunjal15/Research-Paper-Extractor-Tool

## michaelfaerber/makg-linking: Creating owl:sameAs links beteen the Microsoft Academic Knowledge Graph (MAKG) and other Linked Open Data sources (OpenCitations, Wikidata, ...)
https://github.com/michaelfaerber/makg-linking






## ben-var/VISR: Visualizing Institutional Scholar Relationships using D3.js, JavaScript, HTML, and CSS after processing 330GB of data from Microsoft Academic Graph using Hadoop and Python
https://github.com/ben-var/VISR

## karankishinani/VISR: Visualizing Institutional Scholar Relationships using d3.js, JavaScript, HTML, and CSS after processing 330GB of data from Microsoft Academic Graph by Aminer
https://github.com/karankishinani/VISR

## whoyoung388/visualization-of-schools: Visualizing Institutional Scholar Relationships using D3.js, JavaScript, HTML, and CSS after processing 330GB of data from Microsoft Academic Graph using Hadoop and Python.
https://github.com/whoyoung388/visualization-of-schools

View the project live at whoyoung388.github.io/viz-of-schools/ It will take ~30s to download the data and load the graph. Press the blue "Load" button once the screen is no longer greyed out to display the graph for your selected field of study.

All the code for the project contained in the root directory.

Vizualizing the results (Visualization)

This project folder contains the code and the reduced dataset required to perform the analysis and visualization.

The clustering and preprocessing code takes an extensive amount of time and is included for reference purposes only if one desires to reperform our steps or to view how some of the code works. The original MAG file is very large (330GB) but can be freely found online. It can be found at https://aminer.org/open-academic-graph.

### Analysis Notes

I had spotted these 3 repositories, which seemed like forks, but technically are not, while they **do** seem to contain the same data. Probably independent github imports or some such...








## patwaria/pubcite: A .NET citation analyzer application for scholars. It parses citation information from Google Scholar, Microsoft Academic Research and CiteseerX to populate results.
https://github.com/patwaria/pubcite

### Analysis Notes

7 years old code.

Has CAPTCHA detection and robot error report detection but otherwise looks like almost everyone else: vanilla.

Code is at https://github.com/patwaria/pubcite/blob/master/PubCite/PubCite/GSScraper.cs and https://github.com/patwaria/pubcite/blob/master/PubCite/PubCite/GSAuthorPageScraper.cs


## lyeec9/EssayGenerator: Generates an essay of a specified topic and length through the use of Markov Chains, Machine Learning, and Microsoft Academic Search'sAPI.
https://github.com/lyeec9/EssayGenerator

### Analysis Notes

Just a bit of *fun stuff* I wanted to have a look at...


## tobiagru/ArxivAnalyticsCluster: Tool to run analytics on top of papers from arXiv. Provides a dashboard to explore connections between papers and topics. The analytics run inside a spark cluster. The papers are enriched with the microsoft academic graph.
https://github.com/tobiagru/ArxivAnalyticsCluster

## cran/microdemic:  This is a read-only mirror of the CRAN R package repository. microdemic — 'Microsoft Academic' API Client. Homepage: https://github.com/ropensci/microdemic (devel), https://docs.ropensci.org/microdemic (website) Report bugs for this package: https://github.com/ropensci/microdemic/issues
https://github.com/cran/microdemic

## ninyancat13/deepfake-detection-challenge: Deepfake techniques, which present realistic AI-generated videos of people doing and saying fictional things, have the potential to have a significant impact on how people determine the legitimacy of information presented online. These content generation and modification technologies may affect the quality of public discourse and the safeguarding of human rights—especially given that deepfakes may be used maliciously as a source of misinformation, manipulation, harassment, and persuasion. Identifying manipulated media is a technically demanding and rapidly evolving challenge that requires collaborations across the entire tech industry and beyond. AWS, Facebook, Microsoft, the Partnership on AI’s Media Integrity Steering Committee, and academics have come together to build the Deepfake Detection Challenge (DFDC). The goal of the challenge is to spur researchers around the world to build innovative new technologies that can help detect deepfakes and manipulated media. - Kaggle Competiton Summary (https://www.kaggle.com/c/deepfake-detection-challenge)
https://github.com/ninyancat13/deepfake-detection-challenge

### Analysis Notes

Only a README, nothing else yet after 7 months!



## Microsoft Academic Search · timrdf/locv Wiki
https://github.com/timrdf/locv/wiki/Microsoft-Academic-Search

https://github.com/karpathy/researchlei/issues/4 - Microsoft Academic Search is being retired

see https://github.com/karpathy/researchlei/issues/4

Pull the citation network from:

- CVPR
- ICCV
- ECCV
- ISWC: 360
- IPAW: 2113
- 2242 PSSS Practical and Scalable Semantic Systems
- 3 IAWTIC International Conference on Intelligent Agents, Web Technologiesand Internet Commerce
- 46 ICWE International Conference on Web Engineering
- 49 ICWS International Conference on Web Services
- 476 WAIM Web-Age Information Management
- 487 WES Web Services, E-Business, and the Semantic Web
- 483 WebDB International Workshop on the Web and Databases
- 494 WIDM Web Information and Data Management
- 490 WI Web Intelligence
- 496 WIIW Workshop on Information Integration on the Web
- 526 WWW World Wide Web Conference Series
- 633 DIWeb Data Integration over the Web
- 1956 ESWS European Semantic Web Symposium / Conference
- 2499 SWAP Semantic Web Applications and Perspectives

API User Manual

http://academic.research.microsoft.com/About/Help.htm

All our APIs come with the standard 200 queries per minute.

Each API call returns only 100 items per call.

The dataset is available from https://datamarket.azure.com/dataset/mrc/microsoftacademic after you sign up for an Azure account and "buy" the free subscription to the dataset.

[A Comparison of Article Search APIs via Blinded Experiment and Developer Review](https://journal.code4lib.org/articles/7738)



## Citation Search · pubgem/project-guide Wiki
https://github.com/pubgem/project-guide/wiki/Citation-Search

## Read · wikihub/eduwiki Wiki
https://github.com/wikihub/eduwiki/wiki/Read

https://github.com/wikihub/eduwiki.wiki.git

A wiki for the everyday life of students, junior and senior:

- Learn
- Read
- Watch
- Research
- Build
- Code
- Draw
- Write
- Present
- Work

We use MediaWiki or Markdown markup on this wiki. Here is a tutorial on editting MediaWiki pages and here, you can find the basics about Markdown.



## Writing in LaTeX · cognitionemotionlab/lab-docs Wiki
https://github.com/cognitionemotionlab/lab-docs/wiki/Writing-in-LaTeX

https://github.com/cognitionemotionlab/lab-docs.wiki.git

This repository is built solely for the purpose of housing the Cognition & Emotion Lab's WIKI.

You can see the Wiki at https://github.com/cognitionemotionlab/lab-docs.wiki.git

https://www.overleaf.com/learn/latex/Learn_LaTeX_in_30_minutes


## acite · bavla/biblio Wiki
https://github.com/bavla/biblio/wiki/acite

## Patented Foolishness · sgml/signature Wiki
https://github.com/sgml/signature/wiki/Patented-Foolishness

## microsoft/psi - Platform for Situated Intelligence
https://github.com/microsoft/psi

**Platform for Situated Intelligence** is an open, extensible framework that enables the development, fielding and study of multimodal, integrative-AI systems.

In recent years, we have seen significant progress with machine learning techniques on various perceptual and control problems. At the same time, building end-to-end, multimodal, integrative-AI systems that leverage multiple technologies and act autonomously or interact with people in the open world remains a challenging, error-prone and time-consuming engineering task. Numerous challenges stem from the sheer complexity of these systems and are amplified by the lack of appropriate infrastructure and development tools.

The Platform for Situated Intelligence project aims to address these issues and provide a basis for __developing, fielding and studying multimodal, integrative-AI systems__. The platform consists of three layers. The **Runtime** layer provides a parallel programming model centered around temporal streams of data, and enables easy development of components and applications using .NET, while retaining the performance properties of natively written, carefully tuned systems. A set of **Tools** enable multimodal data visualization, annotations, analytics, tuning and machine learning scenarios. Finally, an open ecosystem of **Components** encapsulate various AI technologies and allow for quick compositing of integrative-AI applications.

For more information about the goals of the project, the types of systems that you can build using it, and the various layers see [Platform for Situated Intelligence Overview](https://github.com/microsoft/psi/wiki/Platform-Overview).

# Using and Building

Platform for Situated Intelligence is built on the .NET Framework. Large parts of it are built on .NET Standard and therefore run both on Windows and Linux, whereas some components are specific and available only to one operating system.

You can build applications based on Platform for Situated Intelligence either by leveraging nuget packages, or by cloning and building the code. Below are instructions:

* [Using \\psi via Nuget packages](https://github.com/microsoft/psi/wiki/Using-via-NuGet-Packages)
* [Building the \\psi codebase](https://github.com/microsoft/psi/wiki/Building-the-Codebase)

# Documentation and Getting Started

The documentation for Platform for Situated Intelligence is available in the [github project wiki](https://github.com/microsoft/psi/wiki). The documentation is still under construction and in various phases of completion. If you need further explanation in any area, please open an issue and label it `documentation`, as this will help us target our documentation development efforts to the highest priority needs.

To learn about Platform for Situated Intelligence, we recommend you begin with the [Brief Introduction](https://github.com/microsoft/psi/wiki/Brief-Introduction), which provides a guided walk-through for some of the main concepts in \\psi. It shows how to create a simple program, describes the core concept of a stream, and explains how to transform, synchronize, visualize, persist and replay streams from disk. We recommend that you first work through the examples in this tutorial to familiarize yourself with these core concepts. 

In addition, a number of tutorials, samples, and other resources can help you learn more about the framework, as described below:

__Tutorials__. Several [tutorials](https://github.com/microsoft/psi/wiki/Tutorials) are available to help you get started with using Platform for Situated Intelligence. You can begin with the [Writing Components](https://github.com/microsoft/psi/wiki/Writing-Components) tutorial, which explains how to write new \\psi components, and the [Pipeline-Execution](https://github.com/microsoft/psi/wiki/Pipeline-Execution) and [Delivery Policies](https://github.com/microsoft/psi/wiki/Delivery-Policies) tutorials, which describe how to control the execution of pipelines and how to control throughput on streams in your application. A number of additional tutorials provide information about the set of [basic stream operators](https://github.com/microsoft/psi/wiki/Basic-Stream-Operators) available in the framework, as well as operators for [stream fusion and merging](https://github.com/microsoft/psi/wiki/Stream-Fusion-and-Merging), [interpolation and sampling](https://github.com/microsoft/psi/wiki/Interpolation-and-Sampling), [windowing](https://github.com/microsoft/psi/wiki/Windowing-Operators), and [stream generation](https://github.com/microsoft/psi/wiki/Stream-Generators).

__Other Topics__. Several documents provide information about various specialized scenarios such as running distributed applications via [remoting](https://github.com/microsoft/psi/wiki/Remoting), [bridging to Python, JS, etc.](https://github.com/microsoft/psi/wiki/Interop), [shared objects and memory management](https://github.com/microsoft/psi/wiki/Shared-Objects), etc.

__Samples__. Besides the tutorials and other topics, it may be helpful to look through the set of [Samples](https://github.com/microsoft/psi/wiki/Samples) provided. While some of the samples address specialized topics such as how to leverage speech recognition components or how to bridge to ROS, reading them will give you more insight into programming with \\psi. In addition, some of the samples have a corresponding detailed walkthrough that explains how the samples are constructed and function, and provide further pointers to documentation and learning materials. Going through these walkthroughs can also help you learn more about programming with Platform for Situated Intelligence.




## manubot/rootstock: Clone me to create your Manubot manuscript
https://github.com/manubot/rootstock


This repository is a template manuscript (a.k.a. rootstock).
Actual manuscript instances will clone this repository (see [`SETUP.md`](SETUP.md)) and replace this paragraph with a description of their manuscript.

### Manubot

Manubot is a system for writing scholarly manuscripts via GitHub.
Manubot automates citations and references, versions manuscripts using git, and enables collaborative writing via GitHub.
An [overview manuscript](https://greenelab.github.io/meta-review/ "Open collaborative writing with Manubot") presents the benefits of collaborative writing with Manubot and its unique features.
The [rootstock repository](https://git.io/fhQH1) is a general purpose template for creating new Manubot instances, as detailed in [`SETUP.md`](SETUP.md).
See [`USAGE.md`](USAGE.md) for documentation how to write a manuscript.

### Analysis Notes

Had picked this one as one of the off-topic "fun bits" to check out and I like it: this is very close to what I've been looking for when it comes to writing papers.

Commended. To be checked out further at a later date when there's time for more *fun stuff*.

**Extra**: check out https://github.com/bokeh/bokeh, which is Python, alas. Compare to D3 et al...













# DOI to citation

## ms609/citation-bot: Citation bot is a tool to expand and format references at Wikipedia. It retrieves citation data from a variety of sources including CrossRef (DOI), PMID, PMC and JSTOR, and returns a formatted citation. Report bugs at https://en.wikipedia.org/wiki/User_talk:Citation_bot
https://github.com/ms609/citation-bot

## Apoc2400/Reftag: Wikipedia citation tool for Google Books, New York Times, ISBN, DOI and more
https://github.com/Apoc2400/Reftag

## ropensci/handlr: convert among citation formats
https://github.com/ropensci/handlr


a tool for converting among citation formats.

heavily influenced by, and code ported from <https://github.com/datacite/bolognese>

supported readers:

- [citeproc][]
- [ris][]
- [bibtex][]
- [codemeta][]

supported writers:

- [citeproc][]
- [ris][]
- [bibtex][]
- [schema.org][]
- [rdfxml][] (requires suggested package [jsonld][])
- [codemeta][]

not supported yet, but plan to:

- crosscite




## papis/papis: Powerful and highly extensible command-line based document and bibliography manager.
https://github.com/papis/papis

### Main features

-  Synchronizing of documents: put your documents in some folder and
   synchronize it using the tools you love: git, dropbox, rsync,
   OwnCloud, Google Drive ... whatever.
-  Share libraries with colleagues without forcing them to open an
   account, nowhere, never.
-  Download directly paper information from *DOI* number via *Crossref*.
-  (optional) **scihub** support, use the example papis script
   `examples/scripts/papis-scihub` to download papers from scihub and
   add them to your library with all the relevant information, in a
   matter of seconds, also you can check the documentation
   [here](http://papis.readthedocs.io/en/latest/scihub.html).
-  Import from Zotero and other managers using
   [papis-zotero](https://github.com/papis/papis-zotero).
-  Create custom scripts to help you achieve great tasks easily
   ([doc](http://papis.readthedocs.io/en/latest/scripting.html)).
-  Export documents into many formats (bibtex, yaml..)
-  Command-line granularity, all the power of a library at the tip of
   your fingers.



## dotcs/doimgr: Command line tool using crossref.org's API to search DOIs and obtain formatted citations such as bibtex, apa, and a lot more
https://github.com/dotcs/doimgr

## nushio3/citation-resolve: convert document identifiers such as DOI, ISBN, arXiv ID to bibliographic reference.
https://github.com/nushio3/citation-resolve

convert document identifiers such as DOI, ISBN, arXiv ID to bibliographic reference.



## CrossRef/reddit-dump-experiment: Experimental extraction of DOI citation information from Reddit submission dump.
https://github.com/CrossRef/reddit-dump-experiment

Quick analysis to look at the use of DOIs in Reddit submissions over time. Can be run locally using these instructions or on a cluster using instructions found in Spark docs.

Code here is a little hacky, but does the job. Suggestions welcome.


## foucault/citation: Generate bibtex entries from Document Object Identifiers (DOI)
https://github.com/foucault/citation

citation is a dead simple Python script used to download readily formatted citations for use in bibtex just by providing its Document Object Identifier (DOI). Cut and paste the output into your .bib file and you are ready to go!



## haqle314/doi2citation
https://github.com/haqle314/doi2citation

Just a one-liner shell script to query the API available at [doi.org](https://doi.org) for the citation text for a DOI.


## Lachlan00/EasyCite: Easily download academic citation bib files and pdfs from DOIs
https://github.com/Lachlan00/EasyCite

A simple Python script to download bibtex citations and paper PDFs. Bibtex files pulled from [http://dx.doi.org](http://dx.doi.org/) and PDFs downloaded from SciHub using [scidownl](https://pypi.org/project/scidownl/). 



## machnine/citationgen: Generate citation files from doi/pmid etc.
https://github.com/machnine/citationgen

## kjgarza/doi-citation
https://github.com/kjgarza/doi-citation

## pierre-24/goto-publication: Citation-based URL/DOI searches
https://github.com/pierre-24/goto-publication


*Citation-based URL/DOI searches*, by `Pierre Beaujean <https://pierrebeaujean.net>`.
CLI version of [that previous project](https://github.com/pierre-24/goto-publication-old/).

Because the journal, the volume and the page (and, sometimes, yeah, the issue) should be enough to find an article (for which, of course, you don't have the DOI).

Since I have a (quantum) chemistry background, I will limit this project to the journals that are in the chemistry and physics fields.
I'm working on that, but feel free to propose [improvements](https://github.com/pierre-24/goto-publication/pulls).



## ropenscilabs/rcitoid: Citation data via Wikimedia using the Citoid service
https://github.com/ropenscilabs/rcitoid

Client for the Citoid service <https://www.mediawiki.org/wiki/Citoid>

docs: <https://en.wikipedia.org/api/rest_v1/#!/Citation/getCitation>



## pierre-24/goto-publication-old: Citation-based DOI searches and redirections
https://github.com/pierre-24/goto-publication-old

Citation-based URL/DOI searches and redirections, by Pierre Beaujean.

Because the journal, the volume and the page should be enough to find an article (for which, of course, you don't have the DOI, otherwise this is stupid).

Note: Since I have a (quantum) chemistry background, I will limit this project to the journals that are in the chemistry and physics fields. Feel free to fork the project if you want something else :)


## gaberoo/doitex: Use doi citations in Latex and fetch automatically from CrossRef.
https://github.com/gaberoo/doitex

## exquisapp/CitationApp: A Node Js Application That Uses Zotero's Translation Server To Easily Cite When Queried With Sources Like URI, DOI, ISBN, Titles ...and so on.
https://github.com/exquisapp/CitationApp

## cityofaustin/doi-automation: automation for DataCite DOI citation integration with Socrata
https://github.com/cityofaustin/doi-automation

Datacite (https://en.wikipedia.org/wiki/DataCite) is a non profit organization which provides an easy way to register, cite, and access datasets online.
The City of Austin would like to use this organization's tools to garner insight into the usage of our open data as well as give the public a simple and effective way to cite our open data for any use.

To see some examples of our citations see:
https://search.datacite.org/members/austintx

This project's goal is to explore and implement an integration between DataCite's citation repository and the city's Socrata Open Data portal (https://data.austintexas.gov/). This will be done by developing automation to synchronize datacite's DOI repository with the City of Austin's socrata portal assets and metadata using the two organization's APIs and a python backend.

Socrata Discovery API:
https://socratadiscovery.docs.apiary.io/#

DataCite REST API:
https://support.datacite.org/docs/api



## dvdmrn/citation_scraper: searches pdfs for their doi and attempts to find a pubmed citation
https://github.com/dvdmrn/citation_scraper

### Dependencies:
  * Python 2
  * Scholarly (Install by opening a terminal window and typing `pip install scholarly`)
  * metapub (Install with the same method)
  * pdfminer (Install with the same method)

### How to use:
  * Place all the unprocessed pdfs you want to analyze into the folder `pdfs_to_analyze`
  * Open the program in terminal by typing `python citation_scraper.py`
  * Enter the name that you would like to call your output .csv file
  * A new file will be created in the directory containing `citation_scraper.py`
  * Just relax



## JPFrancoia/reftool: ref2pdf returns a DOI from a formatted citation.
https://github.com/JPFrancoia/reftool

reftool is a simple script to return a DOI from a formatted citation. It can also return a direct link to download the article from Sci-Hub (this option could be illegal, use at your own risk).

This script uses [Crossref's Simple Text Query Tool](https://doi.crossref.org/simpleTextQuery).

Usage is limited to 1000 requests per user/per month, and requires signing up on Crossref's website. The script needs the email address you used to sign up.


## cran/citation:  This is a read-only mirror of the CRAN R package repository. citation — Software Citation Tools. Homepage: https://github.com/pik-piam/citation, https://doi.org/10.5281/zenodo.3813429 Report bugs for this package: https://github.com/pik-piam/citation/issues
https://github.com/cran/citation

## ETspielberg/title2doi: A backend service to transform a list of citations to dois and further to mods via scopus
https://github.com/ETspielberg/title2doi

A small FLASK web service, taking a filename from the POST request, loads the corresponding file from the LIBINTEL_:UPLOAD_DIR directory containing an unformatted list of references.

each reference (= each line) is queried at the CrossRef-API to retrieve the corresponding DOI. If a DOI is found, the MyCoRe repository is queried, whether it contains the entry. In addition, Scopus is queried to retrieve the Scopus ID and actual citation counts. Results are written to the results.txt as spread sheet ascii (delimited by ;).

## YutoMizutani/getdoi: A tool for getting the academic article's DOI from citation text.
https://github.com/YutoMizutani/getdoi

## cbosoft/bibget.py: Fetches citation data given a pdf or doi, returns in bibTeX format.
https://github.com/cbosoft/bibget.py

## joesingo/paperfinder: Script to find the DOI and BibTeX citation for a paper given it's URL on the publisher's website
https://github.com/joesingo/paperfinder

iven a URL to a paper on a publisher's website, find its DOI and a BibTex citation. Output can be given as plain text or JSON.

This takes some of the pain out of dealing with publishers' websites. Of course, it is possible to pair this tool with SciHub to get the actual PDF (go to https://sci-hub.se/<DOI>), but I could not possibly endorse piracy in this way...

Note that pf works for a very small number of publishers, and may break if publisher web pages or URLs change.

Supported publishers

- ScienceDirect
- Springer

## NANCYVALDEBENITO/articles_scopus_wos: Look for articles with scopus and web of science using python selenium. Only with DOI you can obtain affiliations, total citations, journal name and journal Rank, Journal Impact Factory ... among others
https://github.com/NANCYVALDEBENITO/articles_scopus_wos

## NationalLimerickProductions/seq2cite: seq2cite is a citation recommendation engine that improves upon the word of Ebisu & Fang (2017) (https://dl.acm.org/doi/abs/10.1145/3077136.3080730) to recommend citations from small pieces of scientific text. We demonstrate our system with the CORD-19 dataset of articles related to COVID-19.
https://github.com/NationalLimerickProductions/seq2cite

















# keyword extraction

## AimeeLee77/keyword_extraction: 利用Python实现中文文本关键词抽取，分别采用TF-IDF、TextRank、Word2Vec词聚类三种方法。
https://github.com/AimeeLee77/keyword_extraction

## bigzhao/Keyword_Extraction: 神策杯2018高校算法大师赛（中文关键词提取）第二名代码方案
https://github.com/bigzhao/Keyword_Extraction

## aneesha/RAKE: A python implementation of the Rapid Automatic Keyword Extraction
https://github.com/aneesha/RAKE

## LIAAD/yake: Single-document unsupervised keyword extraction
https://github.com/LIAAD/yake

Unsupervised Approach for Automatic Keyword Extraction using Text Features.

YAKE! is a light-weight unsupervised automatic keyword extraction method which rests on text statistical features extracted from single documents to select the most important keywords of a text. Our system does not need to be trained on a particular set of documents, neither it depends on dictionaries, external-corpus, size of the text, language or domain. To demonstrate the merits and the significance of our proposal, we compare it against ten state-of-the-art unsupervised approaches (TF.IDF, KP-Miner, RAKE, TextRank, SingleRank, ExpandRank, TopicRank, TopicalPageRank, PositionRank and MultipartiteRank), and one supervised method (KEA). Experimental results carried out on top of twenty datasets (see Benchmark section below) show that our methods significantly outperform state-of-the-art methods under a number of collections of different sizes, languages or domains.

### Main Features

* Unsupervised approach
* Corpus-Independent
* Domain and Language Independent
* Single-Document

### Benchmark

YAKE!, generically outperforms, statistical methods [tf.idf (in 100% of the datasets), kp-miner (in 55%) and rake (in 100%)], state-of-the-art graph-based methods [TextRank (in 100% of the datasets), SingleRank (in 90%), TopicRank (in 70%), TopicalPageRank (in 90%), PositionRank (in 90%), MultipartiteRank (in 75%) and ExpandRank (in 100%)] and supervised learning methods [KEA (in 70% of the datasets)] across different datasets, languages and domains. The results listed in the table refer to F1 at 10 scores. Bold face marks the current best results for that specific dataset. The column "Method" cites the work of the previous (or current) best method (depending where the bold face is found). The interested reader should refer to [__this table__](https://github.com/LIAAD/yake/blob/master/docs/YAKEvsBaselines.jpg) in order to see a detailed comparison between YAKE and all the state-of-the-art methods.


## boudinfl/pke: Python Keyphrase Extraction module
https://github.com/boudinfl/pke

pke is an open source python-based keyphrase extraction toolkit. It provides an end-to-end keyphrase extraction pipeline in which each component can be easily modified or extended to develop new models. pke also allows for easy benchmarking of state-of-the-art keyphrase extraction models, and ships with supervised models trained on the SemEval-2010 dataset.

### Implemented models

pke currently implements the following keyphrase extraction models:

* Unsupervised models
  * Statistical models
    * TfIdf [[documentation](https://boudinfl.github.io/pke/build/html/unsupervised.html#tfidf)]
    * KPMiner [[documentation](https://boudinfl.github.io/pke/build/html/unsupervised.html#kpminer), [article by (El-Beltagy and Rafea, 2010)](http://www.aclweb.org/anthology/S10-1041.pdf)]
    * YAKE [[documentation](https://boudinfl.github.io/pke/build/html/unsupervised.html#yake), [article by (Campos et al., 2020)](https://doi.org/10.1016/j.ins.2019.09.013)]
  * Graph-based models
    * TextRank [[documentation](https://boudinfl.github.io/pke/build/html/unsupervised.html#textrank), [article by (Mihalcea and Tarau, 2004)](http://www.aclweb.org/anthology/W04-3252.pdf)]
    * SingleRank  [[documentation](https://boudinfl.github.io/pke/build/html/unsupervised.html#singlerank), [article by (Wan and Xiao, 2008)](http://www.aclweb.org/anthology/C08-1122.pdf)]
    * TopicRank [[documentation](https://boudinfl.github.io/pke/build/html/unsupervised.html#topicrank), [article by (Bougouin et al., 2013)](http://aclweb.org/anthology/I13-1062.pdf)]
    * TopicalPageRank [[documentation](https://boudinfl.github.io/pke/build/html/unsupervised.html#topicalpagerank), [article by (Sterckx et al., 2015)](http://users.intec.ugent.be/cdvelder/papers/2015/sterckx2015wwwb.pdf)]
    * PositionRank [[documentation](https://boudinfl.github.io/pke/build/html/unsupervised.html#positionrank), [article by (Florescu and Caragea, 2017)](http://www.aclweb.org/anthology/P17-1102.pdf)]
    * MultipartiteRank [[documentation](https://boudinfl.github.io/pke/build/html/unsupervised.html#multipartiterank), [article by (Boudin, 2018)](https://arxiv.org/abs/1803.08721)]
* Supervised models
  * Feature-based models
    * Kea [[documentation](https://boudinfl.github.io/pke/build/html/supervised.html#kea), [article by (Witten et al., 2005)](https://www.cs.waikato.ac.nz/ml/publications/2005/chap_Witten-et-al_Windows.pdf)]
    * WINGNUS [[documentation](https://boudinfl.github.io/pke/build/html/supervised.html#wingnus), [article by (Nguyen and Luong, 2010)](http://www.aclweb.org/anthology/S10-1035.pdf)]





## zelandiya/keyword-extraction-datasets: Different datasets for developing and testing keyword extraction algorithms
https://github.com/zelandiya/keyword-extraction-datasets

## ibatra/BERT-Keyword-Extractor: Deep Keyphrase Extraction using BERT
https://github.com/ibatra/BERT-Keyword-Extractor

## Keyword Extraction Datasets
https://github.com/zelandiya/keyword-extraction-datasets

Different datasets for developing, evaluating and testing keyword extraction algorithms. For benchmarking performance see: O. Medelyan. 2009. Human-competitive automatic topic indexing. PhD Thesis. University of Waikato, New Zealand.

Extracting keywords using a controlled vocabulary or a thesaurus as a source:

NLM_500.zip - 500 PubMed documents with MeSH terms

fao780.tar.gz - 780 FAO publications with Agrovoc terms

fao30.tar.gz - 30 FAO publications, each annotated by 6 professional FAO indexers

Free-text keyword extraction (without a vocabulary):

citeulike180.tar.gz - 180 publications crawled from CiteULike, and keywords assigned by different CiteULike users who saved these publications

SemEval2010-Maui.zip - SemEval-2010 Keyphrase extraction track data in Maui format

keyphrextr.tar.gz - Keyphrase extraction model created using SemEval-2010 training data. This model is used in the Maui GPL demo when no vocabulary is selected.

Extracting keywords using Wikipedia as a controlled vocabulary of allowed terms:

wiki20.tar.gz - 20 Computer Science papers, each annotated with at least 5 Wikipedia articles by 15 teams of indexers



## JRC1995/TextRank-Keyword-Extraction: Keyword extraction using TextRank algorithm after pre-processing the text with lemmatization, filtering unwanted parts-of-speech and other techniques.
https://github.com/JRC1995/TextRank-Keyword-Extraction

Based on: "TextRank: Bringing Order into Texts - by Rada Mihalcea and Paul Tarau"


## demoyhui/KeywordExtraction: 基于LDA和TextRank的关键子提取算法实现
https://github.com/demoyhui/KeywordExtraction

## Ismael-Hery/rake-keywords: Javascript implementation of the "Rake" keywords extraction algorithm
https://github.com/Ismael-Hery/rake-keywords

Some problems with the Rake original scientific paper

Errors in the paper

- 'numbers' is a stop word in the original Fox stop words list, thus 'natural numbers' can not be a candidate keywords. I removed numbers from the Fox stop list as they probably did for the paper (otherwise they would not have found 'natural numbers')

- the paper does not find mixed types as a candidate keywords. I've added mixed types as a candidates key words

- Non english language

TODO :

- compute keywords from a corpus of articles (see sci paper with computation of 'essential' keywords)
- French implementation with 'mots de liaisons' du/des/d'/… excluded from stop list



## waseem18/node-rake: A NodeJS implementation of the Rapid Automatic Keyword Extraction algorithm.
https://github.com/waseem18/node-rake

## sleepycat/rapid-automated-keyword-extraction: A Javascript implementation of the Rapid Automated Keyword Extraction (RAKE) algorithm
https://github.com/sleepycat/rapid-automated-keyword-extraction

## shopping24/rake-js: JS Implementation of the Rapid Automatic Keyword Extraction Paper
https://github.com/shopping24/rake-js

RAKE is the acronym for Rapid Automated Keyword Extraction. The basic algorithm is described by Stuart Rose, Dave Engel, Nick Cramer and Wendy Cowley in their paper "Automatic keyword extraction from individual documents" (©2010, John Wiley & Sons, Ltd, Source click here).

In short RAKE describes splitting a text into fragments by stop words. Stop words are always considered to be irrelevant to the context. The RAKEd result of Red Zebra and Jaguar would therefore be [Red Zebra, Jaguar].

The score is then calculated by counting the individual words and and creating degrees based on the length of found fragments.

### What is this repository about?

This repository includes advanced methods in addition to the original RAKE description. Furthermore we added a functional wrapper as feature for a more flexible way of handling keyword extraction. The process consists of these steps:

- Extracting fragments from any given text using various available methods.
- Score the fragments.
- Retrieve the end result.

Extraction and scoring functions from any source making use of the Phrases and Phrase classes may be used and executed in the desired order.


## colefichter/NRake: A C# implementation of Rapid Automatic Keyword Extraction (RAKE)
https://github.com/colefichter/NRake

This is an implementation based on the algorithm described in the paper "Automatic keyword extraction from individual documents" http://media.wiley.com/product_data/excerpt/22/04707498/0470749822.pdf.


## benmcevoy/Rake: A C# implementation of the Rapid Automatic Keyword Extraction
https://github.com/benmcevoy/Rake

## fromskyblue/Keywords-Extraction: Zhen Yang-Keywords Extraction
https://github.com/fromskyblue/Keywords-Extraction

Keyword extraction by entropy difference between the intrinsic and extrinsic mode
We strive to propose a new metric to evaluate and rank the relevance of words in a text. The method uses the Shannon’s entropy difference between the intrinsic and extrinsic mode, which refers to the fact that relevant words significantly reflect the author’s writing intention, i.e., their occurrences are modulated by the author’s purpose, while the irrelevant words are distributed randomly in the text. By using The Origin of Species by Charles Darwin as a representative text sample, the performance of our detector is demonstrated and compared to previous proposals. Since a reference text ‘‘corpus’’ is all of an author’s writings, books, papers, etc. his collected works is not needed. Our approach is especially suitable for single documents of which there is no a priori information available.


## GomesNayagam/keyword-extraction-single-document: keyword extraction from single document, algorithm from this paper http://ymatsuo.com/papers/ijait04.pdf
https://github.com/GomesNayagam/keyword-extraction-single-document

## ruiyuanxu/MizumotoKeywordExtraction: A keyword extration tool built for Data Structure & Algorithm course.
https://github.com/ruiyuanxu/MizumotoKeywordExtraction

C#


## ASH1998/Keyword-extraction: Keyword Extraction for PDFs
https://github.com/ASH1998/Keyword-extraction

Dependencies:

- PyPDF2
- sklearn
- pandas

Algorithm used

LDA : Linear Discriminant Analysis A classifier with a linear decision boundary, generated by fitting class conditional densities to the data and using Bayes’ rule. The model fits a Gaussian density to each class, assuming that all classes share the same covariance matrix. The fitted model can also be used to reduce the dimensionality of the input by projecting it to the most discriminative directions.

NMF : Non-Negative Matrix Factorization (NMF) - Find two non-negative matrices (W, H) whose product approximates the non- negative matrix X. This factorization can be used for example for dimensionality reduction, source separation or topic extraction.



## WuLC/KeywordExtraction: Implementation of algorithm in keyword extraction,including TextRank,TF-IDF and the combination of both
https://github.com/WuLC/KeywordExtraction

Implementation of serveral algorithms for keyword extraction,including TextRank,TF-IDF,TextRank along with TFTF-IDF.Cutting words and filtering stop words are relied on HanLP

## hankcs/HanLP: Han Language Processing
https://github.com/hankcs/HanLP

Natural Language Processing for the next decade. Tokenization, Part-of-Speech Tagging, Named Entity Recognition, Syntactic & Semantic Dependency Parsing, Document Classification


The multilingual NLP library for researchers and companies, built on TensorFlow 2.0, for advancing state-of-the-art deep learning techniques in both academia and industry. HanLP was designed from day one to be efficient, user friendly and extendable. It comes with pretrained models for various human languages including English, Chinese and many others. 



## Linguistic/rake: A Java library for Rapid Automatic Keyword Extraction (RAKE) 🍂
https://github.com/Linguistic/rake

RAKE is an algorithm for extracting keywords (technically phrases, but I don't question scientific literature) from a document that have a high relevance or importance to the contents of the document.


## sing1ee/textrank-java: a simple implementation of textrank algorithm for nlp keywords extraction
https://github.com/sing1ee/textrank-java

## ibatra/BERT-Keyword-Extractor: Deep Keyphrase Extraction using BERT
https://github.com/ibatra/BERT-Keyword-Extractor

## aespresso/chinese_nlp_tutorial_clustering_keywords_extraction: 中文自然语言处理聚类与关键词提取教程
https://github.com/aespresso/chinese_nlp_tutorial_clustering_keywords_extraction

## BastinFlorian/Keywords_extraction_with_GOW: Graph of words (Networkx) and keywords extraction (Ktruss, Kcore, DivRank, BestCoverage)
https://github.com/BastinFlorian/Keywords_extraction_with_GOW

* First we present an example of the methods used to extract keywords (see Graph of words and keywords extraction.ipynb and K-truss_code_example.ipynb)
* Then we give a code to compute the k_core and obtain the graphs of directories of files or all files in directories containing sub-directories (see K_core_corpus.py)
* We also give an implementation of the K-truss algorithm (see K-truss_code.py)
* We make a time analysis to see the evolution of some words through time, in order to detect events related to them.


## RHKeng/ShenCeCup: A competition on DataCastle which is about text keyword extraction ! Rank 6 / 622 !
https://github.com/RHKeng/ShenCeCup

A competition on DataCastle which is about text keyword extraction! Rank 6/622!

" Shence Cup" 2018 College Algorithm Masters is a single-player competition that can only be soloed by college students. Shence Data provides the titles and texts of about 100,000 news articles, of which 1,000 articles have corresponding annotation data. There are no more than 5 keywords for each article in the labeled data, and the keywords have appeared in the title or body of the article. According to the existing data, it is necessary to train a "keyword extraction" model to extract the keywords of articles without annotated data, and submit at most two keywords for each article.

Final ranking: 6/622

\[...]

### 5 Model selection

Compare the effects of unsupervised models (tfidf/tfiwf, textRank, topic model LSI/LDA), and finally use tfidf as the basic model to select the keyword candidate set.

#### 5.1 The tfidf
tfidf (term frequency-inverse document frequency) algorithm is a statistical method used to evaluate the importance of a word to a document set or a document in a corpus. The importance of a word increases in proportion to the number of times it appears in the document, but at the same time it decreases in inverse proportion to the frequency of its appearance in the corpus.

TF (term frequency) is the number of times a word appears in the article, TF (term frequency) = the number of times a word appears in the article / the total number of words in the article; IDF (inverse document frequency) is the frequency of the word , IDF reverse document frequency=log (total number of documents in the corpus/(total number of documents containing the word+1)), if a word is more common, then its denominator is larger, and the IDF value is smaller.

#### 5.2 Tfiwf

TF remains unchanged, IWF is the sum of the word frequency of all words in the document/the word frequency

#### 5.3 Pagerank (listed here only to lead to the following textrank)

need to know which webpages are linked to webpage A, that is, first get webpage A's access to the chain, and then calculate webpage A's PR by voting for webpage A from the access chain value. This design can ensure the achievement of such an effect: when some high-quality webpages point to webpage A, then the PR value of webpage A will increase because of these high-quality votes, and webpage A is pointed to by fewer webpages or by some When a web page with a lower PR value points to, the PR value of A will not be very large, which can reasonably reflect the quality level of a web page. Vi represents a certain webpage, Vj represents a webpage linked to Vi (that is, the in-link of Vi), S(Vi) represents the PR value of the webpage Vi, In(Vi) represents the collection of all in-links of the webpage Vi, Out(Vj) Represents the number of web pages Vj linked to other web pages, and d represents the damping coefficient, which is used to overcome the inherent defects of the part after "d *" in this formula: if there is only a summation part, then the formula will not be able to handle The PR value of the web pages that enter the chain, because at this time, according to the formula, the PR value of these web pages is 0, but the actual situation is not like this, so a damping coefficient is added to ensure that each web page has a PR value greater than 0. According to the experimental results, with a damping coefficient of 0.85, the PR value can be converged to a stable value after about 100 iterations. When the damping coefficient is close to 1, the number of iterations required will increase abruptly and the sorting will be unstable. The score in front of S(Vj) in the formula refers to the PR value of Vj that should be divided equally among all the webpages pointed to by Vj, so that it can be regarded as dividing one's votes among the webpages that one links to.

#### 5.4 textrank 

is a graph-based sorting algorithm for text, which can realize keyword extraction only by using the information of a single document itself, without relying on a corpus. (Calling the interface of jieba ) Wji refers to the similarity between the two sentences of Vi and Vj. Edit distance and cosine similarity can be used. When textrank is applied to keyword extraction, it is different from automatic abstract extraction: 1) The association between words has no weight, that is, Wji is 1; 2) Each word is not linked to all words in the document, but through Set a fixed-length sliding window format, with links between words in the window.

#### 5.5 The Topic Model 

topic model believes that there is no direct connection between words and documents, and that they should be connected by a dimension, which is the topic. The topic model is an automatic analysis of each document, counting the words in the document, and judging which topics the current document contains and the proportion of each topic based on the statistical information. A topic model is a generative model. Each word in an article is obtained through a process of "select a topic with a certain probability, and select a word from this topic with a certain probability"; topic models are commonly used The methods are LSI (LSA) and LDA, where LSI uses SVD (Singular Value Decomposition) for brute force cracking, while LDA uses Bayesian methods to fit distribution information. Through the LSA or LDA algorithm, you can get the distribution of the document to the topic and the distribution of the topic to the word. The distribution of the word to the topic can be obtained according to the topic to the word distribution (Bayesian method), and then through this distribution and the document to the topic distribution Calculate the similarity between the document and the word, and select the word list with high similarity as the key word of the document.

#### 5.5.1 LSA

Latent Semantic Analysis (LSA), also called Latent Semantic Indexing, LSI. It is a commonly used simple topic model. LSA is a way to get the text topic based on the singular value decomposition (SVD) method. Umk represents the distribution matrix of documents to topics, and the transposition of Vnk represents the distribution matrix of topics to words. LSA uses SVD to express words and documents more essentially, and maps them to low-dimensional spaces. While limited use of text semantic information, LSA greatly reduces the cost of calculation and improves the quality of analysis. However, the computational complexity is very high, and the feature space dimension is large, and the computational efficiency is very low. When a new document enters the existing feature space, the entire space needs to be retrained to obtain the distribution information of the newly added document. In addition, there are problems of insensitivity to frequency distribution and weak physical interpretation.

#### 5.5.2 pLSA

has been improved on the basis of LSA, by using the EM algorithm to fit the distribution information instead of using SVD for brute force cracking.

In PLSA, the bag-of-words model is also used (the bag-of-words model refers to a document. We only consider whether a word appears, regardless of the order in which it appears. On the contrary, n-gram considers the order in which the words appear). And the documents are independently exchangeable, and the words in the same document are also independently exchangeable. In PLSA, we will extract a topic word with a fixed probability, then find the corresponding word distribution according to the extracted topic word, and then extract a vocabulary according to the word distribution.

#### 5.5.3 LDA

LDA is based on PLSA and adds two Dirichlet prior distributions to topic distribution and word distribution. In PLSA, both topic distribution and word distribution are uniquely determined. However, in LDA, topic distribution and word distribution are uncertain. The authors of LDA adopt Bayesian thinking and believe that they should obey a distribution. Both topic distribution and word distribution are polynomial distributions, because polynomial distributions Dirichlet distribution and Dirichlet distribution are conjugate structures. In LDA, topic distribution and word distribution use Dirichlet distribution as their conjugate prior distribution.

In LDA, there is no fixed optimal solution for the number of topics. When training the model, the number of topics needs to be set in advance, and the trainer needs to manually adjust the parameters according to the training results, and then optimize the number of topics. We can find the posterior distribution according to the polynomial distribution and the prior distribution of the data, and then use this posterior distribution as the next prior distribution, and iteratively update. There are generally two solving methods, the first is based on Gibbs sampling algorithm, and the second is based on variational inference EM algorithm.




## XuMuK1/KeywordsExtraction: Project for courses NLA and Optimization in Sk. The goal is to learn how to test different techniques for extracting keywords from news.
https://github.com/XuMuK1/KeywordsExtraction

## pozhidaevsa/ExtractionKeywords: Extract keywords from russian text
https://github.com/pozhidaevsa/ExtractionKeywords

## csurfer/rake-nltk: Python implementation of the Rapid Automatic Keyword Extraction algorithm using NLTK.
https://github.com/csurfer/rake-nltk

RAKE short for Rapid Automatic Keyword Extraction algorithm, is a domain independent keyword extraction algorithm which tries to determine key phrases in a body of text by analyzing the frequency of word appearance and its co-occurance with other words in the text.


## AidenHuen/SMP-Keyword-Extraction: CSDN博客的关键词提取算法，融合TF，IDF，词性，位置等多特征。该项目用于参加2017 SMP用户画像测评，排名第四,在验证集中精度为59.9%，在最终集中精度为58.7%。模型并未使用机器学习的方法，具有较强的泛化能力。
https://github.com/AidenHuen/SMP-Keyword-Extraction

About CSDN blog keyword extraction algorithm, fusion TF, IDF, part of speech, location and other features. 

This project was used to participate in the 2017 SMP user portrait evaluation, ranking fourth, with an accuracy of 59.9% in the verification set and 58.7% in the final set. 
The model does not use machine learning methods and has strong generalization capabilities.




## bguvenc/keyword_extraction: Keyword extraction with Word2Vec
https://github.com/bguvenc/keyword_extraction

Keyword extraction method by using Word2Vec and Pagerank algorithms
The most common representation of distributional semantics is called one-hot representation in which dimensionality is equal to vocabulary’s cardinality. Elements of this vector space representation consist of 0’s and 1’s. However, this representation has some disadvantages. For example, in these representations, it is difficult to make deductions about word similarity. Due to high dimensionality, they can also cause overfitting. Moreover, it is computationally expensive.

Word embeddings are designed to capture attributional similarities between vocabulary items. Words that appear in similar contexts should be close to each other in the projected vector space. This means that grouping of words in a vector space must share same semantic properties. In word embeddings, Latent Semantic Analysis (LSA) uses a counting base dimensionality reduction method. Word2Vec is created as an alternative. Its low dimensionality can help to reduce computational complexity. Also compared with distributional semantics methods, it causes less overfitting. Word2Vec can also detect analogies between words.

Our model takes Word2Vec representations of words in a vector space. While we construct the Word2Vec model, we decide a threshold of counts of words because words that appear only once or twice in a large corpus are probably not unusual for the model, and there is not enough data to make any meaningful training on those words. A reasonable value for minimum counts changes between 0-100, and it depends on the size of corpora. Another critical parameter for Word2Vec model is the dimension of the vectors. This value changes between 100 and 400. Dimensions larger than 400 require more training but leads to more accurate models. I used Google News corpora which provided by Google which consist of 3 million word vectors. I did not remove stop words or infrequent words because these algorithms use windows and to find vector representations. So I need the nearby words to find vector representations.

The second step of this algorithm is to find PageRank value of each word. PageRank algorithm works with random walk. The original PageRank algorithm takes internet pages as a node. In our model PageRank algorithm takes Word2Vec representations of words. The cosine distance is used to calculate edge weights between nodes. TextRank algorithm uses a similar method. While TextRank chooses the bag of word representations of words and a different similarity measure in finding edge weights, in this algorithm I used the Word2Vec representations and the cosine similarity. After PageRank values of words are found, we can get words which have the highest PageRank values. Finally, these words can be seen as a keyword of a text.




## gaussic/tf-idf-keyword: Keyword extraction based on TF-IDF on specific corpus. 基于特定语料库的TF-IDF的中文关键词提取
https://github.com/gaussic/tf-idf-keyword

Chinese keyword extraction 

requirements based on TF-IDF. 


## naushadzaman/keyword-extraction-from-tweets: keyword extraction from tweets using python
https://github.com/naushadzaman/keyword-extraction-from-tweets

keyword extraction from tweets using python

In this module, we use Pattern tools to do POS tagging/Phrase extraction of tweets. The usual POS tagging/chunking tools do not work well for free form texts like tweets, so we needed to use a tool that is designed and trained for twitter/tweets. From Pattern tool output, we extract phrases as entities. You can decide to use on NP (Noun Phrase), but our default is to use NP (Noun Phrase) and ADJP (Adjective Phrase). With this tool, you can also extract hashtags, usernames, urls from the tweet.


## vgrabovets/multi_rake: Multilingual Rapid Automatic Keyword Extraction (RAKE) for Python
https://github.com/vgrabovets/multi_rake

Multilingual Rapid Automatic Keyword Extraction (RAKE) for Python

Features

- Automatic keyword extraction from text written in any language
- No need to know language of text beforehand
- No need to have list of stopwords
- 26 languages are currently available, for the rest - stopwords are generated from provided text
- Just configure rake, plug in text and get keywords (see implementation details)




## lovit/soykeyword: Python library for keyword extraction
https://github.com/lovit/soykeyword

Python library for Keyword Extraction 

Python library for keyword/association extraction. Keywords and related words extracted from by Lovit (Hyunjoong) and Hunsik Shin 

soykeyword are defined as follows. Keywords in a set of documents are words of good quality (discriminative power) that can distinguish them from other sets of documents, and words (high coverage) that can describe them well. Words with a low frequency are more likely to appear in only one set, so they have a high level of discrimination, but weak explanation. The proposed two algorithms select words that have high explanatory and distinguishing power as keywords. An associative word defines a keyword that separates a set of documents with and without a reference word from an associative word. This also means that the word with high co-occurrence. Choose words with high co-occurrence and good explanation. 



## tarwn/bookmark_analysis: Exploration of text analysis for automatic bookmarking/keyword extraction
https://github.com/tarwn/bookmark_analysis

Automated Keyword Extraction – TF-IDF, RAKE, and TextRank

After initially playing around with text processing in my prior post, I added an additional algorithm and cleaned up the logic to make it easier to perform test runs and reuse later. I tweaked the RAKE algorithm implementation and added TextRank into the mix, with full sample code and links to sources available. I’m also using a read-through cache of the unprocessed and processed files so I can see the content and tweak the cleanse logic.

Context: The ultimate goal is to build a script that could process through 6 years of my bookmarked reading and extract out keywords, so I could do some trend analysis on how my reading has changed over time and maybe later build a supervised model with that data to analyze new online posts and produce a “worth my time or not” score.


## Parsely/serpextract: Easy extraction of keywords and engines from search engine results pages (SERPs).
https://github.com/Parsely/serpextract

serpextract provides easy extraction of keywords from search engine results pages (SERPs).

This module is possible in large part to the very hard work of the Piwik team. Specifically, we make extensive use of their list of search engines.



## singularity014/Keyword-Extraction-Bidirectional-LSTM: Deep learning LSTM + BERT based approach for labelling a corpus with keywords, then training a model to extract keywords.
https://github.com/singularity014/Keyword-Extraction-Bidirectional-LSTM

Deep learning Bi-LSTM based approach for labelling a corpus with keywords, then training a model to extract keywords.

Article was late published in pprints.



## pemagrg1/Hindi-POS-Tagging-and-Keyword-Extraction: Hindi POS Tags and keywords using TNT model. Created Date: 28 Sept 2018
https://github.com/pemagrg1/Hindi-POS-Tagging-and-Keyword-Extraction

Part of speech plays a very major role in NLP task as it is important to know how a word is used in every sentences. POS tagging is used mostly for Keyword Extractions, phrase extractions, Named Entity Recognition, etc. Before going further on POS tagging, I am assuming that you all know about part of speech as we all have studied grammar during school. Didn't we? But anyways let me give a brief explanation on it!

There are eight main Parts of Speech: Nouns(naming word), Pronouns(replaces a noun), Adjectives(describing word), Verbs(action word), Adverbs(describes a verb), Prepositions(shows relationships), Conjunctions(joining word) and Interjections(Expressive word). Most of it are further divided into sub-parts. Noun is divided into Proper Nouns, Common Nouns, Concrete Nouns etc.

Reminds you of school days?? Okay now lets start with Hindi Part of Speech Tagging.

Hindi Part of Speech Tagging is something that people are still doing research on as we have various techniques and libraries available for English Text and rarely for Hindi Text. [1] Manish and Pushpak researched on Hindi POS using a simple HMM based POS tagger with accuracy of 93.12%. while [2]Nisheeth Joshi, Hemant Darbari and Iti Mathur also researched on Hindi POS using Hidden Markov Model with frequency count of two tags seen together in the corpus divided by the frequency count of the previous tag seen independently in the corpus. [3] S Phani Kumar Gadde, Meher Vijay Yeleti used CRF based tagger and Brants TnT (Brants, 2000), a HMM based tagger for hindi POS Tag where they got an acccuracy of 94.21%.




## abner-wong/textrank: keyword extraction and summarization for Chinese text by TextRank
https://github.com/abner-wong/textrank

Based on the TextRank algorithm, the keyword extraction and summarization tasks of Chinese text are realized, and the core calculation code remains consistent with the paper.



## yongzhuo/Macropodus: 自然语言处理工具Macropodus，基于Albert+BiLSTM+CRF深度学习网络架构，中文分词，词性标注，命名实体识别，新词发现，关键词，文本摘要，文本相似度，科学计算器，中文数字阿拉伯数字(罗马数字)转换，中文繁简转换，拼音转换。tookit(tool) of NLP，CWS(chinese word segnment)，POS(Part-Of-Speech Tagging)，NER(name entity recognition)，Find(new words discovery)，Keyword(keyword extraction)，Summarize(text summarization)，Sim(text similarity)，Calculate(scientific calculator)，Chi2num(chinese number to arabic number)
https://github.com/yongzhuo/Macropodus

Macropodus is a natural language processing toolkit trained on large-scale Chinese corpus based on the Albert+BiLSTM+CRF network architecture. Common NLP functions such as Chinese word segmentation, part-of-speech tagging, named entity recognition, keyword extraction, text summarization, new word discovery, text similarity, calculator, number conversion, pinyin conversion, traditional and simplified conversion will be provided.



## kanjirz50/rake-ja: Rapid Automatic Keyword Extraction algorithm for Japanese
https://github.com/kanjirz50/rake-ja

Rapid Automatic Keyword Extraction algorithm for Japanese.

This module builds on rake-nltk.




## killa1218/CopyRNN-Keyword-Extraction
https://github.com/killa1218/CopyRNN-Keyword-Extraction

This is an implementation of Deep Keyphrase Generation based on CopyNet.

One training dataset (KP20k), five testing datasets (KP20k, Inspec, NUS, SemEval, Krapivin) and one pre-trained model are provided.

Note that the model is trained on scientific papers (abstract and keyword) in Computer Science domain, so it's expected to work well only for CS papers.





## CodePothunter/keywordExtract_zh: A Chinese key terminology extraction tool for MOOC.
https://github.com/CodePothunter/keywordExtract_zh

A Chinese key terminology extraction tool for MOOC. This tool needs to rely on the latest version of jieba for word segmentation. When using, put the entire folder in the working directory and call it as a toolkit. At present, in addition to supporting the extraction of key terms, it can also generate a summary of the lecture notes.



## pemagrg1/Nepali-POS-Tagging-and-Keyword-Extraction: Extract part of speech for Nepali words using TNT model. Created Date: 12 October 2018
https://github.com/pemagrg1/Nepali-POS-Tagging-and-Keyword-Extraction


Nepali is the language spoken by the people of Nepal. Nepali is actually written with the Devanagari alphabet and is an Indo-Aryan Language. The Devanagari script, which is generally known as Nagari, is written from left to right. The order of the letters made up of vowels and consonants is known as the "varnamala" which means the "garland of flowers." In the Unicode Conventional, the Devanagari is constituted in three blocks. U+0900–U+097F comprises the Devanagari, U+1CD0–U+1CFF comprises the Devanagari Extended, and U+A8E0–U+A8FF comprises the Vedic Extension. 

The paper, "Structure of Nepali Grammar" by Bal Krishna Bal has an awesome explanation on the grammar of Nepali [1] where he explains how each part of speech is used in Nepali. Asmita (Student of Bal Krishna Bal) has also done her degree project under the guidance of Bal Krishna Bal on "Part of Speech Tagger for Nepali Text using SVM" where she got an accuracy of 88% [2]. Tej Bahadur Shahi,Tank Nath Dhamala, and Bikash Balami also published a paper on "Support Vector Machines based Part of Speech Tagging for Nepali Text" where they got an accuracy of 90% on TNT and 90% on SVM, using 80000 training data size[3].

Nepali and Hindi are quite similar as they both follow the Devanagari script.







