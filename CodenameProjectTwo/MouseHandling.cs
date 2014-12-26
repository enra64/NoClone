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
        private const int NOTHING_SELECTED = -1;
        private static Vector2f mouseMovementStartingPoint;
        private static bool rightButtonClicked = false, leftButtonClicking = false;
        public static int lastClickedIstem = -1;
        public static int[] selectedItems;

        private static Vector2f cChosenBuildingSize, dragStartPoint;
        private static Sprite cChosenBuilding;

        public static void mouseRelease(object sender, MouseButtonEventArgs e){
            if(!rightButtonClicked){
                leftButtonClicking = false;
                if(Client.dragRect.OutlineThickness > 0){
                    //undraw dragrect
                    Client.dragRect.OutlineThickness = 0;
                    //check what has been clicked
                    Vector2f translatedPosition = MapMouseToGame(Client.dragRect.Position.X, Client.dragRect.Position.Y);
                    FloatRect selectionRect = new FloatRect(
                        translatedPosition.X,
                        translatedPosition.Y, 
                        Client.dragRect.Size.X, 
                        Client.dragRect.Size.Y);
                    Console.WriteLine("selrect:: left: " + selectionRect.Left + ", top: " + selectionRect.Top);
                    List<int> selectedPeople=new List<int>();
                    foreach (AbstractClientItem a in Client.cItemList) {
                        if (a.Type >= 100) {
                            Console.WriteLine(a.ID+"left: " + a.Sprite.Position.X + ", top: " + a.Sprite.Position.Y);
                            if (a.BoundingRectangle.Intersects(selectionRect)){

                                selectedPeople.Add(a.Type);
                            }
                        }
                    }
                    int mostSelected = -1;
                    if(selectedPeople.Count > 0)
                        mostSelected = selectedPeople.GroupBy(i => i).OrderByDescending(grp => grp.Count()).Select(grp => grp.Key).First();
                    Console.WriteLine("most: " + mostSelected+", count: " + selectedPeople.Count);
                }
            }
            else if (rightButtonClicked) {
                //deselect everything
                if (selectedItems != null) {
                    Client.cMouseSprite = null;
                    selectedItems = null;
                    return;
                }
                selectedItems = null;
                sendMouseMessage(GetClickedItemId(e.X, e.Y), e.X, e.Y, true);
            }
        }

        public static void Scrolling(object sender, MouseWheelEventArgs e) {
            if (e.Delta < 0) {
                if (Client.cZoomFactor < 3)
                    Client.cZoomFactor += .05f;
                Client.gameView.Zoom(1.03f);
            }
            else {
                if (Client.cZoomFactor > 0.6f)
                    Client.cZoomFactor -= .05f;
                Client.gameView.Zoom(0.97f);
            }
        }

        private static bool IsInMenu(float x) {
            return ((Client.cRenderWindow.Size.X * UserInterface.horizontalMenuSize) > x);
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
                Console.WriteLine("click x: " + e.X + ", y: " + e.Y);
                if (e.Button == Mouse.Button.Left) {
                    leftButtonClicking = true;
                    rightButtonClicked = false;
                    //a building is to be built
                    if (selectedItems != null && selectedItems.Length == 1) {
                        //send message to server for planting the building
                        Console.WriteLine("Planting building " + selectedItems[0] + " at " + e.X + ", " + e.Y);
                        sendPlantMessage(e.X, e.Y, selectedItems[0]);

                        //remove buildSelection
                        selectedItems[0] = NOTHING_SELECTED;
                        Client.cMouseSprite = null;
                    }
                    //nonplanting click; identify clicked item
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
                        else//no item identified, write to dragstartpoint for eventual rectangle
                            dragStartPoint = new Vector2f(e.X, e.Y);
                    }
                }
                //right mouse button
                else if (Mouse.IsButtonPressed(Mouse.Button.Right)) {
                    if(selectedItems == null) {
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
            //map mouse position to screen
            Vector2f convertedMousePosition = MapMouseToGame(x, y);
            //check for collisions
            FloatRect testRectangle = new FloatRect(convertedMousePosition.X, convertedMousePosition.Y, cChosenBuildingSize.X, cChosenBuildingSize.Y);
            foreach (CInterfaces.IDrawable i in Client.cItemList)
                if (i.BoundingRectangle.Intersects(testRectangle))
                    return;
            //create message
            NetOutgoingMessage mes = Communication.netClient.CreateMessage();
            //identify message as mouseclick
            mes.Write(CGlobal.PLANT_BUILDING_MESSAGE);
            //write type of building to init
            mes.Write(type);
            //write "put it there"
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
            //map mouse positions
            Vector2f mappedCoordinates = MapMouseToGame(x, y);
            x = mappedCoordinates.X;
            y = mappedCoordinates.Y;
            //create message
            NetOutgoingMessage mes = Communication.netClient.CreateMessage();
            //identify message as mouseclick
            mes.Write(CGlobal.MOUSE_CLICK_MESSAGE);
            //write id of clicked item
            mes.Write(ID);
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
            Vector2f mappedPosition = MapMouseToGame(e.X, e.Y);
            if (!IsInMenu(e.X) && selectedItems != null && selectedItems.Length == 1 && selectedItems[0] != -1) {
                cChosenBuildingSize = new Vector2f(CGlobal.BUILDING_TEXTURES[selectedItems[0]].Size.X, CGlobal.BUILDING_TEXTURES[selectedItems[0]].Size.Y);
                cChosenBuilding = new Sprite(CGlobal.BUILDING_TEXTURES[selectedItems[0]]);
                cChosenBuilding.Color = new Color(255, 255, 255, 120);
                cChosenBuilding.Position = mappedPosition;
                Client.cMouseSprite = cChosenBuilding;
                return;
            }
            else if (!IsInMenu(e.X)) {
                if (leftButtonClicking){
                    Client.dragRect.OutlineThickness = 2;
                    Client.dragRect.Position = MapMouseToGame(dragStartPoint.X, dragStartPoint.Y);
                    Client.dragRect.Size = new Vector2f(mappedPosition.X - Client.dragRect.Position.X, mappedPosition.Y - Client.dragRect.Position.Y);
                }
                else {
                    Client.dragRect.OutlineThickness = 0;
                }
            }
            else {
                //check whether we are hovering over an item
                Client.cInterface.Hover(e);
            }

        }

        private static Vector2f MapMouseToGame(float x, float y) {
            return Client.cRenderWindow.MapPixelToCoords(new Vector2i((int)x, (int)y));
        }
    }
}
