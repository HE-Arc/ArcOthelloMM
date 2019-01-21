using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Timers;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Animation;

namespace ArcOthelloMM
{
    /// <summary>
    /// Interaction logic for OthelloBoard.xaml
    /// </summary>
    public partial class OthelloBoard : Window
    {
        private OthelloGridCell[,] othelloGridCells; // to change their states

        private bool playerVsPlayer;
        private Dictionary<Tuple<int, int>, HashSet<Tuple<int, int>>> currentPossibleMoves; // to optimize calls

        private Timer timerUpdateGui;

        private const string timeElapsedText = "Temps écoulé joueur ";
        private const string playAgainText = " peut rejouer !";
        private const string scoreText = "Nombre de pièces ";

        /// <summary>
        /// Create a new graphical board
        /// </summary>
        public OthelloBoard()
        {
            InitializeComponent();

            timerUpdateGui = new Timer();
            timerUpdateGui.Interval = 0.1;
            timerUpdateGui.Elapsed += TimerUpdateGui_Elapsed;
            timerUpdateGui.Start();

            lblTimePlayer0Text.Content = timeElapsedText + Player.Player0.Name;
            lblTimePlayer1Text.Content = timeElapsedText + Player.Player1.Name;
            lblScorePlayer0Text.Content = scoreText + Player.Player0.Name;
            lblScorePlayer1Text.Content = scoreText + Player.Player1.Name;

            UpdateDataContext();

            GenerateGrid();
            UpdateGradiant(true);
        }

        private void UpdateDataContext()
        {
            DataContext = LogicalBoard.Instance;
        }

        /// <summary>
        /// Generate the grid with labels and cells
        /// </summary>
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
                OthelloGridLabel label = new OthelloGridLabel("" + Convert.ToChar(i + 64));

                Grid.SetColumn(label, i);
                Grid.SetRow(label, 0);

                graphicalBoard.Children.Add(label);
            }

            //add rows labels
            for (int i = 1; i <= othelloGridCells.GetLength(1); i++)
            {
                OthelloGridLabel label = new OthelloGridLabel("" + i);

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
                    Grid.SetRow(cell, y + 1);

                    graphicalBoard.Children.Add(cell);
                }
            }
        }

        /// <summary>
        /// Launch a new game
        /// </summary>
        /// <param name="playerVsPlayer">true : player vs player / falst : player vs ia</param>
        private void NewGame(bool playerVsPlayer)
        {
            this.playerVsPlayer = playerVsPlayer;
            LogicalBoard.Instance.ResetGame();
            currentPossibleMoves = LogicalBoard.Instance.CurrentPossibleMoves;

            UpdateGui();
        }

        /// <summary>
        /// Launch the game with a player
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnNewGame_Click(object sender, RoutedEventArgs e)
        {
            NewGame(true);
        }

        /// <summary>
        /// Launch the game with an ia
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnNewGameAI_Click(object sender, RoutedEventArgs e)
        {
            NewGame(false);
        }

        /// <summary>
        /// Update the timers
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
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

        /// <summary>
        /// Update the board according to the logic
        /// </summary>
        private void UpdateBoard()
        {
            currentPossibleMoves = LogicalBoard.Instance.CurrentPossibleMoves;
            int[,] lboard = LogicalBoard.Instance.GetBoard();

            for (int x = 0; x < othelloGridCells.GetLength(0); x++)
            {
                for (int y = 0; y < othelloGridCells.GetLength(1); y++)
                {
                    Tuple<int, int> pos = new Tuple<int, int>(x, y);
                    OthelloGridCell gcell = othelloGridCells[x, y];

                    int lcell = lboard[x, y];
                    if (lcell == 1)
                        gcell.State = OthelloGridCell.States.Player1;
                    else if (lcell == 0)
                        gcell.State = OthelloGridCell.States.Player2;
                    else if (lcell == -1 && currentPossibleMoves.ContainsKey(pos))
                    {
                        if (LogicalBoard.Instance.CurrentPlayerTurn)
                            gcell.State = OthelloGridCell.States.PreviewPlayer1;
                        else
                            gcell.State = OthelloGridCell.States.PreviewPlayer2;
                    }
                    else
                        gcell.State = OthelloGridCell.States.Empty;

                    gcell.LastPlay = LogicalBoard.Instance.LastMovePosition != null && pos.Item1 == LogicalBoard.Instance.LastMovePosition.Item1 && pos.Item2 == LogicalBoard.Instance.LastMovePosition.Item2;
                }
            }
        }

        /// <summary>
        /// handle the click of a cell (inform the logic and update the gui)
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Cell_Click(object sender, EventArgs e)
        {
            OthelloGridCell s = (OthelloGridCell)sender;
            int x = s.X;
            int y = s.Y;
            Tuple<int, int> pos = new Tuple<int, int>(x, y);
            if (currentPossibleMoves != null && currentPossibleMoves.ContainsKey(pos))
            {
                LogicalBoard.Instance.PlayMove(x, y, LogicalBoard.Instance.CurrentPlayerTurn);
                NextTurn();
                UpdateGui();
            }
        }

        /// <summary>
        /// A general purpose method to update gui
        /// </summary>
        private void UpdateGui()
        {
            UpdateBoard();
            UpdateGameData();
            UpdateControls();
            UpdateGradiant();
        }

        /// <summary>
        /// Update the game data (on the left of the board)
        /// </summary>
        private void UpdateGameData()
        {
            UpdateTimers();
            lblTurn.Content = LogicalBoard.Instance.CurrentPlayer.Name;
        }

        /// <summary>
        /// Updathe the gradiant
        /// </summary>
        /// <param name="_default">true : half, half gradiant</param>
        private void UpdateGradiant(bool _default = false)
        {
            float percentageBlack = _default ? 0.5f : Player.Player0.Score / (float)(Player.Player0.Score + Player.Player1.Score);
            float percentageWhite = _default ? 0.5f : 1 - percentageBlack;
            
            LinearGradientBrush linearGradientBrush = new LinearGradientBrush(Player.Player0.Color, Player.Player1.Color, 0);
            linearGradientBrush.StartPoint = new Point(0, percentageBlack - 0.5);
            linearGradientBrush.EndPoint = new Point(0, 1 - (percentageWhite - 0.5));

            this.Background = linearGradientBrush;
        }

        /// <summary>
        /// Update the two timers
        /// </summary>
        private void UpdateTimers()
        {
            UpdateTimer(Player.Player0, lblTimePlayer0);
            UpdateTimer(Player.Player1, lblTimePlayer1);
        }

        /// <summary>
        /// Update the undo redo buttons
        /// </summary>
        private void UpdateControls()
        {
            btnUndo.IsEnabled = LogicalBoard.Instance.CanUndo();
            btnRedo.IsEnabled = LogicalBoard.Instance.CanRedo();
        }

        /// <summary>
        /// Update the timer
        /// </summary>
        /// <param name="p"></param>
        /// <param name="lbl"></param>
        private void UpdateTimer(Player p, Label lbl)
        {
            if (p != null)
            {
                TimeSpan remainingTime = TimeSpan.FromMilliseconds(p.GetTime());
                lbl.Content = remainingTime.ToString(@"mm\:ss\.fff");
            }
        }

        /// <summary>
        /// Next turn button
        /// </summary>
        private void NextTurn()
        {
            bool endOfGame = false;
            currentPossibleMoves = LogicalBoard.Instance.CurrentPossibleMoves;

            if (currentPossibleMoves.Count <= 0)
            {
                LogicalBoard.Instance.CurrentPlayerTurn ^= true; // change turn
                currentPossibleMoves = LogicalBoard.Instance.CurrentPossibleMoves;

                if (currentPossibleMoves.Count <= 0)
                {
                    endOfGame = true; //only switch the timers if it's the end of the game
                    Player.Player0.Stop();
                    Player.Player1.Stop();

                    UpdateGui();
                    OthelloEndOfGame othelloEndOfGame;
                    if (Player.Player0.Score != Player.Player1.Score)
                    {
                        Player playerWin = Player.Player0.Score > Player.Player1.Score ? Player.Player0 : Player.Player1;
                        Player playerLose = Player.Player0.Score > Player.Player1.Score ? Player.Player1 : Player.Player0;
                        othelloEndOfGame = new OthelloEndOfGame(playerWin, playerLose);
                    }
                    else //tied
                    {
                        othelloEndOfGame = new OthelloEndOfGame(null, null);
                    }
                    othelloEndOfGame.ShowDialog();
                }
                else
                {
                    ShowPlayAgainMessage();
                }
            }

            if (!endOfGame)
            {
                if (LogicalBoard.Instance.CurrentPlayerTurn)
                {
                    Player.Player1.Start();
                    Player.Player0.Stop();
                }
                else
                {
                    Player.Player1.Stop();
                    Player.Player0.Start();
                }
            }
        }

        /// <summary>
        /// Show a message when a turn is skipped
        /// </summary>
        private void ShowPlayAgainMessage()
        {
            // can play again animation
            lblPlayAgain.Content = LogicalBoard.Instance.CurrentPlayer.Name + playAgainText;
            Storyboard sb = new Storyboard();

            DoubleAnimation da1 = new DoubleAnimation() { From = 0.0, To = 1.0, Duration = TimeSpan.FromSeconds(0.5) };
            Storyboard.SetTargetName(da1, playAgain.Name);
            Storyboard.SetTargetProperty(da1, new PropertyPath(Border.OpacityProperty));

            DoubleAnimation da2 = new DoubleAnimation() { From = 1.0, To = 0.0, Duration = TimeSpan.FromSeconds(0.5) };
            da2.BeginTime = TimeSpan.FromSeconds(3);
            Storyboard.SetTargetName(da2, playAgain.Name);
            Storyboard.SetTargetProperty(da2, new PropertyPath(Border.OpacityProperty));

            //Index animation
            Int32Animation daIndex1 = new Int32Animation() { From = -1000, To = 1000, Duration = TimeSpan.FromSeconds(0.1) };
            Storyboard.SetTargetName(daIndex1, playAgain.Name);
            Storyboard.SetTargetProperty(daIndex1, new PropertyPath(Panel.ZIndexProperty));

            Int32Animation daIndex2 = new Int32Animation() { From = 1000, To = -1000, Duration = TimeSpan.FromSeconds(0.1) };
            daIndex2.BeginTime = TimeSpan.FromSeconds(da2.BeginTime.Value.Seconds + da2.Duration.TimeSpan.Seconds); //start at the end of da2
            Storyboard.SetTargetName(daIndex2, playAgain.Name);
            Storyboard.SetTargetProperty(daIndex2, new PropertyPath(Panel.ZIndexProperty));

            sb.Children.Add(da1);
            sb.Children.Add(da2);
            sb.Children.Add(daIndex1);
            sb.Children.Add(daIndex2);

            sb.Begin(this);
        }

        /// <summary>
        /// Keep the board with square sizes and make it responsive
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
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

        /// <summary>
        /// Save the game file
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
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

        /// <summary>
        /// Load a game file
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
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
                UpdateGui();
            }
            UpdateDataContext();
        }

        /// <summary>
        /// Redo button handling
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnRedo_Click(object sender, RoutedEventArgs e)
        {
            LogicalBoard.Instance.Redo();
            UpdateGui();
        }

        /// <summary>
        /// Undo button handling
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnUndo_Click(object sender, RoutedEventArgs e)
        {
            LogicalBoard.Instance.Undo();
            UpdateGui();
        }
    }
}
