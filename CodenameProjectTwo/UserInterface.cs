using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SFML.Window;
using SFML.Graphics;

namespace CodenameProjectTwo {
    class UserInterface {
        private RenderWindow win;
        private RectangleShape mainBox, textureBox, descriptionBox, infoBox;
        private List<Sprite> allPeopleSpriteList = new List<Sprite>(), allBuildingSpriteList = new List<Sprite>();

        //new dogma: use class
        private List<MenuItem> buildableBuildings = new List<MenuItem>(), peopleMenuItemList = new List<MenuItem>();

        private Text descriptionText = new Text(), infoText = new Text();
        private View menuView;
        private Font menuFont = new Font("assets/ui/ferrum.otf");
        private int maxColumns = 4;
        private float ItemIconSize;
        public float menuPortScale { get; private set; }

        public int cItem = -1;

        public UserInterface() {
            win = Client.cRenderWindow;

            //init view & viewport
            menuPortScale = .2f;
            menuView = new View(new FloatRect(0, 0, (float)win.Size.X * menuPortScale, win.Size.Y));
            menuView.Viewport = new FloatRect(0, 0, menuPortScale, 1);


            //initialize background boxes
            mainBox = new RectangleShape(new Vector2f((float)win.Size.X / 5f, win.Size.Y));
            textureBox = new RectangleShape(new Vector2f(((float)mainBox.Size.X - 10f), ((float)mainBox.Size.Y - 20f) / 3f));
            descriptionBox = new RectangleShape(new Vector2f(((float)mainBox.Size.X - 10f), ((float)mainBox.Size.Y - 20f) / 3f));
            infoBox = new RectangleShape(new Vector2f(((float)mainBox.Size.X - 10f), ((float)mainBox.Size.Y - 20f) / 3f));

            //texts
            descriptionText.DisplayedString = DivideAll("A description would be awesome");
            descriptionText.Color = Color.Black;
            descriptionText.Font = menuFont;
            infoText.DisplayedString = DivideAll("Infos here would be awesome");
            infoText.Color = Color.Black;

            //box colors
            infoBox.FillColor = Color.Green;
            descriptionBox.FillColor = Color.Green;
            textureBox.FillColor = Color.Green;
            mainBox.FillColor = Color.Black;

            //move boxes and text
            MoveToCurrentPosition();

            //load all sprites
            int y = 0, x = 0;
            ItemIconSize = (((float)mainBox.Size.X - 20f) / (float)maxColumns);
            //people
            for (int i = 0; i < CGlobal.PEOPLE_TYPE_COUNT; i++) {
                //create a grid
                if (x == maxColumns) {
                    x = 0;
                    y++;
                }
                //create the menuitems sprite
                float scale = ItemIconSize / (float)CGlobal.PEOPLE_TEXTURES[i].Size.X;
                Sprite newSprite = new Sprite(CGlobal.PEOPLE_TEXTURES[i]);
                newSprite.Scale = new Vector2f(scale, scale);
                newSprite.Position = new Vector2f(textureBox.Position.X + (2f + ItemIconSize) * x + 5f, textureBox.Position.Y + 10f + (10f + ItemIconSize) * y);

                MenuItem newMenuItem=new MenuItem(newSprite, CGlobal.PEOPLE_DESCRIPTIONS[i], i, LoadGrantedBuildings(i));

                peopleMenuItemList.Add(newMenuItem);
                x++;
            }
            //buildings
            y = 0;
            x = 0;
            for (int i = 0; i < CGlobal.BUILDING_TYPE_COUNT; i++) {
                if (!CGlobal.UNBUILDABLE_BUILDINGS.Contains(i)) { 
                    //create a grid
                    if (x == maxColumns) {
                        x = 0;
                        y++;
                    }
                    float scale = ItemIconSize / (float)CGlobal.BUILDING_TEXTURES[i].Size.X;
                    Sprite newSprite = new Sprite(CGlobal.BUILDING_TEXTURES[i]);
                    newSprite.Scale = new Vector2f(scale, scale);
                    newSprite.Position = new Vector2f(infoBox.Position.X + (2f + ItemIconSize) * x + 5f, infoBox.Position.Y + 10f + (10f + ItemIconSize) * y);
                    
                    MenuItem newMenuItem=new MenuItem(newSprite, CGlobal.BUILDING_DESCRIPTIONS[i], i);

                    buildableBuildings.Add(newMenuItem);
                    x++;
                }
            }
        }

        /// <summary>
        /// check each building - "who may i build" array for the people we want to spawn; return a list of the buildings allowed to spawn
        /// peopleType people
        /// </summary>
        /// <param name="peopleType"></param>
        /// <returns></returns>
        private List<int> LoadGrantedBuildings(int peopleType) {
            List<int> returnList = new List<int>();
            //iterate through building allowance list
            for(int currentBuilding=0;currentBuilding<CGlobal.BUILDING_TYPE_COUNT; currentBuilding++){
                int[] checkArray=CGlobal.DISPLAYED_PEOPLE_PER_BUILDING[currentBuilding];
                //if the currentbuilding is allowed to spawn people of type peopleType, add them to the returnarray
                if(checkArray.Contains(peopleType))
                    returnList.Add(currentBuilding);
            }
            return returnList;
        }

        private void MoveToCurrentPosition() {
            //mainbox position is exactly at top left corner
            mainBox.Position = new Vector2f(0, 0);

            //texturebox position is at top, but with a margin
            textureBox.Position = new Vector2f(mainBox.Position.X + 5f, mainBox.Position.Y + 5f);

            //desc box is below texturebox
            descriptionBox.Position = new Vector2f(mainBox.Position.X + 5f, textureBox.Size.Y + textureBox.Position.Y + 5f);

            //infobox is below that
            infoBox.Position = new Vector2f(mainBox.Position.X + 5f, descriptionBox.Size.Y + descriptionBox.Position.Y + 5f);

            //texts
            descriptionText.Position = new Vector2f(descriptionBox.Position.X + 5f, descriptionBox.Position.Y + 5f);
            infoText.Position = new Vector2f(infoBox.Position.X + 5f, infoBox.Position.Y + 5f);

            //move sprites to correct positions
        }

        public void Update() {
            //move menu to current offset
            //MoveToCurrentPosition();
            /* tex=people
             * desc=desc
             * info=buildings
             */
            if (cItem == -1) {

            }
        }

        private List<Sprite> ShowThesePeople(int[] buildingPosition) {
            List<Sprite> returnList = new List<Sprite>();
            int x = 0, y = 0;
            foreach (int wantedIndex in buildingPosition) {
                //create a grid
                if (x == maxColumns) {
                    x = 0;
                    y++;
                }
                Sprite addSprite = allPeopleSpriteList[wantedIndex];
                addSprite.Position = new Vector2f(infoBox.Position.X + (2f + ItemIconSize) * x + 5f, infoBox.Position.Y + 10f + (10f + ItemIconSize) * y);
                returnList.Add(addSprite);
                x++;
            }
            return returnList;
        }

        public void Draw() {
            win.SetView(menuView);
            //standard background boxes
            win.Draw(mainBox);
            win.Draw(textureBox);
            win.Draw(descriptionBox);
            win.Draw(infoBox);

            if (cItem == -1) {
                //show building menu
                foreach (MenuItem m in buildableBuildings)
                    m.Draw();
                win.Draw(descriptionText);
            }
            else{
                foreach (MenuItem m in peopleMenuItemList)
                    if (m.BuildableBy.Contains(Client.cItemList[cItem].Type))
                        m.Draw();
                win.Draw(descriptionText);
                win.Draw(infoText);
            }
            win.SetView(Client.cView);
        }

        public void ShowItem(int itemID) {
            cItem = itemID;
            if (itemID != -1) {
                //set info texts
                descriptionText.DisplayedString = DivideAll(Client.cItemList[itemID].Description);
                //create info string
                //health, id, type
                infoText.DisplayedString = Client.cItemList[itemID].Name + "\n" +
                    Client.cItemList[itemID].Health + "% Leben";
            }
        }

        /// <summary>
        /// Safety call to divide all x characters
        /// </summary>
        private string DivideAll(string input) {
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


        internal void CheckHover(MouseMoveEventArgs e) {
            MenuItem m;
            for (int i = 0; i < buildableBuildings.Count; i++) {
                m = buildableBuildings[i];
                if (m.Sprite.GetGlobalBounds().Contains(e.X, e.Y)) {
                    descriptionText.DisplayedString = DivideAll(m.Description);
                }
            }
            for (int i = 0; i < peopleMenuItemList.Count; i++) {
                m = peopleMenuItemList[i];
                if (m.Sprite.GetGlobalBounds().Contains(e.X, e.Y)) {
                    descriptionText.DisplayedString = DivideAll(CGlobal.PEOPLE_DESCRIPTIONS[i]);
                }
            }
        }

        internal void Click(int X, int Y) {
            MenuItem m;
            //only check for planting buildings if there is currently no building selected
            if (cItem == -1) {
                foreach (MenuItem mi in buildableBuildings){
                    if (mi.Sprite.GetGlobalBounds().Contains(X, Y)) {
                        Console.WriteLine("building " + mi.Type + " activated");
                        MouseHandling.buildingChosen = mi.Type;
                    }
                }
            }
            //spawn people
            else {
                for (int i = 0; i < peopleMenuItemList.Count; i++) {
                    m = peopleMenuItemList[i];
                    if (m.Sprite.GetGlobalBounds().Contains(X, Y)) {
                        Console.WriteLine("spawn people type " + i);
                    }
                }
            }
        }
    }
}
