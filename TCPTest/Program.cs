using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
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
                            
                        }
                        catch (Exception)
                        {
                            Console.WriteLine("Connexion failed");
                            continue;
                        }

                        while ((input != "X")&&(input != "x"))
                        {
                            Console.WriteLine("Enter \"X\" to disconnect\nEnter \"N\" to input a name\nEnter \"L\" to print player list\nEnter \"P\" to check ping");
                            input = Console.ReadLine();
                            switch (input)
                            {
                                case "n":
                                case "N":
                                    Console.WriteLine("Enter your name");
                                    string name = Console.ReadLine();
                                    if (name != "" && name.Length <= 20) client.SendCharIDAndName(name);
                                    break;
                                case "l":
                                case "L":
                                    client.PrintPlayerList();
                                    break;
                                case "p":
                                case "P":
                                    client.pingPrint = true;
                                    Thread.Sleep(1000);
                                    break;

                            }
                        }

                        client.Disconnect();


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
    }
}
