# [[../Links to Stuff To Look At|Stuff To Look At]]: FTS / Full Text Search Engines

index.rst
https://xapian.org/docs/

.net - Indexing .PDF, .XLS, .DOC, .PPT using Lucene.NET - Stack Overflow
https://stackoverflow.com/questions/4905271/indexing-pdf-xls-doc-ppt-using-lucene-net

Welcome to the Lucene.Net website! | Apache Lucene.NET 4.8.0
https://lucenenet.apache.org/

How to compile and use Xapian on Windows with C# - CodeProject
https://www.codeproject.com/Articles/71593/How-to-compile-and-use-Xapian-on-Windows-with-C

https://gist.github.com/arfprogrammer/a0dea5d4003beb7e94bb46c7610faa1b : Elastic vs sphinx vs SOLR

https://ui.adsabs.harvard.edu/ - an advanced example of SOLR in actual use. (Their software is at https://github.com/adsabs/bumblebee )

[Lucene nightly benchmarks (apache.org)](https://home.apache.org/~mikemccand/lucenebench/) - Lucene nightly benchmarks :: each night, an [automated Python tool](https://code.google.com/a/apache-extras.org/p/luceneutil/source/browse/src/python/nightlyBench.py) checks out the Lucene/Solr trunk source code and runs multiple benchmarks: indexing the entire [Wikipedia English export](http://en.wikipedia.org/wiki/Wikipedia:Database_download) three times (with different settings / document sizes); running a near-real-time latency test; running a set of "hardish" auto-generated queries and tasks. The tests take around 2.5 hours to run, and the results are verified against the previous run and then added to the graphs.

[Solr Query Performance benchmarking (narkive.com)](https://solr-user.lucene.apache.narkive.com/m3lrkDfp/solr-query-performance-benchmarking) - comments on a benchmark rig. The benchmark is useless, but the comments are (I expect) very useful when setting up our own SOLR rigs. Hence: take note.

[meilisearch/meilisearch: Powerful, fast, and an easy to use search engine (github.com)](https://github.com/meilisearch/meilisearch) - Rust-based search engine. Nice, but we'll stick with SOLR: more users.

[opensearch-project/OpenSearch: 🔎 Open source distributed and RESTful search engine. (github.com)](https://github.com/opensearch-project/OpenSearch) - This is what ElasticSearch once was before they got creative with their licenses to appease the VC gods (if that's the true reason; it still makes the most sense to me).

[apache/lucene-solr: Apache Lucene and Solr open-source search software (github.com)](https://github.com/apache/lucene-solr) - just FYI

[luceneplusplus/LucenePlusPlus: Lucene++ is an up to date C++ port of the popular Java Lucene library, a high-performance, full-featured text search engine. (github.com)](https://github.com/luceneplusplus/LucenePlusPlus) - you can forget about this one, just like Lucene.NET: lagging behind the curve so far it's over the horizon.

https://tantivy-search.github.io/bench/ - benchmark of pisa, tantivy and lucene. Interesting stuff and it teases me to have a look at those. Goes with this: [quickwit-oss/search-benchmark-game: Search engine benchmark (Tantivy, Lucene, PISA, ...) (github.com)](https://github.com/quickwit-oss/search-benchmark-game)

[pisa-engine/pisa: PISA: Performant Indexes and Search for Academia (github.com)](https://github.com/pisa-engine/pisa) - C++ stuff. (**Not an option, as the entire search index is supposed to fit in memory, according to their documentation.** *Pity.*)








