using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Timers;
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
        private const string LABEL_COL = "ABCDEFGHI";
        private const string LABEL_ROW = "1234567";

        private OthelloGridCell[,] othelloGridCells;

        private bool turnWhite;
        private Tuple<int, int> lastPlay;
        private Dictionary<Tuple<int, int>, HashSet<Tuple<int, int>>> currentPossibleMoves;


        Timer timerUpdateGui;

        Stopwatch swPlayerWhite;
        Stopwatch swPlayerBlack;

        public OthelloBoard()
        {
            InitializeComponent();

            timerUpdateGui = new Timer();
            timerUpdateGui.Interval = 0.1;
            timerUpdateGui.Elapsed += TimerUpdateGui_Elapsed;
            timerUpdateGui.Start();

            DataContext = LogicalBoard.GetInstance();

            NewGame();
        }

        private void NewGame()
        {
            swPlayerWhite = new Stopwatch();
            swPlayerBlack = new Stopwatch();
            othelloGridCells = new OthelloGridCell[LogicalBoard.GetInstance().GetCol(), LogicalBoard.GetInstance().GetRow()];

            turnWhite = false; //black start
            lastPlay = null;
            LogicalBoard.GetInstance().ResetGame();
            currentPossibleMoves = LogicalBoard.GetInstance().GetListPossibleMove(turnWhite);

            GenerateGrid();
            UpdateGui();
        }

        private void btnNewGame_Click(object sender, RoutedEventArgs e)
        {
            NewGame();
        }

        private void TimerUpdateGui_Elapsed(object sender, ElapsedEventArgs e)
        {
            try
            {
                Dispatcher.Invoke((Action)delegate ()
                {
                    UpdateTimers();
                });
            }
            catch
            {
                // try catch to avoid crash on windows close
            }
        }

        private void GenerateGrid()
        {
            //add row col labels
            graphicalBoard.ColumnDefinitions.Clear();
            for (int i = 0; i < LogicalBoard.GetInstance().GetCol() + 1; i++)
            {
                ColumnDefinition col = new ColumnDefinition();
                graphicalBoard.ColumnDefinitions.Add(col);
            }

            graphicalBoard.RowDefinitions.Clear();
            for (int i = 0; i < LogicalBoard.GetInstance().GetRow() + 1; i++)
            {
                RowDefinition row = new RowDefinition();
                graphicalBoard.RowDefinitions.Add(row);
            }

            for (int i = 0; i < LogicalBoard.GetInstance().GetCol(); i++)
            {
                OthelloGridLabel label = new OthelloGridLabel("" + LABEL_COL[i]);

                Grid.SetColumn(label, i + 1);
                Grid.SetRow(label, 0);

                graphicalBoard.Children.Add(label);
            }

            for (int i = 0; i < LogicalBoard.GetInstance().GetRow(); i++)
            {
                OthelloGridLabel label = new OthelloGridLabel("" + LABEL_ROW[i]);

                Grid.SetColumn(label, 0);
                Grid.SetRow(label, i + 1);

                graphicalBoard.Children.Add(label);
            }


            for (int x = 0; x < othelloGridCells.GetLength(0); x++)
            {
                for (int y = 0; y < othelloGridCells.GetLength(1); y++)
                {
                    OthelloGridCell cell = new OthelloGridCell(x, y);
                    othelloGridCells[x, y] = cell;

                    cell.Click += Cell_Click;

                    Grid.SetColumn(cell, x + 1);
                    Grid.SetRow(cell, y+1);

                    graphicalBoard.Children.Add(cell);
                }
            }
        }

        private void UpdateBoard()
        {
            currentPossibleMoves = LogicalBoard.GetInstance().GetListPossibleMove(turnWhite);
            int[,] lboard = LogicalBoard.GetInstance().GetBoard();
            for (int x = 0; x < othelloGridCells.GetLength(0); x++)
            {
                for (int y = 0; y < othelloGridCells.GetLength(1); y++)
                {
                    Tuple<int, int> pos = new Tuple<int, int>(x, y);
                    OthelloGridCell gcell = othelloGridCells[x, y];
                    
                    int lcell = lboard[x,y];
                    if (lcell == 1)
                        gcell.State = OthelloGridCell.States.Player1;
                    else if (lcell == 0)
                        gcell.State = OthelloGridCell.States.Player2;
                    else if(lcell == -1 && currentPossibleMoves.ContainsKey(pos))
                    {
                        if (turnWhite)
                            gcell.State = OthelloGridCell.States.PreviewPlayer1;
                        else
                            gcell.State = OthelloGridCell.States.PreviewPlayer2;
                    }
                    else
                        gcell.State = OthelloGridCell.States.Empty;

                    if (lastPlay != null)
                        gcell.LastPlay = pos.Item1 == lastPlay.Item1 && pos.Item2 == lastPlay.Item2;
                }
            }
        }

        private void Cell_Click(object sender, EventArgs e)
        {
            OthelloGridCell s = (OthelloGridCell)sender;
            int x = s.X;
            int y = s.Y;
            Tuple<int, int> pos = new Tuple<int, int>(x, y);
            if (currentPossibleMoves.ContainsKey(pos))
            {
                LogicalBoard.GetInstance().PlayMove(x, y, turnWhite);
                lastPlay = pos;
                NextTurn();
                UpdateGui();
            }
        }

        private void UpdateGui()
        {
            UpdateBoard();
            UpdateGameData();
        }

        public int ScoreBlack { get; set; }
        public int ScoreWhite { get; set; }

        private void UpdateGameData()
        {
            UpdateTimers();
            lblTurn.Content = turnWhite ? "blanc" : "noir";
            lblNbTokenBlack.GetBindingExpression(Label.ContentProperty).UpdateTarget();
            lblNbTokenWhite.GetBindingExpression(Label.ContentProperty).UpdateTarget();
        }

        private void UpdateTimers()
        {
            UpdateTimer(swPlayerBlack, lblTimeBlack);
            UpdateTimer(swPlayerWhite, lblTimeWhite);
        }

        private void UpdateTimer(Stopwatch sw, Label lbl)
        {
            TimeSpan remainingTime = TimeSpan.FromMilliseconds(sw.ElapsedMilliseconds);
            lbl.Content = remainingTime.ToString(@"mm\:ss\.fff");
        }

        private void NextTurn()
        {
            Console.WriteLine("------");
            foreach (Tuple<int, int> b in currentPossibleMoves.Keys)
            {
                Console.WriteLine(b);
            }

            turnWhite = !turnWhite;
            currentPossibleMoves = LogicalBoard.GetInstance().GetListPossibleMove(turnWhite);

            if (currentPossibleMoves.Count <= 0)
            {
                turnWhite = !turnWhite;
                currentPossibleMoves = LogicalBoard.GetInstance().GetListPossibleMove(turnWhite);
                if (currentPossibleMoves.Count <= 0)
                {
                    Console.WriteLine("Game End :" + currentPossibleMoves.Count);
                }
                else
                {
                    Console.WriteLine("Turn skiped :" + currentPossibleMoves.Count);
                }
            }
            else
            {
                Console.WriteLine("Next turn :" + currentPossibleMoves.Count);
            }

            foreach(Tuple<int, int>  b in currentPossibleMoves.Keys)
            {
                Console.WriteLine(b);
            }

            if (turnWhite)
            {
                swPlayerWhite.Start();
                swPlayerBlack.Stop();
            }
            else
            {
                swPlayerWhite.Stop();
                swPlayerBlack.Start();
            }
        }

        private void Board_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            int row = graphicalBoard.RowDefinitions.Count;
            int col = graphicalBoard.ColumnDefinitions.Count;

            double min = Math.Min(border.ActualHeight / row, border.ActualWidth / col);

            for (int i = 0; i < col; i++)
                graphicalBoard.ColumnDefinitions[i].Width = new GridLength(min, GridUnitType.Pixel);

            for (int i = 0; i < row; i++)
                graphicalBoard.RowDefinitions[i].Height = new GridLength(min, GridUnitType.Pixel);

            double w = (border.ActualWidth - min * col) / 2;
            double h = (border.ActualHeight - min * row) / 2;
            
            graphicalBoard.Margin = new Thickness(w, h, w, h);
        }

        private void btnSave_Click(object sender, RoutedEventArgs e)
        {
            SaveFileDialog saveFileDialog = new SaveFileDialog();
            saveFileDialog.Filter = "Othello Game Status|*.csv";
            saveFileDialog.Title = "Sauvegarder la partie d'Othello";
            saveFileDialog.ShowDialog();
            
            if (saveFileDialog.FileName != "")
            {
                System.IO.FileStream fs = (System.IO.FileStream)saveFileDialog.OpenFile();
                //todo
                fs.Close();
            }
        }

        private void btnLoad_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "Othello Game Status|*.csv";
            openFileDialog.Title = "Charger la partie d'Othello";
            openFileDialog.ShowDialog();

            if (openFileDialog.FileName != "")
            {
                System.IO.FileStream fs = (System.IO.FileStream)openFileDialog.OpenFile();
                //todo
                fs.Close();
            }
        }
    }
}
