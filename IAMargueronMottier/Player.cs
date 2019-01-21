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

        private Color color1;
        private Color color2;
        public Color Color1 { get { return color1; } }
        public Color Color2 { get { return color2; } }

        private Stopwatch Stopwatch { get; set; }

        private long PreviousTime { get; set; }

        private static Player whitePlayer;
        private static Player blackPlayer;

        private static Dictionary<int, Tuple<string, Color, Color>> dictPlayers;

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
            dictPlayers.Add(0, new Tuple<string, Color, Color>("Rouge", Color.FromRgb(255, 97, 97), Color.FromRgb(233, 68, 82)));
            dictPlayers.Add(1, new Tuple<string, Color, Color>("Bleu", Color.FromRgb(97, 111, 255), Color.FromRgb(68, 82, 233)));
        }

        private void InitPlayerById(int id)
        {
            Console.WriteLine(id);
            Name = dictPlayers[id].Item1;
            color1 = dictPlayers[id].Item2;
            color2 = dictPlayers[id].Item3;
        }

        public static Player BlackPlayer
        {
            get
            {
                if (blackPlayer == null)
                    blackPlayer = new Player(0);
                return blackPlayer;
            }

            set
            {
                blackPlayer = value;
            }
        }

        public static Player WhitePlayer
        {
            get
            {
                if (whitePlayer == null)
                    whitePlayer = new Player(1);
                return whitePlayer;
            }
            set
            {
                whitePlayer = value;
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
