using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

namespace Utilities.ProcessTools
{
    public class ProcessOutputReader : IDisposable
    {
        private Process process;
        public List<string> Output = new List<string>();
        public List<string> Error = new List<string>();
        public readonly bool StdOutIsBinary;

        public ProcessOutputReader(Process process, bool stdout_is_binary = false)
        {
            this.process = process;
            this.StdOutIsBinary = stdout_is_binary;

            if (!stdout_is_binary)
            {
                process.OutputDataReceived += (sender, e) => { Output.Add(e.Data); };
            }
            process.ErrorDataReceived += (sender, e) => { Error.Add(e.Data); };
            if (!stdout_is_binary)
            {
                process.BeginOutputReadLine();
            }
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

            try
            {
                if (dispose_count == 0)
                {
                    // Get rid of managed resources
                    process.CancelErrorRead();
                    if (!StdOutIsBinary)
                    {
                        process.CancelOutputRead();
                    }

                    Output.Clear();
                    Error.Clear();
                }

                process = null;
            }
            catch (Exception ex)
            {
                Logging.Error(ex);
            }

            ++dispose_count;
        }

        // -------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------

        public string GetOutputsDumpString()
        {
            // oddly enough this code can produce a race condition exception for some Output: "Collection was modified; enumeration operation may not execute."
            //
            // HACK: we cope with that by re-iterating over the list until success is ours...   :-S :-S  hacky!
			Exception odd_ex = null;
			
            for (int i = 10; i > 0; i--)
            {
                try
                {
                    StringBuilder sb = new StringBuilder();
                    sb.AppendLine("--- Standard output:");
                    foreach (string s in Output)
                    {
                        sb.AppendLine(s);
                    }
                    sb.AppendLine("--- Standard error:");
                    foreach (string s in Error)
                    {
                        sb.AppendLine(s);
                    }
                    return sb.ToString();
                }
                catch (Exception ex)
                {
					odd_ex = ex;
                    Logging.Error(ex, "GetOutputsDumpString failed with this odd condition...");
                }
            }
			
			throw new Exception("Failure", odd_ex);
        }
    }
}
