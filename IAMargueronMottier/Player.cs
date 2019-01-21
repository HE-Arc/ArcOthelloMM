using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Runtime.Serialization;
using System.Windows.Media;

namespace ArcOthelloMM
{
    [Serializable]
    public class Player : ISerializable
    {
        public List<Tuple<int, int>> Tokens { get; set; }
        public int Value { get; set; }
        public string Name { get; set; }

        private Color color;
        public Color Color { get { return color; } }

        private Stopwatch Stopwatch { get; set; }

        private long PreviousTime { get; set; }

        //Double singleton (one for player 0 and another for player 1)
        private static Player player0;
        private static Player player1;

        private static readonly Dictionary<int, Tuple<string, Color>> dictPlayers;

        /// <summary>
        /// Instanciate the white player
        /// </summary>
        private Player(int value)
        {
            Reset();
            Value = value;
            InitPlayerById(value);
            PreviousTime = 0;
        }

        static Player()
        {
            dictPlayers = new Dictionary<int, Tuple<string, Color>>
            {
                { 0, new Tuple<string, Color>("Rouge", Color.FromRgb(233, 68, 82)) },
                { 1, new Tuple<string, Color>("Bleu", Color.FromRgb(68, 82, 233)) }
            };
        }

        private void InitPlayerById(int id)
        {
            Name = dictPlayers[id].Item1;
            color = dictPlayers[id].Item2;
        }

        public static Player BlackPlayer
        {
            get
            {
                if (player0 == null)
                    player0 = new Player(0);
                return player0;
            }

            set
            {
                player0 = value;
            }
        }

        public static Player WhitePlayer
        {
            get
            {
                if (player1 == null)
                    player1 = new Player(1);
                return player1;
            }
            set
            {
                player1 = value;
            }
        }

        /// <summary>
        /// Return the value of white player
        /// </summary>
        /// <returns></returns>
        public int GetValue()
        {
            return Value;
        }

        /// <summary>
        /// Return the score of a player as a sum of the element of the list
        /// </summary>
        public int Score
        {
            get
            {
                return Tokens.Count;
            }
        }

        public void Reset()
        {
            Tokens = new List<Tuple<int, int>>();
            Stopwatch = new Stopwatch();
            PreviousTime = 0;
        }

        protected Player(SerializationInfo info, StreamingContext context)
        {
            Tokens = (List<Tuple<int, int>>)info.GetValue("Tokens", typeof(List<Tuple<int, int>>));
            Value = (int)info.GetValue("Value", typeof(int));
            Stopwatch = new Stopwatch(); // not serializable
            PreviousTime = (long)info.GetValue("PreviousTime", typeof(long)); //mini hack to avoid stopwatch not serializable
            InitPlayerById(Value);
        }

        public void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            info.AddValue("Tokens", Tokens);
            info.AddValue("Value", Value);
            info.AddValue("PreviousTime", GetTime());
        }

        public long GetTime()
        {
            return Stopwatch.ElapsedMilliseconds + PreviousTime;
        }

        public void Start()
        {
            Stopwatch.Start();
        }

        public void Stop()
        {
            Stopwatch.Stop();
        }
    }
}
