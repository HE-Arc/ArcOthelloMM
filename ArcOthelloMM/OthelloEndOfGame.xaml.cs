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
        /// <summary>
        /// Create a windows that show who's the winner
        /// </summary>
        /// <param name="playerWin"></param>
        /// <param name="playerLose"></param>
        public OthelloEndOfGame(Player playerWin, Player playerLose)
        {
            InitializeComponent();
            if (playerWin != null && playerLose != null)
            {
                GradientStopCollection gradientStops = new GradientStopCollection();
                gradientStops.Add(new GradientStop(playerLose.Color1, 0));
                gradientStops.Add(new GradientStop(playerWin.Color1, 0.30));
                RadialGradientBrush radialGradientBrush = new RadialGradientBrush(gradientStops);
                Background = radialGradientBrush;
                msg.Content = "Joueur " + playerWin.Name + " a gagné !";
            }
            else
            {
                GradientStopCollection gradientStops = new GradientStopCollection();
                gradientStops.Add(new GradientStop(Player.BlackPlayer.Color1, 0));
                gradientStops.Add(new GradientStop(Player.WhitePlayer.Color1, 1));
                LinearGradientBrush radialGradientBrush = new LinearGradientBrush(gradientStops, 90.0);
                Background = radialGradientBrush;
                msg.Content = "Egalité !";
            }
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
