 ![logo](../../blob/master/icons/Application/Qiqqa.png) 
# Qiqqa (Open Source)

The open-sourced version of the award-winning Qiqqa research management tool for Windows.

This version includes EVERY feature available in Qiqqa, including Premium and Premium+.

Unfortunately we have had to remove the web cloud sync ability as that is the one area that involves storage costs.  **Users are encouraged to migrate their Web Libraries into Intranet libraries, and use Google Drive or Dropbox as the 'sync point' for those libraries.**


## Qiqqa Open Source Software Releases

- Qiqqa Open Source software installer releases are available in the mainstream repository at https://github.com/jimmejardine/qiqqa-open-source/releases. Here's where you'll find Qiqqa v80 and later.
- Qiqqa Commercial software installer releases are available at http://www.qiqqa.com/Download at least up to / into the year 2020 A.D. (Note: Qiqqa's SSL cert seems to have expired, so your browser will warn you about the website being *unsafe*.)
  + Those very same releases are also available at GitHub in the bleeding edge development repo at https://github.com/GerHobbelt/qiqqa-open-source/tree/master/Qiqqa-Software-Installer-Releases -- make sure to download these "RAW" or they won't work at all.
- Qiqqa Open Source **Experimental Build** software installer releases are available at the bleeding edge development repo at https://github.com/GerHobbelt/qiqqa-open-source/releases -- **DO NOTE** that these releases are only lightly tested and may include grave bugs. Meanwhile you also get an early glance at the state of affairs in Qiqqa Features and Fixes County, which might be enough of an incentive for you to try one of these. *Living on the edge* and all that...



## Fixes & Additions to Qiqqa (in this fork)

[For a full fixed issue list click here.](https://github.com/jimmejardine/qiqqa-open-source/issues?q=is%3Aissue+is%3Aclosed+milestone%3Av82)

See also [CHANGELOG](../../blob/master/CHANGELOG.md) and it's big brother [CHANGELOG-full](../../blob/master/CHANGELOG_full.md).

While I have joined as a collaborator in the qiqqa-open-source mainline repository, the bleeding edge of my work on Qiqqa can be observed here.



## Qiqqa Collaborators & Developers Wanted

You'll need to be a little technically experienced in the .NET world to get a build up and running.
 
Would anyone who is interested in contributing towards this repository please contact @jimmejardine...


## For Developers: Building Qiqqa From Source



### General Notes

As the repository includes "Windows Long Filenames" (at least since commit [0cf15c0d](https://github.com/GerHobbelt/qiqqa-open-source/commit/0cf15c0d4d9377e80ddafd3063cbef038701bb3e)) you MUST run this `git` configuration
command before working on the qiqqa repository:

    git config core.longpaths true

Alternatively you may try to run

    git config --system core.longpaths true

to enable this feature for all your git repositories, but that command will probably fail unless you have Administrator
right in your active shell. (See also [StackOverflow](https://stackoverflow.com/questions/22575662/filename-too-long-in-git-for-windows).)

After you have succesfully changed your git configuration, it might be a very good idea to run

    git reset --hard

next to ensure all files in the project are now correctly extracted from the git repository.



>
> ### Visual Studio 2017 (Original build environment, now *OBSOLETED/UNSUPPORTED*)
> 
> You will need to download Visual Studio 2017 Community Edition and Syncfusion Essential Studio 14.1.0.41 (you can get a free evaluation key from syncfusion.com - works for single developers for private/free projects).
>   
> Then simply go to `./Qiqqa.Build/` and run `go.bat` to build the latest version into the `Qiqqa.Build/Packages` directory.
> 
> (Alternatively, when using `bash` on Windows, you may execute `./build-installer.sh` from the repository base directory to accomplish the same as when you'ld have executed `go.bat`.)
> 


### Visual Studio 2019 (NEW, SUPPORTED build environment)

As above, but instead you'll need to obtain and install Syncfusion Essential Studio 17 as this fixes at least **one very annoying bug** in Qiqqa's handling of (broken!) PDF files which are already part of your Qiqqa librarie(s).

#### How to build the `setup.exe` installer

- Make sure you have these installed (it may work with other tools, but this is what I (@GerHobbelt) use):
  + Microsoft Visual Studio 2019 Professional (IDE)
  + [Git For Windows](https://gitforwindows.org/) - which includes `bash`
- Open `Qiqqa.sln` in MSVS
- Select `Release+SETUP` as Solution Configuration (instead of just `Debug` or `Release`)
- Build > Rebuild Solution
  + When this is done, you should have a Qiqqa.exe binary and assorted files in `./Qiqqa/bin/Release/` 
  + and a matching `setup.exe` Qiqqa installer in a `vNN-YYYYMMDD-HHMMSS` **version+date**-stamped directory in `./Qiqqa.Build/Packages/` 
  
    > The exact path is also listed at the end of the `Output` build log panel in MSVS when the 'Rebuild Solution' action has completed.
- Presto! 🎉 


  
