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

        LogicalBoard logicalBoard;

        OthelloGridCell[,] othelloGridCells;

        bool turn;

        public OthelloBoard()
        {
            InitializeComponent();
            logicalBoard = new LogicalBoard();
            othelloGridCells = new OthelloGridCell[NB_COL, NB_ROW];
            GenerateGrid();
            turn = false;
        }

        public void GenerateGrid()
        {
            for (int i = 0; i < NB_COL; i++)
            {
                ColumnDefinition col = new ColumnDefinition();
                graphicalBoard.ColumnDefinitions.Add(col);
            }

            for (int i = 0; i < NB_ROW; i++)
            {
                RowDefinition row = new RowDefinition();
                graphicalBoard.RowDefinitions.Add(row);
            }

            for (int x = 0; x < othelloGridCells.GetLength(0); x++)
            {
                for (int y = 0; y < othelloGridCells.GetLength(1); y++)
                {
                    OthelloGridCell cell = new OthelloGridCell(x, y);

                    cell.Click += Cell_Click;
                    cell.MouseEnter += Cell_MouseEnter;
                    cell.MouseLeave += Cell_MouseLeave;

                    Grid.SetRow(cell, y);
                    Grid.SetColumn(cell, x);

                    othelloGridCells[x, y] = cell;
                    graphicalBoard.Children.Add(cell);
                }
            }
        }

        private void Cell_MouseLeave(object sender, MouseEventArgs e)
        {
            OthelloGridCell s = (OthelloGridCell)sender;
            ClearPreview();
        }

        private void Cell_MouseEnter(object sender, MouseEventArgs e)
        {
            OthelloGridCell s = (OthelloGridCell)sender;
            if(s.State != OthelloGridCell.States.Player1 && s.State != OthelloGridCell.States.Player2)
                s.State = OthelloGridCell.States.PreviewPlayer1; //replace by player1 or 2 depending of the turn
        }

        private void Cell_Click(object sender, EventArgs e)
        {
            OthelloGridCell s = (OthelloGridCell)sender;
            s.State = OthelloGridCell.States.Player1; //replace by player1 or 2 depending of the turn
        }

        private void ClearPreview()
        {
            for (int x = 0; x < othelloGridCells.GetLength(0); x++)
            {
                for (int y = 0; y < othelloGridCells.GetLength(1); y++)
                {
                    OthelloGridCell cell = othelloGridCells[x, y];
                    if (cell.State == OthelloGridCell.States.PreviewPlayer1 || cell.State == OthelloGridCell.States.PreviewPlayer2)
                        cell.State = OthelloGridCell.States.Empty; // to replace with the actual value of the cell
                }
            }
        }

        private void Board_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            double min = Math.Min(border.ActualHeight / NB_ROW, border.ActualWidth / NB_COL);

            for (int i = 0; i < NB_COL; i++)
                graphicalBoard.ColumnDefinitions[i].Width = new GridLength(min, GridUnitType.Pixel);

            for (int i = 0; i < NB_ROW; i++)
                graphicalBoard.RowDefinitions[i].Height = new GridLength(min, GridUnitType.Pixel);

            double w = (border.ActualWidth - min * NB_COL) / 2;
            double h = (border.ActualHeight - min * NB_ROW) / 2;
            
            graphicalBoard.Margin = new Thickness(w, h, w, h);
        }
    }
}
