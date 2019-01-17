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
        public Color Color { get; }
        public Color Color2 { get; }

        private Stopwatch Stopwatch { get; set; }

        private long PreviousTime { get; set; }


        private static Player whitePlayer;
        private static Player blackPlayer;

        /// <summary>
        /// Instanciate the white player
        /// </summary>
        private Player(int value, string name, Color color, Color color2)
        {
            Reset();
            Value = value;
            Name = name;
            PreviousTime = 0;
            Color = color;
            Color2 = color2;
        }

        public static Player WhitePlayer
        {
            get
            {
                if (whitePlayer == null)
                    whitePlayer = new Player(1, "Bleu", Color.FromRgb(97, 111, 255), Color.FromRgb(68, 82, 233));
                return whitePlayer;
            }
            set
            {
                whitePlayer = value;
            }
        }

        public static Player BlackPlayer
        {
            get
            {
                if (blackPlayer == null)
                    blackPlayer = new Player(0, "Rouge", Color.FromRgb(255, 97, 97), Color.FromRgb(233, 68, 82));
                return blackPlayer;
            }

            set
            {
                blackPlayer = value;
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
            Name = (string)info.GetValue("Name", typeof(string));
            Stopwatch = new Stopwatch(); // not serializable
            PreviousTime = (long)info.GetValue("PreviousTime", typeof(long)); //mini hack to avoid stopwatch not serializable
        }

        public void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            info.AddValue("Tokens", Tokens);
            info.AddValue("Value", Value);
            info.AddValue("Name", Name);
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
