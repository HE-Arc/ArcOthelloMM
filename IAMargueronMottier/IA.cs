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

        private Random Random;
        private int CurrentPlayerValue;

        public Tuple<int, int> GetNextMove(int[,] game, int level, bool whiteTurn)
        {
            CurrentPlayerValue = (whiteTurn) ? Player.Player1.Value : Player.Player0.Value;

            return AlphaBeta(new TreeNode(game, CurrentPlayerValue), level, 1, int.MinValue).Item2;
        }

        private Tuple<int, Tuple<int, int>> AlphaBeta(TreeNode root, int level, int minOrMax, int parentValue)
        {
            // minOrMax = 1 => maximize
            // minOrMax = -1 => minimize

            if (level == 0 || root.Final())
            {
                return new Tuple<int, Tuple<int, int>>(root.Evaluate(), null);
            }

            int optVal = minOrMax * int.MinValue;
            Tuple<int, int> optOp = null;

            // Loop on key of possible move
            foreach (Tuple<int, int> op in root.Ops())
            {
                TreeNode newNode = root.Apply(op);
                Tuple<int, Tuple<int, int>> res = AlphaBeta(newNode, level - 1, minOrMax * -1, optVal);

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

            return new Tuple<int, Tuple<int, int>>(root.Evaluate(), optOp);
        }

        public Tuple<int, int> StupidAI(int[,] game, int level, bool whiteTurn)
        {
            Dictionary<Tuple<int, int>, HashSet<Tuple<int, int>>> moves = LogicalBoard.Instance.CurrentPossibleMoves;
            List<Tuple<int, int>> keys = new List<Tuple<int, int>>(moves.Keys);
            int move = Random.Next(moves.Count);
            return keys[move];
        }
    }
}