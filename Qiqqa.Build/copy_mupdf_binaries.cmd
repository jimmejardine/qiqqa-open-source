REM args: targetdir build qiqqaProjectDir

echo "### Copying %2 MuPDF built EXEcutables and DLLs to %1"
echo "### CopyMuPDFBinaries ARGS: %1 %2 %3"

cd "%3"
cd ..\MuPDF\platform\win32


IF "%2"=="Release" GOTO skipped_first_message
echo "First we check the MuPDF %2 Build environment:"
:skipped_first_message


rem go with the 64-bit builds.

rem cd "bin\%2-Unicode-64bit-x64"
rem cd "bin\Release-Unicode-64bit-x64"
rem cd "bin\%2-Unicode-32bit-x86"

cd "bin\%2-Unicode-32bit-x86"

echo "MuPDF source directory for the binaries:"
cd

mkdir "%1\MuPDF"

REM update the files in the target directory:
robocopy . "%1\MuPDF" *.exe *.dll /PURGE /XO /NP /XF mu-office-*.exe bin2coff.exe



IF "%2"=="Release" GOTO skipped_second_chance



echo "Next we check the MuPDF Release Build environment:"
cd "..\..\bin\Release-Unicode-32bit-x86"

echo "MuPDF source directory for the binaries:"
cd

mkdir "%1\MuPDF"

REM update the files in the target directory:
robocopy . "%1\MuPDF" *.exe *.dll /XO /NP /XF mu-office-*.exe bin2coff.exe



:skip_second_chance

echo "### Copying done! ################################################"
