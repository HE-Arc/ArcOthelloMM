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
    /// Interaction logic for OthelloBoard.xaml
    /// </summary>
    public partial class OthelloBoard : Window
    {
        const int NB_COL = 9;
        const int NB_ROW = 7;

        public OthelloBoard()
        {
            InitializeComponent();
            GenerateGrid();
        }

        public void GenerateGrid()
        {
            for (int i = 0; i < NB_COL; i++)
            {
                ColumnDefinition col = new ColumnDefinition();
                board.ColumnDefinitions.Add(col);
            }

            for (int i = 0; i < NB_ROW; i++)
            {
                RowDefinition row = new RowDefinition();
                board.RowDefinitions.Add(row);
            }

            for (int x = 0; x < NB_COL; x++)
            {
                for (int y = 0; y < NB_ROW; y++)
                {
                    Button txt1 = new Button();
                    txt1.Content = "x: " + x + "y: " + y;
                    txt1.FontSize = 20;
                    txt1.HorizontalAlignment = HorizontalAlignment.Stretch;
                    txt1.VerticalAlignment = VerticalAlignment.Stretch;

                    Grid.SetRow(txt1, y);
                    Grid.SetColumn(txt1, x);
                    board.Children.Add(txt1);
                }
            }
        }

        private void board_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            board.Margin = new Thickness(0);
            double width;
            double height;
            double ratio = NB_ROW / NB_COL;
            if (board.ActualWidth * NB_ROW > board.ActualHeight * NB_COL)
            {
                width = 0;
                double newH = board.ActualHeight * ratio;
                height = (board.ActualHeight - newH) / 2;
            }
            else
            {
                double newW = board.ActualWidth / ratio;
                height = (board.ActualWidth - newW) / 2;
                width = 0;
                height = 0;
            }
            board.Margin = new Thickness(width, height, width, height);
            Console.WriteLine("" + width + ", " + "" + height);
        }
    }
}
