﻿using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
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
        private OthelloGridCell[,] othelloGridCells;

        private bool turnWhite;
        private bool playerVsPlayer;
        private Dictionary<Tuple<int, int>, HashSet<Tuple<int, int>>> currentPossibleMoves;

        Timer timerUpdateGui;

        public OthelloBoard()
        {
            InitializeComponent();

            timerUpdateGui = new Timer();
            timerUpdateGui.Interval = 0.1;
            timerUpdateGui.Elapsed += TimerUpdateGui_Elapsed;
            timerUpdateGui.Start();

            DataContext = LogicalBoard.Instance;

            GenerateGrid();
        }

        private void NewGame(bool playerVsPlayer)
        {
            turnWhite = false; //black start
            this.playerVsPlayer = playerVsPlayer;
            LogicalBoard.Instance.ResetGame();
            currentPossibleMoves = LogicalBoard.Instance.GetListPossibleMove(turnWhite);

            UpdateGui();
        }

        private void btnNewGame_Click(object sender, RoutedEventArgs e)
        {
            NewGame(true);
        }

        private void btnNewGameAI_Click(object sender, RoutedEventArgs e)
        {
            NewGame(false);
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
            othelloGridCells = new OthelloGridCell[LogicalBoard.Instance.GetCol(), LogicalBoard.Instance.GetRow()];

            graphicalBoard.ColumnDefinitions.Clear();
            graphicalBoard.RowDefinitions.Clear();
            graphicalBoard.Children.Clear();


            //define columns
            for (int i = 0; i <= LogicalBoard.Instance.GetCol(); i++)
            {
                ColumnDefinition col = new ColumnDefinition();
                graphicalBoard.ColumnDefinitions.Add(col);
            }

            //define rows
            for (int i = 0; i <= LogicalBoard.Instance.GetRow(); i++)
            {
                RowDefinition row = new RowDefinition();
                graphicalBoard.RowDefinitions.Add(row);
            }

            //add columns labels
            for (int i = 1; i <= othelloGridCells.GetLength(0); i++)
            {
                OthelloGridLabel label = new OthelloGridLabel("" + i);

                Grid.SetColumn(label, i);
                Grid.SetRow(label, 0);

                graphicalBoard.Children.Add(label);
            }

            //add rows labels
            for (int i = 1; i <= othelloGridCells.GetLength(1); i++)
            {
                OthelloGridLabel label = new OthelloGridLabel("" + Convert.ToChar(i + 64));

                Grid.SetColumn(label, 0);
                Grid.SetRow(label, i);

                graphicalBoard.Children.Add(label);
            }

            //add game cells
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
            currentPossibleMoves = LogicalBoard.Instance.GetListPossibleMove(turnWhite);
            int[,] lboard = LogicalBoard.Instance.GetBoard();
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

                    if (LogicalBoard.Instance.LastPlay != null)
                        gcell.LastPlay = pos.Item1 == LogicalBoard.Instance.LastPlay.Item1 && pos.Item2 == LogicalBoard.Instance.LastPlay.Item2;
                }
            }
        }

        private void Cell_Click(object sender, EventArgs e)
        {
            OthelloGridCell s = (OthelloGridCell)sender;
            int x = s.X;
            int y = s.Y;
            Tuple<int, int> pos = new Tuple<int, int>(x, y);
            if (currentPossibleMoves != null && currentPossibleMoves.ContainsKey(pos))
            {
                LogicalBoard.Instance.PlayMove(x, y, turnWhite);
                NextTurn();
                UpdateGui();
            }
        }

        private void UpdateGui()
        {
            UpdateBoard();
            UpdateGameData();
            UpdateControls();
        }

        private void UpdateGameData()
        {
            UpdateTimers();
            lblTurn.Content = turnWhite ? Player.WhitePlayer.Name : Player.BlackPlayer.Name;
            lblNbTokenBlack.GetBindingExpression(Label.ContentProperty).UpdateTarget();
            lblNbTokenWhite.GetBindingExpression(Label.ContentProperty).UpdateTarget();
        }

        private void UpdateTimers()
        {
            UpdateTimer(Player.BlackPlayer, lblTimeBlack);
            UpdateTimer(Player.WhitePlayer, lblTimeWhite);
        }

        private void UpdateControls()
        {
            btnUndo.IsEnabled = LogicalBoard.Instance.CanUndo();
            btnRedo.IsEnabled = LogicalBoard.Instance.CanRedo();
        }

        private void UpdateTimer(Player p, Label lbl)
        {
            if (p != null)
            {
                TimeSpan remainingTime = TimeSpan.FromMilliseconds(p.Stopwatch.ElapsedMilliseconds + p.PreviousTime);
                lbl.Content = remainingTime.ToString(@"mm\:ss\.fff");
            }
        }

        private void NextTurn()
        {
            turnWhite = !turnWhite;
            currentPossibleMoves = LogicalBoard.Instance.GetListPossibleMove(turnWhite);

            if (currentPossibleMoves.Count <= 0)
            {
                turnWhite = !turnWhite;
                currentPossibleMoves = LogicalBoard.Instance.GetListPossibleMove(turnWhite);
                if (currentPossibleMoves.Count <= 0)
                {
                    Console.WriteLine("Game End :" + currentPossibleMoves.Count);
                }
                else
                {
                    Console.WriteLine("Turn skiped :" + currentPossibleMoves.Count);
                }
            }

            if (turnWhite)
            {
                Player.WhitePlayer.Stopwatch.Start();
                Player.BlackPlayer.Stopwatch.Stop();
            }
            else
            {
                Player.WhitePlayer.Stopwatch.Stop();
                Player.BlackPlayer.Stopwatch.Start();
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
            saveFileDialog.Filter = "Othello Game Status|*.mm";
            saveFileDialog.Title = "Sauvegarder la partie d'Othello";
            saveFileDialog.ShowDialog();

            if (saveFileDialog.FileName != "")
            {
                IFormatter formatter = new BinaryFormatter();
                System.IO.FileStream fs = (System.IO.FileStream)saveFileDialog.OpenFile();
                formatter.Serialize(fs, LogicalBoard.Instance);
                fs.Close();
            }
        }

        private void btnLoad_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "Othello Game Status|*.mm";
            openFileDialog.Title = "Charger la partie d'Othello";
            openFileDialog.ShowDialog();

            if (openFileDialog.FileName != "")
            {
                IFormatter formatter = new BinaryFormatter();
                System.IO.FileStream fs = (System.IO.FileStream)openFileDialog.OpenFile();
                LogicalBoard.Instance = (LogicalBoard)formatter.Deserialize(fs);
                fs.Close();
                turnWhite = !LogicalBoard.Instance.GetLastPlayer();
                UpdateGui();
            }
        }

        private void btnRedo_Click(object sender, RoutedEventArgs e)
        {
            LogicalBoard.Instance.Redo();
            turnWhite = !LogicalBoard.Instance.GetLastPlayer();
            UpdateGui();
        }

        private void btnUndo_Click(object sender, RoutedEventArgs e)
        {
            LogicalBoard.Instance.Undo();
            turnWhite = !LogicalBoard.Instance.GetLastPlayer();
            UpdateGui();
        }
    }
}
