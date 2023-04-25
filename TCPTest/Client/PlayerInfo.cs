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
            byte[] stream = new byte[8_192];
            stream[0] = 2;//CONSTANT
            stream[1] = this.clientID;
            stream[2] = this.characterID;

            stream[3] = (byte)this.name.Length;
            byte[] nameAsByte = Encoding.Unicode.GetBytes(name);

            for (byte i = 0; i < nameAsByte.Length; i++)
            {
                stream[3 + i] = nameAsByte[i];
            }

            return stream;
        }

        public static byte[] SerialiseInfoArray(PlayerInfo[] playerList)
        {
            byte[] output = new byte[8_192];
            ushort offset = 2;
            for (byte i = 0; i < playerList.Length;i++)
            {
                if (playerList[i] == null) continue;
                else output[1]++;


                output[offset] = playerList[i].clientID;
                offset++;
                output[offset] = playerList[i].characterID;
                offset++;
                output[offset] = (byte)playerList[i].name.Length;
                byte[] nameAsBytes = Encoding.Unicode.GetBytes(playerList[i].name);
                for(byte j = 0; j < nameAsBytes.Length; j++)
                {
                    output[offset + j] = nameAsBytes[j];
                }
                offset += (byte) nameAsBytes.Length;

            }

            return output;
        }

        public static PlayerInfo[] DeserialiseInfoArray(byte[] data) 
        {
            if (data[1] > 16 || data[1] == 0) return null;

            PlayerInfo[] retArray = new PlayerInfo[16];
            ushort offset = 2;
            for(byte i = 0;i < data[1]; i++)
            {
                PlayerInfo player = new PlayerInfo();

                byte clientID = data[offset];
                offset++;
                byte charaID = data[offset];
                offset++;
                byte nameLength = data[offset];
                offset++;
                string name = Encoding.Unicode.GetString(data, offset, nameLength * 2);

                player.clientID = clientID;
                player.characterID = charaID;
                player.name = name;

                retArray[clientID + 1] = player;
            }

            return retArray;
        }

    }
}
