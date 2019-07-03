using System.IO;
using System.Windows;
using Utilities.Strings;

namespace Utilities.Misc
{
    public class DragDropTools
    {
        public static string GetDataString(string format, DragEventArgs e)
        {
            MemoryStream ms = (MemoryStream)e.Data.GetData("UniformResourceLocator");
            byte[] data = ms.ToArray();
            return StringTools.StringFromByteArray(data);
        }
    }
}
