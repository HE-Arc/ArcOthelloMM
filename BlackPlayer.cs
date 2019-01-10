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
        private static BlackPlayer Instance;

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
        public static BlackPlayer GetInstance()
        {
            if (Instance == null)
            {
                Instance = new BlackPlayer();
            }

            return Instance;
        }

        /// <summary>
        /// Return the value of black player
        /// </summary>
        /// <returns></returns>
        public int GetValue()
        {
            return Value;
        }
    }
}
