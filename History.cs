using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ArcOthelloMM
{
    struct Move
    {
        bool IsPlayerWhite;
        Tuple<int, int> TokenPlayed;
        List<Tuple<int, int>> TokenTaken;

        public Move(bool isPlayerWhite, Tuple<int, int> tokenPlayed, List<Tuple<int, int>> tokenTaken)
        {
            IsPlayerWhite = isPlayerWhite;
            TokenPlayed = tokenPlayed;
            TokenTaken = tokenTaken;
        }
    }

    class History
    {
        private List<Move> Moves { get; set; }
        private Player WhitePlayer { get; set; }
        private Player BlackPlayer { get; set; }
        private int Index { get; set; }

        public History()
        {
            Moves = new List<Move>();
            WhitePlayer = Player.GetWhite();
            BlackPlayer = Player.GetBlack();
        }

        public void SaveMove(bool IsPlayerWhite, Tuple<int, int> TokenPlayed, List<Tuple<int, int>> TokenTaken)
        {
            Move move = new Move(IsPlayerWhite, TokenPlayed, TokenTaken);

            for (int i = Moves.Count - 1; i > Index; ++i)
            {
                Moves.RemoveAt(i);
            }

            Moves.Add(move);
            Index = Moves.Count - 1;
        }

        public void undo (int[,] Board)
        {

        }
    }
}
