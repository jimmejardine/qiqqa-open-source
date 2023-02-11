
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
- When you hit key \[F5] you should now be able to execute, debug and otherwise work with your own Qiqqa build.


Read the [[Misc notes for developers]] and browse the repository for more information on the internals and operation of Qiqqa and the related tools.


