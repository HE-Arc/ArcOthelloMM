using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ArcOthelloMM
{
    class IA
    {
        private static IA Instance;

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
            return StupidAI(game, level, whiteTurn);
        }

        public Tuple<int, int> StupidAI(int[,] game, int level, bool whiteTurn)
        {
            Dictionary<Tuple<int, int>, HashSet<Tuple<int, int>>> moves = LogicalBoard.Instance.CurrentPossibleMoves;
            List<Tuple<int, int>> keys = new List<Tuple<int, int>>(moves.Keys);
            Random random = new Random();
            int move = random.Next(moves.Count);
            return keys[move];
        }
    }
}
