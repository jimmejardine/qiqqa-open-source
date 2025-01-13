#! /bin/bash
#

cat <<EOF

>>> Pushing our work to both remotes...

EOF
for f in GerHobbelt jimmejardine ; do 
    echo ""
    echo "::REPO: git@github.com:$f/qiqqa-open-source.git"
    git push --all --follow-tags git@github.com:$f/qiqqa-open-source.git                                         2>&1
    git push --tags              git@github.com:$f/qiqqa-open-source.git                                         2>&1
done

