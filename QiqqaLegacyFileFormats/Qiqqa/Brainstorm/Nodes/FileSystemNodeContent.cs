using System;
using System.Globalization;

namespace QiqqaLegacyFileFormats          // namespace Qiqqa.Brainstorm.Nodes
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
            return (null != path) && path.ToLower().Contains(keyword);
        }
    }
}
