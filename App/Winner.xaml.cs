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
    /// Logika interakcji dla klasy Winner.xaml
    /// </summary>
    public partial class Winner : UserControl
    {
        MainWindow mainWindow;
        public Winner(MainWindow mainwindow)
        {
            InitializeComponent();
            this.mainWindow = mainwindow;
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            UserControl userControl = new UserControl1(mainWindow);
            this.Content = userControl;
        }
    }
}
