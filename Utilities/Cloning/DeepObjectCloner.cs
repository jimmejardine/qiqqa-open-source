using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

namespace Utilities.Cloning
{
    /// <summary>
    /// Creates a deep clone by object serialising and then deserialising.
    /// </summary>
    public class DeepObjectCloner
    {
        public static T DeepClone<T>(T object_to_clone)
        {
            using (var ms = new MemoryStream())
            {
                var binary_formatter = new BinaryFormatter();
                binary_formatter.Serialize(ms, object_to_clone);
                ms.Flush();
                ms.Seek(0, 0);
                return (T)binary_formatter.Deserialize(ms);
            }
        }
    }
}
