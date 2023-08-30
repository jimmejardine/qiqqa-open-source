# Highlighting matches in SOLR

Previously, Qiqqa did this by itself, after a match-on-this-document signal came back from Lucene.NET.

Now that we move to using SOLR, let's have that one do the highlighting if possible, so we don't have to parse search queries *twice* in completely different code bases. Thank you!

## What we need is [right here](https://dbmdz.github.io/solr-ocrhighlighting/)

https://dbmdz.github.io/solr-ocrhighlighting/

Also note his [ChangeLog](https://dbmdz.github.io/solr-ocrhighlighting/changes/), where he points at a [github issue #147](https://github.com/dbmdz/solr-ocrhighlighting/issues/147#issuecomment-800452975) that revolves around whitespace processing. Important stuff to get right ourselves!

