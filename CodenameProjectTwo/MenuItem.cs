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
        public MenuItem(Sprite _sprite, string _description, int _type, List<int> _buildableBy) {
            Sprite = _sprite;
            boundingShape = new RectangleShape(new Vector2f((float)Sprite.Texture.Size.X * Sprite.Scale.X, 
                (float) Sprite.Texture.Size.Y * Sprite.Scale.Y));
            boundingShape.FillColor = Color.Transparent;
            boundingShape.OutlineThickness = 1;
            boundingShape.Position = Sprite.Position;

            Description = _description;
            Type = _type;
            BuildableBy = _buildableBy;
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

        /// <summary>
        /// Create a menu item accessable everywhere
        /// </summary>
        public MenuItem(Sprite _sprite, string _description, int _type) {
            Sprite = _sprite;

            boundingShape = new RectangleShape(new Vector2f((float)Sprite.Texture.Size.X * Sprite.Scale.X,
                (float)Sprite.Texture.Size.Y * Sprite.Scale.Y));
            boundingShape.FillColor = Color.Transparent;
            boundingShape.OutlineThickness = 1;
            boundingShape.Position = Sprite.Position;

            Description = _description;
            Type = _type;
        }

        public void Draw() {
            Client.cRenderWindow.Draw(Sprite);
        }
    }
}
