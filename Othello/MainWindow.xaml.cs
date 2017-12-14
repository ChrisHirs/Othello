using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Othello
{
    /// <summary>
    /// Logique d'interaction pour MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void BoardHoover(object sender, MouseEventArgs e)
        {
            Canvas board = (Canvas)sender;
            Rectangle r = new Rectangle();

            int h = (int)board.ActualHeight;
            double dH = h / 8.0;
            int w = (int)board.ActualWidth;
            double dW = w / 8.0;

            double eX = e.GetPosition(board).X;
            double eY = e.GetPosition(board).Y;

            r.Width = dW;
            r.Height = dH;

            Canvas.SetTop(r, eY % dH);
            Canvas.SetLeft(r, eX % dW);
            
            r.Fill = Brushes.Black;
            board.Children.Add(r);
        }

        private void Canvas_Loaded(object sender, RoutedEventArgs e)
        {
            Canvas board = (Canvas)sender;
            int h = (int)board.ActualHeight;
            int w = (int)board.ActualWidth;
            for (int i = 0; i < 9; i++)
            {
                Line myLine = new Line();
                myLine.Stroke = System.Windows.Media.Brushes.LightSteelBlue;
                myLine.X1 = 0;
                myLine.X2 = w;
                myLine.Y1 = h / 8.0 * i;
                myLine.Y2 = h / 8.0 * i;
                myLine.StrokeThickness = 2;
                board.Children.Add(myLine);
                myLine = new Line();
                myLine.Stroke = System.Windows.Media.Brushes.LightSteelBlue;
                myLine.Y1 = 0;
                myLine.Y2 = h;
                myLine.X1 = w / 8.0 * i;
                myLine.X2 = w / 8.0 * i;
                myLine.StrokeThickness = 2;
                board.Children.Add(myLine);
            }
        }
    }
}
