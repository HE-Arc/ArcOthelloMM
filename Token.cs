using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace ArcOthelloMM
{
    class Token
    {
        public int X { get; set; }
        public int Y { get; set; }

        /// <summary>
        /// Case on the logicial board
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        public Token(int x, int y)
        {
            X = x;
            Y = y;
        }
    }
}
