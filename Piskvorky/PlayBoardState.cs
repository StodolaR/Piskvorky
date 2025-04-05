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
                    for (int shift = 1; shift < 6; shift++)
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
                    int[] freeInRow = {0,0};
                    int[] freeInHumanRow = {0,0};
                    int[] freeInComputerRow = {0,0};
                    int[] block = {6,6};
                    for (int orientation = -1; orientation <= 1; orientation += 2)
                    {
                        int orientationOrder = 0;
                        switch (orientation)
                        {
                            case -1: orientationOrder = 0; break;
                            case 1: orientationOrder = 1; break;
                        }    
                        bool humanInRow = true;
                        bool computerInRow = true;
                        for (int shift = 1; shift < 6; shift++)
                        {
                            int shiftX = shift * DirectionCoords[direction, 0] * orientation;
                            int shiftY = shift * DirectionCoords[direction, 1] * orientation;
                            if (X + shiftX >= 0 && X + shiftX < PlayBoardFields.GetLength(0)
                                && Y + shiftY >= 0 && Y + shiftY < PlayBoardFields.GetLength(1))
                            {
                                //podminky
                                int checkField = (PlayBoardFields[X + shiftX, Y + shiftY]);
                                if  (checkField== human && humanInRow && shift < 5)
                                {
                                    humansInRow[orientationOrder]++;
                                    computerInRow = false;
                                    if (freeInRow[orientationOrder] == 1)
                                    {
                                        freeInHumanRow[orientationOrder] = 1;
                                    }
                                }
                                else if (checkField == computer && computerInRow && shift < 5)
                                {
                                    computersInRow[orientationOrder]++;
                                    humanInRow = false;
                                    if (freeInRow[orientationOrder] == 1)
                                    {
                                        freeInComputerRow[orientationOrder] = 1;
                                    }
                                }
                                else if (checkField > 2)
                                {
                                    freeInRow[orientationOrder]++;
                                    if (freeInRow[orientationOrder] > 1)
                                    {
                                        humanInRow = false;
                                        computerInRow = false;
                                    }                                  
                                }
                                else
                                {
                                    if ((humansInRow[orientationOrder] > 0 && checkField == computer) || (computersInRow[orientationOrder] > 0 && checkField == human))
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
                    int freeInComputerRows = freeInComputerRow[0] + freeInComputerRow[1];
                    int freeInHumanRows = freeInHumanRow[0] + freeInHumanRow[1];
                    if ((humansInRow[0] == block[0] - 1) || (humansInRow[1] == block[1] - 1) || 
                        (freeInHumanRow[0] == 1 && humansInRow[0] == block[0] - 2) || (freeInHumanRow[1] == 1 && humansInRow[1] == block[1] - 2))
                    {
                        if (humansInRowTotal < 4)
                            humansInRowTotal--;
                    }
                    if ((computersInRow[0] == block[0] - 1) || (computersInRow[1] == block[1] - 1) || 
                        (freeInComputerRow[0] == 1 && computersInRow[0] == block[0] - 2) || (freeInComputerRow[1] == 1 && computersInRow[1] == block[1] - 2))
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
                    fieldScore = EvaluateFieldScore(humansInRowTotal, computersInRowTotal, fieldScore, freeInHumanRows, freeInComputerRows);
                }
                PlayBoardFields[X,Y] = fieldScore;
            }
        }
        private int EvaluateFieldScore(int humansInRow,  int computersInRow, int fieldScore, int freeInHumanRows, int freeInComputerRows)
        {

            switch (humansInRow)
            {
                case 1: fieldScore += 5; fieldScore -= 2 * freeInHumanRows; break;
                case 2: fieldScore += 22; fieldScore -= 7 * freeInHumanRows; break;
                case 3: fieldScore += 89; fieldScore -= 30 * freeInHumanRows; break;
                case > 3: fieldScore += 1441; fieldScore -= 500 * freeInHumanRows; break;
            }
            switch (computersInRow)
            {
                case 1: fieldScore += 4; fieldScore -= 2 * freeInComputerRows; break;
                case 2: fieldScore += 21; fieldScore -= 7 * freeInComputerRows; break;
                case 3: fieldScore += 360; fieldScore -= 120 * freeInComputerRows; break;
                case > 3: fieldScore += 6000; fieldScore -= 2000 * freeInComputerRows; break;
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
