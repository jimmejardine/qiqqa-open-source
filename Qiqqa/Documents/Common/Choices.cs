using System.Collections.Generic;

namespace Qiqqa.Documents.Common
{
    public class Choices
    {
        public static readonly List<string> Ratings = new List<string>
        (
            new string[] { "", "1", "2", "3", "4", "5" }
        );

        public static readonly string ReadingStages_INTERRUPTED = "Interrupted";
        public static readonly string ReadingStages_TOP_PRIORITY = "Top priority";
        public static readonly string ReadingStages_READ_AGAIN = "Read again";
        public static readonly string ReadingStages_UNREAD = "Unread";
        public const string ReadingStages_DUPLICATE = "Duplicate";
        
        public static readonly List<string> ReadingStages = new List<string>
        (
            new string[]             
            { 
                "",
                ReadingStages_TOP_PRIORITY,
                "Started reading",                
                ReadingStages_INTERRUPTED,
                ReadingStages_UNREAD,
                ReadingStages_READ_AGAIN,
                "Finished reading",
                "Browsed",
                "Skim read",
                "Interest only",
                ReadingStages_DUPLICATE
            }
        );
    }
}
