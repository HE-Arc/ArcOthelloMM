using ArcOthelloMM;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IAMargueronMottier
{
    class TreeNode
    {
        private List<Tuple<int, int>> ListMove { get; set; }
        private int[,] Board { get; set; }

        public TreeNode(int[,] board)
        {
            Board = board;
            ListMove = new List<Tuple<int, int>>();
        }

        public int Evaluate()
        {
            return 0;
        }

        public bool Final()
        {
            return false;
        }

        public List<Tuple<int, int>> Ops()
        {
            return ListMove;
        }

        public TreeNode Apply(Tuple<int, int> tuple, bool isWhite)
        {
            LogicalBoard.Instance.PlayMove(tuple.Item1, tuple.Item2, isWhite);
            return new TreeNode(LogicalBoard.Instance.GetBoard());
        }
    }
}
