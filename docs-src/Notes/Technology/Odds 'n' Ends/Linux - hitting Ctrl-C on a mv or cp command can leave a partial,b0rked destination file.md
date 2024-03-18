# Linux: hitting Ctrl-C on a `mv` or `cp` can leave a partial destination file

Discovered on the Linux rig: aborting `mv` or `cp` commands doesn't necessarily abort *nicely* between file transfers: the last file written on the destination disk *may very well be b0rked/partial*.

Not a big problem as the original file at the source is still there, but this becomes a bit of an issue when you run those commands in `--no-clobber` / update mode and the next run of such a command decides the b0rked/partial is not to be overwritten. Sure, it's recoverable, but long-running "fire & forget" sessions like that will require a little more attention during the end inspection: "did everything work out all right?" 
Well... possibly *not entirely*. 

*Cave canem.*

