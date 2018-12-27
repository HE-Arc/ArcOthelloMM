using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace ArcOthelloMM
{
    /// <summary>
    /// Class to manage the game
    /// </summary>
    class LogicalBoard
    {
        public Player Current { get; internal set; }
        public Player Opponent { get; internal set; }
        public Player Player1 { get; internal set; }
        public Player Player2 { get; internal set; }
        public Token[,] Board { get; internal set; }

        public Dictionary<Token, HashSet<Token>> listMove;

        private const int ROW = 7;
        private const int COLUMN = 9;

        /// <summary>
        /// Class to manage logical game
        /// </summary>
        public LogicalBoard()
        {
            // Create players
            Player1 = new Player();
            Player2 = new Player();
            Current = Player1;
            Opponent = Player2;

            // Init Board
            Board = new Token[ROW, COLUMN];
            for (int x = 0; x < ROW; ++x)
            {
                for (int y = 0; y < COLUMN; y++)
                {
                    Board[x, y] = new Token(x, y);
                }
            }

            // Set start tokens
            Player1.Tokens.Add(Board[3, 3]);
            Player1.Tokens.Add(Board[4, 4]);
            Player2.Tokens.Add(Board[3, 4]);
            Player2.Tokens.Add(Board[4, 3]);

            // Init others
            listMove = new Dictionary<Token, HashSet<Token>>();
        }

        /// <summary>
        /// Change active player
        /// </summary>
        private void ChangePlayer()
        {
            Player temp = Current;
            Current = Opponent;
            Opponent = temp;
        }

        /// <summary>
        /// Add a token for the current player
        /// </summary>
        /// <param name="token"></param>
        private void AddToken(Token token)
        {
            int x = token.X;
            int y = token.Y;

            Current.Tokens.Add(Board[x, y]);

            // Steal token
            if (Opponent.Tokens.Contains(token))
                Opponent.Tokens.Remove(Board[x, y]);
        }

        /// <summary>
        /// Get the move for the current player
        /// </summary>
        public void GetMovePlayable()
        {
            foreach (Token token in Current.Tokens)
            {
                CheckAllMove(token);
            }
        }

        /// <summary>
        /// Check move possible
        /// from a token for the current player
        /// </summary>
        /// <param name="token"></param>
        private void CheckAllMove(Token token)
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
            HashSet<Token> listLeft = new HashSet<Token>();
            HashSet<Token> listTop = new HashSet<Token>();
            HashSet<Token> listRight = new HashSet<Token>();
            HashSet<Token> listBottom = new HashSet<Token>();
            HashSet<Token> listLeftTop = new HashSet<Token>();
            HashSet<Token> listRightTop = new HashSet<Token>();
            HashSet<Token> listLeftBottom = new HashSet<Token>();
            HashSet<Token> listRightBottom = new HashSet<Token>();

            // continue until all direction was checked
            do
            {
                leftFinished = CheckMove(token, listLeft, -i, 0);
                rightFinished = CheckMove(token, listRight, i, 0);
                topFinished = CheckMove(token, listTop, 0, -i);
                bottomFinished = CheckMove(token, listBottom, 0, -i);
                leftTopFinished = CheckMove(token, listLeftTop, -i, -i);
                rightTopFinished = CheckMove(token, listRightTop, i, -i);
                leftBottomFinished = CheckMove(token, listBottom, -i, i);
                rightBottomFinished = CheckMove(token, listRightBottom, i, i);

                ++i;
            } while (!leftFinished && !topFinished && !rightFinished && !bottomFinished && !leftTopFinished && !rightTopFinished && !leftBottomFinished && !rightBottomFinished); // check if all direction is finished
        }

        /// <summary>
        /// Check move in one direction
        /// </summary>
        /// <param name="token"></param>
        /// <param name="tokens"></param>
        /// <param name="offsetX"></param>
        /// <param name="offsetY"></param>
        /// <returns></returns>
        private bool CheckMove(Token token, HashSet<Token> tokens, int offsetX, int offsetY)
        {
            bool finished = false;

            int x = token.X + offsetX;
            int y = token.Y + offsetY;

            tokens.Add(token);

            // Is on board
            if (x < 0 || x >= ROW || y < 0 || y >= COLUMN)
            {
                finished = true;
            }
            else if (!Opponent.Tokens.Contains(token))
            {
                if (!Current.Tokens.Contains(token) && (Math.Abs(offsetX) + Math.Abs(offsetY)) > 1)
                {
                    SaveMove(tokens, token); // save valid move
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
        private void SaveMove(HashSet<Token> move, Token key)
        {
            // add pieces affected for the case played
            if (listMove.ContainsKey(key))
            {
                foreach (Token token in move)
                {
                    listMove[key].Add(token);
                }
            }
            else
            {
                listMove.Add(key, move);
            }
        }

        /// <summary>
        /// Play a move and take token
        /// </summary>
        /// <param name="key"></param>
        public void Play(Token key)
        {
            foreach (Token token in listMove[key])
            {
                AddToken(token);
            }

            // Reset possible move
            listMove.Clear();

            ChangePlayer();

            GetMovePlayable();
        }
    }
}
