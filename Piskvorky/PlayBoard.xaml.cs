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
        private PlayBoardState pbState;
        private const int fieldSize = 20;
        private Button? lastButton;
        private bool humanWin = false;
        public PlayBoard()
        {
            InitializeComponent();
            pbState = new PlayBoardState();
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
                    field.PreviewMouseDown += Field_PreviewMouseDown;
                    field.PreviewMouseUp += Field_PreviewMouseUp;
                    field.Tag = i + "," + j;
                    board.Children.Add(field);
                    Canvas.SetLeft(field, i * fieldSize);
                    Canvas.SetTop(field, j * fieldSize);
                }
            }
        }

        private void Field_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            Button humanField = (Button)sender;
            string[] coordinates = ((string)humanField.Tag).Split(',');
            int X = int.Parse(coordinates[0]);
            int Y = int.Parse(coordinates[1]);
            if (!pbState.IsFree(X, Y)) return;
            bool win = ProcessingMove(humanField, "X", PlayBoardState.human, X, Y);
            if (win)
            {
                AnnounceWin(humanField, PlayBoardState.human);
                humanWin = true;
            }
        }

        private void Field_PreviewMouseUp(object sender, MouseButtonEventArgs e)
        {
            if (!humanWin)
            {
                Thread.Sleep(500);
                string bestScoreField = pbState.GetBestScoreField();
                foreach (Button compField in canBoard.Children)
                {
                    if (compField.Tag.Equals(bestScoreField))
                    {
                        string[] coordinates = ((string)compField.Tag).Split(',');
                        int X = int.Parse(coordinates[0]);
                        int Y = int.Parse(coordinates[1]);
                        bool win = ProcessingMove(compField, "O", PlayBoardState.computer, X, Y);
                        if (win)
                        {
                            AnnounceWin(compField, PlayBoardState.computer);
                        }
                        break;
                    }
                }
            }
        }

        
        private bool ProcessingMove(Button field, string mark, int player, int X, int Y)
        {
            
            field.FontWeight = FontWeights.Bold;
            if (lastButton != null)
            {
                lastButton.FontWeight = FontWeights.Normal;
            }
            field.Content = mark;
            lastButton = field;
            bool isWin = pbState.EvaluateMove(X, Y, player);
            return isWin;
        }
        private void AnnounceWin(Button field, int player)
        {
            field.Foreground = Brushes.Red;
            foreach (Button button in canBoard.Children)
            {
                if (pbState.WinnerFields.Contains(button.Tag))
                    button.Foreground = Brushes.Red;
            }
            MessageBox.Show($"Vyhrál Hráč{player}");
            foreach (Button button in canBoard.Children)
            {
                button.Content = "";
                button.Foreground = Brushes.Black;
            }
            pbState.PlayboardReset();
        }
        private void btnStav_Click(object sender, RoutedEventArgs e)
        {
            StringBuilder scoreBoardBuild = new StringBuilder(); 
            for (int j = 0; j < pbState.PlayBoardFields.GetLength(1); j++)
            {
                for (int i = 0; i < pbState.PlayBoardFields.GetLength(0); i++)
                {
                    scoreBoardBuild.AppendFormat("{0,5}", pbState.PlayBoardFields[i,j]);
                }
                scoreBoardBuild.Append(Environment.NewLine);
            }
            string scoreBoard = scoreBoardBuild.ToString();
            MessageBox.Show(scoreBoard);
        }
    }
}
