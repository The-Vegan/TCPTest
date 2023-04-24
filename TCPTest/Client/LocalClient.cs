using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace TCPTest.Client
{
    public class LocalClient
    {
        private BaseClient client;

        PlayerInfo[] players = new PlayerInfo[16];


        public LocalClient(string ip) 
        {
            client = new BaseClient();
            client.ConnectToServer(ip, 1404);
            Console.WriteLine("[LocalClient] Connected to server successfully");
            client.DataRecievedEvent += DataRecived;
        }

        private void DataRecived(object sender, byte[] data, NetworkStream stream)
        {
            Console.WriteLine("[LocalClient] Data Recieved From Server " + data[0] + "," + data[1] + "," + data[2] + "," + data[3] + "," + data[4] + "," + data[5]);
        }

        //Menu
        //-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-\\
        public void SendCharIDAndName(byte clientID,byte charID,string name)
        {
            if (name.Length > 24) return;

            byte[] stream = new byte[8_192];
            stream[0] = clientID;
            stream[1] = charID;
            stream[2] = (byte)name.Length;
            byte[] nameAsByte = Encoding.Unicode.GetBytes(name);
            for(byte i = 0;  i < nameAsByte.Length; i++)
            {
                stream[3 + i] = nameAsByte[i];
            }

        }
        //-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-\\
        //Menu
        public void Disconnect() { client.Disconnect(); }
    }
}
