using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SFML.Window;
using SFML.Graphics;
using Lidgren.Network;

namespace CodenameProjectTwo {
    class UserInterface {
        private RenderWindow win;
        private RectangleShape mainBox, textureBox, descriptionBox, infoBox;

        private List<MenuItem> buildableBuildings = new List<MenuItem>(), peopleMenuItemList = new List<MenuItem>();

        private Text descBoxText = new Text(), infoBoxText = new Text();
        private View menuView;
        private Font menuFont = new Font("assets/ui/ferrum.otf");
        private int maxColumns = 4;
        public static float IconSize;
        public static float horizontalMenuSize;

        private RectangleShape hoverBox;

        /// <summary>
        /// Value if no item has been clicked
        /// </summary>
        internal const int NO_ITEM_SELECTED = -1;

        /// <summary>
        /// Holds last clicked item id
        /// </summary>
        public int cItem = NO_ITEM_SELECTED;

        public UserInterface() {
            win = Client.cRenderWindow;

            //init view & viewport
            horizontalMenuSize = .2f;
            menuView = new View(new FloatRect(0, 0, (float)win.Size.X * horizontalMenuSize, win.Size.Y));
            menuView.Viewport = new FloatRect(0, 0, horizontalMenuSize, 1);


            //initialize background boxes
            mainBox = new RectangleShape(new Vector2f((float)win.Size.X / 5f, win.Size.Y));
            textureBox = new RectangleShape(new Vector2f(((float)mainBox.Size.X - 10f), ((float)mainBox.Size.Y - 20f) / 3f));
            descriptionBox = new RectangleShape(new Vector2f(((float)mainBox.Size.X - 10f), ((float)mainBox.Size.Y - 20f) / 3f));
            infoBox = new RectangleShape(new Vector2f(((float)mainBox.Size.X - 10f), ((float)mainBox.Size.Y - 20f) / 3f));

            //set requested iconsize as early as possible
            IconSize = (((float)mainBox.Size.X - 20f) / (float)maxColumns);
            
            //texts
            descBoxText.Color = Color.Black;
            descBoxText.Font = menuFont;
            infoBoxText.Color = Color.Black;
            infoBoxText.Font = menuFont;

            //box colors
            infoBox.FillColor = Color.Green;
            descriptionBox.FillColor = Color.Green;
            textureBox.FillColor = Color.Green;
            mainBox.FillColor = Color.Black;

            //move boxes and text
            SetBoxPositions();

            //load all sprites
            int y = 0, x = 0;
            /*
             * PEOPLE
             */
            for (int type = 0; type < CGlobal.PEOPLE_TYPE_COUNT; type++) {
                //create a grid
                if (x == maxColumns) {
                    x = 0;
                    y++;
                }
                MenuItem newMenuItem = new MenuItem(type, LoadGrantedBuildings(type), true, new Vector2f(textureBox.Position.X + (2f + IconSize) * x + 5f, textureBox.Position.Y + 10f + (10f + IconSize) * y));
                peopleMenuItemList.Add(newMenuItem);
                x++;
            }
            /*
             * BUILDINGS
             */
            y = 0;
            x = 0;
            for (int type = 0; type < CGlobal.BUILDING_TYPE_COUNT; type++) {
                if (!CGlobal.UNBUILDABLE_BUILDINGS.Contains(type)) {
                    //create a grid
                    if (x == maxColumns) {
                        x = 0;
                        y++;
                    }
                    MenuItem newMenuItem = new MenuItem(type, false, new Vector2f(infoBox.Position.X + (2f + IconSize) * x + 5f, infoBox.Position.Y + 10f + (10f + IconSize) * y));

                    buildableBuildings.Add(newMenuItem);
                    x++;
                }
            }
        }

        /// <summary>
        /// check each building - "who may i build" array for the people we 
        /// want to spawn; return a list of the buildings allowed to spawn
        /// peopleType people
        /// </summary>
        private List<int> LoadGrantedBuildings(int peopleType) {
            List<int> returnList = new List<int>();
            //iterate through building allowance list
            for (int currentBuilding = 0; currentBuilding < CGlobal.BUILDING_TYPE_COUNT; currentBuilding++) {
                //if the currentbuilding is allowed to spawn people of type peopleType, add them to the returnarray
                if (CGlobal.DISPLAYED_PEOPLE_PER_BUILDING[currentBuilding].Contains(peopleType + CGlobal.PEOPLE_ID_OFFSET))
                    returnList.Add(currentBuilding);
            }
            return returnList;
        }

        private void SetBoxPositions() {
            float margin = 5f;
            //mainbox position is exactly at top left corner
            mainBox.Position = new Vector2f(0, 0);
            //texturebox position is at top, but with a margin
            textureBox.Position = new Vector2f(mainBox.Position.X + margin, mainBox.Position.Y + margin);
            //desc box is below texturebox
            descriptionBox.Position = new Vector2f(mainBox.Position.X + margin, textureBox.Size.Y + textureBox.Position.Y + margin);
            //infobox is below that
            infoBox.Position = new Vector2f(mainBox.Position.X + margin, descriptionBox.Size.Y + descriptionBox.Position.Y + margin);
            //texts
            descBoxText.Position = new Vector2f(descriptionBox.Position.X + margin, descriptionBox.Position.Y + margin);
            infoBoxText.Position = new Vector2f(infoBox.Position.X + margin, infoBox.Position.Y + margin);
        }

        public void Update() {
            //move menu to current offset
            //MoveToCurrentPosition();
            /* tex=people
             * desc=desc
             * info=buildings
             */
            if (cItem == -1) {
                //desc text is only filled when hovering above a building
                descBoxText.DisplayedString = "Holz: " + Client.MyRessources.Wood + ", Stein: " + Client.MyRessources.Stone;
            }
            else {
                //item info string
                infoBoxText.DisplayedString = Client.cItemList[cItem].Name + '\n' +
                Client.cItemList[cItem].Health + "% Leben" + '\n' +
                "ID: " + cItem + "; Type: " + Client.cItemList[cItem].Type + "F: " + Client.cItemList[cItem].Faction;
            }
        }

        public void Draw() {
            win.SetView(menuView);
            //standard background boxes
            win.Draw(mainBox);
            win.Draw(textureBox);
            win.Draw(descriptionBox);
            win.Draw(infoBox);

            /*
             * Nothing is clicked, draw usual shit
             */
            if (cItem == NO_ITEM_SELECTED) {
                foreach (MenuItem m in buildableBuildings)
                    m.Draw();
                if (hoverBox != null)
                    win.Draw(hoverBox);
                win.Draw(descBoxText);
            }
            /*
             * a building is clicked, draw its allowed people
             */
            else {
                win.Draw(infoBoxText);
                if (Client.cItemList[cItem].Faction != Client.MyFaction)
                    return;
                foreach (MenuItem m in peopleMenuItemList)
                    if (m.BuildableBy.Contains(Client.cItemList[cItem].Type))
                        m.Draw();
                win.Draw(descBoxText);
            }
            //switch back to game view
            win.SetView(Client.gameView);
        }

        public void ShowItem(int itemID) {
            cItem = itemID;
            if (itemID != NO_ITEM_SELECTED) {
                //set desc texts
                if (Client.cItemList[itemID].Faction == Client.MyFaction)
                    descBoxText.DisplayedString = DivideString(Client.cItemList[itemID].Description);
                else
                    descBoxText.DisplayedString = "";
            }
        }

        /// <summary>
        /// Safety call to divide all x characters
        /// </summary>
        private string DivideString(string input) {
            int maximumLength = 22;
            if (input.Length < maximumLength)
                return input;
            int resultingStringCount = input.Length / maximumLength + 1;
            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < resultingStringCount; i++) {
                int start = i * (maximumLength);
                if (start + maximumLength > input.Length)
                    maximumLength = input.Length - start;
                sb.AppendLine(input.Substring(start, maximumLength).Trim(new char[] { ' ' }));
            }
            return sb.ToString();
        }


        internal void Hover(MouseMoveEventArgs e) {
            bool hovering = false;
            foreach (MenuItem m in buildableBuildings) {
                if (m.Sprite.GetGlobalBounds().Contains(e.X, e.Y)) {
                    hovering = true;
                    descBoxText.DisplayedString = DivideString(m.Description + '\n' + "Holzkosten: " + CGlobal.BUILDING_COSTS_WOOD[m.Type] + '\n' + "Steinkosten: " + CGlobal.BUILDING_COSTS_STONE[m.Type]);
                    if (HasRessources(m.Type))
                        hoverBox = m.GreenBoundingShape;
                    else
                        hoverBox = m.RedBoundingShape;
                }
            }
            foreach (MenuItem m in peopleMenuItemList) {
                if (m.Sprite.GetGlobalBounds().Contains(e.X, e.Y)) {
                    hovering = true;
                    if (HasRessources(m.Type))
                        hoverBox = m.GreenBoundingShape;
                    else
                        hoverBox = m.RedBoundingShape;
                    descBoxText.DisplayedString = DivideString(m.Description);
                }
            }
            if (!hovering)
                hoverBox = null;
        }

        /// <summary>
        /// Handle clicks in the menu, according to whether a building is selected or not
        /// </summary>
        internal void Click(int X, int Y) {
            //only check for planting buildings if there is currently no building selected
            if (cItem == NO_ITEM_SELECTED){
                foreach (MenuItem m in buildableBuildings) {
                    if (m.Sprite.GetGlobalBounds().Contains(X, Y)) {
                        if (HasRessources(m.Type)) {
                            Console.WriteLine("building " + m.Type + " activated");
                            if (MouseHandling.selectedItems == null)
                                MouseHandling.selectedItems = new int[1];
                            MouseHandling.selectedItems[0] = m.Type;
                        }
                    }
                }
            }
            //spawn people
            else {
                foreach (MenuItem m in peopleMenuItemList) {
                    if (m.Sprite.GetGlobalBounds().Contains(X, Y)) {
                        Console.WriteLine("ppl ressource check: type "+ m.Type);
                        if (HasRessources(m.Type + CGlobal.PEOPLE_ID_OFFSET)){
                            Console.WriteLine("spawn people type " + m.Type);
                            sendPeoplePlantMessage(m.Type + CGlobal.PEOPLE_ID_OFFSET, cItem);
                        }
                    }
                }
            }
        }

        private static void sendPeoplePlantMessage(Int32 spawnPeople, Int32 itemSpawnerID) {
            //create message
            NetOutgoingMessage mes = Communication.netClient.CreateMessage();
            //identify message as mouseclick
            mes.Write(CGlobal.SPAWN_PEOPLE_MESSAGE);
            //write type of people to init
            mes.Write(spawnPeople);
            //write id of building to plant next to
            mes.Write(itemSpawnerID);
            //send
            Communication.netClient.SendMessage(mes, NetDeliveryMethod.ReliableOrdered);
            //like, really, send now
            Communication.netClient.FlushSendQueue();
        }

        private bool HasRessources(int itemType) {
            bool returnValue = true;
            if (itemType < CGlobal.PEOPLE_ID_OFFSET) {
                if (Client.MyRessources.Wood < CGlobal.BUILDING_COSTS_WOOD[itemType])
                    returnValue = false;
                if (Client.MyRessources.Stone < CGlobal.BUILDING_COSTS_STONE[itemType])
                    returnValue = false;
            }
            else {
                if (Client.MyRessources.Wood < CGlobal.PEOPLE_COSTS_WOOD[itemType - CGlobal.PEOPLE_ID_OFFSET])
                    returnValue = false;
                if (Client.MyRessources.Stone < CGlobal.PEOPLE_COSTS_STONE[itemType - CGlobal.PEOPLE_ID_OFFSET])
                    returnValue = false;
            }
            return returnValue;
        }
    }
}
