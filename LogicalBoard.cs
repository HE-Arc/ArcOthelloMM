using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;

namespace ArcOthelloMM
{
    /// <summary>
    /// Class to manage the game
    /// </summary>
    [Serializable]
    class LogicalBoard : IPlayable.IPlayable, ISerializable
    {
        private static LogicalBoard instance = null;

        private Player CurrentPlayer { get; set; }
        private Player OpponentPlayer { get; set; }
        private Player WhitePlayer { get; set; }
        private Player BlackPlayer { get; set; }
        private int[,] Board { get; set; }
        private List<Tuple<bool, int[,]>> Archive { get; set; }
        private int IndexHistory { get; set; }

        private Dictionary<Tuple<int, int>, HashSet<Tuple<int, int>>> ListPossibleMove;
        private bool ListPossibleMoveLoaded;

        private bool LastPlayer;

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

        /// <summary>
        /// Constructor for serialization
        /// </summary>
        /// <param name="info"></param>
        /// <param name="context"></param>
        protected LogicalBoard(SerializationInfo info, StreamingContext context)
        {
            WhitePlayer = (Player)info.GetValue("WhitePlayer", typeof(Player));
            BlackPlayer = (Player)info.GetValue("BlackPlayer", typeof(Player));
            Board = (int[,])info.GetValue("Board", typeof(int[,]));
            ListPossibleMove = (Dictionary<Tuple<int, int>, HashSet<Tuple<int, int>>>)info.GetValue("ListPossibleMove", typeof(Dictionary<Tuple<int, int>, HashSet<Tuple<int, int>>>));
            ListPossibleMoveLoaded = (bool)info.GetValue("ListPossibleMoveLoaded", typeof(bool));
            Archive = (List<Tuple<bool, int[,]>>)info.GetValue("BoardHistory", typeof(List<Tuple<bool, int[,]>>));
            IndexHistory = (int)info.GetValue("IndexHistory", typeof(int));
        }

        /// <summary>
        /// Assure there is one white player
        /// </summary>
        /// <returns></returns>
        public static LogicalBoard Instance
        {
            get
            {
                if (instance == null)
                    instance = new LogicalBoard();
                return instance;
            }

            set
            {
                instance = value;
            }
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
            WhitePlayer.Tokens.Add(new Tuple<int, int>(px, py));
            Board[px + 1, py + 1] = WhitePlayer.Value;
            WhitePlayer.Tokens.Add(new Tuple<int, int>(px + 1, py + 1));

            Board[px, py + 1] = BlackPlayer.Value;
            BlackPlayer.Tokens.Add(new Tuple<int, int>(px, py + 1));
            Board[px + 1, py] = BlackPlayer.Value;
            BlackPlayer.Tokens.Add(new Tuple<int, int>(px + 1, py));

            // Init others
            ListPossibleMove = new Dictionary<Tuple<int, int>, HashSet<Tuple<int, int>>>();
            ListPossibleMoveLoaded = false;
            LastPlayer = false;
            Archive = new List<Tuple<bool, int[,]>>();
            IndexHistory = 0;
            AddArchive(LastPlayer, Board);
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
        /// Get the possible moves for a player
        /// </summary>
        /// <param name="isWhite"></param>
        /// <returns></returns>
        public Dictionary<Tuple<int, int>, HashSet<Tuple<int, int>>> GetListPossibleMove(bool isWhite)
        {
            SetPlayer(isWhite);
            foreach (Tuple<int, int> token in CurrentPlayer.Tokens)
            {
                CheckAllPossibleMove(token);
            }

            ListPossibleMoveLoaded = true;
            return ListPossibleMove;
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

            bool finish = false;
            HashSet<Tuple<int, int>> takedTokens = new HashSet<Tuple<int, int>>();
            int i = 1;
            while (!finish)
            {
                finish = CheckOnePossibleMove(token, takedTokens, -i, 0);
                ++i;
            }

            finish = false;
            takedTokens.Clear();
            i = 1;
            while (!finish)
            {
                finish = CheckOnePossibleMove(token, takedTokens, -i, -i);
                ++i;
            }

            finish = false;
            takedTokens.Clear();
            i = 1;
            while (!finish)
            {
                finish = CheckOnePossibleMove(token, takedTokens, 0, -i);
                ++i;
            }

            finish = false;
            takedTokens.Clear();
            i = 1;
            while (!finish)
            {
                finish = CheckOnePossibleMove(token, takedTokens, i, -i);
                ++i;
            }

            finish = false;
            takedTokens.Clear();
            i = 1;
            while (!finish)
            {
                finish = CheckOnePossibleMove(token, takedTokens, i, 0);
                ++i;
            }

            finish = false;
            takedTokens.Clear();
            i = 1;
            while (!finish)
            {
                finish = CheckOnePossibleMove(token, takedTokens, i, i);
                ++i;
            }

            finish = false;
            takedTokens.Clear();
            i = 1;
            while (!finish)
            {
                finish = CheckOnePossibleMove(token, takedTokens, 0, i);
                ++i;
            }

            finish = false;
            takedTokens.Clear();
            i = 1;
            while (!finish)
            {
                finish = CheckOnePossibleMove(token, takedTokens, -i, i);
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
            if (x < 0 || x >= COLUMN || y < 0 || y >= ROW || CurrentPlayer.Tokens.Contains(newToken))
            {
                finished = true;
            }
            else if (!OpponentPlayer.Tokens.Contains(newToken))
            {
                if (Math.Abs(offsetX) > 1 || Math.Abs(offsetY) > 1)
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
            {
                ListPossibleMove.Add(key, new HashSet<Tuple<int, int>>());
                ListPossibleMove[key].Add(new Tuple<int, int>(key.Item1, key.Item2));
            }


            foreach (Tuple<int, int> tokenToAdd in move)
            {
                bool alreadyAdded = false;
                foreach (Tuple<int, int> tokenAdded in ListPossibleMove[key])
                {
                    if (tokenToAdd.Item1 == tokenAdded.Item1 && tokenToAdd.Item2 == tokenAdded.Item2)
                    {
                        alreadyAdded = true;
                        break;
                    }

                    
                }
                if (!alreadyAdded)
                    ListPossibleMove[key].Add(new Tuple<int, int>(tokenToAdd.Item1, tokenToAdd.Item2));
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
            SetPlayer(isWhite);
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
            if (!ListPossibleMoveLoaded)
                GetListPossibleMove(isWhite);

            if (ListPossibleMove.Count == 0)
                return false;
          
            SetPlayer(isWhite);

            foreach (Tuple<int, int> tuple in ListPossibleMove[new Tuple<int, int>(column, line)])
            {
                AddToken(tuple);
            }

            // Reset possible move
            ListPossibleMove.Clear();
            ListPossibleMoveLoaded = false;
            LastPlayer = isWhite;

            ++IndexHistory;
            if (IndexHistory < Archive.Count)
                Archive.RemoveRange(IndexHistory, Archive.Count - IndexHistory);

            AddArchive(isWhite, Board);
            return true;
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

        public void Undo()
        {
            if (IndexHistory > 0)
            {
                --IndexHistory;
                LoadArchive();
            }
        }

        public void Redo()
        {
            if (IndexHistory < Archive.Count - 1)
            {
                ++IndexHistory;
                LoadArchive();
            }
        }

        public void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            info.AddValue("WhitePlayer", WhitePlayer);
            info.AddValue("BlackPlayer", BlackPlayer); 
            info.AddValue("Board", Board);
            info.AddValue("ListPossibleMove", ListPossibleMove);
            info.AddValue("ListPossibleMoveLoaded", ListPossibleMoveLoaded);
            info.AddValue("LastPlayer", LastPlayer);
            info.AddValue("BoardHistory", Archive);
            info.AddValue("IndexHistory", IndexHistory);
        }

        public bool GetLastPlayer()
        {
            return LastPlayer;
        }

        public void AddArchive(bool isWhite, int[,]  Board)
        {
            int[,] BoardCopy = new int[COLUMN, ROW];

            for (int x = 0; x < COLUMN; ++x)
            {
                for (int y = 0; y < ROW; ++y)
                {
                    BoardCopy[x, y] = Board[x, y];
                }
            }

            Archive.Add(new Tuple<bool, int[,]>(isWhite, BoardCopy));
        }

        public void LoadArchive()
        {
            WhitePlayer.Tokens.Clear();
            BlackPlayer.Tokens.Clear();

            if (IndexHistory < Archive.Count - 1)
                LastPlayer = !Archive[IndexHistory + 1].Item1;
            else
                LastPlayer = Archive[IndexHistory].Item1; // Last Redo

            for (int x = 0; x < COLUMN; ++x)
            {
                for (int y = 0; y < ROW; ++y)
                {
                    Board[x, y] = Archive[IndexHistory].Item2[x, y];

                    if (Archive[IndexHistory].Item2[x, y] == WhitePlayer.Value)
                        WhitePlayer.Tokens.Add(new Tuple<int, int>(x, y));
                    else if (Archive[IndexHistory].Item2[x, y] == BlackPlayer.Value)
                        BlackPlayer.Tokens.Add(new Tuple<int, int>(x, y));
                }
            }

            ListPossibleMove.Clear();
            GetListPossibleMove(!LastPlayer);
        }
    }
}
