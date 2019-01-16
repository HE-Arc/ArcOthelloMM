using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace ArcOthelloMM
{
    [Serializable]
    public class Player: ISerializable
    {
        public List<Tuple<int, int>> Tokens { get; set; }
        public int Value { get; set; }
        public string Name { get; set; }

        public Stopwatch Stopwatch { get; set; }

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
            Tokens = new List<Tuple<int, int>>();
            Stopwatch = new Stopwatch();
        }

        public void GetObjectData(SerializationInfo info, StreamingContext context)
        {
        }
    }
}
