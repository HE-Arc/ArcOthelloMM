using IAMargueronMottier;
using System;
using System.Collections.Generic;

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

        private static Random random;
        private int AiMinMaxPlayerValue;

        static IA()
        {
            random = new Random();
        }

        private int GetMinOrMax(TreeNode treeNode)
        {
            if (treeNode.CurrentPlayerValue == AiMinMaxPlayerValue)
                return 1; // it's your turn maximize your outcome
            else
                return -1; //it's his turn, he will minimize your outcome
        }

        private bool GetMinOrMax(TreeNode treeNode, bool b)
        {
            return treeNode.CurrentPlayerValue == AiMinMaxPlayerValue;
        }

        public Tuple<int, int> GetNextMove(int[,] game, int level, bool whiteTurn)
        {
            AiMinMaxPlayerValue = (whiteTurn) ? Player.Player1.Value : Player.Player0.Value;
                        
            return AlphaBetaWikipedia(game, 5);
            //return StupidAI(game, level, whiteTurn);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="root"></param>
        /// <param name="level"></param>
        /// <param name="minOrMax">1 => maximize, -1 => minimize</param>
        /// <param name="parentValue"></param>
        /// <returns></returns>
        private Tuple<int, Tuple<int, int>> AlphaBeta(TreeNode root, int level, int minOrMax, int parentValue)
        {
            Console.WriteLine("Level : " + level + ", isFinal : " + root.Final());

            if (level <= 0 || root.Final())
                return new Tuple<int, Tuple<int, int>>(root.Evaluate(), null);

            int optVal = minOrMax * int.MinValue;
            Tuple<int, int> optOp = null;

            List<Tuple<int, int>> ops = root.Ops();

            foreach (Tuple<int, int> op in ops)
            {
                TreeNode newNode = root.Apply(op);
                newNode.Show();
                //maximize or minimaze depending of the turn
                Tuple<int, Tuple<int, int>> res = AlphaBeta(newNode, level - 1, GetMinOrMax(newNode), optVal);

                if (res.Item1 * minOrMax > optVal * minOrMax)
                {
                    optVal = res.Item1;
                    optOp = op;

                    if (res.Item1 * minOrMax > parentValue * minOrMax)
                        break;
                }
            }

            return new Tuple<int, Tuple<int, int>>(root.Evaluate(), optOp);
        }

        private Tuple<int, int> AlphaBetaWikipedia(int[,] game, int depth)
        {
            TreeNode root = new TreeNode(game, AiMinMaxPlayerValue);

            _AlphaBetaWikipedia(root, depth, int.MinValue, int.MaxValue, true);

            Tuple<int, Tuple<int, int>> res = _AlphaBetaWikipedia(root, depth);

            if (res.Item2 == null)
                throw new Exception("no moves possibles for this player");

            return res.Item2;
        }

        private Tuple<int, Tuple<int, int>> _AlphaBetaWikipedia(TreeNode node, int depth, int alpha = int.MinValue, int beta = int.MaxValue, bool maximizingPlayer = true)
        {
            //Console.WriteLine("Level : " + depth);

            Tuple<int, int> move = null;
            int value;

            if (depth <= 0 || node.Final())
                return new Tuple<int, Tuple<int, int>>(node.Evaluate(), null);

            if (maximizingPlayer)
            {
                value = int.MinValue;
                foreach (Tuple<int, int> op in node.Ops())
                {
                    TreeNode child = node.Apply(op);
                    //Console.WriteLine("move : " + op);
                    //child.Show();
                    Tuple<int, Tuple<int, int>> result = _AlphaBetaWikipedia(child, depth - 1, alpha, beta, GetMinOrMax(child, true));
                    //Console.WriteLine("euristique : " + result.Item1);
                    if (result.Item1 > value)
                    {
                        value = result.Item1;
                        move = op;
                    }
                    if (value > alpha)
                    {
                        alpha = value;
                    }
                    if (alpha >= beta)
                        break;
                }
            }
            else
            {
                value = int.MaxValue;
                foreach (Tuple<int, int> op in node.Ops())
                {
                    TreeNode child = node.Apply(op);
                    //Console.WriteLine("move : " + op);
                    //child.Show();
                    Tuple<int, Tuple<int, int>> result = _AlphaBetaWikipedia(child, depth - 1, alpha, beta, GetMinOrMax(child, true));
                    //Console.WriteLine("euristique : " + result.Item1);
                    if (result.Item1 < value)
                    {
                        value = result.Item1;
                        move = op;
                    }
                    if (value < alpha)
                    {
                        beta = value;
                    }
                    if (alpha >= beta)
                        break;
                }
            }
            return new Tuple<int, Tuple<int, int>>(value, move);
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