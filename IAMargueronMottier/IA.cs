using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ArcOthelloMM
{
    class IA
    {
        private static IA Instance;
        private Random random;

        private IA()
        {
            random = new Random();
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
            int move = random.Next(moves.Count);
            return keys[move];
        }

        public Tuple<int, int> MinMaxAI(int[,] game, int level, bool whiteTurn)
        {
            return new Tuple<int, int>(0, 0);
        }

        private Tuple<int, int> min(TreeNode root, int depth)
        {
            if(depth == 0 || root.final())
            {
                return 
            }
        }
    }
}
