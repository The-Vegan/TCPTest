using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TCPTest.Client
{
    public class PlayerInfo
    {
        public PlayerInfo() { }
        public byte clientID;
        public byte characterID;
        public string name;

        public byte[] ToByte()
        {
            byte[] stream;
            if (this.name == null) 
            {
                stream = new byte[3];
                stream[0] = this.clientID;
                stream[1] = this.characterID;
                stream[2] = 0;
            }
            else
            {
                stream = new byte[4 + (this.name.Length * 2)];
                stream[0] = this.clientID;
                stream[1] = this.characterID;
                stream[2] = (byte)this.name.Length;
                byte[] nameAsByte = Encoding.Unicode.GetBytes(name);
                for (byte i = 0; i < nameAsByte.Length; i++) stream[3 + i] = nameAsByte[i];
            }

            return stream;
        }

        public static byte[] SerialiseInfoArray(PlayerInfo[] playerList)
        {
            byte[] output = new byte[8_192];
            ushort offset = 2;
            output[0] = 251;//SEND_NAME_LIST

            for (byte i = 0; i < playerList.Length;i++)
            {
                if (playerList[i] == null) continue;
                else output[1]++;

                byte[] plyrAsByte = playerList[i].ToByte();

                for(ushort j = 0; j < plyrAsByte.Length; j++) output[offset + j] = plyrAsByte[j];
                
                offset += (ushort)plyrAsByte.Length;

            }
            Console.WriteLine("[PlayerInfo] Server found " + output[1] + " clients in list");
            return output;
        }

        public static PlayerInfo[] DeserialiseInfoArray(byte[] data) 
        {
            byte nmbrOfPlayer = data[1];

            if (nmbrOfPlayer > 16 || nmbrOfPlayer == 0) return null;

            PlayerInfo[] retArray = new PlayerInfo[16];
            ushort offset = 2;
            for(byte i = 0;i < nmbrOfPlayer; i++)
            {
                PlayerInfo player = new PlayerInfo();

                player.clientID = data[offset];
                Console.WriteLine(player.clientID);
                offset++;
                player.characterID = data[offset];
                Console.WriteLine(player.characterID);
                offset++;
                byte nameLength = data[offset];
                Console.WriteLine(nameLength);
                offset++;
                if (nameLength != 0) player.name = Encoding.Unicode.GetString(data, offset, nameLength * 2);
                offset += (ushort)(nameLength * 2);
                Console.WriteLine("[PlayerInfo] name : " +  player.name);
                retArray[player.clientID - 1] = player;
            }

            return retArray;
        }

    }
}
