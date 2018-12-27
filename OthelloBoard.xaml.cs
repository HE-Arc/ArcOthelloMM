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
                    txt1.Content = "x";
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
            double min = Math.Min(border.ActualHeight / NB_ROW, border.ActualWidth / NB_COL);

            for (int i = 0; i < NB_COL; i++)
                board.ColumnDefinitions[i].Width = new GridLength(min, GridUnitType.Pixel);

            for (int i = 0; i < NB_ROW; i++)
                board.RowDefinitions[i].Height = new GridLength(min, GridUnitType.Pixel);

            double w = (border.ActualWidth - min * NB_COL) / 2;
            double h = (border.ActualHeight - min * NB_ROW) / 2;
            
            board.Margin = new Thickness(w, h, w, h);
        }
    }
}
