using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SFML.Window;
using SFML.Graphics;

namespace CodenameProjectTwo
{
     class UserInterface
    {
         private RenderWindow win;
         private RectangleShape mainBox, textureBox, descriptionBox, infoBox;
         private List<Sprite> peopleSpriteList=new List<Sprite>(), buildingSpriteList=new List<Sprite>();
         private Text descriptionText = new Text(), infoText=new Text();
         private View menuView;
         private Font menuFont = new Font("assets/ui/ferrum.otf");
         private int maxColumns=4;
         public float menuPortScale { get; private set; }
         
         public int cItem=-1;

        public UserInterface(){
            win = Client.cRenderWindow;

            //init view & viewport
            menuPortScale = .2f;
            menuView = new View(new FloatRect(0, 0, (float) win.Size.X * menuPortScale, win.Size.Y));
            menuView.Viewport = new FloatRect(0, 0, menuPortScale, 1);


            //initialize background boxes
            mainBox = new RectangleShape(new Vector2f((float)win.Size.X / 5f, win.Size.Y));
            textureBox = new RectangleShape(new Vector2f(((float)mainBox.Size.X - 10f), ((float)mainBox.Size.Y - 20f) / 3f));
            descriptionBox = new RectangleShape(new Vector2f(((float)mainBox.Size.X - 10f), ((float)mainBox.Size.Y - 20f) / 3f));
            infoBox = new RectangleShape(new Vector2f(((float)mainBox.Size.X - 10f), ((float)mainBox.Size.Y - 20f) / 3f));

            //texts
            descriptionText.DisplayedString = DivideAll("A description would be awesome");
            descriptionText.Color = Color.Black;
            descriptionText.Font=menuFont;
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
            float xSize = (((float)mainBox.Size.X - 20f) / (float)maxColumns);
            for (int i = 0; i < CGlobal.PEOPLE_TYPE_COUNT; i++ ){
                //create a grid
                if (x == 2){
                    x = 0;
                    y++;
                }
                float scale = xSize / CGlobal.BUILDING_TEXTURES[i].Size.X;
                Sprite newSprite = new Sprite(CGlobal.BUILDING_TEXTURES[i]);
                newSprite.Scale = new Vector2f(scale, scale);
                newSprite.Position = new Vector2f(textureBox.Position.X + xSize * x + 5f+ i*(75f), textureBox.Position.Y + (5f + xSize) * y);
                peopleSpriteList.Add(newSprite);
            }
            y = 0;
            x = 0;
            for (int i = 0; i < CGlobal.BUILDING_TYPE_COUNT; i++){
                //create a grid
                if (x == 2){
                    x = 0;
                    y++;
                }
                float scale = xSize / CGlobal.BUILDING_TEXTURES[i].Size.X;
                Sprite newSprite = new Sprite(CGlobal.BUILDING_TEXTURES[i]);
                newSprite.Scale = new Vector2f(scale, scale);
                newSprite.Position = new Vector2f(infoBox.Position.X + xSize * x + 5f+i*(75f), infoBox.Position.Y + (5f + xSize) * y);
                buildingSpriteList.Add(newSprite);
            }
        }

        private  void MoveToCurrentPosition(){
            //mainbox position is exactly at top left corner
            mainBox.Position = new Vector2f(0,0);

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

        public  void Update(){
            //move menu to current offset
            //MoveToCurrentPosition();
            /*
             * tex=people
             * desc=desc
             * info=buildings
             */
            if (cItem == -1)
            {

            }
        }

        public void Draw(){
            win.SetView(menuView);
            //standard background boxes
            win.Draw(mainBox);
            win.Draw(textureBox);
            win.Draw(descriptionBox);
            win.Draw(infoBox);

            if (cItem == -1){
                //show building menu
                foreach (Sprite s in buildingSpriteList)
                    win.Draw(s);
                foreach (Sprite s in peopleSpriteList)
                    win.Draw(s);
                win.Draw(descriptionText);
            }
            else
            {
                win.Draw(descriptionText);
                win.Draw(infoText);

            }
            win.SetView(Client.cView);
        }

        public void ShowItem(int itemID){
            cItem = itemID;
            if (itemID != -1){
                //set info texts
                descriptionText.DisplayedString = DivideAll(Client.cItemList[itemID].Description);
                //create info string
                //health, id, type
                infoText.DisplayedString = Client.cItemList[itemID].Name+"\n"+
                    Client.cItemList[itemID].Health+"% Leben";
            }
        }

        /// <summary>
        /// Safety call to divide all x characters
        /// </summary>
        private string DivideAll(string input){
            int maximumLength = 22;
            if (input.Length < maximumLength)
                return input;
            int resultingStringCount=input.Length/maximumLength + 1;
            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < resultingStringCount; i++){
                int start=i * (maximumLength - 1);
                if(start+maximumLength>input.Length)
                    maximumLength=input.Length-start;
                sb.AppendLine(input.Substring(start, maximumLength));
            }
            return sb.ToString();
        }

        internal void CheckHover(MouseMoveEventArgs e)
        {
            Sprite s;
            for(int i=0;i<buildingSpriteList.Count;i++){
                s=buildingSpriteList[i];
                if (s.GetGlobalBounds().Contains(e.X, e.Y))
                {
                    descriptionText.DisplayedString = DivideAll(CGlobal.BUILDING_DESCRIPTIONS[i]);
                }
            }
            for (int i = 0; i < peopleSpriteList.Count; i++){
                s = peopleSpriteList[i];
                if (s.GetGlobalBounds().Contains(e.X, e.Y))
                {
                    descriptionText.DisplayedString = DivideAll(CGlobal.PEOPLE_DESCRIPTIONS[i]);
                }
            }
        }

        internal void Click(int X, int Y)
        {
            Sprite s;
            for (int i = 0; i < buildingSpriteList.Count; i++){
                s = buildingSpriteList[i];
                if (s.GetGlobalBounds().Contains(X, Y)){
                    Console.WriteLine("building " + i + " activated");
                    MouseHandling.buildingChosen = i;
                }
            }
            for (int i = 0; i < peopleSpriteList.Count; i++){
                s = peopleSpriteList[i];
                if (s.GetGlobalBounds().Contains(X, Y)){
                    //typ i muss gespawnt werden
                }
            }
        }
    }
}
