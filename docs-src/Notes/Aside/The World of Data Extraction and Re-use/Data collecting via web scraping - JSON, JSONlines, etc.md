# Data collecting via web scraping - JSON, JSONlines, etc

For example note the remark near the end of https://github.com/BBC-Data-Unit/music-festivals/blob/master/using_spotify_api.Rmd where the BBC collected data by way of scraping the Spotify API:

> Problems? If the process above doesn't work it may be that the data is actually in the ['JSON Lines' format](http://jsonlines.org/). [Try the solutions outlined here](https://stackoverflow.com/questions/24514284/how-do-i-import-data-from-json-format-into-r-using-jsonlite-package). \[....\]

What I've seen in other places as well leads to the general process / assumption:

1. DO NOT assume your `curl` scrape/fetch will deliver the data in a clean format

   1. nor will it always be in the format you *initially expected* (websites change; mistakes & errors abound -- you SHOULD reckon with rerunning your scrape at a later date and too strict / too inflexible expectations (assumptions) will break your beautiful scraper very easily.)
   
2. it MAY behoof you to use a flexible input parser, which attempts to parse the whole incoming caboodle as any of the following, and in this order unless otherwise specified:
   
   1. JSON / JSON5 (JSON with JS/C/C++ comments)
   2. JSONlines (one JSON blurb per text line, with just `\n` LF as a separator)
   3. CSV / TSV
   4. XML
   5. HTML table (here we MAY expect a lot of surrounding cruft as we're pulling data from inside a larger HTML web page, possibly?)
   
3. have a way to detect and register data gaps; 404/403/500 HTTP request error response codes SHOULD NOT pass by unnoticed.
4. possibly track which data bit was obtained from where, i.e. which source URL? -- so we can more easily *detectorate* which source went wrong / introduces flaky data / gaps / notable outliers, without having to resort to trundling through our scraper's error logs.


## 🤔 Tooling up?

🤔 Might be worthwhile to encode the above in a separate scraper / scraper-assist tool.

