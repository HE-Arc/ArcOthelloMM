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
    /// <summary>
    /// Interaction logic for OthelloGridCell.xaml
    /// </summary>
    public partial class OthelloGridCell : UserControl
    {
        public enum States
        {
            Empty,
            Player1,
            Player2,
            PreviewPlayer1,
            PreviewPlayer2
        }

        private States _state;

        public States State
        {
            get { return _state; }
            set
            {
                _state = value;
                tokenContent.Fill = colors[_state].Item1;
            }
        }

        public int X { get; }
        public int Y { get; }

        public event EventHandler Click;

        static Dictionary<States, Tuple<Brush, Brush>> colors;
        static Color colorPlayer1 = Color.FromRgb(255, 255, 255);
        static Color colorPlayer2 = Color.FromRgb(0, 0, 0);
        static byte opacityPreview = 63;
        static byte opacityNormal = 255;

        static OthelloGridCell()
        {
            colors = new Dictionary<States, Tuple<Brush, Brush>>();
            colors.Add(States.Empty, new Tuple<Brush, Brush>(BrushFromColor(colorPlayer1, 0), BrushFromColor(colorPlayer2, 0)));
            colors.Add(States.Player1, new Tuple<Brush, Brush>(BrushFromColor(colorPlayer1, opacityNormal), BrushFromColor(colorPlayer2, opacityNormal)));
            colors.Add(States.Player2, new Tuple<Brush, Brush>(BrushFromColor(colorPlayer2, opacityNormal), BrushFromColor(colorPlayer1, opacityNormal)));
            colors.Add(States.PreviewPlayer1, new Tuple<Brush, Brush>(BrushFromColor(colorPlayer1, opacityPreview), BrushFromColor(colorPlayer2, opacityPreview)));
            colors.Add(States.PreviewPlayer2, new Tuple<Brush, Brush>(BrushFromColor(colorPlayer2, opacityPreview), BrushFromColor(colorPlayer1, opacityPreview)));
        }

        static SolidColorBrush BrushFromColor(Color color, byte alpha)
        {
            return new SolidColorBrush(Color.FromArgb(alpha, color.R, color.G, color.B));
        }

        public OthelloGridCell(int x, int y, States states = States.Empty)
        {
            InitializeComponent();
            this.State = states;
            this.X = x;
            this.Y = y;
            button.Click += ButtonClick; // Adding the click on the parent of the button
        }
        
        private void ButtonClick(object sender, RoutedEventArgs e)
        {
            Click?.Invoke(this, e); // trigger the parent click event on the button (children) event trigger
        }
        
    }
}
