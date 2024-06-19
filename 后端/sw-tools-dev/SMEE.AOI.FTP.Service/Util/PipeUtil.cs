using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Pipes;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SMEE.AOI.FTP.Service
{
    public static class PipeUtil
    {
        public static void SendMessage(
            string pipeName, string msg, int timeoutMS)
        {
            using (var pipeStream = new NamedPipeClientStream(
                ".", pipeName, PipeDirection.Out))
            {
                using (var wt = new StreamWriter(pipeStream))
                {
                    pipeStream.Connect(timeoutMS);
                    wt.WriteLine(msg);
                    wt.Flush();
                }
            }
        }

        public static string ReceiveMessage(string pipeName)
        {
            using (var pipeStream = new NamedPipeServerStream(
                pipeName, PipeDirection.In))
            {
                using (var rd = new StreamReader(pipeStream))
                {
                    pipeStream.WaitForConnection();
                    return rd.ReadLine();
                }
            }
        }

    }
}
