using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TCPTest.Client;
using TCPTest.Server;

namespace TCPTest
{
    internal class Program
    {
        private static HostServer server;
        private static LocalClient client;

        static void Main(string[] args)
        {
            AppDomain.CurrentDomain.ProcessExit += delegate
            {
                client?.Disconnect();
                server?.Terminate();
            };//When you close the console
            while (true)
            {
                Console.WriteLine("C -> client\nS -> server");
                string input = Console.ReadLine();

                switch (input)
                {
                    case "C":
                    case "c":
                        try
                        {
                            Console.WriteLine("Enter IP");
                            input = Console.ReadLine();
                            client = new LocalClient(input);
                            client.UpdateNameListInMenu += PrintPlayerList;
                        }
                        catch (Exception)
                        {
                            Console.WriteLine("Connexion failed");
                            continue;
                        }

                        while ((input != "X")||(input != "x"))
                        {
                            Console.WriteLine("Enter \"X\" to disconnect\nEnter \"N\" to input a name");
                            input = Console.ReadLine();
                            if(input == "n" || input == "N")
                            {
                                Console.WriteLine("Enter your name");
                                string name = Console.ReadLine();
                                if (name != "" && name.Length <= 20) client.ChangeName(name);
                            }
                        }



                        break;

                    case "S":
                    case "s":
                        server = new HostServer();

                        while (true)//Close Server
                        {
                            Console.WriteLine("Enter \"X\" to terminate server");
                            input = Console.ReadLine();
                            if (input == "x" || input == "X") break;
                        }
                        server.Terminate();

                        break;
                }
            }


        }

        private static void PrintPlayerList(PlayerInfo[] players)
        {
            for (byte i = 0; i < players.Length; i++)
            {
                if (players[i].name == null) Console.WriteLine((i + 1) + " : not connected");
                else Console.WriteLine((i + 1) + " : " + players[i].name);
            }
        }
    }
}
