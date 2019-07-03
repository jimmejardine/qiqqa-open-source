using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Qiqqa.Common.Configuration;
using Utilities;

namespace Qiqqa.Synchronisation.PDFSync
{
    /// <summary>
    /// This class keeps track of which PDFs the server has told us it already owns so that the client does not have to interrogate again whether or not to upload a specific PDF.
    /// </summary>
    class PDFsAlreadyStoredOnServerCache
    {
        public static readonly PDFsAlreadyStoredOnServerCache Instance =new PDFsAlreadyStoredOnServerCache();

        private bool is_dirty = false;
        private HashSet<string> tokens = new HashSet<string>();
        
        private PDFsAlreadyStoredOnServerCache()
        {
            Load();
        }


        private string FILENAME
        {
            get
            {
                return ConfigurationManager.Instance.BaseDirectoryForUser + @"Qiqqa.server_stored_pdfs_cache";
            }
        }

        private void Load()
        {
            lock (tokens)
            {
                try
                {
                    if (File.Exists(FILENAME))
                    {
                        string[] lines = File.ReadAllLines(FILENAME);
                        foreach (var line in lines)
                        {
                            tokens.Add(line);
                        }
                    }
                }
                catch (Exception ex)
                {
                    Logging.Warn(ex, "There was a problem loading {0}", FILENAME);
                }
            }
        }

        public void Save()
        {
            lock (tokens)
            {
                if (!is_dirty) return;
                try
                {
                    File.WriteAllLines(FILENAME, tokens.ToArray());
                }
                catch (Exception ex)
                {
                    Logging.Warn(ex, "There was a problem saving {0}", FILENAME);
                }
            }
        }


        public void Add(string token)
        {
            lock (tokens)
            {
                if (tokens.Add(token))
                {
                    is_dirty = true;
                }
            }
        }

        public bool IsAlreadyCached(string token)
        {
            lock (tokens)
            {
                return tokens.Contains(token);
            }
        }
    }
}
