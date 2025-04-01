using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace Piskvorky
{
    internal class PlayBoardState
    { 
        public int[,] PlayBoardFields { get; set; }
        public List<string> WinnerFields { get; set; }
        public List<(int, int)> RecalcFields { get; set; }
        public List<string> BestScoreFields { get; set; }
        public int[,] DirectionCoords { get; set; }
        public const int human = 1;
        public const int computer = 2;
        public PlayBoardState()
        {
            PlayBoardFields = new int[15, 15];
            PlayboardReset();
            WinnerFields = new List<string>();
            RecalcFields = new List<(int,int)>();
            BestScoreFields = new List<string>();
            DirectionCoords = new int[4, 2] { {-1,0}, {-1,-1}, {0,-1},{1,-1} };
        }
        public bool IsFree(int X, int Y)
        {
            return PlayBoardFields[X, Y] > 2;
        }
        public bool EvaluateMove(int X, int Y, int player)
        {
            PlayBoardFields[X, Y] = player;
            bool IsWinner = CheckWin(X,Y,player);
            if (!IsWinner)
            {
                RecalculateFields();
            }            
            return IsWinner;
        }
        private bool CheckWin(int X, int Y, int player)
        {
            RecalcFields.Clear();
            for (int direction = 0; direction < 4; direction++)
            {
                WinnerFields.Clear();
                int charsInRow = 1;
                for (int orientation = -1; orientation <= 1; orientation += 2)
                {
                    bool isInRow = true;
                    for (int shift = 1; shift < 5; shift++)
                    {
                        int shiftX = shift * DirectionCoords[direction, 0] * orientation;
                        int shiftY = shift * DirectionCoords[direction, 1] * orientation;
                        if (X + shiftX >= 0 && X + shiftX < PlayBoardFields.GetLength(0)
                            && Y + shiftY >= 0 && Y + shiftY < PlayBoardFields.GetLength(1))
                        {
                            if (PlayBoardFields[X + shiftX, Y + shiftY] == player && isInRow == true)
                            {
                                charsInRow++;
                                WinnerFields.Add((X + shiftX).ToString() + "," + (Y + shiftY).ToString());
                            }
                            else
                            {
                                isInRow = false;
                            }
                            if (PlayBoardFields[X + shiftX, Y + shiftY] > 2)
                            {
                                RecalcFields.Add(new(X + shiftX, Y + shiftY));
                            }
                        }
                        else break;
                    }
                }
                if (charsInRow >= 5)
                    return true;
            }
            return false;
        }
        private void RecalculateFields()
        {
            foreach (var recalcField in RecalcFields)
            {
                int X = recalcField.Item1;
                int Y = recalcField.Item2;
                int fieldScore = 3;
                for (int direction = 0; direction < 4; direction++)
                {
                    int humansInRow = 0;
                    int computersInRow = 0;
                    for (int orientation = -1; orientation <= 1; orientation += 2)
                    {
                        bool humanInRow = true;
                        bool computerInRow = true;
                        for (int shift = 1; shift < 5; shift++)
                        {
                            int shiftX = shift * DirectionCoords[direction, 0] * orientation;
                            int shiftY = shift * DirectionCoords[direction, 1] * orientation;
                            if (X + shiftX >= 0 && X + shiftX < PlayBoardFields.GetLength(0)
                                && Y + shiftY >= 0 && Y + shiftY < PlayBoardFields.GetLength(1))
                            {
                                //podminky
                                if (PlayBoardFields[X + shiftX, Y + shiftY] == human && humanInRow)
                                {
                                    humansInRow++;
                                    computerInRow = false;
                                }
                                else if (PlayBoardFields[X + shiftX, Y + shiftY] == computer && computerInRow)
                                {
                                    computersInRow++;
                                    humanInRow = false;
                                }
                                else break;
                            }
                            else break;
                        }
                    }
                    fieldScore = EvaluateFieldScore(humansInRow, computersInRow, fieldScore);
                }
                PlayBoardFields[X,Y] = fieldScore;
            }
        }
        private int EvaluateFieldScore(int humansInRow,  int computersInRow, int fieldScore)
        {

            switch (humansInRow)
            {
                case 1: fieldScore += 7; break;
                case 2: fieldScore += 32; break;
                case 3: fieldScore += 140; break;
                case 4: fieldScore += 500; break;
            }
            switch (computersInRow)
            {
                case 1: fieldScore += 6; break;
                case 2: fieldScore += 33; break;
                case 3: fieldScore += 600; break;
                case 4: fieldScore += 2000; break;
            }
            return fieldScore;
        }
        public string GetBestScoreField()
        {
            int max = 4;
            for (int j = 0; j < PlayBoardFields.GetLength(1); j++)
            {
                for (int i = 0; i < PlayBoardFields.GetLength(0); i++)
                {
                    if (PlayBoardFields[i,j] > max)
                    {
                        max = PlayBoardFields[i,j];
                        BestScoreFields.Clear();
                    }
                    if (PlayBoardFields[i, j] == max)
                    {
                        BestScoreFields.Add(i.ToString() + "," + j.ToString());
                    }
                }
            }
            Random random = new Random();
            return BestScoreFields[random.Next(BestScoreFields.Count())];
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
