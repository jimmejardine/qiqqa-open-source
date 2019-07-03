using System;

namespace Qiqqa.Common.MessageBoxControls
{
    [Serializable]
    public class UsefulTextException : Exception
    {
        public string header;
        public string body;

        public UsefulTextException(string header, string body, Exception ex) : 
            base(header + ": " + body, ex)
        {
            this.header = header;
            this.body = body;            
        }
    }
}
