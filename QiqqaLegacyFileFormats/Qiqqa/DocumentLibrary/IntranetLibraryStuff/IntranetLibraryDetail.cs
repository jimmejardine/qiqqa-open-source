using System;
using System.IO;
using Newtonsoft.Json;

namespace QiqqaLegacyFileFormats          // namespace Qiqqa.DocumentLibrary.IntranetLibraryStuff
{
    internal class IntranetLibraryDetail
    {
        public string Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }

        internal static IntranetLibraryDetail Read(string library_detail_path)
        {
            string json = File.ReadAllText(library_detail_path);
            return JsonConvert.DeserializeObject<IntranetLibraryDetail>(json);
        }

        internal static void Write(string library_detail_path, IntranetLibraryDetail library_detail)
        {
            string json = JsonConvert.SerializeObject(library_detail);
            File.WriteAllText(library_detail_path, json);
        }

        internal static string GetRandomId()
        {
            return "INTRANET_" + Guid.NewGuid().ToString().ToUpper();
        }
    }
}
