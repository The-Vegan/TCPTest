using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace TCPTest.Client
{
    public class LocalClient
    {
        public bool connected = false;
        public long ping = 0;

        public bool pingPrint = false;

        private BaseClient client;

        private byte clientID = 0;

        private PlayerInfo[] players = new PlayerInfo[16];

        public delegate void UpdateNameList(PlayerInfo[] names);
        public event UpdateNameList UpdateNameListInMenu= delegate {};
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
        public LocalClient(string ip) 
        {
            client = new BaseClient();
            client.DataRecievedEvent += DataRecived;
            client.ClientDisconnectedEvent += Disconnected;
            client.ConnectToServer(ip, 1404);
            connected = true;
            Console.WriteLine("[LocalClient] Connected to server successfully");
        }

        private void Disconnected(object sender)
        {
            this.Disconnect();
        }

        private void DataRecived(object sender, byte[] data, NetworkStream stream)
        {            
            switch (data[0])
            {
                case PING:
                    client.SendDataToServer(data);
                    
                    ping = (data[1] << 24) + (data[2] << 16) + (data[3] << 8) + data[4];
                    
                    if (pingPrint) 
                    {
                        Console.WriteLine("[LocalClient] Ping : " + ping);
                        pingPrint = false;
                    }
                    break;
                case ABOUT_TO_LAUNCH: 
                    Console.WriteLine("[LocalClient] ABOUT_TO_LAUNCH recieved");
                    break;
                case ABORT_LAUNCH:
                    Console.WriteLine("[LocalClient] ABORT_LAUNCH recieved");
                    break;
                case LAUNCH:
                    Console.WriteLine("[LocalClient] LAUNCH recieved");
                    break;
                case SET_CLIENT_OR_ENTITY_ID:
                    Console.WriteLine("[LocalClient] SET_CLIENT_OR_ENTITY_ID recieved ; clientID : " + data[1] + "\tcharID : " + data[2]);
                    this.clientID = data[1];
                    if (data[2] != 0) this.players[clientID - 1].characterID = data[2];
                    players[clientID - 1] = new PlayerInfo();
                    players[clientID - 1].clientID = clientID;
                    players[clientID - 1].characterID = 0;
                    break;
                case SEND_NAME_LIST:
                    Console.WriteLine("[LocalClient] SEND_NAME_LIST recieved");
                    players = PlayerInfo.DeserialiseInfoArray(data);
                    UpdateNameListInMenu(players);
                    Console.WriteLine("[LocalClient] NameList Decoded");
                    break;
                case SET_LEVEL_CONFIG:
                    Console.WriteLine("[LocalClient] SET_LEVEL_CONFIG recieved");
                    break;
                default: 
                    Console.WriteLine("[LocalClient] recieved \"" + data[0] + "\"");
                    break;
            }
            GC.Collect();
        }

        public void PrintPlayerList()
        {
            Console.WriteLine("[LocalClient]-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-");
            for (byte i = 0; i < players.Length; i++)
            {
                if (players[i] != null)
                {
                    Console.Write("[LocalClient] " + players[i].clientID + "\t:\t" + players[i].characterID + "\t:\t");
                    if (players[i].name == null) Console.Write("Unnamed\n"); else Console.Write(players[i].name + "\n");
                }
            }
            Console.WriteLine("[LocalClient]-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-");
        }

        //Menu
        //-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-\\
        public void SendCharIDAndName(string name)
        {
            if (name.Length > 24) return;

            players[clientID - 1].name = name;
            byte[] stream = new byte[8_192];
            stream[0] = SET_CHARACTER;
            byte[] playerAsBytes = players[clientID - 1].ToByte();
            for(ushort i = 0;i < playerAsBytes.Length; i++) stream[i + 1] = playerAsBytes[i];
            

            client.SendDataToServer(stream);
        }
        //-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-\\
        //Menu
        public void Disconnect() { client.Disconnect(); connected = false; Console.WriteLine("[LocalClient] Disconected"); }
        
    }
}
