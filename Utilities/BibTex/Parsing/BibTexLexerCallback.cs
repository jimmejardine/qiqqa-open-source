using System.Collections.Generic;

namespace Utilities.BibTex.Parsing
{
    public interface BibTexLexerCallback
    {
        void RaiseEntryName(string entry_name);
        void RaiseKey(string key);
        void RaiseFieldName(string field_name);
        void RaiseFieldValue(List<string> field_values);
        void RaiseFinished();
        void RaiseException(string msg);
        void RaiseWarning(string msg);
        void RaiseComment(string comment_content);
    }
}
