using Microsoft.Win32;
using System;
using System.Collections;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;

namespace Othello
{
    internal class FileHandler
    {
        public void Write(MainWindow win)
        {
            SaveFileDialog saveFileDialog1 = new SaveFileDialog();
            //saveFileDialog1.Filter = "JPeg Image|*.jpg|Bitmap Image|*.bmp|Gif Image|*.gif";
            saveFileDialog1.Title = "Save a game File";
            saveFileDialog1.ShowDialog();
            StreamWriter outputFile = new StreamWriter(saveFileDialog1.FileName + ".moth", true);
            outputFile.Write(win.IsIA);
            outputFile.Write(win.board);

        }
        public void Read(MainWindow win)
        {
            OpenFileDialog openFileDialog1 = new OpenFileDialog();
            openFileDialog1.Title = "Open a game File";
            openFileDialog1.ShowDialog();
            MainWindow winTmp = null;
            StreamReader sr = new StreamReader(openFileDialog1.FileName, true);
        }
    }

}