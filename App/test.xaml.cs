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
    /// Logika interakcji dla klasy test.xaml
    /// </summary>
    public partial class test : UserControl
    {
        public test(string s)
        {
            InitializeComponent();

            textBlock.Text = s;
        }


    }
}
