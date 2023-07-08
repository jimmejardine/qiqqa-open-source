# Additional tool: Document Hunter

The basic idea is this: drop any of these "references" into this tool and let it hunt the web for the PDF by itself:
- URL pointing at page for the PDF itself (just fetch it)
- URL pointing at reference / abstract page, where you can click a download or view link to get the PDF: Academia, NIH, etc.
- DOI: find the document and metadata references by searching for the DOI; get it from sci-hub.ru if nobody lese has it (to reduce strain on sci-hub.ru)
- Title (or part of title): search web using search engines (Bing or other that offers API; Google will be a bother here...): inspect the discovered pages and when they are a PDF or offer a PDF, grab it. (Note that for extra search *foo*, you can have "pdf" added to the title-as-a-search-query, just like I do manually in the browser a lot of the times!

The thought is to use cURL as the base and then code the tool like it's some kind of (dedicated) crawler, where the success/stop criterium is when you've found a PDF that seems to match sufficiently.

## Added bonus

When going through the web search engines, often you'll get multiple pages as "search result": visit *all* of them that look sufficiently relevant or until you've collected a maximum number of PDFs: it happens that various sites offer preprints, while others offer the published variant(s): some articles get published and restyled for multiple publications and these *editions* often include content edits, which we would be interested in too! So it behooves us to grab multiple "sibling" PDFs when it so happens we can.
