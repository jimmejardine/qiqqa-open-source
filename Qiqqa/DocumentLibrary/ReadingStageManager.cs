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

        HashSet<string> tags = new HashSet<string>();

        internal void ProcessDocument(PDFDocument pdf_document)
        {
            if (null != pdf_document.ReadingStage && !Choices.ReadingStages.Contains(pdf_document.ReadingStage))
            {
                lock (tags)
                {
                    tags.Add(pdf_document.ReadingStage);
                }
            }
        }

        public List<string> ReadingStages
        {
            get
            {   
                lock (tags)
                {
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
