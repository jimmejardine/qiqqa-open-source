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
            Logging.Debug("~ProcessOutputReader()");
            Dispose(false);            
        }

        public void Dispose()
        {
            Logging.Debug("Disposing ProcessOutputReader");
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        private int dispose_count = 0;
        protected virtual void Dispose(bool disposing)
        {
            Logging.Debug("ProcessOutputReader::Dispose({0}) @{1}", disposing, dispose_count);

            if (dispose_count == 0)
            {
                // Get rid of managed resources
                process.CancelErrorRead();
                process.CancelOutputRead();

                Output.Clear();
                Error.Clear();
            }

            process = null;

            ++dispose_count;
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
