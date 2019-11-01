namespace Utilities.Files
{
    public class CSV
    {
        // You may prefer StringTools.Split_NotInDelims
        public static string[] splitAtCommas(string source)
        {
            return source.Split(',');
        }
    }
}
