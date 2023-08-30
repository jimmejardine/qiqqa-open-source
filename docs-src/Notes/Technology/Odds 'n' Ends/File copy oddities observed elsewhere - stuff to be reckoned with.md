# File copy oddities observed elsewhere :: stuff to be reckoned with

## File copy failures do leave a b0rked copy

Observed this one with several tools (including my otherwise trusty `robocopy`, alas): when copying large amounts of data off a slightly finicky SSD onto another local drive, `robocopy` would report "**access denied**", "**device not connected**" and other odd errors right smack in the middle of the file data copy activity, triggering a *retry* and always failing to pass through.

> No network involved here; everything was hooked up on a USB3 hub that's seen Petabytes of transfer already.

- **Odd observation number 1**: when you configure `robocopy` to *never retry* (`/r:0`), `robocopy` will happily oblige and zip through that (hardware failure) corrupted source file, **only to leave the partial copied result, with the *correct* filesize, on your destination disk.** That's pretty scary because, let's be real, how often do we scrutinize console logs?

  Besides, when multiple files suffer from this effect (which was the case here; remember, the source SSD was already finicky) then the reports will scroll off the screen and having `robocopy` write a log file?... Right. Not.

  **What did I expect?**

  I *expected* WinRAR-like behaviour (which is one of the reasons why I like that tool): any error to complete the output/copy will result in **the destination file being deleted** to ensure nobody ends up with silently corrupted stuff. Great design decision! 

  **Remind self to build this into our own sync code as well!...**

- **Odd observation number 2**: **everybody** suffered from that same issue, not just `robocopy`: I tried a few other applications, plus basic Windows Explorer drag & drop copying and all produced some half-arsed output files, while reporting/logging source disk errors. If you know where to look... but the end result from a user perspective is totally rotten: there's no easy filesystems-only comparison way to detect you've been shafted by your file copier.

  **One exception to no. 2**: *XYplorer* seems to have delivered. I say *seems to* because it miraculously did not report any source disk issues, did not pop up any 'oh bollocks!' popup dialogs telling you the shit has hit the fan, the registry remains curiously silent... **this is just weird, man.** And that was a copy set to 'overwrite if content does not match', so we're in need of another application that can byte-compare a few *huge* source/file trees in total paranoia mode and not b0rk on my blue suede shoes just because a honking source SSD decides to throw a micro-tantrum once in a while -- I still don't get it how the XYplorer copy session did go through without obvious failures; I already checked the files that the others were **consistently** rejecting/erroring out on and they are perfectly readable PDFs which the various sanity checks and a full top-to-bottom quick view in FoxIt all gave the thumbs up all green you're golden merit badge of greatness. *Yes,* of course *I'm spooked by this. WTF?!*
  


## File copy oddities at the edge: PAR2 / checksums to the rescue?

Okay, this happened during the same days that my machine (i7 laptop) decided to go on the Fritz, but only *temporarily*, so, yes, odd, odder, oddest is here & now.

What happened was a large file copy session at speed across an USB3 bus (the same as above, BTW). Everything was hunky dory until, for some unknown reason, one of the **other USB3 devices** decided it wanted to join the local BDSM chapter with a Diamond Premium membership by somehow **locking up the Windows system /SLOWLY/**.

It goes like this: you get yourself another USB2 device like I did, which is kinda special: it has old IDE next to SATA connects for servicing antiquated laptop HDDs and similar -- all that is not really relevant here, but adds... flavour. Anyway, having had *decades* of USB usage and equipment experience by now, I have found that most of the horde of USB controllers plonked in various equipment is a wee bit flaky: when copying enough data across the gate, they somehow, *randomly*, decide to mess up and fail: disconnect errors and such-like will be your meal after loading them heavily enough.

Happy exceptions are (or rather: *were*) the most basic WD Books (external USB HDDs); the niftier ones that came with Ethernet ports alongside were instable like all the rest. I still use them WD Books.
Next came the ORICO HDD mount boxes, which have performed flawlessly for several years now. You load them with 'Internal HDDs' (SATA) and off they go. When you're careful with your (and others') equipment, those are great little boxes to quickly mount an HDD and copy stuff back & forth. My backup go-to hardware on the cheap. Love it.

Now there is (and I have) plenty of other brands' USB HDD mount boxes and other stuff, but there the rule of thumb is: **use sparingly and only for light task or you *will* be punished**. Today was the day. I needed access to an antique laptop HDD and this kit was the only one I still have that could. So it got hooked up to the USB3 router, next to the other HDDs.

Then, of course, I decided to just let those other HDDs rip a backup copy, while I was picking my nose and thinking odd things about old computer hardware and software. This went on for a while, until that Malleus Maleficarum with the laptop HDD decided it wanted to play Bad. It went like this:

- the backup copy session suddenly dropped from 80MB/s down to ZERO(0).
- user (me) looks at screen and goes WTF?
- Mouse is still responding, Windows is doing its thing, the tools are still responsive when I click and scroll.
- this goes on for a bit (the WTF slowed me down) until I run another command in a `cmd` console on a *local disk* (laptop internal, nothing to do with the USB3 stuff) and that command starts, then stops hard, mid-track. Lock up. 
- nothing helps, another WTF+ and Process Hacker (yay! great tool!) is called in to do thee dead: kill 'em.
- done.
- Meanwhile, stuff starts to act weird, then weirder, until the Windows decides to quit to listen to *any* user input and it's total screen freeze time.
- after a bit (I was *slow* today) no blue-screen, but straight to black and reboot it is.
- this doesn't pan out as next thing what happens is the laptop throwing a tantrum, h0rking up a fatal blue-screen every time during boot.
- go ape. disconnect everything. Rip the power cord and accu-pack, make the bastard **cool boot** like it was on *ice*.
- Windows Repair yada yada yada, lots of crap and an *oddly lethargic* boot sequence taking ages and maybe a few crashes along the way (screen going standby mode every once in a while there), but finally The Windows is back from the dead. Registry is filled with disk failures so it's prayer time.
- reconnect the USB3 hub and drives.
- check the running backup copy: all seems fine, as far as it got. (after all, it got aborted *hard* halfway through)
- doing a bit of new work.
- getting a paranoia wave (thanks to previously having experienced the copy corruption item listed at the top) and checksum-checking the source vs. copy content.

Warning: again, at face value everything looked good. This last, deeper check uncovered several files had their timestamps and whatnot, but were corrupted content-wise, some garbled, some all-zeroes. Of course, this is to be expected when you have a weird lockup like that. (You can bet your bottom the seconds (minutes?) leading up to that were already well off into uncharted territory, Known Unknown State and all that jazz.)

**Moral of this story**: it might be a speed optimization to forego verify after write on copy (and it wouldn't have worked here, because what can you do when everything is locked up already?) but it would be a very good thing indeed to verify every you *import*, i.e. verify the data at some later point before you go and *use* it.

Thus I'm inclined to advocate publishing a checksum list with the backup/sync data of any library, including Qiqqa's.

Point in case: we already have a checksum for every *document*: it's fingerprint! So it might be good to check the remote copy against that when you sync and have the bandwidth to spare. Then, if the document turns out to be b0rked, my initial choice would be do *delete it*: if the remote site is the only one that got it, than the remote site can copy it once again at the next sync episode there.
Meanwhile you're out one expected document, so Qiqqa will need to cope with missing documents like that (**not** the same as a *document reference* where the document has never been loaded yet!) and the diff+merge logic should bee made to cope with this eventuality. In other words: a sync destination *can* be *incomplete*, when we sync to it.

Same goes for the metadata, i.e. the database itself: classic Qiqqa includes a content hash field with every record and b0rks when it doesn't match -- while I sometimes feel that's overkill, here's a situation where that saves your bacon by detecting otherwise silent corruption. At a cost, alas.

When we decide to also sync the OCR text content (see elsewhere for that train of thought), that stuff should be checksum-validated as well for the same reason.




