#! /bin/bash

pushd $( dirname $0 )  

echo "Cleaning docs-gen/Notes/ output directory tree..."
rm -rf ./docs-gen/Notes/
mkdir ./docs-gen/Notes/

echo "Re-generating the Obsidian notes into the docs-gen/Notes/ directory..."
./obsidian-export docs-src/Notes/ ./docs-gen/Notes/

popd

