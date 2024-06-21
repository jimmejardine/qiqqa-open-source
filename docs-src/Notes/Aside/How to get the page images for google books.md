# How to get the page images for google books

1. What DOES NOT work is saving the HTML page to disk: at best, you'll get 1 or 2 pages' worth of URLs for the page images as Chrome et al "conveniently" do not save HTML content generated in some *iframes* -- **TO BE INVESTIGATED** what is missing precisely and why.

2. What DOES NOT work is clicking the page image and right-clicking for the popup menu to "*save as...*": you'll get a popup menu alright, but not any "*save image...*" menu items in there! Bummer!

3. What DOES NOT work is believing you've a smart idea (I know I did 😅) and using the Chrome *head-less browser mode*: the bastards clearly coded it just so it does alter its own fingerprint, so you'll be treated to a HTTP 404 file-not-found error, e.g.

         /c/Program\ Files/Google/Chrome/Application/chrome.exe  --headless '--print-to-pdf=R:\ocr\sizes\dump.pdf'       https://books.google.nl/books/content?id=PQEIgK-OztQC&hl=nl&pg=PT3&img=1&zoom=3&sig=ACfU3U2rZzi2DYWhsIYPxEIT-cum5tc8PQ&w=1280

   while the *exact same URL* does deliver a page image in both that *very same* Chrome browser (just not in headless mode) and another mainline browser I use: Brave Browser. 

5. What DOES work is this process: open the google books page in the browser, open the developer panel (F12 key), go to NETWORK, then scroll through the book in the page: you'll see all sorts of network events occur while crossing each page (image) boundary.

Now, at the top of the developer panel, click the download button to "Export HAR...": this will download the network activity log as a JSON format file (called something like `site.har`); next you can `grep` the HAR file for the book page image URLs themselves:

```
grep 'https://books.google.nl/books/content' *.har
```

Each of these URL lines, pulled from from the JSON HAR file, has a URL that looks something like this:

      https://books.google.nl/books/content?id=PQEIgK-OztQC&hl=nl&pg=PT3&img=1&zoom=3&sig=ACfU3U2rZzi2DYWhsIYPxEIT-cum5tc8PQ&w=1280

which is the ALMOST direct link to the page image.

> **NOTE**: yes, that `&w=1280` argument can be modified and, depending on the scans they store in-house, will produce a larger image file, e.g. using `&w=2560` instead may f.e. produce an image that's ~ 1900px wide, because that's the largest they got. (Trying higher numbers, such as `&w=5120`, will produce the very same image file then.)


## Warning!

Fetching those content URLs using your go-to tool `curl` (or `wget`) won't work as google is a horribly fanatic **fingerprinter** and realizes you're not using *recent*(!) Chrome or similar and will barf a hairball *instantly* instead of complying with this bit of Smart Alec-ness.

Fetching the url with your browser does work: it produces a bit of HTML, within which is located the image we seek, and that one can be saved as-is. Allegedly (according to the suggestion of my browser) this is JFIF format.

🤔 I *think* that IFF we get one to work with any of our tools, then we might 85% along the way of making Google Scholar working for us again, as well... Ah well, *allemaal schurftige honden met tegenwerkende belangen*. 🤷 *La vie, c'est une tartine de merde et il faut que tu en manges une bouchée tous les jours.*



