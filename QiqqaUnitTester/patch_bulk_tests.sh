#! /bin/bash
#
# Patch the bulk test source files to help keep those tests complete and correct while we work on them.
# 


pushd $(dirname $0)                                                                                     2> /dev/null  > /dev/null

# go to root of project
cd ..
PROJDIR=$( pwd )


# obtain the root path to the evil collection from the runsettings file:
BASEDIR=$( grep QiqqaEvilPDFCollectionBasePath ./QiqqaTests.runsettings | sed -e 's/.*value="\(.*\)".*/\1/' )
echo "Detected QiqqaEvilPDFCollectionBasePath base directory:  ${BASEDIR}"


echo "Collecting PDF files list..."
pushd "${BASEDIR}"                                                                                     2> /dev/null  > /dev/null
cd TestData/data/fixtures/PDF

find . -type f -iname '*.pdf' -printf '"%p"\n' > "${PROJDIR}/__bulk_pdf_filelist.txt"

popd                                                                                                    2> /dev/null  > /dev/null


echo "Processing bulk test C# source code file(s)..."
node QiqqaUnitTester/patch_bulk_tests.js -g ./__bulk_pdf_filelist.txt QiqqaUnitTester/PDFDocumentMetaInfoBulkTest.cs 

# nuke temporary scratch files:
rm __bulk_pdf_filelist.txt

echo "Done!"

popd                                                                                                    2> /dev/null  > /dev/null


