# Using and Tuning SOLR as a better replacement for current (old) Lucene.NET

- https://solr.apache.org/guide/8_8/solr-tutorial.html
  + https://solr.apache.org/guide/8_8/package-manager.html#package-manager
  + https://solr.apache.org/guide/8_8/learning-to-rank.html
  + https://solr.apache.org/guide/8_8/highlighting.html
  + https://solr.apache.org/guide/8_8/post-tool.html#indexing-xml
  + https://solr.apache.org/guide/8_8/uploading-data-with-index-handlers.html
  + https://solr.apache.org/guide/8_8/updating-parts-of-documents.html
  + https://solr.apache.org/guide/8_8/detecting-languages-during-indexing.html
  + https://solr.apache.org/guide/8_8/reindexing.html#index-to-another-collection
    + https://solr.apache.org/guide/8_8/getting-started-with-solrcloud.html
    
    --> Reindexing in SolrCloud mode is not an option it seems, so that aliasing feature meantioned in this section will not be available to us? Pity.
    
  + https://solr.apache.org/guide/8_8/the-well-configured-solr-instance.html
  + https://solr.apache.org/guide/8_8/implicit-requesthandlers.html
  + https://solr.apache.org/guide/8_8/schemaless-mode.html#schemaless-mode
  + https://solr.apache.org/guide/8_8/overview-of-documents-fields-and-schema-design.html#overview-of-documents-fields-and-schema-design
  + https://solr.apache.org/guide/8_8/field-properties-by-use-case.html
  + https://solr.apache.org/guide/8_8/dynamic-fields.html
  + https://solr.apache.org/guide/8_8/other-schema-elements.html
  
    --> *uniqueKey* :: that would be our document hash, I suppose 😄
  + https://solr.apache.org/guide/8_8/schema-api.html#modify-the-schema
  + https://solr.apache.org/guide/8_8/docvalues.html#docvalues
  + https://solr.apache.org/guide/8_8/putting-the-pieces-together.html#working-with-text
  + https://sease.io/2020/03/docvalues-vs-stored-fields-apache-solr-features-and-performance-smackdown.html
  + https://sease.io/2021/04/lucenematchversion-in-apache-solr.html
  + https://sease.io/2021/01/offline-search-quality-evaluation-rated-ranking-evaluator-rre.html
    + https://github.com/SeaseLtd/rated-ranking-evaluator
  + https://sease.io/2021/06/drop-constant-features-a-real-world-ltr-scenario.html
    + https://sease.io/2020/12/a-learning-to-rank-project-on-a-daily-song-ranking-problem.html
  + https://opensourceconnections.com/blog/2017/02/24/what-is-learning-to-rank/
  + ...
  + https://lists.tartarus.org/pipermail/xapian-discuss/2021-April/009884.html (trouble with pdf text extraction by a xapian user, but still something to keep in mind while we work on the Mupdf+tesseract stuff)
  + ...


> ## How about other engines?
> 
> This is old (2004): https://paginas.fe.up.pt/~ei04073/Search_Engine_Comparison.pdf but things haven't changed much for the better I think. Xapian is still around, but we're going for max support = max user base tools here: SOLR it is. 


## Feeding the Beast

From http://makble.com/how-to-extract-text-from-pdf-and-post-into-solr :

> [...] if you use the default schema.xml. The field "content" is stored, means all the text extracted from the pdf file is stored in Solr document.
>
> ```
> <field name="content" type="text_general" indexed="false" stored="true" multiValued="true"/>
> ```
>
> This field is for search highlight , but I noticed that the query is slow if text is stored into Solr document. If you don't need the highlight, just turn it off, the query will be very fast.
>
> ```
> <field name="content" type="text_general" indexed="false" stored="false" multiValued="true"/>
> ```

---




## Tuning SOLR

- https://github.com/mitre/quaerite

  Quaerite: This project includes tools to help evaluate relevance ranking. This code has been tested with Solr 4.x, 7.x and 8.x, and ES 6.x and 7.x.

  This project is not intended to compete with existing relevance evaluation tools, such as Splainer, Quepid, Rated Ranking Evaluator, or Luigi's Box. Rather, this project was developed for use cases not currently covered by open source software packages. The author encourages collaboration among these projects.

- https://github.com/DiceTechJobs/RelevancyTuning

  Dice.com tutorial on using black box optimization algorithms to tune your Solr search engine configuration, by Simon Hughes ( Dice Data Scientist ). See 'Automated Relevancy Tuning using Black Box Optimization Algorithms.ipynb' for Jupyter Notebook tutorial on using black box optimization algorithms from sci-kit optimize to tune your solr config.
  
- https://github.com/o19s/splainer

  The sandbox that explains your search results for you so you don't have to go digging through explain debug! Paste in your Solr or Elasticsearch query URL and go. 

  Why?
  
  You're a search developer trying to tune search results with Solr or Elasticsearch. You're engaged in search relevancy work.

  You're probably stuck with the question of why? Why do search results come back in the order that they do? Solr and Elasticsearch exposes an explain syntax for you to try to explain search scoring. Unfortunately outside the simplest tasks, its a nightmare to read through. There are parsers like explain.solr.pl but they require a lot of manual copy/pasting of explain information to the tool.

  Splainer is different by being a sandbox. Paste in your Solr or Elasticsearch URL, query parameters and all. As you work with your query, changing parameters, Splainer shows you parsed and summarized explain information alongside your documents. Continue working and see how the search results change.

  Splainer forms the core of the open source tool Quepid that allows you to do this over multiple queries against expert-graded search results to track search changes over a longer period of time.
  
- https://github.com/o19s/quepid

  Quepid makes improving your app's search results a repeatable, reliable engineering process that the whole team can understand. It deals with three issues:

  - Our collaboration stinks Making holistic progress on search requires deep, cross-functional collaboration. Shooting emails or tracking search requirements in spreadsheets won't cut it.

  - Search testing is hard Search changes are cross-cutting: most changes will cause problems. Testing is difficult: you can't run hundreds of searches after every relevance change.

  - Iterations are slow Moving forward seems impossible. To avoid sliding backwards, progress is slow. Many simply give up on search, depriving users of the means to find critical information. 



