# qiqqa-open-source

The open-sourced version of the award-winning Qiqqa research management tool for Windows

This version includes EVERY feature available in Qiqqa, including Premium and Premium+.

Unfortunately we have had to remove the web cloud sync ability as that is the one area that involves storage costs.  Users are encouraged to migrate their Web Libraries into Intranet libraries, and use Google Drive or Dropbox as the 'sync point' for those libraries.

You'll need to be a little technically experienced in the .NET world to get a build up and running.


## Visual Studio 2017 (Original build environment)

You will need to download Visual Studio 2017 Community Edition and Syncfusion Essential Studio 14.1.0.41 (you can get a free evaluation key from syncfusion.com - works for single developers for private/free projects).
  
Then simply go to `./Qiqqa.Build/` and run `go.bat` to build the latest version into the `Qiqqa.Build/Packages` directory.

(Alternatively, when using `bash` on Windows, you may execute `./build-installer.sh` from the repository base directory to accomplish the same as when you'ld have executed `go.bat`.)


Would anyone who is interested in contributing towards this repository please contact @jimmejardine...


## Visual Studio 2019 (updated *EXPERIMENTAL* build environment)

As above, but instead you'll need to obtain and install Syncfusion Essential Studio 17 as this fixes at least **one very annoying bug** in Qiqqa's handling of (broken!) PDF files which are already part of your Qiqqa librarie(s).

See also Qiqqa bug reports:

- #9
- #10
- #11
- #12
- #13
- #14


