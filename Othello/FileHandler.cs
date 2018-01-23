/* Game saving/loading file handler.
 * 
 * Deni Gahlinger, Christophe Hirschi
 * 
 * January 2018
 */

using Microsoft.Win32;
using System.IO;

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
                            boardString += window.board.GetBoard()[i, j] + ",";
                        }
                    }
                    streamWriter.WriteLine(boardString);
                    //Timers
                    streamWriter.WriteLine(window.Player1Time);
                    streamWriter.WriteLine(window.Player2Time);
                    //IA
                    streamWriter.WriteLine(window.IsIA);
                }
            }
        }
        /// <summary>
        /// Loads game file
        /// </summary>
        public void Read()
        {
            OpenFileDialog openFileDialog = new OpenFileDialog { Title = "Open a game File", Filter = "Macarothello game file|*.moth" };
            openFileDialog.ShowDialog();
            //Reading in file... 
            using (StreamReader streamReader = new StreamReader(openFileDialog.FileName, true))
            {
                streamReader.ReadLine();
            }
        }
    }

}