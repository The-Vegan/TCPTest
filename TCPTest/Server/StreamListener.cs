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
        private long lastPing = 0;
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
                    if (lu == 0) { continue; }
                    if (buffer[0] == 0) try { ping.SetResult(true); } catch (InvalidOperationException) { Console.WriteLine("[StreamListener] Ping recieved out of cooldown"); }
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
            byte lostPings = 0;
            while (lostPings < 10)
            {
                Thread.Sleep(1000);


                Stopwatch pingTime = new Stopwatch();
                ping = new TaskCompletionSource<bool>();
                pingTime.Start();
                try 
                {
                        stream.Write(new byte[]{
                        0,//Ping protocol
                        (byte)(lastPing >> 24),
                        (byte)(lastPing >> 16),
                        (byte)(lastPing >> 8),
                        (byte)lastPing },
                        0,
                        5);
                    
                } catch (Exception e) 
                { 
                    Console.WriteLine("[StreamListener] Terminating Stream, Exception caught : " + e);
                    break; 
                }
                
                new Thread(TimeoutThread).Start();
                if (await ping.Task)
                {
                    pingTime.Stop();
                    lostPings = 0;
                    lastPing = pingTime.ElapsedMilliseconds;
                }
                else
                {
                    lostPings++;
                    lastPing = lostPings * 1000;
                    Console.WriteLine("[StreamListener] lost " + lostPings);
                }

            }
            DisconnectedEvent(this);
            stream.Close();
            stream.Dispose();

        }
        private void TimeoutThread()
        {
            Thread.Sleep(500);
            if (ping.TrySetResult(false)) Console.WriteLine("[StreamListener] Ping lost"); ;
        }
        //-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-\\
        //Ping
    }
}
