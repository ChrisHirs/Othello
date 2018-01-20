/* Board logic.
 * 
 * Deni Gahlinger, Christophe Hirschi
 * 
 * January 2018
 */

using System;
using System.Diagnostics;

namespace Othello
{
    /// <summary>
    /// Board logic
    /// </summary>
    public class Board : IPlayable.IPlayable
    {
        //Ponderation matrix
        int[,] evalTabBegGame = {
                { 13, -3, -2, -2, -2, -2, -3, 13 },
                { -3, -3, -1, -1, -1, -1, -3, -3 },
                { -2, -1, 1, 1, 1, 1, -1, -2 },
                { -2, -1, 1, 5, 5, 1, -1, -2 },
                { -2, -1, 1, 5, 5, 1, -1, -2 },
                { -2, -1, 1, 1, 1, 1, -1, -2 },
                { -3, -3, -1, -1, -1, -1, -3, -3 },
                { 13, -3, -2, -2, -2, -2, -3, 13 },
            };
        int[,] evalTabEndGame = {
                { 9, 0, 3, 3, 3, 3, 0, 9 },
                { 0, 0, 1, 1, 1, 1, 0, 0 },
                { 3, 2, 1, 1, 1, 1, 2, 3 },
                { 3, 2, 1, 1, 1, 1, 2, 3 },
                { 3, 2, 1, 1, 1, 1, 2, 3 },
                { 3, 2, 1, 1, 1, 1, 2, 3 },
                { 0, 0, 1, 1, 1, 1, 0, 0 },
                { 9, 0, 3, 3, 3, 3, 0, 9 },
            };
        //End of the game
        bool ended = false;
        //State of the board
        int[,] boardState;
        //Accessors
        public bool Ended
        {
            get; set;
        }
        public int[,] BoardState
        {
            get; set;
        }
        /// <summary>
        /// Constructor
        /// </summary>
        public Board()
        {
            //Initializing board
            boardState = new int[8,8];
            for (int i = 0; i < 8; i++)
            {
                for (int j = 0; j < 8; j++)
                {
                    boardState[i, j] = -1;
                }
            }
            boardState[3, 3] = 0;
            boardState[4, 3] = 1;
            boardState[3, 4] = 1;
            boardState[4, 4] = 0;
        }
        /// <summary>
        /// Returns the number of black tiles
        /// </summary>
        /// <returns></returns>
        public int GetBlackScore()
        {
            int blackScore = 0;
            for (int i = 0; i < 8; i++)
            {
                for (int j = 0; j < 8; j++)
                {
                    if (boardState[i, j] == 1)
                    {
                        blackScore++;
                    }
                }
            }
            return blackScore;
        }
        /// <summary>
        /// Returns the number of white tiles on the board
        /// </summary>
        /// <returns></returns>
        public int GetWhiteScore()
        {
            int whiteScore = 0;
            for (int i = 0; i < 8; i++)
            {
                for (int j = 0; j < 8; j++)
                {
                    if (boardState[i, j] == 0)
                    {
                        whiteScore++;
                    }
                }
            }
            return whiteScore;
        }
        /// <summary>
        /// Returns a reference to a 2D array with the board status
        /// </summary>
        /// <returns>The 8x8 tiles status</returns>
        public int[,] GetBoard()
        {
            return boardState;
        }
        /// <summary>
        /// Returns the IA's name
        /// </summary>
        /// <returns></returns>
        public string GetName()
        {
            return "G7: Deni Gahlinger, Christophe Hischi";
        }
        /// <summary>
        /// Asks the game engine next (valid) move given a game position
        /// </summary>
        /// <param name="game">a 2D board with integer values: 0 for white 1 for black and -1 for empty tiles. First index for the column, second index for the line</param>
        /// <param name="level">an integer value to set the level of the IA, 5 normally</param>
        /// <param name="whiteTurn">true if white players turn, false otherwise</param>
        /// <returns>The column and line indices. Will return {-1,-1} as PASS if no possible move </returns>
        public Tuple<int, int> GetNextMove(int[,] game, int level, bool whiteTurn)
        {
            int opponentMark = 0;
            int playerMark = 1;
            int minOrMax = 1;
            if (whiteTurn)
            {
                opponentMark = 1;
                playerMark = 0;
                minOrMax = -1;
            }
            //First evaluation
            int val = EvalBoard(boardState, whiteTurn, whiteTurn);
            //Alphabeta
            Alphabeta(boardState, level, minOrMax, val, playerMark, opponentMark, whiteTurn, out int res, out Tuple<int, int> op);
            return op;
        }
        /// <summary>
        /// Alphabeta algorithm 
        /// </summary>
        /// <param name="board">board game</param>
        /// <param name="depth">algorithm depth</param>
        /// <param name="minOrMax">-1 or 1</param>
        /// <param name="parentValue">value of the last result</param>
        /// <param name="localplayerMark">local player</param>
        /// <param name="localopponentMark">loacl opponent</param>
        /// <param name="globalIsWhite">global white turn</param>
        /// <param name="val">value returned</param>
        /// <param name="op">operation returned as tuple</param>
        public void Alphabeta(int[,] board, int depth, int minOrMax, int parentValue, int localplayerMark, int localopponentMark, bool globalIsWhite, out int val, out Tuple<int, int> op)
        {
            //Setting local white turn
            bool localwhiteTurn = true;
            if (localplayerMark == 1)
            {
                localwhiteTurn = false;
            }
            //Executing algorithm while depth positive and board not full
            if (depth > 0 && !IsFinal(board, localwhiteTurn))
            {
                op = Tuple.Create(-1, -1);
                //Getting first value based on turn
                if (!globalIsWhite)
                {
                    val = minOrMax * -Int32.MaxValue - 1;
                }
                else
                {
                    val = minOrMax * (Int32.MinValue + 1);
                }
                //For each possible move
                for (int i = 0; i < 8; i++)
                {
                    for (int j = 0; j < 8; j++)
                    {
                        //If this move is playable
                        if (IsPlayableGeneric(board, i, j, localwhiteTurn))
                        {
                            int[,] newboard = (int[,])board.Clone();
                            newboard[i, j] = localplayerMark ;
                            ChangeSquaresAfterPlay(ref newboard, i, j, localwhiteTurn);
                            //Recursive search
                            Alphabeta(newboard, depth - 1, minOrMax * -1, val, localplayerMark, localopponentMark, globalIsWhite, out int newVal, out Tuple<int, int> dummy);
                            //Looking for the best move based on best value
                            if (newVal * minOrMax > val * minOrMax)
                            {
                                //Tuple of best move is created and returned
                                val = newVal;
                                op = Tuple.Create(i,j);
                                if(val * minOrMax > parentValue * minOrMax)
                                {
                                    depth = 0;
                                    break;
                                }
                            }
                        }
                    }
                }
            }
            else
            {
                //Otherwise it passes
                val = EvalBoard(board, globalIsWhite, localwhiteTurn);
                op = Tuple.Create(-1,-1);
            }
        }
        /// <summary>
        /// Counts playable moves for given player
        /// </summary>
        /// <param name="board">board game</param>
        /// <param name="isWhite">player turn</param>
        /// <returns></returns>
        public bool IsFinal(int[,] board, bool isWhite)
        {
            if (CountPlayableSquares(board, isWhite) == 0)
            {
                return true;
            }
            return false;
        }
        /// <summary>
        /// Evaluation function of best move
        /// </summary>
        /// <param name="board">board game</param>
        /// <param name="globalIsWhite">global white turn</param>
        /// <param name="localIsWhite">local white turn</param>
        /// <returns></returns>
        public int EvalBoard(int[,] board, bool globalIsWhite, bool localIsWhite)
        {
            //Counting remaining empty squares on board
            int emptySquares = CountEmptySquares();
            //Stating near end of game
            bool endOfGame = false;
            if (emptySquares < 25)
            {
                endOfGame = true;
            }
            //Setting player mark
            int playerMark = 1;
            if (globalIsWhite)
            {
                playerMark = 0;
            }
            int result = 0;
            //Ponderation matrix : each square has a ponderation number
            for (int i = 0; i < 8; i++)
            {
                for (int j = 0; j < 8; j++)
                {
                    if (board[i, j] != -1)
                    {
                        //Setting actual square based on existing counters. We actually check opponents counters as well
                        int squareMark;
                        if (board[i,j] == playerMark)
                        {
                            squareMark = 1;    
                        }
                        else
                        {
                            squareMark = -1;
                        }
                        //Specific logic for corners
                        if ((i == 0 && (j == 0 || j == 7)) || (i == 7 && (j == 0 || j == 7)))
                        {
                            //Prioritizing corners
                            result += 50;
                        }
                        //Specific logic for adjacent squares
                        else if ((i < 2 && (j < 2 || j > 5)) || (i > 5 && (j < 2 || j > 5)))
                        {
                            //If empty, avoiding move 
                            if (board[(i / 4) * 7, (j / 4) * 7] == -1)
                            {
                                result -= 50 * squareMark;
                            }
                            //If already player owned
                            else if (board[i / 7, j / 7] == playerMark)
                            {
                                result += 5 * squareMark;
                            }
                            //If opponenet owned
                            else
                            {
                                result += 3 * squareMark;
                            }
                        }
                        //Otherwise refering to ponderation matrix
                        else
                        {
                            if (!endOfGame)
                            {
                                result += evalTabBegGame[i, j] * squareMark;
                            }
                            else
                            {
                                result += evalTabEndGame[i, j] * squareMark;
                            }
                        }
                    }
                }
            }
            //Parity : prioritize moves where the opponent can not play to change parity
            if ((localIsWhite && CountEmptySquares() % 2 == 0) || (!localIsWhite && CountEmptySquares() % 2 == 1))
            {
                if (CountPlayableSquares(board, !localIsWhite) == 0)
                {
                    result += 100;
                }
            }
            //Mobility : prioritize moves where the opponent can play the least
            int factor = 8;
            result += (CountPlayableSquares(board, globalIsWhite) - CountPlayableSquares(board, !globalIsWhite)) * factor;
            //Returns result based on global white turn
            if (globalIsWhite)
            {
                return -result;
            }
            else
            {
                return result;
            }
        }
        /// <summary>
        /// Counts remaining empty squares 
        /// </summary>
        /// <returns>empty squares</returns>
        private int CountEmptySquares()
        {
            int nbempty = 0;
            for (int i = 0; i < 8; i++)
            {
                for (int j = 0; j < 8; j++)
                {
                    if (boardState[i, j] == -1)
                    {
                        nbempty++;
                    }
                }
            }
            return nbempty;
        }
        /// <summary>
        /// Returns true if the move is valid for specified color
        /// </summary>
        /// <param name="column">value between 0 and 7</param>
        /// <param name="line">value between 0 and 7</param>
        /// <param name="isWhite">define turn</param>
        /// <returns></returns>
        public bool IsPlayable(int column, int line, bool isWhite)
        {
            return IsPlayableGeneric(boardState, column, line, isWhite);
        }
        /// <summary>
        /// Returns true if the move is valid for specified color from board
        /// </summary>
        /// <param name="board">board game</param>
        /// <param name="column">value between 0 and 7</param>
        /// <param name="line">value between 0 and 7</param>
        /// <param name="isWhite">define turn</param>
        /// <returns></returns>
        public bool IsPlayableGeneric(int[,] board, int column, int line, bool isWhite)
        {
            column = column % 8;
            line = line % 8;
            bool result = false;
            //For a given coulmn and line
            if (board[column, line] == -1)
            {
                int opponentMark = 0;
                int playerMark = 1;
                if (isWhite)
                {
                    opponentMark = 1;
                    playerMark = 0;
                }
                //Setting directions
                int[,] directions = new int[8, 2] { { -1, -1 }, { 0, -1 }, { 1, -1 }, { -1, 0 }, { 1, 0 }, { -1, 1 }, { 0, 1 }, { 1, 1 } };
                //For each direction
                for (int i = 0; i < 8; i++)
                {
                    //For a given direction
                    if (column + directions[i, 0] < 8 && column + directions[i, 0] >= 0 && line + directions[i, 1] < 8 && line + directions[i, 1] >= 0)
                    {
                        //if in this direction there is an opponent counter
                        if (board[column + directions[i, 0], line + directions[i, 1]] == opponentMark)
                        {
                            int iIterator = column + directions[i, 0];
                            int jIterator = line + directions[i, 1];
                            //While there is a counter
                            while (iIterator >= 0 && jIterator >= 0 && iIterator < 8 && jIterator < 8 && board[iIterator, jIterator] != -1)
                            {
                                //If there is a player counter at the end then it is a valid turn
                                if (board[iIterator, jIterator] == playerMark)
                                {
                                    result = true;
                                }
                                iIterator += directions[i, 0];
                                jIterator += directions[i, 1];
                            }
                        }
                    }
                }
            }
            return result;
        }
        /// <summary>
        /// Will update the board status if the move is valid and return true
        /// Will return false otherwise (board is unchanged)
        /// </summary>
        /// <param name="column">value between 0 and 7</param>
        /// <param name="line">value between 0 and 7</param>
        /// <param name="isWhite">true for white move, false for black move</param>
        /// <returns></returns>
        public bool PlayMove(int column, int line, bool isWhite)
        {
            //If the move is playable
            if (IsPlayable(column, line, isWhite))
            {
                //Giving appropriate color to board
                if (isWhite)
                {
                    boardState[column, line] = 0;
                } else
                {
                    boardState[column, line] = 1;
                }
                //Flipping counters after the play 
                ChangeSquaresAfterPlay(ref boardState, column, line, isWhite);
                //Checking if not end of game
                if (CountPlayableSquares(boardState, !isWhite) > 0)
                {
                    return true;
                }
                else
                {
                    //If end of game
                    if(!(CountPlayableSquares(boardState, isWhite) > 0))
                    {
                        this.Ended = true;
                    }
                    return false;
                }
            }
            else
            {
                return false;
            }
            
        }
        /// <summary>
        /// Counts playable counters on board
        /// </summary>
        /// <param name="board">board game</param>
        /// <param name="isWhite">define turn</param>
        /// <returns></returns>
        public int CountPlayableSquares(int[,] board, bool isWhite)
        {
            int playableSquares = 0;
            for (int i = 0; i < 8; i++)
            {
                for (int j = 0; j < 8; j++)
                {
                    if (IsPlayableGeneric(board, i, j, isWhite))
                    {
                        playableSquares++;
                    }
                }
            }
            return playableSquares;
        }
        /// <summary>
        /// Flips counters after a play to given player
        /// </summary>
        /// <param name="board">board game</param>
        /// <param name="column">value between 0 and 7</param>
        /// <param name="line">value between 0 and 7</param>
        /// <param name="isWhite">true for white move, false for black move</param>
        private void ChangeSquaresAfterPlay(ref int[,] board, int column, int line, bool isWhite)
        {
            int opponentMark = 0;
            int playerMark = 1;
            if (isWhite)
            {
                opponentMark = 1;
                playerMark = 0;
            }
            //Setting directions
            int[,] directions = new int[8, 2] { { -1, -1 }, { 0, -1 }, { 1, -1 }, { -1, 0 }, { 1, 0 }, { -1, 1 }, { 0, 1 }, { 1, 1 } };
            //for each direction
            for (int i = 0; i < 8; i++)
            {
                int iIterator = column + directions[i, 0];
                int jIterator = line + directions[i, 1];
                bool isDirectionValid = false;
                //Defining if it is a valid direction or not
                while (iIterator >= 0 && jIterator >= 0 && iIterator < 8 && jIterator < 8 && board[iIterator, jIterator] != -1)
                {
                    if (board[iIterator, jIterator] == playerMark)
                    {
                        isDirectionValid = true;
                    }
                    iIterator += directions[i, 0];
                    jIterator += directions[i, 1];
                }
                //If valid direction
                if (isDirectionValid)
                {
                    iIterator = column + directions[i, 0];
                    jIterator = line + directions[i, 1];
                    //Flipping opponent counters in that direction
                    while (board[iIterator, jIterator] == opponentMark)
                    {
                        board[iIterator, jIterator] = playerMark;
                        iIterator += directions[i, 0];
                        jIterator += directions[i, 1];
                    }
                }
            }
        }
    }
}