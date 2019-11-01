﻿using System;

namespace Qiqqa.DocumentLibrary.AITagsStuff
{
    public class BlackWhiteListEntry
    {
        public enum ListType
        {
            Black,
            White
        }

        public string word;
        public ListType list_type;
        public bool is_deleted;

        public BlackWhiteListEntry(string word, ListType list_type)
        {
            this.word = word;
            this.list_type = list_type;
            is_deleted = false;
        }

        public BlackWhiteListEntry(string line)
        {
            string[] bits = line.Split(new char[] { '|' }, StringSplitOptions.RemoveEmptyEntries);
            word = bits[0];
            list_type = FromListTypeString(bits[1]);
            is_deleted = (0 == bits[2].CompareTo("1"));
        }

        public override string ToString()
        {
            return ToFileString();
        }


        public string ToFileString()
        {
            return string.Format(
                "{0}|{1}|{2}",
                word,
                ToListTypeString(list_type),
                is_deleted ? "1" : "0"
                );
        }

        private static string ToListTypeString(ListType list_type)
        {
            if (false) { }
            else if (ListType.Black == list_type) { return "B"; }
            else if (ListType.White == list_type) { return "W"; }
            else throw new Exception("Unknown list type " + list_type);
        }

        private static ListType FromListTypeString(string list_type)
        {
            if (false) { }
            else if (0 == list_type.CompareTo("B")) { return ListType.Black; }
            else if (0 == list_type.CompareTo("W")) { return ListType.White; }
            else throw new Exception("Unknown list type " + list_type);
        }
    }
}
