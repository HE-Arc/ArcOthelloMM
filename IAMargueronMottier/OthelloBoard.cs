using System;
using System.Collections.Generic;

namespace IAMargueronMottier
{
    /// <summary>
    /// Classe used generate the dll according to the interface IPlayable for the Othello tournament
    /// </summary>
    public class OthelloBoard : IPlayable.IPlayable
    {
        private int[,] game;

        private const int COLUMN = 9; // x length
        private const int ROW = 7; //y length

        private int AIValueOnBoard;

        //Promote randomly a 'bad move' for the current heuristic (can be used to adjust difficulty for the game)
        private static readonly Random random = new Random();
        private static readonly bool RandomPromotions = false;
        private static readonly int RandomPromotionChances = 10000;
        private static readonly int RandomPromotionFactor = 1000;

        /// <summary>
        /// Default constructor, instanciate the board with the initial tokens
        /// </summary>
        public OthelloBoard()
        {
            game = new int[COLUMN, ROW];
            for(int i = 0; i < game.GetLength(0); i++)
            {
                for (int j = 0; j < game.GetLength(1); j++)
                {
                    game[i, j] = Tools.EMPTY;
                }
            }

            int px = COLUMN / 2 - 1;
            int py = ROW / 2;

            game[px, py + 1] = Tools.BLACK;
            game[px + 1, py] = Tools.BLACK;
            game[px, py] = Tools.WHITE;
            game[px + 1, py + 1] = Tools.WHITE;
        }

        /// <summary>
        /// Return the name of the AI
        /// </summary>
        /// <returns></returns>
        public string GetName()
        {
            return "🕹 Margueron Mottier 🕹";
        }

        /// <summary>
        /// Return the actual board
        /// </summary>
        /// <returns></returns>
        public int[,] GetBoard()
        {
            return game;
        }

        /// <summary>
        /// Return the score of the white player
        /// </summary>
        /// <returns></returns>
        public int GetWhiteScore()
        {
            return Tools.FindTokens(game, Tools.WHITE).Count;
        }

        /// <summary>
        /// Return the score of the black player
        /// </summary>
        /// <returns></returns>
        public int GetBlackScore()
        {
            return Tools.FindTokens(game, Tools.BLACK).Count;
        }

        /// <summary>
        /// Check if a move is valid
        /// </summary>
        /// <param name="column"></param>
        /// <param name="line"></param>
        /// <param name="isWhite"></param>
        /// <returns></returns>
        public bool IsPlayable(int column, int line, bool isWhite)
        {
            return GetPossibleMoves(isWhite).ContainsKey(new Tuple<int, int>(column, line));
        }

        /// <summary>
        /// Get next move ai implementation
        /// </summary>
        /// <param name="game"></param>
        /// <param name="level"></param>
        /// <param name="whiteTurn"></param>
        /// <returns></returns>
        public Tuple<int, int> GetNextMove(int[,] game, int level, bool whiteTurn)
        {
            AIValueOnBoard = (whiteTurn) ? Tools.WHITE : Tools.BLACK;

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
                return new Tuple<int, int>(-1, -1);
            }

            Console.WriteLine("move/heuristique : " + res);

            return res.Item2;
        }

        /// <summary>
        /// Return true if must optimize because it's the ai turn or false if not
        /// </summary>
        /// <param name="treeNode"></param>
        /// <returns></returns>
        private bool GetMinOrMax(TreeNode treeNode)
        {
            return treeNode.CurrentValue == AIValueOnBoard;
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
        /// Update the board acording the the selected move
        /// </summary>
        /// <param name="column"></param>
        /// <param name="line"></param>
        /// <param name="isWhite"></param>
        /// <returns>false if the move cant be played</returns>
        public bool PlayMove(int column, int line, bool isWhite)
        {
            Dictionary<Tuple<int, int>, HashSet<Tuple<int, int>>> listPossibleMoves = GetPossibleMoves(isWhite);

            int currentValue = isWhite ? Tools.WHITE : Tools.BLACK;
            int opponentValue = isWhite ? Tools.BLACK : Tools.WHITE;

            Tuple<int, int> position = new Tuple<int, int>(column, line);

            if (!listPossibleMoves.ContainsKey(position))
            {
                return false;
            }

            HashSet<Tuple<int, int>> toReverse = listPossibleMoves[position];

            foreach (Tuple<int, int> tokenToReverse in toReverse)
                game[tokenToReverse.Item1, tokenToReverse.Item2] = currentValue;

            return true;
        }

        /// <summary>
        /// Return every possible moves
        /// </summary>
        /// <param name="isWhite"></param>
        /// <returns></returns>
        public Dictionary<Tuple<int, int>, HashSet<Tuple<int, int>>> GetPossibleMoves(bool isWhite)
        {
            return Tools.GetPossibleMoves(game, isWhite);
        }
    }
}
