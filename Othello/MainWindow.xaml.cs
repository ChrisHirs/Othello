/* MainWindow.xaml interaction logic.
 * 
 * Deni Gahlinger, Christophe Hirschi
 * 
 * January 2018
 */

using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Windows.Threading;

namespace Othello
{
    /// <summary>
    /// MainWindow.xaml interaction logic
    /// </summary>
    public partial class MainWindow : Window, INotifyPropertyChanged
    {
        //Scores
        int whiteScore;
        int blackScore;
        //Game booleans
        bool isIA;
        bool isSkinForPlayer1;
        bool isPlaying;
        bool wasPlaying;
        bool turnToWhite = true;
        //Player times
        string player1Time;
        string player2Time;
        TimeSpan player1TimeS;
        TimeSpan player2TimeS;
        DateTime turnStartTime;
        DispatcherTimer mainTimer;
        //Miscellaneous
        Rectangle rectHover = new Rectangle();
        FileHandler fileHandler = new FileHandler();
        //Base skins (Banana for player 1 and Blueberry for IA/player 2)
        ImageBrush skinPlayer1 = new ImageBrush(Imaging.CreateBitmapSourceFromHBitmap(Properties.Resources.m_banana.GetHbitmap(), IntPtr.Zero, Int32Rect.Empty, BitmapSizeOptions.FromEmptyOptions()));
        ImageBrush skinPlayer2 = new ImageBrush(Imaging.CreateBitmapSourceFromHBitmap(Properties.Resources.m_blueberry.GetHbitmap(), IntPtr.Zero, Int32Rect.Empty, BitmapSizeOptions.FromEmptyOptions()));
        //Board
        public Board board = new Board();
        //Databinding
        public event PropertyChangedEventHandler PropertyChanged;
        //Accessors
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
        void RaisePropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        /// <summary>
        /// Constructor
        /// </summary>
        public MainWindow()
        {
            //Intializing
            InitializeComponent();
            //Game booleans
            isIA = true;
            isSkinForPlayer1 = true;
            isPlaying = false;
            //Background
            this.Background = new ImageBrush(Imaging.CreateBitmapSourceFromHBitmap(Properties.Resources.BackgroundOthello.GetHbitmap(), IntPtr.Zero, Int32Rect.Empty, BitmapSizeOptions.FromEmptyOptions()));
            //Player times
            player1TimeS = new TimeSpan();
            player2TimeS = new TimeSpan();
            Player2Time = player2TimeS.ToString("mm\\:ss\\:ff");
            Player1Time = player1TimeS.ToString("mm\\:ss\\:ff");
            //Player skins applied to menu buttons
            btnSkinPlayerA.Background = skinPlayer1;
            Image imagePlayer1 = new Image { Source = skinPlayer1.ImageSource };
            btnSkinPlayerA.Content = imagePlayer1;
            btnSkinPlayerB.Background = skinPlayer2;
            Image imagePlayer2 = new Image { Source = skinPlayer2.ImageSource };
            btnSkinPlayerB.Content = imagePlayer2;
        }
        /// <summary>
        /// Shows where a player can possibly play by anchoring his counter where he can actually play
        /// otherwise shows his counter nearly visible when mouse is on board
        /// </summary>
        /// <param name="sender">sender object</param>
        /// <param name="e">mouse event</param>
        private void BoardHover(object sender, MouseEventArgs e)
        {
            //If game has started
            if (isPlaying)
            {
                rectHover.Name = "hoverRect";
                //Getting board canvas dimensions
                double canvasHeight = canBoard.ActualHeight;
                double dH = canvasHeight / 8.0;
                double canvasWidth = canBoard.ActualWidth;
                double dW = canvasWidth / 8.0;
                //Getting mouse positions
                double mouseX = e.GetPosition(canBoard).X;
                double mouseY = e.GetPosition(canBoard).Y;
                //Setting counter dimensions
                rectHover.Width = dW;
                rectHover.Height = dH;
                //Setting counter positions
                int squareIdI = (int)(mouseX / dW);
                int squareIdJ = (int)(mouseY / dH);
                //Setting counter skin based on player turn
                if (turnToWhite)
                {
                    rectHover.Fill = skinPlayer1.Clone();
                }
                else
                {
                    rectHover.Fill = skinPlayer2.Clone();
                }
                //Anchoring counter if possible play
                if (board.IsPlayable(squareIdI, squareIdJ, turnToWhite))
                {
                    Canvas.SetTop(rectHover, (squareIdJ * dH));
                    Canvas.SetLeft(rectHover, (squareIdI * dW));
                    rectHover.Fill.Opacity = 0.6;
                }
                else
                {
                    Canvas.SetTop(rectHover, mouseY - dH / 2);
                    Canvas.SetLeft(rectHover, mouseX - dW / 2);
                    rectHover.Fill.Opacity = 0.2;
                }
                rectHover.InvalidateVisual();
                //Counter only if on board
                if (canBoard.Children.Contains(rectHover))
                {
                    if (mouseX > canvasWidth || mouseX > canvasHeight || mouseX < 0 || mouseY < 0)
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
        /// <summary>
        /// Displays board when XAML canvas has loaded
        /// </summary>
        /// <param name="sender">sender object</param>
        /// <param name="e">routed event</param>
        private void Canvas_Loaded(object sender, RoutedEventArgs e)
        {
            PrintBoard();
        }
        /// <summary>
        /// Creates and displays board
        /// </summary>
        private void PrintBoard()
        {
            canBoard.Children.Clear();
            //Getting board canvas dimensions
            int canvasHeight = (int)canBoard.ActualHeight;
            int canvasWidth = (int)canBoard.ActualWidth;
            //Background
            Rectangle bg = new Rectangle
            {
                Height = canvasHeight,
                Width = canvasWidth
            };
            //Modifying canvas based on background
            Canvas.SetTop(bg, 0);
            Canvas.SetLeft(bg, 0);
            bg.Fill = Brushes.White;
            canBoard.Children.Add(bg);
            //Brush
            Brush myBrush = new SolidColorBrush(Color.FromArgb(100,200,10,10));
            //Painting board lines
            for (int i = 0; i < 4; i++)
            {
                Line myLine = new Line
                {
                    Stroke = myBrush,
                    X1 = 0,
                    X2 = canvasWidth,
                    Y1 = (canvasHeight / 4.0 * i) + (canvasWidth / 16.0),
                    Y2 = (canvasHeight / 4.0 * i) + (canvasHeight / 16.0),
                    StrokeThickness = (int)(canvasHeight / 8.0)
                };
                canBoard.Children.Add(myLine);
                myLine = new Line
                {
                    Stroke = myBrush,
                    Y1 = 0,
                    Y2 = canvasHeight,
                    X1 = (canvasWidth / 4.0 * i) + (canvasWidth / 16.0),
                    X2 = (canvasHeight / 4.0 * i) + (canvasHeight / 16.0),
                    StrokeThickness = (int)(canvasWidth / 8.0)
                };
                canBoard.Children.Add(myLine);
            }
            //Texture rectangle
            Rectangle textileFilter = new Rectangle
            {
                Height = canvasHeight,
                Width = canvasWidth
            };
            //Modifying canvas based on rectangle
            Canvas.SetTop(textileFilter, 0);
            Canvas.SetLeft(textileFilter, 0);
            //Filling rectangle with texture image
            ImageBrush textileBrush = new ImageBrush { ImageSource = new BitmapImage(new Uri(@"imgs\texttexture.png", UriKind.Relative)) };
            textileFilter.Fill = textileBrush;
            //Painting board squares
            canBoard.Children.Add(textileFilter);
            //Painting existing counters on board
            for (int i = 0; i < 8; i++)
            {
                for (int j = 0; j < 8; j++)
                {
                    if(board.GetBoard()[i,j] >= 0)
                    {
                        Rectangle square = new Rectangle
                        {
                            Height = canvasHeight / 8.0,
                            Width = canvasWidth / 8.0
                        };
                        Canvas.SetTop(square, j * canvasHeight / 8.0);
                        Canvas.SetLeft(square, i * canvasWidth / 8.0);
                        //Setting counter image based on player
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
        /// <summary>
        /// Called when window has loaded. Displays board
        /// </summary>
        /// <param name="sender">sender object</param>
        /// <param name="e">routed event</param>
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            rectHover.Fill = Brushes.Black;
            canBoard.Children.Add(rectHover);
            PrintBoard();
        }
        /// <summary>
        /// Called when mouse leaves canvas board. Removes counter from mouse
        /// </summary>
        /// <param name="sender">sender object</param>
        /// <param name="e">mouse event</param>
        private void CanBoard_MouseLeave(object sender, MouseEventArgs e)
        {
            if (canBoard.Children.Contains(rectHover))
            {
                canBoard.Children.Remove(rectHover);
            }
        }
        /// <summary>
        /// Called when mouse click is released on canvas board. Makes moves for player and IA
        /// </summary>
        /// <param name="sender">sender object</param>
        /// <param name="e">mouse event</param>
        private void CanBoard_MouseUp(object sender, MouseButtonEventArgs e)
        {
            //If game as started and it is player turn
            if (isPlaying && (!isIA || turnToWhite))
            {
                //Getting canvas board dimensions
                double canvasHeight = canBoard.ActualHeight / 8.0;
                double canvasWidth = canBoard.ActualWidth / 8.0;
                //Getting mouse positions
                double eX = e.GetPosition(canBoard).X;
                double eY = e.GetPosition(canBoard).Y;
                //Setting counter positions
                int squareIdI = (int)(eX / canvasWidth);
                int squareIdJ = (int)(eY / canvasHeight);
                //If move can be played
                if (board.IsPlayable(squareIdI, squareIdJ, turnToWhite))
                {
                    //Move is played
                    bool changePlayer = board.PlayMove(squareIdI, squareIdJ, turnToWhite);
                    //If game is finished
                    if(board.Ended)
                    {
                        //Displaying a label with the winner
                        DisplayWinner();
                    }
                    //Changing players
                    if (changePlayer)
                    {
                        turnToWhite = !turnToWhite;
                    }
                }
            }
            //If game as started and it is IA turn
            if (isPlaying && isIA && !turnToWhite)
            {
                bool changePlayer = false;
                while (!changePlayer && !board.Ended)
                {
                    //IA finds its next move to play
                    Tuple<int, int> IA = board.GetNextMove(board.GetBoard(), 1, turnToWhite);
                    //Move is played
                    changePlayer = board.PlayMove(IA.Item1, IA.Item2, turnToWhite);
                    //If game is finished
                    if (board.Ended)
                    {
                        //Displaying a label with the winner
                        DisplayWinner();
                    }
                }
                //Changing players
                if (changePlayer)
                {
                    turnToWhite = !turnToWhite;
                }
            }
            //Updating board
            PrintBoard();
            //Updating scores
            this.WhiteScore = board.GetWhiteScore();
            this.BlackScore = board.GetBlackScore();
        }
        /// <summary>
        /// Displays a label with the winner
        /// </summary>
        private void DisplayWinner()
        {
            //Setting end game
            isPlaying = false;
            string winner = "Player 1";
            //If IA/player 2 has more counters, winner is IA/player 2
            if (board.GetBlackScore() > board.GetWhiteScore())
            {
                winner = lblPlayer2.Content.ToString();
            }
            lblWinner.Content = "The winner is : " + winner;
            //Displaying winning label
            lblWinner.Visibility = Visibility.Visible;
        }
        /// <summary>
        /// Called when mouse touches a button. Changes button opacity
        /// </summary>
        /// <param name="sender">sender object</param>
        /// <param name="e">mouse event</param>
        private void BtnMenuEnter(object sender, MouseEventArgs e)
        {
            Button btn = (Button)sender;
            btn.Opacity = 1;
        }
        /// <summary>
        /// Called when mouse leaves a button. Changes button opacity
        /// </summary>
        /// <param name="sender">sender object</param>
        /// <param name="e">mouse event</param>
        private void BtnMenuLeave(object sender, MouseEventArgs e)
        {
            Button btn = (Button)sender;
            btn.Opacity = 1;
        }
        /// <summary>
        /// Updates and displays time during players turns
        /// </summary>
        /// <param name="sender">sender object</param>
        /// <param name="e">event</param>
        private void UpdateTimeStrings(object sender, EventArgs e)
        {
            //If game has started
            if (isPlaying)
            {
                //If is player 2 turn
                if (!turnToWhite)
                {
                    //Updating player 2's time
                    player2TimeS += DateTime.Now - turnStartTime;
                    Player2Time = player2TimeS.ToString("mm\\:ss\\:ff");
                }
                //If is player 1 turn
                else
                {
                    //updating player 1's time
                    player1TimeS += DateTime.Now - turnStartTime;
                    Player1Time = player1TimeS.ToString("mm\\:ss\\:ff");
                }
                //Turn started time
                turnStartTime = DateTime.Now;
            }
        }
        /// <summary>
        /// Starts a new game
        /// </summary>
        private void StartGame()
        {
            //Hiding winning label
            lblWinner.Visibility = Visibility.Hidden;
            //Reseting player times
            player1TimeS = new TimeSpan();
            player2TimeS = new TimeSpan();
            Player2Time = player2TimeS.ToString("mm\\:ss\\:ff");
            Player1Time = player1TimeS.ToString("mm\\:ss\\:ff");
            //Starting a new board
            board = new Board();
            turnToWhite = true;
            //Starting main timer
            mainTimer = new DispatcherTimer();
            mainTimer.Tick += new EventHandler(UpdateTimeStrings);
            mainTimer.Interval = new TimeSpan(0, 0, 0, 0, 10);
            mainTimer.Start();
            //turn srated time
            turnStartTime = DateTime.Now;
            //Scores
            whiteScore = 2;
            blackScore = 2;
        }
        /// <summary>
        /// Dispplays skins grid based on clicked button
        /// </summary>
        /// <param name="sender">sender object</param>
        /// <param name="e">routed event</param>
        private void ChangeSkin(object sender, RoutedEventArgs e)
        {
            //Acknowledging player who wants to change skin based on clicked button
            if(((Button)sender).Name == btnSkinPlayerA.Name)
            {
                isSkinForPlayer1 = true;
            } else
            {
                isSkinForPlayer1 = false;
            }
            //Stopping game
            wasPlaying = isPlaying;
            isPlaying = false;
            //Displaying skins grid
            grdSkinSelector.Visibility = Visibility.Visible;
        }
        /// <summary>
        /// Called when "New" button is clicked. Starts new game
        /// </summary>
        /// <param name="sender">sender object</param>
        /// <param name="e">routed event</param>
        private void btnNew_Click(object sender, RoutedEventArgs e)
        {
            StartGame();
            isPlaying = true;
            PrintBoard();
        }
        /// <summary>
        /// Called when skin selection buttons are clicked. Changes player skin
        /// </summary>
        /// <param name="sender">sender object</param>
        /// <param name="e">routed event</param>
        private void btnSelectSkin(object sender, RoutedEventArgs e)
        {
            //Resuming game
            if(wasPlaying)
            {
                isPlaying = true;
            }
            turnStartTime = DateTime.Now;
            //Hiding skins grid
            grdSkinSelector.Visibility = Visibility.Hidden;
            //Changing skin fot player who wanted to change
            if (isSkinForPlayer1)
            {
                skinPlayer1 = (ImageBrush)((Button)sender).Background;
            } else
            {
                skinPlayer2 = (ImageBrush)((Button)sender).Background;
            }
            //Player skins applied to menu buttons
            btnSkinPlayerA.Background = skinPlayer1;
            Image imagePlayer1 = new Image { Source = skinPlayer1.ImageSource };
            btnSkinPlayerA.Content = imagePlayer1;
            btnSkinPlayerB.Background = skinPlayer2;
            Image imagePlayer2 = new Image { Source = skinPlayer2.ImageSource };
            btnSkinPlayerB.Content = imagePlayer2;
            //updating board
            PrintBoard();
        }
        /// <summary>
        /// Called when "IA/Player" button is clicked. Enables/disables IA
        /// </summary>
        /// <param name="sender">sender object</param>
        /// <param name="e">routed event</param>
        private void btnIA_Click(object sender, RoutedEventArgs e)
        {
            if (isIA)
            {
                isIA = false;
                //Updating button contents
                lblPlayer2.Content = "Player 2";
                btnIA.Content = "I.A";
            }
            else
            {
                isIA = true;
                //Updating button contents
                lblPlayer2.Content = "I.A.";
                btnIA.Content = "Player";
            }
        }
        /// <summary>
        /// Called when "Save" button is clicked. Writes game file
        /// </summary>
        /// <param name="sender">sender object</param>
        /// <param name="e">routed event</param>
        private void btnSave_Click(object sender, RoutedEventArgs e)
        {
            fileHandler.Write(this);
        }
        /// <summary>
        /// Called when "Open" button is clicked. Loads game file
        /// </summary>
        /// <param name="sender">sender object</param>
        /// <param name="e">routed event</param>
        private void btnOpen_Click(object sender, RoutedEventArgs e)
        {
            fileHandler.Read(this);
        }
        /// <summary>
        /// Called when window size has changed. Not used
        /// </summary>
        /// <param name="sender">sender object</param>
        /// <param name="e">size changed event</param>
        private void Window_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            //pass
        }
    }
}
