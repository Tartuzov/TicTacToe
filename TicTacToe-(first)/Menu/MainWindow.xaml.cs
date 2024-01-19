using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace Menu
{
    public partial class MainWindow : Window
    {
        private bool Symbol;
        private UdpClient udpClient;
        private const int serverPort = 16450;
        bool BOT;
        bool BOTforservcheck;
        private MainWindow()
        {
            InitializeComponent();
        }

        public MainWindow(bool CurrentSymbol, bool BOT) : this()
        {
            this.BOT = BOT;
            this.BOTforservcheck = BOT;
            Symbol = CurrentSymbol;
            udpClient = new UdpClient();
            if (Symbol == true)
            {
                udpClient.Client.Bind(new IPEndPoint(IPAddress.Parse("127.0.0.1"), 5000));
            }
            else
            {
                this.IsEnabled = false;
                udpClient.Client.Bind(new IPEndPoint(IPAddress.Parse("127.0.0.1"), 5200));
            }
            Task.Run(() => ReceiveDataFromServer());
        }

        private void Label_MouseDown(object sender, MouseButtonEventArgs e)
        {
            Label label = (Label)sender;

            if (!label.IsEnabled)
                return;

            int cellNumber = int.Parse(label.Tag.ToString());
            SendDataToServer(cellNumber);

            label.Content = Symbol ? "X" : "O";
            label.IsEnabled = false;

            this.IsEnabled = false;
        }

        private void SendDataToServer(int cellNumber)
        {
            try
            {
                if (BOTforservcheck == true) 
                { 
                    byte[] data = Encoding.UTF8.GetBytes($"{Symbol},{cellNumber},0");
                    udpClient.Send(data, data.Length, "127.0.0.1", serverPort);
                    BOTforservcheck = false;
                }
                else
                {
                    byte[] data = Encoding.UTF8.GetBytes($"{Symbol},{cellNumber}");
                    udpClient.Send(data, data.Length, "127.0.0.1", serverPort);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error sending data to server: {ex.Message}");
            }
        }

        private async void ReceiveDataFromServer()
        {
            while (true)
            {
                IPEndPoint serverEndpoint = new IPEndPoint(IPAddress.Parse("127.0.0.1"), serverPort);
                byte[] data = udpClient.Receive(ref serverEndpoint);
                string message = Encoding.UTF8.GetString(data);

                if (message.StartsWith("Win:"))
                {
                    bool isNobody = message.EndsWith("N");
                    bool isWinner = message.EndsWith("X");
                    Dispatcher.Invoke(() =>
                    {
                        this.IsEnabled = true;

                        if (isWinner)
                        {
                            ShowVictoryMessage(true,isNobody);
                        }
                        else
                        {
                            ShowVictoryMessage(false, isNobody);
                        }
                        CloseServer();
                    });
                    
                }
                else
                {
                    string[] parts = message.Split(',');
                    bool symbol = bool.Parse(parts[0]);
                    int cellNumber = int.Parse(parts[1]);
                    HandleReceivedData(symbol, cellNumber);
                }
            }
        }

        private void CloseServer()
        {
            Process[] processes = Process.GetProcessesByName("Console");
            foreach (Process process in processes)
            {
                process.CloseMainWindow();
                process.WaitForExit();
            }
        }

        private void HandleReceivedData(bool symbol, int cellNumber)
        {
            Dispatcher.Invoke(() =>
            {
                this.IsEnabled = true;

                Label targetLabel = FindLabelByTag(cellNumber);
                targetLabel.IsEnabled = false;
                targetLabel.Content = symbol ? "X" : "O";
            });
        }

        public void ShowVictoryMessage(bool isWinner,bool isN)
        {
            Dispatcher.Invoke(() =>
            {
                if (isN==false)
                {
                    string m = isWinner ? "You Win!" : "You Lose!";
                    if (m == "You Win!")
                    {
                        LabelWinner.Foreground = Brushes.DarkGreen;
                        LabelWinner.Content = m;
                    }
                    else
                    {
                        LabelWinner.Foreground = Brushes.Red;
                        LabelWinner.Content = m;
                    }
                    this.IsEnabled = false;
                }
                else {
                    LabelWinner.Foreground = Brushes.Gray;
                    LabelWinner.Content = "Nobody win";
                    this.IsEnabled = false;
                }
            });
        }

        private Label FindLabelByTag(int tagValue)
        {
            foreach (UIElement element in GridGame.Children)
            {
                if (element is Label label && Convert.ToInt32(label.Tag) == tagValue)
                {
                    return label;
                }
            }
            return null;
        }
    }
}