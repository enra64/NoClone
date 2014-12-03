using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodenameProjectServer
{
    static class SGlobal
    {
        /*
         * SERVER GLOBAL FOR SERVER CONTAINING SERVER STUFF; DONT THINK THIS BELONGS TO CLIENT
         */
        public const int MOUSE_CLICK_MESSAGE = 0, STRING_MESSAGE = 1, GAMESTATE_BROADCAST=2, PLANT_BUILDING_MESSAGE=3;
        public static Int32 ID_COUNTER = 0;

        public const int DEFAULT = 0, BLUE = 1, RED = 2;

    }
}
