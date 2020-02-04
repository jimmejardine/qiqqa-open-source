using System.Collections.Generic;
using Qiqqa.Documents.Common;
using Qiqqa.Documents.PDF;

namespace Qiqqa.DocumentLibrary
{
    public class ReadingStageManager
    {
        #region --- Singleton ------------------------------------------------------------------------------------

        public static readonly ReadingStageManager Instance = new ReadingStageManager();

        private ReadingStageManager()
        {
        }

        #endregion

        private HashSet<string> tags = new HashSet<string>();
        private object tags_lock = new object();

        internal void ProcessDocument(PDFDocument pdf_document)
        {
            if (null != pdf_document.ReadingStage && !Choices.ReadingStages.Contains(pdf_document.ReadingStage))
            {
                // Utilities.LockPerfTimer l1_clk = Utilities.LockPerfChecker.Start();
                lock (tags_lock)
                {
                    // l1_clk.LockPerfTimerStop();
                    tags.Add(pdf_document.ReadingStage);
                }
            }
        }

        public List<string> ReadingStages
        {
            get
            {
                // Utilities.LockPerfTimer l1_clk = Utilities.LockPerfChecker.Start();
                lock (tags_lock)
                {
                    // l1_clk.LockPerfTimerStop();
                    List<string> results = new List<string>();
                    results.AddRange(tags);
                    results.Sort();
                    results.AddRange(Choices.ReadingStages);
                    return results;
                }
            }
        }
    }
}
