
![logo](../../blob/master/icons/Application/Qiqqa.png) 
# *Developer Info*

## Development Software Requirements

### Required tools

- OS: Windows 10
- [Visual Studio 2019](https://visualstudio.microsoft.com/downloads/) with `.NET desktop development` workload.
- [git](https://git-scm.com/downloads) or [git for Windows](https://gitforwindows.org/)
- `bash` (comes as part of git packages)
- `node` (NodeJS) & `npm` (NodeJS Package Manager) —— preferrably installed via [nvm](https://github.com/coreybutler/nvm-windows)


### Suggested tools

- [TortoiseGIT](https://tortoisegit.org/) — UI for git
- [Beyond Compare](https://scootersoftware.com/) — visual comparison tool, invoked by the Unit and System Tests [ApprovalTests framework](https://github.com/approvals/ApprovalTests.Net).
- [Syncfusion Essential Studio 17](https://www.syncfusion.com/wpf-ui-controls) — the required redistributable binaries are already available in `/libs/`, needed only if you wish to upgrade SyncFusion libraries.
    > Commercial Qiqqa uses SyncFusion v14 and the upgrade to v17 fixed one very annoying bug in Qiqqa's handling of (broken!) PDF files which are already part of your Qiqqa libraries. SyncFusion v17 isn't the bee knees in PDF processing either, so another upgrade might be in order one day...



## Building Qiqqa From Source

### 1. Get the source code

To fetch whole repository find `Git Bash` from Windows' start menu  and run:

    git clone https://github.com/jimmejardine/qiqqa-open-source.git
    cd qiqqa-open-source

Or alternatively just download the source code .zip file.


### 2. Enable Long Filename Support in `git`

While still in the qiqqa-open-source folder, run these commands:

    git config core.longpaths true
    git reset --hard

> This is needed because the repository includes "Windows Long Filenames" (at least since commit [0cf15c0d](https://github.com/GerHobbelt/qiqqa-open-source/commit/0cf15c0d4d9377e80ddafd3063cbef038701bb3e)). See [StackOverflow](https://stackoverflow.com/questions/22575662/filename-too-long-in-git-for-windows).

### 3. Install required tools with `npm`

While still in the same folder, now run:

    npm i

> This will set up the Node/npm package environment for the JavaScript/NodeJS based `bash` scripts which help build Qiqqa. See below.


### 4. Build Qiqqa

- Open `qiqqa-open-source/Qiqqa.sln` file to open the project with Visual Studio 2019.

- Choose `Release+SETUP` solution on top toolbar *(there are three different solutions: Debug, Release, Release+SETUP).*

- To build Qiqqa, in `Build` menu click on `Build Solution` or `Rebuild Solution`. This should take a while.

- Find `setup.exe` file in folder `qiqqa-open-source/Qiqqa.Build/Packages/v82 - 20200426-212303` or similar. Done.

	> The number will be `vNN-YYYYMMDD-HHMMSS` version+date of the build. 
The exact path is also listed at the end of the `Output` build log panel in Visual Studio when the `Rebuild Solution` action has completed.
	> In folder `qiqqa-open-source/Qiqqa/bin/Release/` folder there will be `Qiqqa.exe` and other files if you don't need the installer.


## Preparing the release

There are several scripts which help to prepare the release.

- `npm run refresh-data` is a helper script which edits the Unit Test C# source code to ensure that all test data reference files ('fixtures') have been included in the test set.

  > This one comes in handy if, for instance, you add a bunch of BibTeX test files which should be parse-tested or otherwise.
  This script will find those (when you have placed them in the `TestData/...` directory tree) and add comments and code lines in the appropriate Unit Test C# source files to ensure the new files show up in the tests.

- `npm run syncver` will synchronize all Qiqqa parts to have the same version number info.
 
  > The master *major* version is obtained from the `package.json` file — Qiqqa has historically used only the *major* version number to identify a Qiqqa version. Experimental prereleases, etc. can be identified in Qiqqa Open Source by watching the full version number as it is shown during Qiqqa start up and elsewhere in the application.

- `npm run bump` : this will *bump* the Qiqqa major version number by +1, i.e. this command should be run after every official release as the next time a newer (higher) Qiqqa version should be reported by the binaries to be built.

- `./update_CHANGELOG.sh` : grabs the `git log` output and dumps that into [`CHANGELOG_full.md`](./CHANGELOG_full.md)
  > You can later postprocess CHANGELOG_full.md file using Beyond Compare to produce a readable [`CHANGELOG.md`](./CHANGELOG.md) file.

- `./superclean.sh` : this script cleans up everything that has been written by Visual Studio in the devtree.  

  > If you run this script and restart Visual Studio afterwards, you **must** re-configure Visual Studio configuration. See that the active "Solution" is "Release+SETUP" and in dialog from menu `Build` > `Configuration Manager` for solutions nameed "Debug" and "Release" two projects "ClickOnceUninstaller" and "QiqqaPackager" would be unchecked.

- `./build_installer.sh` : the alternative to building the Qiqqa.Packager project and the older way to build a Qiqqa `setup.exe` installer.
