(ripped from https://github.com/jimmejardine/qiqqa-open-source/issues/196)

# Where to get Qiqqa?

> Bottom Line/TL;DR: if a [listed release](https://github.com/jimmejardine/qiqqa-open-source/releases/) doesn't have a setup **EXEcutable** of any kind in its **Assets** list, then it's just a version label, not a full release. Many of those are experimental releases which  are only available at the secondary URL, listed further below. 

---

New Qiqqa releases are published at two different URLs:

1. the main URL is used for tested major releases: these are published in the Releases section of repository at this URL:

   https://github.com/jimmejardine/qiqqa-open-source/releases/

   ![Snag_9e3530ee](https://user-images.githubusercontent.com/402462/77826912-cd8ad800-7112-11ea-922b-cbfaed109885.png)

   ![Snag_9e340dac](https://user-images.githubusercontent.com/402462/77826888-a3d1b100-7112-11ea-80ac-125fd5b6f003.png)

   **Note: only those labels/entries which include a `setup.exe` (or a versioned version of that file, e.g. `setup-v82pre.exe`) in their Assets subsection are "main releases".** Currently there's only Qiqqa v80 there; newer releases are still *experimental* and only available at the secondary URL.

2. All *experimental* releases are published at the secondary URL:

   https://github.com/GerHobbelt/qiqqa-open-source/releases

   The same rule applies here: only actual *releases* have a setup exe ready for download in their Assets subsection:

   ![Snag_9e3b2316](https://user-images.githubusercontent.com/402462/77827037-b5678880-7113-11ea-82b3-d37550161bc4.png)


# Installing a Qiqqa release

   When you  double-click the installer after downloading, it will run and *overwrite* the existing Qiqqa version (after a dialog has reported a different version is being installed).

   This is harmless, as your libraries reside elsewhere on your disk and those **are not touched** during the install, only the Qiqqa executable and underlying binaries are replaced by an install action.

   Qiqqa v80 and v82 releases are backwards compatible with Qiqqa v79 (commercial), which means:

   When you want to 'revert' to the older version you had before, you download its installer (for v79 that's either at qiqqa.com or the copy in the secondary repository's releases list: https://github.com/GerHobbelt/qiqqa-open-source/releases/tag/v79

   That install process will be exactly the same and will overwrite the installed Qiqqa version once again, thus reverting to your old version. 

   Bottom Line: every Qiqqa installer replaces what you have. It does not care whether it overwrites with newer or older software. That way you can always revert to a "known to work" version.

---

# FAQ (TBD)

> What steps should some take to update to a new version? 

See the description above.

> If I install a new version will it automatically delete the old version? 

Yes. All your data is kept intact. Only the Qiqqa binaries are replaced.

> When I download and unzip the latest release it appears that not all packages have a setup.exe file to update to. Does this mean that one should just browse the folders and look for the folder that has the most current setup.exe file? 

No, the ZIP files are **source code only** archives. For a while, the generated setup binaries were included in the source code repo but that was *wrong* use of git & GitHub.

Actual releases are available as setup*.exe installers at the afore-mentioned URLs in the **Assets** subsection(s).

> It appears that there was a transition in the naming convention of the setup.exe in open source. The file no longer contains the version number. Is this correct? 

Yes, v80 was published without a version encoded in the setup.exe; this has happened with a few experimental releases as well. Newer experimental releases follow a version scheme similar to MAJOR.minor.buildnumber, e.g. v82.0.7357.40407 (which is sometimes also referred to as v82pre9,  i.e. the 9th experimental prerelease of Qiqqa v82). The buildnumber `7357.40407` is an encoded timestamp: higher is more recent, thus newer release.

> Makes it harder to search for the most current version. 

Yup, I concur. That is a bit messy, while we got our act together. 😄 
