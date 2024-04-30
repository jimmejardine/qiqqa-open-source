# Creating git patches from repo commits

Various ways, with different results:

## Method 1: numbered patches, one per commit

```
git format-patch --pretty=email --stat -m --first-parent  old..HEAD
```

gives you a nicely numbered set of patch files, one per commit, from commit `old` up to now. These files contain both the diff and the commit message in a reasonably easy to read format -- if you like processing patches anyhow.

> Of course, I `git checkout`-ed the branch I wanted to patch-dump before this, so HEAD points to the top of that one.


## Method 2: all them commits as individual patches, all bundled together in a single patch file

For the real do-it-by-mail heroes!

```
git log -p --pretty=email --stat -m --first-parent  original-trunk..HEAD > MSVC2022-projectfiles-upgraded.patch
```

produces a single patch file with lots of content: each patch is formatted very similar to method 1 above, so you still have each commit separated and identifiable, including the relevant commit message above that little patch chop. Makes for really large files, this one.

## Method 3: *flattened*, *single* patch for the whole kaboodle

If you're more of the "*do you feel lucky, punk?!*" Dirty Harry persuasion. 😎🥳

```
git diff -p --pretty=email --stat -m --first-parent  original-trunk..HEAD > MSVC2022-solution-flattened.patch
```

does away with all your beautiful (*koff koff*) commits and simply report a single overlord patch to rule them all. Ahhhh 😍

But BEWARE... applying patches was never my favorite job as it somehow doesn't seem to mesh very well with yours truly, hence there also...


# Applying patches and MAYBE not going crazy

While everyone and their granny tell you to run `git am` or simple `git apply`, this will ONLY work when there's NOTHING -- and I *mean* ABSOLUTELY NOTHING !!!1! -- sitting between that previous state at the remote and over at yours, i.e. you are simply applying patches to move forward without having to cope with any bits of development having happened out of sync between parties.

I don't know how Linus copes with this, but I have, time and again, wishes for a special shotgun, something small like say, the BFG9000, to help me "express my feelings" about this process, as, usually, I get treated to stuff like this:

```
(Stripping trailing CRs from patch; use --binary to disable.)
patching file device/lib/_mullonglong.c
Hunk #1 succeeded at 40 (offset 2 lines).
(Stripping trailing CRs from patch; use --binary to disable.)
patching file device/lib/_mullonglong.c
Hunk #2 FAILED at 30.
1 out of 2 hunks FAILED -- saving rejects to file device/lib/_mullonglong.c.rej
```

*Rrrrhaaghhh!!!*

or how about this:

```
$ git apply  /c/Users/Ger/Downloads/lib.patch
error: lib/_mullonglong.c: No such file or directory
error: lib/_muluchar.c: No such file or directory
```

which *persists* even when I hand-tweak the patch file.

And so on, and so on. Patches being rejected, not being applied, or only *very* partly, because there's some line somewhere that the patch tool decides is utter shite and untouchable.

The *only* command that worked for me, and even then, only 66% of the time, is to run a 3-way patch ALL THE TIME, no matter what they say or you believe "should be okay". Just run the 3-way fucker if you don't want to be *stark red hoppin' mad and aggravated* in 60 seconds flat.

```
git apply --3way /c/Users/Ger/Downloads/lib.patch
```



## The Moral Of This Story

Only do patches for other folks who ask for them; if you're me, DON'T even try. Okay, try the 3-way command and if that doesn't deliver pronto, flip a bird and find another way to merge/apply those changes. This is why I advocate git for world hegemony. To much of a guerilla for other world domination crap, but git should simply be made mandatory in every household. 

<sup>tip-toes to the kitchen cabinet to pop a few more happy pills as we're derailing across the madness horizon and coming out the other end...... *groovy*...</sup>




