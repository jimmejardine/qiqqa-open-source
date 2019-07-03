using Newtonsoft.Json;

namespace Qiqqa.DocumentLibrary.BundleLibrary
{
    class BundleLibraryManifest
    {
        public string Id { get; set; }
        public string Version { get; set; }

        public string Title { get; set; }
        public string Description { get; set; }
        public string Comments { get; set; }

        public string BaseUrl { get; set; }
        public string SupportEmail { get; set; }
        
        public bool IncludesPDFs { get; set; }

        public string ToJSON()
        {
            return JsonConvert.SerializeObject(this, Formatting.Indented);
        }

        public static BundleLibraryManifest FromJSON(string json)
        {
            BundleLibraryManifest manifest = JsonConvert.DeserializeObject<BundleLibraryManifest>(json);
            return manifest;
        }
    }
}
