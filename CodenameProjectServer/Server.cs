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
using CodenameProjectServer.Ressources;
using System.Diagnostics;

namespace CodenameProjectServer {
    class Server {
        private const long UNDEFINED_CLIENT = -1;

        private static NetServer netServer;
        public static List<AbstractServerItem> Sendlist = new List<AbstractServerItem>();
        public static List<RessourceKeeper> RessourceList = new List<RessourceKeeper>();

        private static long clientIdentifier1 = UNDEFINED_CLIENT, clientIdentifier2 = UNDEFINED_CLIENT;
        private static NetConnection con1, con2;

        private static int lastExecutedClick = -1;
        private static List<int> MassSelectionList =new List<int>();

        private static BackgroundWorker workerThread = new BackgroundWorker();

        static void Main(string[] args) {
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
            //health is normalized to 100!
            InstanceClass(SGlobal.BUILDING_RED, 1, new Vector2f(260, 3110), 150);
            InstanceClass(SGlobal.BUILDING_BLUE, 2, new Vector2f(2900, -1350), 150);

            //instantiate ressources
            InstanceClass(SGlobal.RESSOURCE_STONE, 0, new Vector2f(200, 3000), 300);
            InstanceClass(SGlobal.RESSOURCE_STONE, 0, new Vector2f(150, 2814), 300);
            InstanceClass(SGlobal.RESSOURCE_STONE, 0, new Vector2f(468, 2977), 300);
            InstanceClass(SGlobal.RESSOURCE_STONE, 0, new Vector2f(333, 2939), 300);
            InstanceClass(SGlobal.RESSOURCE_STONE, 0, new Vector2f(213, 2635), 300);
            InstanceClass(SGlobal.RESSOURCE_STONE, 0, new Vector2f(475, 2702), 300);
            InstanceClass(SGlobal.RESSOURCE_STONE, 0, new Vector2f(650, 2884), 300);

            InstanceClass(SGlobal.RESSOURCE_WOOD, 0, new Vector2f(800, 2875), 300);
            InstanceClass(SGlobal.RESSOURCE_WOOD, 0, new Vector2f(1079, 2892), 300);
            InstanceClass(SGlobal.RESSOURCE_WOOD, 0, new Vector2f(1384, 2655), 300);
            InstanceClass(SGlobal.RESSOURCE_WOOD, 0, new Vector2f(901, 2566), 300);
            InstanceClass(SGlobal.RESSOURCE_WOOD, 0, new Vector2f(1127, 2379), 300);
            InstanceClass(SGlobal.RESSOURCE_WOOD, 0, new Vector2f(1716, 2508), 300);

            InstanceClass(SGlobal.RESSOURCE_STONE, 0, new Vector2f(2500, -1300), 300);
            InstanceClass(SGlobal.RESSOURCE_STONE, 0, new Vector2f(2939, -974), 300);
            InstanceClass(SGlobal.RESSOURCE_STONE, 0, new Vector2f(2819, -574), 300);
            InstanceClass(SGlobal.RESSOURCE_STONE, 0, new Vector2f(2346, -1048), 300);
            InstanceClass(SGlobal.RESSOURCE_STONE, 0, new Vector2f(2085, -539), 300);
            InstanceClass(SGlobal.RESSOURCE_STONE, 0, new Vector2f(2484, -825), 300);

            InstanceClass(SGlobal.RESSOURCE_WOOD, 0, new Vector2f(800, 2875), 300);
            InstanceClass(SGlobal.RESSOURCE_WOOD, 0, new Vector2f(2587, -1024), 300);
            InstanceClass(SGlobal.RESSOURCE_WOOD, 0, new Vector2f(2040, -1057), 300);
            InstanceClass(SGlobal.RESSOURCE_WOOD, 0, new Vector2f(1623, -741), 300);
            InstanceClass(SGlobal.RESSOURCE_WOOD, 0, new Vector2f(2159, -769), 300);
            InstanceClass(SGlobal.RESSOURCE_WOOD, 0, new Vector2f(2632, -422), 300);

            //instance ressourcekeeper
            RessourceList.Add(new RessourceKeeper(1, 100, 100));
            RessourceList.Add(new RessourceKeeper(2, 100, 100));

            //asynchronous server worker
            workerThread.DoWork += new DoWorkEventHandler(
                delegate(object o, DoWorkEventArgs arg) {
                    #region Serverloop
                    while (true) {
                        #region DoCalculations
                        foreach (AbstractServerItem s in Sendlist)
                            if (s != null && s.Health > 0.5)
                                s.Update();
                        for (int i = Sendlist.Count - 1; i >= 0; i--) {
                            if (Sendlist[i] != null) {
                                Sendlist[i].Update();
                                if (Sendlist[i].IsDead)
                                    Sendlist[i] = null;
                            }
                        }
                        //do intersection calculation; iterate through every item
                        for (int i = Sendlist.Count - 1; i >= 0; i--) {
                            if (Sendlist[i] != null) {
                                //only iterate through items that actually would react, like people...
                                if (Sendlist[i].implementAggroOrEffectEffects) {
                                    //iterate through all other items
                                    for (int j = Sendlist.Count - 1; j >= 0; j--){
                                        //dont check yourself
                                        if (Sendlist[j] != null && i != j) {
                                            //the effectiverectangle hit; we have possibly arrived at our destination, or whatever. take action!
                                            if (Sendlist[i].effectiveRectangle.Intersects(Sendlist[j].effectiveRectangle))
                                                Sendlist[i].TakeEffect(j);
                                            //maybe we have an enemy somewhere? check that too.
                                            else if (Sendlist[i].aggroRectangle.Intersects(Sendlist[j].aggroRectangle))
                                                Sendlist[i].TargetAggro(j);
                                        }
                                    }
                                }
                            }
                        }
                        #endregion
                        #region Communication
                        NetIncomingMessage im;
                        while ((im = netServer.ReadMessage()) != null) {
                            // handle incoming message
                            switch (im.MessageType) {
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


                                    if (status == NetConnectionStatus.Connected) {
                                        //save uid
                                        string hail = "";
                                        if (clientIdentifier1 == UNDEFINED_CLIENT) {
                                            clientIdentifier1 = im.SenderConnection.RemoteUniqueIdentifier;
                                            hail = "Client 1 (" + clientIdentifier1 + ")";
                                            con1 = im.SenderConnection;
                                            SendClientIdentification(1);
                                        }
                                        else if (clientIdentifier2 == UNDEFINED_CLIENT) {
                                            clientIdentifier2 = im.SenderConnection.RemoteUniqueIdentifier;
                                            hail = "Client 2 (" + clientIdentifier2 + ")";
                                            con2 = im.SenderConnection;
                                            SendClientIdentification(2);
                                        }
                                        if (clientIdentifier1 != UNDEFINED_CLIENT && clientIdentifier2 != UNDEFINED_CLIENT)
                                            SendClientIdentification(0);//send to both
                                        //Console.WriteLine(hail);
                                    }
                                    break;
                                #endregion

                                #region DataHandling
                                case NetIncomingMessageType.Data:
                                    //identify client
                                    byte messageByClient = 255;
                                    // incoming chat message from a client
                                    if (im.SenderConnection.RemoteUniqueIdentifier == clientIdentifier1) {
                                        messageByClient = 1;
                                    }
                                    else if (im.SenderConnection.RemoteUniqueIdentifier == clientIdentifier2) {
                                        messageByClient = 2;
                                    }
                                    else {
                                        Console.WriteLine("unknown client detected - abort message read");
                                        return;
                                    }
                                    //identify message
                                    switch (im.ReadInt32()) {
                                        case SGlobal.MOUSE_CLICK_MESSAGE:
                                            Console.Write("Click Message: ");
                                            int item = im.ReadInt32();
                                            //get position
                                            Vector2f clickPosition = new Vector2f(im.ReadFloat(), im.ReadFloat());
                                            //left click: save clicked item
                                            if (im.ReadBoolean() == false){
                                                MassSelectionList.Clear();
                                                if(item != -1)
                                                Console.WriteLine("Item " + item + " clicked, Type: " + Sendlist[item].Type);
                                                else
                                                    Console.WriteLine("item deselected");
                                                lastExecutedClick = item;
                                            }
                                            //got right click, notify first clicked item of new targetposition
                                            else {
                                                if (lastExecutedClick != -1 && MassSelectionList.Count == 0) {
                                                    if (Sendlist[lastExecutedClick] != null && Sendlist[lastExecutedClick].Faction == messageByClient) {
                                                        if (item == -1) {
                                                            Sendlist[lastExecutedClick].Target = clickPosition;
                                                            Sendlist[lastExecutedClick].TargetID = -1;
                                                            Console.WriteLine("Trying to send " + lastExecutedClick + " to position " + clickPosition.ToString());
                                                        }
                                                        else {
                                                            Console.WriteLine("sending lec to Sendlistitem " + item);
                                                            Sendlist[lastExecutedClick].Target = Sendlist[item].Position;
                                                            Sendlist[lastExecutedClick].TargetID = item;
                                                        }
                                                    }
                                                }
                                                else {
                                                    if (item == -1) {
                                                        foreach (int i in MassSelectionList) {
                                                            Sendlist[i].Target = clickPosition;
                                                            Sendlist[i].TargetID = -1;
                                                        }
                                                    }
                                                    else {
                                                        foreach (int i in MassSelectionList) {
                                                            Sendlist[i].Target = Sendlist[item].Position;
                                                            Sendlist[i].TargetID = item;
                                                }
                                                    }
                                                }
                                            }
                                            break;
                                        case SGlobal.MASS_SELECTION_MESSAGE:
                                            MassSelectionList.Clear();
                                            while (im.PeekInt32() != -44) {
                                                MassSelectionList.Add(im.ReadInt32());
                                            }
                                            break;
                                        case SGlobal.BOUNDINGSIZE_MESSAGE:
                                            while (im.PeekInt32() != -1) {
                                                Int32 type = im.ReadInt32();
                                                Int32 xSize = im.ReadInt32();
                                                Int32 ySize = im.ReadInt32();
                                                SizeKeeper match = SGlobal.SizeList.Find(i => i.Type == type);
                                                if (match == null) {
                                                    SGlobal.SizeList.Add(new SizeKeeper(type, xSize, ySize));
                                                }
                                                else {
                                                    //check whether we are changing 
                                                    //a reference here or a local variable
                                                    match.X = xSize;
                                                    match.Y = ySize;
                                                }
                                            }
                                            break;
                                        case SGlobal.STRING_MESSAGE:
                                            Console.WriteLine("got string message: " + im.ReadString());
                                            break;
                                        case SGlobal.PLANT_BUILDING_MESSAGE:
                                            Console.WriteLine("oh shit boys gotta plant a building now");
                                            Int32 instanceType = im.ReadInt32();
                                            InstanceClass(instanceType, messageByClient, new Vector2f(im.ReadFloat(), im.ReadFloat()), 100f);
                                            RessourceList.Find(s => s.Faction == messageByClient).subtract(SGlobal.BUILDING_COSTS_STONE[instanceType], 
                                                SGlobal.BUILDING_COSTS_WOOD[instanceType]);
                                            break;
                                        case SGlobal.SPAWN_PEOPLE_MESSAGE:
                                            Console.WriteLine("plant people now");
                                            Int32 spawnType = im.ReadInt32();
                                            Int32 plantingBuildingID = im.ReadInt32();
                                            if (Sendlist[plantingBuildingID].Faction == messageByClient) {
                                                Vector2f plantPosition = new Vector2f(Sendlist[plantingBuildingID].Size.X + Sendlist[plantingBuildingID].Position.X,
                                                    Sendlist[plantingBuildingID].Size.Y + Sendlist[plantingBuildingID].Position.Y);
                                                InstanceClass(spawnType, Sendlist[plantingBuildingID].Faction, plantPosition, 100);
                                                RessourceList.Find(s => s.Faction == messageByClient).subtract(SGlobal.PEOPLE_COSTS_STONE[spawnType - 100],
                                                    SGlobal.PEOPLE_COSTS_WOOD[spawnType - 100]);
                                            }
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
                        if (all.Count > 0) {
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
                            foreach (AbstractServerItem s in Sendlist) {
                                if(s != null){
                                    om.Write(s.Type);
                                    om.Write(s.Faction);
                                    om.Write(s.ID);
                                    om.Write(s.Position.X);
                                    om.Write(s.Position.Y);
                                    om.Write(s.Health);
                                }
                            }
                            //declare broadcast middle
                            om.Write(-42);
                            foreach (RessourceKeeper r in RessourceList) {
                                om.Write(r.Faction);
                                om.Write(r.Stone);
                                om.Write(r.Wood);
                            }
                            om.Write(-1);
                            //finally send now
                            if (Sendlist.Count != 0)
                                netServer.SendMessage(om, all, NetDeliveryMethod.Unreliable, 0);
                        }
                        //dont clog the cpu
                        Thread.Sleep(10);
                        //Console.WriteLine("work");
                    }
                        #endregion
                    #endregion
                });
            workerThread.RunWorkerAsync();
            Console.WriteLine("Press enter to kill!");
            Console.ReadLine();
            netServer.Shutdown("Requested by user");
        }

        private static void SendClientIdentification(byte which) {
            if (which == 1) {
                //send to first client
                List<NetConnection> one = netServer.Connections;
                one.Remove(con2);
                NetOutgoingMessage om1 = netServer.CreateMessage();
                om1.Write(SGlobal.CLIENT_IDENTIFICATION_MESSAGE);
                om1.Write((byte)1);
                netServer.SendMessage(om1, one, NetDeliveryMethod.ReliableUnordered, 0);
            }
            else if (which == 2) {
                //send to second client
                List<NetConnection> two = netServer.Connections;
                two.Remove(con1);
                NetOutgoingMessage om2 = netServer.CreateMessage();
                om2.Write(SGlobal.CLIENT_IDENTIFICATION_MESSAGE);
                om2.Write((byte)2);
                netServer.SendMessage(om2, two, NetDeliveryMethod.ReliableUnordered, 0);
            }
            else {
                //send to first client
                List<NetConnection> one = netServer.Connections;
                one.Remove(con2);
                NetOutgoingMessage om1 = netServer.CreateMessage();
                om1.Write(SGlobal.CLIENT_IDENTIFICATION_MESSAGE);
                om1.Write((byte)1);
                netServer.SendMessage(om1, one, NetDeliveryMethod.ReliableUnordered, 0);

                //send to second client
                List<NetConnection> two = netServer.Connections;
                two.Remove(con1);
                NetOutgoingMessage om2 = netServer.CreateMessage();
                om2.Write(SGlobal.CLIENT_IDENTIFICATION_MESSAGE);
                om2.Write((byte)2);
                netServer.SendMessage(om2, two, NetDeliveryMethod.ReliableUnordered, 0);
            }
        }

        /// <summary>
        /// use this method to divert the types into the appropriate classes
        /// </summary>
        internal static void InstanceClass(Int32 _type, byte _faction, Vector2f _position, float _health) {
            bool success = true;
            int _ID = SGlobal.ID_COUNTER++;
            ElongateList(_ID);
            switch (_type) {
                case SGlobal.BUILDING_DEFAULT:
                    Sendlist[_ID] = new Buildings.Centre(_type, _faction, _ID, _position, _health);
                    break;
                case SGlobal.BUILDING_RED:
                    Sendlist[_ID] = new Buildings.Centre(_type, _faction, _ID, _position, _health);
                    break;
                case SGlobal.BUILDING_BLUE:
                    Sendlist[_ID] = new Buildings.Centre(_type, _faction, _ID, _position, _health);
                    break;
                case SGlobal.BUILDING_BARRACK:
                    Sendlist[_ID] = new Barrack(_type, _faction, _ID, _position, _health);
                    break;
                case SGlobal.BUILDING_STONEHACKER:
                    Sendlist[_ID] = new Buildings.StoneHackerBuilding(_type, _faction, _ID, _position, _health);
                    break;
                case SGlobal.RESSOURCE_STONE:
                    Sendlist[_ID] = new Stone(_type, _faction, _ID, _position, _health);
                    break;
                case SGlobal.RESSOURCE_WOOD:
                    Sendlist[_ID] = new Wood(_type, _faction, _ID, _position, _health);
                    break;
                case SGlobal.PEOPLE_PEASANT:
                    Sendlist[_ID] = new Entities.Peasant(_type, _faction, _ID, _position, _health);
                    break;
                case SGlobal.PEOPLE_SWORDMAN:
                    Sendlist[_ID] = new Entities.Swordsman(_type, _faction, _ID, _position, _health);
                    break;
                case SGlobal.PEOPLE_STONEMAN:
                    Sendlist[_ID] = new Entities.Stoneguy(_type, _faction, _ID, _position, _health);
                    break;
                default:
                    success = false;
                    SGlobal.ID_COUNTER--;
                    break;
            }
            if (success)
                Console.WriteLine("instanced: " + _type + ";player " + _faction);
            else
                Console.WriteLine('\n' + "Failed to find the correct building type; see Server.cs:InstanceClass" + '\n');
        }

        private static void ElongateList(int _ID) {
            //if the list size is smaller than the id, elongate it
            if (Sendlist.Count - 1 < _ID)
                while (Sendlist.Count - 1 < _ID)
                    Sendlist.Add(null);
        }

        // called by the UI
        public static void Shutdown() {
            netServer.Shutdown("Requested by user");
        }
    }
}
