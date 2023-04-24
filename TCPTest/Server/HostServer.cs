using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace TCPTest.Server
{
    public class HostServer
    {
        BaseServer server;

        public HostServer() 
        {
            server = new BaseServer();

            server.ClientConnectedEvent += Connected;
            server.ClientDisconnectedEvent += Disconnected;
            server.DataRecievedEvent += DataRecieved;

            while (true) { Thread.Sleep(5000); server.SendDataOnAllStreams(new byte[] { 0, 1, 2, 3 }); };

        }

        public void Terminate()
        {
            server.Terminate();
        }

        private void DataRecieved(object sender, byte[] data, NetworkStream stream)
        {
            Console.WriteLine("[HostServer] Data Recieved");
        }

        private void Disconnected(object sender)
        {
            Console.WriteLine("[HostServer] Client Disconnected");
        }

        private void Connected(object sender, byte id)
        {
            Console.WriteLine("[HostServer] Client Connected on Slot " + id);
        }
    }
}
