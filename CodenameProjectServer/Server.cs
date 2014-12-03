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

            //instantiate village centers
            InstanceClass(SGlobal.VILLAGE_CENTRE_TYPE, false, new Vector2f(200f, 200f), 100);
            InstanceClass(SGlobal.VILLAGE_CENTRE_TYPE, true, new Vector2f(200f, 200f), 100);

            //asynchronous server worker
            workerThread.DoWork += new DoWorkEventHandler(
                delegate(object o, DoWorkEventArgs arg)
                {
                    #region Serverloop
                    while (true){
                        NetIncomingMessage im;
                        while ((im = netServer.ReadMessage()) != null){
                            // handle incoming message
                            switch (im.MessageType)
                            {
                                #region UnusedMessageTypes
                                case NetIncomingMessageType.DebugMessage:
                                case NetIncomingMessageType.ErrorMessage:
                                case NetIncomingMessageType.WarningMessage:
                                case NetIncomingMessageType.VerboseDebugMessage:
                                    string text = im.ReadString();
                                    //Console.WriteLine("d/e/w/v: "+text);
                                    break;
                                #endregion

                                #region ClientIdentification
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
                                #endregion

                                #region DataHandling
                                case NetIncomingMessageType.Data:
                                    //identify client
                                    bool client=false;
                                    // incoming chat message from a client
                                    if (im.SenderConnection.RemoteUniqueIdentifier == clientIdentifier1){
                                        Console.Write("client 1: ");
                                        client = false;
                                    }
                                    else if (im.SenderConnection.RemoteUniqueIdentifier == clientIdentifier2)
                                    {
                                        Console.Write("client 2: ");
                                        client = true;
                                    }
                                    else
                                    {
                                        Console.WriteLine("unknown client detected - abort message read");
                                        return;
                                    }
                                    //identify message
                                    switch (im.ReadInt32()){
                                        case SGlobal.MOUSE_CLICK_MESSAGE:
                                            int item=im.ReadInt32();
                                            //left click: save clicked item
                                            if (im.ReadBoolean() == false){
                                                Console.WriteLine("Item "+item+" was clicked!");
                                                lastExecutedClick = item;
                                            }
                                            //got right click, notify first clicked item of new targetposition
                                            else
                                                if (Sendlist.Count - 1 < lastExecutedClick)
                                                    Sendlist[lastExecutedClick].Target = Sendlist[item].Position;
                                            break;

                                        case SGlobal.STRING_MESSAGE:
                                            Console.WriteLine("got string message: " + im.ReadString());
                                            break;
                                        case SGlobal.PLANT_BUILDING_MESSAGE:
                                            Console.WriteLine("oh shit boys gotta plant a building now");
                                            InstanceClass(im.ReadInt32(), client, new Vector2f(im.ReadFloat(), im.ReadFloat()), 100f);
                                            break;
                                    }
                                    break;
                                #endregion
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
                                //Console.WriteLine("server send");
                                om.Write(s.Type);
                                om.Write(s.Faction);
                                om.Write(s.ID);
                                om.Write(s.Position.X);
                                om.Write(s.Position.Y);
                                om.Write(s.Health);
                            }
                            //declare message end
                            //Console.WriteLine("server send");
                            om.Write(-1);

                            //adding tags so that retarded me can find this _kinda_ important line: mistake error change
                            //warning: change to != when we finally have objects!
                            if (Sendlist.Count != 0)
                                netServer.SendMessage(om, all, NetDeliveryMethod.Unreliable, 0);
                        }
                        //dont clog the cpu
                        Thread.Sleep(10);
                        //Console.WriteLine("work");
                    }
                    #endregion
                });
            workerThread.RunWorkerAsync();
            Console.ReadKey();
            //ServerLoop();
            netServer.Shutdown("Requested by user");
        }

        /// <summary>
        /// use this class to divert the types into the appropriate classes
        /// </summary>
        internal static void InstanceClass(Int32 _type, bool _faction, Vector2f _position, float _health)
        {
            bool success = true;
            int _ID = SGlobal.ID_COUNTER++;
            ElongateList(_ID);
            switch (_type)
            {
                case SGlobal.VILLAGE_CENTRE_TYPE:
                    //check for needed resources
                    Sendlist[_ID] = new Building(_type, _faction, _ID, _position, 100);
                    break;
                default:
                    success = false;
                    SGlobal.ID_COUNTER--;
                    break;
            }
            if (success)
                Console.WriteLine("instanced a building type " + _type + " of player " + _faction);
            else
                Console.WriteLine("Failed to find the correct building type; see Server.cs:InstanceClass");
        }

        private static void ElongateList(int _ID){
            //if the list size is smaller than the id, elongate it
            if (Sendlist.Count - 1 < _ID)
                while (Sendlist.Count - 1 < _ID)
                {
                    Sendlist.Add(null);
                    Console.WriteLine("elongating");
                }
        }

        // called by the UI
        public static void Shutdown()
        {
            netServer.Shutdown("Requested by user");
        }
    }
}
