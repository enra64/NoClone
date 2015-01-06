using SFML.Graphics;
using SFML.Window;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodenameProjectTwo {
    class MenuItem {
        public Sprite Sprite { get; set; }
        public String Description { get; set; }
        public int Type { get; set; }
        public List<int> BuildableBy { get; set; }

        private RectangleShape boundingShape;
        /// <summary>
        /// This MenuItem is only accessable from buildings whose id is contained in BuildableBy
        /// </summary>
        public MenuItem(int _type, List<int> _buildableBy, bool isPeople, Vector2f _position) {
            CommonConstructor(_type, isPeople, _position);
            BuildableBy = _buildableBy;
        }

        /// <summary>
        /// Create a menu item accessable everywhere
        /// </summary>
        public MenuItem(int _type, bool isPeople, Vector2f _position) {
            CommonConstructor(_type, isPeople, _position);
        }

        private void CommonConstructor(int _type, bool _isPeople, Vector2f _position) {
            float IconSize = UserInterface.IconSize;
            //create the menuitems sprite
            float scale;
            Sprite newSprite;
            if (_isPeople) {//if the id is >99, we are dealing with people.
                scale = IconSize / (float)CGlobal.PEOPLE_TEXTURES[_type].Size.X;
                newSprite = new Sprite(CGlobal.PEOPLE_TEXTURES[_type]);
                Description = CGlobal.PEOPLE_DESCRIPTIONS[_type];
            }
            //buildings
            else {
                scale = IconSize / (float)CGlobal.BUILDING_TEXTURES[_type].Size.X;
                newSprite = new Sprite(CGlobal.BUILDING_TEXTURES[_type]);
                Description = CGlobal.BUILDING_DESCRIPTIONS[_type];
            }
            newSprite.Scale = new Vector2f(scale, scale);
            newSprite.Position = _position;

            //calculate bounding shape for hover indication
            boundingShape = new RectangleShape(new Vector2f((float)newSprite.Texture.Size.X * newSprite.Scale.X,
                (float)newSprite.Texture.Size.Y * newSprite.Scale.Y));
            boundingShape.FillColor = Color.Transparent;
            boundingShape.OutlineThickness = 1;
            boundingShape.Position = newSprite.Position;
            //set type, sprite
            Type = _type;
            Sprite = newSprite;
        }

        public RectangleShape GreenBoundingShape{
            get{
                this.boundingShape.OutlineColor = Color.Blue;
                return boundingShape;
            }
        }

        public RectangleShape RedBoundingShape {
            get {
                this.boundingShape.OutlineColor = Color.Red;
                return boundingShape;
            }
        }

        public void Draw() {
            Client.cRenderWindow.Draw(Sprite);
        }
    }
}
