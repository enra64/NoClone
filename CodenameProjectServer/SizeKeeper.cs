using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodenameProjectServer {
    class SizeKeeper {
        public Int32 Type { get; set; }
        public Int32 X { get; set; }
        public Int32 Y { get; set; }
        public SizeKeeper(Int32 _type, Int32 _x, Int32 _y) {
            Type = _type;
            X = _x;
            Y = _y;
        }
    }
}
