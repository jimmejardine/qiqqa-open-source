using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

namespace Utilities.ProcessTools
{
    public class ProcessOutputReader : IDisposable
    {
        Process process;
        public List<string> Output = new List<string>();
        public List<string> Error = new List<string>();

        public ProcessOutputReader(Process process)
        {
            this.process = process;

            process.OutputDataReceived += (sender, e) => { Output.Add(e.Data); };
            process.ErrorDataReceived += (sender, e) => { Error.Add(e.Data); };
            process.BeginOutputReadLine();
            process.BeginErrorReadLine();
        }

            // -------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------

        ~ProcessOutputReader()
        {
            Logging.Info("~ProcessOutputReader()");
            Dispose(false);            
        }

        public void Dispose()
        {
            Logging.Info("Disposing ProcessOutputReader");
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        private void Dispose(bool disposing)
        {
            if (disposing)
            {
                // Get rid of managed resources
                process.CancelErrorRead();
                process.CancelOutputRead();
            }

            // Get rid of unmanaged resources 
        }

        // -------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
    

        public string GetOutputsDumpString()
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine("--- Standard output:");
            foreach (string s in Output) sb.AppendLine(s);
            sb.AppendLine("--- Standard error:");
            foreach (string s in Error) sb.AppendLine(s);
            return sb.ToString();
        }
    }
}
