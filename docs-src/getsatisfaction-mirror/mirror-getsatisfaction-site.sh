#! /bin/bash
#
# sources:
# - `wget --help`
# - https://www.guyrutenberg.com/2014/05/02/make-offline-mirror-of-a-site-using-wget/
# - https://gist.github.com/simonw/27e810771137408fd7834ad153750c41

if ! test -d site ; then
	mkdir site
fi
cd site
wget -e robots=off --mirror -nc -v --convert-links -p -np -nH --cut-dirs=0 --adjust-extension --rejected-log=rejected-URLs.log --convert-links  https://getsatisfaction.com/qiqqa

