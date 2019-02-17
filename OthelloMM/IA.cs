using IAMargueronMottier;
using System;
using System.Collections.Generic;

namespace ArcOthelloMM
{
    class IA
    {
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
        private int AIValueOnBoard;

        //Promote randomly a 'bad move' for the current heuristic (can be used to adjust difficulty for the game)
        private static readonly bool RandomPromotions = false;
        private static readonly int RandomPromotionChances = 10000;
        private static readonly int RandomPromotionFactor = 1000;

        static IA()
        {
            random = new Random();
        }

        private bool GetMinOrMax(TreeNode treeNode)
        {
            return treeNode.CurrentValue == AIValueOnBoard;
        }

        /// <summary>
        /// The required fonction for the tournament
        /// </summary>
        /// <param name="game"></param>
        /// <param name="level"></param>
        /// <param name="whiteTurn"></param>
        /// <returns></returns>
        public Tuple<int, int> GetNextMove(int[,] game, int level, bool whiteTurn)
        {
            AIValueOnBoard = (whiteTurn) ? Player.Player1.Value : Player.Player0.Value;

            // Should we count root as a level?
            // If we should, swap comment the two following lines
            // -> if level is 5 it's 5 level plus the root
            //return AlphaBeta(game, level - 1);
            return AlphaBeta(game, level);
            //return StupidAI(game, level, whiteTurn);
        }

        /// <summary>
        /// Alpha-beta for the othello
        /// </summary>
        /// <param name="game"></param>
        /// <param name="depth"></param>
        /// <returns></returns>
        private Tuple<int, int> AlphaBeta(int[,] game, int depth)
        {
            TreeNode root = new TreeNode(game, AIValueOnBoard);

            Tuple<int, Tuple<int, int>> res = _AlphaBeta(root, depth);

            if (res.Item2 == null)
            {
                Console.WriteLine("no moves possibles for this player");
            }

            Console.WriteLine("move/heuristique : " + res);

            return res.Item2;
        }

        /// <summary>
        /// Alpha-beta recursive function, adapted version of //https://en.wikipedia.org/wiki/Alpha%E2%80%93beta_pruning
        /// </summary>
        /// <param name="node">The start node for the algorithm, corresponds to a state of the game</param>
        /// <param name="depth">Current depth</param>
        /// <param name="alpha">Alpha value</param>
        /// <param name="beta">Beta value</param>
        /// <param name="maximizingPlayer">Indicate if it's a maximization or not</param>
        /// <returns></returns>
        private Tuple<int, Tuple<int, int>> _AlphaBeta(TreeNode node, int depth, int alpha = int.MinValue, int beta = int.MaxValue, bool maximizingPlayer = true)
        {
            Tuple<int, int> move = null;
            int value;

            if (depth <= 0 || node.Final())
                return new Tuple<int, Tuple<int, int>>(node.Evaluate(AIValueOnBoard), null);

            if (maximizingPlayer)
            {
                value = int.MinValue;
                foreach (Tuple<int, int> op in node.Ops())
                {
                    TreeNode child = node.Apply(op);
                    Tuple<int, Tuple<int, int>> result = _AlphaBeta(child, depth - 1, alpha, beta, GetMinOrMax(child));
                    
                    RandomPromotion(ref result);

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
                    Tuple<int, Tuple<int, int>> result = _AlphaBeta(child, depth - 1, alpha, beta, GetMinOrMax(child));

                    RandomPromotion(ref result);

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

        /// <summary>
        /// Random promotion of a leaf, can be used to adapt the ai difficulty
        /// </summary>
        /// <param name="result"></param>
        private void RandomPromotion(ref Tuple<int, Tuple<int, int>> result) //ref not necessary but more clear
        {
            //Random promotion
            if (RandomPromotions && random.Next(RandomPromotionChances) == 0)
            {
                result = new Tuple<int, Tuple<int, int>>(RandomPromotionFactor * result.Item1, result.Item2);
                Console.WriteLine("promotion", result);
            }
        }

        /// <summary>
        /// First ai used for the test of the .NET projet, play a random possible move
        /// </summary>
        /// <param name="game"></param>
        /// <param name="level"></param>
        /// <param name="whiteTurn"></param>
        /// <returns></returns>
        public Tuple<int, int> StupidAI(int[,] game, int level, bool whiteTurn)
        {
            Dictionary<Tuple<int, int>, HashSet<Tuple<int, int>>> moves = LogicalBoard.Instance.CurrentPossibleMoves;
            List<Tuple<int, int>> keys = new List<Tuple<int, int>>(moves.Keys);
            int move = random.Next(moves.Count);
            return keys[move];
        }
    }
}