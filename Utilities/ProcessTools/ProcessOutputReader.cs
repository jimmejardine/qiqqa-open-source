using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using Utilities.GUI;

namespace Utilities.ProcessTools
{
    public class ProcessOutputReader : IDisposable
    {
        private Process process;
        public List<string> Output = new List<string>();
        public List<string> Error = new List<string>();
        private object io_buffers_lock = new object();
        public readonly bool StdOutIsBinary;

        public ProcessOutputReader(Process process, bool stdout_is_binary = false)
        {
            WPFDoEvents.AssertThisCodeIs_NOT_RunningInTheUIThread();

            this.process = process;
            this.StdOutIsBinary = stdout_is_binary;

            if (!stdout_is_binary)
            {
                process.OutputDataReceived += (sender, e) => {
                    // terminated processes CAN produce one last event where e.Data == null: this does not add anything
                    // and causes GetOutputsDumpString() to fail with an internal List<> exception error otherwise:
                    if (e.Data != null)
                    {
                        lock (io_buffers_lock)
                        {
                            Output.Add(e.Data);
                        }
                    }
                };
            }
            process.ErrorDataReceived += (sender, e) => {
                // terminated processes CAN produce one last event where e.Data == null: this does not add anything
                // and causes GetOutputsDumpString() to fail with an internal List<> exception error otherwise:
                if (e.Data != null)
                {
                    lock (io_buffers_lock)
                    {
                        Error.Add(e.Data);
                    }
                }
            };
            process.Exited += (sender, e) => {
                lock (io_buffers_lock)
                {
                    Error.Add("--EXIT--");
                }
            }; 
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

            WPFDoEvents.SafeExec(() =>
            {
                if (dispose_count == 0)
                {
                    // Get rid of managed resources
                    process.CancelErrorRead();
                    if (!StdOutIsBinary)
                    {
                        process.CancelOutputRead();
                    }

                    lock (io_buffers_lock)
                    {
                        Output.Clear();
                        Error.Clear();
                    }
                }
            });

            WPFDoEvents.SafeExec(() =>
            {
                process = null;
            });

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
                    lock (io_buffers_lock)
                    {
                        foreach (string s in Output)
                        {
                            sb.AppendLine(s);
                        }
                    }
                    sb.AppendLine("--- Standard error:");
                    lock (io_buffers_lock)
                    {
                        foreach (string s in Error)
                        {
                            sb.AppendLine(s);
                        }
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
