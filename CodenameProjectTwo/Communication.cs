using Lidgren.Network;
using SFML.Window;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

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
        }

        internal static void BroadcastUpdate(NetIncomingMessage msg)
        {
            //read newest message until empty
            while (msg.PeekInt32() != -1)
            {
                //Console.WriteLine("client: broadcast update");
                int type = msg.ReadInt32();
                bool faction = msg.ReadBoolean();
                int ID = msg.ReadInt32();
                Vector2f position = new Vector2f(msg.ReadFloat(), msg.ReadFloat());
                float health = msg.ReadFloat();
                //if the list size is smaller than the id, elongate it
                if (Client.cItemList.Count - 1 < ID)
                    while (Client.cItemList.Count - 1 < ID) {
                            Client.cItemList.Add(null);
                            Console.WriteLine("elong 1");
                        }
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
        }

        /// <summary>
        /// use this class to divert the types into the appropriate classes
        /// </summary>
        internal static void InstanceClass(int _type, bool _faction, int _ID, Vector2f _position, float _health)
        {
            Console.WriteLine("instanced a type " + _type + " of player " + _faction);
            switch (_type)
            {
                case CGlobal.VILLAGE_CENTRE_TYPE:
                    Client.cItemList[_ID] = new Building(_type, _faction, _ID, _position, _health);
                    break;
                default:
                    Client.cItemList[_ID] = new Building(_type, _faction, _ID, _position, _health);
                    break;
            }
            Console.WriteLine("instantiating");
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
                        Console.WriteLine(text);
                        break;
                    case NetIncomingMessageType.StatusChanged:
                        NetConnectionStatus status = (NetConnectionStatus)im.ReadByte();
                        string reason = im.ReadString();
                        //Console.WriteLine(status.ToString() + ": " + reason);
                        break;
                    case NetIncomingMessageType.Data:
                        if (im.ReadInt32() == CGlobal.GAMESTATE_BROADCAST)
                            BroadcastUpdate(im);
                        break;
                    default:
                        Console.WriteLine("Unhandled type: " + im.MessageType + " " + im.LengthBytes + " bytes");
                        break;
                }
                netClient.Recycle(im);
            }
        }
    }
}
