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
            SaveFileDialog saveFileDialog1 = new SaveFileDialog { Title = "Save a game File" };
            saveFileDialog1.ShowDialog();
            StreamWriter streamWriter = new StreamWriter(saveFileDialog1.FileName + ".moth", true);

        }
        /// <summary>
        /// Loads game file
        /// </summary>
        /// <param name="window">mainwindow</param>
        public void Read(MainWindow window)
        {
            OpenFileDialog openFileDialog1 = new OpenFileDialog { Title = "Open a game File" };
            openFileDialog1.ShowDialog();
            StreamReader streamReader = new StreamReader(openFileDialog1.FileName, true);
        }
    }

}