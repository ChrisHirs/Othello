using System;
using System.Diagnostics;

namespace Othello
{
    public class Board : IPlayable.IPlayable
    {
        bool ended = false;
        public bool Ended
        {
            get; set;
        }
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

            int val = EvalBoard(boardState, whiteTurn, whiteTurn);
            int res;
            Tuple<int, int> op;
            alphabeta(boardState, level, playerMark, val, playerMark, opponentMark, whiteTurn, out res, out op);
            Debug.WriteLine("OP : <" + op.Item1 + ", " + op.Item2 + ">");
            return op;
        }
        public void alphabeta(int[,] board, int depth, int minOrMax, int parentValue, int localplayerMark, int localopponentMark, bool globalIsWhite, out int val, out Tuple<int, int> op)
        {
            bool localwhiteTurn = true;
            if (localplayerMark == 1)
            {
                localwhiteTurn = false;
            }
            if (depth > 0)
            {
                op = Tuple.Create(-1, -1); ;
                val = minOrMax * -Int32.MaxValue - 1;
                for (int i = 0; i < 8; i++)
                {
                    for (int j = 0; j < 8; j++)
                    {
                        if (IsPlayableGeneric(board, i, j, localwhiteTurn))
                        {
                            int[,] newboard = (int[,])board.Clone();
                            newboard[i, j] = localplayerMark ;
                            changeSquaresAfterPlay(ref newboard, i, j, localwhiteTurn);
                            int newVal;
                            Tuple<int, int> dummy;
                            alphabeta(newboard, depth - 1, minOrMax * -1, val, localplayerMark, localopponentMark, globalIsWhite, out newVal, out dummy);
                            if (newVal * minOrMax > val * minOrMax)
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
                val = EvalBoard(board, globalIsWhite, localwhiteTurn);
                op = Tuple.Create(-1,-1);
            }
        }
        public bool isFinal(int[,] board)
        {
            if (countPlayableSquares(board, true) == 0 && countPlayableSquares(board, false) == 0)
            {
                return true;
            }
            return false;
        }
        public int EvalBoard(int[,] board, bool globalIsWhite, bool localIsWhite)
        {
            int[,] ponderTab = { 
                { 5, 1, 1, 2, 2, 1, 1, 5 },
                { 1, 1, 2, 2, 2, 2, 1, 1 },
                { 1, 2, 1, 2, 2, 1, 2, 1 },
                { 2, 2, 2, 1, 1, 2, 2, 2 },
                { 2, 2, 2, 1, 1, 2, 2, 2 },
                { 1, 2, 1, 2, 2, 1, 2, 1 },
                { 1, 1, 2, 2, 2, 2, 1, 1 },
                { 5, 1, 1, 2, 2, 1, 1, 5 },
            };

            int opponentMark = 0;
            int playerMark = 1;
            if (globalIsWhite)
            {
                opponentMark = 1;
                playerMark = 0;
            }
            int result = 0;
            for (int i = 0; i < 8; i++)
            {
                for (int j = 0; j < 8; j++)
                {
                    if (board[i, j] != -1)
                    {
                        if (board[i,j] == playerMark)
                        {
                            result += ponderTab[i,j];
                        }
                        else if (board[i, j] == opponentMark)
                        {
                            result -= ponderTab[i, j];
                        }
                    }
                }
            }
            result *= 3;
            //Mobility
            if (localIsWhite == globalIsWhite)
            {
                result -= countPlayableSquares(board, localIsWhite);
            }
            else
            {
                result += countPlayableSquares(board, localIsWhite);
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
                    if (column + directions[i, 0] < 8 && column + directions[i, 0] >= 0 && line + directions[i, 1] < 8 && line + directions[i, 1] >= 0)
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
                changeSquaresAfterPlay(ref boardState, column, line, isWhite);
                Debug.WriteLine("nb playable for " + !isWhite + " : " + countPlayableSquares(boardState, !isWhite));
                Debug.WriteLine("nb playable for " + isWhite + " : " + countPlayableSquares(boardState, isWhite));
                if (countPlayableSquares(boardState, !isWhite) > 0)
                {
                    return true;
                }
                else
                {
                    if(!(countPlayableSquares(boardState, isWhite) > 0))
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
        public int countPlayableSquares(int[,] board, bool isWhite)
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
        private void changeSquaresAfterPlay(ref int[,] b, int column, int line, bool isWhite)
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
                while (iIterator >= 0 && jIterator >= 0 && iIterator < 8 && jIterator < 8 && b[iIterator, jIterator] != -1)
                {
                    if (b[iIterator, jIterator] == playerMark)
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
                    while (b[iIterator, jIterator] == opponentMark)
                    {
                        b[iIterator, jIterator] = playerMark;
                        iIterator += directions[i, 0];
                        jIterator += directions[i, 1];
                    }
                }
            }
        }
    }
}