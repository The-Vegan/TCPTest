using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Runtime.InteropServices.ComTypes;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace TCPTest.Server
{
    public class BaseServer
    {
        private TcpListener listener;
        private StreamListener[] streams = new StreamListener[16];

        //private NetworkStream[] streams = new NetworkStream[16];

        //Events
        //-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-\\
        public delegate void DataRecieved(object sender, byte[] data, NetworkStream stream);
        public event DataRecieved DataRecievedEvent = delegate { };

        public delegate void ClientConnected(object sender,byte id);
        public event ClientConnected ClientConnectedEvent = delegate { };

        public delegate void ClientDisconnected(object sender,byte clientID);
        public event ClientDisconnected ClientDisconnectedEvent = delegate { };
        //-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-\\
        //Events
        public BaseServer()
        {
            var host = Dns.GetHostEntry(Dns.GetHostName());
            foreach (var ip in host.AddressList)
            {
                if (ip.AddressFamily == AddressFamily.InterNetwork)
                {
                    listener = new TcpListener(ip , 1404);
                    break;
                }
            }//Finds local IP address
             
            new Thread(ListeningThread).Start();
        }
        public void Terminate()
        {
            for (byte i = 0; i < streams.Length; i++)
            {
                if (streams[i] != null)
                {
                    streams[i].Close();
                    streams[i] = null;
                }
            }
        }
        private void ListeningThread()
        {
            listener.Start(20);
            while (true)
            {
                TcpClient c = listener.AcceptTcpClient();
                for(byte i = 0; i < streams.Length; i++)
                {
                    if (streams[i] == null) 
                    { 
                        streams[i] = new StreamListener(c.GetStream());
                        streams[i].DisconnectedEvent += Disconnected;
                        streams[i].DataRecievedEvent += DataRecievedByServer;
                        ClientConnectedEvent(this, (byte)(i + 1));
                        break;
                    }//Connect client and wires the corresponding events
                    if (i == streams.Length) c.Dispose();//If server is full, don't
                }
            }
        }

        private void DataRecievedByServer(object sender, byte[] data, NetworkStream stream)
        {
            this.DataRecievedEvent(this, data, stream);
        }

        private void Disconnected(StreamListener Sender)
        {
            Sender.DisconnectedEvent -= Disconnected;
            Sender.DataRecievedEvent -= DataRecievedByServer;

            for(byte i = 0; i < streams.Length; ++i)
            {
                if (streams[i] == Sender) 
                { streams[i] = null;
                    GC.Collect(); 
                    ClientDisconnectedEvent(this, (byte)(i+1));
                    return; 
                }
            }
        }

        public void SendDataOnSingleStream(byte[] data,byte clientID)
        {
            if (streams[clientID - 1] == null) return;
            if(data.Length > 8_192)
            {
                Console.WriteLine("[BaseServer] Err : Message too long : " + data.Length);
            }
            else
            {
                streams[clientID - 1].Write(data);
            }
        }
        
        public void SendDataOnAllStreams(byte[] data) //avoids checks and returns to save optimisations
        {
            Console.WriteLine("[BaseServer] Broadcast called ");
            if (data.Length > 8_192)
            {
                Console.WriteLine("[BaseServer] Err : Message too long : " + data.Length);
            }
            else
            {
                if (streams[0] != null) streams[0].Write(data);
                if (streams[1] != null) streams[1].Write(data);
                if (streams[2] != null) streams[2].Write(data);
                if (streams[3] != null) streams[3].Write(data);

                if (streams[4] != null) streams[4].Write(data);
                if (streams[5] != null) streams[5].Write(data);
                if (streams[6] != null) streams[6].Write(data);
                if (streams[7] != null) streams[7].Write(data);

                if (streams[8] != null) streams[8].Write(data);
                if (streams[9] != null) streams[9].Write(data);
                if (streams[10] != null) streams[10].Write(data);
                if (streams[11] != null) streams[11].Write(data);

                if (streams[12] != null) streams[12].Write(data);
                if (streams[13] != null) streams[13].Write(data);
                if (streams[14] != null) streams[14].Write(data);
                if (streams[15] != null) streams[15].Write(data);
            }
        }

        public NetworkStream GetStream(byte idx)
        {
            return streams[idx].GetStream();
        }
    }
}
