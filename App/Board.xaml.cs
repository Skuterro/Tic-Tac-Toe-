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

namespace Ships_Game
{
    /// <summary>
    /// Logika interakcji dla klasy Board.xaml
    /// </summary>
    public partial class Board : UserControl
    {
        MainWindow mainWindow;

        public Board(MainWindow mainWindow)
        {
            InitializeComponent();
            this.mainWindow = mainWindow;

            if(mainWindow.gameStatus.turn != mainWindow.player)
            {
                var buttons = board.Children.OfType<Button>().ToList();

                buttons.ForEach(button => button.IsEnabled = false);
            }

            int id = 1;

            for (int row = 0; row < 3; row++)
            {
                for (int col = 0; col < 3; col++)
                {
                    int x = mainWindow.gameStatus.board[row,col];
                    Button button = board.Children.OfType<Button>().FirstOrDefault(b => b.Tag != null && b.Tag.ToString() == id.ToString());

                    if(x == 1)
                    {
                        button.Content = "O";
                        button.IsEnabled = false;
                        button.Foreground = Brushes.Red;
                    }
                    else if(x == 2) 
                    {
                        button.Content = "X";
                        button.IsEnabled = false;
                        button.Foreground = Brushes.Blue;
                    }
                    id++;
                }
            }
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            Button button = (Button)sender;
            int clickedId = int.Parse(button.Tag.ToString());

            mainWindow.MakeMove(clickedId);
        }
    }
}
