using Lidgren.Network;
using SFML.Window;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using CodenameProjectTwo.Buildings;

namespace CodenameProjectTwo
{
    static class Communication
    {
        public static NetClient netClient { get; private set; }
        //shut down communication
        internal static void Shutdown()
        {
            netClient.Disconnect("Requested by user");
        }

        //connect to something
        internal static void Connect(string host, int port)
        {
            //need to create this b/c console app
            SynchronizationContext.SetSynchronizationContext(new SynchronizationContext());

            //init and config network connection
            NetPeerConfiguration config = new NetPeerConfiguration("CodenameProjectTwo");
            config.AutoFlushSendQueue = false;
            netClient = new NetClient(config);
            netClient.RegisterReceivedCallback(new SendOrPostCallback(GotMessage));

            netClient.Start();
            NetOutgoingMessage hail = netClient.CreateMessage("Connection success!");
            netClient.Connect(host, port, hail);

            //wait for connection success
            Console.WriteLine("Waiting for Connection...");
            while (netClient.ConnectionStatus != NetConnectionStatus.Connected) { }

            //communication established - inform server of item sizes!
            NetOutgoingMessage om = netClient.CreateMessage();
            //identify message
            om.Write(CGlobal.BOUNDINGSIZE_MESSAGE);
            for (int i = 0; i < CGlobal.PEOPLE_TEXTURES.Length; i++) {
                om.Write((Int32)i + CGlobal.PEOPLE_ID_OFFSET);//type
                om.Write((Int32)CGlobal.PEOPLE_TEXTURES[i].Size.X);//x size
                om.Write((Int32)CGlobal.PEOPLE_TEXTURES[i].Size.Y);//y size
            }
            for (int i = 0; i < CGlobal.BUILDING_TEXTURES.Length; i++) {
                om.Write((Int32)i);//type
                om.Write((Int32)CGlobal.BUILDING_TEXTURES[i].Size.X);//x size
                om.Write((Int32)CGlobal.BUILDING_TEXTURES[i].Size.Y);//y size
            }

            //casting everything because i am paranoid btw
            om.Write((Int32) (-1));

            //send
            netClient.SendMessage(om, NetDeliveryMethod.ReliableOrdered);
            //like, really, send now
            netClient.FlushSendQueue();
        }

        internal static void BroadcastUpdate(NetIncomingMessage msg)
        {
            //read newest message until all data has been read
            while (msg.PeekInt32() != -42){
                //read message
                int type = msg.ReadInt32();
                byte faction = msg.ReadByte();
                int ID = msg.ReadInt32();
                Vector2f position = new Vector2f(msg.ReadFloat(), msg.ReadFloat());
                float health = msg.ReadFloat();
                //if the list size is smaller than the id, elongate it
                if (Client.cItemList.Count - 1 < ID)
                    while (Client.cItemList.Count - 1 < ID)
                            Client.cItemList.Add(null);
                //decide whether to instance or update
                if (Client.cItemList[ID] != null)//update
                {
                    //now assign the new values
                    Client.cItemList[ID].Health = health;
                    Client.cItemList[ID].Position = position;
                }
                else//instance
                    InstanceClass(type, faction, ID, position, health);
            }
            //kill broadcast part 1 stop bit
            msg.ReadInt32();
            //read ressource states
            while (msg.PeekInt32() != -1){
                Byte factionIdentification = msg.ReadByte();
                //Console.WriteLine("factionident: " + factionIdentification+", soll: "+Client.MyFaction);
                if (factionIdentification == Client.MyFaction)
                {
                    Client.MyRessources.Stone = msg.ReadInt32();
                    Client.MyRessources.Wood = msg.ReadInt32();
                }
                else{
                    msg.ReadInt32();
                    msg.ReadInt32();
                }
            }
        }

        /// <summary>
        /// use this class to divert the types into the appropriate classes
        /// </summary>
        internal static void InstanceClass(int _type, byte _faction, int _ID, Vector2f _position, float _health)
        {
            switch (_type)
            {
                case CGlobal.BUILDING_RED:
                    Client.cItemList[_ID] = new Centre(_type, _faction, _ID, _position, _health);
                    break;
                case CGlobal.BUILDING_BLUE:
                    Client.cItemList[_ID] = new Centre(_type, _faction, _ID, _position, _health);
                    break;
                case CGlobal.BUILDING_BARRACK:
                    Client.cItemList[_ID] = new Barrack(_type, _faction, _ID, _position, _health);
                    break;
                case CGlobal.RESSOURCE_STONE:
                    Client.cItemList[_ID] = new Ressources.Stone(_type, _faction, _ID, _position, _health);
                    break;
                case CGlobal.BUILDING_STONEHACKER:
                    Client.cItemList[_ID] = new StoneHackerBuilding(_type, _faction, _ID, _position, _health);
                    break;
                case CGlobal.RESSOURCE_WOOD:
                    Client.cItemList[_ID] = new Ressources.Wood(_type, _faction, _ID, _position, _health);
                    break;
                case CGlobal.PEOPLE_PEASANT:
                    Client.cItemList[_ID] = new Peasant(_type, _faction, _ID, _position, _health);
                    break;
                case CGlobal.PEOPLE_SWORDMAN:
                    Client.cItemList[_ID] = new Swordsman(_type, _faction, _ID, _position, _health);
                    break;
                case CGlobal.PEOPLE_STONEMAN:
                    Client.cItemList[_ID] = new Stoneguy(_type, _faction, _ID, _position, _health);
                    break;
                default:
                    Client.cItemList[_ID] = new Building(_type, _faction, _ID, _position, _health);
                    break;
            }
        }

        internal static void GotMessage(object peer)
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
                        //Console.WriteLine(text);
                        break;
                    case NetIncomingMessageType.StatusChanged:
                        NetConnectionStatus status = (NetConnectionStatus)im.ReadByte();
                        string reason = im.ReadString();
                        break;
                    case NetIncomingMessageType.Data:
                        Int32 dataType=im.ReadInt32();
                        switch (dataType)
                        {
                            case CGlobal.CLIENT_IDENTIFICATION_MESSAGE:
                                Client.MyFaction = im.ReadByte();
                                Console.WriteLine("client ident " + Client.MyFaction);
                                break;
                            case CGlobal.GAMESTATE_BROADCAST:
                                BroadcastUpdate(im);
                                break;
                        }
                        break;
                    default:
                        Console.WriteLine("Unhandled type: " + im.MessageType + " " + im.LengthBytes + " bytes");
                        break;
                }
                netClient.Recycle(im);
            }
        }

        internal static void sendMassSelection(List<int> selected) {
            //create message
            NetOutgoingMessage mes = Communication.netClient.CreateMessage();
            //identify message as mouseclick
            mes.Write(CGlobal.MASS_SELECTION_MESSAGE);
            //write id of clicked item
            foreach(int i in selected)
                mes.Write(i);
            //write "left mouse button"
            mes.Write(-44);
            //send
            Communication.netClient.SendMessage(mes, NetDeliveryMethod.ReliableOrdered);
            //like, really, send now
            Communication.netClient.FlushSendQueue();
        }
    }
}
