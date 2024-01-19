# Searching PDFs on archive.org anno 2023,2024 after the bloody publishers' lawsuit(s)

Use advanced search, which should produce something like this, e.g. when you search for publications mentioning 'algorithm' anywhere in their metadata:

```
(algorithm) AND mediatype:(texts) AND format:(PDF) AND -access-restricted-item:(true)
```

## Important bits, No.1:

```
AND mediatype:(texts)
```

capital AND and mediatype restriction so we don't get swamped by videos and other undesirable cruft.

## Important bits, No.2

```
AND format:(PDF)
```

which filters out any entry that doesn't have any form of PDF available. 

Of course one could also search for EPUB, etc., but PDF is what we're digging for at this archaeological site.


## Important bits, No.3

```
AND -access-restricted-item:(true)
```

very important filter or you'll spend ages going through the locked-off stuff, bugger it Milenninimum Hand and Shrimp, Buggerit, Buggerit.

Ow, and the `AND NOT <cond>` (written as `AND -<cond>`) is the magic touch you're looking for here because the obvious one:

```
AND access-restricted-item:(false)
```

delivers Jack Shit whatever you try: while it may seem a binary/boolean field, archive.org apparently treat it as either `true` or some sort of `undefined`. Sigh.

Also note that the tag is all-lowercase; *if you don't, it won't*. 😜 (`AND -Access-restricted-item:(true)` doesn't deliver, 's what I mean...)


----------

Use the above refinements to save yourself a heck of a lot of time when you trawl archive.org, otherwise I've found the site to have become horribly unusable.

