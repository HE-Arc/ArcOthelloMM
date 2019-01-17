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
using System.Windows.Shapes;

namespace ArcOthelloMM
{
    /// <summary>
    /// Interaction logic for OthelloEndOfGame.xaml
    /// </summary>
    public partial class OthelloEndOfGame : Window
    {
        public OthelloEndOfGame(Player player)
        {
            InitializeComponent();
            //msg.Content = "Joueur " + player.Name + " a gagné !";
        }
    }
}
