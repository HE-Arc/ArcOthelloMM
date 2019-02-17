using ArcOthelloMM;
using System;
using System.Collections.Generic;
using System.Linq;

namespace IAMargueronMottier
{
    class TreeNode
    {
        public int[,] Board;

        public int CurrentValue;
        public int OpponentValue { get { return CurrentValue == 0 ? 1 : 0; } }
        public const int EmptyValue = -1;

        private List<Tuple<int, int>> TokenPlayer0;
        private List<Tuple<int, int>> TokenPlayer1;

        private List<Tuple<int, int>> CurrentToken { get { return CurrentValue == 0 ? TokenPlayer0 : TokenPlayer1; } }
        private List<Tuple<int, int>> OpponentToken { get { return CurrentValue == 1 ? TokenPlayer0 : TokenPlayer1; } }

        private Dictionary<Tuple<int, int>, HashSet<Tuple<int, int>>> ListPossibleMove { get; set; }

        private bool GameIsFinished;

        private static readonly Dictionary<Tuple<int, int>, int[,]> PonderationGrids;
        private static readonly List<Tuple<int, int>> directions;

        static TreeNode()
        {
            PonderationGrids = new Dictionary<Tuple<int, int>, int[,]>
            {
                [new Tuple<int, int>(9, 7)] = new int[,] {
                {1000,  -50,   50,    7,   50,  -50, 1000},
                { -50, -300,    0,    0,    0, -300,  -50},
                {  50,    0,    7,    7,    7,    0,   50},
                {   7,    0,    7,    7,    7,    0,    7},
                {  50,    0,    7,    7,    7,    0,   50},
                {   7,    0,    7,    7,    7,    0,    7},
                {  50,    0,    7,    7,    7,    0,   50},
                { -50, -300,    0,    0,    0, -300,  -50},
                {1000,  -50,   50,    7,   50,  -50, 1000},
            }
            };

            //C# doesnt have a vector2 native class?
            directions = new List<Tuple<int, int>>
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

            //TestMoveDetection();
        }

        public static void TestMoveDetection()
        {
            ExempleMoveNormal();
            Console.WriteLine("-----------");
            ExempleMoveSkip();
            Console.WriteLine("-----------");
            ExempleMoveFinish();
            Console.WriteLine("-----------");
        }

        private static void ExempleMoveNormal()
        {
            int[,] board = new int[,]
            {
                {-1, -1, -1, -1, -1, -1, -1},
                {-1, -1, -1, -1, -1, -1, -1},
                {-1, -1, -1, -1, -1, -1, -1},
                {-1, -1,  1,  0, -1, -1, -1},
                {-1, -1,  0,  1, -1, -1, -1},
                {-1, -1, -1, -1, -1, -1, -1},
                {-1, -1, -1, -1, -1, -1, -1},
                {-1, -1, -1, -1, -1, -1, -1},
                {-1, -1, -1, -1, -1, -1, -1},
            };
            TreeNode treeNode = new TreeNode(board, 0);
            treeNode.Show();
            Console.WriteLine("----");
            treeNode = treeNode.Apply(new Tuple<int, int>(2, 2));
            treeNode.Show();
            treeNode = treeNode.Apply(new Tuple<int, int>(2, 3));
            treeNode.Show();
        }

        private static void ExempleMoveSkip()
        {
            int[,] board = new int[,]
            {
                {-1, -1, -1, -1,  0,  0,  0},
                {-1, -1, -1, -1,  1,  1,  0},
                {-1, -1, -1, -1, -1, -1,  0},
                {-1, -1, -1, -1, -1, -1,  0},
                {-1, -1, -1, -1, -1, -1, -1},
                {-1, -1, -1, -1, -1, -1, -1},
                {-1, -1, -1, -1, -1, -1, -1},
                {-1, -1, -1, -1, -1, -1, -1},
                {-1, -1, -1, -1, -1, -1, -1},
            };
            TreeNode treeNode = new TreeNode(board, 0);
            treeNode.Show();
            Console.WriteLine("----");
            treeNode = treeNode.Apply(new Tuple<int, int>(2, 5));
            treeNode.Show();            
        }

        private static void ExempleMoveFinish()
        {
            int[,] board = new int[,]
            {
                {-1, -1, -1, -1, -1, -1, -1},
                {-1, -1, -1, -1, -1, -1, -1},
                {-1, -1, -1, -1, -1, -1, -1},
                {-1, -1, -1,  0, -1, -1, -1},
                {-1, -1, -1,  1, -1, -1, -1},
                {-1, -1, -1,  1, -1, -1, -1},
                {-1, -1, -1, -1, -1, -1, -1},
                {-1, -1, -1, -1, -1, -1, -1},
                {-1, -1, -1, -1, -1, -1, -1},
            };
            TreeNode treeNode = new TreeNode(board, 0);
            treeNode.Show();
            Console.WriteLine("----");
            treeNode = treeNode.Apply(new Tuple<int, int>(6, 3));
            treeNode.Show();

            Console.WriteLine("-----------");
        }

        public TreeNode(TreeNode treeNode)
        {
            // ListPossibleMove Copy
            ListPossibleMove = new Dictionary<Tuple<int, int>, HashSet<Tuple<int, int>>>();
            foreach (KeyValuePair<Tuple<int, int>, HashSet<Tuple<int, int>>> entry in treeNode.ListPossibleMove)
                ListPossibleMove.Add(new Tuple<int, int>(entry.Key.Item1, entry.Key.Item2), new HashSet<Tuple<int, int>>(entry.Value));

            // Board copy
            Board = new int[treeNode.Board.GetLength(0), treeNode.Board.GetLength(1)];
            for (int x = 0; x < Board.GetLength(0); x++)
                for (int y = 0; y < Board.GetLength(1); y++)
                    Board[x, y] = treeNode.Board[x, y];

            // CurrentPlayerValue Copy
            CurrentValue = treeNode.CurrentValue;

            // CurrentPlayerTokens Copy
            TokenPlayer0 = new List<Tuple<int, int>>();
            foreach (Tuple<int, int> token in treeNode.TokenPlayer0)
                TokenPlayer0.Add(new Tuple<int, int>(token.Item1, token.Item2));

            // OpponentPlayerTokens Copy
            TokenPlayer1 = new List<Tuple<int, int>>();
            foreach (Tuple<int, int> token in treeNode.TokenPlayer1)
                TokenPlayer1.Add(new Tuple<int, int>(token.Item1, token.Item2));

            GameIsFinished = treeNode.GameIsFinished;
        }

        public TreeNode(int[,] board, int currentPlayerValue)
        {
            GameIsFinished = false;
            Board = board;
            CurrentValue = currentPlayerValue;
            ListPossibleMove = new Dictionary<Tuple<int, int>, HashSet<Tuple<int, int>>>();

            // Simulation of token possession of players
            TokenPlayer0 = new List<Tuple<int, int>>();
            TokenPlayer1 = new List<Tuple<int, int>>();

            for (int x = 0; x < Board.GetLength(0); ++x)
            {
                for (int y = 0; y < Board.GetLength(1); ++y)
                {
                    if (Board[x, y] == 0)
                        TokenPlayer0.Add(new Tuple<int, int>(x, y));
                    else if (Board[x, y] == 1)
                        TokenPlayer1.Add(new Tuple<int, int>(x, y));
                }
            }

            UpdateListPossibleMove();
        }

        public List<Tuple<int, int>> Ops()
        {
            List<Tuple<int, int>> moves = ListPossibleMove.Keys.ToList<Tuple<int, int>>();
            return moves;
        }

        public TreeNode Apply(Tuple<int, int> move)
        {
            if (GameIsFinished)
            {
                Console.WriteLine("move : " + move);
                Show();
                throw new Exception("Can't play this move, the game is finished");
            }

            if (!ListPossibleMove.Keys.Contains(move))
            {
                Console.WriteLine("move : " + move);
                Show();
                throw new Exception("Can't play this move, not a possible move");
            }

            TreeNode copy = new TreeNode(this);

            foreach (Tuple<int, int> tokenToReverse in copy.ListPossibleMove[move])
            {
                copy.Board[tokenToReverse.Item1, tokenToReverse.Item2] = copy.CurrentValue;
                if (!copy.CurrentToken.Contains(tokenToReverse))
                    copy.CurrentToken.Add(tokenToReverse);
                if (copy.OpponentToken.Contains(tokenToReverse))
                    copy.OpponentToken.Remove(tokenToReverse);
            }

            copy.SwitchPlayer();

            if (copy.ListPossibleMove.Count <= 0) //turn skiped
            {
                copy.SwitchPlayer();
                if (copy.ListPossibleMove.Count <= 0)
                    copy.GameIsFinished = true;
            }

            return copy;
        }

        public void SwitchPlayer()
        {
            CurrentValue = CurrentValue == Player.Player0.Value ? Player.Player1.Value : Player.Player0.Value;
            UpdateListPossibleMove();
        }

        public void Show()
        {
            Console.WriteLine("turn " + CurrentValue);

            List<Tuple<int, int>> moves = ListPossibleMove.Keys.ToList<Tuple<int, int>>();
            for (int y = 0; y < Board.GetLength(1); ++y)
            {
                for (int x = 0; x < Board.GetLength(0); ++x)
                {
                    int value = Board[x, y];

                    if (moves.Contains(new Tuple<int, int>(x, y)))
                    {
                        if (CurrentValue == 0)
                            Console.Write("o");
                        else
                            Console.Write("x");
                    }
                    else
                    {
                        if (value >= 0)
                            Console.Write(value);
                        else
                            Console.Write("-");
                    }
                    Console.Write(" ");

                }
                Console.Write("\n");
            }
        }

        private void UpdateListPossibleMove()
        {
            ListPossibleMove.Clear();

            foreach (Tuple<int, int> tokenStart in CurrentToken)
            {
                foreach (Tuple<int, int> direction in directions)
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
                        if (toReverse.Count == 0 && valueOnBoardAtTokenPosition == EmptyValue || valueOnBoardAtTokenPosition == CurrentValue)
                        {
                            directionIsEligibleForAMove = false;
                            break;
                        }
                        else if (valueOnBoardAtTokenPosition == OpponentValue)
                        {
                            toReverse.Add(tokenPosition);
                        }
                        else if(valueOnBoardAtTokenPosition == EmptyValue)
                        {
                            toReverse.Add(tokenPosition);
                            break;
                        }
                        i++;
                    }

                    if (directionIsEligibleForAMove)
                    {
                        if (ListPossibleMove.Keys.Contains(tokenPosition))
                            ListPossibleMove[tokenPosition].UnionWith(toReverse);
                        else
                            ListPossibleMove.Add(tokenPosition, toReverse);
                    }
                }
            }
        }

        private bool BoardContains(Tuple<int, int> position)
        {
            return !(position.Item1 < 0 || position.Item2 < 0 || position.Item1 >= Board.GetLength(0) || position.Item2 >= Board.GetLength(1));
        }
        
        public bool Final()
        {
            return GameIsFinished;
        }

        public int Evaluate()
        {
            if (IsVictory())
                return int.MaxValue;
            if (IsDefeate())
                return int.MinValue;
            
            int heuristic = 0;

            heuristic = EvaluatePositionsWithPonderation();

            return heuristic;
        }

        private bool IsVictory()
        {
            return GameIsFinished && CurrentToken.Count > OpponentToken.Count;
        }

        private bool IsDefeate()
        {
            return GameIsFinished && CurrentToken.Count < OpponentToken.Count;
        }

        private int EvaluatePositionsWithPonderation()
        {
            Tuple<int, int> gridDim = new Tuple<int, int>(Board.GetLength(0), Board.GetLength(1));
            if (!PonderationGrids.ContainsKey(gridDim))
                throw new Exception("grid dim not handled for the AI");

            int[,] ponderationGrid = PonderationGrids[gridDim];
            int sum = 0;
            foreach (Tuple<int, int> pos in OpponentToken)
                sum -= ponderationGrid[pos.Item1, pos.Item2];
            foreach (Tuple<int, int> pos in CurrentToken)
                sum += ponderationGrid[pos.Item1, pos.Item2];
            return sum;
        }

        private int EvaluatePossibleMove()
        {
            UpdateListPossibleMove();
            int value = - ListPossibleMove.Count();
            UpdateListPossibleMove();
            value += ListPossibleMove.Count();
            return value;
        }

        private int EvaluateWeakBorder()
        {
            // Check if the opponent can take
            // the tokens on board of the current player

            int value = 0;

            /*
            foreach (Tuple<int, int> token in CurrentPlayerTokens)
            {
                if (token.Item1 == 0 || token.Item1 == Board.GetLength(0) - 1)
                {
                    bool emptyCase = false;
                    bool opponentToken = false;

                    for (int i = token.Item1 - 1; i >= 0; --i)
                    {
                        if (Board[i, token.Item2] == -1)
                        {
                            emptyCase = true;
                            i = 0;
                        }
                        else if (Board[i, token.Item2] != CurrentPlayerValue)
                        {
                            opponentToken = true;
                            i = 0;
                        }
                    }

                    for (int i = token.Item1 + 1; i < Board.GetLength(0); ++i)
                    {
                        if (Board[i, token.Item2] == -1)
                        {
                            emptyCase = true;
                            i = Board.GetLength(0);
                        }
                        else if (Board[i, token.Item2] != CurrentPlayerValue)
                        {
                            opponentToken = true;
                            i = Board.GetLength(0);
                        }
                    }

                    if (emptyCase && opponentToken)
                    {
                        value -= 5;
                    }
                }

                if (token.Item2 == 0 || token.Item2 == Board.GetLength(1) - 1)
                {
                    bool emptyCase = false;
                    bool opponentToken = false;

                    for (int i = token.Item2 - 1; i >= 0; --i)
                    {
                        if (Board[token.Item1, i] == -1)
                        {
                            emptyCase = true;
                            i = 0;
                        }
                        else if (Board[token.Item1, i] != CurrentPlayerValue)
                        {
                            opponentToken = true;
                            i = 0;
                        }
                    }

                    for (int i = token.Item2 + 1; i < Board.GetLength(1); ++i)
                    {
                        if (Board[token.Item1, i] == -1)
                        {
                            emptyCase = true;
                            i = Board.GetLength(1);
                        }
                        else if (Board[token.Item1, i] != CurrentPlayerValue)
                        {
                            opponentToken = true;
                            i = Board.GetLength(1);
                        }
                    }

                    if (emptyCase && opponentToken)
                    {
                        value -= 5;
                    }
                }
            }
            */
            return value;
        }
    }
}
