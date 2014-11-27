﻿using System;
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
        public const int MOUSE_CLICK_MESSAGE = 0, STRING_MESSAGE = 1, GAMESTATE_BROADCAST=2;
        public static Int32 ID_COUNTER = 0;
    }
}
