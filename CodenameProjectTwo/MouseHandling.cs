using Lidgren.Network;
using SFML.Graphics;
using SFML.Window;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodenameProjectTwo {
    static class MouseHandling {
        //handle right-click-mouse-moving
        private static Vector2f mouseMovementStartingPoint;
        private static bool rightButtonClicked = false;
        public static int buildingChosen = -1;
        private static Sprite cChosenBuilding;

        public static void mouseRelease(object sender, MouseButtonEventArgs e) {
            //only do for right mouse button
            if (rightButtonClicked == false)
                return;
            //abort building choosing
            if (buildingChosen != -1) {
                buildingChosen = -1;
                return;
            }
            buildingChosen = -1;
            //check whether the mouse moved significantly or not
            FloatRect checkRect = new FloatRect(mouseMovementStartingPoint.X - 4f, mouseMovementStartingPoint.Y - 4f, 8f, 8f);
            if (checkRect.Contains(e.X, e.Y))
                sendMouseMessage(GetClickedItemId(e.X, e.Y), e.X, e.Y, true);
            else
                Client.cView.Move(new Vector2f(-(e.X - mouseMovementStartingPoint.X), -(e.Y - mouseMovementStartingPoint.Y)));
            Console.WriteLine(checkRect);
        }

        public static void Scrolling(object sender, MouseWheelEventArgs e) {
            if (e.Delta < 0)
                Client.cView.Zoom(1.02f);
            else
                Client.cView.Zoom(0.98f);
        }

        private static bool IsInMenu(float x) {
            return ((Client.cRenderWindow.Size.X * Client.cInterface.menuPortScale) > x);
        }

        /// <summary>
        /// Handle mouse clicks, decide whether to move map
        /// or send event to server
        /// </summary>
        public static void mouseClick(object sender, MouseButtonEventArgs e) {
            //decide whether menu or game screen
            //menu
            if (IsInMenu(e.X) && e.Button == Mouse.Button.Left) {
                Client.cInterface.Click(e.X, e.Y);
                Console.WriteLine("Menu click");
            }
            //game view
            else {
                if (e.Button == Mouse.Button.Left) {
                    rightButtonClicked = false;
                    //no building chosen
                    if (buildingChosen != -1) {
                        //send message to server for planting the building
                        Console.WriteLine("Planting building " + buildingChosen + " at " + e.X + ", " + e.Y);
                        sendPlantMessage(e.X, e.Y, buildingChosen);
                        buildingChosen = -1;
                        Client.cMouseSprite = null;
                    }
                    //building clicked
                    else {
                        //check what we clicked
                        Int32 clickedItemId = GetClickedItemId(e.X, e.Y);
                        //inform ui
                        Client.cInterface.ShowItem(clickedItemId);
                        //if an item was identified, send that to the server
                        if (clickedItemId != -1) {
                            Console.WriteLine("item " + clickedItemId + " clicked!");
                            sendMouseMessage(clickedItemId, e.X, e.Y, false);
                        }
                    }
                }
                //right mouse button
                else if (Mouse.IsButtonPressed(Mouse.Button.Right)) {
                    if (buildingChosen != -1) {
                        buildingChosen = -1;
                        Client.cMouseSprite = null;
                    }
                    else {
                        rightButtonClicked = true;
                        mouseMovementStartingPoint = new Vector2f(e.X, e.Y);
                    }
                }
            }
        }

        private static Int32 GetClickedItemId(float x, float y) {
            Vector2f mappedCoordinates = MapMouseToGame(x, y);
            x = mappedCoordinates.X;
            y = mappedCoordinates.Y;
            //return the clicked item id if found, -1 otherwise
            foreach (CInterfaces.IDrawable item in Client.cItemList) 
                if (item.BoundingRectangle.Contains(x, y)) 
                    return item.ID;
            return -1;
        }

        private static void sendPlantMessage(float x, float y, Int32 type) {
            //create message
            NetOutgoingMessage mes = Communication.netClient.CreateMessage();
            //identify message as mouseclick
            mes.Write(CGlobal.PLANT_BUILDING_MESSAGE);
            //write type of building to init
            mes.Write(type);
            //write "put it there"
            Vector2f convertedMousePosition = MapMouseToGame(x, y);
            mes.Write(convertedMousePosition.X);
            mes.Write(convertedMousePosition.Y);
            //send
            Communication.netClient.SendMessage(mes, NetDeliveryMethod.ReliableOrdered);
            //like, really, send now
            Communication.netClient.FlushSendQueue();
        }

        /// <summary>
        /// Called when you click a planted item
        /// </summary>
        private static void sendMouseMessage(int ID, float x, float y, bool rightButton) {
            //create message
            NetOutgoingMessage mes = Communication.netClient.CreateMessage();
            //identify message as mouseclick
            mes.Write(CGlobal.MOUSE_CLICK_MESSAGE);
            //write id of clicked item
            mes.Write(ID);
            //map mouse positions
            Vector2f mappedCoordinates = MapMouseToGame(x, y);
            x = mappedCoordinates.X;
            y = mappedCoordinates.Y;
            //write position incase id=-1
            mes.Write(x);
            mes.Write(y);
            //write "left mouse button"
            mes.Write(rightButton);
            //send
            Communication.netClient.SendMessage(mes, NetDeliveryMethod.ReliableOrdered);
            //like, really, send now
            Communication.netClient.FlushSendQueue();
        }

        internal static void MouseMoved(object sender, MouseMoveEventArgs e) {
            if (!IsInMenu(e.X) && buildingChosen != -1) {
                cChosenBuilding = new Sprite(CGlobal.BUILDING_TEXTURES[buildingChosen]);
                cChosenBuilding.Color = new Color(255, 255, 255, 120);
                cChosenBuilding.Position = MapMouseToGame(e.X, e.Y);
                Client.cMouseSprite = cChosenBuilding;
                return;
            }
            else {
                //check whether we are hovering over an item
                Client.cInterface.CheckHover(e);
            }

        }

        private static Vector2f MapMouseToGame(float x, float y) {
            return Client.cRenderWindow.MapPixelToCoords(new Vector2i((int)x, (int)y));
        }
    }
}
