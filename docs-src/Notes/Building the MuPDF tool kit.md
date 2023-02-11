# Building the MuPDF/PDF/tesseract C/C++ tool kit

See also the Obsidian Vault at [the MuPDF submodule/project](file://../../MuPDF/docs/Notes/DEVELOPERS-README.md)

## TL;DR: `./MuPDF/platforms/`

- For Windows, the MSVC2019 (Visual Studio 2019) Solution (`.sln`) file containing all the relevant project files is at `./MuPDF/platforms/win32/mupdf.sln`. Load that one into a separate Visual Studio instance and Build All in either Debug or Release mode.

### WARNING (july/2021): build 32-bit binaries for use with Qiqqa

Until we have migrated Qiqqa entirely to 64-bit (or away from WPF, whichever comes first), you're stuck with using the 32-bit MuPDF/Tesseract tools as those get invoked by Qiqqa and Windows prevents 32bit applications from invoking 64bit executables.

See also [[Where do my Qiqqa binaries look for the MuPDF+tesseract+misc tools]]?