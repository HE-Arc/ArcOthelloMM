using ArcOthelloMM;
using System;
using System.Collections.Generic;
using System.Linq;

namespace IAMargueronMottier
{
    class TreeNode
    {
        private Dictionary<Tuple<int, int>, HashSet<Tuple<int, int>>> ListPossibleMove { get; set; }
        public int[,] Board;
        private int CurrentPlayerValue;
        private List<Tuple<int, int>> CurrentPlayerTokens;
        private List<Tuple<int, int>> OpponentPlayerTokens;

        private static Dictionary<Tuple<int,int>, int[,]> PonderationGrids;

        static TreeNode()
        {
            PonderationGrids = new Dictionary<Tuple<int, int>, int[,]>();
            // for other dimension grid add the ponderation grid here
            PonderationGrids[new Tuple<int, int>(9, 7)] = new int[,] {
                {20, 03, 07, 05, 07, 05, 07, 03, 20},
                {03, 01, 01, 01, 01, 01, 01, 01, 03},
                {07, 01, 05, 05, 05, 05, 05, 01, 07},
                {05, 01, 05, 05, 05, 05, 05, 01, 05},
                {07, 01, 05, 05, 05, 05, 05, 01, 07},
                {03, 01, 05, 05, 05, 05, 05, 01, 03},
                {20, 03, 07, 05, 07, 05, 07, 03, 20},
            };
        }

        public TreeNode(TreeNode treeNode)
        {
            //ListPossibleMove Copy
            ListPossibleMove = new Dictionary<Tuple<int, int>, HashSet<Tuple<int, int>>>();
            foreach (KeyValuePair<Tuple<int, int>, HashSet<Tuple<int, int>>> entry in ListPossibleMove)
            {
                ListPossibleMove[entry.Key] = new HashSet<Tuple<int, int>>(entry.Value);
            }

            //Board copy
            Board = new int[treeNode.Board.GetLength(0), treeNode.Board.GetLength(1)];
            for (int x = 0; x < Board.GetLength(0); x++)
                for (int y = 0; y < Board.GetLength(1); y++)
                    Board[x, y] = treeNode.Board[x, y];

            //CurrentPlayerValue Copy
            CurrentPlayerValue = treeNode.CurrentPlayerValue;

            //CurrentPlayerTokens Copy
            CurrentPlayerTokens = new List<Tuple<int, int>>();
            foreach (Tuple<int, int> token in treeNode.CurrentPlayerTokens)
                CurrentPlayerTokens.Add(new Tuple<int, int>(token.Item1, token.Item2));

            //OpponentPlayerTokens Copy
            OpponentPlayerTokens = new List<Tuple<int, int>>();
            foreach (Tuple<int, int> token in treeNode.OpponentPlayerTokens)
                OpponentPlayerTokens.Add(new Tuple<int, int>(token.Item1, token.Item2));
        }

        public TreeNode(int[,] board, int currentPlayerValue)
        {
            Board = board;
            CurrentPlayerValue = currentPlayerValue;
            ListPossibleMove = new Dictionary<Tuple<int, int>, HashSet<Tuple<int, int>>>();

            // Simulation of token possession of players
            CurrentPlayerTokens = new List<Tuple<int, int>>();
            OpponentPlayerTokens = new List<Tuple<int, int>>();

            for (int x = 0; x < Board.GetLength(0); ++x)
            {
                for (int y = 0; y < Board.GetLength(1); ++y)
                {
                    if (Board[x, y] == currentPlayerValue)
                        CurrentPlayerTokens.Add(new Tuple<int, int>(x, y));
                    else if (Board[x, y] != -1)
                        OpponentPlayerTokens.Add(new Tuple<int, int>(x, y));
                }
            }

            GetListPossibleMove();
        }

        public int Evaluate()
        {
            return EvaluatePositionsWithPonderation();
        }

        public int EvaluatePositionsWithPonderation()
        {
            Tuple<int, int> gridDim = new Tuple<int, int>(Board.GetLength(0), Board.GetLength(1));
            if (!PonderationGrids.ContainsKey(gridDim))
                throw new Exception("grid dim not handled for the AI");

            int[,] ponderationGrid = PonderationGrids[gridDim];
            int sum = 0;
            foreach(Tuple<int, int> pos in CurrentPlayerTokens)
                sum += ponderationGrid[pos.Item1, pos.Item2];
            foreach (Tuple<int, int> pos in OpponentPlayerTokens)
                sum -= ponderationGrid[pos.Item1, pos.Item2];
            return sum;
        }

        public bool Final()
        {
            return (ListPossibleMove.Count == 0);
        }

        public List<Tuple<int, int>> Ops()
        {
            List<Tuple<int, int>> moves = ListPossibleMove.Keys.ToList<Tuple<int, int>>();
            return moves;
        }

        public TreeNode Apply(Tuple<int, int> key)
        {
            TreeNode copy = new TreeNode(this);

            foreach (Tuple<int, int> token in ListPossibleMove[key])
            {
                copy.CurrentPlayerTokens.Add(token);
                copy.Board[token.Item1, token.Item2] = CurrentPlayerValue;

                if (copy.OpponentPlayerTokens.Contains(token))
                    copy.OpponentPlayerTokens.Remove(token);
            }

            copy.SwitchPlayer();
            copy.GetListPossibleMove();
            
            if (copy.ListPossibleMove.Count <= 0) //turn skiped
            {
                copy.SwitchPlayer();
                copy.GetListPossibleMove();
                //if(copy.ListPossibleMove.Count == 0)
                  //  throw new Exception("Why did you ask the ai to play on a finished game");
            }
           
            return copy;
        }

        public void SwitchPlayer()
        {
            CurrentPlayerValue = CurrentPlayerValue == Player.Player0.Value ? Player.Player1.Value : Player.Player0.Value;
        }

        private void GetListPossibleMove()
        {
            foreach (Tuple<int, int> token in CurrentPlayerTokens)
            {
                CheckAllPossibleMove(token);
            }
        }

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
            if (x < 0 || x >= Board.GetLength(1) || y < 0 || y >= Board.GetLength(0) || CurrentPlayerTokens.Contains(newToken))
            {
                finished = true;
            }
            else if (!OpponentPlayerTokens.Contains(newToken))
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
    }
}
