using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ArcOthelloMM
{
    class Board
    {
        public enum Player
        {
            None, Black, White
        }

        public int Width { get; }
        public int Height { get; }
        public Player[,] pieces;
        public Player Turn { get; }

        public Board(int width, int height)
        {
            this.Width = width;
            this.Height = height;
            this.Turn = Player.Black;
            InitBoard();
            AddBeginingPieces();
        }

        private void InitBoard()
        {
            pieces = new Player[Width, Height];
            for (int i = 0; i < Width; ++i)
                for (int j = 0; j < Height; ++j)
                    pieces[i, j] = Player.None;
        }

        private void AddBeginingPieces()
        {
            int centerX = this.Width / 2;
            int centerY = this.Height / 2;
            pieces[centerX - 1, centerY - 1] = Player.Black;
            pieces[centerX, centerY - 1] = Player.White;
            pieces[centerX - 1, centerY] = Player.White;
            pieces[centerX, centerY] = Player.Black;
        }

        private Player this[int i, int j]
        {
            get => pieces[i, j];
            set => pieces[i, j] = value;
        }

        public bool TryToPlay(int x, int y, Player player)
        {
            if (!IsAPossiblePlay(x, y, player))
                return false;
            this[x, y] = player;
            return true;
        }

        public Player GetPosition(int x, int y)
        {
            return this[x, y];
        }

        public bool IsAPossiblePlay(int x, int y, Player player)
        {
            //Todo
            return true;
        }

    }
}
