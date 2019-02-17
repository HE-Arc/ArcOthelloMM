using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace ArcOthelloMM
{
    public partial class OthelloGridLabel : UserControl
    {
        /// <summary>
        /// Create a label for the othello grid
        /// </summary>
        /// <param name="content"></param>
        public OthelloGridLabel(String content)
        {
            InitializeComponent();
            text.Content = content;
        }
    }
}
