# LINK : fatal error LNK1181: cannot open input file '`...\lib_nas_lockfile.lib`'

That one is, fortunately, easy. While it *is* a weird one, but I've seen this happen here regularly. Haven't found a fix with MSVC2022, but a simple re-execution of the previous build command fixes this issue -- my *hunch* is there's a race condition problem somewhere deep inside the MSVC parallel task execution engine -- i.e. their Visual Studio equivalent of 'make', so to speak.
 
The work-around:

Select the `__build_target_mupdf` project (or one of the others, if you were *Build*-ing one of those) and right-click, select the *Build* command from the pop-up menu *again*: this time around all link actions should complete successfully and you'ld have a set of executables to use/test (in the case of the `__build_target_mupdf` project line).



**(rewrite / re-organize into another item/document:)**

P.S.: may I suggest starting with `mutool.exe`, which is still pretty close to the Artifex/mupdf original? It is a 'monolithic build' of `mudraw` and many other Artifex/mupdf tools, plus `tesseract` (OCR). 

I mostly use `mutool_ex` myself (or `bulktest`, but that one is for running bulk test scripts on my large PDF corpus, to see if anything breaks particularly badly) -- `mutool_ex` is "*extended `mutool`*" and can be quite overwhelming at first as it contains a *lot* of test / demo / sample tools and utilities from the various libraries we use.[^1]

[^1]:  The `mutool_ex` is the original *monolithic build* when you read the git commit history and see mention of "*monolithic build mode*": `mutool_ex` serves, ideally, as a single entry point and dispatcher to all the tools / applications / tests we are interested in. Next to that, we also build all major tools and special tests as stand-alone executables as well.


Documentation of these tools is still a (large) TODO item, so you may have to check the source code files when you move outside the original Artifex/mupdf scope.

The way `mutool` + `mutool_ex` work is you specify the incorporated tool you wish to use and then add command line arguments for that tool as you like, e.g.

```
mutool mudraw -h

mutool draw -h
```

Note: the mupdf commands, such as `mudraw`, are recognized without their '`mu`' prefix, so '`draw`' == '`mudraw`' in `mutool`.

```
mutool tesseract -h
```

etc....
