using Lidgren.Network;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using SFML.Graphics;
using SFML.Window;
using System.ComponentModel;

namespace CodenameProjectServer
{
    class Server
    {
        private const long UNDEFINED_CLIENT=-1;

        private static NetServer netServer;
        private static long clientIdentifier1 = UNDEFINED_CLIENT, clientIdentifier2 = UNDEFINED_CLIENT;
        private static List<SInterfaces.ISendable> Sendlist = new List<SInterfaces.ISendable>();

        private static int lastExecutedClick = -1;

        private static BackgroundWorker workerThread=new BackgroundWorker();

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
            netServer.Start();

            //asynchronous server worker
            workerThread.DoWork += new DoWorkEventHandler(
                delegate(object o, DoWorkEventArgs arg){
                    while (true){
                        NetIncomingMessage im;
                        while ((im = netServer.ReadMessage()) != null){
                            // handle incoming message
                            switch (im.MessageType){
                                case NetIncomingMessageType.DebugMessage:
                                case NetIncomingMessageType.ErrorMessage:
                                case NetIncomingMessageType.WarningMessage:
                                case NetIncomingMessageType.VerboseDebugMessage:
                                    string text = im.ReadString();
                                    //Console.WriteLine("d/e/w/v: "+text);
                                    break;

                                case NetIncomingMessageType.StatusChanged:
                                    NetConnectionStatus status = (NetConnectionStatus)im.ReadByte();

                                    string reason = im.ReadString();
                                    Console.WriteLine(NetUtility.ToHexString(im.SenderConnection.RemoteUniqueIdentifier) + " " + status + ": " + reason);

                                    if (status == NetConnectionStatus.Connected)
                                    {
                                        //save uid
                                        string hail = "";
                                        if (clientIdentifier1 == UNDEFINED_CLIENT)
                                        {
                                            clientIdentifier1 = im.SenderConnection.RemoteUniqueIdentifier;
                                            hail = "Client 1 (" + clientIdentifier1 + ")";
                                        }
                                        else if (clientIdentifier2 == UNDEFINED_CLIENT)
                                        {
                                            clientIdentifier2 = im.SenderConnection.RemoteUniqueIdentifier;
                                            hail = "Client 2 (" + clientIdentifier2 + ")";
                                        }
                                        Console.WriteLine(hail);
                                    }
                                    break;
                                case NetIncomingMessageType.Data:
                                    // incoming chat message from a client
                                    if (im.SenderConnection.RemoteUniqueIdentifier == clientIdentifier1)
                                        Console.Write("client 1: ");
                                    else if (im.SenderConnection.RemoteUniqueIdentifier == clientIdentifier2)
                                        Console.Write("client 2: ");
                                    else
                                        Console.WriteLine("unknown client detected");
                                    //identify message
                                    switch (im.ReadInt32())
                                    {
                                        case SGlobal.MOUSE_CLICK_MESSAGE:
                                            int item=im.ReadInt32();
                                            //left click: save clicked item
                                            if (im.ReadBoolean() == false){
                                                Console.WriteLine("Item "+item+" was clicked!");
                                                lastExecutedClick = item;
                                            }
                                            //got right click, notify first clicked item of new targetposition
                                            else{
                                                Sendlist[lastExecutedClick].Target = Sendlist[item].Position;
                                            }
                                            break;
                                        case SGlobal.STRING_MESSAGE:
                                            Console.WriteLine("got string message: " + im.ReadString());
                                            break;
                                    }
                                    break;
                                default:
                                    Console.WriteLine("Unhandled type: " + im.MessageType + " " + im.LengthBytes + " bytes " + im.DeliveryMethod + "|" + im.SequenceChannel);
                                    break;
                            }
                            netServer.Recycle(im);
                        }
                        /*
                        BROADCAST GAME STATE
                        */
                        //whom to send to
                        List<NetConnection> all = netServer.Connections;
                        if (all.Count > 0){
                            NetOutgoingMessage om = netServer.CreateMessage();
                            /*
                             Write everything the clients need to know on each update.
                             * Currently we broadcast everything all the time; this
                             * gets more and more inefficient the more stuff we need to
                             * send, prompting better handling by server-side changed testing
                             */
                            //identify as server broadcast
                            om.Write(SGlobal.GAMESTATE_BROADCAST);
                            //protocol: send the type first, then an unique identifier (see SGlobal.ID_COUNT)
                            foreach (SInterfaces.ISendable s in Sendlist){
                                Console.WriteLine("server send");
                                om.Write(s.Type);
                                om.Write(s.Faction);
                                om.Write(s.ID);
                                om.Write(s.Position.X);
                                om.Write(s.Position.Y);
                                om.Write(s.Health);
                            }
                            //declare message end
                            Console.WriteLine("server send");
                            om.Write(-1);

                            //warning: change to != when we finally have objects!
                            if (Sendlist.Count == 0)
                                netServer.SendMessage(om, all, NetDeliveryMethod.Unreliable, 0);
                        }
                        //dont clog the cpu
                        Thread.Sleep(10);
                        //Console.WriteLine("work");
                    }
                });
            workerThread.RunWorkerAsync();
            Console.ReadKey();
            //ServerLoop();
            netServer.Shutdown("Requested by user");
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
                            //Console.WriteLine("d/e/w/v: "+text);
                            break;

                        case NetIncomingMessageType.StatusChanged:
                            NetConnectionStatus status = (NetConnectionStatus)im.ReadByte();

                            string reason = im.ReadString();
                            Console.WriteLine(NetUtility.ToHexString(im.SenderConnection.RemoteUniqueIdentifier) + " " + status + ": " + reason);

                            if (status == NetConnectionStatus.Connected)
                            {
                                //save uid
                                string hail="";
                                if (clientIdentifier1 == UNDEFINED_CLIENT){
                                    clientIdentifier1 = im.SenderConnection.RemoteUniqueIdentifier;
                                    hail = "Client 1 (" + clientIdentifier1 + ")";
                                }
                                else if (clientIdentifier2 == UNDEFINED_CLIENT)
                                {
                                    clientIdentifier2 = im.SenderConnection.RemoteUniqueIdentifier;
                                    hail = "Client 2 (" + clientIdentifier2 + ")";
                                }
                                Console.WriteLine(hail);
                            }
                            break;
                        case NetIncomingMessageType.Data:
                            // incoming chat message from a client
                            //debug identifying clients
                            if (im.SenderConnection.RemoteUniqueIdentifier == clientIdentifier1)
                                Console.Write("client 1: ");
                            else if (im.SenderConnection.RemoteUniqueIdentifier == clientIdentifier2)
                                Console.Write("client 2: ");
                            else
                                Console.WriteLine("unknown client detected");
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
                            break;
                        default:
                            Console.WriteLine("Unhandled type: " + im.MessageType + " " + im.LengthBytes + " bytes " + im.DeliveryMethod + "|" + im.SequenceChannel);
                            break;
                    }
                    netServer.Recycle(im);
                }
                /*
                BROADCAST GAME STATE
                */
                List<NetConnection> all = netServer.Connections; // get copy
                if (all.Count > 0)
                {
                    NetOutgoingMessage om = netServer.CreateMessage();
                    /*
                     Write everything the clients need to know on each update.
                     * Currently we broadcast everything all the time; this
                     * gets more and more inefficient the more stuff we need to
                     * send, prompting a data-identifier if the game begins to lag
                     */
                    //identify as server broadcast
                    om.Write(SGlobal.GAMESTATE_BROADCAST);
                    //protocol: send the type first, then an unique identifier (see SGlobal.ID_COUNT)
                    foreach (SInterfaces.ISendable s in Sendlist)
                    {
                        Console.WriteLine("server send");
                        om.Write(s.Type);
                        om.Write(s.Faction);
                        om.Write(s.ID);
                        om.Write(s.Position.X);
                        om.Write(s.Position.Y);
                        om.Write(s.Health);
                    }
                    //declare message end
                    Console.WriteLine("server send");
                    om.Write(-1);
                    if (Sendlist.Count == 0)
                        netServer.SendMessage(om, all, NetDeliveryMethod.Unreliable, 0);
                }
                Thread.Sleep(10);
            }
        }


        // called by the UI
        public static void Shutdown()
        {
            netServer.Shutdown("Requested by user");
        }
    }
}
