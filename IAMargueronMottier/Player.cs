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

        public Color Color { get; internal set; }

        private Stopwatch Stopwatch { get; set; }
        private long PreviousTime { get; set; }

        //Double singleton (one for player 0 and another for player 1)
        private static Player player0;
        private static Player player1;
        
        private static readonly Dictionary<int, Tuple<string, Color>> dictPlayers;
        
        /// <summary>
        /// Create a player with a value
        /// </summary>
        /// <param name="value"></param>
        private Player(int value)
        {
            Reset();
            Value = value;
            InitPlayerById(value);
            PreviousTime = 0;
        }
        
        /// <summary>
        /// Initalize static field for both players
        /// </summary>
        static Player()
        {
            dictPlayers = new Dictionary<int, Tuple<string, Color>>
            {
                { 0, new Tuple<string, Color>("Rouge", Color.FromRgb(252, 89, 84)) },
                { 1, new Tuple<string, Color>("Bleu", Color.FromRgb(89, 84, 252)) }
            };
        }

        /// <summary>
        /// Initialize the player with player dicionary (function use for the unserialization)
        /// </summary>
        /// <param name="id"></param>
        private void InitPlayerById(int id)
        {
            Name = dictPlayers[id].Item1;
            Color = dictPlayers[id].Item2;
        }

        /// <summary>
        /// Singleton for player 0
        /// </summary>
        public static Player Player0
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

        /// <summary>
        /// Singleton for player 1
        /// </summary>
        public static Player Player1
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

        public static Player Winner
        {
            get
            {
                if (Player.Player0.Score == Player.Player1.Score) // when tied
                    return null;
                return Player.Player0.Score > Player.Player1.Score ? Player.Player0 : Player.Player1;
            }
        }

        public static Player Loser
        {
            get
            {
                if (Player.Player0.Score == Player.Player1.Score) // when tied
                    return null;
                return Player.Player0.Score > Player.Player1.Score ? Player.Player1 : Player.Player0;
            }
        }

        /// <summary>
        /// Return the value of the player
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

        /// <summary>
        /// Reset the player
        /// </summary>
        public void Reset()
        {
            Tokens = new List<Tuple<int, int>>();
            Stopwatch = new Stopwatch();
            PreviousTime = 0;
        }

        /// <summary>
        /// constructor for deserialization
        /// </summary>
        /// <param name="info"></param>
        /// <param name="context"></param>
        protected Player(SerializationInfo info, StreamingContext context)
        {
            Tokens = (List<Tuple<int, int>>)info.GetValue("Tokens", typeof(List<Tuple<int, int>>));
            Value = (int)info.GetValue("Value", typeof(int));
            Stopwatch = new Stopwatch(); // not serializable
            PreviousTime = (long)info.GetValue("PreviousTime", typeof(long)); //mini hack to avoid stopwatch not serializable
            InitPlayerById(Value);
        }

        /// <summary>
        /// Implementation serialization
        /// </summary>
        /// <param name="info"></param>
        /// <param name="context"></param>
        public void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            info.AddValue("Tokens", Tokens);
            info.AddValue("Value", Value);
            info.AddValue("PreviousTime", GetTime());
        }

        /// <summary>
        /// Get the time
        /// </summary>
        /// <returns></returns>
        public long GetTime()
        {
            return Stopwatch.ElapsedMilliseconds + PreviousTime;
        }

        /// <summary>
        /// Start the timer
        /// </summary>
        public void Start()
        {
            Stopwatch.Start();
        }

        /// <summary>
        /// Stop the timer
        /// </summary>
        public void Stop()
        {
            Stopwatch.Stop();
        }
    }
}
