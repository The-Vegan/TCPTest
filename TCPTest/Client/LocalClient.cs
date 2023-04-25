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

        private byte clientID = 0;

        PlayerInfo[] players = new PlayerInfo[16];

        public delegate void UpdateNameList(string[] names);
        public event UpdateNameList UpdateNameListInMenu= delegate {};
        //Packet Constants
        //-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-\\
        /*CLIENT -> SERVER*/
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
            client.ConnectToServer(ip, 1404);
            Console.WriteLine("[LocalClient] Connected to server successfully");
            client.DataRecievedEvent += DataRecived;
        }

        private void DataRecived(object sender, byte[] data, NetworkStream stream)
        {
            switch (data[0])
            {
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
                    Console.WriteLine("[LocalClient] SET_CLIENT_OR_ENTITY_ID recieved");
                    break;
                case SEND_NAME_LIST:
                    Console.WriteLine("[LocalClient] SEND_NAME_LIST recieved");
                    string[] nameList = new string[16];
                    byte nmbOfNames = data[1];
                    ushort offset = 2;
                    for(byte i = 0; i < nmbOfNames; i++)
                    {
                        byte id = data[offset];
                        offset++;

                        if (id > 16) return;

                        byte lengthOfName = data[offset];
                        offset++;

                        string name = Encoding.Unicode.GetString(data, offset,lengthOfName);
                        offset += lengthOfName;

                        nameList[id - 1] = name;
                    }
                    UpdateNameListInMenu(nameList);
                    Console.WriteLine("[LocalClient] NameList Decoded");
                    for(byte i = 0; i < nameList.Length; i++)
                    {
                        if (nameList[i] == null) Console.WriteLine((i + 1) + "not connected");
                        else Console.WriteLine((i + 1) + nameList[i]);
                    }
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

        //Menu
        //-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-\\
        public void ChangeName(string name)
        {
            SendCharIDAndName(clientID,0,name);
        }

        public void SendCharIDAndName(byte clientID,byte charID,string name)
        {
            if (name.Length > 24) return;

            byte[] stream = new byte[8_192];
            stream[0] = SET_CHARACTER;
            stream[1] = clientID;
            stream[2] = charID;
            stream[3] = (byte)name.Length;
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
