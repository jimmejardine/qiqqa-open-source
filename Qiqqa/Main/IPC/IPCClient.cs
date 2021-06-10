using System;
using System.IO;
using System.IO.Pipes;
using Utilities;
using Directory = Alphaleonis.Win32.Filesystem.Directory;
using File = Alphaleonis.Win32.Filesystem.File;
using Path = Alphaleonis.Win32.Filesystem.Path;


namespace Qiqqa.Main.IPC
{
    internal class IPCClient
    {
        public static void SendMessage(string msg)
        {
            try
            {
                using (var client = new NamedPipeClientStream(".", IPCCommon.PIPE_NAME, PipeDirection.InOut))
                {
                    client.Connect(500);
                    using (StreamWriter sw = new StreamWriter(client))
                    {
                        sw.WriteLine(msg);
                        sw.Flush();
                    }
                }
            }
            catch (Exception ex)
            {
                Logging.Error(ex, "Error while sending IPC message");
            }
        }
    }
}
