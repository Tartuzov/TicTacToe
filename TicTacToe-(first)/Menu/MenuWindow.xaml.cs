using System.IO;
using System;
using System.Collections.Generic;
using System.Diagnostics;
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
using System.Net.Sockets;
using System.Net;

namespace Menu
{
    public partial class MenuWindow : Window
    {
        private Process myProcess;
        public MenuWindow()
        {
            InitializeComponent();
        }

        private void LabelStart_MouseDown(object sender, MouseButtonEventArgs e)
        {
            OpenServer();
            if (rd1.IsChecked == true)
            {
                PlayerSelect playerSelect = new PlayerSelect();
                playerSelect.Show();
            }
            if (rd2.IsChecked == true)
            {
                MainWindow mainWindow = new MainWindow(true, true);
                mainWindow.Show();
            }
            this.Close();
        }

        public void OpenServer()
        {
            string exePath = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Console.exe");
            if (File.Exists(exePath))
            {
                Process.Start(exePath);
            }
            else
            {
                MessageBox.Show("EXE file not found!", "Error");
            }
        }
    }
}
