From [Systemd: The Biggest Fallacies | Jude's Blog (judecnelson.blogspot.com)](http://judecnelson.blogspot.com/2014/09/systemd-biggest-fallacies.html), but also good to remember for our application/suite (as I've been considering binary log files as well 🤫 )

----

### Fallacy № 10ꓽ «Binary log files aren't a problem in practice»

This can be either faulty generalization or false induction, and is often made in the context of "I've used binary logging, therefore binary logging must be okay for everyone."  
  
The question we're interested in answering is "Are the log format and logging facility _always_ able to record the set of events and failures that the user requires, in the order in which they occur?"  If the answer to this question is "yes," then the choice of formatting is arbitrary, since the implementation details won't matter.  Otherwise, if there are cases where the logging facility can behave unexpectedly (such as a bug in the logger, a kernel panic, or hardware failure--things that happen somewhat regularly), the implementation details _do_ matter, since the user will need to manually intervene to determine the sequence of events and failures leading up to the bad behavior.  In this case, logging structured binary records is almost always a worse design choice than logging unstructured human-readable text, since it's almost always easier to recover a plain-text log from unrecoverable logging failures (particularly corruption).  
  
Log corruption is perhaps the hardest problem for a logging facility to address, since information is lost.  However, human-readable text remains at least partially readable when corrupted, since the rules, grammar, and semantics of written language serve as naturally-occurring error-correcting measures.  For example, a user can tell at a glance whether or not a string of text is truncated, or is missing one or more words, or contains data that shouldn't be there (i.e. non-printable characters), or doesn't fit any expected log message pattern, and so on, simply because logging only human-readable text lets the user deduce what the logger _intended_ to write.  However, the more binary data gets included with log records, the harder it becomes for the user to leverage written language to deduce intent (e.g. is a string of arbitrary bytes a sign of corruption, or is it part of the record, or is it a bit of both?).  
  
At the time of this writing, journald can corrupt the log simply by crashing, putting the log into a state where it is impossible to tell whether or not it was corrupted or tampered with (the developers have known about this for years, and [they don't think this is a bug](https://bugs.freedesktop.org/show_bug.cgi?id=64116)).   Now, it is possible to manually parse a corrupt journald log if journalctl can't help, but it requires more effort than parsing a corrupted plain-text log since the user must be aware of the journald log format in order to extract the human-readable strings from the binary records.  So, when it comes to dealing with failure modes that involve log corruption (in journald's case, these are all the failures that can lead to unclean shutdown--exactly the failures a logging facility should capture), a plain-text log will almost always be of more immediate use than journald's log.  
  
Now, journald's logging could be more robust.  It could improve resilience in the face of corruption by including [forward error correction codes](https://en.wikipedia.org/wiki/Forward_error_correction), by keeping its binary data in a separate file (or encoding it as human-readable text), and by implementing [ACID semantics](https://en.wikipedia.org/wiki/ACID) on log writes.  However, it does not do any of these things at the time of this writing, which makes it all the more fragile in the face of failure.

----

and another nice bit for extra *funzies* from [SystemD – it keeps getting worse | Musings from the Chiefio (wordpress.com)](https://chiefio.wordpress.com/2016/05/18/systemd-it-keeps-getting-worse/):

----

\[...\]
So one bit of over reaching bloatware that was trying to do things with the OS that it never ought to have done, needed a more “uniform” layer below it to take the pain and suffering out of their bad decision to be a giant Does-All and not be part of The Unix Way of modular code each doing one thing very well. No thanks. Just deep six Gnome and move on…

> “ I understand the natural urge to design something newer than sysvinit, but how about testing it a bit more? I have 5 different computers, and on any given random reboot, 1 out of 5 of these won’t boot. That’s a 20% failure rate. Its been a 20% failure rate for over 6 years now.
> 
> Exactly how much system testing is needed to push the failure rate to less than 1-out-of-5? Is it really that hard to test software before you ship it? Especially system software related to booting!? If systemd plans to take over the world, it should at least work, instead of failing.”
> 
> — Linas Vepstas,
> 
> “ I don’t actually have any particularly strong opinions on systemd itself. I’ve had issues with some of the core developers that I think are much too cavalier about bugs and compatibility, and **I think some of the design details are insane** (I dislike the binary logs, for example), but those are details, not big issues.”
> 
> **— Linus Torvalds,**

Nice when you find yourself out on a fringe edge, and next to you stands someone like Linus… Yes, Linus, I too saw some of it as a bit insane, and really detest binary logs you can only read with a special program…

----------

... and that nails the lingering issue I have with binary logging: the need to provide decoding/filtering tooling.

🤔 If we want something of a 'record format' *anyway*, we can either accept what we do already, which is prefix *error* and *warning* level messages with :

    ERROR: …
    WARNING: …

where `…` is the actual message. *info* / *debug* level messages don't have a fixed prefix at all, i.e. those are the "*anything else*" in the log file.

Of course, this breaks in subtle ways due to multi-line error messages thus possible incorrectly interpreted as *one* line of error report, followed by a slew of *info*-level logging.

So it MAY be useful to prefix *every* log message (which possibly contains *multiple lines of logging*) with a unique RECORD-START prefix, e.g. ASCII/Unicode *RecordSeparator* U+001E: `` -- or to make it more readable in arbitrary text editors, follow it with a colon: `:` 

> Another approach would be to employ Unicode/UTF8 and use one or more Unicode symbols to mark the start of each 'log record', e.g. 
> 
> - ⍾ BELL
> - ␁ SOH 
> - ␠ SPACE
> - ␊ LINEFEED
> - ␃ ETX = End Of Text
> - ␄ EOT = End Of Transmission (though we're more about the *start* of transmission here!)
> - ␞ RS = Record Separator (as a legible symbol this time!)
> - ⎆ ENTER
> - ࿄ Tibetan Dril Bu
> - 𝄞 CLEF
> - 𝅝 (Music) Whole Note
> - ␂ STX = Start Of Text
> 

Then the various logging levels would be prefixed thusly:

- fatal errors: `:FATAL: `
- errors: `:ERROR: `
- warnings: `:WARNING: `
- informationals (alt 1): `:INFO: `
- informationals (alt 2): ` ` (RS + SPACE)
- debug messages / anything else: ` ` (RS + SPACE)

or (for example)

- fatal errors: `⇢FATAL: `
- errors: `⇢ERROR: `
- warnings: `⇢WARNING: `
- informationals (alt 1): `⇢INFO: `
- informationals (alt 2): `⇢`
- debug messages / anything else: `⇢`

The key idea here is to use a Unicode character you won't expect to see in the log messages themselves -- or with *very low probability* at least.

