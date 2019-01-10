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
        private static WhitePlayer Instance;

        /// <summary>
        /// Instanciate the white player
        /// </summary>
        private WhitePlayer()
        {
            Tokens = new List<Tuple<int, int>>();
            Value = 0;
        }

        /// <summary>
        /// Assure there is one white player
        /// </summary>
        /// <returns></returns>
        public static WhitePlayer GetInstance()
        {
            if (Instance == null)
            {
                Instance = new WhitePlayer();
            }

            return Instance;
        }

        /// <summary>
        /// Return the value of white player
        /// </summary>
        /// <returns></returns>
        public int GetValue()
        {
            return Value;
        }
    }
}
