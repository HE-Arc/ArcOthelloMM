using IAMargueronMottier;
using System;
using System.Collections.Generic;

namespace ArcOthelloMM
{
    class IA
    {
        /// <summary>
        /// Property for singleton
        /// </summary>
        private static IA instance;
        public static IA Instance
        {
            get
            {
                if (instance == null)
                    instance = new IA();
                return instance;
            }

            set
            {
                instance = value;
            }
        }

        private static Random random;
        private int CurrentPlayerValue;

        static IA()
        {
            random = new Random();
        }

        public Tuple<int, int> GetNextMove(int[,] game, int level, bool whiteTurn)
        {
            CurrentPlayerValue = (whiteTurn) ? Player.Player1.Value : Player.Player0.Value;

            return AlphaBeta(new TreeNode(game, CurrentPlayerValue), level, 1, int.MinValue).Item2;
            //return StupidAI(game, level, whiteTurn);
        }

        private Tuple<int, Tuple<int, int>> AlphaBeta(TreeNode root, int level, int minOrMax, int parentValue)
        {
            // minOrMax = 1 => maximize
            // minOrMax = -1 => minimize

            Console.WriteLine("Level : " + level + ", isFinal : " + root.Final());

            if (level <= 0 || root.Final())
                return new Tuple<int, Tuple<int, int>>(root.Evaluate(), null);

            int optVal = minOrMax * int.MinValue;
            Tuple<int, int> optOp = null;

            List<Tuple<int, int>> ops = root.Ops();
            Console.WriteLine("Possibles moves : ");
            ShowMoves(ops);
            Console.WriteLine("Total : " + ops.Count);

            // Loop on key of possible move
            foreach (Tuple<int, int> op in ops)
            {
                TreeNode newNode = root.Apply(op);
                Console.WriteLine("Selected exploration mode : " + op);
                Tuple<int, Tuple<int, int>> res = AlphaBeta(newNode, level - 1, minOrMax * -1, optVal);
                Console.WriteLine("Current move : " + op);
                ShowBoard(newNode.Board);
                Console.WriteLine("Score : " + res.Item1);
                Console.WriteLine("");

                if (res.Item1 * minOrMax > optVal * minOrMax)
                {
                    optVal = res.Item1;
                    optOp = op;

                    if (res.Item1 * minOrMax > parentValue * minOrMax)
                        break;
                }
            }

            return new Tuple<int, Tuple<int, int>>(root.Evaluate(), optOp);
        }


        private static void ShowMoves(List<Tuple<int, int>> moves)
        {
            foreach (Tuple<int, int> move in moves)
            {
                Console.WriteLine(move);
            }
        }

        private static void ShowBoard(int[,] Board)
        {
            for (int y = 0; y < Board.GetLength(1); ++y)
            {
                for (int x = 0; x < Board.GetLength(0); ++x)
                {
                    int value = Board[x, y];
                    if (value >= 0)
                        Console.Write(value + " ");
                    else
                        Console.Write("- ");
                }
                Console.Write("\n");
            }
        }

        public Tuple<int, int> StupidAI(int[,] game, int level, bool whiteTurn)
        {
            Dictionary<Tuple<int, int>, HashSet<Tuple<int, int>>> moves = LogicalBoard.Instance.CurrentPossibleMoves;
            List<Tuple<int, int>> keys = new List<Tuple<int, int>>(moves.Keys);
            int move = random.Next(moves.Count);
            return keys[move];
        }
    }
}