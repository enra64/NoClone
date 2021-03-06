﻿using System;
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
        //public static NetClient netClient {get;private set;}
        //c for current
        public static RenderWindow cRenderWindow { get; private set; }
        public static View gameView{get;private set;}
        public static UserInterface cInterface { get; private set; }
        public static Sprite cMouseSprite { get; set; }
        public static bool cIsFocused { get; private set; }
        public static byte MyFaction { get; set; }
        public static Ressources.RessourceKeeper MyRessources {get; set;}
        //zoom level
        public static float cZoomFactor = 1;
        //declare map
        public static TileEngine map;


        //drag rectangle
        public static RectangleShape dragRect;

        //list of everything meant to draw
        public static List<AbstractClientItem> cItemList {get; set;}
        static void Main(string[] args){
            //create list of items
            cItemList = new List<AbstractClientItem>();

            //create window
            //cRenderWindow = new RenderWindow(VideoMode.FullscreenModes[0], "Codename Project Two", Styles.Fullscreen); //fullscreen
            cRenderWindow = new RenderWindow(new VideoMode(1366, 768), "Codename Project Two", Styles.Default);
            cRenderWindow.SetVerticalSyncEnabled(true);
            cRenderWindow.SetFramerateLimit(35);

            //event handlers
            cRenderWindow.Closed += delegate { Communication.Shutdown(); cRenderWindow.Close(); };
            cRenderWindow.MouseButtonPressed += MouseHandling.mouseClick;
            cRenderWindow.MouseWheelMoved += MouseHandling.Scrolling;
            cRenderWindow.MouseButtonReleased += MouseHandling.mouseRelease;
            cRenderWindow.MouseMoved += MouseHandling.MouseMoved;

            //only listen to key on window focus
            cRenderWindow.MouseEntered += delegate { cIsFocused = true; };
            cRenderWindow.MouseLeft += delegate { cIsFocused = false; };
            cRenderWindow.GainedFocus += delegate { cIsFocused = true; };
            cRenderWindow.LostFocus += delegate { cIsFocused = false; };

            //first and only call to init, do everything else there
            Initialize();
            //main game loop
            while (cRenderWindow.IsOpen()){
                //mandatory and draw calls; note that update does _not_ calculate anything
                Update();
                Draw();
                //dispatch events
                cRenderWindow.DispatchEvents();
            }
        }

        private static void KeyCheck()
        {
            //dont react to keys if window is not active
            if (!cIsFocused)
                return;
            if (Keyboard.IsKeyPressed(Keyboard.Key.Escape))
                cRenderWindow.Close();
            if (Keyboard.IsKeyPressed(Keyboard.Key.W))
                gameView.Move(new Vector2f(0, -20f * cZoomFactor));
            if (Keyboard.IsKeyPressed(Keyboard.Key.S))
                gameView.Move(new Vector2f(0, 20f * cZoomFactor));
            if (Keyboard.IsKeyPressed(Keyboard.Key.A))
                gameView.Move(new Vector2f(-20f * cZoomFactor, 0));
            if (Keyboard.IsKeyPressed(Keyboard.Key.D))
                gameView.Move(new Vector2f(20f * cZoomFactor, 0));
            //zoom out
            if (Keyboard.IsKeyPressed(Keyboard.Key.Q)){
                if(cZoomFactor < 3)
                    cZoomFactor+=.05f;
                Console.WriteLine(cZoomFactor);
                gameView.Zoom(1.07f);
            }
            //zoom in
            if (Keyboard.IsKeyPressed(Keyboard.Key.E)){
                if (cZoomFactor > 0.6f)
                    cZoomFactor-=.05f;
                Console.WriteLine(cZoomFactor);
                gameView.Zoom(.93f);
            }
        }

        static bool isAnim = false;


        private static void Update(){

            //check for keys
            KeyCheck();
            //update offset
            CGlobal.CURRENT_WINDOW_ORIGIN = cRenderWindow.GetView().Center - new Vector2f((float)cRenderWindow.Size.X / 2f, (float)cRenderWindow.Size.Y / 2f);
            //update ui
            cInterface.Update();
        }

        private static void Draw(){
            cRenderWindow.SetView(gameView);
            cRenderWindow.Clear();
            //draw tilemap
            map.Draw();

            //draw buildings
            for (int i = cItemList.Count - 1; i >= 0; i--) {
                AbstractClientItem s = cItemList[i];
                if (s != null && s.Health > 0 && s.Type < 100)
                    s.Draw();
            }
            //draw all people
            for (int i = cItemList.Count - 1; i >= 0; i--) {
                AbstractClientItem s = cItemList[i];
                if (s != null && s.Health > 0 && s.Type >= 100)
                    s.Draw();
            }

            if(MouseHandling.b != null)
                cRenderWindow.Draw(MouseHandling.b);

            for (int i = cItemList.Count - 1; i >= 0; i--) {
                break;
                AbstractClientItem s = cItemList[i];
                if (s != null && s.Health > 0)
                    s.DrawOutline();
            }
            cInterface.Draw();
            //draw collection rect
            cRenderWindow.Draw(dragRect);
            //draw hovering building
            if(cMouseSprite!=null)
                cRenderWindow.Draw(cMouseSprite);
            //display everything
            cRenderWindow.Display();
            cRenderWindow.SetMouseCursorVisible(true);
        }

        private static void Initialize(){
            //first and only call to load content, mandatory to use
            LoadContent();
            //set beginning view.
            gameView = new View(new FloatRect(0, 0, 1366, 768));

            //set view origin and current in static client global class
            CGlobal.BEGIN_WINDOW_ORIGIN = cRenderWindow.GetView().Center - new Vector2f((float)cRenderWindow.Size.X / 2f, (float)cRenderWindow.Size.Y / 2f);
            CGlobal.CURRENT_WINDOW_ORIGIN = cRenderWindow.GetView().Center - new Vector2f((float)cRenderWindow.Size.X / 2f, (float)cRenderWindow.Size.Y / 2f);

            cInterface= new UserInterface();

            //build tilemap
            map = new TileEngine(cRenderWindow, new Vector2u(100, 100), "maps/levelTest1.oel");

            //instance ressourcekeeper
            MyRessources = new Ressources.RessourceKeeper(MyFaction, 100, 100);

            //Console.WriteLine("Write IP and press Enter to connect!");
            //Communication.Connect(Console.ReadLine(), 14242);

            dragRect = new RectangleShape();
            dragRect.FillColor = Color.Transparent;
            dragRect.OutlineColor = Color.White;
            dragRect.OutlineThickness = 2;

            //init connection
            Console.WriteLine("Autoconnecting to localhost!");
            //at this point, the size of every texture is known, not the scale however, which is ultra shitty.
            Communication.Connect("localhost", 14242);
        }

        /// <summary>
        /// Load the stuff the UI (and you!) needs here
        /// </summary>
        private static void LoadContent(){
            //load building textures
            CGlobal.BUILDING_TEXTURES[0] = new Texture("assets/graphics/buildings/default.png");
            CGlobal.BUILDING_TEXTURES[1] = new Texture("assets/graphics/buildings/hqred.png");
            CGlobal.BUILDING_TEXTURES[2] = new Texture("assets/graphics/buildings/hqblue.png");
            CGlobal.BUILDING_TEXTURES[3] = new Texture("assets/graphics/buildings/barrack.png");
            CGlobal.BUILDING_TEXTURES[4] = new Texture("assets/graphics/ressources/stone.png");
            CGlobal.BUILDING_TEXTURES[5] = new Texture("assets/graphics/ressources/wood.png");
            CGlobal.BUILDING_TEXTURES[6] = new Texture("assets/graphics/buildings/stonehacker.png");

            //load people textures
            CGlobal.PEOPLE_TEXTURES[0] = new Texture("assets/graphics/units/Bauer.png");
            CGlobal.PEOPLE_TEXTURES[1] = new Texture("assets/graphics/units/soldier.png");
            CGlobal.PEOPLE_TEXTURES[2] = new Texture("assets/graphics/units/stonepeople.png");
        }
    }
}