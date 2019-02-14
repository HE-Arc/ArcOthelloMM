using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.Serialization;

namespace ArcOthelloMM
{
    /// <summary>
    /// Class to manage the game
    /// </summary>
    [Serializable]
    public class LogicalBoard : IPlayable.IPlayable, ISerializable, INotifyPropertyChanged
    {
        private static LogicalBoard instance = null;

        public Player CurrentPlayer { get { return CurrentPlayerTurn ? Player.Player1 : Player.Player0; } }
        public Player OpponentPlayer { get { return CurrentPlayerTurn ? Player.Player0 : Player.Player1; } }

        private int[,] Board { get; set; }
        private List<Tuple<bool, int[,], Tuple<int, int>>> Archive { get; set; }
        private int IndexHistory { get; set; }

        public Tuple<int, int> LastMovePosition { get; set; }

        private Dictionary<Tuple<int, int>, HashSet<Tuple<int, int>>> ListPossibleMove;

        public bool CurrentPlayerTurn { get; set; }

        private const int ROW = 7;
        private const int COLUMN = 9;

        /// <summary>
        /// Event for binding the score
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        /// Property to bind
        /// </summary>
        private int player0Score;
        public int Player0Score
        {
            get
            {
                return player0Score;
            }
            internal set
            {
                player0Score = value;
                NotifyPropertyChanged("Player0Score");
            }
        }

        /// <summary>
        /// Property to bind
        /// </summary>
        private int player1Score;
        public int Player1Score
        {
            get
            {
                return player1Score;
            }
            internal set
            {
                player1Score = value;
                NotifyPropertyChanged("Player1Score");
            }
        }

        private void UpdateScores()
        {
            Player0Score = Player.Player0.Score;
            Player1Score = Player.Player1.Score;
        }

        /// <summary>
        /// Notify the modification for the binding
        /// </summary>
        /// <param name="info"></param>
        private void NotifyPropertyChanged(String info)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(info));
            }
        }
        /// <summary>
        /// Class to manage logical game
        /// </summary>
        public LogicalBoard()
        {
            ResetGame();
        }

        /// <summary>
        /// Assure there is one instance
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

        /// <summary>
        /// Get the number of row of board
        /// </summary>
        /// <returns></returns>
        public int GetRow()
        {
            return ROW;
        }

        /// <summary>
        /// Get the number of column of board
        /// </summary>
        /// <returns></returns>
        public int GetCol()
        {
            return COLUMN;
        }

        /// <summary>
        /// Restart a game
        /// </summary>
        public void ResetGame()
        {
            // Create players
            Player.Player1.Reset();
            Player.Player0.Reset();

            Player1Score = 0;
            Player0Score = 0;

            LastMovePosition = null;

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

            Board[px, py] = Player.Player1.Value;
            Player.Player1.Tokens.Add(new Tuple<int, int>(px, py));
            Board[px + 1, py + 1] = Player.Player1.Value;
            Player.Player1.Tokens.Add(new Tuple<int, int>(px + 1, py + 1));

            Board[px, py + 1] = Player.Player0.Value;
            Player.Player0.Tokens.Add(new Tuple<int, int>(px, py + 1));
            Board[px + 1, py] = Player.Player0.Value;
            Player.Player0.Tokens.Add(new Tuple<int, int>(px + 1, py));

            

            // Init others
            ListPossibleMove = new Dictionary<Tuple<int, int>, HashSet<Tuple<int, int>>>();
            CurrentPlayerTurn = false;
            Archive = new List<Tuple<bool, int[,], Tuple<int, int>>>();
            IndexHistory = 0;
            AddArchive();

            UpdateScores();
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
        private Dictionary<Tuple<int, int>, HashSet<Tuple<int, int>>> UpdatePossibleMove(bool isWhite)
        {
            ListPossibleMove.Clear();
            foreach (Tuple<int, int> token in CurrentPlayer.Tokens)
            {
                CheckAllPossibleMove(token);
            }
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
            //      Loop for each direction
            //      if the case is ennemy continue to check in the direction
            //      if the case is empty and the increment is greater than 1
            //      => it means there is only one or more token of the opponent between the token
            //         and a possible token to play
            //      else move impossible

            // Left
            bool finish = false;
            HashSet<Tuple<int, int>> takedTokens = new HashSet<Tuple<int, int>>();
            int i = 1;
            while (!finish)
            {
                finish = CheckOnePossibleMove(token, takedTokens, -i, 0);
                ++i;
            }

            // Left-top
            finish = false;
            takedTokens.Clear();
            i = 1;
            while (!finish)
            {
                finish = CheckOnePossibleMove(token, takedTokens, -i, -i);
                ++i;
            }

            // Top
            finish = false;
            takedTokens.Clear();
            i = 1;
            while (!finish)
            {
                finish = CheckOnePossibleMove(token, takedTokens, 0, -i);
                ++i;
            }

            // Right-top
            finish = false;
            takedTokens.Clear();
            i = 1;
            while (!finish)
            {
                finish = CheckOnePossibleMove(token, takedTokens, i, -i);
                ++i;
            }

            // Right
            finish = false;
            takedTokens.Clear();
            i = 1;
            while (!finish)
            {
                finish = CheckOnePossibleMove(token, takedTokens, i, 0);
                ++i;
            }

            // Right-bottom
            finish = false;
            takedTokens.Clear();
            i = 1;
            while (!finish)
            {
                finish = CheckOnePossibleMove(token, takedTokens, i, i);
                ++i;
            }

            // Bottom
            finish = false;
            takedTokens.Clear();
            i = 1;
            while (!finish)
            {
                finish = CheckOnePossibleMove(token, takedTokens, 0, i);
                ++i;
            }

            // Left-bottom
            finish = false;
            takedTokens.Clear();
            i = 1;
            while (!finish)
            {
                finish = CheckOnePossibleMove(token, takedTokens, -i, i);
                ++i;
            }
        }

        public void PlayAI()
        {
            Tuple<int, int> move = IA.GetInstance().GetNextMove(Board, 0, CurrentPlayerTurn);
            LogicalBoard.Instance.PlayMove(move.Item1, move.Item2, CurrentPlayerTurn);
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
                // Check if there is token between the start and the current token
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
                ListPossibleMove.Add(key, new HashSet<Tuple<int, int>>());


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
            UpdatePossibleMove(isWhite);

            if (Board[line, column] != 0 || ListPossibleMove.ContainsKey(new Tuple<int, int>(column, line)))
                return true;

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
            UpdatePossibleMove(isWhite);

            if (ListPossibleMove.Count == 0)
                return false;

            foreach (Tuple<int, int> tuple in ListPossibleMove[new Tuple<int, int>(column, line)])
            {
                AddToken(tuple);
            }

            // Reset possible move
            ListPossibleMove.Clear();
            CurrentPlayerTurn = !isWhite;

            // Remove useless moves if there was undo before the play
            ++IndexHistory;
            if (IndexHistory < Archive.Count)
                Archive.RemoveRange(IndexHistory, Archive.Count - IndexHistory);

            // Archive the move
            LastMovePosition = new Tuple<int, int>(column, line);
            AddArchive();

            // Update the scrore
            Player1Score = Player.Player1.Score;
            Player0Score = Player.Player0.Score;

            return true;
        }

        /// <summary>
        /// Return the move of the IA
        /// </summary>
        /// <param name="game"></param>
        /// <param name="level"></param>
        /// <param name="whiteTurn"></param>
        /// <returns></returns>
        public Tuple<int, int> GetNextMove(int[,] game, int level, bool whiteTurn)
        {
            return IA.GetInstance().GetNextMove(game, level, whiteTurn);
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
        /// Property to get the list of possible moves for a player
        /// </summary>
        public Dictionary<Tuple<int, int>, HashSet<Tuple<int, int>>> CurrentPossibleMoves
        {
            get
            {
                return UpdatePossibleMove(CurrentPlayerTurn);
            }
        }

        /// <summary>
        /// Ride up the archive for an undo
        /// </summary>
        public void Undo()
        {
            if (CanUndo())
            {
                --IndexHistory;
                LoadArchive();
            }
        }

        /// <summary>
        /// Ride down the archive for an redo
        /// </summary>
        public void Redo()
        {
            if (CanRedo())
            {
                ++IndexHistory;
                LoadArchive();
            }
        }

        /// <summary>
        /// Check if undo is possible
        /// </summary>
        /// <returns></returns>
        public bool CanUndo()
        {
            return IndexHistory > 0;
        }

        /// <summary>
        /// Check if redo is possible
        /// </summary>
        /// <returns></returns>
        public bool CanRedo()
        {
            return IndexHistory < Archive.Count - 1;
        }

        /// <summary>
        /// Save a move in the archive
        /// </summary>
        /// <param name="isWhite"></param>
        /// <param name="Board"></param>
        /// <param name="lastPlay"></param>
        public void AddArchive()
        {
            // Depth copy
            int[,] BoardCopy = new int[COLUMN, ROW];
            for (int x = 0; x < COLUMN; ++x)
            {
                for (int y = 0; y < ROW; ++y)
                {
                    BoardCopy[x, y] = Board[x, y];
                }
            }

            // Archive copy
            Archive.Add(new Tuple<bool, int[,], Tuple<int,int>>(CurrentPlayerTurn, BoardCopy, LastMovePosition));
        }

        /// <summary>
        /// Load an archive from the current index of the archive
        /// </summary>
        public void LoadArchive()
        {
            Player.Player0.Tokens.Clear();
            Player.Player1.Tokens.Clear();

            // Get the turn
            CurrentPlayerTurn = Archive[IndexHistory].Item1;

            // Get the last play
            LastMovePosition = Archive[IndexHistory].Item3;

            // Give palyers tokens
            for (int x = 0; x < COLUMN; ++x)
            {
                for (int y = 0; y < ROW; ++y)
                {
                    Board[x, y] = Archive[IndexHistory].Item2[x, y];

                    if (Archive[IndexHistory].Item2[x, y] == Player.Player1.Value)
                        Player.Player1.Tokens.Add(new Tuple<int, int>(x, y));
                    else if (Archive[IndexHistory].Item2[x, y] == Player.Player0.Value)
                        Player.Player0.Tokens.Add(new Tuple<int, int>(x, y));
                }
            }

            UpdateScores();
        }

        /// <summary>
        /// Return the white score
        /// </summary>
        /// <returns></returns>
        public int GetWhiteScore()
        { 
            return Player1Score;
        }

        /// <summary>
        /// Return the black score
        /// </summary>
        /// <returns></returns>
        public int GetBlackScore()
        {
            return Player0Score;
        }


        /// <summary>
        /// Serialization implementation
        /// </summary>
        /// <param name="info"></param>
        /// <param name="context"></param>
        public void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            info.AddValue("Player.Player0", Player.Player0);
            info.AddValue("Player.Player1", Player.Player1);
            info.AddValue("Board", Board);
            info.AddValue("CurrentPlayerTurn", CurrentPlayerTurn);
            info.AddValue("BoardHistory", Archive);
            info.AddValue("IndexHistory", IndexHistory);
            info.AddValue("LastMovePosition", LastMovePosition);
        }

        /// <summary>
        /// Constructor for deserialization
        /// </summary>
        /// <param name="info"></param>
        /// <param name="context"></param>
        protected LogicalBoard(SerializationInfo info, StreamingContext context)
        {
            ResetGame();
            Player.Player0 = (Player)info.GetValue("Player.Player0", typeof(Player));
            Player.Player1 = (Player)info.GetValue("Player.Player1", typeof(Player));
            Board = (int[,])info.GetValue("Board", typeof(int[,]));
            CurrentPlayerTurn = (bool)info.GetValue("CurrentPlayerTurn", typeof(bool));
            Archive = (List<Tuple<bool, int[,], Tuple<int, int>>>)info.GetValue("BoardHistory", typeof(List<Tuple<bool, int[,], Tuple<int, int>>>));
            IndexHistory = (int)info.GetValue("IndexHistory", typeof(int));
            LastMovePosition = (Tuple<int, int>)info.GetValue("LastMovePosition", typeof(Tuple<int, int>));
            UpdateScores();
        }
    }
}
