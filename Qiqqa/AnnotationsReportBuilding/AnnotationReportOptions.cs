using System;
using System.Collections.Generic;
using System.Text;

namespace Qiqqa.AnnotationsReportBuilding
{
    public class AnnotationReportOptions
    {
        public HashSet<string> filter_tags = null;
        public bool ObeySuppressedImages;
        public bool ObeySuppressedText;
        public bool SuppressAllImages;
        public bool SuppressAllText;

        public bool IncludeComments = false;
        public bool IncludeAbstract = false;
        public bool IncludeAllPapers = false;

        public bool SuppressPDFDocumentHeader = false;
        public bool SuppressPDFAnnotationTags = false;

        public bool FilterByCreationDate = false;
        public DateTime FilterByCreationDate_From = DateTime.MinValue;
        public DateTime FilterByCreationDate_To = DateTime.MaxValue;
        public bool FilterByFollowUpDate = false;
        public DateTime FilterByFollowUpDate_From = DateTime.MinValue;
        public DateTime FilterByFollowUpDate_To = DateTime.MaxValue;
        public bool FilterByCreator = false;
        public string FilterByCreator_Name = null;

        public int InitialRenderDelayMilliseconds = 0;
    }
}
