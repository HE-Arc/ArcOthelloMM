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
        Dictionary<Tuple<int, int>, HashSet<Tuple<int, int>>> currentPossibleMoves;

        public OthelloBoard()
        {
            InitializeComponent();
            logicalBoard = new LogicalBoard();
            othelloGridCells = new OthelloGridCell[NB_COL, NB_ROW];
            GenerateGrid();
            turn = false;
            UpdateBoard();
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
                    //cell.MouseEnter += Cell_MouseEnter;
                    //cell.MouseLeave += Cell_MouseLeave;

                    Grid.SetRow(cell, y);
                    Grid.SetColumn(cell, x);

                    othelloGridCells[x, y] = cell;
                    graphicalBoard.Children.Add(cell);
                }
            }
        }

        private void UpdateBoard()
        {
            currentPossibleMoves = logicalBoard.listMove;
            for (int x = 0; x < othelloGridCells.GetLength(0); x++)
            {
                for (int y = 0; y < othelloGridCells.GetLength(1); y++)
                {
                    Tuple<int, int> pos = new Tuple<int, int>(x, y);
                    OthelloGridCell gcell = othelloGridCells[x, y];
                    int[,] lboard = logicalBoard.GetBoard();
                    int lcell = lboard[x,y];
                    if (lcell == -1)
                        gcell.State = OthelloGridCell.States.Empty;
                    else if (lcell == 0)
                        gcell.State = OthelloGridCell.States.Player1;
                    else if (lcell == 1)
                        gcell.State = OthelloGridCell.States.Player2;

                    if(currentPossibleMoves.ContainsKey(pos))
                    {
                        if (turn)
                            gcell.State = OthelloGridCell.States.PreviewPlayer1;
                        else
                            gcell.State = OthelloGridCell.States.PreviewPlayer2;
                    }
                }
            }
        }

        private void Cell_MouseLeave(object sender, MouseEventArgs e)
        {

        }

        private void Cell_MouseEnter(object sender, MouseEventArgs e)
        {
            
        }

        private void Cell_Click(object sender, EventArgs e)
        {
            OthelloGridCell s = (OthelloGridCell)sender;
            int x = s.X;
            int y = s.Y;
            Tuple<int, int> pos = new Tuple<int, int>(x, y);
            if (currentPossibleMoves.ContainsKey(pos))
            {
                logicalBoard.PlayMove(x, y, turn);
                ChangeTurn();
                UpdateGui();
            }
        }

        private void UpdateGui()
        {
            UpdateBoard();
        }

        private void ChangeTurn()
        {
            turn = !turn;
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
