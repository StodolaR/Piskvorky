using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Piskvorky
{
    /// <summary>
    /// Interaction logic for PlayBoard.xaml
    /// </summary>
    public partial class PlayBoard : UserControl
    {
        private PlayBoardState pbState;
        private int noOfMoves;
        private const int fieldSize = 20;
        public PlayBoard()
        {
            InitializeComponent();
            pbState = new PlayBoardState();
            noOfMoves = 0;
            InsertFields(canBoard);
        }
        private void InsertFields(Canvas board)
        {
            for (int j = 0; j < pbState.PlayBoardFields.GetLength(1); j++)
            {
                for (int i = 0; i < pbState.PlayBoardFields.GetLength(0); i++)
                {
                    Button field = new Button
                    {
                        Height = fieldSize,
                        Width = fieldSize,
                    };
                    field.Click += Field_Click;
                    field.Tag = i + "," + j;
                    board.Children.Add(field);
                    Canvas.SetLeft(field, i * fieldSize);
                    Canvas.SetTop(field, j * fieldSize);
                }
            }
        }
        private void Field_Click(object sender, RoutedEventArgs e)
        {
            Button field = (Button)sender;
            string[] coordinates = ((string)field.Tag).Split(',');
            int X = int.Parse(coordinates[0]);
            int Y = int.Parse(coordinates[1]);
            if (!pbState.IsFree(X,Y)) return;
            noOfMoves++;
            int player;
            if(noOfMoves % 2 == 0)
            {
                field.Content = "O";
                player = 2;
            }
            else
            {
                field.Content = "X";
                player = 1;
            }
            bool win = pbState.EvaluateMove(X,Y, player);
            if (win)
            {
                MessageBox.Show($"Vyhrál Hráč{player}");
                foreach (Button button in canBoard.Children)
                {
                    button.Content = "";
                }
                pbState.PlayboardReset();
                noOfMoves = 0;
            }
        }

        private void btnStav_Click(object sender, RoutedEventArgs e)
        {
            StringBuilder scoreBoardBuild = new StringBuilder(); 
            for (int j = 0; j < pbState.PlayBoardFields.GetLength(1); j++)
            {
                for (int i = 0; i < pbState.PlayBoardFields.GetLength(0); i++)
                {
                    scoreBoardBuild.AppendFormat("{0,3}", pbState.PlayBoardFields[i,j]);
                }
                scoreBoardBuild.Append(Environment.NewLine);
            }
            string scoreBoard = scoreBoardBuild.ToString();
            MessageBox.Show(scoreBoard);
        }
    }
}
