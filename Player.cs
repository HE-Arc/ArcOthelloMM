using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace ArcOthelloMM
{
    public class Player
    {
        private List<Token> Tokens;
        public int Value { get; set; }
        public string Name { get; set; }
        
        private static Player whitePlayer;
        private static Player blackPlayer;

        /// <summary>
        /// Instanciate the white player
        /// </summary>
        private Player(int value, string name)
        {
            Reset();
            Value = value;
            Name = name;
        }

        /// <summary>
        /// Assure there is one white player
        /// </summary>
        /// <returns></returns>
        public static Player GetWhite()
        {
            if (whitePlayer == null)
                whitePlayer = new Player(1, "Blanc");
            return whitePlayer;
        }

        public static Player GetBlack()
        {
            if (blackPlayer == null)
                blackPlayer = new Player(0, "Noir");
            return blackPlayer;
        }

        /// <summary>
        /// Return the value of white player
        /// </summary>
        /// <returns></returns>
        public int GetValue()
        {
            return Value;
        }

        public void Reset()
        {
            Tokens = new List<Token>();
        }

        public List<Tuple<int, int>> GetTuples()
        {
            List<Tuple<int, int>> tuples = new List<Tuple<int, int>>();
            foreach (Token token in Tokens)
            {
                tuples.Add(token.Tuple);
            }
            return tuples;
        }

        public List<Token> GetTokens()
        {
            return Tokens;
        }

        public void AddToken(Tuple<int, int> tuple)
        {
            Tokens.Add(new Token(tuple));
        }

        public void RemoveToken(Tuple<int, int> tuple)
        {
            foreach (Token token in Tokens)
            {
                if (token.Tuple.Item1 == tuple.Item1 && token.Tuple.Item2 == tuple.Item2)
                {
                    Tokens.Remove(token);
                    break;
                }
            }
        }

        public bool ContainsToken(Tuple<int, int> tuple)
        {
            foreach (Token token in Tokens)
            {
                if (token.Tuple.Item1 == tuple.Item1 && token.Tuple.Item2 == tuple.Item2)
                {
                    return true;
                }
            }

            return false;
        }

        public void ResetTokens()
        {
            foreach (Token token in Tokens)
            {
                token.ResetDirections();
            }
        }
    }
}
