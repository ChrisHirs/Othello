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
            throw new NotImplementedException();
        }

        public void play(int i, int j)
        {

        }

        public bool PlayMove(int column, int line, bool isWhite)
        {
            throw new NotImplementedException();
        }
    }
}