# curl :: commandline and notes

## Fetch a PDF from a website

Example:

```
--create-dirs --ftp-create-dirs --progress-bar --progress-meter --verbose --insecure --no-clobber --remote-name-all --remote-header-name https://forexuseful.com/files/guide/Moving_Average_Crossover_Strategy.pdf
```

The above URL has an invalid (expired) HTTPS certificate, which is dealt with using the `--insecure` flag.

The `--no-clobber` feature is borrowed from the `wget` tool and is a private addition to `curl`, so you'll need to run the GerHobbelt build to use the above.

`--no-clobber` will generate a new (*not yet existing*) filename from the base one by appending a 4 digit sequence number.



## Note: memory leaks (?)

Looked like we had some memory leaks in our `curl` run. Keep this in mind when constructing the localhost server software: it might be handy to fork/restart that server every once in a while in order to allow Windows / OS to collect and clean up the heap storage.

As we'll be working with reasonably complex software in that background server, we SHOULD reckon with memory leaks anyway, even if we happen to have a no-leakage situation at the time of testing...

--> how should we implement local *fail over* then? (Say we don't want any downtime, while we restart the server software: that would mean we would have at least *two* instances running at some point(s) in time and allow the using software (Qiqqa frontend) to address these instances in a fail-over manner, either through a proxy server (ZeroMQ!) or other means, so userland code will also be able to easily access these... Food for thought!)


## curl waits indefinitely for unreachable sites?

Example:

```
./bin/Debug-Unicode-32bit-x86/curl.exe --no-progress-bar  --insecure --no-clobber --remote-name-all --remote-header-name --output-dir ~/Downloads/pdfs                        https://178.128.16.139/VSeoN6_outlier-analysis_71aR.pdf         http://178.128.16.139/VSeoN671_outlier-analysis_aR.pdf 
```

See [Does curl have a timeout? - Unix & Linux Stack Exchange](https://unix.stackexchange.com/questions/94604/does-curl-have-a-timeout):

`--max-time` and `--connect-timeout` options should help here.



## Nasty PDFs

> **Note**: Also check these for more PDF download/fetching woes:
>
> - [[PDF cannot be Saved.As in browser (Microsoft Edge)]]
> - [[Testing - Nasty URLs for PDFs]]
> - [[Testing - PDF URLs with problems]]
> 
 


## Nasty URLs: JavaScript required for PDF download (Wiley.com)

Example:

```
[Appendix B: A Deeper Look at Moving Averages and the MACD - The Art and Science of Technical Analysis - Wiley Online Library](https://onlinelibrary.wiley.com/doi/pdf/10.1002/9781119202837.app2)
```

This link leads to an on-line viewer, where the PDF can be downloaded using the provided download button.

You'll need a full-fledged (*headless*?) web browser to grab this, curl isn't powerful enough.


## Nasty URLs: URLs that have moved

Example:

```
https://dl.acm.org/doi/pdf/10.5555/2946645.3053487 
```

which produces:

```
The URL has moved <a href="https://dl.acm.org/doi/pdf/10.5555/2946645.3053487?cookieSet=1">here</a>
```

which, when followed, gives you the run-around as `curl` doesn't do cookies:

```
The URL has moved <a href="https://dl.acm.org/action/cookieAbsent">here</a>
```

hence ACM checks for the presence of their desired cookies, before they're going to provide the PDF for you. Another case where a full browser may be needed to get at the PDF content.

### Does `curl --location` (follow redirects) help?

No.

It does cope with the cycle, though, so it's safe to use, but doesn't produce the requested PDF.



## Nasty URLs: your automated process has been *blocked* 

Example:

```
[PCR-6-222.pdf (nih.gov)](https://www.ncbi.nlm.nih.gov/pmc/articles/PMC4640017/pdf/PCR-6-222.pdf)
```

which produces a 17kByte '*PDF*' file, which, in fact is a HTML chunk saying this (*cleaned up*):

```
  <h1>Page not available</h1>
  <div class="main-exception-content">
    <p>
      <em>Your access to PubMed Central has been blocked because you are using an automated process to retrieve content from PMC, in violation of the terms of the PMC Copyright Notice.</em>
    </p>
    <div class="details">
      <div class="el-exception-reason">Reason: Automated retrieval by user agent "curl/7.81.0-DEV".</div>
   <div class="el-exception-url">URL: https://www.ncbi.nlm.nih.gov/pmc/articles/PMC4640017/pdf/PCR-6-222.pdf</div>
  </div>
  <p>Use of PMC is free, but must comply with the terms of the <a href="/pmc/about/copyright/">Copyright Notice</a> on the PMC site. For additional information, or to request that your IP address be unblocked, please send an email to PMC. For requests to be unblocked, you <em>must</em> include all of the information in the box above in your message.</p>
</div>
```

The remedy here is to provide a browser-like ID via this curl commandline option:

```
--user-agent "Mozilla/5.0 (Windows NT 10.0; Win64;x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/97.0.4692.20 Safari/537.36 Edg/97.0.1072.13"
```

Turns out they also accept a patched variant thereof:

```
--user-agent "Mozilla/5.0 (Windows NT 10.0; Win64;x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/97.0.4692.20 Safari/537.36 Edg/97.0.1072.13 Curl/7.81.0-DEV"
```

Also note the very useful PHP code(s) provided at [How to let Curl use same cookie as the browser from PHP - Stack Overflow](https://stackoverflow.com/questions/1121280/how-to-let-curl-use-same-cookie-as-the-browser-from-php)


## Curl improvements: automatic output file naming

Example:

```
https://upcommons.upc.edu/bitstream/handle/2117/79915/Design+and+development+of+an+app+for+statistical+data+analysis+learning.pdf?sequence=1
```

will produce a filename like:

```
Design+and+development+of+an+app+for+statistical+data+analysis+learning.pdf_sequence=1
```

which is a bit nasty as the mimetype clearly is `application/pdf` so it would be *very handy* here to make sure the filename extension is then always set to `.pdf`.

### Additional grievance

It would also help a lot if the URL that produced the PDF is kept *with the PDF*. Either we use Windows-specific ADS (*Data Streams*: a 'Zone 3' URL)[^1] or save [an `.opf` metadata file](http://idpf.org/epub/20/spec/OPF_2.0_latest.htm) alongside, where we keep the URL. (`.opf` is used by [calibre](https://calibre-ebook.com/) et al to keep ebook metadata, so we might want to jump on that bandwagon, but this really could be anything.)





[^1]: trouble with Windows ADS is two-fold:
   1. it's non-portable. It may serve as a metadata *source* where we can extract the download URL for a given document file when there's Zone 3 metadata stored in its ADS, but that's about the size of it when it comes to *usefulness*.
   2. Many tools don't copy the ADS info with the file when it's moved, e.g. when using `robocopy` or other Windows commandline tools. It also looks like third-party file managers (e.g. the nice XYplorer) generally don't care much about ADS either, so the metadata attached that way is easily lost. Same goes for files which are bundled into a ZIP or other archive file and then extracted: the ADS info will be lost.


