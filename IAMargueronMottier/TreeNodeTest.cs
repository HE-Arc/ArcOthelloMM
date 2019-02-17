using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IAMargueronMottier
{
    class TreeNodeTest
    {
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
    }
}
