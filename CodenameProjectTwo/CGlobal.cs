﻿using SFML.Graphics;
using SFML.Window;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodenameProjectTwo
{
    static class CGlobal
    {
        /*
         * TILE TYPES VS THEIR ID
         */
        public const int
            EARTH_TILE = 0,
            SAND_TILE = 1,
            WATER_TILE = 2;
        public const int MOUSE_CLICK_MESSAGE = 0, STRING_MESSAGE = 1, GAMESTATE_BROADCAST = 2;
        /// <summary>
        /// DIS BE OF UTMOST IMPORTANCY. More readable in Global.cs.
        /// On each update and during the Initialization (e.g. dont worry about availability)
        /// this will get updated to contain the current offset of the view. 
        /// ALL POSITIONS _MUST_ BE RELATIVE TO THIS (except if you are sure you want to draw absolute to the window origin),
        /// meaning that if you for example want to position an enemy at top left of the currently seen view, you can not use
        /// position(0,0), but have to use position(CURRENT_WINDOW_ORIGIN.X, CURRENT_WINDOW_ORIGIN.Y) GRRRRRRRRR :X ONLY const UPPERCASE
        /// </summary>
        public static Vector2f CURRENT_WINDOW_ORIGIN;
        /// <summary>
        /// This stores the original window origin, you should be able to use CURRENT_WINDOW_ORIGIN instead to save on
        /// calculations
        /// </summary>
        public static Vector2f BEGIN_WINDOW_ORIGIN;

        /// <summary>
        /// Count of items buildable. Increase whenever you add one
        /// </summary>
        public static int ITEM_TYPE_COUNT=1;
        /// <summary>
        /// Add a description to your item
        /// </summary>
        public static string[] ITEM_DESCRIPTIONS = new string[] {"Schwertkämpfer, stark gegen irgendwas"};
        /// <summary>
        /// Save the texture of your item here
        /// </summary>
        public static Texture[] ITEM_TEXTURES = new Texture[ITEM_TYPE_COUNT];
    }
}
