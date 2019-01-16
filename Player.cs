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

        private Stopwatch Stopwatch { get; set; }

        private long PreviousTime { get; set; }

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
            PreviousTime = 0;
        }

        public static Player WhitePlayer
        {
            get
            {
                if (whitePlayer == null)
                    whitePlayer = new Player(1, "Blanc");
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
                    blackPlayer = new Player(0, "Noir");
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
