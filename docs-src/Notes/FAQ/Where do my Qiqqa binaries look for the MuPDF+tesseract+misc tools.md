# Where do my Qiqqa binaries look for the MuPDF+tesseract+misc tools?

The Qiqqa binary looks for these in the `MuPDF/` subdirectory relative to its own executable's path.

Almost all tooling is invoked through the `MuPDF/mutool.exe` tool -- apart from legacy code, which invokes different binaries, e.g. `pdfdraw.exe`: that one will be obsoleted in due course.

## For Qiqqa Developers:

The Qiqqa  and QiqqaOCR MSVC projects have custom code in their Build Events (project > Properties > Build Events) in the pre-build section:

```
$(SolutionDir)Qiqqa.Build\copy_mupdf_binaries.cmd $(TargetDir) $(ConfigurationName) $(ProjectDir)
```

which invokes that Windows CMD script to run `robocopy` to help us by copying the appropriate MuPDF binaries to the .NET binary's directory, i.e. the Project Output directory.

Note that the CMD script does the following:

- When invoked in Debug Build mode, it will first look in the 32-bit Debug Build Output Directory of the MuPDF solution (see [[Building the MuPDF tool kit]]) and grab the compiled executables from there if they're fresh.

  Next, the script will look in the 32-bit Release Build Output Directory of that solution and grab the executables available there if they're *fresher* than the Debug Build's.
  
  This is done to ensure that your Debug Build uses the latest (**32-bit**) MuPDF builds at all times.
- When invoked in Release Build mode, it will just look in the 32-bit Release Build Output Directory of the MuPDF solution (see [[Building the MuPDF tool kit#WARNING jul 2021 build 32-bit binaries for use with Qiqqa]]) and grab the compiled executables from there if they're fresh.

  This is done to ensure that a Release Build only uses other, fitting, Release Build executables and cannot be *polluted* with Debug Build binaries. After all, you're working with a Release Build here, which would quite properly be used for public release when your final tests have passed with positive results!
  We don't want to burden users with (slow) Debug executables, now do we?!
  
