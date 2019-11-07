using System;
using System.IO;
using System.IO.Pipes;
using System.Security.AccessControl;
using Utilities;

namespace Qiqqa.Main.IPC
{
    internal class IPCServer
    {
        public delegate void IPCServerMessageDelegate(string message);
        public event IPCServerMessageDelegate IPCServerMessage;

        private bool is_running;

        public IPCServer()
        {
            is_running = false;
        }

        public void Start()
        {
            is_running = true;
            StartServerPump();
        }

        public void Stop()
        {
            is_running = false;
        }


        private void StartServerPump()
        {
            if (!is_running)
            {
                return;
            }

            try
            {
                PipeSecurity ps = new PipeSecurity();
                ps.AddAccessRule(new PipeAccessRule("Everyone", PipeAccessRights.ReadWrite, AccessControlType.Allow));

                using (var npss = new NamedPipeServerStream(IPCCommon.PIPE_NAME, PipeDirection.InOut, 1, PipeTransmissionMode.Byte, PipeOptions.Asynchronous, 512, 512, ps))
                {
                    npss.BeginWaitForConnection(async_result =>
                        {
                            try
                            {
                                using (var npss_in_callback = (NamedPipeServerStream)async_result.AsyncState)
                                {
                                    npss_in_callback.EndWaitForConnection(async_result);
                                    npss_in_callback.WaitForPipeDrain();

                                    StreamReader sr = new StreamReader(npss_in_callback);
                                    var line = sr.ReadLine();
                                    IPCServerMessage?.Invoke(line);

                                    npss_in_callback.Close();

                                    // Listen for another client.  Note that this is NOT recursive as we are currently inside a lambda.
                                    StartServerPump();
                                }
                            }
                            catch (Exception ex)
                            {
                                Logging.Error(ex, "Error while processing pipe connection. ({0})", IPCCommon.PIPE_NAME);
                            }
                        },
                        npss);
                }
            }
            catch (Exception ex)
            {
                Logging.Error(ex, "Error while waiting for pipe connection. ({0})", IPCCommon.PIPE_NAME);
            }
        }
    }
}
