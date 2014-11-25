using Lidgren.Network;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace CodenameProjectServer
{
    class Program
    {
        private static NetServer s_server;
        static void Main(string[] args)
        {
            //debug, cant distinguish server/client xD
            Console.WriteLine("server");

            //need to create this b/c console app
            SynchronizationContext.SetSynchronizationContext(new SynchronizationContext());

            // set up network, name must be the same
            NetPeerConfiguration config = new NetPeerConfiguration("CodenameProjectTwo");
            config.MaximumConnections = 2;
            config.Port = 14242;
            s_server = new NetServer(config);
            ServerLoop();
        }

        private static void ServerLoop()
        {
            while (true)
            {
                NetIncomingMessage im;
                while ((im = s_server.ReadMessage()) != null)
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

                            string reason = im.ReadString();
                            Console.WriteLine(NetUtility.ToHexString(im.SenderConnection.RemoteUniqueIdentifier) + " " + status + ": " + reason);

                            if (status == NetConnectionStatus.Connected)
                                Console.WriteLine("Remote hail: " + im.SenderConnection.RemoteHailMessage.ReadString());
                            break;
                        case NetIncomingMessageType.Data:
                            // incoming chat message from a client
                            string chat = im.ReadString();

                            Console.WriteLine("Broadcasting '" + chat + "'");

                            // broadcast this to all connections, except sender
                            List<NetConnection> all = s_server.Connections; // get copy
                            all.Remove(im.SenderConnection);

                            if (all.Count > 0)
                            {
                                NetOutgoingMessage om = s_server.CreateMessage();
                                om.Write(NetUtility.ToHexString(im.SenderConnection.RemoteUniqueIdentifier) + " said: " + chat);
                                s_server.SendMessage(om, all, NetDeliveryMethod.ReliableOrdered, 0);
                            }
                            break;
                        default:
                            Console.WriteLine("Unhandled type: " + im.MessageType + " " + im.LengthBytes + " bytes " + im.DeliveryMethod + "|" + im.SequenceChannel);
                            break;
                    }
                    s_server.Recycle(im);
                }
                Thread.Sleep(1);
            }
        }

        // called by the UI
        public static void StartServer()
        {
            s_server.Start();
        }

        // called by the UI
        public static void Shutdown()
        {
            s_server.Shutdown("Requested by user");
        }
    }
}
