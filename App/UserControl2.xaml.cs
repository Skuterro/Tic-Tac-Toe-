using Microsoft.EntityFrameworkCore.Metadata.Conventions;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
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
    /// Logika interakcji dla klasy Lobby.xaml
    /// </summary>
    public partial class UserControl2 : UserControl
    {
        MainWindow mainWindow;

        public struct Gameroom
        {
            public int Id { get; set; }
            public string Player { get; set; }
        }

        public List<Gameroom> Gamerooms { get; set; }

        public UserControl2(MainWindow mainwindow)
        {
            InitializeComponent();

            this.mainWindow = mainwindow;

            Gamerooms = new List<Gameroom>();

            string[] pairs = mainWindow.gameRoomsString.Split('|');

            foreach (string pair in pairs)
            {
                string[] data = pair.Split(':');

                int id = int.Parse(data[0]);

                Gameroom gm = new Gameroom { Id = id, Player = data[1] };

                Gamerooms.Add(gm);
            }
            
            graczeListBox.ItemsSource = Gamerooms;
        }

        private void WybierzButtonClick(object sender, RoutedEventArgs e)
        {
            Button button = (Button)sender;
            int clickedId = (int)button.Tag;

            mainWindow.JoinRoom(clickedId);
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            UserControl userControl = new UserControl1(mainWindow);
            this.Content = userControl;
        }
    }
}
