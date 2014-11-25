using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Lidgren.Network;
using System.Diagnostics;

namespace CodenameProjectTwo
{
    class Program
    {
        private static NetClient netClient;

        static void Main(string[] args)
        {
            //debug, cant distinguish server/client xD
            Console.WriteLine("client");
            //need to create this b/c console app
            SynchronizationContext.SetSynchronizationContext(new SynchronizationContext());

            NetPeerConfiguration config = new NetPeerConfiguration("CodenameProjectTwo");
            config.AutoFlushSendQueue=false;
            netClient = new NetClient(config);
            netClient.RegisterReceivedCallback(new SendOrPostCallback(GotMessage));

            Console.WriteLine("Press something to connect!");
            Console.ReadKey();
            Connect("localhost", 14242);
            while(netClient.ConnectionStatus!=NetConnectionStatus.Connected){ }
            SendGameState();
            Console.ReadLine();
            Shutdown();
        }

        public static void GotMessage(object peer)
        {
            NetIncomingMessage im;
            while ((im = netClient.ReadMessage()) != null)
            {
                // handle incoming message
                switch (im.MessageType)
                {
                    case NetIncomingMessageType.DebugMessage:
                    case NetIncomingMessageType.ErrorMessage:
                    case NetIncomingMessageType.WarningMessage:
                    case NetIncomingMessageType.VerboseDebugMessage:
                        string text = im.ReadString();
                        Console.WriteLine(text);
                        break;
                    case NetIncomingMessageType.StatusChanged:
                        NetConnectionStatus status = (NetConnectionStatus)im.ReadByte();

                        //if (status == NetConnectionStatus.Connected)

                        //if (status == NetConnectionStatus.Disconnected)
                        
                        string reason = im.ReadString();
                        Console.WriteLine(status.ToString() + ": " + reason);

                        break;
                    case NetIncomingMessageType.Data:
                        string chat = im.ReadString();
                        Console.WriteLine(chat);
                        break;
                    default:
                        Console.WriteLine("Unhandled type: " + im.MessageType + " " + im.LengthBytes + " bytes");
                        break;
                }
                netClient.Recycle(im);
            }
        }

        //shut down communication
        public static void Shutdown()
        {
            netClient.Disconnect("Requested by user");
            // s_client.Shutdown("Requested by user");
        }

        //connevt to somethinf
        public static void Connect(string host, int port)
        {
            netClient.Start();
            NetOutgoingMessage hail = netClient.CreateMessage("Connection success!");
            netClient.Connect(host, port, hail);
        }

        private static void SendGameState()
        {
            Console.WriteLine("debug");
            NetOutgoingMessage om = netClient.CreateMessage();
            for (int i = 0; i < 100; i++)
                om.Write(42f);
            Stopwatch sw = new Stopwatch();
            sw.Start();
            netClient.SendMessage(om, NetDeliveryMethod.Unreliable);
            sw.Stop();
            netClient.FlushSendQueue();
            Console.WriteLine("elapsed: "+sw.ElapsedMilliseconds);
        }

        // called by the UI
        public static void Send(string text)
        {
            NetOutgoingMessage om = netClient.CreateMessage(text);
            netClient.SendMessage(om, NetDeliveryMethod.Unreliable);
            Console.WriteLine("Sending '" + text + "'");
            netClient.FlushSendQueue();
        }
    }
}
