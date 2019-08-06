using System;
using System.Globalization;
using Qiqqa.Brainstorm.Common.Searching;

namespace Qiqqa.Brainstorm.Nodes
{
    [Serializable]
    public class FileSystemNodeContent : ISearchable
    {
        public string path;

        public FileSystemNodeContent(string path)
        {
            this.path = path;
        }

        public bool MatchesKeyword(string keyword)
        {
            return (null != path) && path.ToLower(CultureInfo.CurrentCulture).Contains(keyword);
        }
    }
}
