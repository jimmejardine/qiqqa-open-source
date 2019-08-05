using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Lucene.Net.Analysis;
using Lucene.Net.Analysis.Standard;
using Lucene.Net.Documents;
using Lucene.Net.Index;
using Lucene.Net.QueryParsers;
using Lucene.Net.Search;
using Utilities.BibTex.Parsing;
using Utilities.Files;
using Version = Lucene.Net.Util.Version;

namespace Utilities.Language.TextIndexing
{
    public class LuceneIndex : IDisposable
    {
        static readonly string INDEX_VERSION = "4.0";
        string LIBRARY_INDEX_BASE_PATH;

        Analyzer analyzer;
        object index_writer_lock = new object();
        IndexWriter index_writer = null;

        public LuceneIndex(string LIBRARY_INDEX_BASE_PATH)
        {
            this.LIBRARY_INDEX_BASE_PATH = LIBRARY_INDEX_BASE_PATH;

            CheckIndexVersion();

            // Write the version of the index
            Directory.CreateDirectory(LIBRARY_INDEX_BASE_PATH);
            File.WriteAllText(VersionFilename, INDEX_VERSION);

            // Delete any old locks
            if (File.Exists(LuceneWriteLockFilename))
            {
                Logging.Warn("The lucene file lock was still there (bad shutdown perhaps) - so deleting it");
                File.Delete(LuceneWriteLockFilename);
            }

            // Create our common parts
            analyzer = new StandardAnalyzer(Version.LUCENE_29, new Hashtable());            
        }

        ~LuceneIndex()
        {
            Logging.Info("~LuceneIndex()");
            Dispose(false);            
        }

        public void Dispose()
        {
            Logging.Info("Disposing LuceneIndex");
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        private void Dispose(bool disposing)
        {
            if (disposing)
            {
                // Get rid of managed resources
                Logging.Info("Disposing the lucene index writer");

                lock (index_writer_lock)
                {
                    FlushIndexWriter_LOCK();
                }
            }

            // Get rid of unmanaged resources 
        }

        public void WriteMasterList()
        {
            lock (index_writer_lock)
            {
                FlushIndexWriter_LOCK();
            }
        }
        
        public void FlushIndexWriter_LOCK()
        {
            Logging.Info("+Flushing a lucene IndexWriter");
            if (null != index_writer)
            {
                index_writer.Commit();
                index_writer.Optimize();
                index_writer.Close();
                index_writer = null;
            }
            Logging.Info("-Flushing a lucene IndexWriter");
        }

        private static void AddDocumentMetadata_SB(Document document, StringBuilder sb, string field_name, string field_value)
        {
            if (!String.IsNullOrEmpty(field_value))
            {
                sb.AppendLine(field_value);

                document.Add(new Field(field_name, field_value, Field.Store.NO, Field.Index.ANALYZED));
            }
        }

        private static void AddDocumentMetadata_BibTex(Document document, BibTexItem bibtex_item)
        {
            if (null == bibtex_item) return;

            try
            {
                if (String.IsNullOrWhiteSpace(bibtex_item.Type) || String.IsNullOrWhiteSpace(bibtex_item.Key))
                {
                    Logging.Warn("boo");
                }
                document.Add(new Field("type", bibtex_item.Type, Field.Store.NO, Field.Index.ANALYZED));
                document.Add(new Field("key", bibtex_item.Key, Field.Store.NO, Field.Index.ANALYZED));

                foreach (var pair in bibtex_item.EnumerateFields())
                {
                    document.Add(new Field(pair.Key, pair.Value, Field.Store.NO, Field.Index.ANALYZED));
                }
            }
            catch (Exception ex)
            {
                Logging.Error(ex, "unexpected exception in AddDocumentMetaData_Bibtex");
            }
        }

        public void AddDocumentMetadata(bool is_deleted, string fingerprint, string title, string author, string year, string comment, string tag, string annotation, string bibtex, BibTexItem bibtex_item)
        {
            Document document = null;

            // Create the document only if it is not to be deleted
            if (!is_deleted)
            {
                document = new Document();
                document.Add(new Field("fingerprint", fingerprint, Field.Store.YES, Field.Index.NOT_ANALYZED_NO_NORMS));
                document.Add(new Field("page", "0", Field.Store.YES, Field.Index.NOT_ANALYZED_NO_NORMS));

                StringBuilder content_sb = new StringBuilder();

                AddDocumentMetadata_SB(document, content_sb, "title", title);
                AddDocumentMetadata_SB(document, content_sb, "author", author);
                AddDocumentMetadata_SB(document, content_sb, "year", year);
                AddDocumentMetadata_SB(document, content_sb, "comment", comment);
                AddDocumentMetadata_SB(document, content_sb, "tag", tag);
                AddDocumentMetadata_SB(document, content_sb, "annotation", annotation);
                AddDocumentMetadata_SB(document, content_sb, "bibtex", bibtex);

                AddDocumentMetadata_BibTex(document, bibtex_item);

                string content = content_sb.ToString();
                document.Add(new Field("content", content, Field.Store.NO, Field.Index.ANALYZED));
            }
            
            AddDocumentPage_INTERNAL(fingerprint, 0, document);
        }

        public void AddDocumentPage(bool is_deleted, string fingerprint, int page, string content)
        {            
            Document document = null;

            // Create the document only if it is not to be deleted
            if (!is_deleted)
            {
                document = new Document();
                document.Add(new Field("fingerprint", fingerprint, Field.Store.YES, Field.Index.NOT_ANALYZED_NO_NORMS));
                document.Add(new Field("page", Convert.ToString(page), Field.Store.YES, Field.Index.NOT_ANALYZED_NO_NORMS));
                document.Add(new Field("content", content, Field.Store.NO, Field.Index.ANALYZED));
            }

            AddDocumentPage_INTERNAL(fingerprint, page, document);
        }

        private void AddDocumentPage_INTERNAL(string fingerprint, int page, Document document)
        {
            // Write to the index            
            lock (index_writer_lock)
            {
                if (null == index_writer)
                {
                    Logging.Info("+Creating a new lucene IndexWriter");
                    index_writer = new IndexWriter(LIBRARY_INDEX_BASE_PATH, analyzer, IndexWriter.MaxFieldLength.UNLIMITED);
                    Logging.Info("-Creating a new lucene IndexWriter");
                }

                // Delete the document if it already exists
                BooleanQuery bq = new BooleanQuery();
                bq.Add(new TermQuery(new Term("fingerprint", fingerprint)), BooleanClause.Occur.MUST);
                bq.Add(new TermQuery(new Term("page", Convert.ToString(page))), BooleanClause.Occur.MUST);
                index_writer.DeleteDocuments(bq);

                // Add the new document
                if (null != document)
                {
                    index_writer.AddDocument(document);
                }
            }
        }

        public int GetDocumentCountForKeyword(string keyword)
        {
            HashSet<string> docs = GetDocumentsWithWord(keyword);
            return docs.Count;
        }


        /***
         * Understands the lucene query syntax
         */
        public List<IndexResult> GetDocumentsWithQuery(string query)
        {
            List<IndexResult> fingerprints = new List<IndexResult>();
            HashSet<string> fingerprints_already_seen = new HashSet<string>();

            try
            {
                IndexReader index_reader = IndexReader.Open(LIBRARY_INDEX_BASE_PATH, true);
                Searcher index_searcher = new IndexSearcher(index_reader);

                QueryParser query_parser = new QueryParser(Version.LUCENE_29, "content", analyzer);

                Query query_object = query_parser.Parse(query);
                Hits hits = index_searcher.Search(query_object);

                var i = hits.Iterator();
                while (i.MoveNext())
                {
                    Hit hit = (Hit)i.Current;
                    string fingerprint = hit.Get("fingerprint");
                    string page = hit.Get("page");

                    if (!fingerprints_already_seen.Contains(fingerprint))
                    {
                        fingerprints_already_seen.Add(fingerprint);

                        IndexResult index_result = new IndexResult { fingerprint = fingerprint, score = hit.GetScore() };
                        fingerprints.Add(index_result);
                    }
                }

                // Close the index
                index_searcher.Close();
                index_reader.Close();
            }
            catch (Exception ex)
            {
                Logging.Warn(ex, "GetDocumentsWithQuery: There was a problem opening the index file for searching.");
            }

            return fingerprints;
        }

        public List<IndexPageResult> GetDocumentPagesWithQuery(string query)
        {
            List<IndexPageResult> results = new List<IndexPageResult>();
            Dictionary<string, IndexPageResult> fingerprints_already_seen = new Dictionary<string, IndexPageResult>();

            try
            {
                IndexReader index_reader = IndexReader.Open(LIBRARY_INDEX_BASE_PATH, true);
                Searcher index_searcher = new IndexSearcher(index_reader);

                QueryParser query_parser = new QueryParser(Version.LUCENE_29, "content", analyzer);

                Query query_object = query_parser.Parse(query);
                Hits hits = index_searcher.Search(query_object);

                var i = hits.Iterator();
                while (i.MoveNext())
                {
                    Hit hit = (Hit)i.Current;
                    string fingerprint = hit.Get("fingerprint");
                    int page = Convert.ToInt32(hit.Get("page"));
                    double score = hit.GetScore();

                    // If this is the first time we have seen this fingerprint, make the top-level record
                    if (!fingerprints_already_seen.ContainsKey(fingerprint))
                    {
                        IndexPageResult result = new IndexPageResult();
                        result.fingerprint = fingerprint;
                        result.score = score;

                        // Add to our structures
                        results.Add(result);
                        fingerprints_already_seen[fingerprint] = result;
                    }

                    // And add the page record
                    {
                        IndexPageResult result = fingerprints_already_seen[fingerprint];
                        result.page_results.Add(new IndexPageResult.PageResult { page = page, score=score});
                    }
                }

                // Close the index
                index_searcher.Close();
                index_reader.Close();
            }
            catch (Exception ex)
            {
                Logging.Warn(ex, "GetDocumentPagesWithQuery: There was a problem opening the index file for searching.");
            }

            return results;
        }

        public HashSet<string> GetDocumentsWithWord(string keyword)
        {
            HashSet<string> fingerprints = new HashSet<string>();

            try
            {
                keyword = ReasonableWord.MakeReasonableWord(keyword);
                if (null != keyword)
                {
                    ////Do a quick check for whether there are actually any segments files, otherwise we throw many exceptions in the IndexReader.Open in a very tight loop.
                    ////Added by Nik to cope with some exception...will uncomment this when i know what the problem is...
                    //var segments_files = System.IO.Directory.GetFiles(LIBRARY_INDEX_BASE_PATH, "segments*",SearchOption.AllDirectories);
                    //if (segments_files.Length <= 0)
                    //{
                    //    Logging.Debug("No index segments files found");
                    //    return fingerprints;
                    //}
                    
                    IndexReader index_reader = IndexReader.Open(LIBRARY_INDEX_BASE_PATH, true);
                    Searcher index_searcher = new IndexSearcher(index_reader);

                    TermQuery term_query = new TermQuery(new Term("content", keyword));
                    Hits hits = index_searcher.Search(term_query);

                    var i = hits.Iterator();
                    while (i.MoveNext())
                    {
                        Hit hit = (Hit)i.Current;
                        string fingerprint = hit.Get("fingerprint");
                        fingerprints.Add(fingerprint);
                    }

                    // Close the index
                    index_searcher.Close();
                    index_reader.Close();
                }
            }
            catch (Exception ex)
            {
                Logging.Warn(ex, "GetDocumentsWithWord: There was a problem opening the index file for searching.");
            }

            return fingerprints;
        }

        public List<string> GetDocumentsSimilarToDocument(string document_filename)
        {
            List<string> fingerprints = new List<string>();

            try
            {
                IndexReader index_reader = IndexReader.Open(LIBRARY_INDEX_BASE_PATH, true);
                Searcher index_searcher = new IndexSearcher(index_reader);
            
                LuceneMoreLikeThis mlt = new LuceneMoreLikeThis(index_reader);
                mlt.SetFieldNames(new string[] { "content" });
                mlt.SetMinTermFreq(0);

                Query query = mlt.Like(new StreamReader(document_filename));
                Hits hits = index_searcher.Search(query);
                var i = hits.Iterator();
                while (i.MoveNext())
                {
                    Hit hit = (Hit)i.Current;
                    string fingerprint = hit.Get("fingerprint");
                    fingerprints.Add(fingerprint);
                }

                // Close the index
                index_searcher.Close();
                index_reader.Close();
            }
            catch (Exception ex)
            {
                Logging.Warn(ex, "GetDocumentsSimilarToDocument: There was a problem opening the index file for searching.");
            }

            return fingerprints;
        }

        // ---------------------------------------------------------

        private string VersionFilename
        {
            get
            {
                return LIBRARY_INDEX_BASE_PATH + "index_version.txt";
            }
        }

        private string LuceneWriteLockFilename
        {
            get
            {
                return LIBRARY_INDEX_BASE_PATH + "write.lock";
            }
        }

        public void InvalidateIndex()
        {
            Logging.Warn("Invalidating Lucene index at {0}", LIBRARY_INDEX_BASE_PATH);
            FileTools.Delete(VersionFilename);
        }

        
        private void CheckIndexVersion()
        {
            string version = null;

            try
            {
                if (File.Exists(VersionFilename))
                {
                    string[] index_version_lines = File.ReadAllLines(VersionFilename);
                    version = index_version_lines[0];
                }
            }
            catch (Exception ex)
            {
                Logging.Error(ex, "There was a problem while trying to check the index version");
            }

            if (0 != String.Compare(version, INDEX_VERSION))
            {
                Logging.Warn("This index is out of date (it's version is {0}), so deleting the index.", version);
                DeleteIndex();
            }
        }

        private void DeleteIndex()
        {
            Logging.Info("Deleting the index at path '{0}'", LIBRARY_INDEX_BASE_PATH);
            DirectoryTools.DeleteDirectory(LIBRARY_INDEX_BASE_PATH, true);
        }
    }
}
