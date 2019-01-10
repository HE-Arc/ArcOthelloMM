using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using IPlayable;

namespace ArcOthelloMM
{
    /// <summary>
    /// Class to manage the game
    /// </summary>
    class LogicalBoard : IPlayable.IPlayable
    {
        private static LogicalBoard Instance;

        private Player CurrentPlayer { get; set; }
        private Player OpponentPlayer { get; set; }
        private WhitePlayer WhitePlayer { get; set; }
        private BlackPlayer BlackPlayer { get; set; }
        private int[,] Board { get; set; }

        public Dictionary<Tuple<int, int>, HashSet<Tuple<int, int>>> listMove;

        private const int ROW = 7;
        private const int COLUMN = 9;

        /// <summary>
        /// Class to manage logical game
        /// </summary>
        private LogicalBoard()
        {
            // Create players
            WhitePlayer = WhitePlayer.GetInstance();
            BlackPlayer = BlackPlayer.GetInstance();

            // Init Board
            Board = new int[COLUMN, ROW];
            for (int x = 0; x < COLUMN; ++x)
            {
                for (int y = 0; y < ROW; ++y)
                {
                    Board[x, y] = -1;
                }
            }

            // Init others
            listMove = new Dictionary<Tuple<int, int>, HashSet<Tuple<int, int>>>();
        }

        /// <summary>
        /// Assure there is one white player
        /// </summary>
        /// <returns></returns>
        public static LogicalBoard GetInstance()
        {
            if (Instance == null)
            {
                Instance = new LogicalBoard();
            }

            return Instance;
        }

        /// <summary>
        /// Prepare the board for the party
        /// </summary>
        /// <param name="whiteFirstPlayer"></param>
        public void StartGame(bool whiteFirstPlayer)
        {
            // Set start tokens
            Board[3, 3] = WhitePlayer.Value;
            WhitePlayer.Tokens.Add(new Tuple<int, int>(3, 3));
            Board[4, 4] = WhitePlayer.Value;
            WhitePlayer.Tokens.Add(new Tuple<int, int>(4, 4));

            Board[3, 4] = BlackPlayer.Value;
            BlackPlayer.Tokens.Add(new Tuple<int, int>(3, 4));
            Board[4, 3] = BlackPlayer.Value;
            BlackPlayer.Tokens.Add(new Tuple<int, int>(4, 3));

            // Move for the first player
            SetPlayer(whiteFirstPlayer);
            GetPlayableMove();
        }

        /// <summary>
        /// Set active player
        /// </summary>
        private void SetPlayer(bool isWhite)
        {
            if (isWhite)
            {
                CurrentPlayer = WhitePlayer;
                OpponentPlayer = BlackPlayer;
            }
            else
            {
                CurrentPlayer = BlackPlayer;
                OpponentPlayer = WhitePlayer;
            }
        }

        /// <summary>
        /// Add a token for the current player
        /// </summary>
        /// <param name="token"></param>
        private void AddToken(Tuple<int, int> token)
        {
            CurrentPlayer.Tokens.Add(token);
            Board[token.Item1, token.Item2] = CurrentPlayer.Value;

            // Steal token
            if (OpponentPlayer.Tokens.Contains(token))
                OpponentPlayer.Tokens.Remove(token);
        }

        /// <summary>
        /// Get the move for the current player
        /// </summary>
        public void GetPlayableMove()
        {
            foreach (Tuple<int, int> token in CurrentPlayer.Tokens)
            {
                CheckAllPossibleMove(token);
            }
        }

        /// <summary>
        /// Check move possible
        /// from a token for the current player
        /// </summary>
        /// <param name="token"></param>
        private void CheckAllPossibleMove(Tuple<int, int> token)
        {
            // Algo :
            //      Loop and check in every direction
            //      if the case is ennemy continue to check in the direction
            //      if the case is empty and the increment is greater than 1
            //      => it means there is only one or more token of the opponent between the token
            //         and a possible token to play
            //      else move impossible

            int i = 1;
            bool leftFinished = true;
            bool topFinished = true;
            bool rightFinished = true;
            bool bottomFinished = true;

            // Diagonal
            bool leftTopFinished = true;
            bool rightTopFinished = true;
            bool leftBottomFinished = true;
            bool rightBottomFinished = true;

            // pieces affected by possible move in every direction
            HashSet<Tuple<int, int>> listLeft = new HashSet<Tuple<int, int>>();
            HashSet<Tuple<int, int>> listTop = new HashSet<Tuple<int, int>>();
            HashSet<Tuple<int, int>> listRight = new HashSet<Tuple<int, int>>();
            HashSet<Tuple<int, int>> listBottom = new HashSet<Tuple<int, int>>();
            HashSet<Tuple<int, int>> listLeftTop = new HashSet<Tuple<int, int>>();
            HashSet<Tuple<int, int>> listRightTop = new HashSet<Tuple<int, int>>();
            HashSet<Tuple<int, int>> listLeftBottom = new HashSet<Tuple<int, int>>();
            HashSet<Tuple<int, int>> listRightBottom = new HashSet<Tuple<int, int>>();

            // continue until all direction was checked
            do
            {
                leftFinished = CheckOnePossibleMove(token, listLeft, -i, 0);
                rightFinished = CheckOnePossibleMove(token, listRight, i, 0);
                topFinished = CheckOnePossibleMove(token, listTop, 0, -i);
                bottomFinished = CheckOnePossibleMove(token, listBottom, 0, -i);
                leftTopFinished = CheckOnePossibleMove(token, listLeftTop, -i, -i);
                rightTopFinished = CheckOnePossibleMove(token, listRightTop, i, -i);
                leftBottomFinished = CheckOnePossibleMove(token, listBottom, -i, i);
                rightBottomFinished = CheckOnePossibleMove(token, listRightBottom, i, i);

                ++i;
            } while (!leftFinished
                && !topFinished
                && !rightFinished
                && !bottomFinished
                && !leftTopFinished
                && !rightTopFinished
                && !leftBottomFinished
                && !rightBottomFinished); // check if all direction is finished
        }

        /// <summary>
        /// Check move in one direction
        /// </summary>
        /// <param name="token"></param>
        /// <param name="tokens"></param>
        /// <param name="offsetX"></param>
        /// <param name="offsetY"></param>
        /// <returns></returns>
        private bool CheckOnePossibleMove(Tuple<int, int> token, HashSet<Tuple<int, int>> tokens, int offsetX, int offsetY)
        {
            bool finished = false;

            int x = token.Item1 + offsetX;
            int y = token.Item2 + offsetY;

            Tuple<int, int> newToken = new Tuple<int, int>(x, y);
            tokens.Add(newToken);

            // Is on board
            if (x < 0 || x >= COLUMN || y < 0 || y >= ROW)
            {
                finished = true;
            }
            else if (!OpponentPlayer.Tokens.Contains(newToken))
            {
                if (!CurrentPlayer.Tokens.Contains(newToken) && (Math.Abs(offsetX) + Math.Abs(offsetY)) > 1)
                {
                    SavePossibleMove(tokens, token); // save valid move
                }

                finished = true;
            }

            return finished;
        }

        /// <summary>
        /// Save a valid move
        /// </summary>
        /// <param name="move"></param>
        /// <param name="key"></param>
        private void SavePossibleMove(HashSet<Tuple<int, int>> move, Tuple<int, int> key)
        {
            // add pieces affected for the case played
            if (listMove.ContainsKey(key))
            {
                foreach (Tuple<int, int> tuple in move)
                {
                    listMove[key].Add(tuple);
                }
            }
            else
            {
                listMove.Add(key, move);
            }
        }

        /// <summary>
        /// Check if an empty case is playable
        /// for the current player
        /// </summary>
        /// <param name="token"></param>
        /// <returns></returns>
        private bool CheckAllPlayableCase(Tuple<int, int> token)
        {
            // Algo :
            //      Loop and check in every direction
            //      if the case is ennemy continue to check in the direction
            //      if the case belong to the current player and the increment is greater than 1
            //      => it means there is possible move

            int i = 1;
            bool valid = false;

            // Values of states:
            //      -1  impossible move
            //      0   need to continue
            //      1   possible move

            int leftState = 0;
            int topState = 0;
            int rightState = 0;
            int bottomState = 0;

            // Diagonal
            int leftTopState = 0;
            int rightTopState = 0;
            int leftBottomState = 0;
            int rightBottomState = 0;

            // continue until all direction was checked
            do
            {
                leftState = CheckOnePlayableCase(token, -i, 0);
                if (leftState == 1)
                {
                    valid = true;
                    break;
                }

                rightState = CheckOnePlayableCase(token, i, 0);
                if (rightState == 1)
                {
                    valid = true;
                    break;
                }

                topState = CheckOnePlayableCase(token, 0, -i);
                if (topState == 1)
                {
                    valid = true;
                    break;
                }

                bottomState = CheckOnePlayableCase(token, 0, -i);
                if (bottomState == 1)
                {
                    valid = true;
                    break;
                }

                leftTopState = CheckOnePlayableCase(token, -i, -i);
                if (leftTopState == 1)
                {
                    valid = true;
                    break;
                }

                rightTopState = CheckOnePlayableCase(token, i, -i);
                if (rightTopState == 1)
                {
                    valid = true;
                    break;
                }

                leftBottomState = CheckOnePlayableCase(token, -i, i);
                if (leftBottomState == 1)
                {
                    valid = true;
                    break;
                }

                rightBottomState = CheckOnePlayableCase(token, i, i);
                if (rightBottomState == 1)
                {
                    valid = true;
                    break;
                }

                ++i;
            } while (leftState == 0
                && topState == 0
                && rightState == 0
                && bottomState == 0
                && leftTopState == 0
                && rightTopState == 0
                && leftBottomState == 0
                && rightBottomState == 0); // check if all direction is finished

            return valid;
        }

        /// <summary>
        /// Check move for empty case
        /// in one direction
        /// </summary>
        /// <param name="token"></param>
        /// <param name="offsetX"></param>
        /// <param name="offsetY"></param>
        /// <returns></returns>
        private int CheckOnePlayableCase(Tuple<int, int> token, int offsetX, int offsetY)
        {
            int state = 0;

            int y = token.Item1 + offsetY;
            int x = token.Item2 + offsetX;
            Tuple<int, int> newToken = new Tuple<int, int>(x, y);

            // Is on board
            if (x < 0 || x >= COLUMN || y < 0 || y >= ROW)
            {
                state = -1;
            }
            else if (!OpponentPlayer.Tokens.Contains(newToken))
            {
                if (CurrentPlayer.Tokens.Contains(newToken) && (Math.Abs(offsetX) + Math.Abs(offsetY)) > 1)
                {
                    state = 1;
                }
                else
                {
                    state = -1;
                }
            }

            return state;
        }

        /// <summary>
        /// Return the name of the IA
        /// </summary>
        /// <returns></returns>
        public string GetName()
        {
            return "MargueronMottierIA";
        }

        /// <summary>
        /// Check if the case is playable for a people
        /// </summary>
        /// <param name="column"></param>
        /// <param name="line"></param>
        /// <param name="isWhite"></param>
        /// <returns></returns>
        public bool IsPlayable(int column, int line, bool isWhite)
        {
            SetPlayer(isWhite);
            return Board[line, column] == 0 && CheckAllPlayableCase(new Tuple<int, int>(column, line));
        }

        /// <summary>
        /// Play move and take tokens
        /// </summary>
        /// <param name="column"></param>
        /// <param name="line"></param>
        /// <param name="isWhite"></param>
        /// <returns></returns>
        public bool PlayMove(int column, int line, bool isWhite)
        {
            SetPlayer(isWhite);

            foreach (Tuple<int, int> tuple in listMove[new Tuple<int, int>(column, line)])
            {
                AddToken(tuple);
            }

            // Reset possible move
            listMove.Clear();

            GetPlayableMove();

            return (listMove.Count > 0);
        }

        public Tuple<int, int> GetNextMove(int[,] game, int level, bool whiteTurn)
        {
            // logic ia
            throw new NotImplementedException();
        }

        /// <summary>
        /// Get the current state of board
        /// </summary>
        /// <returns></returns>
        public int[,] GetBoard()
        {
            return Board;
        }

        /// <summary>
        /// Get the score of the white player
        /// </summary>
        /// <returns></returns>
        public int GetWhiteScore()
        {
            return WhitePlayer.Tokens.Count;
        }

        /// <summary>
        /// Get the score of the black player
        /// </summary>
        /// <returns></returns>
        public int GetBlackScore()
        {
            return BlackPlayer.Tokens.Count;
        }

        /// <summary>
        /// Get the tokens of the white player
        /// </summary>
        /// <returns></returns>
        public List<Tuple<int, int>> GetWhiteTokens()
        {
            return WhitePlayer.Tokens;
        }

        /// <summary>
        /// Get the tokens of the black player
        /// </summary>
        /// <returns></returns>
        public List<Tuple<int, int>> GetBlackTokens()
        {
            return BlackPlayer.Tokens;
        }
    }
}
