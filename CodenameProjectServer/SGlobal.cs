﻿using System;


namespace CodenameProjectServer
{
    static class SGlobal
    {
        /*
         * SERVER GLOBAL FOR SERVER CONTAINING SERVER STUFF; DONT THINK THIS BELONGS TO CLIENT
         */
        public const int MOUSE_CLICK_MESSAGE = 0, STRING_MESSAGE = 1, GAMESTATE_BROADCAST=2, PLANT_BUILDING_MESSAGE=3, CLIENT_IDENTIFICATION_MESSAGE=4;
        public static Int32 ID_COUNTER = 0;

        public const int BUILDING_DEFAULT = 0, BUILDING_BLUE = 1, BUILDING_RED = 2, BUILDING_BARRACK = 3, RESSOURCE_STONE = 4 , PEOPLE_PEASENT = 100;

        public const int AGGRO_RECT_SIZE = 150, EFFECTIVE_RECT_SIZE = 40;
    }
}
