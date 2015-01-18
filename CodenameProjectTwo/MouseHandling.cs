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
        public static RectangleShape b;

        public static void mouseRelease(object sender, MouseButtonEventArgs e){
            if(!rightButtonClicked){
                leftButtonClicking = false;
                if(Client.dragRect.OutlineThickness > 0){
                    //undraw dragrect
                    Client.dragRect.OutlineThickness = 0;
                    //check what has been clicked
                    Vector2f translatedPosition = Client.dragRect.Position;
                    Vector2f translatedSize = Client.dragRect.Size;
                    //invert on negative size values because sfml shits on how to do math
                    if (translatedSize.X < 0) {
                        translatedSize.X = -translatedSize.X;
                        translatedPosition.X -= translatedSize.X;
                    }
                    if (translatedSize.Y < 0) {
                        translatedSize.Y = -translatedSize.Y;
                        translatedPosition.Y -= translatedSize.Y;
                    }
                    FloatRect selectionRect = new FloatRect(
                        translatedPosition.X,
                        translatedPosition.Y,
                        translatedSize.X,
                        translatedSize.Y);
                    b = new RectangleShape(new Vector2f(selectionRect.Width, selectionRect.Height));
                    b.Position = new Vector2f(selectionRect.Left, selectionRect.Top);

                    List<int> selectedPeopleTypes=new List<int>();
                    List<int> selectedPeopleIDs = new List<int>();
                    foreach (AbstractClientItem a in Client.cItemList) {
                        if (a.Type >= 100) {
                            //Console.WriteLine(a.ID+"left: " + a.Sprite.Position.X + ", top: " + a.Sprite.Position.Y);
                            if(selectionRect.Contains(a.Center.X, a.Center.Y)){
                                selectedPeopleIDs.Add(a.ID);
                            }
                        }
                    }
                    //send all selected to server...
                    List<int> selected = new List<int>();
                    foreach (int i in selectedPeopleIDs)
                        if(Client.cItemList[i] != null && /*Client.cItemList[i].Type == mostSelected &&*/ Client.cItemList[i].Health > 0)
                            selected.Add(i);
                    Communication.sendMassSelection(selected);
                    Console.WriteLine(", count: " + selectedPeopleTypes.Count);
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
            Console.Write("mouseclick");
            //decide whether menu or game screen
            //menu
            if (IsInMenu(e.X)) {
                if (e.Button == Mouse.Button.Right)
                    return;
                Client.cInterface.Click(e.X, e.Y);
                Console.WriteLine("Menu click");
            }
            //game view
            else {
                Console.Write(" at x: " + e.X + ", y: " + e.Y);//already designated as mouseclick
                if (e.Button == Mouse.Button.Left){
                    Console.Write(" left ");
                    leftButtonClicking = true;
                    rightButtonClicked = false;
                    //a building is to be built
                    if (selectedItems != null && selectedItems.Length == 1 && selectedItems[0] != -1) {
                        //send message to server for planting the building
                        Console.WriteLine("Planting building " + selectedItems[0] + " at " + e.X + ", " + e.Y);
                        sendBuildingPlantMessage(e.X, e.Y, selectedItems[0]);

                        //remove buildSelection
                        selectedItems[0] = NOTHING_SELECTED;
                        Client.cMouseSprite = null;
                    }
                    //nonplanting click; identify clicked item
                    else{
                        //check what we clicked
                        Int32 clickedItemId = GetClickedItemId(e.X, e.Y);
                        //inform ui
                        Client.cInterface.ShowItem(clickedItemId);
                        //if an item was identified, send that to the server
                        Console.WriteLine("nonplanting click on item " + clickedItemId);
                        sendMouseMessage(clickedItemId, e.X, e.Y, false);
                        dragStartPoint = new Vector2f(e.X, e.Y);
                    }
                }
                //right mouse button
                else if (Mouse.IsButtonPressed(Mouse.Button.Right)) {
                    Console.Write(" right ");
                    if (selectedItems == null || true) {
                        Console.Write(" selectedItems!=null ");
                        rightButtonClicked = true;
                        leftButtonClicking = false;
                        mouseMovementStartingPoint = new Vector2f(e.X, e.Y);
                    }
                    else
                        Console.Write("godfuckindammit");//as soon as we build a barrack we always have
                    //selecteditems == null
                }
            }
        }

        private static Int32 GetClickedItemId(float x, float y) {
            Vector2f mappedCoordinates = MapMouseToGame(x, y);
            x = mappedCoordinates.X;
            y = mappedCoordinates.Y;
            //return the clicked item id if found, -1 otherwise
            foreach (AbstractClientItem item in Client.cItemList)
                if (item.Type >= 100 && item.BoundingRectangle.Contains(x, y) && item.Health > 0)
                    return item.ID;

            foreach (AbstractClientItem item in Client.cItemList)
                if (item.Type < 100 && item.BoundingRectangle.Contains(x, y) && item.Health > 0)
                    return item.ID;
            return -1;
        }

        private static void sendBuildingPlantMessage(float x, float y, Int32 type) {
            //map mouse position to screen
            Vector2f convertedMousePosition = MapMouseToGame(x, y);
            //check for collisions
            FloatRect testingRectangle = new FloatRect(convertedMousePosition.X, convertedMousePosition.Y, 
                cChosenBuildingSize.X, cChosenBuildingSize.Y);
            foreach (AbstractClientItem i in Client.cItemList)
                if (i.BoundingRectangle.Intersects(testingRectangle))
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
