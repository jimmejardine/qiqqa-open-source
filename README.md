# Qiqqa (Open Source)

The open-sourced version of the award-winning Qiqqa research management tool for Windows.

This version includes EVERY feature available in Qiqqa, including Premium and Premium+.

Unfortunately we have had to remove the web cloud sync ability as that is the one area that involves storage costs.  Users are encouraged to migrate their Web Libraries into Intranet libraries, and use Google Drive or Dropbox as the 'sync point' for those libraries.

You'll need to be a little technically experienced in the .NET world to get a build up and running.


## Visual Studio 2017 (Original build environment, now *OBSOLETED/UNSUPPORTED*)

You will need to download Visual Studio 2017 Community Edition and Syncfusion Essential Studio 14.1.0.41 (you can get a free evaluation key from syncfusion.com - works for single developers for private/free projects).
  
Then simply go to `./Qiqqa.Build/` and run `go.bat` to build the latest version into the `Qiqqa.Build/Packages` directory.

(Alternatively, when using `bash` on Windows, you may execute `./build-installer.sh` from the repository base directory to accomplish the same as when you'ld have executed `go.bat`.)


Would anyone who is interested in contributing towards this repository please contact @jimmejardine...


## Visual Studio 2019 (NEW, SUPPORTED build environment)

As above, but instead you'll need to obtain and install Syncfusion Essential Studio 17 as this fixes at least **one very annoying bug** in Qiqqa's handling of (broken!) PDF files which are already part of your Qiqqa librarie(s).

### How to build the `setup.exe` installer

- Make sure you have these installed (it may work with other tools, but this is what I (@GerHobbelt) use):
  + Microsoft Visual Studio 2019 Professional (IDE)
  + [Git For Windows](https://gitforwindows.org/) - which includes `bash`
- Open `Qiqqa.sln` in MSVS
- Build > Batch Build > (tick all 'Release' items or simply Select All)
  + When this is done, you should have a Qiqqa.exe binary and assorted files in `./Qiqqa/bin/Release/`
- run a `bash` shell and `cd` to the `qiqqa-open-source` repository root directory if you haven't already :wink:
- in `bash`, run:

  ```
  ./build-installer.sh
  ```

  + At the end of which you should see a line reporting success, with a version number and a path where the `setup.exe` has been dropped.
    
    > That'll be in the `./Qiqqa.Build/Packages/` directory tree: it should match the `vNN-YYYYMMDD-HHMMSS` with the latest date. 

  + Presto! 🎉 


  


## Fixes & Additions to Qiqqa

[For a full list click here.](https://github.com/jimmejardine/qiqqa-open-source/issues?q=is%3Aissue+is%3Aclosed+milestone%3Av82)

- [#8](https://github.com/jimmejardine/qiqqa-open-source/issues/8)
- [#10](https://github.com/jimmejardine/qiqqa-open-source/issues/10)
- [#11](https://github.com/jimmejardine/qiqqa-open-source/issues/11)
- [#13](https://github.com/jimmejardine/qiqqa-open-source/issues/13)
- [#14](https://github.com/jimmejardine/qiqqa-open-source/issues/14)
- [#16](https://github.com/jimmejardine/qiqqa-open-source/issues/16)
- [#17](https://github.com/jimmejardine/qiqqa-open-source/issues/17)
- [#18](https://github.com/jimmejardine/qiqqa-open-source/issues/18)
- [#19](https://github.com/jimmejardine/qiqqa-open-source/issues/19)
- [#20](https://github.com/jimmejardine/qiqqa-open-source/issues/20)
- and many more. (See the Qiqqa issues list [here](https://github.com/jimmejardine/qiqqa-open-source/issues?utf8=%E2%9C%93&q=is%3Aissue+milestone%3Av82).)
