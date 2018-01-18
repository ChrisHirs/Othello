using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using System.Timers;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;

namespace Othello
{
    /// <summary>
    /// Logique d'interaction pour MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window, INotifyPropertyChanged
    {
        FileHandler f = new FileHandler();
        bool isIA;
        public bool IsIA
        {
            get; set;
        }
        bool isSkinForPlayer1;
        bool isPlaying;
        bool wasPlaying;
        int whiteScore;
        int blackScore;
        TimeSpan player1TimeS;
        TimeSpan player2TimeS;
        string player1Time;
        string player2Time;
        DateTime turnStartTime;
        DispatcherTimer mainTimer;
        bool turnToWhite = true;
        Rectangle rectHover = new Rectangle();
        public Board board = new Board();
        int skinIdPlayer1;
        int skinIdPlayer2;
        Brush skinPlayer2 = new ImageBrush(Imaging.CreateBitmapSourceFromHBitmap(Properties.Resources.m_blueberry.GetHbitmap(), IntPtr.Zero, Int32Rect.Empty, BitmapSizeOptions.FromEmptyOptions()));
        Brush skinPlayer1 = new ImageBrush(Imaging.CreateBitmapSourceFromHBitmap(Properties.Resources.m_banana.GetHbitmap(), IntPtr.Zero, Int32Rect.Empty, BitmapSizeOptions.FromEmptyOptions()));

        public event PropertyChangedEventHandler PropertyChanged;

        public int WhiteScore
        {
            get { return whiteScore; }
            set { whiteScore = value; RaisePropertyChanged("WhiteScore"); }
        }
        public int BlackScore
        {
            get { return blackScore; }
            set { blackScore = value; RaisePropertyChanged("BlackScore"); }
        }
        public string Player1Time
        {
            get { return player1Time; }
            set { player1Time = value; RaisePropertyChanged("Player1Time"); }
        }
        public string Player2Time
        {
            get { return player2Time; }
            set { player2Time = value; RaisePropertyChanged("Player2Time"); }
        }

        public BitmapSource BitmapSource { get; }

        void RaisePropertyChanged(string propertyName)
        {
            if(PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }
        public MainWindow()
        {
            skinIdPlayer1 = 0;
            skinIdPlayer2 = 0;
            isIA = true;
            isSkinForPlayer1 = true;
            isPlaying = false;
            InitializeComponent();
            /*ImageBrush bgImage = new ImageBrush();
            bgImage.ImageSource = new BitmapImage(new Uri(@"imgs\BackgroundOthello.jpg", UriKind.Relative));
            BitmapSource = Imaging.CreateBitmapSourceFromHBitmap(Properties.Resources.BackgroundOthello.GetHbitmap(), IntPtr.Zero, Int32Rect.Empty, BitmapSizeOptions.FromEmptyOptions());*/
            this.Background = new ImageBrush(Imaging.CreateBitmapSourceFromHBitmap(Properties.Resources.BackgroundOthello.GetHbitmap(), IntPtr.Zero, Int32Rect.Empty, BitmapSizeOptions.FromEmptyOptions()));
            player1TimeS = new TimeSpan();
            player2TimeS = new TimeSpan();
            Player2Time = player2TimeS.ToString("mm\\:ss\\:ff");
            Player1Time = player1TimeS.ToString("mm\\:ss\\:ff");
            btnSkinPlayerA.Background = skinPlayer1;
            btnSkinPlayerB.Background = skinPlayer2;
        }

        private void BoardHover(object sender, MouseEventArgs e)
        {
            if (isPlaying)
            {
                rectHover.Name = "hoverRect";

                double h = canBoard.ActualHeight;
                double dH = h / 8.0;
                double w = canBoard.ActualWidth;
                double dW = w / 8.0;

                double eX = e.GetPosition(canBoard).X;
                double eY = e.GetPosition(canBoard).Y;

                rectHover.Width = dW;
                rectHover.Height = dH;

                int squareIdI = (int)(eX / dW);
                int squareIdJ = (int)(eY / dH);
                
                if (turnToWhite)
                {
                    rectHover.Fill = skinPlayer1.Clone();
                }
                else
                {
                    rectHover.Fill = skinPlayer2.Clone();
                }

                if (board.IsPlayable(squareIdI, squareIdJ, turnToWhite))
                {

                    Canvas.SetTop(rectHover, (squareIdJ * dH));
                    Canvas.SetLeft(rectHover, (squareIdI * dW));
                    rectHover.Fill.Opacity = 0.55;
                }
                else
                {

                    Canvas.SetTop(rectHover, eY - dH / 2);
                    Canvas.SetLeft(rectHover, eX - dW / 2);
                    rectHover.Fill.Opacity = 0.2;
                }
                rectHover.InvalidateVisual();
                if (canBoard.Children.Contains(rectHover))
                {
                    if (eX > w || eX > h || eX < 0 || eY < 0)
                    {
                        canBoard.Children.Remove(rectHover);
                    }
                }
                else
                {
                    canBoard.Children.Add(rectHover);
                }
            }
        }

        private void Canvas_Loaded(object sender, RoutedEventArgs e)
        {
            printBoard();
        }

        private void Window_SizeChanged(object sender, SizeChangedEventArgs e)
        {
        }
        private void printBoard()
        {

            canBoard.Children.Clear();

            int h = (int)canBoard.ActualHeight;
            int w = (int)canBoard.ActualWidth;
            
            Rectangle bg = new Rectangle();

            bg.Height = h;
            bg.Width = w;

            Canvas.SetTop(bg, 0);
            Canvas.SetLeft(bg, 0);
            bg.Fill = Brushes.White;

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
            Rectangle textileFilter = new Rectangle();

            textileFilter.Height = h;
            textileFilter.Width = w;

            Canvas.SetTop(textileFilter, 0);
            Canvas.SetLeft(textileFilter, 0);
            ImageBrush textileBrush = new ImageBrush();
            textileBrush.ImageSource = new BitmapImage(new Uri(@"imgs\texttexture.png", UriKind.Relative));
            textileFilter.Fill = textileBrush;

            canBoard.Children.Add(textileFilter);
            for (int i = 0; i < 8; i++)
            {
                for (int j = 0; j < 8; j++)
                {
                    if(board.GetBoard()[i,j] >= 0)
                    {
                        Rectangle square = new Rectangle();

                        square.Height = h/8.0;
                        square.Width = w/8.0;
                        Canvas.SetTop(square, j * h / 8.0);
                        Canvas.SetLeft(square, i * w / 8.0);
                        if (board.GetBoard()[i, j] == 0)
                        {
                            square.Fill = skinPlayer1.Clone();
                        } else
                        {
                            square.Fill = skinPlayer2.Clone();
                        }
                        canBoard.Children.Add(square);
                    }
                }
            }
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            rectHover.Fill = Brushes.Black;
            canBoard.Children.Add(rectHover);
            printBoard();
        }

        private void canBoard_MouseLeave(object sender, MouseEventArgs e)
        {
            if (canBoard.Children.Contains(rectHover))
            {
                canBoard.Children.Remove(rectHover);
            }
        }

        private void canBoard_MouseUp(object sender, MouseButtonEventArgs e)
        {
            if (isPlaying && (!isIA || turnToWhite))
            {
                double dH = canBoard.ActualHeight / 8.0;
                double dW = canBoard.ActualWidth / 8.0;

                double eX = e.GetPosition(canBoard).X;
                double eY = e.GetPosition(canBoard).Y;

                int squareIdI = (int)(eX / dW);
                int squareIdJ = (int)(eY / dH);

                if (board.IsPlayable(squareIdI, squareIdJ, turnToWhite))
                {
                    bool changePlayer = board.PlayMove(squareIdI, squareIdJ, turnToWhite);
                    if(board.Ended)
                    {
                        isPlaying = false;
                        string winner = "1";
                        if(board.GetBlackScore() > board.GetWhiteScore())
                        {
                            winner = lblPlayer2.Content.ToString();
                        }
                        lblWinner.Content = "The winner is : " + winner;
                        lblWinner.Visibility = Visibility.Visible;
                    }
                    if (changePlayer)
                    {
                        turnToWhite = !turnToWhite;
                    }
                }
            }
            if (isPlaying && isIA && !turnToWhite)
            {
                Debug.WriteLine("in IA turn");

                bool changePlayer = false;
                while (!changePlayer && !board.Ended)
                {
                    Tuple<int, int> IA = board.GetNextMove(board.GetBoard(), 1, turnToWhite);
                    Debug.WriteLine("in while");
                    changePlayer = board.PlayMove(IA.Item1, IA.Item2, turnToWhite);
                    if (board.Ended)
                    {
                        Debug.WriteLine("in if board ended");
                        isPlaying = false;
                        lblWinner.Content = "The winner is : Player  1";
                        if (board.GetBlackScore() > board.GetWhiteScore())
                        {
                            lblWinner.Content = "The winner is : " + lblPlayer2.Content;
                        }
                        lblWinner.Visibility = Visibility.Visible;
                    }
                }
                if (changePlayer)
                {
                    Debug.WriteLine("changing player");
                    turnToWhite = !turnToWhite;
                }
            }
            printBoard();
            this.WhiteScore = board.GetWhiteScore();
            this.BlackScore = board.GetBlackScore();
        }

        private void btnMenuEnter(object sender, MouseEventArgs e)
        {
            Button btn = (Button)sender;
            btn.Opacity = 1;
        }

        private void btnMenuLeave(object sender, MouseEventArgs e)
        {
            Button btn = (Button)sender;
            btn.Opacity = 0.7;
        }

        private void btnNew_Click(object sender, RoutedEventArgs e)
        {
            startGame();
            isPlaying = true;
            printBoard();
        }
        private void updateTimeStrings(object sender, EventArgs e)
        {
            if (isPlaying)
            {
                if (!turnToWhite)
                {
                    player2TimeS += DateTime.Now - turnStartTime;
                    Player2Time = player2TimeS.ToString("mm\\:ss\\:ff");
                }
                else
                {
                    player1TimeS += DateTime.Now - turnStartTime;
                    Player1Time = player1TimeS.ToString("mm\\:ss\\:ff");
                }
                turnStartTime = DateTime.Now;
            }
        }
        private void startGame()
        {
            lblWinner.Visibility = Visibility.Hidden;
            player1TimeS = new TimeSpan();
            player2TimeS = new TimeSpan();
            Player2Time = player2TimeS.ToString("mm\\:ss\\:ff");
            Player1Time = player1TimeS.ToString("mm\\:ss\\:ff");
            board = new Board();
            turnToWhite = true;
            mainTimer = new DispatcherTimer();
            mainTimer.Tick += new EventHandler(updateTimeStrings);
            mainTimer.Interval = new TimeSpan(0, 0, 0, 0, 10);
            mainTimer.Start();
            turnStartTime = DateTime.Now;
            whiteScore = 2;
            blackScore = 2;
        }

        private void ChangeSkin(object sender, RoutedEventArgs e)
        {
            wasPlaying = isPlaying;
            if(((Button)sender).Name == btnSkinPlayerA.Name)
            {
                isSkinForPlayer1 = true;
            } else
            {
                isSkinForPlayer1 = false;
            }
            isPlaying = false;
            grdSkinSelector.Visibility = Visibility.Visible;
        }

        private void btnSelectSkin(object sender, RoutedEventArgs e)
        {
            if(wasPlaying)
            {
                isPlaying = true;
            }
            turnStartTime = DateTime.Now;
            grdSkinSelector.Visibility = Visibility.Hidden;
            if (isSkinForPlayer1)
            {
                skinPlayer1 = ((Button)sender).Background;
            } else
            {
                skinPlayer2 = ((Button)sender).Background;
            }
            btnSkinPlayerA.Background = skinPlayer1;
            btnSkinPlayerB.Background = skinPlayer2;
            printBoard();
        }

        private void btnIA_Click(object sender, RoutedEventArgs e)
        {
            if (isIA)
            {
                isIA = false;
                lblPlayer2.Content = "Player 2";
            }
            else
            {
                isIA = true;
                lblPlayer2.Content = "I.A.";
            }
        }

        private void btnSave_Click(object sender, RoutedEventArgs e)
        {
            f.Write(this);
        }

        private void btnOpen_Click(object sender, RoutedEventArgs e)
        {
            f.Read(this);
        }
    }
}
