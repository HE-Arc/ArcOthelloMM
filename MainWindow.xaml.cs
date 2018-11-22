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
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private const int ROW = 7;
        private const int COLUMN = 9;
        private const int SIZE_PIECE = 30;
        private int[,] board;

        public MainWindow()
        {
            InitializeComponent();

            initBoard();
            initGame();
        }

        private void initBoard()
        {
            board = new int[ROW, COLUMN];

            for (int i = 0; i < ROW; ++i)
            {
                for (int j = 0; j < COLUMN; ++j)
                {

                    board[i, j] = 0;
                }
            }
        }

        private void initGame()
        {
            addPiece(r33, false);
            addPiece(r34, true);
            addPiece(r43, true);
            addPiece(r44, false);
        }

        private void addPiece(Rectangle rec, bool black)
        {
            Ellipse circle = new Ellipse
            {
                Name = "c" + rec.Name.Substring(1),
                HorizontalAlignment = HorizontalAlignment.Left,
                VerticalAlignment = VerticalAlignment.Top,
                Width = SIZE_PIECE,
                Height = SIZE_PIECE,
                Fill = (black) ? Brushes.Black : Brushes.White,
                Margin = new Thickness(rec.Margin.Left + 5, rec.Margin.Top + 5, 0, 0)
            };

            if (black)
            {
                circle.Fill = Brushes.Black;
                placeOnBoard(1, rec.Name);
            }
            else
            {
                circle.Fill = Brushes.White;
                placeOnBoard(2, rec.Name);
            }

            grid.Children.Add(circle);
        }

        private void pickPiece(string name)
        {
            Ellipse circle = (Ellipse)grid.FindName(name);

            if (circle.Fill == Brushes.Black)
            {
                circle.Fill = Brushes.Black;
                placeOnBoard(1, circle.Name);
            }
            else
            {
                circle.Fill = Brushes.White;
                placeOnBoard(2, circle.Name);
            }
        }

        private void placeOnBoard(int value, string position)
        {
            board[int.Parse(position[1].ToString()), int.Parse(position[2].ToString())] = value;
        }
    }
}
