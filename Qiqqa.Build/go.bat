@echo off
cls
echo:
echo Have you remembered to:
echo:
echo 1.) Set the Qiqqa.csproj major version (which sets the client version).
echo 2.) Update ClientVersion.xml Release Notes (and optionally the ObsoleteFromVersion and CompliantFromVersion settings)

echo:
echo:

pause

set version.client=
if NOT "%1%" == "-debug" (
    if NOT "%1%" == "" (
        set version.client=-D:version.client=%1
    )
)

set debug=
if "%1%" == "-debug"  (
set debug=-debug
)
if "%2%" == "-debug" (
set debug=-debug
)

.\nant\bin\nant.exe -buildfile:.\nant\default.build %version.client% %debug%

pause
