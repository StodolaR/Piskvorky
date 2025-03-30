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
        public List<(int, int)> RecalcFields { get; set; }
        public List<string> BestScoreFields { get; set; }
        public PlayBoardState()
        {
            PlayBoardFields = new int[20, 20];
            PlayboardReset();
            WinnerFields = new List<string>();
            RecalcFields = new List<(int,int)>();
            BestScoreFields = new List<string>();
        }
        public bool IsFree(int X, int Y)
        {
            return PlayBoardFields[X, Y] > 2;
        }
        public bool EvaluateMove(int X, int Y, int player)
        {
            PlayBoardFields[X, Y] = player;
            RecalcFields.Clear();
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
                    bool isInRows = true;
                    for (int shift = 1; shift < 5; shift++)
                    {                       
                        int shiftX = shift * directionX * orientation;
                        int shiftY = shift * directionY * orientation;
                        if (X + shiftX >= 0 && X + shiftX < PlayBoardFields.GetLength(0)
                            && Y + shiftY >= 0 && Y + shiftY < PlayBoardFields.GetLength(1))
                        {
                            if (PlayBoardFields[X + shiftX, Y + shiftY] == player && isInRows==true)
                            {
                                charsInRows++;
                                WinnerFields.Add((X + shiftX).ToString() + "," + (Y + shiftY).ToString());
                            }
                            else
                            {
                                isInRows = false;
                            }
                            if (PlayBoardFields[X + shiftX, Y + shiftY] > 2)
                            {
                                RecalcFields.Add( new (X + shiftX, Y + shiftY));
                            }
                        }
                        else break;
                    }
                }               
                if (charsInRows >= 5)
                    return true;
                RecalculateFields();
            }
            return false;
        }
        private void RecalculateFields()
        {
            int player = 1;
            int computer = 2;
            foreach (var recalcField in RecalcFields)
            {
                int X = recalcField.Item1;
                int Y = recalcField.Item2;
                int fieldScore = 3;
                for (int direction = 0; direction < 4; direction++)
                {
                    int directionX = 0;
                    int directionY = 0;
                    int playersInRow = 0;
                    int computersInRow = 0;
                    switch (direction)
                    {
                        case 0: directionX = -1; break;
                        case 1: directionX = -1; directionY = -1; break;
                        case 2: directionY = -1; break;
                        case 3: directionX = 1; directionY = -1; break;
                    }
                    for (int orientation = -1; orientation <= 1; orientation += 2)
                    {
                        bool playerInRow = true;
                        bool computerInRow = true;
                        //bool block = false;
                        //int last = free;
                        for (int shift = 1; shift < 5; shift++)
                        {
                            int shiftX = shift * directionX * orientation;
                            int shiftY = shift * directionY * orientation;
                            if (X + shiftX >= 0 && X + shiftX < PlayBoardFields.GetLength(0)
                                && Y + shiftY >= 0 && Y + shiftY < PlayBoardFields.GetLength(1))
                            {
                                //podminky
                                if (PlayBoardFields[X + shiftX, Y + shiftY] == player && playerInRow)
                                {
                                    playersInRow++;
                                    computerInRow = false;
                                    //last = player;
                                }
                                else if (PlayBoardFields[X + shiftX, Y + shiftY] == computer && computerInRow)
                                {
                                    computersInRow++;
                                    playerInRow = false;
                                    //last = computer;
                                }
                                //else
                                //{
                                //    if ((last == player && PlayBoardFields[X + shiftX, Y + shiftY] == computer)
                                //        || (last == computer && PlayBoardFields[X + shiftX, Y + shiftY] == player))
                                //    {
                                //        //block = true;
                                //    }
                                //    last = free;
                                //}
                                if (PlayBoardFields[X + shiftX, Y + shiftY] > 2)
                                {
                                    playerInRow = false;
                                    computerInRow = false;
                                }
                            }
                            else
                            {
                                //block = true;
                                break;
                            }
                        }
                    }
                    switch (playersInRow)
                    {
                        case 1: fieldScore += 7; break;
                        case 2: fieldScore += 32;break;
                        case 3: fieldScore += 140; break;
                        case 4: fieldScore += 399; break;
                    }
                    switch (computersInRow)
                    {
                        case 1: fieldScore += 6; break;
                        case 2: fieldScore += 33; break;
                        case 3: fieldScore += 141; break;
                        case 4: fieldScore += 400; break;
                    }
                }
                PlayBoardFields[X,Y] = fieldScore;
            }
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
