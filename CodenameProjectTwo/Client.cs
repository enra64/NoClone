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
        public static NetClient netClient {get;private set;}
        //c for current
        public static RenderWindow cRenderWindow { get; private set; }
        public static View cView{get;private set;}
        public static Interface cInterface { get; private set; }
        public static Sprite cMouseSprite { get; set; }

        //declare map
        public static TileEngine map;

        //list of everything meant to draw
        public static List<CInterfaces.IDrawable> cItemList {get; set;}
        static void Main(string[] args)
        {
            //need to create this b/c console app
            SynchronizationContext.SetSynchronizationContext(new SynchronizationContext());

            //init and config network connection
            NetPeerConfiguration config = new NetPeerConfiguration("CodenameProjectTwo");
            config.AutoFlushSendQueue=false;
            netClient = new NetClient(config);
            netClient.RegisterReceivedCallback(new SendOrPostCallback(GotMessage));

            Console.WriteLine("Write IP and press Enter to connect!");
            Connect(Console.ReadLine(), 14242);
            
            Console.WriteLine("Waiting for Connection...");
            
            //wait for connection success
            while(netClient.ConnectionStatus!=NetConnectionStatus.Connected){}

            //create list of items
            cItemList=new List<CInterfaces.IDrawable>();

            //create window
            //cRenderWindow = new RenderWindow(VideoMode.FullscreenModes[0], "Codename Project Two", Styles.Fullscreen); //fullscreen
            cRenderWindow = new RenderWindow(new VideoMode(1366, 768), "Codename Project Two", Styles.Default);
            cRenderWindow.SetVerticalSyncEnabled(true);
            cRenderWindow.SetFramerateLimit(35);

            //event handlers
            cRenderWindow.Closed += windowClosed;
            cRenderWindow.MouseButtonPressed += MouseHandling.mouseClick;
            cRenderWindow.MouseWheelMoved += MouseHandling.Scrolling;
            cRenderWindow.MouseButtonReleased += MouseHandling.mouseRelease;
            cRenderWindow.MouseMoved += MouseHandling.MouseMoved;

            //first and only call to load content, not mandatory to use
            LoadContent();
            //first and only call to init, do everything else there
            Initialize();

            //main game loop
            while (cRenderWindow.IsOpen())
            {
                //mandatory and draw calls; note that update does _not_ calculate anything
                Update();
                Draw();
                //dispatch events
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

        private static void KeyCheck(){
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
        }

        private static void Update(){
            KeyCheck();
            CGlobal.CURRENT_WINDOW_ORIGIN = cRenderWindow.GetView().Center - new Vector2f((float)cRenderWindow.Size.X / 2f, (float)cRenderWindow.Size.Y / 2f);
            cInterface.Update();
        }

        private static void BroadcastUpdate(NetIncomingMessage msg){
                    //read newest message until empty
                    while (msg.PeekInt32() != -1){
                        int type = msg.ReadInt32();
                        bool faction = msg.ReadBoolean();
                        int ID = msg.ReadInt32();
                        Vector2f position = new Vector2f(msg.ReadFloat(), msg.ReadFloat());
                        float health = msg.ReadFloat();
                        //if the list size is smaller than the id, we need to instance the correct class
                        if (cItemList.Count - 1 < ID)
                            while (cItemList.Count - 1 < ID)
                                cItemList.Add(null);
                        //decide whether to instance or update
                        if (cItemList[ID] != null)//update
                        {
                            //now assign the new values
                            cItemList[ID].Health = health;
                            cItemList[ID].Position = position;
                        }
                        else//instance
                            InstanceClass(type, faction, ID, position, health);
                    }
                }


        /// <summary>
        /// use this class to divert the types into the appropriate classes
        /// </summary>
        private static void InstanceClass(int _type, bool _faction, int _ID, Vector2f _position, float _health){
            Console.WriteLine("instanced a type " + _type + " of player " + _faction);
            switch (_type)
            {
                case 0:default:
                    cItemList[_ID]=new Building(_type, _faction, _ID, _position, _health);
                    break;
            }
        }

        private static void Draw()
        {
            cRenderWindow.SetView(cView);
            cRenderWindow.Clear();
            //these two lines are why we use interfaces ;)
            foreach (CInterfaces.IDrawable s in cItemList)
                s.Draw();
            map.Draw();
            cInterface.Draw();
            if(cMouseSprite!=null)
                cRenderWindow.Draw(cMouseSprite);
            cRenderWindow.Display();
            cRenderWindow.SetMouseCursorVisible(true);
        }

        private static void Initialize()
        {
            //set beginning view.
            cView = new View(new FloatRect(0, 0, 1366, 768));
            //set view origin and current in static client global class
            CGlobal.BEGIN_WINDOW_ORIGIN = cRenderWindow.GetView().Center - new Vector2f((float)cRenderWindow.Size.X / 2f, (float)cRenderWindow.Size.Y / 2f);
            CGlobal.CURRENT_WINDOW_ORIGIN = cRenderWindow.GetView().Center - new Vector2f((float)cRenderWindow.Size.X / 2f, (float)cRenderWindow.Size.Y / 2f);

            cInterface= new Interface();

            //build tilemap
            map = new TileEngine(cRenderWindow, new Vector2u(100, 100), "maps/levelTest1.oel");
        }


        private static void LoadContent()
        {
            CGlobal.BUILDING_TEXTURES[0] = new Texture("assets/graphics/test.png");
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

        //shut down communication
        private static void Shutdown(){
            netClient.Disconnect("Requested by user");
        }

        //connect to something
        private static void Connect(string host, int port){
            netClient.Start();
            NetOutgoingMessage hail = netClient.CreateMessage("Connection success!");
            netClient.Connect(host, port, hail);
        }
    }
}
