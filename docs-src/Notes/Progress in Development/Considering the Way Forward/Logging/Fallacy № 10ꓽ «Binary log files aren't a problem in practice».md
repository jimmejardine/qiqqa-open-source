From [Systemd: The Biggest Fallacies  |  Jude's Blog (judecnelson.blogspot.com)](http://judecnelson.blogspot.com/2014/09/systemd-biggest-fallacies.html), but also good to remember for our application/suite (as I've been considering binary log files as well 🤫)

(**Edit**: note the [[#^797a6c]](Some problems with text log file systems that binary logging systems *potentially solve*) section further below: as I was on the fence (*binary log or not to binary log*) for quite a while, this is the conclusion I ended up with: iff I'm willing to spend the extra effort on the reader/tools, then it's a decent choice and *not insane* for an application, where telemetrics data gathering is a major part of the logging architecture goal.)

----

### Fallacy № 10ꓽ «Binary log files aren't a problem in practice»

This can be either faulty generalization or false induction, and is often made in the context of "I've used binary logging, therefore binary logging must be okay for everyone."  
  
The question we're interested in answering is "Are the log format and logging facility _always_ able to record the set of events and failures that the user requires, in the order in which they occur?"  If the answer to this question is "yes," then the choice of formatting is arbitrary, since the implementation details won't matter.  Otherwise, if there are cases where the logging facility can behave unexpectedly (such as a bug in the logger, a kernel panic, or hardware failure--things that happen somewhat regularly), the implementation details _do_ matter, since the user will need to manually intervene to determine the sequence of events and failures leading up to the bad behavior.  In this case, logging structured binary records is almost always a worse design choice than logging unstructured human-readable text, since it's almost always easier to recover a plain-text log from unrecoverable logging failures (particularly corruption).  
  
Log corruption is perhaps the hardest problem for a logging facility to address, since information is lost.  However, human-readable text remains at least partially readable when corrupted, since the rules, grammar, and semantics of written language serve as naturally-occurring error-correcting measures.  For example, a user can tell at a glance whether or not a string of text is truncated, or is missing one or more words, or contains data that shouldn't be there (i.e. non-printable characters), or doesn't fit any expected log message pattern, and so on, simply because logging only human-readable text lets the user deduce what the logger _intended_ to write.  However, the more binary data gets included with log records, the harder it becomes for the user to leverage written language to deduce intent (e.g. is a string of arbitrary bytes a sign of corruption, or is it part of the record, or is it a bit of both?).  
  
At the time of this writing, `journald` can corrupt the log simply by crashing, putting the log into a state where it is impossible to tell whether or not it was corrupted or tampered with (the developers have known about this for years, and [they don't think this is a bug](https://bugs.freedesktop.org/show_bug.cgi?id=64116)).   Now, it is possible to manually parse a corrupt `journald` log if `journalctl` can't help, but it requires more effort than parsing a corrupted plain-text log since the user must be aware of the `journald` log format in order to extract the human-readable strings from the binary records.  So, when it comes to dealing with failure modes that involve log corruption (in `journald`'s case, these are all the failures that can lead to unclean shutdown--exactly the failures a logging facility should capture), a plain-text log will almost always be of more immediate use than `journald`'s log.  
  
Now, `journald`'s logging could be more robust.  It could improve resilience in the face of corruption by including [forward error correction codes](https://en.wikipedia.org/wiki/Forward_error_correction), by keeping its binary data in a separate file (or encoding it as human-readable text), and by implementing [ACID semantics](https://en.wikipedia.org/wiki/ACID) on log writes.  However, it does not do any of these things at the time of this writing, which makes it all the more fragile in the face of failure.

  

#### Fallacy #10.1: "`journald`'s binary format lets its perform authenticity and integrity checks"

This is propaganda. Most modern `syslog` implementations do this too, so this doesn't compel `journald`'s adoption.  
  

#### Fallacy #10.2: "`journald` binary format lets it have an index that makes log access faster"

This is also propaganda. While no one will contest that indexing will speed up log access (at least in theory), there's also no technical reason why `journald` can't keep the index separate from the logfile, or can't write the index into the logfile as human-readable text.  
  

#### Fallacy #10.3: "All the problems you have with `journald` can be avoided by having it simply pass everything to `syslog`"

This assumes a faulty premise that there is a need for `journald` at all in this case. If all the user needs is `syslog`, then why require `journald` at all?

----

and another nice bit for extra *funzies* from [SystemD – it keeps getting worse  |  Musings from the Chiefio (wordpress.com)](https://chiefio.wordpress.com/2016/05/18/systemd-it-keeps-getting-worse/):

----

\[...\]
So one bit of over reaching bloatware that was trying to do things with the OS that it never ought to have done, needed a more “uniform” layer below it to take the pain and suffering out of their bad decision to be a giant Does-All and not be part of The Unix Way of modular code each doing one thing very well. No thanks. Just deep six Gnome and move on…

> “ I understand the natural urge to design something newer than `sysvinit`, but how about testing it a bit more? I have 5 different computers, and on any given random reboot, 1 out of 5 of these won’t boot. That’s a 20% failure rate. Its been a 20% failure rate for over 6 years now.
> 
> Exactly how much system testing is needed to push the failure rate to less than 1-out-of-5? Is it really that hard to test software before you ship it? Especially system software related to booting!? If `systemd` plans to take over the world, it should at least work, instead of failing.”
> 
> — Linas Vepstas,
> 
> “ I don’t actually have any particularly strong opinions on `systemd` itself. I’ve had issues with some of the core developers that I think are much too cavalier about bugs and compatibility, and **I think some of the design details are insane** (I dislike the binary logs, for example), but those are details, not big issues.”
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

> Another approach would be to employ Unicode/UTF8 and use one or more Unicode symbols to mark the start of each 'log record' as an alternative to direct use of [C0 and C1 control codes in the log text](https://en.wikipedia.org/wiki/C0_and_C1_control_codes), e.g. 
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


-------------


From Wikipedia:  

[ASCII control codes](https://en.wikipedia.org/wiki/ASCII#Control_characters "ASCII"), originally defined in [ANSI X3.4](https://en.wikipedia.org/wiki/ANSI_X3.4 "ANSI X3.4").[^4]

| [Caret notation](https://en.wikipedia.org/wiki/Caret_notation "Caret notation") | Decimal | Hexadecimal | Abbreviations     | [Control Pictures](https://en.wikipedia.org/wiki/Control_Pictures "Control Pictures") | Name                                                                                                                                 | [C escape](https://en.wikipedia.org/wiki/Escape_Sequences_in_C "Escape Sequences in C") | Description                                                                                                                                                                                                                                                                                                                                                    |
| ------------------------------------------------------------------------------- | ------- | ----------- | ----------------- | ------------------------------------------------------------------------------------- | ------------------------------------------------------------------------------------------------------------------------------------ | --------------------------------------------------------------------------------------- | -------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------- |
| ^@                                                                              | 0       | 00          | NUL               | ␀                                                                                     | [Null](https://en.wikipedia.org/wiki/Null_character "Null character")                                                                | \0                                                                                      | Does nothing. The code of blank paper tape, and also used for padding to slow transmission.                                                                                                                                                                                                                                                                    |
| \^A                                                                             | 1       | 01          | TC1, SOH          | ␁                                                                                     | Start of Heading                                                                                                                     |                                                                                         | First character of the heading of a message.                                                                                                                                                                                                                                                                                                                   |
| \^B                                                                             | 2       | 02          | TC2, STX          | ␂                                                                                     | Start of Text                                                                                                                        |                                                                                         | Terminates the header and starts the message text.                                                                                                                                                                                                                                                                                                             |
| \^C                                                                             | 3       | 03          | TC3, ETX          | ␃                                                                                     | [End of Text](https://en.wikipedia.org/wiki/End-of-Text_character "End-of-Text character")                                           |                                                                                         | Ends the message text, starts a footer (up to the next TC character).                                                                                                                                                                                                                                                                                          |
| \^D                                                                             | 4       | 04          | TC4, EOT          | ␄                                                                                     | [End of Transmission](https://en.wikipedia.org/wiki/End-of-Transmission_character "End-of-Transmission character")                   |                                                                                         | Ends the transmission of one or more messages. May place terminals on standby.                                                                                                                                                                                                                                                                                 |
| \^E                                                                             | 5       | 05          | TC5, ENQ, WRU[^1] | ␅                                                                                     | [Enquiry](https://en.wikipedia.org/wiki/Enquiry_character "Enquiry character")                                                       |                                                                                         | Trigger a response at the receiving end, to see if it is still present.                                                                                                                                                                                                                                                                                        |
| \^F                                                                             | 6       | 06          | TC6, ACK          | ␆                                                                                     | [Acknowledge](https://en.wikipedia.org/wiki/Acknowledge_character "Acknowledge character")                                           |                                                                                         | Indication of successful receipt of a message.                                                                                                                                                                                                                                                                                                                 |
| \^G                                                                             | 7       | 07          | BEL[^2]           | ␇                                                                                     | [Bell](https://en.wikipedia.org/wiki/Bell_character "Bell character"), Alert                                                         | \a                                                                                      | Call for attention from an operator.                                                                                                                                                                                                                                                                                                                           |
| \^H                                                                             | 8       | 08          | FE0, BS           | ␈                                                                                     | [Backspace](https://en.wikipedia.org/wiki/Backspace "Backspace")                                                                     | \b                                                                                      | Move one position leftwards. Next character may overprint or replace the character that was there.                                                                                                                                                                                                                                                             |
| \^I                                                                             | 9       | 09          | FE1, HT           | ␉                                                                                     | Character Tabulation,  <br>[Horizontal Tabulation](https://en.wikipedia.org/wiki/Horizontal_Tab "Horizontal Tab")                    | \t                                                                                      | Move right to the next [tab stop](https://en.wikipedia.org/wiki/Tab_stop "Tab stop").                                                                                                                                                                                                                                                                          |
| \^J                                                                             | 10      | 0A          | FE2, LF           | ␊                                                                                     | [Line Feed](https://en.wikipedia.org/wiki/Line_feed "Line feed")                                                                     | \n                                                                                      | Move down to the same position on the next line (some devices also moved to the left column).                                                                                                                                                                                                                                                                  |
| \^K                                                                             | 11      | 0B          | FE3, VT           | ␋                                                                                     | Line Tabulation,  <br>[Vertical Tabulation](https://en.wikipedia.org/wiki/Vertical_Tab "Vertical Tab")                               | \v                                                                                      | Move down to the next vertical tab stop.                                                                                                                                                                                                                                                                                                                       |
| \^L                                                                             | 12      | 0C          | FE4, FF           | ␌                                                                                     | [Form Feed](https://en.wikipedia.org/wiki/Form_feed "Form feed")                                                                     | \f                                                                                      | Move down to the top of the next page.                                                                                                                                                                                                                                                                                                                         |
| \^M                                                                             | 13      | 0D          | FE5, CR           | ␍                                                                                     | [Carriage Return](https://en.wikipedia.org/wiki/Carriage_return "Carriage return")                                                   | \r                                                                                      | Move to column zero while staying on the same line.                                                                                                                                                                                                                                                                                                            |
| \^N                                                                             | 14      | 0E          | SO, LS1[^3]       | ␎                                                                                     | [Shift Out](https://en.wikipedia.org/wiki/Shift_Out_and_Shift_In_characters "Shift Out and Shift In characters")                     |                                                                                         | Switch to an alternative character set.                                                                                                                                                                                                                                                                                                                        |
| \^O                                                                             | 15      | 0F          | SI, LS0[^3]       | ␏                                                                                     | [Shift In](https://en.wikipedia.org/wiki/Shift_Out_and_Shift_In_characters "Shift Out and Shift In characters")                      |                                                                                         | Return to regular character set after SO.                                                                                                                                                                                                                                                                                                                      |
| \^P                                                                             | 16      | 10          | TC7, DC0, DLE[^4] | ␐                                                                                     | Data Link Escape                                                                                                                     |                                                                                         | Cause a limited number of contiguously following characters to be interpreted in some different way.                                                                                                                                                                                                                                                           |
| \^Q                                                                             | 17      | 11          | DC1, XON          | ␑                                                                                     | Device Control One                                                                                                                   |                                                                                         | Turn on (DC1 and DC2) or off (DC3 and DC4) devices.<br><br>Teletype used these for the paper tape reader and the paper tape punch. The first use became the [de facto standard](https://en.wikipedia.org/wiki/De_facto_standard "De facto standard") for [software flow control](https://en.wikipedia.org/wiki/Software_flow_control "Software flow control"). |
| \^R                                                                             | 18      | 12          | DC2, TAPE         | ␒                                                                                     | Device Control Two                                                                                                                   |                                                                                         |                                                                                                                                                                                                                                                                                                                                                                |
| \^S                                                                             | 19      | 13          | DC3, XOFF         | ␓                                                                                     | Device Control Three                                                                                                                 |                                                                                         |                                                                                                                                                                                                                                                                                                                                                                |
| \^T                                                                             | 20      | 14          | DC4, ~~TAPE~~     | ␔                                                                                     | Device Control Four                                                                                                                  |                                                                                         |                                                                                                                                                                                                                                                                                                                                                                |
| \^U                                                                             | 21      | 15          | TC8, NAK          | ␕                                                                                     | [Negative Acknowledge](https://en.wikipedia.org/wiki/Negative-acknowledge_character "Negative-acknowledge character")                |                                                                                         | Negative response to a sender, such as a detected error.                                                                                                                                                                                                                                                                                                       |
| \^V                                                                             | 22      | 16          | TC9, SYN          | ␖                                                                                     | Synchronous Idle                                                                                                                     |                                                                                         | Sent in synchronous transmission systems when no other character is being transmitted.                                                                                                                                                                                                                                                                         |
| \^W                                                                             | 23      | 17          | TC10, ETB         | ␗                                                                                     | [End of Transmission Block](https://en.wikipedia.org/wiki/End-of-Transmission-Block_character "End-of-Transmission-Block character") |                                                                                         | End of a transmission block of data when data are divided into such blocks for transmission purposes.                                                                                                                                                                                                                                                          |
| \^X                                                                             | 24      | 18          | CAN               | ␘                                                                                     | [Cancel](https://en.wikipedia.org/wiki/Cancel_character "Cancel character")                                                          |                                                                                         | Indicates that the data preceding it are in error or are to be disregarded.                                                                                                                                                                                                                                                                                    |
| \^Y                                                                             | 25      | 19          | EM                | ␙                                                                                     | End of medium                                                                                                                        |                                                                                         | Indicates on paper or magnetic tapes that the end of the usable portion of the tape had been reached.                                                                                                                                                                                                                                                          |
| \^Z                                                                             | 26      | 1A          | SUB               | ␚                                                                                     | [Substitute](https://en.wikipedia.org/wiki/Substitute_character "Substitute character")                                              |                                                                                         | Replaces a character that was found to be [invalid or in error](https://en.wikipedia.org/wiki/Error_detection_and_correction "Error detection and correction"). Should be ignored.                                                                                                                                                                             |
| \^\[                                                                            | 27      | 1B          | ESC[^5]           | ␛                                                                                     | [Escape](https://en.wikipedia.org/wiki/Escape_character "Escape character")                                                          | \e                                                                                      | Alters the meaning of a limited number of following bytes.  <br>Nowadays this is almost always used to introduce an [ANSI escape sequence](https://en.wikipedia.org/wiki/ANSI_escape_sequence "ANSI escape sequence").                                                                                                                                         |
| \^\                                                                             | 28      | 1C          | IS4, FS           | ␜                                                                                     | File Separator                                                                                                                       |                                                                                         | Can be used as [delimiters](https://en.wikipedia.org/wiki/Delimiter "Delimiter") to mark fields of data structures. US is the lowest level, while RS, GS, and FS are of increasing level to divide groups made up of items of the level beneath it. SP (space) could be considered an even lower level.                                                        |
| \^]                                                                             | 29      | 1D          | IS3, GS           | ␝                                                                                     | Group Separator                                                                                                                      |                                                                                         |                                                                                                                                                                                                                                                                                                                                                                |
| \^\^                                                                            | 30      | 1E          | IS2, RS           | ␞                                                                                     | Record Separator                                                                                                                     |                                                                                         |                                                                                                                                                                                                                                                                                                                                                                |
| \^_                                                                             | 31      | 1F          | IS1, US           | ␟                                                                                     | Unit Separator                                                                                                                       |                                                                                         |                                                                                                                                                                                                                                                                                                                                                                |

While not technically part of the C0 control character range, the following two characters can be thought of as having some characteristics of control characters.

| [Caret notation](https://en.wikipedia.org/wiki/Caret_notation "Caret notation") | Decimal | Hexadecimal | Abbreviations | [Control Pictures](https://en.wikipedia.org/wiki/Control_Pictures "Control Pictures") | Name                                                                               | [C escape](https://en.wikipedia.org/wiki/Escape_Sequences_in_C "Escape Sequences in C") | Description                                                                                 |     |
| ------------------------------------------------------------------------------- | ------- | ----------- | ------------- | ------------------------------------------------------------------------------------- | ---------------------------------------------------------------------------------- | --------------------------------------------------------------------------------------- | ------------------------------------------------------------------------------------------- | --- |
|                                                                                 | 32      | 20          | SP            | ␠                                                                                     | [Space](https://en.wikipedia.org/wiki/Space_\(punctuation\) "Space (punctuation)") |                                                                                         | Move right one character position.                                                          |     |
| \^?                                                                             | 127     | 7F          | DEL           | ␡                                                                                     | [Delete](https://en.wikipedia.org/wiki/Delete_character "Delete character")        |                                                                                         | Should be ignored. Used to delete characters on punched tape by punching out all the holes. |     |


[^1]: Teletype labelled the key WRU for 'who are you?'

[^2]: The name BELL is assigned by Unicode to the unrelated emoji character 🔔 (U+1F514). While C0 and C1 control characters were not formally named by the Unicode standard itself at the time, this collided with existing use of BELL as the name of this control character in software following the previous versions of UTS#18 (the Unicode Regular Expressions standard), e.g. in [Perl](https://en.wikipedia.org/wiki/Perl "Perl"). Unicode now accepts ALERT and BEL (but not BELL) as formal aliases for the control character, although the code chart still lists BELL as the ISO 6429 alias, and the corresponding [control picture code point](https://en.wikipedia.org/wiki/Control_Pictures "Control Pictures") is called SYMBOL FOR BELL. Perl subsequently switched to using BELL for the emoji in version 5.18.

[^3]: [ISO/IEC 2022](https://en.wikipedia.org/wiki/ISO/IEC_2022 "ISO/IEC 2022") (ECMA-35) refers to these as LS0 and LS1 in 8-bit environments, and as SI and SO in 7-bit environments.

[^4]: The first, 1963 edition of ASCII classified [DLE](https://en.wikipedia.org/wiki/C0_and_C1_control_codes#DLE) as a device control, rather than a transmission control, and gave it the abbreviation DC0 ("device control reserved for data link escape").

[^5]: The '\e' escape sequence is not part of ISO C and many other language specifications. However, it is understood by several compilers, including [GCC](https://en.wikipedia.org/wiki/GNU_Compiler_Collection "GNU Compiler Collection").



## Some problems with text log file systems that binary logging systems *potentially solve*:

^797a6c

So binary logging systems carry the smell of an *anti-pattern*, once you've read the sections above?
Yes, you need separate tooling to `grep` through a binary log file (or *database*) and often these are proprietary or otherwise limited access/availability -- heck, you need you build them from source if they're not readily available on your system!

Then there's the *many eyes* issues involving performance and ease-of-use: as these invariably are bespoke and thus *localized* solutions (include `journald`: very specific for a bunch of UNIXes), so one may fear there's more bugs lingering in these systems, plus doubt about their optimal performance as fewer people have been involved with their development and/or use, so the pressure to get the very best speed and usability is lower than for those ubiquitous text-line based logfile systems.

All valid points, but text-based logfile systems have their own set of bothersome points:

1. high costs in:

   + (in-app) writing (string/line formatting via `sprintf()` et al), 
   + transmission (sending data as text takes more bytes than sending in binary form),
   + storage (text takes up more space on your HDDs per unit of information = the state and data values your application wanted to report)
   
2. a plethora of formats and more often than not no easily discernable delineation of parameter values to help parsing/querying/summarizing the logged information, at least not beyond the point of the *severity level* of the text line itself: it's often very hard to extract data values from logged text lines in any dependable fashion.

If you intend to *analyze* your logfiles afterwards, then a binary log format can provide clear, easy-to-use and performant methods to drill down into the log lines themselves and gather the data you seek by using the query machine provided to *read* those binary log files.

Ok. How about providing this feature (No. 2 from the list above) in a text-line log format? For example by:

1. using the same Unicode symbol trick from above to delineate the variable (name, value) fields, so these can be unambiguously decoded by machine.
2. appending a machine-readable (f.e. JSONL?) copy of all logged key/value pairs at the end of each text log line: that way the human can read the lines and any querying machine can use the appended records.

While the latter option may sound nice, it adds significantly to the mentioned costs as all those key/value pairs are *duplicated* in the writing+transmission+storage costs, making for a slow and cumbersome system.

Using special start and stop "markers" inside the log statement itself, plus a a slightly more rigid key/value to text conversion, the log text remains largely readable to the human while a querying machine can parse those lines and extract the info it seeks. Not an optimal system for either as the regular log viewing tools don't often deal very nicely with such "markers", which clutter the human read/view, plus the added cost for the querying machine as it needs to judiciously discard/skip any text surrounding those key/value pairs it was going after...

Which brings me to why I find using a binary log system MAY be a boon for applications & users, **but** at the cost of having to provide more-than-adequate tooling for reading/querying/analyzing those log records, where the tooling SHOULD easily interface with regular user tools for log reading/processing, such as `grep`.

### So is binary logging a sign of insanity?

*So, all in all, is having a binary log file system an insane design decision, as Linus Torvalds and others suggest (and I sometimes feel like, myself)?!* 

*No.* And, *yes*, this is still "*No.*" as I am very aware that *problem No. 1 (costs)* for text log files can be rather easily dealt with by *compressing* the data stream using off-the-shelf compression technology: modern systems can often deal directly with gzipped/bzipped text data as if these were *raw/plain* text.

Do note, however, text file proponents, that text log stream compression does not reduce the writing costs, *au contraire*, those are *increased* as now the formatted text data first has to travel through a compressor if you have to reduce transmission costs, or compressed at the receiving end if only storage costs is a concern.

Meanwhile, binary log file systems can apply the same (and other) compression tech to achieve even better results for the same added cost, so binary logging has its benefits.


Linus and friends are correct when they focus on (backwards) compatibility -- and they must do that a lot for they want to present a *stable Linux system* to the world: replacing something that's not really a problem for the majority of their users with something new will surely raise a few hairs; I dare say it will raise the entire scalp!
I, however, am *not* concerned about backwards compatibility and sysadmin learning curves and that particular bit of The Human Condition: I'm looking at logging application behaviour and data in an end-user environment, where those end users are non-expert sysadmins anyway, so `grep` et al is probably not their forte and day-to-day go-to toolset.
… which is why I'm still on the fence re using a system a la Morgan Stanley's for our application-level *binary logging*, rather than plain old text file logging (using `spdlog` or similar).

The second reason I'm still on the fence is that little phrase above: "*the cost of having to provide more-than-adequate tooling for reading/querying/analyzing those log records*", consequently the extra human effort required, which would end up having to be... *mine*. So should I bother?

### Before we go, …

… there's that other part of the critique at binary log systems above and `journald` in particular: they mention it's quite susceptible to **log file corruption**.

Ah! Now **that** is a real issue that we need to address, either way!
Of course, one might go and create an utterly solid ACID solution around the storage part of the log (I'm looking at you: 💖SQLite💖! ) but then the question becomes:
*at the time of application crash, how much of the very last log data lines are we willing to loose?* as ACID implies *rollback* to keep the log file/database *uncorrupted*.

How about instead... we accept that we MAY end up with broken records? Yes, I don't care about adversarial attacks to corrupt my log file/database and the need to identify those, as *I am not in the system security monitoring business* with this -- and therefore can make different choices than Torvalds et al have to, as "activity hiding by editing the logfile" is **not a threat** in my application context! 🤔 Hm, isn't that particular threat not exactly the same for text-based log file systems -- so is that bit of critique valid, or... the other camp employing the same *cavalier* argument tactics? Damn. 🤔 Arguing without politics is bloody hard! … so, back to my question:

*How about instead... we accept that we MAY end up with broken records?* 
⇒ if we don't want to do "full ACID", then we must have a way to deal with b0rked data records at the reader/decoder end of our chain! 
⇒ our reader must be able to unambiguously decide when the data snippet is a *complete* and *valid* record/line: (as this is, in principle, streamed data) append a checksum or record length: "number of bytes written".
We also need to be able to "resync" the reader to the start of the next record/line, when we have just ran into b0rked data ⇒ prefix every record with an easily identifiable "Start Of Record" marker.

At the writer side we MAY assume that we are fully aware of the content of the logged line/record at the time of writing, so there's objection to *prefixing* the record with a "number of bytes in record" type length value, as this is basically a zero cost move of the record from end to start. The checksum, however, is best kept at the end iff that pass is the same as for the act of the transmission itself... In application level code, it is **not** (you don't get record checksums back from network / disk I/O kernel calls) so there's no harm to performance when we move that checksum to the start of the record as well.
Hold on! We don't want to have to read arbitrary length records and checksum them to validate a record/line StartOfRecord marker at resync time, so how about we checksum that marker and the following record/line data *independently*? Where each checksum follows immediately after the thing it checks? So that a stream resync action only needs to buffer the size of a marker+checksum in order to find the proper start of the next log record?
*That* should take care of the `journald` problem and ditto for us: any crap is discovered at the end of the line; the worst-case buffer/rewind action is rolling back one record of variable length's worth and then, while on *b0rk*, there's two scenarios:

- *either* we failed the StartOfRecord validation and must rewind the stream about the size of a header + checksum, minus 1 byte, and scan forward in the stream from there, looking for a fresh and valid StartOfRecord marker -- 📓 it would behove us to have a fixed byte sequence at the start of that thing for simplified initial recognition during such a scan 📓 ,
- *or* we failed the validation of record data itself and must therefore rewind the stream one (variable length) record data's worth **plus** the preceding Marker as that one is part of a b0rkb0rkb0rk and MAY therefore, while checking out itself, possibly, under very very rare circumstances, *overlap* with a new, *proper* marker+record byte series!
  This is our worst case "rewind" action/buffer cost for a binary log stream like that.

That's all one would ever need to deal with data I/O b0rks in an application-level binary log file system, where we intend to deliver the same *write success probabilities on crash* as for classic/modern text line based log file systems -- which, really, suffer from the same corruption issues as discussed here for the binary log format: it's just that *some of you* leave the "recovery/resync" to the human, rather than the machine, and we cannot afford that with a binary log file system: the machine recovery mechanism must be well defined and sure-fire, able to deliver under the most adverse circumstances.

⇒ binary logging is feasible, provided the logging structure is *recoverable* by using a Start Marker + Checksum sync point & record/line validation means.

Note: the Record Length is very nice to have, but you can come up with tricks to do without, e.g. scan until next marker and/or last byte of input makes the checksum pan out -- the latter Smart Alec Move would require a cryptographically strong checksum to work as otherwise you could potentially match incomplete records thanks to checksum collision! However, I feel such chicaneries are uncalled for as simply including the record length makes life so much more easy and robust at very minimal cost.

> Awareness notice: 
> incidentally, I focus on *log data query access* more than on Torvalds/OS level concerns like adversarial logfile editing *post factum* as, to me, the logging is not a means to *audit accountability of system components*, but rather a *diagnostics means* for *post partum* diagnostics and *application telemetrics*. Both are much more of a statistical nature than OS system auditing and thus my argument for binary logging prevails over the arguments against. Unless the very last criterium fails: *my required extra efforts* needed to provide high quality means to access & process the log data files produced.
> 
> Let's see if the Morgan Stanley system is close enough...
> 










====================================================

From Ycombinator about nanlolog:


NanoLog – a nanosecond scale logging system for C++ (github.com/platformlab)

144 points by tzm on Sept 1, 2019 | 63 comments

	
muststopmyths on Sept 1, 2019 

I spent a lot of time trying to build a fast logging system in my last couple of jobs. The basic lesson (and I'm only talking about C/C++/C# here) is that you will spend most of your time formatting strings if you do your file I/O asynchronously.
Since this system has a preprocessor mode, I assume they learnt the same lesson.

The bigger lesson is that it really doesn't matter how many millions of logs you can generate per second if you don't have the infrastructure to store and analyze them easily. No one is going to enjoy digging through these things and the more you generate, the harder it is to extract meaningful information.

In other words, pay a lot more attention to what you'll do with the logs rather than how fast you can spew them out. I have rarely needed more than a few thousand log messages/sec even for a loaded MMO server. I spent way more time creating ways to look at these logs and making them accessible in near real time.


	
cjensen on Sept 2, 2019   

Your points about logging with care saving more cycles that logging efficiently is very right.
That said, here's how I've handled efficient logging in realtime threads since the old days of 100MHz processors: postpone the formatting to a non-realtime thread. Assume your log_printf() will have no more than 10 int-sized args. Grab and save the 10 "ints". When doing the format in the non-realtime thread, pass in the 10 ints. Obviously this won't work with every CPU arch, but it does work fine for all the most common ones. Now every time you do a log_printf(), 1 pointer and 10 ints are copied to a buffer, which is very quick.

The catch is that your format string must be not be changed (e.g. use a string literal). Also any strings you want to print must also be const.


	
gpderetta on Sept 1, 2019  

> is that you will spend most of your time formatting strings if you do your file I/O asynchronously.

That's why you are supposed to do your formatting in the background thread :).

> doesn't matter how many millions of logs you can generate per second

Usually the issue is not generating millions of line of logs (in whitch case dispatching to a background thread just adds overhead), but being able to log with minimal overhead on the surrounding code.


	
Matthias247 on Sept 1, 2019   

> That's why you are supposed to do your formatting in the background thread :).

Which then has the drawback that every argument that gets transferred to the background thread must first be heap allocated or static - instead of only the final buffer being that. My gut feeling is that the additional allocation and memory management costs typically outweigh the cost of formatting in the current thread. Even if one pools all logging buffers less buffers are needed for transferring only formatted buffers.


	
gpderetta on Sept 1, 2019   

Normally you would copy the arguments to be formatted. This is usually less bytes than the formatted output.

	
paulddraper on Sept 1, 2019  

Or you'd copy them.
Unless you do something dumb (like copy a vector when you only need to print the size), copying is significantly faster than string formatting.


	
SnorkelTan on Sept 1, 2019   

I think what OP is hinting at here is cache invalidation/eviction caused by the additional thread's processing and memory operations. If your requirements are down to nanosecond granularities then cache misses are probably being measured and noticeable. A third party logging thread doing memory copies and other log processing sounds like a fine way to unintentionally evict a bunch of cache entries. You might be able to mitigate this to some extent with CPU affinities for threads I suppose. Another option would be to move logging to the network and record packets in/out with an out of band monitoring solution.

	
Rapzid on Sept 1, 2019   

Logging the raw structured log entry and off-loading any string formatting to another system is a very interesting idea. Just push the whole "friendly message" stuff off to other logging infrastructure where the latencies matter less.

	
paulddraper on Sept 1, 2019  

Well yes, using another thread is a only good idea if you have an extra core available.
L1 cache is per-core anyway, and I doubt L2 is going to hurt much.


	
jnordwick on Sept 1, 2019  

At a few places I've worked, logging and util stuff had it's own core to prevent l1/l2 cache pollution (either using threads or shared memory just as long as you got it out of the hot path).

	
Matthias247 on Sept 2, 2019  

Yes, but you need to have a location where to copy them to. They can’t be on the stack, since the logging task runs asynchronously. So each captured argument must be in some form heap allocated. Eg passing 2 strings as arguments and one integer might require 3 additional heap allocation - where the malloc/free overhead might outweigh the string formatting costs. You can try to optimize here with specialized allocators (eg Arenas) or trying to generate dedicated structures for each logging callsite where all arguments can be carried inside a single heap allocated struct (instead of 3 here). But that might lead to other disadvantages - eg code bloat.

	
gpderetta on Sept 2, 2019   

That would be a terrible implementation strategy. You would simply copy each argument into the communication ring buffer. You can also use specialised strategies for some types, for example string contents can be transmitted inline instead of copying the strings themselves.

	
RafaGC on Sept 2, 2019  

On my logger the thread queue was(is) node based, so it was just a matter of making a bigger contiguous allocation and placing things there contiguously, the log entry and the data copies. 1 allocation.

	
paulddraper on Sept 2, 2019  

Good point

	
cma on Sept 1, 2019  

Shared memory to a background process is better, in that it can survive most classes of process crash.

	
gpderetta on Sept 1, 2019   

Yes it is significantly more robust, the downside is that you have to copy everything, including static vars and format strings. An option is to fork early on process startup and possibly share some memory.
We simply catch all signals and do a best effort attempt to flush the queue before aborting.


	
holy_city on Sept 1, 2019  

I've dealt with this problem too, mostly in trying to log/debug real-time media processing where a log trace equivalent would be long chunks of floating point data at multiple trace points in the data flow.
I came to the same conclusion. Raw bandwidth to your logging endpoint is meaningless when you start dealing with big chunks of information, which is common sense I guess. You need a fast way to digest it into whatever format works for the person reading the log later, and it's tough to make it abstract since how data is interpretted can be quite varied even in the same application.


	
cbsmith on Sept 4, 2019  

> you will spend most of your time formatting strings if you do your file I/O asynchronously

Which is why good logging systems try to defer as much formatting logic as possible until the last possible moment (ideally render time).

I wish more logging frameworks would log in some efficient intermediary serialization format like Cap'nProto or Protobuf.


	
profitnot on Sept 2, 2019  

This is, of course, true. Many fail with the mindset that if only they can build big enough haystacks, the needles will coalesce like shiny seaglass upon the shore, because surely they will hire the algorithmic wizards, and oh yes.. AI(!!) and ML(!!).
Still, this really could have useful applications in scientific applications where the goal is to capture extremely fast events in detail... super-colliders, fast-fusion events, etc...


	
sharpshadow on Sept 1, 2019  

That's why I open comments first.. hypetaker +1

	
MauranKilom on Sept 1, 2019  

The code overall is pretty clever, but at the core of all this they completely ignore strict aliasing to dump stuff into a char* buffer indiscriminately... Look at this function:
https://github.com/PlatformLab/NanoLog/blob/master/runtime/N...

     T argument = *reinterpret_cast<T*>(*in);
     ...
     uint32_t stringBytes = *reinterpret_cast<uint32_t*>(*in);
A total of 6 reinterpret_casts in that file alone.
I didn't see any indication that you have to turn off strict aliasing to use this library (and would you really want to make all your remaining code slower just so your logging has better benchmark scores?). Which makes all this code UB as far as the C++ standard is concerned (no, their allocator does not magically placement-new the correct object types into the right places).

They could just inspect their T values as char* and/or memcpy them around instead of all this pointer aliasing (well, as long as their values are trivial types - I couldn't figure out whether you can log non-trivial stuff here?) at virtually no additional cost.

Edit: And this right here convinces me that I definitely wouldn't want to use this library in production... Adding a volatile to fix a threading bug is just a big no-no.

https://github.com/PlatformLab/NanoLog/commit/e9691246ede6da...


	
gpderetta on Sept 1, 2019   

> Adding a volatile to fix a threading bug is just a big no-no.

The fence instructions also don't make much sense. Not sure why one would not use std::atomic in a c++17 only library.


	
Leszek on Sept 1, 2019  

You'd think so, but in fact reinterpret_cast from char* (and unsigned char, and std::byte) is explicitly allowed by the type aliasing rules.

	
MauranKilom on Sept 1, 2019   

To the best of my knowledge, casting to char* is totally fine (inspecting an object as bytes) under certain constraints.
What is not fine is pretending that an object lives at a position in memory where it does not (i.e. treating bytes of memory as some object through a reinterpret_cast away from char* ).

Edit: To clarify, casting away from char* is of course allowed if you cast to whatever object type actually lives at the pointed-to location.


	
userbinator on Sept 1, 2019   

(i.e. treating bytes of memory as some object through a reinterpret_cast away from char ).*
That's how people have used C and C++ for decades in low-level work, and it still works exactly as you'd expect, so stop saying "don't do it" because you're only encouraging the compiler-writer-UB-optimisation-nonsense crowd to make things even worse. It's already bad enough that they think the Holy Standard is the only thing that matters.

I believe Linus has several memorable rants on this topic already.


	
pingyong on Sept 2, 2019   

Doesn't matter anymore, Clang and GCC will both make any and all optimizations they can find by assuming you will never hit UB, including just erasing all UB code. You can dislike it, but that's just the reality now, and ignoring it won't make it go away. The only way to change it would be a change in the standard.

	
detaro on Sept 1, 2019  

If I remember correctly, it is fine if there actually was an object of that type (or a "similar" type) in that location. So casting to char* and back is fine, casting to char and then to int-type to inspect multiple bytes is fine as long as alignment plays correctly, ...
It seems like the function only cast to T* if the input pointer hasn't been changed (the paths that do modify it return early), so there's a value there?


	
MauranKilom on Sept 1, 2019   

> it is fine if there actually was an object of that type (or a "similar" type) in that location.

That is correct. See my other nearby reply.

> So casting to char* and back is fine

Indeed.

> casting to char and then to int-type to inspect multiple bytes is fine as long as alignment plays correctly, ...

Only if the place in memory you are pointing to actually has a live int object. In particular, inspecting the byte representation of an object is fine (including copying the bytes elsewhere, which is why they could just use memcpy instead of reinterpret_cast).

See also http://eel.is/c++draft/basic.types#2 and surroundings.


	
gpderetta on Sept 1, 2019  

> casting to char and then to int-type to inspect multiple bytes is fine as long as alignment plays correctly

It is only fine if there were the same exact int types at that address. Remember, UB usually isn't the reinterpret_cast but the dereference.


	
detaro on Sept 1, 2019   

I thought the int-types had a special exception making that always possible (if there's a bunch of bytes, you can turn them into an int), but I could very well be wrong on that detail.

	
gpderetta on Sept 1, 2019   

Char is the only exception.
And now std::byte IIRC


	
Leszek on Sept 1, 2019  

I had thought that, as long as the object in that location is valid (e.g. constructed with placement new and not since destructed, with the `char` as a pointer to the storage), then accessing it through the `char` is valid -- but maybe that's only true for `void*` now that I think about it. Either way, fair enough, placement new probably hasn't happened (haven't read the rest of the code).

	
Iwan-Zotow on Sept 1, 2019  

char* is universal memory format, you could specialize from that into anything you want

	
MauranKilom on Sept 1, 2019   

That's not how the C++ object model works. Objects in C++ have clearly defined lifetimes. If you reinterpret_cast<int* >(someCharBuffer) and then dereference that, you have Undefined Behavior unless an int object is alive at that exact location.
You can do

    int value[2] = {0, 0};
    char* ptr = reinterpret_cast<char*>(value) + sizeof(int);
    (*reinterpret_cast<int*>(ptr))++;
	
or (given knowledge about compiler padding):

    struct XY { double x; int y; };
    XY xy = {};
    char* ptr = reinterpret_cast<char*>(&xy) + sizeof(double);
    (*reinterpret_cast<int*>(ptr))++;
	
or even (placement new):

    unsigned char* buf = new unsigned char[12];
    buf += 4;
    new (buf) int(0); // Create an int object in the buffer.
    (*reinterpret_cast<int*>(buf))++;
    delete[] buf;
	
But in each case an int object has had its lifetime begin before you can treat the bytes in memory like an int. Everything else is UB.

	
bonzini on Sept 1, 2019   

You cannot do the last unless you know that the argument of placement new is suitably aligned to receive an int object, however.

	
jnordwick on Sept 2, 2019  

What's your problem with the volatile? Using it to prevent register caching of or a value or prevent optimizing out repeated reads is perfectly fine. That is how it is supposed to be used.
The bug was that one thread was reading the gcc thought it wouldn't be update between reads. It is even used properly by grabbing a copy of the volatile value instead of reading it repeatedly.

edit: I also don't understand your aliasing issues. A `char*` can point anything and you are allowed to cast back out of it to the correct type. It is casting to int because the string is prepended by its length (the var names are "size" and "nibble").


	
MauranKilom on Sept 3, 2019   

> Using it to prevent register caching of or a value or prevent optimizing out repeated reads is perfectly fine. That is how it is supposed to be used.

The code has a race condition, plain and simple. volatile does not lead to thread synchronization, and that function is (as gcc correctly identified) nonsensical in a single-threaded world. The code expects that a different thread writes to the variable at the same time that this thread might read from it - that's the definition of a race condition, which is straight up UB (volatile or not).

You are trying to reason about compiler optimizations from the perspective of the machine and memory model you know ("prevent register caching of a value" etc.). That's wrong though - compiler optimizations operate on the abstract machine that C++ is defined on. In the abstract machine, this code has a race condition (the absence of which is guaranteed to compilers by the C++ standard) and so optimizations could easily violate whatever mental model you are using to conclude the code is fine. The code will likely break again some time in the future, just like it broke this time for a newer compiler version. Or maybe it compiles to subtly broken code right now, who knows!

> I also don't understand your aliasing issues. A `char* ` can point anything and you are allowed to cast back out of it to the correct type.

int is not the "correct type" because no int object lives at the pointed-to location after the cast. They could just memcpy and everything would be fine, but writing to/reading from type aliased pointers (other than char* and friends, in specific circumstances) is UB.

Here is a good description (in the context of C, not C++) of strict aliasing: https://stackoverflow.com/questions/98650/what-is-the-strict...


	
jnordwick on Sept 4, 2019   

> The code expects that a different thread writes to the variable at the same time that this thread might read from it - that's the definition of a race condition,

No, that isn't. In one thread it reads a value over and over. Another thread will periodically set it. This isn't a race condition and requires no synchonization. On writer thread only, and you're fine. If the code was doing more, I didn't see it.

I just wrong something very similar yesterday where I had a counter that is read over and over and an async handler updated it. I had to make it volatile to prevent the the spin on it from optimizing out the load.

Yes, in low level programming, you have to know about the hardware, what a register is, what cache it, etc. C++ has a very lose (almost nonexistent) abstract model. This isn't UB.

> int is not the "correct type" because no int object lives at the pointed-to location after the cast

The string had its length prepended to it. There was an int there. Also, those aliasing rules are more about optimization issues, not as much about alignment issues (eg, if you are only writing for intel/amd hardware alignment only really matter for SIMD, and there is no penalty on unaligned access (with a few small exceptions that didn't apply here).


	
MauranKilom on Sept 4, 2019   

> No, that isn't. In one thread it reads a value over and over. Another thread will periodically set it. This isn't a race condition and requires no synch\[r]onization. On\[e] writer thread only, and you're fine.

Sorry, wrong term by me. I meant data race, not race condition (although many would regard the former as a form of the latter). A data race is two or more threads accessing the same location without synchronization, where at least one of them is modifying the value. A data race results in undefined behavior. Here are the relevant sections from the C++ (draft) standard:

http://eel.is/c++draft/intro.multithread#intro.races-21

http://eel.is/c++draft/intro.multithread#intro.races-2

> I just wro\[te] something very similar yesterday where I had a counter that is read over and over and an async handler updated it.

If you wrote this code in C++, you created a data race and your program has undefined behavior.

> I had to make it volatile to prevent the the spin on it from optimizing out the load.

volatile may prevent the load from being optimized out, but it does not prevent the data race (because it does not prevent instruction reordering). Your program still has undefined behavior.

Further reading:

https://www.kernel.org/doc/html/latest/process/volatile-cons...

> C++ has a very lose (almost nonexistent) abstract model

I don't know what you mean with "lo\[o]se" or "almost nonexistent" but here are a few specifications of the abstract machine that C++ is defined on:

http://eel.is/c++draft/basic.memobj

http://eel.is/c++draft/basic.exec

> There was an int there.

No. Just because you operate on the memory as if it was an int doesn't create an int there.

> Also, those aliasing rules are more about optimization issues

Correct! And those optimization issues are the primary way in which UB manifests itself in your program.


	
jnordwick on Sept 7, 2019   

It seems like you're one of the UB brigade that demands rigid adherence to the standard because some 8-bit microprocessor might not be able to read a 32-bit value atomically. Nobody care about that, and when writing high performance code, you write the hardware you target and care about.
Reading and writing to the same int in memory is atomic for every hardware platform I care about. Even alignment issues don't matter for anything this is going to be run on.

> No. Just because you operate on the memory as if it was an int doesn't create an int there.

Don't be dumb. They wrote an int to the beginning of the buffer and then had to cast back to it, like every piece of networking or file code that had to save a binary number.


	
SnorkelTan on Sept 5, 2019  

You can leverage the cpu's atomic operations such as compare and set, compare and swap, as well as the properties of the memory subsystem to achieve thread safety without an operating system mediating the interaction. Here's a blog post from some people that have street cred to give you some idea of the potential performance benefits: https://mechanical-sympathy.blogspot.com/2013/08/lock-based-...

	
rafa1981 on Sept 1, 2019  

Author of mini-async-log and mini-async-log-c here.
https://github.com/RafaGago/mini-async-log

https://github.com/RafaGago/mini-async-log-c

I wrote both some time ago and AFAIK they were the fastest, how does nanolog compare against them?

At least before the source file preprocessor Nanolog was slower.

Here I had a benchmark project with Nanolog on an old version, maybe you can PR an update:

https://github.com/RafaGago/logger-bench


	
RafaGC on Sept 2, 2019   

As comments can't be edited: there are two nanolog projects on github. They may not be the same one.

	
wrs on Sept 2, 2019  

I did something very similar to the “preprocessor” version of this back in 2001 for Microsoft application crash reporting (the problem then being upload size, not so much logging overhead). It’s very cool to see how you can do the whole thing with constexpr now. As I recall, people really didn’t like using the preprocessor!

	
usefulcat on Sept 1, 2019  

The description says "the compilation builds a version of the NanoLog library that is non-portable, even between compilations of the same application". They also mention that a decompressor application also gets built and can be used to reconstitute a human-readable version of the log file. My question is, is the decompressor application also tightly coupled to a particular compilation? It sounds like the answer is yes, but I'd really like to be wrong about that.
That would really suck for cases where you want to store old log files for later use. You'd have to also store a copy of the decompressor program that corresponds to each log file. Or you'd have to convert to human-readable format before storing the log files, which loses a number of the benefits of the binary log format (compact size, faster to parse).


	
nemetroid on Sept 1, 2019   

In the related paper\[1], this paragraph (5.1.2) strongly suggests that the decompressor is non-portable as well:

> NanoLog performs best when there is little dynamic information in the log message. This is reflected by staticString, a static message, in the throughput benchmark. Here, NanoLog only needs to output about 3-4 bytes per log message due to its compaction and static extraction techniques.

1: https://www.usenix.org/conference/atc18/presentation/yang-st...


	
Mathnerd314 on Sept 2, 2019  

Did you skip over the section where they say to use the C++17 version that doesn't have this issue?
I looked in the paper to find the explanation for why the preprocessor version is non-portable; there wasn't anything clearly written but it seems to be related to how it assigns unique ID's to log statements.


	
gmueckl on Sept 1, 2019  

I built a similar system with a focus on profiling events with extremely low overhead. When taking an timestamp, the processor cycle count register is read, and pushed into a ring buffer with a tiny descriotion including a static string label. A background thread perodically writes that buffer contents to disk. An offline tool visualizes the recorded events and intervals over time and can do a bit of statistics.
This system has served me quite well, but I need to rework the UI because zooming to microsecond resolution on a 30 second trace overflows the range values in the Qt scrollbars.


	
liuliu on Sept 1, 2019   

We use custom format and an offline process to transform the format into Google Catapult for visualization. Catapult visualization for 30s trace should be pretty reasonable.

	
Const-me on Sept 1, 2019  

Recently built similar stuff. Even using same __rdtsc() for time. I didn't require any preprocessing. Instead, I require the strings come from readonly section of a module, and logging pointer values. Also logging into circular buffer in shared memory instead of file, as I was only interested in knowing what the app did immediately before a rare crash which takes hours to reproduce.

	
rafa1981 on Sept 1, 2019   

I did exactly the same on my two loggers, as a C literal is always read only and unique, passing a printf-like string is passing a pointer.
The it's just a matter of adding validation and passing the arguments in a compact way.


	
bonzini on Sept 1, 2019  

How do you deal with ASLR (logging from shared libraries or from position-independent executables)?

	
Const-me on Sept 1, 2019   

In that particular case I’ve cheated. I’m writing out of band file which maps pointer addresses into strings. In runtime it’s CAtlMap<const char*, int> really fast because the keys are just addresses. I only have ~200 unique messages (ignoring format arguments) so the file is only written for the first few milliseconds. I did that because it was the simplest thing to do. I could use debug symbols + crash dumps, they can resolve these pointers regardless of ASLR, just it was way more complex to implement. BTW, the only reason I was doing that, the main executable is gta5.exe, it’s encrypted, it resists debugging, and I don’t have debug symbols. If it was my own app, I would probably do something much simpler instead.

	
nemetroid on Sept 1, 2019  

From a quick reading of the paper, it sounds like Nanolog is internally translating logging messages into a compacted format, which must be further processed to become human readable. This further processing is not included in the benchmark.
section 4.3, "Decompressor/Aggregator"

> The final component of the NanoLog system is the decompressor/aggregator, which takes as input the compacted log file generated by the runtime and either outputs a human-readable log file or runs aggregations over the compacted log messages. The decompressor reads the dictionary information from the log header, then it processes each of the log messages in turn. For each message, it uses the log id embedded in the file to find the corresponding dictionary entry. It then decompresses the log data as indicated in the dictionary entry and combines that data with static information from the dictionary to generate a human-readable log message. If the decompressor is being used for aggregation, it skips the message formatting step and passes the decompressed log data, along with the dictionary information, to an aggregation function.

section 5.1.2, "Throughput":

> \[...] NanoLog performs best when there is little dynamic information in the log message. This is reflected by staticString, a static message, in the throughput benchmark. Here, NanoLog only needs to output about 3-4 bytes per log message due to its compaction and static extraction techniques. Other systems require over an order of magnitude more bytes to represent the messages (41-90 bytes).

> \[...] Overall, NanoLog is faster than all other logging systems tested. This is primarily due to NanoLog consistently outputting fewer bytes per message and secondarily because NanoLog defers the formatting and sorting of log messages.

section 5.1.3, "Latency":

> All of the other systems except ETW require the logging thread to either fully or partially materialize the human-readable log message before transferring control to the background thread, resulting in higher invocation latencies. NanoLog on the other hand, performs no formatting and simply pushes all arguments to the staging buffer. This means less computation and fewer bytes copied, resulting in a lower invocation latency.

So NanoLog performs less work and therefore has much higher throughput. This separation of logging and formatting may possibly be a great idea, but it should probably be more prominently mentioned wherever the benchmark table is posted.


	
bt848 on Sept 1, 2019  

Looks interesting but how does one reproduce the comparative benchmarks from the paper? I see the code that benchmarks nanolog but how do I reproduce the baseline for glog?

	
rafa1981 on Sept 1, 2019   

Author of mini-async-log and its C variant here.
Some time ago I wrote a benchmark for different loggers, but it uses an old version of Nanolog.

https://github.com/RafaGago/logger-bench


	
bt848 on Sept 1, 2019   

Neat. Your code actually contains the issue I was curious about. Why do you log at level ERROR? As I understand it this causes glog to synchronize the output stream after every message.

	
rafa1981 on Sept 1, 2019   

But worst case latency, the most interesting property of an async logger is still unaffected I guess...

	
rafa1981 on Sept 1, 2019  

Lack of knowledge on glog

	
IloveHN84 on Sept 1, 2019  

You can't claim it is faster if you don't provide reproducible benchmarks.

	
RafaGC on Sept 2, 2019  

Is this a new incarnation of this project?
https://github.com/Iyengar111/NanoLog

Otherwise it seems that the name is already taken.


	
RafaGC on Sept 3, 2019   

Both projects are a different C++ logger implementations. The first commit of both repositories is unrelated. If I wanted to benchmark both name clashes are likely to happen.
This is a legitimate question. Why downvoting?


	
jart on Sept 2, 2019 | prev  

Cool stuff. Judging by his code, this guys mind is probably going to get blown when he discovers mmap() and bcd arithmetic.


