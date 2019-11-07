namespace Qiqqa.Common.WebcastStuff
{
    public class Webcast
    {
        public string key;
        public string title;
        public string description;
        public string url;

        public Webcast(string key, string title, string description, string url)
        {
            this.key = key;
            this.title = title;
            this.description = description;
            this.url = url;
        }
    }
}
