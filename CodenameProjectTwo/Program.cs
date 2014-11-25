using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Lidgren.Network;

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
            netClient = new NetClient(config);
            netClient.RegisterReceivedCallback(new SendOrPostCallback(GotMessage));
            Connect("localhost", 14242);
            string input = Console.ReadLine();
            while (!input.Equals("quit"))
            {
                Send(Console.ReadLine());
            }
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
            NetOutgoingMessage hail = netClient.CreateMessage("This is the hail message");
            netClient.Connect(host, port, hail);
        }

        // called by the UI
        public static void Send(string text)
        {
            NetOutgoingMessage om = netClient.CreateMessage(text);
            netClient.SendMessage(om, NetDeliveryMethod.ReliableOrdered);
            Console.WriteLine("Sending '" + text + "'");
            netClient.FlushSendQueue();
        }
    }
}
