using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ArcOthelloMM
{
    public class Token
    {
        public bool Left { get; set; }
        public bool LeftTop { get; set; }
        public bool Top { get; set; }
        public bool RightTop { get; set; }
        public bool Right { get; set; }
        public bool RightBottom { get; set; }
        public bool Bottom { get; set; }
        public bool LeftBottom { get; set; }
        public Tuple<int, int> Tuple { get; set; }

        public Token(Tuple<int, int> tuple)
        {
            Tuple = new Tuple<int, int>(tuple.Item1, tuple.Item2);
            ResetDirections();
        }

        public void ResetDirections()
        {
            Left = false;
            LeftTop = false;
            Top = false;
            RightTop = false;
            Right = false;
            RightBottom = false;
            Bottom = false;
            LeftBottom = false;
        }
    }
}
