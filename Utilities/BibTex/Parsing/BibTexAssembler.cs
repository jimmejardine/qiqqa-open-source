using System.Collections.Generic;
using Utilities.Strings;

namespace Utilities.BibTex.Parsing
{
    internal class BibTexAssembler : BibTexLexerCallback
    {
        internal static BibTexParseResult Parse(string bibtex)
        {
            BibTexAssembler assembler = new BibTexAssembler();
            BibTexLexer lexer = new BibTexLexer(bibtex);
            lexer.Parse(assembler);
            return new BibTexParseResult(assembler.items, assembler.comments);
        }

        private List<BibTexItem> items = new List<BibTexItem>();
        private BibTexItem current_item = null;
        private string current_field_name = null;
        private List<string> comments = new List<string>();

        // ------------------------------------------------------------------------------------

        private BibTexAssembler()
        {
        }

        // ------------------------------------------------------------------------------------

        public void RaiseEntryName(string entry_name)
        {
            //Logging.Info("EntryName=" + entry_name);

            ClearDownCurrentItem();

            current_item = new BibTexItem();
            current_item.Type = entry_name;
        }

        public void RaiseKey(string key)
        {
            //Logging.Info("Key=" + key);            

            if (null == current_item)
            {
                Logging.Error("Invalid key outside item: '{0}'", key);
            }
            else
            {
                current_item.Key = key;
            }
        }

        public void RaiseFieldName(string field_name)
        {
            //Logging.Info("FieldName=" + field_name);

            if (null == current_item)
            {
                Logging.Error("Invalid field name outside item: '{0}'", field_name);
            }
            else
            {
                current_field_name = field_name.ToLower();
            }
        }

        public void RaiseFieldValue(List<string> field_values)
        {
            //Logging.Debug("FieldValue=", StringTools.ConcatenateStrings(field_values, " # "));

            if (null == current_item)
            {
                Logging.Error("Invalid field value outside item: '{0}'", StringTools.ConcatenateStrings(field_values, " # "));
            }
            else if (null == current_field_name)
            {
                Logging.Error("Invalid field value without field name: '{0}'", StringTools.ConcatenateStrings(field_values, " # "));
            }
            else
            {
                string field_value = StringTools.ConcatenateStrings(field_values, "");
                string field_value_decoded = BibTexCharacterMap.BibTexToASCII(field_value);
                current_item[current_field_name] = field_value_decoded;
                current_field_name = null;
            }
        }

        public void RaiseFinished()
        {
            //Logging.Info("Finished");
            ClearDownCurrentItem();
        }

        public void RaiseException(string msg)
        {
            if (null == current_item)
            {
                Logging.Error("An error occurred while parsing the bibtex:\n{0}", msg);
            }
            else
            {
                current_item.Exceptions.Add(msg);
            }
        }

        public void RaiseWarning(string msg)
        {
            if (null == current_item)
            {
                Logging.Warn("A warning occurred while parsing the bibtex:\n{0}", msg);
            }
            else
            {
                current_item.Warnings.Add(msg);
            }
        }

        public void RaiseComment(string comment_content)
        {
            comments.Add(comment_content);
        }

        private void ClearDownCurrentItem()
        {
            // If we were already building one, it is done
            if (null != current_item)
            {
                items.Add(current_item);
            }

            // Reset our memory
            current_item = null;
            current_field_name = null;
        }

        // ------------------------------------------------------------------------------------
    }
}
