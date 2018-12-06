using System.Collections.Generic;
using System.Windows;
using System.Windows.Media;
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
        private const int PLAYER_ONE = 1;
        private const int PLAYER_TWO = -1;

        private int currentPlayer;
        private int[,] board;

        private List<Ellipse> lstPiece;
        private List<Ellipse> cirPlayable;
        private List<Rectangle> recPlayable;
        private Dictionary<string, HashSet<int[]>> lstMove;

        public MainWindow()
        {
            InitializeComponent();

            lstPiece = new List<Ellipse>();
            cirPlayable = new List<Ellipse>();
            recPlayable = new List<Rectangle>();
            lstMove = new Dictionary<string, HashSet<int[]>>();
            board = new int[ROW, COLUMN];

            initGame();
        }

        /// <summary>
        /// Init the first pieces
        /// Set the first player
        /// </summary>
        private void initGame()
        {
            currentPlayer = PLAYER_ONE;
            setPiece(3, 3);
            setPiece(4, 4);
            currentPlayer = PLAYER_TWO;
            setPiece(3, 4);
            setPiece(4, 3);
            currentPlayer = PLAYER_ONE;
            getCasePlayable();
        }

        private void addPiece(string name)
        {
            // get case to add piece
            Rectangle rec = (Rectangle)grid.FindName("r" + name.Substring(1));

            // create piece
            Ellipse circle = new Ellipse
            {
                Name = name,
                HorizontalAlignment = HorizontalAlignment.Left,
                VerticalAlignment = VerticalAlignment.Top,
                Width = SIZE_PIECE,
                Height = SIZE_PIECE,
                Margin = new Thickness(rec.Margin.Left + 5, rec.Margin.Top + 5, 0, 0),
                Fill = (currentPlayer == PLAYER_ONE) ? Brushes.Black : Brushes.White
            };

            // add piece
            grid.Children.Add(circle);
            lstPiece.Add(circle);
        }

        /// <summary>
        /// set a piece of current user in the case
        /// </summary>
        /// <param name="name"></param>
        private void setPiece(string name)
        {
            int x = int.Parse(name[1].ToString());
            int y = int.Parse(name[2].ToString());

            setPiece(x, y);
        }

        /// <summary>
        /// set a piece of current user in the case
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        private void setPiece(int x, int y)
        {
            // set new owner of the case
            board[x, y] = currentPlayer;

            // check if a piece exist already in the case
            string name = "c" + x.ToString() + y.ToString();
            Ellipse circle = lstPiece.Find(c => c.Name == name);

            if (circle == null)
            {
                addPiece(name);
            }
            else
            {
                circle.Fill = (currentPlayer == PLAYER_ONE) ? Brushes.Black : Brushes.White;
            }
        }

        /// <summary>
        /// Check move from all cases who belongs to currentplayer
        /// </summary>
        private void getCasePlayable()
        {
            for (int x = 0; x < ROW; ++x)
            {
                for (int y = 0; y < COLUMN; ++y)
                {
                    if (board[x, y] == currentPlayer)
                    {
                        checkMove(-currentPlayer, x, y);
                    }
                }
            }
        }

        /// <summary>
        /// Check if the move is possible
        /// and save the pieces affected
        /// </summary>
        /// <param name="opponent"></param>
        /// <param name="row"></param>
        /// <param name="column"></param>
        private void checkMove(int opponent, int row, int column)
        {
            // Loop and check in every direction
            // if the case is ennemy continue to check in the direction
            // if the case is empty and the increment is greater than 1 => move possible and save the pieces
            // else move impossible

            int i = 1;
            bool left = true;
            bool top = true;
            bool right = true;
            bool bottom = true;

            // Diagonal
            bool leftTop = true;
            bool rightTop = true;
            bool leftBottom = true;
            bool rightBottom = true;

            // pieces affected by possible move in every direction
            HashSet<int[]> listLeft = new HashSet<int[]>();
            HashSet<int[]> listTop = new HashSet<int[]>();
            HashSet<int[]> listRight = new HashSet<int[]>();
            HashSet<int[]> listBottom = new HashSet<int[]>();
            HashSet<int[]> listLeftTop = new HashSet<int[]>();
            HashSet<int[]> listRightTop = new HashSet<int[]>();
            HashSet<int[]> listLeftBottom = new HashSet<int[]>();
            HashSet<int[]> listRightBottom = new HashSet<int[]>();

            do
            {
                if (left)
                {
                    int x = row - i;
                    int y = column;

                    listLeft.Add(new int[] { x, y });

                    if (x < 0)
                    {
                        left = false;
                    }
                    else if (board[x, y] != opponent)
                    {
                        if (board[x, y] == 0 && i > 1)
                        {
                            addMove(listLeft, x, y); // save move
                        }
                        left = false;
                    }
                }

                if (right)
                {
                    int x = row + i;
                    int y = column;

                    listRight.Add(new int[] { x, y });

                    if (x >= ROW)
                    {
                        right = false;
                    }
                    else if (board[x, y] != opponent)
                    {
                        if (board[x, y] == 0 && i > 1)
                        {
                            addMove(listRight, x, y); // save move
                        }
                        right = false;
                    }
                }

                if (top)
                {
                    int x = row;
                    int y = column - i;

                    listTop.Add(new int[] { x, y });

                    if (y < 0)
                    {
                        top = false;
                    }
                    else if (board[x, y] != opponent)
                    {
                        if (board[x, y] == 0 && i > 1)
                        {
                            addMove(listTop, x, y); // save move
                        }
                        top = false;
                    }
                }

                if (bottom)
                {
                    int x = row;
                    int y = column + i;

                    listBottom.Add(new int[] { x, y });

                    if (y >= COLUMN)
                    {
                        bottom = false;
                    }
                    else if (board[x, y] != opponent)
                    {
                        if (board[x, y] == 0 && i > 1)
                        {
                            addMove(listBottom, x, y); // save move
                        }
                        bottom = false;
                    }
                }

                if (leftTop)
                {
                    int x = row - i;
                    int y = column - i;

                    listLeftTop.Add(new int[] { x, y });

                    if (x < 0 || y < 0)
                    {
                        leftTop = false;
                    }
                    else if (board[x, y] != opponent)
                    {
                        if (board[x, y] == 0 && i > 1)
                        {
                            addMove(listLeftTop, x, y); // save move
                        }
                        leftTop = false;
                    }
                }

                if (rightTop)
                {
                    int x = row + i;
                    int y = column - i;

                    listRightTop.Add(new int[] { x, y });

                    if (x >= ROW || y < 0)
                    {
                        rightTop = false;
                    }
                    else if (board[x, y] != opponent)
                    {
                        if (board[x, y] == 0 && i > 1)
                        {
                            addMove(listRightTop, x, y); // save move
                        }
                        rightTop = false;
                    }
                }

                if (leftBottom)
                {
                    int x = row - i;
                    int y = column + i;

                    listLeftBottom.Add(new int[] { x, y });

                    if (x < 0 || y >= COLUMN)
                    {
                        leftBottom = false;
                    }
                    else if (board[x, y] != opponent)
                    {
                        if (board[x, y] == 0 && i > 1)
                        {
                            addMove(listLeftBottom, x, y); // save move
                        }
                        leftBottom = false;
                    }
                }

                if (rightBottom)
                {
                    int x = row + i;
                    int y = column + i;

                    listRightBottom.Add(new int[] { x, y });

                    if (x >= ROW || y >= COLUMN)
                    {
                        rightBottom = false;
                    }
                    else if (board[x, y] != opponent)
                    {
                        if (board[x, y] == 0 && i > 1)
                        {
                            addMove(listRightBottom, x, y); // save move
                        }
                        rightBottom = false;
                    }
                }

                ++i;
            } while (left || top || right || bottom || leftTop || rightTop || leftBottom || rightBottom); // check if a direction need to continue

        }

        /// <summary>
        /// Save move
        /// and show the possibility on the board
        /// </summary>
        /// <param name="move"></param>
        /// <param name="row"></param>
        /// <param name="column"></param>
        private void addMove(HashSet<int[]> move, int row, int column)
        {
            string fName = "f" + row.ToString() + column.ToString();

            // create little token to show the possible move
            Ellipse circle = (Ellipse)grid.FindName(fName);
            if (circle == null)
            {
                Rectangle rec = (Rectangle)grid.FindName("r" + fName.Substring(1));
                circle = new Ellipse
                {
                    Name = fName,
                    HorizontalAlignment = HorizontalAlignment.Left,
                    VerticalAlignment = VerticalAlignment.Top,
                    Width = rec.Width - 20,
                    Height = rec.Height - 20,
                    Fill = Brushes.Black,
                    Margin = new Thickness(rec.Margin.Left + 10, rec.Margin.Top + 10, 0, 0),
                    Opacity = 0.2
                };

                grid.Children.Add(circle);
                cirPlayable.Add(circle);
                recPlayable.Add(rec);

                // and event to play here
                rec.MouseDown += Rec_MouseDown;
                circle.MouseDown += Cir_MouseDown;
            }

            // add pieces affected for the case played
            if (lstMove.ContainsKey(circle.Name))
            {
                foreach (int[] location in move)
                {
                    lstMove[circle.Name].Add(location);
                }
            }
            else
            {
                lstMove.Add(circle.Name, move);
            }
        }

        /// <summary>
        /// Event to click on a case
        /// where we can play
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Rec_MouseDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            Rectangle rec = (Rectangle)sender;
            Ellipse circle = cirPlayable.Find(f => f.Name == "f" + rec.Name.Substring(1));
            play(rec, circle);
        }

        /// <summary>
        /// Event to click on token
        /// where we can play
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Cir_MouseDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            Ellipse circle = (Ellipse)sender;
            Rectangle rec = (Rectangle)grid.FindName("r" + circle.Name.Substring(1));
            play(rec, circle);
        }

        /// <summary>
        /// Play a move
        /// </summary>
        /// <param name="rec"></param>
        /// <param name="circle"></param>
        private void play(Rectangle rec, Ellipse circle)
        {
            // Remove events from each cases
            foreach (Rectangle playable in recPlayable)
            {
                playable.MouseDown -= Rec_MouseDown;
            }
            recPlayable.Clear();

            // Remove token
            foreach (Ellipse playable in cirPlayable)
            {
                grid.Children.Remove(playable);
            }
            cirPlayable.Clear();

            // Set piece for each case affected by the move
            HashSet<int[]> move = lstMove[circle.Name];
            foreach (int[] location in move)
            {
                setPiece(location[0], location[1]);
            }
            lstMove.Clear();

            // change player
            currentPlayer = -currentPlayer;
            getCasePlayable();

            // if the player can't play change player
            if (lstMove.Count == 0)
            {
                currentPlayer = -currentPlayer;
                getCasePlayable();

                if (lstMove.Count == 0)
                {
                    // no player can play => and game
                }
            }

        }
    }
}
