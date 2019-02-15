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
        private int AiMinMaxPlayerValue;

        static IA()
        {
            random = new Random();
        }

        private int GetMinOrMax(TreeNode treeNode)
        {
            if (treeNode.CurrentPlayerValue == AiMinMaxPlayerValue)
                return 1; // it's your turn maximize your outcome
            else
                return -1; //it's his turn, he will minimize your outcome
        }

        public Tuple<int, int> GetNextMove(int[,] game, int level, bool whiteTurn)
        {
            AiMinMaxPlayerValue = (whiteTurn) ? Player.Player1.Value : Player.Player0.Value;

            TreeNode root = new TreeNode(game, AiMinMaxPlayerValue);
            root.Show();
            return AlphaBeta(root, level, GetMinOrMax(root), int.MinValue).Item2;

            //return StupidAI(game, level, whiteTurn);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="root"></param>
        /// <param name="level"></param>
        /// <param name="minOrMax">1 => maximize, -1 => minimize</param>
        /// <param name="parentValue"></param>
        /// <returns></returns>
        private Tuple<int, Tuple<int, int>> AlphaBeta(TreeNode root, int level, int minOrMax, int parentValue)
        {

            Console.WriteLine("Level : " + level + ", isFinal : " + root.Final());

            if (level <= 0 || root.Final())
                return new Tuple<int, Tuple<int, int>>(root.Evaluate(), null);

            int optVal = minOrMax * int.MinValue;
            Tuple<int, int> optOp = null;

            List<Tuple<int, int>> ops = root.Ops();

            foreach (Tuple<int, int> op in ops)
            {
                TreeNode newNode = root.Apply(op);
                newNode.Show();
                //maximize or minimaze depending of the turn
                Tuple<int, Tuple<int, int>> res = AlphaBeta(newNode, level - 1, GetMinOrMax(newNode), optVal);

                if (res.Item1 * minOrMax > optVal * minOrMax)
                {
                    optVal = res.Item1;
                    optOp = op;

                    //if (res.Item1 * minOrMax > parentValue * minOrMax)
                      //  break;
                }
            }

            return new Tuple<int, Tuple<int, int>>(root.Evaluate(), optOp);
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