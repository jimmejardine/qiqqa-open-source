#! /bin/bash
#

for f in GerHobbelt jimmejardine ; do 
	echo $f 
	git push --all git@github.com:$f/qiqqa-open-source.git
done
