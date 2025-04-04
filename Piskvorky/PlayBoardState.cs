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
            PlayBoardFields = new int[20, 20];
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
                    int[] humansInRow = {0,0};
                    int[] computersInRow = {0,0};
                    int[] block = {5,5};
                    for (int orientation = -1; orientation <= 1; orientation += 2)
                    {
                        int orientationOrder;
                        if (orientation == -1)
                        {
                            orientationOrder = 0;
                        }
                        else
                        {
                            orientationOrder = 1;
                        }
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
                                int checkField = (PlayBoardFields[X + shiftX, Y + shiftY]);
                                if  (checkField== human && humanInRow)
                                {
                                    humansInRow[orientationOrder]++;
                                    computerInRow = false;
                                }
                                else if (checkField == computer && computerInRow)
                                {
                                    computersInRow[orientationOrder]++;
                                    humanInRow = false;
                                }
                                else if (checkField > 2)
                                {
                                    humanInRow = false;
                                    computerInRow = false;
                                }
                                else
                                {
                                    if ((humansInRow[orientationOrder] > 0 && checkField == computer) 
                                        || (computersInRow[orientationOrder] > 0 && checkField == human))
                                    {
                                        block[orientationOrder] = shift;
                                        break;
                                    }
                                }
                            }
                            else
                            {
                                block[orientationOrder] = shift;
                                break;
                            }
                        }
                    }
                    int humansInRowTotal = humansInRow[0] + humansInRow[1];
                    int computersInRowTotal = computersInRow[0] + computersInRow[1];
                    if ((humansInRow[0] == block[0]-1)|| (humansInRow[1] == block[1] - 1))
                    {
                        if (humansInRowTotal < 4)
                            humansInRowTotal--;
                    }
                    if ((computersInRow[0] == block[0] - 1) || (computersInRow[1] == block[1] - 1))
                    {
                        if (computersInRowTotal < 4)
                            computersInRowTotal--;
                    }
                    if (block[0] + block[1] < 6)
                    {
                        humansInRowTotal = 0;
                        computersInRowTotal = 0;
                    }
                    else if (humansInRow[0] > 0 && computersInRow[1] > 0)
                    {
                        if (block[0] < 5)
                        {
                            humansInRowTotal = 0;
                        }
                        if (block[1] < 5)
                        {
                            computersInRowTotal = 0;
                        }
                    }
                    else if (computersInRow[0] > 0 && humansInRow[1] > 0)
                    {
                        if (block[0] < 5)
                        {
                            computersInRowTotal = 0;
                        }
                        if (block[1] < 5)
                        {
                            humansInRowTotal = 0;
                        }
                    }
                    fieldScore = EvaluateFieldScore(humansInRowTotal, computersInRowTotal, fieldScore);
                }
                PlayBoardFields[X,Y] = fieldScore;
            }
        }
        private int EvaluateFieldScore(int humansInRow,  int computersInRow, int fieldScore)
        {

            switch (humansInRow)
            {
                case 1: fieldScore += 5; break;
                case 2: fieldScore += 22; break;
                case 3: fieldScore += 89; break;
                case 4: fieldScore += 1441; break;
            }
            switch (computersInRow)
            {
                case 1: fieldScore += 4; break;
                case 2: fieldScore += 21; break;
                case 3: fieldScore += 360; break;
                case 4: fieldScore += 6000; break;
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
