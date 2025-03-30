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
        private const int fieldSize = 20;
        private Button? lastButton;
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
            int player;
            field.FontWeight = FontWeights.Bold;
            if (lastButton != null)
            {
                lastButton.FontWeight = FontWeights.Normal;
            }
            field.Content = "X";
            player = 1;
            lastButton = field;
            bool win = pbState.EvaluateMove(X,Y, player);
            if (win)
            {
                field.Foreground = Brushes.Red;
                foreach (Button button in canBoard.Children)
                {
                    if(pbState.WinnerFields.Contains(button.Tag))
                        button.Foreground = Brushes.Red;
                }
                MessageBox.Show($"Vyhrál Hráč{player}");
                foreach (Button button in canBoard.Children)
                {
                    button.Content = "";
                    button.Foreground = Brushes.Black;
                }
                pbState.PlayboardReset();
                return;
            }
            foreach (Button compField in canBoard.Children)
            {
                if (pbState.GetBestScoreField().Equals(compField.Tag))
                {
                    coordinates = ((string)compField.Tag).Split(',');
                    X = int.Parse(coordinates[0]);
                    Y = int.Parse(coordinates[1]);
                    compField.FontWeight = FontWeights.Bold;
                    if (lastButton != null)
                    {
                        lastButton.FontWeight = FontWeights.Normal;
                    }
                    compField.Content = "O";
                    player = 2;
                    lastButton = compField;
                    win = pbState.EvaluateMove(X, Y, player);
                    if (win)
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
                    break;
                }
            }
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
