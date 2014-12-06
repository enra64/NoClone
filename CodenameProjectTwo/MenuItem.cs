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

        /// <summary>
        /// This MenuItem is only accessable from buildings whose id is contained in BuildableBy
        /// </summary>
        public MenuItem(Sprite _sprite, string _description, int _type, List<int> _buildableBy) {
            Sprite = _sprite;
            Description = _description;
            Type = _type;
            BuildableBy = _buildableBy;
        }

        /// <summary>
        /// Create a menu item accessable everywhere
        /// </summary>
        public MenuItem(Sprite _sprite, string _description, int _type) {
            Sprite = _sprite;
            Description = _description;
            Type = _type;
        }

        public void Draw() {
            Client.cRenderWindow.Draw(Sprite);
        }
    }
}
