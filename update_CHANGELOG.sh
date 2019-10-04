#! /bin/bash
#

git log --date=short --pretty="%cd:::%nCommit %h%n- %B%n" dec531335.. > changelog.dump.tmp.txt
node ./Qiqqa.Build/munch_changelog.js > CHANGELOG_full.md
rm changelog.dump.tmp.txt
