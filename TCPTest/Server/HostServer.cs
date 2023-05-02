using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using TCPTest.Client;

namespace TCPTest.Server
{
    public class HostServer
    {
        private BaseServer server;

        private PlayerInfo[] players = new PlayerInfo[16];

        public HostServer() 
        {
            server = new BaseServer();

            server.ClientConnectedEvent += Connected;
            server.ClientDisconnectedEvent += Disconnected;
            server.DataRecievedEvent += DataRecieved;

            for(byte i = 0; i < players.Length; i++) players[i] = new PlayerInfo();  

        }
        //Packet Constants
        //-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-\\
        /*CLIENT -> SERVER*/
        private const byte PING = 0;
        private const byte MOVE = 1;
        private const byte SET_CHARACTER = 2;
        /*SERVER -> CLIENT*/
        //Pre launch
        private const byte ABOUT_TO_LAUNCH = 255;
        private const byte ABORT_LAUNCH = 254;
        private const byte LAUNCH = 253;
        private const byte SET_CLIENT_OR_ENTITY_ID = 252;
        private const byte SEND_NAME_LIST = 251;
        private const byte SET_LEVEL_CONFIG = 250;
        //Post launch
        private const byte GAME_OVER = 249;
        private const byte GAME_SOON_OVER = 248;
        private const byte SET_MOVES = 247;
        private const byte SYNC = 246;
        private const byte ITEM_GIVEN_BY_SERVER = 245;
        private const byte BLUNDERED_BY_SERVER = 244;
        //-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-\\
        //Packet Constants
        public void Terminate()
        {
            server.Terminate();
        }

        private void DataRecieved(object sender, byte[] data, NetworkStream stream)
        {
            if (server.GetStream((byte)(data[1] - 1)) != stream) Console.WriteLine("[HostServer] Stream doesn't match with corresponding ID : " + data[1]);

            switch (data[0]) 
            {
                case PING:
                    Console.WriteLine("[HostServer] PING recieved from " + data[1]);
                    server.GetStream((byte)(data[1] - 1)).Write(data, 0, data.Length);
                    break;
                case MOVE:
                    Console.WriteLine("[HostServer] MOVE recieved");
                    break;
                case SET_CHARACTER:
                    Console.WriteLine("[HostServer] SET_CHARACTER recieved");

                    byte id = data[1];
                    players[id - 1].clientID = id;
                    players[id - 1].characterID = data[2];
                    byte nameLength = (byte)(data[3] * 2);
                    string name = Encoding.Unicode.GetString(data, 4, nameLength);
                    players[id - 1].name = name;

                    break;
                    
            }
        }

        private void UpdateNameList()
        {

        }

        private void Disconnected(object sender, byte clientID)
        {
            Console.WriteLine("[HostServer] Client "+ clientID+" Disconnected");
            
        }

        private void Connected(object sender, byte id)
        {
            Console.WriteLine("[HostServer] Client Connected on Slot " + id);
            byte[] output = new byte[8_192];
            output[0] = SET_CLIENT_OR_ENTITY_ID;
            output[1] = id;

            server.SendDataOnSingleStream(output, id);
        }
    }
}
