using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Utilities.Misc
{
    public static class ExceptionHelpers
    {
        // https://stackoverflow.com/questions/8039660/net-how-to-convert-exception-to-string
        //
        // 
        private static void WriteExceptionDetails(StringBuilder builderToFill, Exception exception, int level)
        {
            var indent = new string(' ', level);

            if (level > 0)
            {
                builderToFill.AppendLine(indent + "=== INNER EXCEPTION ===");
            }

            var exType = exception.GetType();
            builderToFill.AppendLine(indent + exType.FullName);

            void append(string prop, Func<object, string> fmt = null)
            {
                var propInfo = exType.GetProperty(prop);
                var val = propInfo?.GetValue(exception) ?? null;

                if (val != null)
                {
                    if (fmt == null)
                    {
                        builderToFill.AppendFormat("{0}{1}: {2}{3}", indent, prop, val.ToString(), Environment.NewLine);
                    }
                    else
                    {
                        builderToFill.AppendFormat("{0}{1}: {2}{3}", indent, prop, fmt(val), Environment.NewLine);
                    }
                }
            }

            string winErrCodeToString(object errcode)
            {
                return String.Format("0x{0:X8}", errcode);
            }

            append("Message");
            append("ErrorCode", winErrCodeToString);
            append("HResult", winErrCodeToString);
            append("HelpLink");
            append("Source");
            append("StackTrace");
            append("TargetSite");

            foreach (DictionaryEntry de in exception.Data)
            {
                builderToFill.AppendFormat("{0} {1} = {2}{3}", indent, de.Key, de.Value, Environment.NewLine);
            }

            if (exception.InnerException != null)
            {
                WriteExceptionDetails(builderToFill, exception.InnerException, level + 1);
            }
        }
        //
        // and call it via extension: 
        //
        public static string ToStringAllExceptionDetails(this Exception exception)
        {
            StringBuilder builderToFill = new StringBuilder();

#if false
            {
                PropertyInfo[] properties = exception.GetType()
                                        .GetProperties();
                List<string> fields = new List<string>();
                foreach (PropertyInfo property in properties)
                {
                    object value = property.GetValue(exception, null);
                    fields.Add(String.Format(
                                     "{0} = {1}",
                                     property.Name,
                                     value != null ? value.ToString() : String.Empty
                    ));
                }
                builderToFill.AppendLine(String.Join("\n", fields.ToArray()));
            }
#endif

            WriteExceptionDetails(builderToFill, exception, 0);
            return builderToFill.ToString();
        }
    }
}
