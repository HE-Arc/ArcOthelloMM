using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IAMargueronMottier
{
    class TreeNode
    {
        public int Eval()
        {
            return 0;
        }

        public bool Final()
        {
            return true;
        }

        public List<Tuple<int, int>> Ops()
        {
            return new List<Tuple<int, int>>();
        }

        public TreeNode Apply(Tuple<int, int> op)
        {
            return new TreeNode();
        }
    }
}
