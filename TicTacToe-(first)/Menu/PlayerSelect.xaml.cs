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
using System.Windows.Shapes;

namespace Menu
{
    public partial class PlayerSelect : Window
    {
        private int countOpenedPlayers = 0;
        public PlayerSelect()
        {
            InitializeComponent();
        }
        private void Player1_MouseDown(object sender, MouseButtonEventArgs e)
        {
            Player1.IsEnabled = false;
            MainWindow mainWindow = new MainWindow(true, false);
            mainWindow.Show();
            countOpenedPlayers++;
            if (countOpenedPlayers == 2)
            {
                this.Close();
                countOpenedPlayers = 0;
            }
        }

        private void Player2_MouseDown(object sender, MouseButtonEventArgs e)
        {
            Player2.IsEnabled = false;
            MainWindow mainWindow = new MainWindow(false, false);
            mainWindow.Show();
            countOpenedPlayers++;
            if (countOpenedPlayers == 2)
            {
                this.Close();
                countOpenedPlayers = 0;
            }
        }
    }
}
