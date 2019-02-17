using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IAMargueronMottier
{
    /// <summary>
    /// Tools for various classes (everything is static here)
    /// </summary>
    class Tools
    {
        public static readonly int EMPTY = -1;
        public static readonly int WHITE = 0;
        public static readonly int BLACK = 1;

        private static readonly List<Tuple<int, int>> Directions = new List<Tuple<int, int>>
        {
            new Tuple<int, int>(1, 1),
            new Tuple<int, int>(-1, -1),
            new Tuple<int, int>(0, 1),
            new Tuple<int, int>(0, -1),
            new Tuple<int, int>(1, 0),
            new Tuple<int, int>(-1, 0),
            new Tuple<int, int>(1, -1),
            new Tuple<int, int>(-1, 1)
        };

        /// <summary>
        /// Return every possible moves
        /// </summary>
        /// <param name="isWhite"></param>
        /// <returns></returns>
        public static Dictionary<Tuple<int, int>, HashSet<Tuple<int, int>>> GetPossibleMoves(int[,] game, bool isWhite)
        {
            Dictionary<Tuple<int, int>, HashSet<Tuple<int, int>>> listPossibleMoves = new Dictionary<Tuple<int, int>, HashSet<Tuple<int, int>>>();

            int currentValue = isWhite ? WHITE : BLACK;
            int opponentValue = isWhite ? BLACK : WHITE;

            List<Tuple<int, int>> currentTokens = FindTokens(game, currentValue);

            foreach (Tuple<int, int> tokenStart in currentTokens)
            {
                foreach (Tuple<int, int> direction in Directions)
                {
                    Tuple<int, int> tokenPosition = null;
                    HashSet<Tuple<int, int>> toReverse = new HashSet<Tuple<int, int>>();
                    bool directionIsEligibleForAMove = true;
                    int i = 1;

                    while (true)
                    {
                        tokenPosition = new Tuple<int, int>(tokenStart.Item1 + i * direction.Item1, tokenStart.Item2 + i * direction.Item2);

                        if (!BoardContains(game, tokenPosition))
                        {
                            directionIsEligibleForAMove = false;
                            break;
                        }

                        int valueOnBoardAtTokenPosition = game[tokenPosition.Item1, tokenPosition.Item2];

                        // The direct direction neighbour is empty or the 
                        if (toReverse.Count == 0 && valueOnBoardAtTokenPosition == EMPTY || valueOnBoardAtTokenPosition == currentValue)
                        {
                            directionIsEligibleForAMove = false;
                            break;
                        }
                        else if (valueOnBoardAtTokenPosition == opponentValue)
                        {
                            toReverse.Add(tokenPosition);
                        }
                        else if (valueOnBoardAtTokenPosition == EMPTY)
                        {
                            toReverse.Add(tokenPosition);
                            break;
                        }
                        i++;
                    }

                    if (directionIsEligibleForAMove)
                    {
                        if (listPossibleMoves.ContainsKey(tokenPosition))
                            listPossibleMoves[tokenPosition].UnionWith(toReverse);
                        else
                            listPossibleMoves.Add(tokenPosition, toReverse);
                    }
                }
            }
            return listPossibleMoves;
        }

        /// <summary>
        /// Check if the board contain the position given
        /// </summary>
        /// <param name="position"></param>
        /// <returns></returns>
        public static bool BoardContains(int[,] game, Tuple<int, int> position)
        {
            return !(position.Item1 < 0 || position.Item2 < 0 || position.Item1 >= game.GetLength(0) || position.Item2 >= game.GetLength(1));
        }

        /// <summary>
        /// Find every tokens for the specified player
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static List<Tuple<int, int>> FindTokens(int[,] game, int value)
        {
            List<Tuple<int, int>> tokens = new List<Tuple<int, int>>();
            for (int i = 0; i < game.GetLength(0); i++)
            {
                for (int j = 0; j < game.GetLength(1); j++)
                {
                    if (game[i, j] == value)
                        tokens.Add(new Tuple<int, int>(i, j));
                }
            }
            return tokens;
        }
    }
}
