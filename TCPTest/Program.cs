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
                client = null;
                server = null;
                GC.Collect();
            };//When you close the console

            Console.WriteLine("C -> client\nS -> server");
            string input = Console.ReadLine();

            switch (input)
            {
                case "C":
                case "c":
                    Console.WriteLine("Enter IP");
                    input = Console.ReadLine();
                    client = new LocalClient(input);
                    break;
                    
                case "S":
                case "s":
                    server = new HostServer();
                    break;
            }

        }
    }
}
