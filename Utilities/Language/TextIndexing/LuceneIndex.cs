#if !HAS_NO_LUCENE

using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text;
using Lucene.Net.Analysis;
using Lucene.Net.Documents;
using Lucene.Net.Index;
using Lucene.Net.QueryParsers;
using Lucene.Net.Search;
using Utilities.Files;
using Utilities.GUI;
using Directory = Alphaleonis.Win32.Filesystem.Directory;
using File = Alphaleonis.Win32.Filesystem.File;
using Path = Alphaleonis.Win32.Filesystem.Path;
using Version = Lucene.Net.Util.Version;


namespace Utilities.Language.TextIndexing
{
    public class LuceneIndex : IDisposable
    {
        private static readonly string INDEX_VERSION = "4.0";
        private string LIBRARY_INDEX_BASE_PATH;
        private Analyzer analyzer;
        private object index_writer_lock = new object();
        private IndexWriter index_writer = null;

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
            analyzer = new Lucene.Net.Analysis.Standard.StandardAnalyzer(Lucene.Net.Util.Version.LUCENE_29, new Hashtable());
        }

        ~LuceneIndex()
        {
            Logging.Debug("~LuceneIndex()");
            Dispose(false);
        }

        public void Dispose()
        {
            Logging.Debug("Disposing LuceneIndex");
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        private int dispose_count = 0;
        protected virtual void Dispose(bool disposing)
        {
            Logging.Debug("LuceneIndex::Dispose({0}) @{1}", disposing, dispose_count);

            WPFDoEvents.SafeExec(() =>
            {
                if (dispose_count == 0)
                {
                    // Get rid of managed resources
                    Logging.Info("Disposing the lucene index writer");

                    // Utilities.LockPerfTimer l1_clk = Utilities.LockPerfChecker.Start();
                    lock (index_writer_lock)
                    {
                        // l1_clk.LockPerfTimerStop();
                        FlushIndexWriter_LOCK();
                    }
                }
            });

            WPFDoEvents.SafeExec(() =>
            {
                // Utilities.LockPerfTimer l2_clk = Utilities.LockPerfChecker.Start();
                lock (index_writer_lock)
                {
                    // l2_clk.LockPerfTimerStop();
                    index_writer = null;
                }
            });

            ++dispose_count;
        }

        public void WriteMasterList()
        {
            // Utilities.LockPerfTimer l1_clk = Utilities.LockPerfChecker.Start();
            lock (index_writer_lock)
            {
                // l1_clk.LockPerfTimerStop();
                FlushIndexWriter_LOCK();
            }
        }

        private void FlushIndexWriter_LOCK()
        {
            Stopwatch clk = Stopwatch.StartNew();

            Logging.Info("+Flushing a lucene IndexWriter");
            if (null != index_writer)
            {
                index_writer.Commit();
                index_writer.Optimize();
                index_writer.Close();
                index_writer.Dispose();
                index_writer = null;
            }
            Logging.Info("-Flushing a lucene IndexWriter (time spent: {0} ms)", clk.ElapsedMilliseconds);
        }

        private static void AddDocumentMetadata_SB(Document document, StringBuilder sb, string field_name, string field_value)
        {
            if (!String.IsNullOrEmpty(field_value))
            {
                sb.AppendLine(field_value);

                document.Add(new Lucene.Net.Documents.Field(field_name, field_value, Lucene.Net.Documents.Field.Store.NO, Lucene.Net.Documents.Field.Index.ANALYZED));
            }
        }

        private static void AddDocumentMetadata_BibTex(Document document, Utilities.BibTex.Parsing.BibTexItem bibtex_item)
        {
            if (null == bibtex_item) return;

            document.Add(new Lucene.Net.Documents.Field("type", bibtex_item.Type, Lucene.Net.Documents.Field.Store.NO, Lucene.Net.Documents.Field.Index.ANALYZED));
            document.Add(new Lucene.Net.Documents.Field("key", bibtex_item.Key, Lucene.Net.Documents.Field.Store.NO, Lucene.Net.Documents.Field.Index.ANALYZED));

            foreach (KeyValuePair<string, string> pair in bibtex_item.Fields)
            {
                document.Add(new Lucene.Net.Documents.Field(pair.Key, pair.Value, Lucene.Net.Documents.Field.Store.NO, Lucene.Net.Documents.Field.Index.ANALYZED));
            }
        }

        // TODO: refactor call interface: way too many parameters to be legible.
        public void AddDocumentMetadata(bool is_deleted, string fingerprint, string title, string author, string year, string comment, string tag, string annotation, string bibtex, Utilities.BibTex.Parsing.BibTexItem bibtex_item)
        {
            Lucene.Net.Documents.Document document = null;

            // Create the document only if it is not to be deleted
            if (!is_deleted)
            {
                document = new Lucene.Net.Documents.Document();
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
            Lucene.Net.Documents.Document document = null;

            // Create the document only if it is not to be deleted
            if (!is_deleted)
            {
                document = new Lucene.Net.Documents.Document();
                document.Add(new Field("fingerprint", fingerprint, Field.Store.YES, Field.Index.NOT_ANALYZED_NO_NORMS));
                document.Add(new Field("page", Convert.ToString(page), Field.Store.YES, Field.Index.NOT_ANALYZED_NO_NORMS));
                document.Add(new Field("content", content, Field.Store.NO, Field.Index.ANALYZED));
            }

            AddDocumentPage_INTERNAL(fingerprint, page, document);
        }

        private void AddDocumentPage_INTERNAL(string fingerprint, int page, Document document)
        {
            // Write to the index
            // Utilities.LockPerfTimer l1_clk = Utilities.LockPerfChecker.Start();
            lock (index_writer_lock)
            {
                // l1_clk.LockPerfTimerStop();
                if (null == index_writer)
                {
                    Logging.Info("+Creating a new lucene IndexWriter");
                    index_writer = new Lucene.Net.Index.IndexWriter(LIBRARY_INDEX_BASE_PATH, analyzer, IndexWriter.MaxFieldLength.UNLIMITED);
                    Logging.Info("-Creating a new lucene IndexWriter");
                }

                // Delete the document if it already exists
                Lucene.Net.Search.BooleanQuery bq = new Lucene.Net.Search.BooleanQuery();
                bq.Add(new Lucene.Net.Search.TermQuery(new Lucene.Net.Index.Term("fingerprint", fingerprint)), Lucene.Net.Search.BooleanClause.Occur.MUST);
                bq.Add(new Lucene.Net.Search.TermQuery(new Lucene.Net.Index.Term("page", System.Convert.ToString(page))), Lucene.Net.Search.BooleanClause.Occur.MUST);
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
        public List<Utilities.Language.TextIndexing.IndexResult> GetDocumentsWithQuery(string query)
        {
            List<Utilities.Language.TextIndexing.IndexResult> fingerprints = new List<Utilities.Language.TextIndexing.IndexResult>();
            HashSet<string> fingerprints_already_seen = new HashSet<string>();

            try
            {
                using (Lucene.Net.Index.IndexReader index_reader = Lucene.Net.Index.IndexReader.Open(LIBRARY_INDEX_BASE_PATH, true))
                {
                    using (Lucene.Net.Search.IndexSearcher index_searcher = new Lucene.Net.Search.IndexSearcher(index_reader))
                    {
                        Lucene.Net.QueryParsers.QueryParser query_parser = new Lucene.Net.QueryParsers.QueryParser(Version.LUCENE_29, "content", analyzer);

                        Lucene.Net.Search.Query query_object = query_parser.Parse(query);
                        Lucene.Net.Search.Hits hits = index_searcher.Search(query_object);

                        var i = hits.Iterator();
                        while (i.MoveNext())
                        {
                            Lucene.Net.Search.Hit hit = (Lucene.Net.Search.Hit)i.Current;
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
                    }
                    index_reader.Close();
                }
            }
            catch (Exception ex)
            {
                Logging.Error(ex, $"GetDocumentsWithQuery: There was a problem opening the index file for searching (path: '{LIBRARY_INDEX_BASE_PATH}', query: '{query}')");
            }

            return fingerprints;
        }

        public List<IndexPageResult> GetDocumentPagesWithQuery(string query)
        {
            List<IndexPageResult> results = new List<IndexPageResult>();
            Dictionary<string, IndexPageResult> fingerprints_already_seen = new Dictionary<string, IndexPageResult>();

            try
            {
                using (IndexReader index_reader = IndexReader.Open(LIBRARY_INDEX_BASE_PATH, true))
                {
                    using (IndexSearcher index_searcher = new IndexSearcher(index_reader))
                    {
                        QueryParser query_parser = new QueryParser(Version.LUCENE_29, "content", analyzer);

                        Lucene.Net.Search.Query query_object = query_parser.Parse(query);
                        Lucene.Net.Search.Hits hits = index_searcher.Search(query_object);

                        var i = hits.Iterator();
                        while (i.MoveNext())
                        {
                            Lucene.Net.Search.Hit hit = (Lucene.Net.Search.Hit)i.Current;
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
                                result.page_results.Add(new PageResult { page = page, score = score });
                            }
                        }

                        // Close the index
                        index_searcher.Close();
                    }
                    index_reader.Close();
                }
            }
            catch (Exception ex)
            {
                Logging.Warn(ex, $"GetDocumentPagesWithQuery: There was a problem opening the index file for searching (path: '{LIBRARY_INDEX_BASE_PATH}', query: '{query}')");
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
                    //var segments_files = Directory.GetFiles(LIBRARY_INDEX_BASE_PATH, "segments*", SearchOption.AllDirectories);
                    //if (segments_files.Length <= 0)
                    //{
                    //    Logging.Debug("No index segments files found");
                    //    return fingerprints;
                    //}

                    using (IndexReader index_reader = IndexReader.Open(LIBRARY_INDEX_BASE_PATH, true))
                    {
                        using (IndexSearcher index_searcher = new IndexSearcher(index_reader))
                        {
                            Lucene.Net.Search.TermQuery term_query = new Lucene.Net.Search.TermQuery(new Term("content", keyword));
                            Lucene.Net.Search.Hits hits = index_searcher.Search(term_query);

                            var i = hits.Iterator();
                            while (i.MoveNext())
                            {
                                Lucene.Net.Search.Hit hit = (Lucene.Net.Search.Hit)i.Current;
                                string fingerprint = hit.Get("fingerprint");
                                fingerprints.Add(fingerprint);
                            }

                            // Close the index
                            index_searcher.Close();
                        }
                        index_reader.Close();
                    }
                }
            }
            catch (Exception ex)
            {
                Logging.Warn(ex, $"GetDocumentsWithWord: There was a problem opening the index file for searching (path: '{LIBRARY_INDEX_BASE_PATH}', keyword: '{keyword}')");
            }

            return fingerprints;
        }

        public List<string> GetDocumentsSimilarToDocument(string document_filename)
        {
            List<string> fingerprints = new List<string>();
            Lucene.Net.Search.Query query = null;

            try
            {
                using (IndexReader index_reader = IndexReader.Open(LIBRARY_INDEX_BASE_PATH, true))
                {
                    using (IndexSearcher index_searcher = new IndexSearcher(index_reader))
                    {
                        LuceneMoreLikeThis mlt = new LuceneMoreLikeThis(index_reader);
                        mlt.SetFieldNames(new string[] { "content" });
                        mlt.SetMinTermFreq(0);

                        query = mlt.Like(new StreamReader(document_filename));
                        Lucene.Net.Search.Hits hits = index_searcher.Search(query);
                        var i = hits.Iterator();
                        while (i.MoveNext())
                        {
                            Lucene.Net.Search.Hit hit = (Lucene.Net.Search.Hit)i.Current;
                            string fingerprint = hit.Get("fingerprint");
                            fingerprints.Add(fingerprint);
                        }

                        // Close the index
                        index_searcher.Close();
                    }
                    index_reader.Close();
                }
            }
            catch (Exception ex)
            {
                Logging.Warn(ex, $"GetDocumentsSimilarToDocument: There was a problem opening the index file for searching (path: '{LIBRARY_INDEX_BASE_PATH}', document: '{document_filename}' -> query: '{query}')");
            }

            return fingerprints;
        }

        // ---------------------------------------------------------

        private string VersionFilename => Path.GetFullPath(Path.Combine(LIBRARY_INDEX_BASE_PATH, @"index_version.txt"));

        private string LuceneWriteLockFilename => Path.GetFullPath(Path.Combine(LIBRARY_INDEX_BASE_PATH, @"write.lock"));

        public void InvalidateIndex()
        {
            Logging.Warn($"Invalidating Lucene index at '{LIBRARY_INDEX_BASE_PATH}' => nuking file '{VersionFilename}'.");
            FileTools.Delete(VersionFilename);
        }

        private void CheckIndexVersion()
        {
            string version = String.Empty;

            try
            {
                if (File.Exists(VersionFilename))
                {
                    string[] index_version_lines = File.ReadAllLines(VersionFilename);
                    version = index_version_lines[0].Trim();
                }
            }
            catch (Exception ex)
            {
                Logging.Error(ex, $"There was a problem while trying to check the index version (path: '{LIBRARY_INDEX_BASE_PATH}')");
            }

            if (0 != String.Compare(version, INDEX_VERSION))
            {
                Logging.Warn("This index is out of date (it's version is {0}), so deleting the index.", version);
                DeleteIndex();
            }
        }

        private void DeleteIndex()
        {
            Logging.Info($"Deleting the index at path '{LIBRARY_INDEX_BASE_PATH}'");
            Utilities.Files.DirectoryTools.DeleteDirectory(LIBRARY_INDEX_BASE_PATH, true);
        }
    }
}

#endif
