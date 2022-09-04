# Processing PDFs - how to handle errors and corrections in batch and single mode

> Copied from the author's `bulktest` batch test runs' notes:

## THOUGHT:

After working an evening on some Metadata (XML) recovery code in `mupdf` core, I believe this is the WRONG TACTIC for this problem:

1) it takes a long time to get stuff done as the test/debug round trip is relatively slow, due to having to run the PDFs through the correct commands to trigger recovery, etc.
2) we limit ourselves to recovery code in C, using the xml parser (which fails) and the now-used gumbo html parser as a second attempt -- which turns out to be worthless as it expects HTML and nothing else, so it is pretty useless parsing semi-b0rked or fully-b0rked XMP records (XML).

Now we could persist and find ourselves the most lenient XML parser in the world and some very smart recovery codes, but here's a thought:

> Isn't this much more suitable to SCRIPTED processing? Where userland scripts can be created and used to fiddle to your heart's content?

**YES!**

Question is then: do we do this through an external process invoked through a callback, or via QuickJS and specifying user script to run or ...?

All those sound really nice, until you look at reality: https://jsonformatter.org/xml-formatter had no problem cleaning it up for us and that might not be batchable, but is surely very useful to any user anyway.

Which leads us to a meta-question: what can we do best to have the same tools suitable for batch and incident edits, including manual user operations like this copy&pasting to an external website?

--> while I was initially thinking in the direction of callbacks, i.e. staying in the current exec/run, now I believe the better and faster way forward is this:

allow the repair/recovery process to take arbitrary time and be executed at arbitrary times, hence:

- have the ability to at least detect these errors/issues in a batch or single run.
- use that signal (or do this as part of that signaling): ability to dump the raw data with sufficient markers so we can process it any way we want ...
- ... and then have a second tool (or batch run) import/replace those buggered bits with the corrected data provided by the external process (which might be anything, including manual actions such as copy/paste to/from websites, editors, ...)

Hence we must have tools with the ability to EXPORT these erroneous chunks ...
... and have the ability to IMPORT replacement data from files, preferably produced during previous EXPORT so users / external processes only need to bother with replacing the bad stuff for some good stuff.


^^^^^^ That means we can keep our tools relatively simple and don't need elaborate recovery schemes in the core tools themselves as everything is reduced to EXPORT + IMPORT/REPLACE (when available).

> Then the whole correction process becomes external activity that can be dealt with any way we like!


