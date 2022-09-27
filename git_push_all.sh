#! /bin/bash
#

cat <<EOF

>>> Pushing our work to both remotes...

EOF
for f in GerHobbelt jimmejardine ; do 
	echo $f 
	git push --all git@github.com:$f/qiqqa-open-source.git
done

# fetch all remote work as an afterthought
cat <<EOF


>>> Fetching all remote work...

EOF
git fetch --all 
