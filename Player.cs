using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace ArcOthelloMM
{
    class Player
    {
        public List<Token> Tokens { get; set; }

        /// <summary>
        /// Create a player who own tokens
        /// </summary>
        public Player()
        {
            Tokens = new List<Token>();
        }
    }
}
