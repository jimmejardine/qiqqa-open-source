using System;

namespace Utilities.Misc
{
    public class ArrayExcelColumnMapper
    {
        private string[] parts;
        private IFormatProvider fallback_format_provider;

        public ArrayExcelColumnMapper(string[] parts, IFormatProvider format_provider)
        {
            this.parts = parts;
            fallback_format_provider = format_provider;
        }

        public string this[string C]
        {
            get
            {
                C = C.ToUpper();

                int offset = 0;
                for (int i = 0; i < C.Length; ++i)
                {
                    char c = C[i];
                    int value = c - 'A' + 1;
                    offset = offset * 26 + value;
                }

                string s = parts[offset - 1];
                if (String.IsNullOrEmpty(s)) return null;
                return s;
            }
        }

        public double? AsDoubleNullable(string C)
        {
            string s = this[C];
            if (String.IsNullOrEmpty(s)) return null;
            return Convert.ToDouble(s, fallback_format_provider);
        }

        public double AsDouble(string C)
        {
            string s = this[C];
            return Convert.ToDouble(s, fallback_format_provider);
        }

        public DateTime? AsDateTimeNullable(string C, IFormatProvider format_provider)
        {
            if (null == format_provider) format_provider = fallback_format_provider;

            string s = this[C];
            if (String.IsNullOrEmpty(s)) return null;
            DateTime result = DateTime.Parse(s, format_provider);
            result = result.Date;
            return result;
        }

        public DateTime AsDateTime(string C, IFormatProvider format_provider)
        {
            if (null == format_provider) format_provider = fallback_format_provider;

            string s = this[C];
            DateTime result = DateTime.Parse(s, format_provider);
            result = result.Date;
            return result;
        }
    }
}
