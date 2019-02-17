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

        private Dictionary<Tuple<int, int>, HashSet<Tuple<int, int>>> ListPossibleMoves { get; set; }

        private bool GameIsFinished;

        private static readonly Dictionary<Tuple<int, int>, int[,]> PonderationGrids;
        private static readonly List<Tuple<int, int>> Directions;
        private static readonly Dictionary<string, List<Tuple<int, int>>> Borders;

        /// <summary>
        /// Static field initalization
        /// </summary>
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
            Directions = new List<Tuple<int, int>>
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

            Borders = new Dictionary<string, List<Tuple<int, int>>>
            {
                {
                    "Top",
                    new List<Tuple<int, int>>
                    {
                        new Tuple<int, int>(0, 2),
                        new Tuple<int, int>(0, 3),
                        new Tuple<int, int>(0, 4),
                        new Tuple<int, int>(0, 5),
                        new Tuple<int, int>(0, 6),
                        new Tuple<int, int>(0, 7)
                    }
                },
                {
                    "Bottom",
                    new List<Tuple<int, int>>
                    {
                        new Tuple<int, int>(6, 2),
                        new Tuple<int, int>(6, 3),
                        new Tuple<int, int>(6, 4),
                        new Tuple<int, int>(6, 5),
                        new Tuple<int, int>(6, 6),
                        new Tuple<int, int>(6, 7)
                    }
                },
                {
                    "Left",
                    new List<Tuple<int, int>>
                    {
                        new Tuple<int, int>(2, 0),
                        new Tuple<int, int>(3, 0),
                        new Tuple<int, int>(4, 0),
                        new Tuple<int, int>(5, 0)
                    }
                },
                {
                    "Right",
                    new List<Tuple<int, int>>
                    {
                        new Tuple<int, int>(2, 8),
                        new Tuple<int, int>(3, 8),
                        new Tuple<int, int>(4, 8),
                        new Tuple<int, int>(5, 8)
                    }
                }
            };

            //TestMoveDetection();
        }

        /// <summary>
        /// Test method for apply function
        /// </summary>
        public static void TestMoveDetection()
        {
            ExempleMoveNormal();
            Console.WriteLine("-----------");
            ExempleMoveSkipTurn();
            Console.WriteLine("-----------");
            ExempleMoveFinishGame();
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

        private static void ExempleMoveSkipTurn()
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

        private static void ExempleMoveFinishGame()
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

        /// <summary>
        /// Copy constructor
        /// </summary>
        /// <param name="treeNode"></param>
        public TreeNode(TreeNode treeNode)
        {
            // ListPossibleMove Copy
            ListPossibleMoves = new Dictionary<Tuple<int, int>, HashSet<Tuple<int, int>>>();
            foreach (KeyValuePair<Tuple<int, int>, HashSet<Tuple<int, int>>> entry in treeNode.ListPossibleMoves)
                ListPossibleMoves.Add(new Tuple<int, int>(entry.Key.Item1, entry.Key.Item2), new HashSet<Tuple<int, int>>(entry.Value));

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

        /// <summary>
        /// Constructor for the root of the alphabeta
        /// </summary>
        /// <param name="board"></param>
        /// <param name="currentPlayerValue"></param>
        public TreeNode(int[,] board, int currentPlayerValue)
        {
            GameIsFinished = false;
            Board = board;
            CurrentValue = currentPlayerValue;
            ListPossibleMoves = new Dictionary<Tuple<int, int>, HashSet<Tuple<int, int>>>();

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

            UpdateListPossibleMoves();
        }

        /// <summary>
        /// Return a list of possible moves at this TreeNode (game state)
        /// </summary>
        /// <returns></returns>
        public List<Tuple<int, int>> Ops()
        {
            List<Tuple<int, int>> moves = ListPossibleMoves.Keys.ToList<Tuple<int, int>>();
            return moves;
        }

        /// <summary>
        /// Return a TreeNode from the current node after playing a move
        /// </summary>
        /// <param name="move"></param>
        /// <returns></returns>
        public TreeNode Apply(Tuple<int, int> move)
        {
            if (GameIsFinished)
            {
                Console.WriteLine("move : " + move);
                Show();
                throw new Exception("Can't play this move, the game is finished");
            }

            if (!ListPossibleMoves.Keys.Contains(move))
            {
                Console.WriteLine("move : " + move);
                Show();
                throw new Exception("Can't play this move, not a possible move");
            }

            TreeNode copy = new TreeNode(this);

            foreach (Tuple<int, int> tokenToReverse in copy.ListPossibleMoves[move])
            {
                copy.Board[tokenToReverse.Item1, tokenToReverse.Item2] = copy.CurrentValue;
                if (!copy.CurrentToken.Contains(tokenToReverse))
                    copy.CurrentToken.Add(tokenToReverse);
                if (copy.OpponentToken.Contains(tokenToReverse))
                    copy.OpponentToken.Remove(tokenToReverse);
            }

            copy.SwitchPlayer();

            if (copy.ListPossibleMoves.Count <= 0) //turn skiped
            {
                copy.SwitchPlayer();
                if (copy.ListPossibleMoves.Count <= 0)
                    copy.GameIsFinished = true;
            }

            return copy;
        }

        /// <summary>
        /// Switch the turn of the player in this node (also update his move list)
        /// </summary>
        public void SwitchPlayer()
        {
            CurrentValue = CurrentValue == Player.Player0.Value ? Player.Player1.Value : Player.Player0.Value;
            UpdateListPossibleMoves();
        }

        /// <summary>
        /// Debug function used to show the TreeNode (game state) in the console
        /// </summary>
        public void Show()
        {
            Console.WriteLine("turn " + CurrentValue);

            List<Tuple<int, int>> moves = ListPossibleMoves.Keys.ToList<Tuple<int, int>>();
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

        /// <summary>
        /// Find every possible move in the current game state and store them into ListPossibleMoves
        /// </summary>
        private void UpdateListPossibleMoves()
        {
            ListPossibleMoves.Clear();

            foreach (Tuple<int, int> tokenStart in CurrentToken)
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
                        if (toReverse.Count == 0 && valueOnBoardAtTokenPosition == EmptyValue || valueOnBoardAtTokenPosition == CurrentValue)
                        {
                            directionIsEligibleForAMove = false;
                            break;
                        }
                        else if (valueOnBoardAtTokenPosition == OpponentValue)
                        {
                            toReverse.Add(tokenPosition);
                        }
                        else if (valueOnBoardAtTokenPosition == EmptyValue)
                        {
                            toReverse.Add(tokenPosition);
                            break;
                        }
                        i++;
                    }

                    if (directionIsEligibleForAMove)
                    {
                        if (ListPossibleMoves.Keys.Contains(tokenPosition))
                            ListPossibleMoves[tokenPosition].UnionWith(toReverse);
                        else
                            ListPossibleMoves.Add(tokenPosition, toReverse);
                    }
                }
            }
        }

        /// <summary>
        /// Return true if the postion specified is inside the board
        /// </summary>
        /// <param name="position"></param>
        /// <returns></returns>
        private bool BoardContains(Tuple<int, int> position)
        {
            return !(position.Item1 < 0 || position.Item2 < 0 || position.Item1 >= Board.GetLength(0) || position.Item2 >= Board.GetLength(1));
        }

        /// <summary>
        /// Is the game finished? State updated when calling the Apply function (cant detect finished game on the root)
        /// </summary>
        /// <returns></returns>
        public bool Final()
        {
            return GameIsFinished;
        }

        /// <summary>
        /// Return an heuristic of "How good is the current game state for the specified player"
        /// The greater the return value the greater he has chance of winning
        /// </summary>
        /// <param name="playerValue"></param>
        /// <returns></returns>
        public int Evaluate(int playerValue)
        {
            if (IsVictory())
                return int.MaxValue;
            if (IsDefeate())
                return int.MinValue;

            int heuristic = 0;

            heuristic = EvaluatePositionsWithPonderation();

            if (playerValue != CurrentValue)
                heuristic *= -1;

            return heuristic;
        }

        /// <summary>
        /// Return true if it's a won state for the current player 
        /// </summary>
        /// <returns></returns>
        private bool IsVictory()
        {
            return GameIsFinished && CurrentToken.Count > OpponentToken.Count;
        }

        /// <summary>
        /// Return true if it's a lose state for the current player 
        /// </summary>
        /// <returns></returns>
        private bool IsDefeate()
        {
            return GameIsFinished && CurrentToken.Count < OpponentToken.Count;
        }

        /// <summary>
        /// Evaluate the state for the current player with a weighted sum
        /// EvaluateBorderFive modify the ponderation grid to optimize it depending of board configuration
        /// </summary>
        /// <returns></returns>
        private int EvaluatePositionsWithPonderation()
        {
            Tuple<int, int> gridDim = new Tuple<int, int>(Board.GetLength(0), Board.GetLength(1));
            if (!PonderationGrids.ContainsKey(gridDim))
                throw new Exception("grid dim not handled for the AI");

            int[,] ponderationGrid = new int[Board.GetLength(0), Board.GetLength(1)];
            Array.Copy(PonderationGrids[gridDim], ponderationGrid, PonderationGrids[gridDim].Length);

            EvaluateBorderFive(ponderationGrid);

            int sum = 0;
            foreach (Tuple<int, int> pos in OpponentToken)
                sum -= ponderationGrid[pos.Item1, pos.Item2];
            foreach (Tuple<int, int> pos in CurrentToken)
                sum += ponderationGrid[pos.Item1, pos.Item2];
            return sum;
        }

        /// <summary>
        /// todo
        /// </summary>
        /// <param name="ponderationGrid"></param>
        private void EvaluateBorderFive(int[,] ponderationGrid)
        {
            Tuple<int, int> token00 = new Tuple<int, int>(0, 0);
            Tuple<int, int> token01 = new Tuple<int, int>(0, 1);
            Tuple<int, int> token05 = new Tuple<int, int>(0, 5);
            Tuple<int, int> token06 = new Tuple<int, int>(0, 6);
            Tuple<int, int> token10 = new Tuple<int, int>(1, 0);
            Tuple<int, int> token11 = new Tuple<int, int>(1, 1);
            Tuple<int, int> token15 = new Tuple<int, int>(1, 5);
            Tuple<int, int> token16 = new Tuple<int, int>(1, 6);
            Tuple<int, int> token70 = new Tuple<int, int>(7, 0);
            Tuple<int, int> token71 = new Tuple<int, int>(7, 1);
            Tuple<int, int> token75 = new Tuple<int, int>(7, 5);
            Tuple<int, int> token76 = new Tuple<int, int>(7, 6);
            Tuple<int, int> token80 = new Tuple<int, int>(8, 0);
            Tuple<int, int> token81 = new Tuple<int, int>(8, 1);
            Tuple<int, int> token85 = new Tuple<int, int>(8, 5);
            Tuple<int, int> token86 = new Tuple<int, int>(8, 6);

            if (OpponentToken.Contains(token00)
                && (
                    ListPossibleMoves.Keys.Contains(token10)
                    && (ListPossibleMoves[token10].Contains(token11) || CurrentToken.Contains(token11))
                    && Borders["Top"].Intersect(OpponentToken).Any()
                    && OpponentToken.Contains(token70)
                    && !OpponentToken.Contains(token80)
                    || ListPossibleMoves.Keys.Contains(token01)
                    && (ListPossibleMoves[token01].Contains(token11) || CurrentToken.Contains(token11))
                    && Borders["Left"].Intersect(OpponentToken).Any()
                    && OpponentToken.Contains(token05)
                    && !OpponentToken.Contains(token06)
                    )
                )
            {
                ponderationGrid[1, 1] = 200;
            }

            
            if (OpponentToken.Contains(token06)
                && (
                    ListPossibleMoves.Keys.Contains(token05)
                    && (ListPossibleMoves[token05].Contains(token15) || CurrentToken.Contains(token15))
                    && Borders["Left"].Intersect(OpponentToken).Any()
                    && OpponentToken.Contains(token01)
                    && !OpponentToken.Contains(token00)
                    || ListPossibleMoves.Keys.Contains(token16)
                    && (ListPossibleMoves[token16].Contains(token15) || CurrentToken.Contains(token15))
                    && Borders["Bottom"].Intersect(OpponentToken).Any()
                    && OpponentToken.Contains(token76)
                    && !OpponentToken.Contains(token86)
                    )
                )
            {
                ponderationGrid[1, 5] = 200;
            }

            
            if (
                OpponentToken.Contains(token80)
                && (
                    ListPossibleMoves.Keys.Contains(token70)
                    && (ListPossibleMoves[token70].Contains(token71) || CurrentToken.Contains(token71))
                    && Borders["Top"].Intersect(OpponentToken).Any()
                    && OpponentToken.Contains(token10)
                    && !OpponentToken.Contains(token00)
                    || ListPossibleMoves.Keys.Contains(token81)
                    && (ListPossibleMoves[token81].Contains(token71) || CurrentToken.Contains(token71))
                    && Borders["Right"].Intersect(OpponentToken).Any()
                    && OpponentToken.Contains(token85)
                    && !OpponentToken.Contains(token86)
                    )
                )
            {
                ponderationGrid[7, 1] = 200;
            }

            if (
                OpponentToken.Contains(token86)
                && (
                    ListPossibleMoves.Keys.Contains(token85)
                    && (ListPossibleMoves[token85].Contains(token75) || CurrentToken.Contains(token75))
                    && Borders["Right"].Intersect(OpponentToken).Any()
                    && OpponentToken.Contains(token81)
                    && !OpponentToken.Contains(token80)
                    || ListPossibleMoves.Keys.Contains(token76)
                    && (ListPossibleMoves[token76].Contains(token75) || CurrentToken.Contains(token75))
                    && Borders["Bottom"].Intersect(OpponentToken).Any()
                    && OpponentToken.Contains(token16)
                    && !OpponentToken.Contains(token06)
                    )
                )
            {
                ponderationGrid[7, 5] = 200;
            }
        }
    }
}
