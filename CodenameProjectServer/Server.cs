﻿using Lidgren.Network;
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

namespace CodenameProjectServer
{
    class Server
    {
        private const long UNDEFINED_CLIENT=-1;

        private static NetServer netServer;
        public static List<SInterfaces.ISendable> Sendlist = new List<SInterfaces.ISendable>();

        private static long clientIdentifier1 = UNDEFINED_CLIENT, clientIdentifier2 = UNDEFINED_CLIENT;
        private static NetConnection con1, con2;

        private static int lastExecutedClick = -1;
        private static Vector2f lastMouseClickPosition;

        private static BackgroundWorker workerThread=new BackgroundWorker();

        static void Main(string[] args){
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
            InstanceClass(SGlobal.BUILDING_DEFAULT, 0, new Vector2f(100f, 100f), 100);
            InstanceClass(SGlobal.BUILDING_RED, 1, new Vector2f(570f, 720f), 100);
            InstanceClass(SGlobal.BUILDING_BLUE, 2, new Vector2f(400f, 200f), 100);

            //instantiate ressources
            InstanceClass(SGlobal.RESSOURCE_STONE, 0, new Vector2f(200f, 200f), 100);

            //instantiate ressources
            InstanceClass(SGlobal.PEOPLE_PEASENT, 1, new Vector2f(200, 300), 100);
            //asynchronous server worker
            workerThread.DoWork += new DoWorkEventHandler(
                delegate(object o, DoWorkEventArgs arg)
                {
                    #region Serverloop
                    while (true) {
                        #region DoCalculations
                        foreach (SInterfaces.ISendable s in Sendlist)
                            if (s != null)
                                s.Update();
                        //do intersection calculation; iterate through every item
                        for (int i = Sendlist.Count - 1; i >= 0; i--) {
                            //only iterate through items that actually would react, like people...
                            if (Sendlist[i].implementAggroOrEffectEffects) {
                                //iterate through all other items
                                for (int j = Sendlist.Count - 1; j >= 0; j--) {
                                    //dont check yourself
                                    if (i != j) {
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
                        #endregion
                        #region sendCalculations
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
                                            con1=im.SenderConnection;
                                        }
                                        else if (clientIdentifier2 == UNDEFINED_CLIENT)
                                        {
                                            clientIdentifier2 = im.SenderConnection.RemoteUniqueIdentifier;
                                            hail = "Client 2 (" + clientIdentifier2 + ")";
                                            con2 = im.SenderConnection;
                                        }
                                        if (clientIdentifier1 != UNDEFINED_CLIENT && clientIdentifier2 != UNDEFINED_CLIENT)
                                            SendClientIdentification();
                                        //Console.WriteLine(hail);
                                    }
                                    break;
                                #endregion

                                #region DataHandling
                                case NetIncomingMessageType.Data:
                                    //identify client
                                    byte client=255;
                                    // incoming chat message from a client
                                    if (im.SenderConnection.RemoteUniqueIdentifier == clientIdentifier1){
                                        Console.Write("client 1: ");
                                        client = 1;
                                    }
                                    else if (im.SenderConnection.RemoteUniqueIdentifier == clientIdentifier2)
                                    {
                                        Console.Write("client 2: ");
                                        client = 2;
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
                                            //get position
                                            Vector2f clickPosition = new Vector2f(im.ReadFloat(), im.ReadFloat());
                                            //left click: save clicked item
                                            if (im.ReadBoolean() == false){
                                                Console.WriteLine("Item "+item+" was clicked! type: "+Sendlist[item].Type);
                                                lastExecutedClick = item;
                                            }
                                            //got right click, notify first clicked item of new targetposition
                                            else {
                                                if (lastExecutedClick != -1) {
                                                    if(item==-1){
                                                    
                                                        Console.WriteLine(clickPosition);
                                                        Sendlist[lastExecutedClick].Target = clickPosition; //Sendlist[item].Position; TODO Arne mach mal XD
                                                        Sendlist[lastExecutedClick].TargetID = -1;
                                                    }
                                                    else {
                                                        Console.WriteLine(Sendlist[item].Position);
                                                        Sendlist[lastExecutedClick].Target = Sendlist[item].Position;
                                                        Sendlist[lastExecutedClick].TargetID = item;
                                                    }
                                                }
                                            }
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
                    #endregion
                });
            workerThread.RunWorkerAsync();
            Console.WriteLine("Press enter to kill!");
            Console.ReadLine();
            //ServerLoop();
            netServer.Shutdown("Requested by user");
        }

        private static void SendClientIdentification()
        {
            //send to first client
            List<NetConnection> one = netServer.Connections;
            one.Remove(con2);
            NetOutgoingMessage om1 = netServer.CreateMessage();
            om1.Write(SGlobal.CLIENT_IDENTIFICATION_MESSAGE);
            om1.Write(false);
            netServer.SendMessage(om1, one, NetDeliveryMethod.ReliableUnordered, 0);

            //send to second client
            List<NetConnection> two = netServer.Connections;
            two.Remove(con1);
            NetOutgoingMessage om2 = netServer.CreateMessage();
            om2.Write(SGlobal.CLIENT_IDENTIFICATION_MESSAGE);
            om2.Write(true);
            netServer.SendMessage(om2, two, NetDeliveryMethod.ReliableUnordered, 0);
        }

        /// <summary>
        /// use this class to divert the types into the appropriate classes
        /// </summary>
        internal static void InstanceClass(Int32 _type, byte _faction, Vector2f _position, float _health)
        {
            bool success = true;
            int _ID = SGlobal.ID_COUNTER++;
            ElongateList(_ID);
            switch (_type)
            {
                case SGlobal.BUILDING_DEFAULT:
                    Sendlist[_ID] = new Buildings.Centre(_type, _faction, _ID, _position, _health);
                    break;
                case SGlobal.BUILDING_RED:
                    //check for needed resources
                    Sendlist[_ID] = new Buildings.Centre(_type, _faction, _ID, _position, _health);
                    break;
                case SGlobal.BUILDING_BLUE:
                    Sendlist[_ID] = new Buildings.Centre(_type, _faction, _ID, _position, _health);
                    break;
                case SGlobal.BUILDING_BARRACK:
                    Sendlist[_ID] = new Barrack(_type, _faction, _ID, _position, _health);
                    break;
                case SGlobal.RESSOURCE_STONE:
                    Sendlist[_ID] = new Stone(_type, _faction, _ID, _position, _health);
                    break;
                case SGlobal.PEOPLE_PEASENT:
                    Sendlist[_ID] = new Entities.Peasant(_type, _faction, _ID, _position, _health);
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
