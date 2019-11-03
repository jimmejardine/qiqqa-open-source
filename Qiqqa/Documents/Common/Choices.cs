using System.Collections.Generic;

namespace Qiqqa.Documents.Common
{
    public static class Choices
    {
        public static readonly List<string> Ratings = new List<string>
        (
            new string[] { "", "1", "2", "3", "4", "5" }
        );

        public const string ReadingStages_INTERRUPTED = "Interrupted";
        public const string ReadingStages_TOP_PRIORITY = "Top priority";
        public const string ReadingStages_READ_AGAIN = "Read again";
        public const string ReadingStages_UNREAD = "Unread";
        public const string ReadingStages_DUPLICATE = "Duplicate";
        public const string ReadingStages_STARTED_READING = "Started reading";
        public const string ReadingStages_FINISHED_READING = "Finished reading";
        public const string ReadingStages_BROWSED = "Browsed";
        public const string ReadingStages_SKIM_READ = "Skim read";
        public const string ReadingStages_INTEREST_ONLY = "Interest only";

        public static readonly List<string> ReadingStages = new List<string>
        (
            new string[]
            {
                "",
                ReadingStages_TOP_PRIORITY,
                ReadingStages_STARTED_READING,
                ReadingStages_INTERRUPTED,
                ReadingStages_UNREAD,
                ReadingStages_READ_AGAIN,
                ReadingStages_FINISHED_READING,
                ReadingStages_BROWSED,
                ReadingStages_SKIM_READ,
                ReadingStages_INTEREST_ONLY,
                ReadingStages_DUPLICATE
            }
        );
    }
}
