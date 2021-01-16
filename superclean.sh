#! /bin/bash
#
# Cleans out anything that's cached/stored by Microsoft Visual Studio.
# 
# Run this script to clean up your build environment to a *pristine* condition.
#
# Note: 
# All *local* MSVS settings will be lost. 
# Read DEVELOPER-INFO.md for what to do next.
# 

# Blow away NuPkg cache
rm -rf packages
rm -rf node_modules

# Nuke anything that's part of the MSVS build process
find . -maxdepth 5 -type d  -iname obj -a ! -ipath './libs/*' -a ! -ipath './Qiqqa.Build/nant/*' -a ! -ipath '*openjpeg/src/bin'                                   -exec rm -rf "{}" +
find . -maxdepth 5 -type d  -iname bin -a ! -ipath './libs/*' -a ! -ipath './Qiqqa.Build/nant/*' -a ! -ipath '*openjpeg/src/bin' -a ! -ipath '*research*'          -exec rm -rf "{}" +

# and nuke the MSVS local config cache
find . -maxdepth 5 -type d  -iname '.vs'                         -exec rm -rf "{}" +
