using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Lidgren.Network;
using System.Diagnostics;
using SFML.Graphics;
using SFML.Window;

namespace CodenameProjectTwo
{
    class Client
    {
        private static NetClient netClient;
        //c for current
        static RenderWindow cRenderWindow;
        static View cView;
        //you throw an int in, it throws an object back
        //static Dictionary<int, Interfaces.ISendable> receivedItemsList=new Dictionary<int, Interfaces.ISendable>();
        static List<CInterfaces.ISendable> cItemlist = new List<CInterfaces.ISendable>();
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
            //wait for connection success
            while(netClient.ConnectionStatus!=NetConnectionStatus.Connected){ }

            /*
             * These calls are done even before Initialize(), because
             * of a) their importance and b) dont fuck these up.
             * Do window creation
             */
            //ignore at this moment, because we are going to fuck up at first, and doing that in fullscreen is bad
            //creates fullscreen window at your maximum resolution
            //currentRenderWindow = new RenderWindow(VideoMode.FullscreenModes[0], "Dungeon Dwarf", Styles.Fullscreen);

            //create window
            cRenderWindow = new RenderWindow(new VideoMode(1366, 768), "Codename Project Two", Styles.Default);
            //currentRenderWindow = new RenderWindow(VideoMode.FullscreenModes[0], "Dungeon Dwarf", Styles.Fullscreen);
            //sets framerate to a maximum of 45; changing the value will likely result in bad things
            cRenderWindow.SetFramerateLimit(35);
            //add event handler for klicking the X icon
            cRenderWindow.Closed += windowClosed;
            //vertical sync is enabled, because master graphics n shit
            cRenderWindow.SetVerticalSyncEnabled(true);

            //add mouse click handling for getting focus
            cRenderWindow.MouseButtonPressed += mouseClick;

            //first and only call to load content, not mandatory to use
            LoadContent();
            //first and only call to init, do everything else there
            Initialize();

            /*
             * shit be about to get real... starting main loop.
             */
            while (cRenderWindow.IsOpen())
            {
                //mandatory and draw calls; note that update does _not_ calculate anything
                Update();
                Draw();
                //dispatch things like "i would like to close this window" and "someone clicked me".
                //only important if you want to close the window. ever.
                cRenderWindow.DispatchEvents();
            }
            
            
            
            //dont immediately shut down
            Console.ReadLine();
            Shutdown();
        }

        private static void windowClosed(object sender, EventArgs e)
        {
            ((RenderWindow)sender).Close();
        }

        private static void Update(){
            // process message here
            NetIncomingMessage msg;
            while ((msg = netClient.ReadMessage()) != null)
            {
                //read until message is empty
                while(msg.PeekInt32()!=null)
                {
                    int type = msg.ReadInt32();
                    int ID = msg.ReadInt32();
                    Vector2f position = new Vector2f(msg.ReadFloat(), msg.ReadFloat());
                    float health = msg.ReadFloat();
                    //if the list size is smaller than the id, we need to increase it
                    if (cItemlist.Count < ID)
                        while (cItemlist.Count < ID)
                            cItemlist.Add(null);
                    //now assign the new values
                    cItemlist[ID].Health = health;
                    cItemlist[ID].Position = position;
                }
            }
        }

        private static void Draw()
        {
            //these two lines are why we use interfaces ;)
            foreach (CInterfaces.ISendable s in cItemlist)
                s.Draw();
        }

        private static void mouseClick(object sender, MouseButtonEventArgs e)
        {
            Console.WriteLine(e.X + ": x, y: " + e.Y);
            NetOutgoingMessage mes = netClient.CreateMessage();
            //identify message as mouseclick
            mes.Write(CGlobal.MOUSE_CLICK_MESSAGE);
            //write x
            mes.Write(e.X + CGlobal.CURRENT_WINDOW_ORIGIN.X);
            //write y
            mes.Write(e.Y + CGlobal.CURRENT_WINDOW_ORIGIN.Y);
            //send
            netClient.SendMessage(mes, NetDeliveryMethod.ReliableOrdered);
            netClient.FlushSendQueue();
        }

        /// <summary>
        /// Generic send data function, takes the same stuff as lidgren send, so be careful ;)
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="_messageType"></param>
        /// <param name="_data"></param>
        private static void SendData<T>(int _messageType, T _data)
        {
            NetOutgoingMessage mes = netClient.CreateMessage();
            for (int i = 0; i < 100; i++)
                mes.Write(42f);
            netClient.SendMessage(mes, NetDeliveryMethod.ReliableUnordered);
            netClient.FlushSendQueue();
        }

        private static void Initialize()
        {
            //set beginning view.
            cView = new View(new FloatRect(0, 0, 1366, 768));
            //set view origin and current in static global class
            CGlobal.BEGIN_WINDOW_ORIGIN = cRenderWindow.GetView().Center;
            CGlobal.CURRENT_WINDOW_ORIGIN = cRenderWindow.GetView().Center;
        }

        private static void LoadContent()
        {
            //throw new NotImplementedException();
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
