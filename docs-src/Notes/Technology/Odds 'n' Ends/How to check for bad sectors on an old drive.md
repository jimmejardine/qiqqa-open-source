# How to check for bad sectors on an old drive we decommissioned some time ago?
 
I was wondering if this one could still serve as a scratch disk for some rough work that would be hammering the drive relentlessly; I was looking for an older drive that I wouldn't mind going up in flames, but rather near *the end* of the testing runs than at/near *the start* of it. I knew this one is making noises that make me uncomfortable.
 
```
badblocks -v -n -s /dev/sdm
```

--> 8 bad blocks reported on the first run. No bad blocks on the second, so that drive is old, going bad, but still has recovery powers remaining. Good choice for a test/scratch disk that's bound to fail in a few weeks of testing!
 