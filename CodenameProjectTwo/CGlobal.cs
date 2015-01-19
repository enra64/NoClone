using SFML.Graphics;
using SFML.Window;
using System;

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
        public const int 
            MOUSE_CLICK_MESSAGE = 0, 
            STRING_MESSAGE = 1, 
            GAMESTATE_BROADCAST = 2, 
            PLANT_BUILDING_MESSAGE=3, 
            CLIENT_IDENTIFICATION_MESSAGE=4,
            BOUNDINGSIZE_MESSAGE = 5,
            SPAWN_PEOPLE_MESSAGE = 6,
            MASS_SELECTION_MESSAGE = 7;

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
        /// Count of items buildable and ressources. Increase whenever you add one
        /// </summary>
        public const int BUILDING_TYPE_COUNT = 7;
        /// <summary>
        /// building types. should be unique to the building, and is used to find the correct description and texture
        /// </summary>

        public const int
            BUILDING_DEFAULT = 0,
            BUILDING_BLUE = 1,
            BUILDING_RED = 2,
            BUILDING_BARRACK = 3,
            RESSOURCE_STONE = 4,
            RESSOURCE_WOOD = 5,
            BUILDING_STONEHACKER = 6,
            PEOPLE_PEASANT = 100,
            PEOPLE_SWORDMAN = 101,
            PEOPLE_STONEMAN = 102;
        
        //building costs: wood
        public static int[] BUILDING_COSTS_WOOD = { 200, 0, 0, 100, 0, 0, 400 };
        //building costs: stone
        public static int[] BUILDING_COSTS_STONE = { 0, 0, 0, 300, 0, 0, 0 };
        /// <summary>
        /// Array of buildings users are not allowed to build
        /// </summary>
        public static int[] UNBUILDABLE_BUILDINGS = { 1, 2, 4, 5 };
        /// <summary>
        /// Add a description to your item
        /// </summary>
        public static String[] BUILDING_DESCRIPTIONS = new string[] { 
            "I am the default Building", 
            "You are the proud leader of the Red Army! You have to fight against the blue oppressors!", 
            "You are the proud leader of the Blue Armada! You have to fight against the red conquerors!", 
            "Soldiers are getting their basic \"Training\" here!", 
            "Stone! This stuff is everywhere!", 
            "A Tree! Maybe someday this one will march against Isengart...",
            "The stone people live here! Nobody knows where they come from!"};
        /// <summary>
        /// Save the texture of your item here
        /// </summary>
        public static Texture[] BUILDING_TEXTURES = new Texture[BUILDING_TYPE_COUNT];

        /// <summary>
        /// Count of items buildable. Increase whenever you add one
        /// </summary>
        public const int PEOPLE_TYPE_COUNT = 3, PEOPLE_ID_OFFSET = 100;

        //ppl costs: wood
        public static int[] PEOPLE_COSTS_WOOD = { 100, 0, 200 };
        //ppl costs: stone
        public static int[] PEOPLE_COSTS_STONE = { 0, 100, 0 };

        /// <summary>
        /// Add a description to your item
        /// </summary>
        public static string[] PEOPLE_DESCRIPTIONS = new string[] { 
            "Just a friendly peasant, not a madman with an scythe - at least i hope so.",
            "Swordfighter. Main strength is looking evil.",
            "Somehow the only guy that can get stone."};

        /// <summary>
        /// Save the texture of your item here
        /// </summary>
        public static Texture[] PEOPLE_TEXTURES = new Texture[PEOPLE_TYPE_COUNT];

        public static Vector2i textureVector = new Vector2i(0, 0);

        /// <summary>
        /// in x is the id of the building that spawns the menu concerned by this;
        /// in y is an array of people this menu is allowed to show
        /// </summary>
        /// //only the third position is currently not senseless - it is the barrack. 0 and 2 are the standard buildings
        /// //public const int                                             BUILDING_DEFAULT = 0, BUILDING_RED = 1,          BUILDING_BLUE = 2,              BUILDING_BARRACK = 3,           STONE = 4;      WOOD = 5        STONEHACKER
        public static int[][] DISPLAYED_PEOPLE_PER_BUILDING = new int[][] { new int[] { -1 }, new int[] { PEOPLE_PEASANT }, new int[] { PEOPLE_PEASANT }, new int[] { PEOPLE_SWORDMAN }, new int[] { -1 }, new int[] { -1 }, new int[] { PEOPLE_STONEMAN } };
    }
}
