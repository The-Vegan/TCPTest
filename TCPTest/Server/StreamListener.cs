using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace TCPTest.Server
{
    public class StreamListener
    {
        private NetworkStream stream;
        public NetworkStream GetStream() { return stream; }

        public delegate void Disconnected(StreamListener Sender);
        public event Disconnected DisconnectedEvent = delegate { };

        public delegate void DataRecieved(object sender, byte[] data, NetworkStream stream);
        public event DataRecieved DataRecievedEvent = delegate { };

        public StreamListener(NetworkStream str)
        {
            this.stream = str;

            new Thread(new ThreadStart(Listening)).Start();
        }
        public void Close() { stream.Close(); }
        public void Listening()
        {
            byte[] buffer = new byte[8_192];
            while (true)
            {
                try
                {
                    
                    int lu = stream.Read(buffer, 0, buffer.Length);
                    if (lu == 0)
                    {
                        continue;
                    }
                    DataRecievedEvent(this, buffer, stream);

                }
                catch
                {
                    DisconnectedEvent(this);
                    stream.Close();
                    stream.Dispose();
                    break;
                }

            }
            
        }

        public void Write(byte[] data)
        {
            stream.Write(data, 0, data.Length);
        }
    }
}
