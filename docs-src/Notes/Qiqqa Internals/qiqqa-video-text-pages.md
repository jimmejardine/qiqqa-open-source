We are going to use the Qiqqa Sniffer to grab some BibTeX record from Google Scholar for a PDF that's in our library.

As the PDF document *may* already have some BibTeX meta data attached, we will delete ("Clear") that first when we're in the Sniffer, so you can see Qiqqa picking up the new BibTeX, without the need for manual copy&paste.








Sometimes, the embedded browser will recognize that you're logged into Google and when your personal settings for Scholar include the "Import as BibTeX" option ticked, then you will see that extra item and you can use that until the Captchas etc. are going to be bothering you.

*This* is how you gain access to the BibTeX record in Scholar without the need for a Google account and being logged in, etc.: this method should always work. 

(At least it seems to work well at time of writing of this video: April 2020)




Also note that this problem is not just us: Google is tightening access lanes to their data.

See https://www.zdnet.com/article/google-bans-logins-from-embedded-browser-frameworks-to-prevent-mitm-phishing/
for some more info on this.







Qiqqa: XULRunner --> CEFSharp

As we will migrate from using old XULRunner as embedded web browser, we're going to use CEFSharp (which is based on Chrome). That one has the same trouble: you cannot log into your Google account from that one either, so no "Import as BibTeX" in Scholar unless the login cookies happen to work cross-browser: sometimes they do, often they don't. 
Hence the value of the new method: it always works.





By the way: note the Qiqqa list display update bug: we deleted the BibTeX metadata for both documents, but their entry in the list still shows that metadata until we refresh the list by toggling SORT.
