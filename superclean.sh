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

# Nuke anything that's part of the MSVS build process
find . -maxdepth 2 -type d  -iname obj -exec rm -rf "{}" +
find . -maxdepth 2 -type d  -iname bin -exec rm -rf "{}" +

# and nuke the MSVS local config cache
rm -rf .vs
