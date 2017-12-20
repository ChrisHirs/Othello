﻿using System;
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
        Rectangle rectHover = new Rectangle();
        public MainWindow()
        {
            InitializeComponent();

            canBoard.Children.Add(rectHover);
            initBoard();
        }

        private void BoardHover(object sender, MouseEventArgs e)
        {
            rectHover.Name = "hoverRect";

            int h = (int)canBoard.ActualHeight;
            double dH = h / 8.0;
            int w = (int)canBoard.ActualWidth;
            double dW = w / 8.0;

            double eX = e.GetPosition(canBoard).X;
            double eY = e.GetPosition(canBoard).Y;

            rectHover.Width = dW;
            rectHover.Height = dH;

            lblDebug.Content = "X : " + eX + ", Y : " + eY;

            Canvas.SetTop(rectHover, (int)(eY / dH) * dH);
            Canvas.SetLeft(rectHover, (int)(eX / dW) * dW);
            
            rectHover.Fill = Brushes.Black;
            rectHover.InvalidateVisual();
        }

        private void Canvas_Loaded(object sender, RoutedEventArgs e)
        {
            initBoard();
        }

        private void Window_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            int boardDimensions = (int)Width - 50;
            if (Height < Width)
            {
                boardDimensions = (int)Height - 100;
            }
            canBoard.Height = boardDimensions;
            canBoard.Width = canBoard.Height;
            canBoard.Children.Clear();
            initBoard();
        }
        private void initBoard()
        {
            int h = (int)canBoard.ActualHeight;
            int w = (int)canBoard.ActualWidth;

            Rectangle bg = new Rectangle();
            bg.Height = h;
            bg.Width = w;

            Canvas.SetTop(bg, 0);
            Canvas.SetLeft(bg, 0);
            bg.Fill = Brushes.Yellow;

            canBoard.Children.Add(bg);

            Brush myBrush = new SolidColorBrush(Color.FromArgb(100,200,10,10));
            

            for (int i = 0; i < 4; i++)
            {
                Line myLine = new Line();
                myLine.Stroke = myBrush;
                myLine.X1 = 0;
                myLine.X2 = w;
                myLine.Y1 = (h / 4.0 * i) + (w/16.0);
                myLine.Y2 = (h / 4.0 * i) + (h/16.0);
                myLine.StrokeThickness = (int)(h/8.0);
                canBoard.Children.Add(myLine);
                myLine = new Line();
                myLine.Stroke = myBrush;
                myLine.Y1 = 0;
                myLine.Y2 = h;
                myLine.X1 = (w / 4.0 * i) + (w/16.0);
                myLine.X2 = (h / 4.0 * i) + (h/16.0);
                myLine.StrokeThickness = (int)(w/8.0);
                canBoard.Children.Add(myLine);
            }
        }
    }
}
