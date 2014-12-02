using System;

using SFML;
using SFML.Graphics;
using SFML.Window;

using CodenameProjectTwo;

namespace CodenameProjectTwo.buildings
{
    class VillageCentre : CInterfaces.IDrawable
    {
        private RenderWindow win;
        
        private Sprite hqr;
        private Sprite hqb;

        public void VillageHQ(RenderWindow _w, String color)
        {
            win = _w;
            if (color == "red")
            {
                Texture hqtexr = new Texture("assets/graphics/buildings/hqred.png");
                hqr = new Sprite(hqtexr);
            }

            else
            {
                Texture hqtexb = new Texture("assets/graphics/buildings/hqblue.png");
                hqb = new Sprite(hqtexb);
            }
        }

        public void Update()
        {
            
            Vector2f hqrPosition = new Vector2f(0f, 0f);
            Vector2f hqbPosition = new Vector2f(100f, 100f);
            hqr.Position = hqrPosition;
            hqb.Position = hqbPosition;
        }

        public void Draw()
        {
            win.Draw(hqr);
            win.Draw(hqb);
            win.Display();
        }
    }
}
