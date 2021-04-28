REM args: targetdir build qiqqaProjectDir

echo "### Copying %2 MuPDF built EXEcutables and DLLs to %1"
REM echo "ARGS: %1 %2 %3"

cd "%3"
cd ..\MuPDF\platform\win32
rem go with the 64-bit builds.
cd "%2-64"

echo "MuPDF source directory for the binaries:"
cd

mkdir "%1\MuPDF"

REM update the files in the target directory:
robocopy . "%1\MuPDF" mudraw.exe mupdf*.exe mutool.exe curl.exe *.dll /PURGE /XO /NP

echo "### Copying done! ################################################"
