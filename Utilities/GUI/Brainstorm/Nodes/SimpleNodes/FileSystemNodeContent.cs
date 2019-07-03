using System;
using System.Globalization;
using Utilities.GUI.Brainstorm.Common.Searching;

namespace Utilities.GUI.Brainstorm.Nodes.SimpleNodes
{
    [Serializable]
    public class FileSystemNodeContent : Searchable
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
