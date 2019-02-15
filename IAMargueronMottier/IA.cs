using IAMargueronMottier;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ArcOthelloMM
{
    class IA
    {
        /// <summary>
        /// Property for singleton
        /// </summary>
        private static IA instance;
        public static IA Instance
        {
            get
            {
                if (instance == null)
                    instance = new IA();
                return instance;
            }

            set
            {
                instance = value;
            }
        }

        private Random random;

        public Tuple<int, int> GetNextMove(int[,] game, int level, bool whiteTurn)
        {
            return StupidAI(game, level, whiteTurn);
        }

        private Tuple<int, object> AlphaBeta(TreeNode root, int level, int minOrMax, int parentValue)
        {
            if (level == 0 || root.Final())
            {
                return new Tuple<int, object>(root.Evaluate(), null);
            }

            int optVal = minOrMax * int.MinValue;
            object optOp = null;

            foreach (var op in root.Ops())
            {
                TreeNode newNode = root.Apply(op);
                Tuple<int, object> res = AlphaBeta(newNode, level - 1, minOrMax * -1, optVal);

                if (res.Item1 * minOrMax > optVal * minOrMax)
                {
                    optVal = res.Item1;
                    optOp = op;

                    if (res.Item1 * minOrMax > parentValue * minOrMax)
                    {
                        break;
                    }
                }
            }

            return new Tuple<int, object>(root.Evaluate(), null);
        }

        public Tuple<int, int> StupidAI(int[,] game, int level, bool whiteTurn)
        {
            Dictionary<Tuple<int, int>, HashSet<Tuple<int, int>>> moves = LogicalBoard.Instance.CurrentPossibleMoves;
            List<Tuple<int, int>> keys = new List<Tuple<int, int>>(moves.Keys);
            int move = random.Next(moves.Count);
            return keys[move];
        }
    }
}