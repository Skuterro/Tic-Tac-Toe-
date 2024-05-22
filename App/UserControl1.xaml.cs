using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
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
    /// Logika interakcji dla klasy UserControl1.xaml
    /// </summary>
    public partial class UserControl1 : UserControl
    {
        private MainWindow mainWindow;

        public UserControl1(MainWindow mainWindow)
        {
            InitializeComponent();
            this.mainWindow = mainWindow;
        }

        private void Leaderboard_button_Click(object sender, RoutedEventArgs e)
        {
            mainWindow.Leaderboard();
        }

        private void CreateRoom_Button_Click(object sender, RoutedEventArgs e)
        {
            mainWindow.CreateRoom();
        }

        private void ShowRooms_Button_Click(object sender, RoutedEventArgs e)
        {
            mainWindow.ShowRooms();

            UserControl2 userControl2 = new UserControl2(mainWindow);
            this.Content = userControl2;
        }
    }
}
