using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Piskvorky
{
    internal class PlayBoardState
    {
        public int[,] PlayBoardFields { get; set; }
        public List<string> WinnerFields { get; set; }
        public PlayBoardState()
        {
            PlayBoardFields = new int[20, 20];
            PlayboardReset();
            WinnerFields = new List<string>();
        }
        public bool IsFree(int X, int Y)
        {
            return PlayBoardFields[X, Y] > 2;
        }
        public bool EvaluateMove(int X, int Y, int player)
        {
            PlayBoardFields[X, Y] = player;
            for (int direction = 0; direction < 4; direction++)
            {
                WinnerFields.Clear();
                int charsInRows = 1;
                int directionX = 0;
                int directionY = 0;
                switch (direction)
                {
                    case 0: directionX = -1; break;
                    case 1: directionX = -1; directionY = -1; break;
                    case 2: directionY = -1; break;
                    case 3: directionX = 1; directionY = -1; break;
                }
                for (int orientation = -1; orientation <= 1; orientation+=2)
                {
                    for (int shift = 1; shift < 5; shift++)
                    {
                        int shiftX = shift * directionX * orientation;
                        int shiftY = shift * directionY * orientation;
                        if (X + shiftX >= 0 && X + shiftX < PlayBoardFields.GetLength(0)
                            && Y + shiftY >= 0 && Y + shiftY < PlayBoardFields.GetLength(1))
                        {
                            if (PlayBoardFields[X + shiftX, Y + shiftY] == player)
                            {
                                charsInRows++;
                                WinnerFields.Add((X + shiftX).ToString() + "," + (Y + shiftY).ToString());
                            }
                            else break;
                        }
                        else break;
                    }
                }               
                if (charsInRows >= 5)
                    return true;               
            }
            return false;
        }
        public void PlayboardReset()
        {
            for (int j = 0; j < PlayBoardFields.GetLength(1); j++)
            {
                for (int i = 0; i < PlayBoardFields.GetLength(0); i++)
                {
                    // 3 = clear field without score
                    PlayBoardFields[i, j] = 3;
                }
            }
        }
    }
}
