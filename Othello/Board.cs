using System;

namespace Othello
{
    internal class Board : IPlayable
    {
        int[,] boardState;
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
            throw new NotImplementedException();
        }

        public int[,] GetBoard()
        {
            return boardState;
        }

        public string GetName()
        {
            throw new NotImplementedException();
        }

        public Tuple<int, int> GetNextMove(int[,] game, int level, bool whiteTurn)
        {
            throw new NotImplementedException();
        }

        public int GetWhiteScore()
        {
            throw new NotImplementedException();
        }

        public bool IsPlayable(int column, int line, bool isWhite)
        {
            column = column % 8;
            line = line % 8;
            bool result = false;
            if (boardState[column,line] == -1)
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
                        if (boardState[column + directions[i, 0], line + directions[i, 1]] == opponentMark)
                        {
                            int iIterator = column + directions[i, 0];
                            int jIterator = line + directions[i, 1];
                            while (iIterator >= 0 && jIterator >= 0 && iIterator < 8 && jIterator < 8 && boardState[iIterator, jIterator] != -1)
                            {
                                if (boardState[iIterator, jIterator] == playerMark)
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
                /*if (column + directions[i, 0] < 8 && column + directions[i, 0] > 0 && line + directions[i, 1] < 8 && line + directions[i, 1] > 0)
                {
                    if (boardState[column + directions[i, 0], line + directions[i, 1]] == opponentMark)
                    {
                        int iIterator = column + directions[i, 0];
                        int jIterator = line + directions[i, 1];
                        while (iIterator > 0 && jIterator > 0 && iIterator < 8 && jIterator < 8)
                        {
                            if (boardState[iIterator, jIterator] == playerMark)
                            {
                                result = true;
                            }
                            iIterator += directions[i, 0];
                            jIterator += directions[i, 1];
                        }
                    }
                }*/
            }
        }
    }
}