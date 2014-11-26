using Lidgren.Network;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using SFML.Graphics;
using SFML.Window;

namespace CodenameProjectServer
{
    class Program
    {
        private static NetServer netServer;
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
            netServer = new NetServer(config);
            StartServer();
            ServerLoop();
        }

        private static void ServerLoop()
        {
            while (true)
            {
                NetIncomingMessage im;
                while ((im = netServer.ReadMessage()) != null)
                {
                    // handle incoming message
                    switch (im.MessageType)
                    {
                        case NetIncomingMessageType.DebugMessage:
                        case NetIncomingMessageType.ErrorMessage:
                        case NetIncomingMessageType.WarningMessage:
                        case NetIncomingMessageType.VerboseDebugMessage:
                            string text = im.ReadString();
                            Console.WriteLine("d/e/w/v: "+text);
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
                            
                            //identify message
                            switch (im.ReadInt32())
                            {
                                case SGlobal.MOUSE_CLICK_MESSAGE:
                                    Console.WriteLine("Mouseclick at: "+im.ReadFloat()+", "+im.ReadFloat());
                                    break;
                                case SGlobal.STRING_MESSAGE:
                                    Console.WriteLine("got string message: " + im.ReadString());
                                    break;
                            }
                            
                            //Console.WriteLine("Broadcasting '" + chat + "'");
                            /*
                            // broadcast this to all connections, except sender
                            List<NetConnection> all = netServer.Connections; // get copy
                            all.Remove(im.SenderConnection);

                            if (all.Count > 0)
                            {
                                NetOutgoingMessage om = netServer.CreateMessage();
                                om.Write(NetUtility.ToHexString(im.SenderConnection.RemoteUniqueIdentifier) + " said: " + chat);
                                netServer.SendMessage(om, all, NetDeliveryMethod.ReliableOrdered, 0);
                            }
                            */
                            break;
                        default:
                            Console.WriteLine("Unhandled type: " + im.MessageType + " " + im.LengthBytes + " bytes " + im.DeliveryMethod + "|" + im.SequenceChannel);
                            break;
                    }
                    netServer.Recycle(im);
                }
                Thread.Sleep(1);
            }
        }

        // called by the UI
        public static void StartServer()
        {
            netServer.Start();
        }

        // called by the UI
        public static void Shutdown()
        {
            netServer.Shutdown("Requested by user");
        }
    }
}
