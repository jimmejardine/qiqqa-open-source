
# How to build Qiqqa from source


<toc/>


## Prerequisites :: setting up your development environment

Qiqqa is largely written in C#, while some "backend/utility/processing" parts are done in C and C++, plus a few odds and ends done in JavaScript.

Qiqqa is built using a Microsoft Developer Studio 'solution file' with multiple 'project files', one for each executable, library or DLL that's part of the Qiqqa project.

Qiqqa **software releases** are created using the [Inno Setup System](https://jrsoftware.org/isinfo.php), which packs the relevant compiled binaries and other files into a single `setup.exe` installer. The `setup.exe` is produced using `bash` shell scripts, as are a few other bits & pieces in the Qiqqa software.

If you want to build Qiqqa from scratch yourself and/or wish to participate in its Open Source development process, which is centered around the [Qiqqa GitHub repository](https://github.com/jimmejardine/qiqqa-open-source) and GitHub website, then you first need to install these tools to make sure you don't run into nasty surprises where the compile/build/packaging processes expect certain tools to be present:

- [Microsoft Developer Studio 2019 ![](assets/visualstudio-help-me-choose.png)](https://visualstudio.microsoft.com/)
- [The .NET Framework SDK for Visual Studio](https://dotnet.microsoft.com/download/visual-studio-sdks), version 4.8 or later. (Migration to .NET Core is considered, but not decided on yet.)
- `git` : https://git-scm.com/downloads
- `bash` UNIX shell - this one is included in the git-for-windows install linked above: https://git-scm.com/downloads
- `node` + `npm` : NodeJS v12 or later - since we have some JavaScript in Qiqqa and build the documentation site and other bits & pieces using Node.

  While there is a basic NodeJS installer, we advise you to install `nvm` so you can switch Node versions on your development machine. This comes in handy when you do other projects with Node / JavaScript too:
  + https://docs.microsoft.com/en-us/windows/nodejs/setup-on-windows : a nice write-up by the folks at Microsoft how to setup `nvm` and the rest.
  + https://nodejs.org/en/download/ : the basic NodeJS installer. 
  
    **DOES NOT include `nvm` and DOES NOT work well with `nvm` when you install that one later on!**
    
    
  You are strongly advised to [use the Windows-nvm approach described above](https://docs.microsoft.com/en-us/windows/nodejs/setup-on-windows). Using that approach you also can easily switch Node versions when you need to while working on other projects *outside* Qiqqa.
  
  Make sure to install and activate the latest node v12 release via `nvm`, e.g. 
  
  ```bash
  nvm install 12.18.3
  nvm use 12.18.3
  ```
  
  Do *not* forget the `nvm use <version>` command there as that one *activates* the installed node version!
  
    
    

### Extra bits you might need later

- MSYS2 - or another way to get at tools like `wget` et al on Windows. Not mandatory and certainly not advised to install on a first try / first meet date with Qiqqa sources, unless you are already familiar with this material.
  + https://www.msys2.org/#installation - MSYS2 installer
  + https://docs.microsoft.com/en-us/windows/wsl/install-win10 - Microsoft's new approach: Windows Subsystem for Linux a.k.a. WSL or WSL2. Use this if you want/need something stronger than MSYS2.







## Building Qiqqa from scratch using Visual Studio

- Make sure all **git submodules** have been fetched to your local disk/machine: run this command in the Qiqqa repository root directory:
   
       git submodule update --init --recursive
       
   which will load `MuPDF`, `tesseract` and several other git repositories onto your local storage.
- Double-click the `Qiqqa.sln` file to open it in Visual Studio.
- Wait for Visual Studio to install the required NuGet packages.
  > You may need to forcibly re-install NuGet packages when you upgrade Visual Studio later on. Then you find https://stackoverflow.com/questions/38074578/nuget-has-problems-with-missing-packages-how-to-restore very useful!
- From the Build menu, pick "Rebuild all".
   + This will take *quite a while* as this will instruct Visual Studio to go and build all referenced projects (libraries, etc. used by Qiqqa) from scratch.
- Select the `Qiqqa` project in the right-hand panel of Visual Studio, right-click and choose "Set as Active Project".
- When you hit key <key>F5</key> you should now be able to execute, debug and otherwise work with your own Qiqqa build.


Read the [[Misc notes for developers]] and browse the repository for more information on the internals and operation of Qiqqa and the related tools.



------------

## Update: Building Qiqqa<sup>NG</sup>/MuPDF with Microsoft Visual Studio 2022

A system fatality -- I have a track record record with Windows that's consistent for about 20 years! Every install lasts about 1 year, where it gets crufty either slow or fast, but nevertheless fails dramatically to work after about 9 - 15 months (median ~ 12 month) and a full OS + applications' software re-install is called for.
Which is a great moment to check install / build assumptions... 😅😇😱

In order to build the present backend code (work in progress) a.k.a. Qiqqa<sup>NG</sup>/MuPDF/tesseract, you'll need these:

1. **nvm for windows** -- NodeJS manager. Use that one to install latest stable NodeJS.
2. **git for windows** -- duh!
3. **TortoiseGit** -- gosh!
4. **Scooter Software Beyond Compare** -- if you're like me and often do `git merge` operations and alike: it's far better at resolving merge conflicts then anything else I've met during my entire career! 👍
5. **Microsoft Visual Studio** -- the full Monty. Turns out enabling all I wish/want in there gobbles nearly 50GB (*sic*) install space today. *Meh.*
6. **NASM for windows** -- grab that one via **VSNASM**, by the way.
7. **VSNASM** -- Visual Studio plugin for nasm support -- if you don't have it, the build will fail due to errors reporting crap in `nasm.props`. 
   - [GitHub - ShiftMediaProject/VSNASM: Provides Visual Studio integration for the NASM assembler.](https://github.com/ShiftMediaProject/VSNASM)
   - [NASM](https://nasm.us/)
8. **Python 3** -- not needed immediately, but there's some scripting stuff in there that may need your python to be up and running when you wish to regenerate some coding tables, etc. in various libraries/submodules.


### Note on installing VSNASM

By the way: you'll get errors from their install bat script, unless you do this:

- you have MSVC2022 installed
- you have installed NASM-for-Windows in the default directory **by running its setup as Administrator** and installed "for all users" that way -- the NASM installer mentions that you do this if you want to install for "All Users on this machine" and I do / I did.
- *specifically* open Visual Studio 2022 -> **Developer Command Prompt for VS 2022** and **run as Administrator** -- I have a tendency to run everything from a git-for-win bash console and that's not going to cut it for this one, even if we switch to `cmd` from in there; you MUST use the **Developer Command Prompt for VS 2022**!
- `cd` to the VSNASM directory, i.e. where you unpacked the VSNASM zip from github.
- run VSNASM's `install_script.bat`
- *WIN*:

```
**********************************************************************
** Visual Studio 2022 Developer Command Prompt v17.9.4
** Copyright (c) 2022 Microsoft Corporation
**********************************************************************

C:\Windows\System32>cd "C:\Users\Ger\Downloads\warez\VSNASM-master"

C:\Users\Ger\Downloads\warez\VSNASM-master>set NASMPATH="C:\Program Files\NASM"

C:\Users\Ger\Downloads\warez\VSNASM-master>install_script.bat
Detected 64 bit system...
Existing Visual Studio 2022 environment detected...
Installing build customisations...
Checking for existing NASM in NASMPATH...
Using existing NASM binary from "C:\Program Files\NASM"...
Finished Successfully
Press any key to continue . . .

C:\Users\Ger\Downloads\warez\VSNASM-master>
```

Note: do NOT forget to set the `NASMPATH` environment variable as indicated or the MSVC build process will fail anyway, this time with an error report like: "`[...]"C:\Program Files\Microsoft Visual Studio\2022\Community\VC\nasm.exe"[...] process failed with exit code 1."` due to the `nasm.exe` not having been copied there by the install batch file. Simply re-running the `install_script.bat` batch file again *after* having set up `NASMPATH` as indicated in the code above fixes this remaining issue.


### Note on Node/JS tools/scripts

The project uses several Node/JS scripts to generate/update/patch the MSVC project files, update the `owemdjee` README documentation, etc.
For these to work, `node` must be installed on the development machine, preferably using `nvm` / nvm for Windows, as you can then easily switch between installed node releases for testing further JavaScript-based code & projects, such as website generators.
However, installing `node` does not suffice: we use several npm packages in these gen/patch scripts! The quickest way to get these packages installed *globally* in a way that's compatible and easy to use for every project on the developer machine, fetch the git utility scripts in the github repo: https://github.com/GerHobbelt/developer-utility-commands (suggestion: place these in `/z/tools/` f.e., where `/z/` is our local drive's development tree root directory).
Then run the `npm-install-global-required-packages.sh` bash script in there (using bash-for-windows, which is installed as part of git-for-windows) to install all npm packages that we need for this Qiqqa development project, e.g. the `json` commandline tool.


