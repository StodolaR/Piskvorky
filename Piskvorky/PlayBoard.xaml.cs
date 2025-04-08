using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
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
        private Button[,] fieldButtons;
        private PlayBoardState pbState;
        private const int fieldSize = 20;
        private Button? lastButton;
        private int scoreHuman = 0;
        private int scoreComputer = 0;
        private bool isFree = true;
        public PlayBoard()
        {
            InitializeComponent();
            pbState = new PlayBoardState();
            fieldButtons = new Button[pbState.PlayBoardFields.GetLength(0), pbState.PlayBoardFields.GetLength(1)];
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
                    field.Background = Brushes.LightGray;
                    field.PreviewMouseDown += Field_PreviewMouseDown;
                    field.PreviewMouseUp += Field_PreviewMouseUp;
                    field.Tag = i + "," + j;
                    board.Children.Add(field);
                    Canvas.SetLeft(field, i * fieldSize);
                    Canvas.SetTop(field, j * fieldSize);
                    fieldButtons[i,j] = field;
                }
            }
        }
        private void Field_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            Button humanField = (Button)sender;
            string[] coordinates = ((string)humanField.Tag).Split(',');
            int X = int.Parse(coordinates[0]);
            int Y = int.Parse(coordinates[1]);
            if (!pbState.IsFree(X, Y))
            {
                isFree = false;
                return;
            }            
            bool win = ProcessingMove(humanField, PlayBoardState.human, X, Y);
            if (win)
            {
                AnnounceWin(humanField, PlayBoardState.human);
            }
        }
        private void Field_PreviewMouseUp(object sender, MouseButtonEventArgs e)
        {
            if (isFree == false)
            {
                isFree = true;
                return;
            }
            Thread.Sleep(200);
            (int,int) bestScoreField = pbState.GetBestScoreField();
            int X = bestScoreField.Item1;
            int Y = bestScoreField.Item2;
            Button compField = fieldButtons[X,Y];
            bool win = ProcessingMove(compField, PlayBoardState.computer, X, Y);
            if (win)
            {
                AnnounceWin(compField, PlayBoardState.computer);
            }
        }
        private bool ProcessingMove(Button field, int player, int X, int Y)
        {
            
            field.FontWeight = FontWeights.Bold;
            if (lastButton != null)
            {
                lastButton.FontWeight = FontWeights.Normal;
            }
            if (player == 1)
            {
                field.Foreground = Brushes.DarkRed;
                field.Content = "X";
            }
            else
            {
                field.Foreground = Brushes.DarkBlue;
                field.Content = "O";
            }
            lastButton = field;
            bool isWin = pbState.EvaluateMove(X, Y, player);
            return isWin;
        }
        private void AnnounceWin(Button field, int player)
        {
            field.Background = Brushes.Gold;
            foreach (var winnerField in pbState.WinnerFields)
            {
                int X = winnerField.Item1;
                int Y = winnerField.Item2;
                fieldButtons[X,Y].Background = Brushes.Gold;
            }
            if (player == PlayBoardState.human)
            {
                scoreHuman++;
                tbScoreHuman.Text = scoreHuman.ToString(); 
                MessageBox.Show("Gratuluji, vyhrál jsi!");
            }
            else
            {
                scoreComputer++;
                tbScoreComputer.Text = scoreComputer.ToString();
                MessageBox.Show("Tentokrát byl lepší počítač...");
            }
            foreach (Button button in canBoard.Children)
            {
                button.Content = "";
                button.Background = Brushes.LightGray;
            }
            pbState.PlayboardReset();
        }
    }
}
