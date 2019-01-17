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
using System.Windows.Media.Animation;
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
                tokenContent.Fill = colors[_state];
                tokenContent.Width = sizes[_state];
                tokenContent.Height = sizes[_state];
            }
        }

        public int X { get; }
        public int Y { get; }

        private bool _lastPlay;
        public bool LastPlay
        {
            get
            {
                return _lastPlay;
            }
            set
            {
                _lastPlay = value;
                tokenContent.Stroke = _lastPlay ? borderColorEllipseLastPlay : transparent;
            }
        }

        public event EventHandler Click;

        static Dictionary<States, Brush> colors;
        static Dictionary<States, int> sizes;

        static Brush borderColorEllipseLastPlay= BrushFromColor(Color.FromRgb(111, 255, 97), 255);
        static Brush transparent = BrushFromColor(Color.FromRgb(0, 0, 0), 0);

        static Brush backgroundColor = BrushFromColor(Color.FromRgb(0, 0, 0), 127);
        static Brush borderColor = BrushFromColor(Color.FromRgb(255, 255, 255), 127);

        static int normalSize = 100;
        static byte normalOpacity = 220;

        static int previewSize = (int)(0.75*normalSize);
        static byte previewOpacity = (byte)(0.25 * normalOpacity);


        static OthelloGridCell()
        {
            colors = new Dictionary<States, Brush>();
            colors.Add(States.Empty, BrushFromColor(Color.FromRgb(0,0,0), 0));
            colors.Add(States.Player1, BrushFromColor(Player.WhitePlayer.Color2, normalOpacity));
            colors.Add(States.Player2, BrushFromColor(Player.BlackPlayer.Color2, normalOpacity));
            colors.Add(States.PreviewPlayer1, BrushFromColor(Player.WhitePlayer.Color2, previewOpacity));
            colors.Add(States.PreviewPlayer2, BrushFromColor(Player.BlackPlayer.Color2, previewOpacity));
            sizes = new Dictionary<States, int>();
            sizes.Add(States.Empty, 0);
            sizes.Add(States.Player1, normalSize);
            sizes.Add(States.Player2, normalSize);
            sizes.Add(States.PreviewPlayer1, previewSize);
            sizes.Add(States.PreviewPlayer2, previewSize);
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
            this.LastPlay = false;
            button.Background = backgroundColor;
            button.BorderBrush = borderColor;
            button.Click += ButtonClick; // Adding the click on the parent of the button
        }

        private void ButtonClick(object sender, RoutedEventArgs e)
        {
            Click?.Invoke(this, e); // trigger the parent click event on the button (children) event trigger
        }
    }
}
