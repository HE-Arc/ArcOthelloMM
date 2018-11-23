using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace ArcOthelloMM
{
    /// <summary>
    /// Bridge class between the Board and the Window
    /// </summary>
    class BoardManager
    {
        private Board board;
        private Window window;

        private List<String> labelsX;
        private List<String> labelsY;

        public BoardManager(Board board, Window window, List<String> labelsX, List<String> labelsY)
        {
            this.board = board;
            this.window = window;
            this.labelsX = labelsX;
            this.labelsY = labelsY;
        }
    }
}
