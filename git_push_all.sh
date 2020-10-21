#! /bin/bash
#

for f in $( git remote | grep -e 'Hobbelt\|jardine$' ) ; do 
	echo $f 
	git push --all https://github.com/$f/qiqqa-open-source.git
done
