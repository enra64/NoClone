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
        public static View cView{get;private set;}

        //handle right-click-mouse-moving
        private static Vector2f mouseMovementStartingPoint;

        //declare map
        private static TileEngine map;

        //list of everything meant to draw
        static List<CInterfaces.IDrawable> cItemlist = new List<CInterfaces.IDrawable>();
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
            Connect("192.168.0.7", 14242);

            //wait for connection success
            while(netClient.ConnectionStatus!=NetConnectionStatus.Connected)
                Console.WriteLine("Connection failure");

            //create window
            cRenderWindow = new RenderWindow(VideoMode.FullscreenModes[0], "Dungeon Dwarf", Styles.Fullscreen); //fullscreen
            //cRenderWindow = new RenderWindow(new VideoMode(1366, 768), "Codename Project Two", Styles.Default);
            cRenderWindow.SetVerticalSyncEnabled(true);
            cRenderWindow.SetFramerateLimit(35);

            //event handlers
            cRenderWindow.Closed += windowClosed;
            cRenderWindow.MouseButtonPressed += mouseClick;
            cRenderWindow.MouseWheelMoved += Scrolling;
            cRenderWindow.MouseButtonReleased += mouseRelease;

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

        private static void mouseRelease(object sender, MouseButtonEventArgs e)
        {
            //check whether the mouse moved significantly or not
            FloatRect checkRect = new FloatRect(mouseMovementStartingPoint.X - 4f, mouseMovementStartingPoint.Y - 4f, 8f, 8f);
            if (checkRect.Contains(e.X, e.Y))
                Console.WriteLine("should handle this somehow");
            else
                cView.Move(new Vector2f(e.X - mouseMovementStartingPoint.X, e.Y - mouseMovementStartingPoint.Y));
            Console.WriteLine(checkRect);
            Console.WriteLine(e.X+" x;y "+e.Y);
        }

        private static void Scrolling(object sender, MouseWheelEventArgs e)
        {
            if (e.Delta < 0)
                cView.Zoom(1.02f);
            else
                cView.Zoom(0.98f);
        }

        /// <summary>
        /// Handle mouse clicks, decide whether to move map
        /// or send event to server
        /// </summary>
        private static void mouseClick(object sender, MouseButtonEventArgs e)
        {
            if (e.Button==Mouse.Button.Left){
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
            else if (Mouse.IsButtonPressed(Mouse.Button.Right))
            {
                mouseMovementStartingPoint = new Vector2f(e.X, e.Y);
            }
        }

        private static void windowClosed(object sender, EventArgs e)
        {
            ((RenderWindow)sender).Close();
        }

        private static void KeyCheck()
        {
            if (Keyboard.IsKeyPressed(Keyboard.Key.Escape))
                cRenderWindow.Close();
            if (Keyboard.IsKeyPressed(Keyboard.Key.W))
                cView.Move(new Vector2f(0, -20f));
            if (Keyboard.IsKeyPressed(Keyboard.Key.S))
                cView.Move(new Vector2f(0, 20f));
            if (Keyboard.IsKeyPressed(Keyboard.Key.A))
                cView.Move(new Vector2f(-20f, 0));
            if (Keyboard.IsKeyPressed(Keyboard.Key.D))
                cView.Move(new Vector2f(20f, 0));
            if (Keyboard.IsKeyPressed(Keyboard.Key.Q))
                cView.Zoom(.99f);
            if (Keyboard.IsKeyPressed(Keyboard.Key.E))
                cView.Zoom(1.01f);
        }

        private static void Update(){
            KeyCheck();
        }

        private static void ServerUpdate()
        {
            NetIncomingMessage msg;
            while ((msg = netClient.ReadMessage()) != null)
            {
                if (msg.MessageType == NetIncomingMessageType.Data)
                {
                    if (msg.ReadInt32() == CGlobal.GAMESTATE_BROADCAST)
                    {
                        //Console.WriteLine("länge: " + msg.LengthBytes);
                        //read newest message until empty
                        while (msg.PeekInt32() != -1)
                        {
                            int type = msg.ReadInt32();
                            bool faction = msg.ReadBoolean();
                            int ID = msg.ReadInt32();
                            Vector2f position = new Vector2f(msg.ReadFloat(), msg.ReadFloat());
                            float health = msg.ReadFloat();
                            //if the list size is smaller than the id, we need to instance the correct class, 
                            if (cItemlist.Count - 1 < ID)
                                while (cItemlist.Count - 1 < ID)
                                    cItemlist.Add(null);
                            //decide whether to instance or update
                            if (cItemlist[ID] != null)//update
                            {
                                //now assign the new values
                                cItemlist[ID].Health = health;
                                cItemlist[ID].Position = position;
                            }
                            else//instance
                                InstanceClass(type, faction, ID, position, health);
                        }
                    }
                }
            }
        }

        /// <summary>
        /// use this class to divert the types into the appropriate classes
        /// </summary>
        /// <param name="_type"></param>
        /// <param name="_faction"></param>
        /// <param name="_ID"></param>
        /// <param name="_position"></param>
        /// <param name="_health"></param>
        private static void InstanceClass(int _type, bool _faction, int _ID, Vector2f _position, float _health){
            Console.WriteLine("instanced a type " + _type + " of player " + _faction);
            switch (_type)
            {
                case 0:default:
                    cItemlist[_ID]=new Building(_type, _faction, _ID, _position, _health);
                    break;
            }
        }

        private static void Draw()
        {
            cRenderWindow.SetView(cView);
            cRenderWindow.Clear();
            //these two lines are why we use interfaces ;)
            foreach (CInterfaces.IDrawable s in cItemlist)
                s.Draw();
            map.Draw();
            cRenderWindow.Display();
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
            //set view origin and current in static client global class
            CGlobal.BEGIN_WINDOW_ORIGIN = cRenderWindow.GetView().Center;
            CGlobal.CURRENT_WINDOW_ORIGIN = cRenderWindow.GetView().Center;

            //build tilemap
            map = new TileEngine(cRenderWindow, new Vector2u(100, 100), "maps/levelTest1.oel");
        }

        private static void LoadContent()
        {
            //throw new NotImplementedException();
        }

        private static void GotMessage(object peer)
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
                        ServerUpdate();
                        break;
                    default:
                        Console.WriteLine("Unhandled type: " + im.MessageType + " " + im.LengthBytes + " bytes");
                        break;
                }
                netClient.Recycle(im);
            }
        }

        //shut down communication
        private static void Shutdown()
        {
            netClient.Disconnect("Requested by user");
            // s_client.Shutdown("Requested by user");
        }

        //connevt to somethinf
        private static void Connect(string host, int port)
        {
            netClient.Start();
            NetOutgoingMessage hail = netClient.CreateMessage("Connection success!");
            netClient.Connect(host, port, hail);
        }

        // called by the UI
        private static void Send(string text)
        {
            NetOutgoingMessage om = netClient.CreateMessage(text);
            netClient.SendMessage(om, NetDeliveryMethod.Unreliable);
            Console.WriteLine("Sending '" + text + "'");
            netClient.FlushSendQueue();
        }
    }
}
