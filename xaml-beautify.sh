#! /bin/bash
#

find . -type f -iname '*.xaml' | xargs node Qiqqa.Build/beautify-xaml.js

