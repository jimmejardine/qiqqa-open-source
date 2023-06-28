# [[../Links to Stuff To Look At|Stuff To Look At]] - Keyword Extraction / Topic Extraction






## AimeeLee77/keyword_extraction: 利用Python实现中文文本关键词抽取，分别采用TF-IDF、TextRank、Word2Vec词聚类三种方法。
https://github.com/AimeeLee77/keyword_extraction

## bigzhao/Keyword_Extraction: 神策杯2018高校算法大师赛（中文关键词提取）第二名代码方案
https://github.com/bigzhao/Keyword_Extraction

## aneesha/RAKE: A python implementation of the Rapid Automatic Keyword Extraction
https://github.com/aneesha/RAKE

## LIAAD/yake: Single-document unsupervised keyword extraction
https://github.com/LIAAD/yake

Unsupervised Approach for Automatic Keyword Extraction using Text Features.

YAKE! is a light-weight unsupervised automatic keyword extraction method which rests on text statistical features extracted from single documents to select the most important keywords of a text. Our system does not need to be trained on a particular set of documents, neither it depends on dictionaries, external-corpus, size of the text, language or domain. To demonstrate the merits and the significance of our proposal, we compare it against ten state-of-the-art unsupervised approaches (TF.IDF, KP-Miner, RAKE, TextRank, SingleRank, ExpandRank, TopicRank, TopicalPageRank, PositionRank and MultipartiteRank), and one supervised method (KEA). Experimental results carried out on top of twenty datasets (see Benchmark section below) show that our methods significantly outperform state-of-the-art methods under a number of collections of different sizes, languages or domains.

### Main Features

* Unsupervised approach
* Corpus-Independent
* Domain and Language Independent
* Single-Document

### Benchmark

YAKE!, generically outperforms, statistical methods [tf.idf (in 100% of the datasets), kp-miner (in 55%) and rake (in 100%)], state-of-the-art graph-based methods [TextRank (in 100% of the datasets), SingleRank (in 90%), TopicRank (in 70%), TopicalPageRank (in 90%), PositionRank (in 90%), MultipartiteRank (in 75%) and ExpandRank (in 100%)] and supervised learning methods [KEA (in 70% of the datasets)] across different datasets, languages and domains. The results listed in the table refer to F1 at 10 scores. Bold face marks the current best results for that specific dataset. The column "Method" cites the work of the previous (or current) best method (depending where the bold face is found). The interested reader should refer to [__this table__](https://github.com/LIAAD/yake/blob/master/docs/YAKEvsBaselines.jpg) in order to see a detailed comparison between YAKE and all the state-of-the-art methods.


## boudinfl/pke: Python Keyphrase Extraction module
https://github.com/boudinfl/pke

pke is an open source python-based keyphrase extraction toolkit. It provides an end-to-end keyphrase extraction pipeline in which each component can be easily modified or extended to develop new models. pke also allows for easy benchmarking of state-of-the-art keyphrase extraction models, and ships with supervised models trained on the SemEval-2010 dataset.

### Implemented models

pke currently implements the following keyphrase extraction models:

* Unsupervised models
  * Statistical models
    * TfIdf [[documentation](https://boudinfl.github.io/pke/build/html/unsupervised.html#tfidf)]
    * KPMiner [[documentation](https://boudinfl.github.io/pke/build/html/unsupervised.html#kpminer), [article by (El-Beltagy and Rafea, 2010)](http://www.aclweb.org/anthology/S10-1041.pdf)]
    * YAKE [[documentation](https://boudinfl.github.io/pke/build/html/unsupervised.html#yake), [article by (Campos et al., 2020)](https://doi.org/10.1016/j.ins.2019.09.013)]
  * Graph-based models
    * TextRank [[documentation](https://boudinfl.github.io/pke/build/html/unsupervised.html#textrank), [article by (Mihalcea and Tarau, 2004)](http://www.aclweb.org/anthology/W04-3252.pdf)]
    * SingleRank  [[documentation](https://boudinfl.github.io/pke/build/html/unsupervised.html#singlerank), [article by (Wan and Xiao, 2008)](http://www.aclweb.org/anthology/C08-1122.pdf)]
    * TopicRank [[documentation](https://boudinfl.github.io/pke/build/html/unsupervised.html#topicrank), [article by (Bougouin et al., 2013)](http://aclweb.org/anthology/I13-1062.pdf)]
    * TopicalPageRank [[documentation](https://boudinfl.github.io/pke/build/html/unsupervised.html#topicalpagerank), [article by (Sterckx et al., 2015)](http://users.intec.ugent.be/cdvelder/papers/2015/sterckx2015wwwb.pdf)]
    * PositionRank [[documentation](https://boudinfl.github.io/pke/build/html/unsupervised.html#positionrank), [article by (Florescu and Caragea, 2017)](http://www.aclweb.org/anthology/P17-1102.pdf)]
    * MultipartiteRank [[documentation](https://boudinfl.github.io/pke/build/html/unsupervised.html#multipartiterank), [article by (Boudin, 2018)](https://arxiv.org/abs/1803.08721)]
* Supervised models
  * Feature-based models
    * Kea [[documentation](https://boudinfl.github.io/pke/build/html/supervised.html#kea), [article by (Witten et al., 2005)](https://www.cs.waikato.ac.nz/ml/publications/2005/chap_Witten-et-al_Windows.pdf)]
    * WINGNUS [[documentation](https://boudinfl.github.io/pke/build/html/supervised.html#wingnus), [article by (Nguyen and Luong, 2010)](http://www.aclweb.org/anthology/S10-1035.pdf)]





## zelandiya/keyword-extraction-datasets: Different datasets for developing and testing keyword extraction algorithms
https://github.com/zelandiya/keyword-extraction-datasets

## ibatra/BERT-Keyword-Extractor: Deep Keyphrase Extraction using BERT
https://github.com/ibatra/BERT-Keyword-Extractor

## Keyword Extraction Datasets
https://github.com/zelandiya/keyword-extraction-datasets

Different datasets for developing, evaluating and testing keyword extraction algorithms. For benchmarking performance see: O. Medelyan. 2009. Human-competitive automatic topic indexing. PhD Thesis. University of Waikato, New Zealand.

Extracting keywords using a controlled vocabulary or a thesaurus as a source:

NLM_500.zip - 500 PubMed documents with MeSH terms

fao780.tar.gz - 780 FAO publications with Agrovoc terms

fao30.tar.gz - 30 FAO publications, each annotated by 6 professional FAO indexers

Free-text keyword extraction (without a vocabulary):

citeulike180.tar.gz - 180 publications crawled from CiteULike, and keywords assigned by different CiteULike users who saved these publications

SemEval2010-Maui.zip - SemEval-2010 Keyphrase extraction track data in Maui format

keyphrextr.tar.gz - Keyphrase extraction model created using SemEval-2010 training data. This model is used in the Maui GPL demo when no vocabulary is selected.

Extracting keywords using Wikipedia as a controlled vocabulary of allowed terms:

wiki20.tar.gz - 20 Computer Science papers, each annotated with at least 5 Wikipedia articles by 15 teams of indexers



## JRC1995/TextRank-Keyword-Extraction: Keyword extraction using TextRank algorithm after pre-processing the text with lemmatization, filtering unwanted parts-of-speech and other techniques.
https://github.com/JRC1995/TextRank-Keyword-Extraction

Based on: "TextRank: Bringing Order into Texts - by Rada Mihalcea and Paul Tarau"


## demoyhui/KeywordExtraction: 基于LDA和TextRank的关键子提取算法实现
https://github.com/demoyhui/KeywordExtraction

## Ismael-Hery/rake-keywords: Javascript implementation of the "Rake" keywords extraction algorithm
https://github.com/Ismael-Hery/rake-keywords

Some problems with the Rake original scientific paper

Errors in the paper

- 'numbers' is a stop word in the original Fox stop words list, thus 'natural numbers' can not be a candidate keywords. I removed numbers from the Fox stop list as they probably did for the paper (otherwise they would not have found 'natural numbers')

- the paper does not find mixed types as a candidate keywords. I've added mixed types as a candidates key words

- Non English language

TODO :

- compute keywords from a corpus of articles (see sci paper with computation of 'essential' keywords)
- French implementation with 'mots de liaisons' du/des/d'/… excluded from stop list



## waseem18/node-rake: A NodeJS implementation of the Rapid Automatic Keyword Extraction algorithm.
https://github.com/waseem18/node-rake

## sleepycat/rapid-automated-keyword-extraction: A Javascript implementation of the Rapid Automated Keyword Extraction (RAKE) algorithm
https://github.com/sleepycat/rapid-automated-keyword-extraction

## shopping24/rake-js: JS Implementation of the Rapid Automatic Keyword Extraction Paper
https://github.com/shopping24/rake-js

RAKE is the acronym for Rapid Automated Keyword Extraction. The basic algorithm is described by Stuart Rose, Dave Engel, Nick Cramer and Wendy Cowley in their paper "Automatic keyword extraction from individual documents" (©2010, John Wiley & Sons, Ltd, Source click here).

In short RAKE describes splitting a text into fragments by stop words. Stop words are always considered to be irrelevant to the context. The RAKEd result of Red Zebra and Jaguar would therefore be [Red Zebra, Jaguar].

The score is then calculated by counting the individual words and and creating degrees based on the length of found fragments.

### What is this repository about?

This repository includes advanced methods in addition to the original RAKE description. Furthermore we added a functional wrapper as feature for a more flexible way of handling keyword extraction. The process consists of these steps:

- Extracting fragments from any given text using various available methods.
- Score the fragments.
- Retrieve the end result.

Extraction and scoring functions from any source making use of the Phrases and Phrase classes may be used and executed in the desired order.


## colefichter/NRake: A C# implementation of Rapid Automatic Keyword Extraction (RAKE)
https://github.com/colefichter/NRake

This is an implementation based on the algorithm described in the paper "Automatic keyword extraction from individual documents" http://media.wiley.com/product_data/excerpt/22/04707498/0470749822.pdf.


## benmcevoy/Rake: A C# implementation of the Rapid Automatic Keyword Extraction
https://github.com/benmcevoy/Rake

## fromskyblue/Keywords-Extraction: Zhen Yang-Keywords Extraction
https://github.com/fromskyblue/Keywords-Extraction

Keyword extraction by entropy difference between the intrinsic and extrinsic mode
We strive to propose a new metric to evaluate and rank the relevance of words in a text. The method uses the Shannon’s entropy difference between the intrinsic and extrinsic mode, which refers to the fact that relevant words significantly reflect the author’s writing intention, i.e., their occurrences are modulated by the author’s purpose, while the irrelevant words are distributed randomly in the text. By using The Origin of Species by Charles Darwin as a representative text sample, the performance of our detector is demonstrated and compared to previous proposals. Since a reference text ‘‘corpus’’ is all of an author’s writings, books, papers, etc. his collected works is not needed. Our approach is especially suitable for single documents of which there is no a priori information available.


## GomesNayagam/keyword-extraction-single-document: keyword extraction from single document, algorithm from this paper http://ymatsuo.com/papers/ijait04.pdf
https://github.com/GomesNayagam/keyword-extraction-single-document

## ruiyuanxu/MizumotoKeywordExtraction: A keyword extraction tool built for Data Structure & Algorithm course.
https://github.com/ruiyuanxu/MizumotoKeywordExtraction

C#


## ASH1998/Keyword-extraction: Keyword Extraction for PDFs
https://github.com/ASH1998/Keyword-extraction

Dependencies:

- PyPDF2
- sklearn
- pandas

Algorithm used

LDA : Linear Discriminant Analysis A classifier with a linear decision boundary, generated by fitting class conditional densities to the data and using Bayes’ rule. The model fits a Gaussian density to each class, assuming that all classes share the same covariance matrix. The fitted model can also be used to reduce the dimensionality of the input by projecting it to the most discriminative directions.

NMF : Non-Negative Matrix Factorization (NMF) - Find two non-negative matrices (W, H) whose product approximates the non- negative matrix X. This factorization can be used for example for dimensionality reduction, source separation or topic extraction.



## WuLC/KeywordExtraction: Implementation of algorithm in keyword extraction,including TextRank,TF-IDF and the combination of both
https://github.com/WuLC/KeywordExtraction

Implementation of several algorithms for keyword extraction,including TextRank,TF-IDF,TextRank along with TFTF-IDF.Cutting words and filtering stop words are relied on HanLP

## hankcs/HanLP: Han Language Processing
https://github.com/hankcs/HanLP

Natural Language Processing for the next decade. Tokenization, Part-of-Speech Tagging, Named Entity Recognition, Syntactic & Semantic Dependency Parsing, Document Classification


The multilingual NLP library for researchers and companies, built on TensorFlow 2.0, for advancing state-of-the-art deep learning techniques in both academia and industry. HanLP was designed from day one to be efficient, user friendly and extendable. It comes with pretrained models for various human languages including English, Chinese and many others. 



## Linguistic/rake: A Java library for Rapid Automatic Keyword Extraction (RAKE) 🍂
https://github.com/Linguistic/rake

RAKE is an algorithm for extracting keywords (technically phrases, but I don't question scientific literature) from a document that have a high relevance or importance to the contents of the document.


## sing1ee/textrank-java: a simple implementation of textrank algorithm for nlp keywords extraction
https://github.com/sing1ee/textrank-java

## ibatra/BERT-Keyword-Extractor: Deep Keyphrase Extraction using BERT
https://github.com/ibatra/BERT-Keyword-Extractor

## aespresso/chinese_nlp_tutorial_clustering_keywords_extraction: 中文自然语言处理聚类与关键词提取教程
https://github.com/aespresso/chinese_nlp_tutorial_clustering_keywords_extraction

## BastinFlorian/Keywords_extraction_with_GOW: Graph of words (Networkx) and keywords extraction (Ktruss, Kcore, DivRank, BestCoverage)
https://github.com/BastinFlorian/Keywords_extraction_with_GOW

* First we present an example of the methods used to extract keywords (see Graph of words and keywords extraction.ipynb and K-truss_code_example.ipynb)
* Then we give a code to compute the k_core and obtain the graphs of directories of files or all files in directories containing sub-directories (see K_core_corpus.py)
* We also give an implementation of the K-truss algorithm (see K-truss_code.py)
* We make a time analysis to see the evolution of some words through time, in order to detect events related to them.


## RHKeng/ShenCeCup: A competition on DataCastle which is about text keyword extraction ! Rank 6 / 622 !
https://github.com/RHKeng/ShenCeCup

A competition on DataCastle which is about text keyword extraction! Rank 6/622!

" Shence Cup" 2018 College Algorithm Masters is a single-player competition that can only be soloed by college students. Shence Data provides the titles and texts of about 100,000 news articles, of which 1,000 articles have corresponding annotation data. There are no more than 5 keywords for each article in the labeled data, and the keywords have appeared in the title or body of the article. According to the existing data, it is necessary to train a "keyword extraction" model to extract the keywords of articles without annotated data, and submit at most two keywords for each article.

Final ranking: 6/622

\[...]

### 5 Model selection

Compare the effects of unsupervised models (tfidf/tfiwf, textRank, topic model LSI/LDA), and finally use tfidf as the basic model to select the keyword candidate set.

#### 5.1 The tfidf
tfidf (term frequency-inverse document frequency) algorithm is a statistical method used to evaluate the importance of a word to a document set or a document in a corpus. The importance of a word increases in proportion to the number of times it appears in the document, but at the same time it decreases in inverse proportion to the frequency of its appearance in the corpus.

TF (term frequency) is the number of times a word appears in the article, TF (term frequency) = the number of times a word appears in the article / the total number of words in the article; IDF (inverse document frequency) is the frequency of the word , IDF reverse document frequency=log (total number of documents in the corpus/(total number of documents containing the word+1)), if a word is more common, then its denominator is larger, and the IDF value is smaller.

#### 5.2 Tfiwf

TF remains unchanged, IWF is the sum of the word frequency of all words in the document/the word frequency

#### 5.3 Pagerank (listed here only to lead to the following textrank)

need to know which webpages are linked to webpage A, that is, first get webpage A's access to the chain, and then calculate webpage A's PR by voting for webpage A from the access chain value. This design can ensure the achievement of such an effect: when some high-quality webpages point to webpage A, then the PR value of webpage A will increase because of these high-quality votes, and webpage A is pointed to by fewer webpages or by some When a web page with a lower PR value points to, the PR value of A will not be very large, which can reasonably reflect the quality level of a web page. Vi represents a certain webpage, Vj represents a webpage linked to Vi (that is, the in-link of Vi), S(Vi) represents the PR value of the webpage Vi, In(Vi) represents the collection of all in-links of the webpage Vi, Out(Vj) Represents the number of web pages Vj linked to other web pages, and d represents the damping coefficient, which is used to overcome the inherent defects of the part after "d *" in this formula: if there is only a summation part, then the formula will not be able to handle The PR value of the web pages that enter the chain, because at this time, according to the formula, the PR value of these web pages is 0, but the actual situation is not like this, so a damping coefficient is added to ensure that each web page has a PR value greater than 0. According to the experimental results, with a damping coefficient of 0.85, the PR value can be converged to a stable value after about 100 iterations. When the damping coefficient is close to 1, the number of iterations required will increase abruptly and the sorting will be unstable. The score in front of S(Vj) in the formula refers to the PR value of Vj that should be divided equally among all the webpages pointed to by Vj, so that it can be regarded as dividing one's votes among the webpages that one links to.

#### 5.4 textrank 

is a graph-based sorting algorithm for text, which can realize keyword extraction only by using the information of a single document itself, without relying on a corpus. (Calling the interface of jieba ) Wji refers to the similarity between the two sentences of Vi and Vj. Edit distance and cosine similarity can be used. When textrank is applied to keyword extraction, it is different from automatic abstract extraction: 1) The association between words has no weight, that is, Wji is 1; 2) Each word is not linked to all words in the document, but through Set a fixed-length sliding window format, with links between words in the window.

#### 5.5 The Topic Model 

topic model believes that there is no direct connection between words and documents, and that they should be connected by a dimension, which is the topic. The topic model is an automatic analysis of each document, counting the words in the document, and judging which topics the current document contains and the proportion of each topic based on the statistical information. A topic model is a generative model. Each word in an article is obtained through a process of "select a topic with a certain probability, and select a word from this topic with a certain probability"; topic models are commonly used The methods are LSI (LSA) and LDA, where LSI uses SVD (Singular Value Decomposition) for brute force cracking, while LDA uses Bayesian methods to fit distribution information. Through the LSA or LDA algorithm, you can get the distribution of the document to the topic and the distribution of the topic to the word. The distribution of the word to the topic can be obtained according to the topic to the word distribution (Bayesian method), and then through this distribution and the document to the topic distribution Calculate the similarity between the document and the word, and select the word list with high similarity as the key word of the document.

#### 5.5.1 LSA

Latent Semantic Analysis (LSA), also called Latent Semantic Indexing, LSI. It is a commonly used simple topic model. LSA is a way to get the text topic based on the singular value decomposition (SVD) method. Umk represents the distribution matrix of documents to topics, and the transposition of Vnk represents the distribution matrix of topics to words. LSA uses SVD to express words and documents more essentially, and maps them to low-dimensional spaces. While limited use of text semantic information, LSA greatly reduces the cost of calculation and improves the quality of analysis. However, the computational complexity is very high, and the feature space dimension is large, and the computational efficiency is very low. When a new document enters the existing feature space, the entire space needs to be retrained to obtain the distribution information of the newly added document. In addition, there are problems of insensitivity to frequency distribution and weak physical interpretation.

#### 5.5.2 pLSA

has been improved on the basis of LSA, by using the EM algorithm to fit the distribution information instead of using SVD for brute force cracking.

In PLSA, the bag-of-words model is also used (the bag-of-words model refers to a document. We only consider whether a word appears, regardless of the order in which it appears. On the contrary, n-gram considers the order in which the words appear). And the documents are independently exchangeable, and the words in the same document are also independently exchangeable. In PLSA, we will extract a topic word with a fixed probability, then find the corresponding word distribution according to the extracted topic word, and then extract a vocabulary according to the word distribution.

#### 5.5.3 LDA

LDA is based on PLSA and adds two Dirichlet prior distributions to topic distribution and word distribution. In PLSA, both topic distribution and word distribution are uniquely determined. However, in LDA, topic distribution and word distribution are uncertain. The authors of LDA adopt Bayesian thinking and believe that they should obey a distribution. Both topic distribution and word distribution are polynomial distributions, because polynomial distributions Dirichlet distribution and Dirichlet distribution are conjugate structures. In LDA, topic distribution and word distribution use Dirichlet distribution as their conjugate prior distribution.

In LDA, there is no fixed optimal solution for the number of topics. When training the model, the number of topics needs to be set in advance, and the trainer needs to manually adjust the parameters according to the training results, and then optimize the number of topics. We can find the posterior distribution according to the polynomial distribution and the prior distribution of the data, and then use this posterior distribution as the next prior distribution, and iteratively update. There are generally two solving methods, the first is based on Gibbs sampling algorithm, and the second is based on variational inference EM algorithm.




## XuMuK1/KeywordsExtraction: Project for courses NLA and Optimization in Sk. The goal is to learn how to test different techniques for extracting keywords from news.
https://github.com/XuMuK1/KeywordsExtraction

## pozhidaevsa/ExtractionKeywords: Extract keywords from russian text
https://github.com/pozhidaevsa/ExtractionKeywords

## csurfer/rake-nltk: Python implementation of the Rapid Automatic Keyword Extraction algorithm using NLTK.
https://github.com/csurfer/rake-nltk

RAKE short for Rapid Automatic Keyword Extraction algorithm, is a domain independent keyword extraction algorithm which tries to determine key phrases in a body of text by analyzing the frequency of word appearance and its co-occurrence with other words in the text.


## AidenHuen/SMP-Keyword-Extraction: CSDN博客的关键词提取算法，融合TF，IDF，词性，位置等多特征。该项目用于参加2017 SMP用户画像测评，排名第四,在验证集中精度为59.9%，在最终集中精度为58.7%。模型并未使用机器学习的方法，具有较强的泛化能力。
https://github.com/AidenHuen/SMP-Keyword-Extraction

About CSDN blog keyword extraction algorithm, fusion TF, IDF, part of speech, location and other features. 

This project was used to participate in the 2017 SMP user portrait evaluation, ranking fourth, with an accuracy of 59.9% in the verification set and 58.7% in the final set. 
The model does not use machine learning methods and has strong generalization capabilities.




## bguvenc/keyword_extraction: Keyword extraction with Word2Vec
https://github.com/bguvenc/keyword_extraction

Keyword extraction method by using Word2Vec and Pagerank algorithms
The most common representation of distributional semantics is called one-hot representation in which dimensionality is equal to vocabulary’s cardinality. Elements of this vector space representation consist of 0’s and 1’s. However, this representation has some disadvantages. For example, in these representations, it is difficult to make deductions about word similarity. Due to high dimensionality, they can also cause overfitting. Moreover, it is computationally expensive.

Word embeddings are designed to capture attributional similarities between vocabulary items. Words that appear in similar contexts should be close to each other in the projected vector space. This means that grouping of words in a vector space must share same semantic properties. In word embeddings, Latent Semantic Analysis (LSA) uses a counting base dimensionality reduction method. Word2Vec is created as an alternative. Its low dimensionality can help to reduce computational complexity. Also compared with distributional semantics methods, it causes less overfitting. Word2Vec can also detect analogies between words.

Our model takes Word2Vec representations of words in a vector space. While we construct the Word2Vec model, we decide a threshold of counts of words because words that appear only once or twice in a large corpus are probably not unusual for the model, and there is not enough data to make any meaningful training on those words. A reasonable value for minimum counts changes between 0-100, and it depends on the size of corpora. Another critical parameter for Word2Vec model is the dimension of the vectors. This value changes between 100 and 400. Dimensions larger than 400 require more training but leads to more accurate models. I used Google News corpora which provided by Google which consist of 3 million word vectors. I did not remove stop words or infrequent words because these algorithms use windows and to find vector representations. So I need the nearby words to find vector representations.

The second step of this algorithm is to find PageRank value of each word. PageRank algorithm works with random walk. The original PageRank algorithm takes internet pages as a node. In our model PageRank algorithm takes Word2Vec representations of words. The cosine distance is used to calculate edge weights between nodes. TextRank algorithm uses a similar method. While TextRank chooses the bag of word representations of words and a different similarity measure in finding edge weights, in this algorithm I used the Word2Vec representations and the cosine similarity. After PageRank values of words are found, we can get words which have the highest PageRank values. Finally, these words can be seen as a keyword of a text.




## gaussic/tf-idf-keyword: Keyword extraction based on TF-IDF on specific corpus. 基于特定语料库的TF-IDF的中文关键词提取
https://github.com/gaussic/tf-idf-keyword

Chinese keyword extraction 

requirements based on TF-IDF. 


## naushadzaman/keyword-extraction-from-tweets: keyword extraction from tweets using python
https://github.com/naushadzaman/keyword-extraction-from-tweets

keyword extraction from tweets using python

In this module, we use Pattern tools to do POS tagging/Phrase extraction of tweets. The usual POS tagging/chunking tools do not work well for free form texts like tweets, so we needed to use a tool that is designed and trained for twitter/tweets. From Pattern tool output, we extract phrases as entities. You can decide to use on NP (Noun Phrase), but our default is to use NP (Noun Phrase) and ADJP (Adjective Phrase). With this tool, you can also extract hashtags, usernames, urls from the tweet.


## vgrabovets/multi_rake: Multilingual Rapid Automatic Keyword Extraction (RAKE) for Python
https://github.com/vgrabovets/multi_rake

Multilingual Rapid Automatic Keyword Extraction (RAKE) for Python

Features

- Automatic keyword extraction from text written in any language
- No need to know language of text beforehand
- No need to have list of stopwords
- 26 languages are currently available, for the rest - stopwords are generated from provided text
- Just configure rake, plug in text and get keywords (see implementation details)




## lovit/soykeyword: Python library for keyword extraction
https://github.com/lovit/soykeyword

Python library for Keyword Extraction 

Python library for keyword/association extraction. Keywords and related words extracted from by Lovit (Hyunjoong) and Hunsik Shin 

soykeyword are defined as follows. Keywords in a set of documents are words of good quality (discriminative power) that can distinguish them from other sets of documents, and words (high coverage) that can describe them well. Words with a low frequency are more likely to appear in only one set, so they have a high level of discrimination, but weak explanation. The proposed two algorithms select words that have high explanatory and distinguishing power as keywords. An associative word defines a keyword that separates a set of documents with and without a reference word from an associative word. This also means that the word with high co-occurrence. Choose words with high co-occurrence and good explanation. 



## tarwn/bookmark_analysis: Exploration of text analysis for automatic bookmarking/keyword extraction
https://github.com/tarwn/bookmark_analysis

Automated Keyword Extraction – TF-IDF, RAKE, and TextRank

After initially playing around with text processing in my prior post, I added an additional algorithm and cleaned up the logic to make it easier to perform test runs and reuse later. I tweaked the RAKE algorithm implementation and added TextRank into the mix, with full sample code and links to sources available. I’m also using a read-through cache of the unprocessed and processed files so I can see the content and tweak the cleanse logic.

Context: The ultimate goal is to build a script that could process through 6 years of my bookmarked reading and extract out keywords, so I could do some trend analysis on how my reading has changed over time and maybe later build a supervised model with that data to analyze new online posts and produce a “worth my time or not” score.


## Parsely/serpextract: Easy extraction of keywords and engines from search engine results pages (SERPs).
https://github.com/Parsely/serpextract

serpextract provides easy extraction of keywords from search engine results pages (SERPs).

This module is possible in large part to the very hard work of the Piwik team. Specifically, we make extensive use of their list of search engines.



## singularity014/Keyword-Extraction-Bidirectional-LSTM: Deep learning LSTM + BERT based approach for labeling a corpus with keywords, then training a model to extract keywords.
https://github.com/singularity014/Keyword-Extraction-Bidirectional-LSTM

Deep learning Bi-LSTM based approach for labeling a corpus with keywords, then training a model to extract keywords.

Article was late published in pprints.



## pemagrg1/Hindi-POS-Tagging-and-Keyword-Extraction: Hindi POS Tags and keywords using TNT model. Created Date: 28 Sept 2018
https://github.com/pemagrg1/Hindi-POS-Tagging-and-Keyword-Extraction

Part of speech plays a very major role in NLP task as it is important to know how a word is used in every sentences. POS tagging is used mostly for Keyword Extractions, phrase extractions, Named Entity Recognition, etc. Before going further on POS tagging, I am assuming that you all know about part of speech as we all have studied grammar during school. Didn't we? But anyways let me give a brief explanation on it!

There are eight main Parts of Speech: Nouns(naming word), Pronouns(replaces a noun), Adjectives(describing word), Verbs(action word), Adverbs(describes a verb), Prepositions(shows relationships), Conjunctions(joining word) and Interjections(Expressive word). Most of it are further divided into sub-parts. Noun is divided into Proper Nouns, Common Nouns, Concrete Nouns etc.

Reminds you of school days?? Okay now lets start with Hindi Part of Speech Tagging.

Hindi Part of Speech Tagging is something that people are still doing research on as we have various techniques and libraries available for English Text and rarely for Hindi Text. [1] Manish and Pushpak researched on Hindi POS using a simple HMM based POS tagger with accuracy of 93.12%. while [2]Nisheeth Joshi, Hemant Darbari and Iti Mathur also researched on Hindi POS using Hidden Markov Model with frequency count of two tags seen together in the corpus divided by the frequency count of the previous tag seen independently in the corpus. [3] S Phani Kumar Gadde, Meher Vijay Yeleti used CRF based tagger and Brants TnT (Brants, 2000), a HMM based tagger for hindi POS Tag where they got an acccuracy of 94.21%.




## abner-wong/textrank: keyword extraction and summarization for Chinese text by TextRank
https://github.com/abner-wong/textrank

Based on the TextRank algorithm, the keyword extraction and summarization tasks of Chinese text are realized, and the core calculation code remains consistent with the paper.



## yongzhuo/Macropodus: 自然语言处理工具Macropodus，基于Albert+BiLSTM+CRF深度学习网络架构，中文分词，词性标注，命名实体识别，新词发现，关键词，文本摘要，文本相似度，科学计算器，中文数字阿拉伯数字(罗马数字)转换，中文繁简转换，拼音转换。tookit(tool) of NLP，CWS(chinese word segnment)，POS(Part-Of-Speech Tagging)，NER(name entity recognition)，Find(new words discovery)，Keyword(keyword extraction)，Summarize(text summarization)，Sim(text similarity)，Calculate(scientific calculator)，Chi2num(chinese number to arabic number)
https://github.com/yongzhuo/Macropodus

Macropodus is a natural language processing toolkit trained on large-scale Chinese corpus based on the Albert+BiLSTM+CRF network architecture. Common NLP functions such as Chinese word segmentation, part-of-speech tagging, named entity recognition, keyword extraction, text summarization, new word discovery, text similarity, calculator, number conversion, pinyin conversion, traditional and simplified conversion will be provided.



## kanjirz50/rake-ja: Rapid Automatic Keyword Extraction algorithm for Japanese
https://github.com/kanjirz50/rake-ja

Rapid Automatic Keyword Extraction algorithm for Japanese.

This module builds on rake-nltk.




## killa1218/CopyRNN-Keyword-Extraction
https://github.com/killa1218/CopyRNN-Keyword-Extraction

This is an implementation of Deep Keyphrase Generation based on CopyNet.

One training dataset (KP20k), five testing datasets (KP20k, Inspec, NUS, SemEval, Krapivin) and one pre-trained model are provided.

Note that the model is trained on scientific papers (abstract and keyword) in Computer Science domain, so it's expected to work well only for CS papers.





## CodePothunter/keywordExtract_zh: A Chinese key terminology extraction tool for MOOC.
https://github.com/CodePothunter/keywordExtract_zh

A Chinese key terminology extraction tool for MOOC. This tool needs to rely on the latest version of jieba for word segmentation. When using, put the entire folder in the working directory and call it as a toolkit. At present, in addition to supporting the extraction of key terms, it can also generate a summary of the lecture notes.



## pemagrg1/Nepali-POS-Tagging-and-Keyword-Extraction: Extract part of speech for Nepali words using TNT model. Created Date: 12 October 2018
https://github.com/pemagrg1/Nepali-POS-Tagging-and-Keyword-Extraction


Nepali is the language spoken by the people of Nepal. Nepali is actually written with the Devanagari alphabet and is an Indo-Aryan Language. The Devanagari script, which is generally known as Nagari, is written from left to right. The order of the letters made up of vowels and consonants is known as the "varnamala" which means the "garland of flowers." In the Unicode Conventional, the Devanagari is constituted in three blocks. U+0900–U+097F comprises the Devanagari, U+1CD0–U+1CFF comprises the Devanagari Extended, and U+A8E0–U+A8FF comprises the Vedic Extension. 

The paper, "Structure of Nepali Grammar" by Bal Krishna Bal has an awesome explanation on the grammar of Nepali [1] where he explains how each part of speech is used in Nepali. Asmita (Student of Bal Krishna Bal) has also done her degree project under the guidance of Bal Krishna Bal on "Part of Speech Tagger for Nepali Text using SVM" where she got an accuracy of 88% [2]. Tej Bahadur Shahi,Tank Nath Dhamala, and Bikash Balami also published a paper on "Support Vector Machines based Part of Speech Tagging for Nepali Text" where they got an accuracy of 90% on TNT and 90% on SVM, using 80000 training data size[3].

Nepali and Hindi are quite similar as they both follow the Devanagari script.







