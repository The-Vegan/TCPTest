using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace TCPTest.Client
{
    public class BaseClient
    {
        public BaseClient() { }
        private bool connected;
        private NetworkStream stream;


        //Events
        //-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-\\
        public delegate void DataRecieved(object sender, byte[] data, NetworkStream stream);
        public event DataRecieved DataRecievedEvent = delegate { };

        public delegate void ClientConnected(object sender, NetworkStream stream);
        public event ClientConnected ClientConnectedEvent = delegate { };

        public delegate void ClientDisconnected(object sender);
        public event ClientDisconnected ClientDisconnectedEvent = delegate { };
        //-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-\\
        //Events
        public void ConnectToServer(string host, int port) 
        {
            if(connected) return;

            try
            {
                TcpClient client = new TcpClient();
                client.Connect(host, port);
                stream = client.GetStream();
                connected = true;
                new Thread(InputRecievingThread).Start();
            }
            catch (IOException) { }
        }//Connexion method

        public void Disconnect()
        {
            if(!connected) return;

            this.stream.Close();
        }

        private void InputRecievingThread()
        {
            ClientConnectedEvent(this, stream);
            while (connected)
            {
                
                byte[] buffer = new byte[8_192];
                try
                {
                    stream.Read(buffer, 0, buffer.Length);
                    DataRecievedEvent(this, buffer, stream);
                }
                catch (Exception) { connected = false; }

            }
            ClientDisconnectedEvent(this);
        }//Listening thread

        public void SendDataToServer(byte[] data)
        {
            if(!connected) return;

            if(data.Length > 8_192) 
            {
                //Split message
                Console.WriteLine("[BaseClient] Err : Message too long : " +  data.Length);
            }
            else
            {
                stream.Write(data, 0, data.Length);
            }
        }

    }
}
