This folder builds a production release of Qiqqa.

You need to have installed:
- Visual Studio 2017 Community Edition
- Syncfusion Essential Studio 14.1.0.41 - you can get a free evaluation key from syncfusion - works for single developers for private/free projects



Files:
- `go.bat`: kicks off the `nant` build, can pass version and/or debug for `nant`
- `ClientVersion.xml`: the basis for the file deployed to the server, and contains the version number / download location / etc.  

The build fills in the `LastestVersion` from the Qiqqa.exe assembly version, you should fill in the rest with the latest release details.

Folders:
-`nant`: does the `nant` build of the Qiqqa code, ties everything together
-`Packaging`: creates the installer exe from the compiled code (Inno Setup files)
-`Packages`: the latest and historical packages that have been built