# CLI: Upgrade To SHA256

## Purpose

Given shattered.io and other SHA1 vulnerabilities, which are further aggravated by the qiqqa bug 
where 0 valued most-significant-in-byte nibbles in the PDF content checksum are discarded, it's 
time we go and see what it would take to upgrade Qiqqa storage to SHA256, preferrably in a 
backwards-compatible manner, where we keep the existing SHA1 signatures and convert to SHA256 on the fly.

Not the highest priority technology upgrade today, but when one wants to store both PDFs from shattered.io as part of their qiqqa PDF library, you're toast as Qiqqa won't be able to differentiate between those two and consider the one identical to the other, which is bothersome.

No matter what you feed Qiqqa (as long as it's PDF), it should be (*uniquely*) identifiable!


---

## Motto

This here is part of the technical storyboarding side of a UI & UX overhaul of Qiqqa.

Before we put it to Qiqqa, it will be tested here.


---

## Task Completed

This technology test has been done in my MuPDF-based toolchain. One notablee difference is the use of BLAKE3 instead of SHA256 as the new hash algorithm as that onee is *faster*, both theoretically and in practice, while offering the same quality of uniqueness guarantees. 

The new `mutool qiqqa_fingerprint0` and `mutool qiqqa_fingerprint1` tools calculate the classic Qiqqa hash and the new BLAKE3+Base58 hash, respectively.

Read the story in [Fingerprinting :: moving forward and away from b0rked SHA1](../docs-src/Progress in Development/Fingerprinting - moving forward and away from b0rked SHA1.md).


