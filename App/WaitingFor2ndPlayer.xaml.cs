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
using System.Windows.Threading;

namespace Ships_Game
{
    /// <summary>
    /// Logika interakcji dla klasy WaitingFor2ndPlayer.xaml
    /// </summary>
    public partial class WaitingFor2ndPlayer : UserControl
    {
        private DispatcherTimer timer;
        private int counter = 0;
        private string[] dots = { ".", ". .", ". . ." };

        public WaitingFor2ndPlayer()
        {
            InitializeComponent();

            timer = new DispatcherTimer();
            timer.Interval = TimeSpan.FromSeconds(1);
            timer.Tick += Timer_Tick;
            timer.Start();
        }

        private void Timer_Tick(object sender, EventArgs e)
        {
            Dots.Text = dots[counter % dots.Length];
            counter++;
        }
    }
}
