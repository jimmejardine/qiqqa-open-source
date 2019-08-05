#! /bin/bash
#
# This is a clone of the Qiqqa.Build/go.bat Win32/CMD batch file, suitable for bash on Windows (part of Git-for-Windows)
# 

cd Qiqqa.Build

cat <<EOF

  Have you remembered to:

  1.) Set the Qiqqa.csproj major version (which sets the client version).
  2.) Update ClientVersion.xml Release Notes (and optionally the ObsoleteFromVersion and CompliantFromVersion settings)


EOF

read -p "Press ENTER to continue..." -n 1


QIQQA_CLIENT=
if [ "$1" != "-debug" ] ; then
	if [ -n "$1" ] ; then
		set -x
		QIQQA_CLIENT=-D:version.client=$1
		set +x
		shift
    fi
fi

QIQQA_DEBUG=
if [ "$1" == "-debug" ] ; then
	set -x
	QIQQA_DEBUG=-debug
	set +x
fi

set -x
./nant/bin/nant.exe -buildfile:./nant/default.build $QIQQA_CLIENT $QIQQA_DEBUG
set +x

cd ..

#
# read -p "Press ENTER to continue..." -n 1
#

