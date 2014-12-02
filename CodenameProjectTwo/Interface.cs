using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SFML.Window;
using SFML.Graphics;

namespace CodenameProjectTwo
{
     class Interface
    {
         private RenderWindow win;
         private RectangleShape mainBox, textureBox, descriptionBox, infoBox;
         private Text descriptionText = new Text(), infoText=new Text();
         private View menuView;
         public float menuPortScale { get; private set; }
         
         private int cItem=-1;

        public Interface(){
            win = Client.cRenderWindow;

            menuPortScale = .2f;
            menuView = new View(new FloatRect(0, 0, (float) win.Size.X * menuPortScale, win.Size.Y));
            menuView.Viewport = new FloatRect(0, 0, menuPortScale, 1);

            mainBox = new RectangleShape(new Vector2f((float)win.Size.X / 5f, win.Size.Y));
            textureBox = new RectangleShape(new Vector2f(((float)mainBox.Size.X - 10f), ((float)mainBox.Size.X - 10f)));
            descriptionBox = new RectangleShape(new Vector2f(((float)mainBox.Size.X - 10f), ((float)mainBox.Size.X - 110f)));
            infoBox = new RectangleShape(new Vector2f(((float)mainBox.Size.X - 10f), ((float)mainBox.Size.X - 110f)));

            //texts
            descriptionText.DisplayedString = DivideAll("A description would be awesome", 20);
            descriptionText.Color = Color.Black;
            infoText.DisplayedString = DivideAll("Infos here would be awesome", 20);
            infoText.Color = Color.Black;

            //box colors
            infoBox.FillColor = Color.Green;
            descriptionBox.FillColor = Color.Green;
            textureBox.FillColor = Color.Green;
            mainBox.FillColor = Color.Black;

            //boxes
            MoveToCurrentPosition();
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
        }

        public  void Update(){
            //move menu to current offset
            //MoveToCurrentPosition();
        }

        public  void Draw(){
            win.SetView(menuView);
            if (cItem != -1)
            {
                //show building menu
            }
            else
            {
                win.Draw(mainBox);
                win.Draw(textureBox);
                win.Draw(descriptionBox);
                win.Draw(infoBox);

                win.Draw(descriptionText);
                win.Draw(infoText);
            }
            win.SetView(Client.cView);
        }

        public  void ShowItem(int itemID)
        {
            cItem = itemID;
            if (itemID == -1)
            {
                //show building menu
            }
            else
            {
                //set info texts
                descriptionText.DisplayedString = DivideAll(Client.cItemList[itemID].Description, 20);
                //create info string
                //health, id, type
                infoText.DisplayedString = Client.cItemList[itemID].Name+"\n"+
                    Client.cItemList[itemID].Health/Client.cItemList[itemID].MaxHealth+"% Leben";
            }
        }

        /// <summary>
        /// Safety call to divide all x characters
        /// </summary>
        private  string DivideAll(string input, int maximumLength)
        {
            if (input.Length < maximumLength)
                return input;
            var list = Enumerable
            .Range(0, input.Length / maximumLength)
            .Select(i => input.Substring(i * maximumLength, maximumLength));
            return string.Join("\n", list);
        }

        internal void CheckHover(MouseMoveEventArgs e)
        {
            //throw new NotImplementedException();
        }
    }
}
