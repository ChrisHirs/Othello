/* Game saving/loading file handler.
 * 
 * Deni Gahlinger, Christophe Hirschi
 * 
 * January 2018
 */

using Microsoft.Win32;
using System;
using System.IO;
using System.Linq;

namespace Othello
{
    /// <summary>
    /// Game saving/loading file handler
    /// </summary>
    internal class FileHandler
    {
        /// <summary>
        /// Writes game file
        /// </summary>
        /// <param name="window">mainwindow</param>
        public void Write(MainWindow window)
        {
            //Opening file dialog
            SaveFileDialog saveFileDialog = new SaveFileDialog { Title = "Save a game File", Filter = "Macarothello game file|*.moth" };
            saveFileDialog.ShowDialog();
            if (saveFileDialog.FileName != "") {
                //Writing in file...
                using (StreamWriter streamWriter = new StreamWriter(saveFileDialog.FileName, false))
                {
                    //Game board
                    string boardString = "";
                    for (int i = 0; i < 8; i++) {
                        for (int j  = 0; j < 8; j++)
                        {
                            boardString += window.board.GetBoard()[i, j];
                            if (!(i == 7 && j == 7))
                            {
                                boardString += ",";
                            }
                        }
                    }
                    streamWriter.WriteLine(boardString);
                    //Timers
                    streamWriter.WriteLine(window.Player1TimeS.ToString());
                    streamWriter.WriteLine(window.Player2TimeS.ToString());
                    //IA
                    streamWriter.WriteLine(window.IsIA);
                    //Skins
                    streamWriter.WriteLine(window.imagePlayer1.Source);
                    streamWriter.WriteLine(window.imagePlayer2.Source);
                }
            }
        }
        /// <summary>
        /// Loads game file
        /// </summary>
        /// <param name="board">board out from file</param>
        /// <param name="player1Time">player 1 time out from file</param>
        /// <param name="player2Time">player 2 time out from file</param>
        /// <param name="isIA">IA bool out from file</param>
        /// <param name="skinSource1">skin source for player 1 out from file</param>
        /// <param name="skinSource2">skin source for player 2 out from file</param>
        public void Read(out int[,] board, out TimeSpan player1Time, out TimeSpan player2Time, out bool isIA, out string skinSource1, out string skinSource2)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog { Title = "Open a game File", Filter = "Macarothello game file|*.moth" };
            openFileDialog.ShowDialog();
            //Reading in file... 
            using (StreamReader streamReader = new StreamReader(openFileDialog.FileName, true))
            {
                //Game board
                string boardString = streamReader.ReadLine();
                int[] boardTab = boardString.Split(',').Select(int.Parse).ToArray();
                int[,] boardTmp = new int[8, 8];
                int r = 0;
                int c = 0;
                for (int i= 0; i < 64; i++)
                {
                    boardTmp[r, c] = boardTab[i];
                    c++;
                    if (c == 8)
                    {
                        r++;
                        c = 0;
                    }
                }
                board = boardTmp;
                //Timers
                player1Time = TimeSpan.Parse(streamReader.ReadLine());
                player2Time = TimeSpan.Parse(streamReader.ReadLine());
                //IA
                isIA = bool.Parse(streamReader.ReadLine());
                //Skins
                skinSource1 = streamReader.ReadLine();
                skinSource2 = streamReader.ReadLine();
            }
        }
    }

}