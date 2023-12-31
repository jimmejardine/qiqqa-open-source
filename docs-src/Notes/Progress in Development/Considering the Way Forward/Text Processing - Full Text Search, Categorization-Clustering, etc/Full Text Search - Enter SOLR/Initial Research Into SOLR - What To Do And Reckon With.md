# Initial Research Into SOLR :: What To Do And Reckon With

We forget about Lucene.NET. We'll have to "re-invent the wheel" using SOLR instead. Also check [[Full Text Search Engines]] and [[Full-Text Search Engines]]

## (Expected) Benefits

- SOLR is cross-platform without any fuss, so that's that bit covered for this part of Qiqqa functionalities, at least.
- SOLR is its own application and is supported by a large, long-lived organization (Apache) and has a large user base: hence we may expect this to be a reliable, well-behaved piece of software without us having to spend *development time* (compiling, debugging, etc.) on it.
- SOLR covers the entire feature set of our (antiquated) Lucene.Net library and the way it was used by Qiqqa, so we "only have to find out" (learning curve!) how to accomplish what we need in SOLR.
- SOLR has added features, which are expected to be useful.
	- This includes *Streams + {Tokenizers/Analyzers/Filters[^1]}*, which **look like they might help us** replace the (somewhat crufty) LDA .NET code for keyword / topic (auto-)discovery with something that's part SOLR, part .NET code? [^And hopefully **not** subject to the disastrous out-of-memory crashes Qiqqa currently suffers from when applying this technique in larger libraries/documents.] [^This is a feature that's *separate* from the FTS (Full Text Search) abilities currently used via Lucene.Net; I also noted that SOLR 8 mentions [TF\*IDF](https://en.wikipedia.org/wiki/Tf%E2%80%93idf) in their Analyzers/Filters [documentation](https://solr.apache.org/guide/8_8/term-vectors.html)



xxx




[^1]: [SOLR: Understanding Analyzers, Tokenizers, and Filters](https://solr.apache.org/guide/8_8/understanding-analyzers-tokenizers-and-filters.html)
