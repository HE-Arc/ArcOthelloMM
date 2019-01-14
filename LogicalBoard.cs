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
        private Player WhitePlayer { get; set; }
        private Player BlackPlayer { get; set; }
        private int[,] Board { get; set; }

        private Dictionary<Tuple<int, int>, HashSet<Tuple<int, int>>> ListPossibleMove;
        private bool ListPossibleMoveLoaded;

        private const int ROW = 7;
        private const int COLUMN = 9;

        /// <summary>
        /// Class to manage logical game
        /// </summary>
        private LogicalBoard()
        {
            WhitePlayer = Player.GetWhite();
            BlackPlayer = Player.GetBlack();
            ResetGame();
        }

        public int GetRow()
        {
            return ROW;
        }

        public int GetCol()
        {
            return COLUMN;
        }

        public void ResetGame()
        {
            // Create players
            Player.GetWhite().Reset();
            Player.GetBlack().Reset();


            // Init Board
            Board = new int[COLUMN, ROW];
            for (int x = 0; x < COLUMN; ++x)
            {
                for (int y = 0; y < ROW; ++y)
                {
                    Board[x, y] = -1;
                }
            }

            // Set start tokens
            int px = COLUMN / 2 - 1;
            int py = ROW / 2;

            Board[px, py] = WhitePlayer.Value;
            WhitePlayer.AddToken(new Tuple<int, int>(px, py));
            Board[px + 1, py + 1] = WhitePlayer.Value;
            WhitePlayer.AddToken(new Tuple<int, int>(px + 1, py + 1));

            Board[px, py + 1] = BlackPlayer.Value;
            BlackPlayer.AddToken(new Tuple<int, int>(px, py + 1));
            Board[px + 1, py] = BlackPlayer.Value;
            BlackPlayer.AddToken(new Tuple<int, int>(px + 1, py));

            // Init others
            ListPossibleMove = new Dictionary<Tuple<int, int>, HashSet<Tuple<int, int>>>();
            ListPossibleMoveLoaded = false;
        }

        /// <summary>
        /// Assure there is one white player
        /// </summary>
        /// <returns></returns>
        public static LogicalBoard GetInstance()
        {
            if (Instance == null)
                Instance = new LogicalBoard();
            return Instance;
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
            CurrentPlayer.AddToken(token);
            Board[token.Item1, token.Item2] = CurrentPlayer.Value;

            // Steal token
            if (OpponentPlayer.ContainsToken(token))
                OpponentPlayer.RemoveToken(token);
        }

        /// <summary>
        /// Get the possible moves for a player
        /// </summary>
        /// <param name="isWhite"></param>
        /// <returns></returns>
        public Dictionary<Tuple<int, int>, HashSet<Tuple<int, int>>> GetListPossibleMove(bool isWhite)
        {
            SetPlayer(isWhite);

            foreach (Token token in CurrentPlayer.GetTokens())
            {
                CheckAllPossibleMove(token);
            }
            CurrentPlayer.ResetTokens();
            ListPossibleMoveLoaded = true;
            return ListPossibleMove;
        }

        /// <summary>
        /// Check move possible
        /// from a token for the current player
        /// </summary>
        /// <param name="token"></param>
        private void CheckAllPossibleMove(Token token)
        {
            // Algo :
            //      Loop and check in every direction
            //      if the case is ennemy continue to check in the direction
            //      if the case is empty and the increment is greater than 1
            //      => it means there is only one or more token of the opponent between the token
            //         and a possible token to play
            //      else move impossible

            /*int i = 1;
            bool leftFinished = false;
            bool topFinished = false;
            bool rightFinished = false;
            bool bottomFinished = false;

            // Diagonal
            bool leftTopFinished = false;
            bool rightTopFinished = false;
            bool leftBottomFinished = false;
            bool rightBottomFinished = false;

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
                if (!leftFinished)
                    leftFinished = CheckOnePossibleMove(token, listLeft, -i, 0);
                if (!rightFinished)
                    rightFinished = CheckOnePossibleMove(token, listRight, i, 0);
                if (!topFinished)
                    topFinished = CheckOnePossibleMove(token, listTop, 0, -i);
                if (!bottomFinished)
                    bottomFinished = CheckOnePossibleMove(token, listBottom, 0, i);
                if (!leftTopFinished)
                    leftTopFinished = CheckOnePossibleMove(token, listLeftTop, -i, -i);
                if (!rightTopFinished)
                    rightTopFinished = CheckOnePossibleMove(token, listRightTop, i, -i);
                if (!leftBottomFinished)
                    leftBottomFinished = CheckOnePossibleMove(token, listLeftBottom, -i, i);
                if (!rightBottomFinished)
                    rightBottomFinished = CheckOnePossibleMove(token, listRightBottom, i, i);

                ++i;
            } while (!leftFinished
                || !topFinished
                || !rightFinished
                || !bottomFinished
                || !leftTopFinished
                || !rightTopFinished
                || !leftBottomFinished
                || !rightBottomFinished); // check if all direction is finished
            */

            HashSet<Tuple<int, int>> takedTokens = new HashSet<Tuple<int, int>>();
            int i = 1;
            while (!token.Left)
            {
                token.Left = CheckOnePossibleMove(token.Tuple, takedTokens, -i, 0);
                ++i;
            }

            takedTokens.Clear();
            i = 1;
            while (!token.LeftTop)
            {
                token.LeftTop = CheckOnePossibleMove(token.Tuple, takedTokens, -i, -i);
                ++i;
            }

            takedTokens.Clear();
            i = 1;
            while (!token.Top)
            {
                token.Top = CheckOnePossibleMove(token.Tuple, takedTokens, i, 0);
                ++i;
            }

            takedTokens.Clear();
            i = 1;
            while (!token.RightTop)
            {
                token.RightTop = CheckOnePossibleMove(token.Tuple, takedTokens, -i, i);
                ++i;
            }

            takedTokens.Clear();
            i = 1;
            while (!token.Right)
            {
                token.Right = CheckOnePossibleMove(token.Tuple, takedTokens, 0, -i);
                ++i;
            }

            takedTokens.Clear();
            i = 1;
            while (!token.RightBottom)
            {
                token.RightBottom = CheckOnePossibleMove(token.Tuple, takedTokens, i, -i);
                ++i;
            }

            takedTokens.Clear();
            i = 1;
            while (!token.Bottom)
            {
                token.Bottom = CheckOnePossibleMove(token.Tuple, takedTokens, 0, i);
                ++i;
            }

            takedTokens.Clear();
            i = 1;
            while (!token.LeftBottom)
            {
                token.LeftBottom = CheckOnePossibleMove(token.Tuple, takedTokens, -i, i);
                ++i;
            }
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
            else if (!OpponentPlayer.ContainsToken(newToken))
            {
                if (!CurrentPlayer.ContainsToken(newToken) && (Math.Abs(offsetX) > 1 || Math.Abs(offsetY) > 1))
                {
                    SavePossibleMove(tokens, newToken); // save valid move
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
            // with a depth copy

            if (!ListPossibleMove.ContainsKey(key))
                ListPossibleMove.Add(key, new HashSet<Tuple<int, int>>());

            foreach (Tuple<int, int> tuple in move)
            {
                ListPossibleMove[key].Add(new Tuple<int, int>(tuple.Item1, tuple.Item2));
            }
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
            GetListPossibleMove(isWhite);

            if (Board[line, column] != 0)
            {
                foreach (KeyValuePair<Tuple<int, int>, HashSet<Tuple<int, int>>> possibleMove in ListPossibleMove)
                {
                    if (possibleMove.Value.Contains(new Tuple<int, int>(column, line)))
                    {
                        return true;
                    }
                }
            }

            return false;
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
            if (ListPossibleMoveLoaded)
                GetListPossibleMove(isWhite);

            if (ListPossibleMove.Count == 0)
                return false;
  
            foreach (Tuple<int, int> tuple in ListPossibleMove[new Tuple<int, int>(column, line)])
            {
                AddToken(tuple);
            }

            // Reset possible move
            ListPossibleMove.Clear();
            ListPossibleMoveLoaded = false;

            return (ListPossibleMove.Count > 0);
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
            return WhitePlayer.GetTokens().Count;
        }

        public int BlackScore {
            get
            {
                return GetBlackScore();
            }
        }

        public int WhiteScore
        {
            get
            {
                return GetWhiteScore();
            }
        }

        /// <summary>
        /// Get the score of the black player
        /// </summary>
        /// <returns></returns>
        public int GetBlackScore()
        {
            return BlackPlayer.GetTokens().Count;
        }

        /// <summary>
        /// Get the tokens of the white player
        /// </summary>
        /// <returns></returns>
        public List<Tuple<int, int>> GetWhiteTokens()
        {
            return WhitePlayer.GetTuples();
        }

        /// <summary>
        /// Get the tokens of the black player
        /// </summary>
        /// <returns></returns>
        public List<Tuple<int, int>> GetBlackTokens()
        {
            return BlackPlayer.GetTuples();
        }
    }
}
