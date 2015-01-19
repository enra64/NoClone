using System;
using System.Collections.Generic;

namespace CodenameProjectServer
{
    static class SGlobal
    {
        /*
         * SERVER GLOBAL FOR SERVER CONTAINING SERVER STUFF; DONT THINK THIS BELONGS TO CLIENT
         */
        public const int MOUSE_CLICK_MESSAGE = 0, 
            STRING_MESSAGE = 1, 
            GAMESTATE_BROADCAST=2, 
            PLANT_BUILDING_MESSAGE=3, 
            CLIENT_IDENTIFICATION_MESSAGE=4,
            BOUNDINGSIZE_MESSAGE = 5,
            SPAWN_PEOPLE_MESSAGE = 6,
            MASS_SELECTION_MESSAGE = 7;

        public static Int32 ID_COUNTER = 0;

        public enum Direction {
            Uhhhh,
            Top,
            Left,
            Bottom,
            Right
        }

        public static List<SizeKeeper> SizeList = new List<SizeKeeper>();

        public const int 
            BUILDING_DEFAULT = 0, 
            BUILDING_BLUE = 1, 
            BUILDING_RED = 2, 
            BUILDING_BARRACK = 3, 
            RESSOURCE_STONE = 4,
            RESSOURCE_WOOD =5,
            BUILDING_STONEHACKER = 6,
            PEOPLE_PEASANT = 100, 
            PEOPLE_SWORDMAN = 101,
            PEOPLE_STONEMAN = 102;
        //building costs: wood
        public static float[] BUILDING_COSTS_WOOD = { 200, 0, 0, 400, 0, 0, 200 };
        //building costs: stone
        public static float[] BUILDING_COSTS_STONE = { 0, 0, 0, 300, 0, 0, 0 };
        //ppl costs: wood
        public static float[] PEOPLE_COSTS_WOOD = { 75, 150, 75 };
        //ppl costs: stone
        public static float[] PEOPLE_COSTS_STONE = { 0, 100, 0 };


        public const int AGGRO_RECT_SIZE = 150, EFFECTIVE_RECT_SIZE = 40;

        public const float RESSOURCE_INCREASE_WOOD = 0.09f, RESSOURCE_INCREASE_STONE = 0.09f;
    }
}
