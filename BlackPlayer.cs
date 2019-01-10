using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ArcOthelloMM
{
    /// <summary>
    /// Singleton for the black player
    /// </summary>
    class BlackPlayer : Player
    {
        private static BlackPlayer instance;

        /// <summary>
        /// Instanciate the black player
        /// </summary>
        private BlackPlayer()
        {
            Tokens = new List<Tuple<int, int>>();
            Value = 1;
        }

        /// <summary>
        /// Assure there is one black player
        /// </summary>
        /// <returns></returns>
        public static BlackPlayer GetBlackPlayer()
        {
            if (instance == null)
            {
                instance = new BlackPlayer();
            }

            return instance;
        }
    }
}
