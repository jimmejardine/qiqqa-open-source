# PDF bulktest + mutool_ex PDF + URL tests: logbook notes


# Test run notes at the bleeding edge

This is about the multiple test runs covering the `evil-base` PDF corpus: I've been collecting these notes over the years. **Big Caveat: these notes were valid at the time of writing, but MAY be obsolete or even contradicting current behaviour at any later moment, sometimes even *seconds* away from the original event.**

This is about the things we observe when applying our tools at the bleeding edge of development to existing PDFs of all sorts, plus more or less wicked Internet URLs we checked out and the (grave) bugs that pop up most unexpectedly.

This is the lump sum notes (logbook) of these test runs' *odd observations*.

**The Table Of Contents / Overview Index is at [[../PDF bulktest test run notes at the bleeding edge]].**

-------------------------------

(The logbook was started quite a while ago, back in 2020.)

*Here goes -- lower is later ==> updates are appended at the bottom.*

-------------------------------



# Google Scholar anno 2020:


## Bad: CEFbrowser 403 for bibTeX Scholar requests

Does not work from CefBrowser (Embedded Chrome), as it produces HTTP ERROR 403 after about 1 or 2 correct BibTeX records having been obtained:

------------------------------------------

```
Request URL: https://scholar.googleusercontent.com/scholar.bib?q=info:aksg9HnvcEUJ:scholar.google.com/&output=citation&scisdr=CgUJ8X-2EJ7jxOLlya8:AAGBfm0AAAAAXg3g0a_sbyKtTZUN2F1HE0wgJje_mVp1&scisig=AAGBfm0AAAAAXg3g0YUrw5OqCnigPXrV9Yfvrco2KFr5&scisf=4&ct=citation&cd=0&hl=nl&scfhb=1
Request Method: GET
Status Code: 403 
Remote Address: 172.217.168.225:443
Referrer Policy: origin-when-cross-origin

alt-svc: quic=":443"; ma=2592000; v="46,43",h3-Q050=":443"; ma=2592000,h3-Q049=":443"; ma=2592000,h3-Q048=":443"; ma=2592000,h3-Q046=":443"; ma=2592000,h3-Q043=":443"; ma=2592000
cache-control: private
content-encoding: gzip
content-length: 1009
content-type: text/html; charset=UTF-8
date: Thu, 02 Jan 2020 12:29:47 GMT
server: scholar
status: 403
x-content-type-options: nosniff
x-frame-options: SAMEORIGIN
x-xss-protection: 0

:authority: scholar.googleusercontent.com
:method: GET
:path: /scholar.bib?q=info:aksg9HnvcEUJ:scholar.google.com/&output=citation&scisdr=CgUJ8X-2EJ7jxOLlya8:AAGBfm0AAAAAXg3g0a_sbyKtTZUN2F1HE0wgJje_mVp1&scisig=AAGBfm0AAAAAXg3g0YUrw5OqCnigPXrV9Yfvrco2KFr5&scisf=4&ct=citation&cd=0&hl=nl&scfhb=1
:scheme: https
accept: text/html,application/xhtml+xml,application/xml;q=0.9,image/webp,image/apng,*/*;q=0.8,application/signed-exchange;v=b3
accept-encoding: gzip, deflate, br
accept-language: en-US,en;q=0.9
referer: https://scholar.google.com/
sec-fetch-mode: navigate
sec-fetch-site: cross-site
sec-fetch-user: ?1
upgrade-insecure-requests: 1
user-agent: Mozilla/5.0 (Windows NT 10.0; WOW64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/78.0.3904.108 Safari/537.36

q: info:aksg9HnvcEUJ:scholar.google.com/
output: citation
scisdr: CgUJ8X-2EJ7jxOLlya8:AAGBfm0AAAAAXg3g0a_sbyKtTZUN2F1HE0wgJje_mVp1
scisig: AAGBfm0AAAAAXg3g0YUrw5OqCnigPXrV9Yfvrco2KFr5
scisf: 4
ct: citation
cd: 0
hl: nl
scfhb: 1
```

---

https://scholar.google.com/scholar?hl=nl&oe=ASCII&as_sdt=0%2C5&q=lcc+power+supply&btnG=

https://scholar.googleusercontent.com/scholar.bib?q=info:aksg9HnvcEUJ:scholar.google.com/&output=citation&scisdr=CgUJ8X-2EJ7jxOLlya8:AAGBfm0AAAAAXg3g0a_sbyKtTZUN2F1HE0wgJje_mVp1&scisig=AAGBfm0AAAAAXg3g0YUrw5OqCnigPXrV9Yfvrco2KFr5&scisf=4&ct=citation&cd=0&hl=nl&scfhb=1


```
Request URL: https://scholar.googleusercontent.com/scholar.bib?q=info:aksg9HnvcEUJ:scholar.google.com/&output=citation&scisdr=CgUJ8X-2EJ7jxOLlya8:AAGBfm0AAAAAXg3g0a_sbyKtTZUN2F1HE0wgJje_mVp1&scisig=AAGBfm0AAAAAXg3g0YUrw5OqCnigPXrV9Yfvrco2KFr5&scisf=4&ct=citation&cd=0&hl=nl&scfhb=1
Request Method: GET
Status Code: 403 
Remote Address: 172.217.168.225:443
Referrer Policy: origin-when-cross-origin


alt-svc: quic=":443"; ma=2592000; v="46,43",h3-Q050=":443"; ma=2592000,h3-Q049=":443"; ma=2592000,h3-Q048=":443"; ma=2592000,h3-Q046=":443"; ma=2592000,h3-Q043=":443"; ma=2592000
cache-control: private
content-encoding: gzip
content-length: 1009
content-type: text/html; charset=UTF-8
date: Thu, 02 Jan 2020 12:29:47 GMT
server: scholar
status: 403
x-content-type-options: nosniff
x-frame-options: SAMEORIGIN
x-xss-protection: 0


:authority: scholar.googleusercontent.com
:method: GET
:path: /scholar.bib?q=info:aksg9HnvcEUJ:scholar.google.com/&output=citation&scisdr=CgUJ8X-2EJ7jxOLlya8:AAGBfm0AAAAAXg3g0a_sbyKtTZUN2F1HE0wgJje_mVp1&scisig=AAGBfm0AAAAAXg3g0YUrw5OqCnigPXrV9Yfvrco2KFr5&scisf=4&ct=citation&cd=0&hl=nl&scfhb=1
:scheme: https
accept: text/html,application/xhtml+xml,application/xml;q=0.9,image/webp,image/apng,*/*;q=0.8,application/signed-exchange;v=b3
accept-encoding: gzip, deflate, br
accept-language: en-US,en;q=0.9
referer: https://scholar.google.com/
sec-fetch-mode: navigate
sec-fetch-site: cross-site
sec-fetch-user: ?1
upgrade-insecure-requests: 1
user-agent: Mozilla/5.0 (Windows NT 10.0; WOW64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/78.0.3904.108 Safari/537.36


q=info:aksg9HnvcEUJ:scholar.google.com/&output=citation&scisdr=CgUJ8X-2EJ7jxOLlya8:AAGBfm0AAAAAXg3g0a_sbyKtTZUN2F1HE0wgJje_mVp1&scisig=AAGBfm0AAAAAXg3g0YUrw5OqCnigPXrV9Yfvrco2KFr5&scisf=4&ct=citation&cd=0&hl=nl&scfhb=1
```








## Good: Google Chrome browser for bibTeX Scholar requests

Google Scholar works from Chrome OK:

------------------------------------------

```
Request URL: https://scholar.googleusercontent.com/scholar.bib?q=info:aksg9HnvcEUJ:scholar.google.com/&output=citation&scisdr=CgUJ8X-2EIDlrsvm4g8:AAGBfm0AAAAAXg3j-g93p_g6k6d8wax3gwpyVux0IVZg&scisig=AAGBfm0AAAAAXg3j-r-Bw0ynzeRyoxrnypG8FYEn0O29&scisf=4&ct=citation&cd=0&hl=nl&scfhb=1
Request Method: GET
Status Code: 200 
Remote Address: 127.0.0.1:8118
Referrer Policy: origin-when-cross-origin

alt-svc: quic=":443"; ma=2592000; v="46,43",h3-Q050=":443"; ma=2592000,h3-Q049=":443"; ma=2592000,h3-Q048=":443"; ma=2592000,h3-Q046=":443"; ma=2592000,h3-Q043=":443"; ma=2592000
cache-control: private, max-age=0
content-encoding: gzip
content-length: 288
content-type: text/plain; charset=UTF-8
date: Thu, 02 Jan 2020 12:27:52 GMT
expires: Thu, 02 Jan 2020 12:27:52 GMT
server: scholar
status: 200
x-content-type-options: nosniff
x-frame-options: SAMEORIGIN
x-xss-protection: 0

:authority: scholar.googleusercontent.com
:method: GET
:path: /scholar.bib?q=info:aksg9HnvcEUJ:scholar.google.com/&output=citation&scisdr=CgUJ8X-2EIDlrsvm4g8:AAGBfm0AAAAAXg3j-g93p_g6k6d8wax3gwpyVux0IVZg&scisig=AAGBfm0AAAAAXg3j-r-Bw0ynzeRyoxrnypG8FYEn0O29&scisf=4&ct=citation&cd=0&hl=nl&scfhb=1
:scheme: https
accept: text/html,application/xhtml+xml,application/xml;q=0.9,image/webp,image/apng,*/*;q=0.8,application/signed-exchange;v=b3;q=0.9
accept-encoding: gzip, deflate, br
accept-language: en-US,en;q=0.9,nl;q=0.8,de;q=0.7
cache-control: no-cache
pragma: no-cache
referer: https://scholar.google.com/
sec-fetch-mode: navigate
sec-fetch-site: cross-site
sec-fetch-user: ?1
upgrade-insecure-requests: 1
user-agent: Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/79.0.3945.88 Safari/537.36
x-client-data: CIi2yQEIpLbJAQjEtskBCKmdygEIyKvKAQi9sMoBCPe0ygEImbXKAQjstcoBGKukygEYjLLKAQ==

q: info:aksg9HnvcEUJ:scholar.google.com/
output: citation
scisdr: CgUJ8X-2EIDlrsvm4g8:AAGBfm0AAAAAXg3j-g93p_g6k6d8wax3gwpyVux0IVZg
scisig: AAGBfm0AAAAAXg3j-r-Bw0ynzeRyoxrnypG8FYEn0O29
scisf: 4
ct: citation
cd: 0
hl: nl
scfhb: 1
```

---

https://scholar.google.com/scholar?hl=nl&oe=ASCII&as_sdt=0%2C5&q=lcc+power+supply&btnG=

https://scholar.googleusercontent.com/scholar.bib?q=info:aksg9HnvcEUJ:scholar.google.com/&output=citation&scisdr=CgUJ8X-2EIDlrsvm4g8:AAGBfm0AAAAAXg3j-g93p_g6k6d8wax3gwpyVux0IVZg&scisig=AAGBfm0AAAAAXg3j-r-Bw0ynzeRyoxrnypG8FYEn0O29&scisf=4&ct=citation&cd=0&hl=nl&scfhb=1


```
Request URL: https://scholar.googleusercontent.com/scholar.bib?q=info:aksg9HnvcEUJ:scholar.google.com/&output=citation&scisdr=CgUJ8X-2EIDlrsvm4g8:AAGBfm0AAAAAXg3j-g93p_g6k6d8wax3gwpyVux0IVZg&scisig=AAGBfm0AAAAAXg3j-r-Bw0ynzeRyoxrnypG8FYEn0O29&scisf=4&ct=citation&cd=0&hl=nl&scfhb=1
Request Method: GET
Status Code: 200 
Remote Address: 127.0.0.1:8118
Referrer Policy: origin-when-cross-origin


alt-svc: quic=":443"; ma=2592000; v="46,43",h3-Q050=":443"; ma=2592000,h3-Q049=":443"; ma=2592000,h3-Q048=":443"; ma=2592000,h3-Q046=":443"; ma=2592000,h3-Q043=":443"; ma=2592000
cache-control: private, max-age=0
content-encoding: gzip
content-length: 288
content-type: text/plain; charset=UTF-8
date: Thu, 02 Jan 2020 12:27:52 GMT
expires: Thu, 02 Jan 2020 12:27:52 GMT
server: scholar
status: 200
x-content-type-options: nosniff
x-frame-options: SAMEORIGIN
x-xss-protection: 0


:authority: scholar.googleusercontent.com
:method: GET
:path: /scholar.bib?q=info:aksg9HnvcEUJ:scholar.google.com/&output=citation&scisdr=CgUJ8X-2EIDlrsvm4g8:AAGBfm0AAAAAXg3j-g93p_g6k6d8wax3gwpyVux0IVZg&scisig=AAGBfm0AAAAAXg3j-r-Bw0ynzeRyoxrnypG8FYEn0O29&scisf=4&ct=citation&cd=0&hl=nl&scfhb=1
:scheme: https
accept: text/html,application/xhtml+xml,application/xml;q=0.9,image/webp,image/apng,*/*;q=0.8,application/signed-exchange;v=b3;q=0.9
accept-encoding: gzip, deflate, br
accept-language: en-US,en;q=0.9,nl;q=0.8,de;q=0.7
cache-control: no-cache
pragma: no-cache
referer: https://scholar.google.com/
sec-fetch-mode: navigate
sec-fetch-site: cross-site
sec-fetch-user: ?1
upgrade-insecure-requests: 1
user-agent: Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/79.0.3945.88 Safari/537.36
x-client-data: CIi2yQEIpLbJAQjEtskBCKmdygEIyKvKAQi9sMoBCPe0ygEImbXKAQjstcoBGKukygEYjLLKAQ==


q=info:aksg9HnvcEUJ:scholar.google.com/&output=citation&scisdr=CgUJ8X-2EIDlrsvm4g8:AAGBfm0AAAAAXg3j-g93p_g6k6d8wax3gwpyVux0IVZg&scisig=AAGBfm0AAAAAXg3j-r-Bw0ynzeRyoxrnypG8FYEn0O29&scisf=4&ct=citation&cd=0&hl=nl&scfhb=1
```






## Good(?): CEFbrowser

Curiously enough, Google Scholar now does deliver at least about 12 BibTeX files 
without throwing out a 403 HTTP ERROR, after we tweaked CefSharp to send a custom UserAgent:

------------------------------------------

```
Request URL: https://scholar.googleusercontent.com/scholar.bib?q=info:Kxwr6hemMoEJ:scholar.google.com/&output=citation&scisdr=CgUJ8X-2EJ7jxOLsoYg:AAGBfm0AAAAAXg3puYje2b9Z6NuLK2JA3j67vrkE9cPl&scisig=AAGBfm0AAAAAXg3puRtnWb3vxUi7YuJRNzMqpapwRH9w&scisf=4&ct=citation&cd=2&hl=nl
Request Method: GET
Status Code: 200 
Remote Address: 172.217.168.225:443
Referrer Policy: unsafe-url

alt-svc: quic=":443"; ma=2592000; v="46,43",h3-Q050=":443"; ma=2592000,h3-Q049=":443"; ma=2592000,h3-Q048=":443"; ma=2592000,h3-Q046=":443"; ma=2592000,h3-Q043=":443"; ma=2592000
cache-control: private, max-age=0
content-encoding: gzip
content-length: 318
content-type: text/plain; charset=ISO-8859-1
date: Thu, 02 Jan 2020 12:52:29 GMT
expires: Thu, 02 Jan 2020 12:52:29 GMT
server: scholar
status: 200
x-content-type-options: nosniff
x-frame-options: SAMEORIGIN
x-xss-protection: 0

:authority: scholar.googleusercontent.com
:method: GET
:path: /scholar.bib?q=info:Kxwr6hemMoEJ:scholar.google.com/&output=citation&scisdr=CgUJ8X-2EJ7jxOLsoYg:AAGBfm0AAAAAXg3puYje2b9Z6NuLK2JA3j67vrkE9cPl&scisig=AAGBfm0AAAAAXg3puRtnWb3vxUi7YuJRNzMqpapwRH9w&scisf=4&ct=citation&cd=2&hl=nl
:scheme: https
accept: text/html,application/xhtml+xml,application/xml;q=0.9,image/webp,image/apng,*/*;q=0.8,application/signed-exchange;v=b3
accept-encoding: gzip, deflate, br
accept-language: en-US,en;q=0.9
referer: https://scholar.google.com/scholar?hl=nl&oe=ASCII&as_sdt=0%2C5&q=lcc+power+supply&btnG=
sec-fetch-mode: navigate
sec-fetch-site: cross-site
sec-fetch-user: ?1
upgrade-insecure-requests: 1
user-agent: CefSharp Browser76.1.90.0

q: info:Kxwr6hemMoEJ:scholar.google.com/
output: citation
scisdr: CgUJ8X-2EJ7jxOLsoYg:AAGBfm0AAAAAXg3puYje2b9Z6NuLK2JA3j67vrkE9cPl
scisig: AAGBfm0AAAAAXg3puRtnWb3vxUi7YuJRNzMqpapwRH9w
scisf: 4
ct: citation
cd: 2
hl: nl
```

---

https://scholar.google.com/scholar?hl=nl&oe=ASCII&as_sdt=0%2C5&q=lcc+power+supply&btnG=

https://scholar.googleusercontent.com/scholar.bib?q=info:Kxwr6hemMoEJ:scholar.google.com/&output=citation&scisdr=CgUJ8X-2EJ7jxOLsoYg:AAGBfm0AAAAAXg3puYje2b9Z6NuLK2JA3j67vrkE9cPl&scisig=AAGBfm0AAAAAXg3puRtnWb3vxUi7YuJRNzMqpapwRH9w&scisf=4&ct=citation&cd=2&hl=nl


```
Request URL: https://scholar.googleusercontent.com/scholar.bib?q=info:Kxwr6hemMoEJ:scholar.google.com/&output=citation&scisdr=CgUJ8X-2EJ7jxOLsoYg:AAGBfm0AAAAAXg3puYje2b9Z6NuLK2JA3j67vrkE9cPl&scisig=AAGBfm0AAAAAXg3puRtnWb3vxUi7YuJRNzMqpapwRH9w&scisf=4&ct=citation&cd=2&hl=nl
Request Method: GET
Status Code: 200 
Remote Address: 172.217.168.225:443
Referrer Policy: unsafe-url


alt-svc: quic=":443"; ma=2592000; v="46,43",h3-Q050=":443"; ma=2592000,h3-Q049=":443"; ma=2592000,h3-Q048=":443"; ma=2592000,h3-Q046=":443"; ma=2592000,h3-Q043=":443"; ma=2592000
cache-control: private, max-age=0
content-encoding: gzip
content-length: 318
content-type: text/plain; charset=ISO-8859-1
date: Thu, 02 Jan 2020 12:52:29 GMT
expires: Thu, 02 Jan 2020 12:52:29 GMT
server: scholar
status: 200
x-content-type-options: nosniff
x-frame-options: SAMEORIGIN
x-xss-protection: 0


:authority: scholar.googleusercontent.com
:method: GET
:path: /scholar.bib?q=info:Kxwr6hemMoEJ:scholar.google.com/&output=citation&scisdr=CgUJ8X-2EJ7jxOLsoYg:AAGBfm0AAAAAXg3puYje2b9Z6NuLK2JA3j67vrkE9cPl&scisig=AAGBfm0AAAAAXg3puRtnWb3vxUi7YuJRNzMqpapwRH9w&scisf=4&ct=citation&cd=2&hl=nl
:scheme: https
accept: text/html,application/xhtml+xml,application/xml;q=0.9,image/webp,image/apng,*/*;q=0.8,application/signed-exchange;v=b3
accept-encoding: gzip, deflate, br
accept-language: en-US,en;q=0.9
referer: https://scholar.google.com/scholar?hl=nl&oe=ASCII&as_sdt=0%2C5&q=lcc+power+supply&btnG=
sec-fetch-mode: navigate
sec-fetch-site: cross-site
sec-fetch-user: ?1
upgrade-insecure-requests: 1
user-agent: CefSharp Browser76.1.90.0


q=info:Kxwr6hemMoEJ:scholar.google.com/&output=citation&scisdr=CgUJ8X-2EJ7jxOLsoYg:AAGBfm0AAAAAXg3puYje2b9Z6NuLK2JA3j67vrkE9cPl&scisig=AAGBfm0AAAAAXg3puRtnWb3vxUi7YuJRNzMqpapwRH9w&scisf=4&ct=citation&cd=2&hl=nl
```






## Further notes:





Google Scholar works from Chrome OK:

```
Request URL: https://scholar.googleusercontent.com/scholar.bib?q=info:aksg9HnvcEUJ:scholar.google.com/&output=citation&scisdr=CgUJ8X-2EIDlrsvm4g8:AAGBfm0AAAAAXg3j-g93p_g6k6d8wax3gwpyVux0IVZg&scisig=AAGBfm0AAAAAXg3j-r-Bw0ynzeRyoxrnypG8FYEn0O29&scisf=4&ct=citation&cd=0&hl=nl&scfhb=1
Request Method: GET
Status Code: 200 
Remote Address: 127.0.0.1:8118
Referrer Policy: origin-when-cross-origin
alt-svc: quic=":443"; ma=2592000; v="46,43",h3-Q050=":443"; ma=2592000,h3-Q049=":443"; ma=2592000,h3-Q048=":443"; ma=2592000,h3-Q046=":443"; ma=2592000,h3-Q043=":443"; ma=2592000
cache-control: private, max-age=0
content-encoding: gzip
content-length: 288
content-type: text/plain; charset=UTF-8
date: Thu, 02 Jan 2020 12:27:52 GMT
expires: Thu, 02 Jan 2020 12:27:52 GMT
server: scholar
status: 200
x-content-type-options: nosniff
x-frame-options: SAMEORIGIN
x-xss-protection: 0
:authority: scholar.googleusercontent.com
:method: GET
:path: /scholar.bib?q=info:aksg9HnvcEUJ:scholar.google.com/&output=citation&scisdr=CgUJ8X-2EIDlrsvm4g8:AAGBfm0AAAAAXg3j-g93p_g6k6d8wax3gwpyVux0IVZg&scisig=AAGBfm0AAAAAXg3j-r-Bw0ynzeRyoxrnypG8FYEn0O29&scisf=4&ct=citation&cd=0&hl=nl&scfhb=1
:scheme: https
accept: text/html,application/xhtml+xml,application/xml;q=0.9,image/webp,image/apng,*/*;q=0.8,application/signed-exchange;v=b3;q=0.9
accept-encoding: gzip, deflate, br
accept-language: en-US,en;q=0.9,nl;q=0.8,de;q=0.7
cache-control: no-cache
pragma: no-cache
referer: https://scholar.google.com/
sec-fetch-mode: navigate
sec-fetch-site: cross-site
sec-fetch-user: ?1
upgrade-insecure-requests: 1
user-agent: Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/79.0.3945.88 Safari/537.36
x-client-data: CIi2yQEIpLbJAQjEtskBCKmdygEIyKvKAQi9sMoBCPe0ygEImbXKAQjstcoBGKukygEYjLLKAQ==
q: info:aksg9HnvcEUJ:scholar.google.com/
output: citation
scisdr: CgUJ8X-2EIDlrsvm4g8:AAGBfm0AAAAAXg3j-g93p_g6k6d8wax3gwpyVux0IVZg
scisig: AAGBfm0AAAAAXg3j-r-Bw0ynzeRyoxrnypG8FYEn0O29
scisf: 4
ct: citation
cd: 0
hl: nl
scfhb: 1
```


Does not work from CefBrowser (Embedded Chrome), as it produces HTTP ERROR 403 after about 1 or 2 correct BibTeX records having been obtained:

```
Request URL: https://scholar.googleusercontent.com/scholar.bib?q=info:aksg9HnvcEUJ:scholar.google.com/&output=citation&scisdr=CgUJ8X-2EJ7jxOLlya8:AAGBfm0AAAAAXg3g0a_sbyKtTZUN2F1HE0wgJje_mVp1&scisig=AAGBfm0AAAAAXg3g0YUrw5OqCnigPXrV9Yfvrco2KFr5&scisf=4&ct=citation&cd=0&hl=nl&scfhb=1
Request Method: GET
Status Code: 403 
Remote Address: 172.217.168.225:443
Referrer Policy: origin-when-cross-origin
alt-svc: quic=":443"; ma=2592000; v="46,43",h3-Q050=":443"; ma=2592000,h3-Q049=":443"; ma=2592000,h3-Q048=":443"; ma=2592000,h3-Q046=":443"; ma=2592000,h3-Q043=":443"; ma=2592000
cache-control: private
content-encoding: gzip
content-length: 1009
content-type: text/html; charset=UTF-8
date: Thu, 02 Jan 2020 12:29:47 GMT
server: scholar
status: 403
x-content-type-options: nosniff
x-frame-options: SAMEORIGIN
x-xss-protection: 0
:authority: scholar.googleusercontent.com
:method: GET
:path: /scholar.bib?q=info:aksg9HnvcEUJ:scholar.google.com/&output=citation&scisdr=CgUJ8X-2EJ7jxOLlya8:AAGBfm0AAAAAXg3g0a_sbyKtTZUN2F1HE0wgJje_mVp1&scisig=AAGBfm0AAAAAXg3g0YUrw5OqCnigPXrV9Yfvrco2KFr5&scisf=4&ct=citation&cd=0&hl=nl&scfhb=1
:scheme: https
accept: text/html,application/xhtml+xml,application/xml;q=0.9,image/webp,image/apng,*/*;q=0.8,application/signed-exchange;v=b3
accept-encoding: gzip, deflate, br
accept-language: en-US,en;q=0.9
referer: https://scholar.google.com/
sec-fetch-mode: navigate
sec-fetch-site: cross-site
sec-fetch-user: ?1
upgrade-insecure-requests: 1
user-agent: Mozilla/5.0 (Windows NT 10.0; WOW64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/78.0.3904.108 Safari/537.36
q: info:aksg9HnvcEUJ:scholar.google.com/
output: citation
scisdr: CgUJ8X-2EJ7jxOLlya8:AAGBfm0AAAAAXg3g0a_sbyKtTZUN2F1HE0wgJje_mVp1
scisig: AAGBfm0AAAAAXg3g0YUrw5OqCnigPXrV9Yfvrco2KFr5
scisf: 4
ct: citation
cd: 0
hl: nl
scfhb: 1
```


