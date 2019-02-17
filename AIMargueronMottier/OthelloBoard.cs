using System;
using System.Collections.Generic;

namespace AIMargueronMottier
{
    public class OthelloBoard : IPlayable.IPlayable
    {
        private int[,] Board;

        private const int EMPTY = -1;
        private const int WHITE = 0;
        private const int BLACK = 1;

        private const int COLUMN = 9;
        private const int ROW = 9;

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

        public OthelloBoard()
        {
            Board = new int[9,7];
            for(int i = 0; i < Board.GetLength(0); i++)
            {
                for (int j = 0; j < Board.GetLength(1); j++)
                {
                    Board[i, j] = EMPTY;
                }
            }

            int px = COLUMN / 2 - 1;
            int py = ROW / 2;

            Board[px, py + 1] = WHITE;
            Board[px + 1, py] = WHITE;
            Board[px, py] = BLACK;
            Board[px + 1, py + 1] = BLACK;
        }

        public string GetName()
        {
            return "MargueronMottier";
        }

        public int[,] GetBoard()
        {
            return Board;
        }

        public int GetWhiteScore()
        {
            return FindTokens(WHITE).Count;
        }

        public int GetBlackScore()
        {
            return FindTokens(BLACK).Count;
        }

        private List<Tuple<int,int>> FindTokens(int value)
        {
            List<Tuple<int, int>> tokens = new List<Tuple<int, int>>();
            for (int i = 0; i < Board.GetLength(0); i++)
            {
                for (int j = 0; j < Board.GetLength(1); j++)
                {
                    if (Board[i, j] == value)
                        tokens.Add(new Tuple<int, int>(i, j));
                }
            }
            return tokens;
        }

        public bool IsPlayable(int column, int line, bool isWhite)
        {
            return GetPossibleMoves(isWhite).ContainsKey(new Tuple<int, int>(column, line));
        }

        public Tuple<int, int> GetNextMove(int[,] game, int level, bool whiteTurn)
        {
            throw new NotImplementedException();
        }

        public bool PlayMove(int column, int line, bool isWhite)
        {
            Dictionary<Tuple<int, int>, HashSet<Tuple<int, int>>> listPossibleMoves = GetPossibleMoves(isWhite);

            int currentValue = isWhite ? WHITE : BLACK;
            int opponentValue = isWhite ? BLACK : WHITE;

            Tuple<int, int> position = new Tuple<int, int>(column, line);

            if (listPossibleMoves.ContainsKey(position))
                return false;

            HashSet<Tuple<int, int>> toReverse = listPossibleMoves[position];

            foreach (Tuple<int, int> tokenToReverse in toReverse)
            {
                Board[tokenToReverse.Item1, tokenToReverse.Item2] = currentValue;
            }

            return true;
        }

        public Dictionary<Tuple<int, int>, HashSet<Tuple<int, int>>> GetPossibleMoves(bool isWhite)
        {
            Dictionary<Tuple<int, int>, HashSet<Tuple<int, int>>> listPossibleMoves = new Dictionary<Tuple<int, int>, HashSet<Tuple<int, int>>>();

            int currentValue = isWhite ? WHITE : BLACK;
            int opponentValue = isWhite ? BLACK: WHITE;

            List<Tuple<int, int>> currentTokens = FindTokens(currentValue);

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

                        if (!BoardContains(tokenPosition))
                        {
                            directionIsEligibleForAMove = false;
                            break;
                        }

                        int valueOnBoardAtTokenPosition = Board[tokenPosition.Item1, tokenPosition.Item2];

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

        private bool BoardContains(Tuple<int, int> position)
        {
            return !(position.Item1 < 0 || position.Item2 < 0 || position.Item1 >= Board.GetLength(0) || position.Item2 >= Board.GetLength(1));
        }
    }
}
