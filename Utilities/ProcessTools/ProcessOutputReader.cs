using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Utilities.GUI;
using Directory = Alphaleonis.Win32.Filesystem.Directory;
using File = Alphaleonis.Win32.Filesystem.File;
using Path = Alphaleonis.Win32.Filesystem.Path;


namespace Utilities.ProcessTools
{
    public struct ProcessOutputDump
    {
        public string stdout;
        public string stderr;

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();

            if (!String.IsNullOrWhiteSpace(stdout))
            {
                sb.AppendLine("--- Standard output:");
                sb.AppendLine(stdout);
            }
            if (!String.IsNullOrWhiteSpace(stderr))
            {
                sb.AppendLine("--- Standard error:");
                sb.AppendLine(stderr);
            }
            return sb.ToString();
        }
    }

    // -------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------

    public class ProcessOutputReader : IDisposable
    {
        private Process process;
        public List<string> Output = new List<string>();
        public byte[] BinaryOutput = null;
        public List<string> Error = new List<string>();
        private object io_buffers_lock = new object();
        public readonly bool StdOutIsBinary;

        private CancellationTokenSource CancelToken = new CancellationTokenSource();
        private TaskCompletionSource<object> waitOutputDataCompleted = null;
        private TaskCompletionSource<object> waitErrorDataCompleted = null;

        public ProcessOutputReader(Process process, bool stdout_is_binary = false)
        {
            WPFDoEvents.AssertThisCodeIs_NOT_RunningInTheUIThread();

            this.process = process;
            this.StdOutIsBinary = stdout_is_binary;

            waitOutputDataCompleted = new TaskCompletionSource<object>();
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
                    else
                    {
                        waitOutputDataCompleted.TrySetResult(null);
                    }
                };
            }
            else
            {
                BinaryOutput = null;
            }

            waitErrorDataCompleted = new TaskCompletionSource<object>();
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
                else
                {
                    waitErrorDataCompleted.TrySetResult(null);
                }
            };
            process.Exited += async (sender, e) => {
                await waitErrorDataCompleted.Task.ConfigureAwait(false);
                //await waitOutputDataCompleted.Task.ConfigureAwait(false);

                lock (io_buffers_lock)
                {
                    try
                    {
                        // under rare circumstances this line can crash:
                        /*
                          System.InvalidOperationException
                          HResult = 0x80131509
                          Message = No process is associated with this object.
                            Source = System
                          StackTrace:
                           at System.Diagnostics.Process.EnsureState(State state)
                           at System.Diagnostics.Process.get_HasExited()
                           at System.Diagnostics.Process.EnsureState(State state)
                           at System.Diagnostics.Process.get_ExitCode()
                           at Utilities.ProcessTools.ProcessOutputReader.<> c__DisplayClass9_0.<< -ctor > b__2 > d.MoveNext() in Z:\lib\tooling\qiqqa\Utilities\ProcessTools\ProcessOutputReader.cs:line 103

                          This exception was originally thrown at this call stack:
                            [External Code]
                            Utilities.ProcessTools.ProcessOutputReader..ctor.AnonymousMethod__2(object, System.EventArgs) in ProcessOutputReader.cs
                        */
                        int rv = process.ExitCode;
                        Error.Add($"--EXIT:{process.ExitCode}--");
                    }
                    catch (Exception ex)
                    {
                        Logging.Error(ex, "ProcessOutputReader::process.ErrorDataReceived handler event borked.");

                        Error.Add($"--EXIT:FAILED TO PRODUCE AN EXITCODE--");
                    }
                }

                CancelToken.Cancel();
            };

            try
            {
                ProcessPriorityClass pcl = process.PriorityClass;
                int bp = process.BasePriority;
            }
            catch (Exception ex)
            {
                if (!process.Start())
                {
                    throw new InvalidOperationException("Can't start process. FileName:" + process.StartInfo.FileName + ", Arguments:" + process.StartInfo.Arguments);
                }
            }

            if (!stdout_is_binary)
            {
                process.BeginOutputReadLine();
            }
            else
            {
                // ripped from Cysharp/ProcessX after a long journey via:
                // - https://devblogs.microsoft.com/oldnewthing/20110707-00/?p=10223
                // - https://unix.stackexchange.com/questions/343302/anonymous-pipe-kernel-buffer-size
                // - https://unix.stackexchange.com/questions/11946/how-big-is-the-pipe-buffer
                // - https://stackoverflow.com/questions/33553837/win32-named-pipes-and-message-size-limits-is-the-old-64k-limit-still-applicabl
                // - https://stackoverflow.com/questions/12028986/c-sharp-reading-writing-bytes-to-from-console
                // - only to find out that the whole pipe buffer size is a no-go for C#/.NET as no matter what I tried, the STDOUT output would
                //   block very shortly after startup when NOT redirected, or would NOT DELIVER ANYTHING at all on the caller/.NET side:
                //   that's how this all started again: suddenly the code which worked before didn't want to play any more since today!
                // - https://docs.microsoft.com/en-us/windows/console/writeconsole
                // - https://docs.microsoft.com/en-us/windows/console/getconsolemode
                // - https://docs.microsoft.com/en-us/windows/console/console-handles
                // - https://docs.microsoft.com/en-us/windows/win32/api/fileapi/nf-fileapi-createfilea
                // - https://docs.microsoft.com/en-us/windows/console/console-functions
                // - Only to find at the C++/Called side of these matters that writing to redirected to redirected stdout is sort-of-nice
                //   while the crap is redirected via pipe to .NET but goes bonkers and crashing immediately when NOT redirected.
                //   This includes *system* alerts about me trying to overrun Kernel stack and being warned a malicious process
                //   (that would be me!) is attempting to gain access via buffer overrun attempts.
                //   See mupdf::output.c:: write_stdout() for that ordeal.
                // - https://docs.microsoft.com/en-us/dotnet/standard/io/pipelines
                // - https://adamsitnik.com/Array-Pool/
                // - https://github.com/Microsoft/Microsoft.IO.RecyclableMemoryStream
                // - https://jonskeet.uk/csharp/readbinary.html
                // - https://docs.microsoft.com/en-us/dotnet/api/system.io.stream.readasync?view=net-5.0
                // - https://docs.microsoft.com/en-us/dotnet/api/system.diagnostics.process.outputdatareceived?view=net-5.0
                // - https://multipleinheritance.wordpress.com/2012/09/05/process-async-outputdatareceivederrordatareceived-has-a-flaw-when-dealing-with-prompts/
                //   ^^^ FINALLY someone seemed to talked some sense and relevance there. Bugger with the account-walled ZIP with source files, though!
                //       That did look fishy!
                // - https://social.msdn.microsoft.com/Forums/vstudio/en-US/be4b7f96-fec0-45fe-a8d5-e262f2f9a219/outputdatareceived-from-process-waits-until-the-end
                // - https://docs.microsoft.com/en-us/dotnet/api/system.diagnostics.process.outputdatareceived?redirectedfrom=MSDN&view=net-5.0
                // - https://stackoverflow.com/questions/4143281/capturing-binary-output-from-process-standardoutput
                //   ^^^ AH. That answer definitely points the way. Now to see if someone has gone before me, for real?
                // - https://stackoverflow.com/questions/17043631/using-stream-read-vs-binaryreader-read-to-process-binary-streams
                // - https://stackoverflow.com/questions/8897656/when-to-use-byte-array-and-when-to-use-stream
                // - https://stackoverflow.com/questions/43000681/why-do-most-serializers-use-a-stream-instead-of-a-byte-array
                // - So far, all fluff.
                // - https://social.msdn.microsoft.com/Forums/vstudio/en-US/7f834606-2ea8-4072-9b4f-e4dfa4852c3c/standard-output-as-binary
                // - https://github.com/Cysharp/ProcessX
                //   ^^^ BINGO.
                // - https://github.com/jamesmanning/RunProcessAsTask
                //   ^^^ Fluff. Doesn't bother with binary stdout; all known, nothing new here.
                RunAsyncReadBinaryFully(process.StandardOutput.BaseStream, this, CancelToken.Token);
            }
            process.BeginErrorReadLine();
        }

        private static async void RunAsyncReadBinaryFully(Stream stream, ProcessOutputReader completion, CancellationToken cancellationToken)
        {
            try
            {
                var ms = new MemoryStream();
                await stream.CopyToAsync(ms, 81920, cancellationToken);
                var result = ms.ToArray();
                lock (completion.io_buffers_lock)
                {
                    completion.BinaryOutput = result;
                }
            }
            catch (Exception ex)
            {
                lock (completion.io_buffers_lock)
                {
                    completion.BinaryOutput = null;
                }
                Logging.Error(ex, "RunAsyncReadBinaryFully failed with this odd condition...");
            }
            finally
            {
                completion.waitOutputDataCompleted.TrySetResult(null);
            }
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
                    waitOutputDataCompleted?.TrySetResult(null);
                    waitErrorDataCompleted?.TrySetResult(null);
                    CancelToken.Cancel();

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

        public ProcessOutputDump GetOutputsDumpStrings()
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

                    lock (io_buffers_lock)
                    {
                        foreach (string s in Output)
                        {
                            sb.AppendLine(s);
                        }
                    }

                    StringBuilder sb2 = new StringBuilder();

                    lock (io_buffers_lock)
                    {
                        foreach (string s in Error)
                        {
                            sb2.AppendLine(s);
                        }
                    }
                    return new ProcessOutputDump()
                    {
                        stdout = sb.ToString(),
                        stderr = sb2.ToString()
                    };
                }
                catch (Exception ex)
                {
                    odd_ex = ex;
                    Logging.Error(ex, "GetOutputsDumpStrings failed with this odd condition...");
                }
            }
			
			throw new Exception("Failure", odd_ex);
        }
    }
}
