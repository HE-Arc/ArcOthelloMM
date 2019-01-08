using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ArcOthelloMM
{
    /// <summary>
    /// Singleton for the white player
    /// </summary>
    class WhitePlayer : Player
    {
        private static WhitePlayer instance;

        /// <summary>
        /// Instanciate the white player
        /// </summary>
        private WhitePlayer()
        {
            Tokens = new List<Tuple<int, int>>();
            Value = 1;
        }

        /// <summary>
        /// Assure there is one white player
        /// </summary>
        /// <returns></returns>
        public static WhitePlayer GetWhitePlayer()
        {
            if (instance == null)
            {
                instance = new WhitePlayer();
            }

            return instance;
        }
    }
}
