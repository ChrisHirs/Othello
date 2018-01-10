using System;
using System.Diagnostics;

namespace Othello
{
    public class Board : IPlayable
    {
        int[,] boardState;
        public int[,] BoardState
        {
            get; set;
        }
        public Board()
        {
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

        public int[,] GetBoard()
        {
            return boardState;
        }

        public string GetName()
        {
            return "Christophe Hischi, Deni Gahlinger";
        }

        public Tuple<int, int> GetNextMove(int[,] game, int level, bool whiteTurn)
        {
            Debug.WriteLine("GET NEXT MOVE");
            int opponentMark = 0;
            int playerMark = 1;
            if (whiteTurn)
            {
                opponentMark = 1;
                playerMark = 0;
            }

            int val = EvalBoard(boardState, playerMark, opponentMark);
            int res;
            Tuple<int, int> op;
            alphabeta(boardState, level, playerMark, val, playerMark, opponentMark, out res, out op);

            return op;
        }
        public void alphabeta(int[,] board, int depth, int minOrMax, int parentValue, int playerMark, int opponentMark, out int val, out Tuple<int, int> op)
        {
            Debug.WriteLine("Depth : " + depth);
            Debug.WriteLine("bard : " + board);
            if (depth < 0)
            {
                op = null;
                val = minOrMax * Int32.MaxValue - 1;
                bool whiteTurn = true;
                if (playerMark == 1)
                {
                    whiteTurn = false;
                }
                for (int i = 0; i < 8; i++)
                {
                    for (int j = 0; j < 8; j++)
                    {
                        if (IsPlayableGeneric(boardState, i, j, whiteTurn))
                        {
                            int[,] newboard = (int[,])board.Clone();
                            newboard[i, j] = playerMark;
                            int newVal;
                            Tuple<int, int> dummy;
                            alphabeta(newboard, depth - 1, minOrMax, val, playerMark, opponentMark, out newVal, out dummy);
                            if(newVal * minOrMax > val * minOrMax)
                            {
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
                val = EvalBoard(board, playerMark, opponentMark);
                op = null;
            }
            
        }
        public bool isFinal(int[,] board)
        {

            return true;
        }
        public int EvalBoard(int[,] board, int playerMark, int opponentMark)
        {
            int result = 0;
            int force = 1;
            for (int i = 0; i < 8; i++)
            {
                for (int j = 0; j < 8; j++)
                {
                    if (board[i, j] != -1)
                    {
                        force = 1;
                        if((i == 0 || i == 7 || i == 0 || i == 7))
                        {
                            force = 2;
                        }
                        if(board[i,j] == playerMark)
                        {
                            result += force;
                        } else if (board[i, j] == opponentMark)
                        {
                            result -= force;
                        }
                    }
                }
            }
            return result;
        }

        public int GetWhiteScore()
        {
            int whiteScore = 0;
            for (int i = 0; i < 8; i++)
            {
                for (int j = 0; j < 8; j++)
                {
                    if(boardState[i, j] == 0)
                    {
                        whiteScore++;
                    }
                }
            }
            return whiteScore;
        }

        public bool IsPlayable(int column, int line, bool isWhite)
        {
            return IsPlayableGeneric(boardState, column, line, isWhite);
        }
        public bool IsPlayableGeneric(int[,] board, int column, int line, bool isWhite)
        {
            column = column % 8;
            line = line % 8;
            bool result = false;
            if (board[column, line] == -1)
            {
                int opponentMark = 0;
                int playerMark = 1;
                if (isWhite)
                {
                    opponentMark = 1;
                    playerMark = 0;
                }
                int[,] directions = new int[8, 2] { { -1, -1 }, { 0, -1 }, { 1, -1 }, { -1, 0 }, { 1, 0 }, { -1, 1 }, { 0, 1 }, { 1, 1 } };
                for (int i = 0; i < 8; i++)
                {
                    if (column + directions[i, 0] < 8 && column + directions[i, 0] > 0 && line + directions[i, 1] < 8 && line + directions[i, 1] > 0)
                    {
                        if (board[column + directions[i, 0], line + directions[i, 1]] == opponentMark)
                        {
                            int iIterator = column + directions[i, 0];
                            int jIterator = line + directions[i, 1];
                            while (iIterator >= 0 && jIterator >= 0 && iIterator < 8 && jIterator < 8 && board[iIterator, jIterator] != -1)
                            {
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
        public bool PlayMove(int column, int line, bool isWhite)
        {
            if (IsPlayable(column, line, isWhite))
            {
                if (isWhite)
                {
                    boardState[column, line] = 0;
                } else
                {

                    boardState[column, line] = 1;
                }
                changeSquaresAfterPlay(column, line, isWhite);
                return true;
            } else
            {
                return false;
            }
        }
        private void changeSquaresAfterPlay(int column, int line, bool isWhite)
        {
            int opponentMark = 0;
            int playerMark = 1;
            if (isWhite)
            {
                opponentMark = 1;
                playerMark = 0;
            }
            int[,] directions = new int[8, 2] { { -1, -1 }, { 0, -1 }, { 1, -1 }, { -1, 0 }, { 1, 0 }, { -1, 1 }, { 0, 1 }, { 1, 1 } };
            for (int i = 0; i < 8; i++)
            {
                int iIterator = column + directions[i, 0];
                int jIterator = line + directions[i, 1];
                bool isDirectionValid = false;
                while (iIterator >= 0 && jIterator >= 0 && iIterator < 8 && jIterator < 8 && boardState[iIterator, jIterator] != -1)
                {
                    if (boardState[iIterator, jIterator] == playerMark)
                    {
                        isDirectionValid = true;
                    }
                    iIterator += directions[i, 0];
                    jIterator += directions[i, 1];
                }
                if (isDirectionValid)
                {
                    iIterator = column + directions[i, 0];
                    jIterator = line + directions[i, 1];
                    while (boardState[iIterator, jIterator] == opponentMark)
                    {
                        boardState[iIterator, jIterator] = playerMark;
                        iIterator += directions[i, 0];
                        jIterator += directions[i, 1];
                    }
                }
            }
        }
    }
}