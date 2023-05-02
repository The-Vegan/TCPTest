using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace TCPTest.Server
{
    public class StreamListener
    {
        private NetworkStream stream;
        public NetworkStream GetStream() { return stream; }
        //Events
        //-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-\\
        public delegate void Disconnected(StreamListener Sender);
        public event Disconnected DisconnectedEvent = delegate { };

        public delegate void DataRecieved(object sender, byte[] data, NetworkStream stream);
        public event DataRecieved DataRecievedEvent = delegate { };
        //-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-\\
        //Events


        public StreamListener(NetworkStream str)
        {
            this.stream = str;

            new Thread(Listening).Start();
            new Thread(PingThread).Start();
        }
        public void Close() { stream.Close(); }
        public void Listening()
        {
            byte[] buffer = new byte[8_192];
            while (true)
            {
                try
                {

                    int lu = stream.Read(buffer, 0, buffer.Length);
                    if (lu == 0)
                    {
                        continue;
                    }
                    if (buffer[0] == 0) ping.SetResult(true);
                    else DataRecievedEvent(this, buffer, stream);

                }
                catch
                {
                    DisconnectedEvent(this);
                    stream.Close();
                    stream.Dispose();
                    break;
                }

            }

        }

        public void Write(byte[] data)
        {
            stream.Write(data, 0, data.Length);
        }

        //Ping
        //-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-\\
        private bool pingON = false;
        private TaskCompletionSource<bool> ping;

        private async void PingThread()
        {
            while (true)
            {
                Thread.Sleep(1000);


                Stopwatch pingTime = new Stopwatch();
                ping = new TaskCompletionSource<bool>();
                pingTime.Start();
                stream.Write(new byte[] { 0 }, 0, 1);
                new Thread(TimeoutThread).Start();
                if (await ping.Task)
                {
                    pingTime.Stop();
                    Console.WriteLine("[StreamListener] Ping : " + pingTime.ElapsedMilliseconds);
                }
                else Console.WriteLine("[StreamListener] Ping lost");

            }
        }
        private void TimeoutThread()
        {
            Thread.Sleep(500);
            ping.TrySetResult(false);
        }
        //-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-\\
        //Ping
    }
}
