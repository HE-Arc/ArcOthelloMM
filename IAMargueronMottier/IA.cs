using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ArcOthelloMM
{
    class IA
    {
        public static IA Instance;

        private IA()
        {
            
        }

        public static IA GetInstance()
        {
            if (Instance == null)
                Instance = new IA();
            return Instance;
        }

        public Tuple<int, int> GetNextMove(int[,] game, int level, bool whiteTurn)
        {
            return null;
        }
    }
}
