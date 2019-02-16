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

        private bool GetMinOrMax(TreeNode treeNode)
        {
            return treeNode.CurrentValue == AiMinMaxPlayerValue;
        }

        public Tuple<int, int> GetNextMove(int[,] game, int level, bool whiteTurn)
        {
            Console.WriteLine("Next move");
            AiMinMaxPlayerValue = (whiteTurn) ? Player.Player1.Value : Player.Player0.Value;
                        
            return AlphaBetaWikipedia(game, 5);
            //return StupidAI(game, level, whiteTurn);
        }

        private Tuple<int, int> AlphaBetaWikipedia(int[,] game, int depth)
        {
            TreeNode root = new TreeNode(game, AiMinMaxPlayerValue);

            _AlphaBetaWikipedia(root, depth, int.MinValue, int.MaxValue, true);

            Tuple<int, Tuple<int, int>> res = _AlphaBetaWikipedia(root, depth);

            if (res.Item2 == null)
                throw new Exception("no moves possibles for this player");

            Console.WriteLine("move/heuristique : " + res);

            return res.Item2;
        }

        private Tuple<int, Tuple<int, int>> _AlphaBetaWikipedia(TreeNode node, int depth, int alpha = int.MinValue, int beta = int.MaxValue, bool maximizingPlayer = true)
        {
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
                    Tuple<int, Tuple<int, int>> result = _AlphaBetaWikipedia(child, depth - 1, alpha, beta, GetMinOrMax(child));
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
                    Tuple<int, Tuple<int, int>> result = _AlphaBetaWikipedia(child, depth - 1, alpha, beta, GetMinOrMax(child));
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