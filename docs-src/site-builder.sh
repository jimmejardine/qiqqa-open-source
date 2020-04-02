#! /bin/bash

wd="$( pwd )";

pushd $(dirname $0)                                                                                     2> /dev/null  > /dev/null

echo "PWD= $( pwd )"
echo "ARGV= $@"

# go to root of project
cd ..

rootdir="$( pwd )";


getopts ":bdsch" opt
#echo opt+arg = "$opt$OPTARG"
case "$opt$OPTARG" in
b )
  echo "--- (re)build site ---"

  vuepress build docs-src
  prettier --write docs/

  node docs-src/site-builder.js

  echo done.
  ;;

d )
  echo "--- start VuePress dev server ---"

  vuepress dev docs-src

  echo done.
  ;;

s )
  echo "--- start live-server dev server ---"

  mkdir -p docs
  live-server .

  echo done.
  ;;

c )
  echo "--- clean website output/dist directory ---"

  rm -rf docs

  echo done.
  ;;

* )
  cat <<EOT
$0 [-b] [-d] [-s] [-c]

build & run vuepress-based documentation website QiQQa.ORG.

-b       : (re)build website from sources
-d       : run the vuepress dev server
-s       : run the live_server local dev webserver
-c       : clean the website output/dist directory

Note
----
  You MUST specify a commandline option to have this script execute
  *anything*. This is done to protect you from inadvertently executing
  this long-running script when all you wanted was see what it'd do.

EOT
  ;;
esac


popd                                                                                                    2> /dev/null  > /dev/null



