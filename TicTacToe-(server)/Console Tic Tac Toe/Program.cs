using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Net.Sockets;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Threading;

namespace Server
{

    internal class Program
    {
        static bool flag = true;
        static bool bot = false;
        static int[] mas = new int[9] { 0, 0, 0, 0, 0, 0, 0, 0, 0 };
        static int bhods = 0;
        static int bplaceofhod = -1;
        static void Main(string[] args)
        {
            UdpClient udpServer = new UdpClient(16450);
            IPEndPoint client1 = new IPEndPoint(IPAddress.Parse("127.0.0.1"), 5000);
            IPEndPoint client2 = new IPEndPoint(IPAddress.Parse("127.0.0.1"), 5200);
            while (true)
            {
                if (flag == true)
                {
                    byte[] data = udpServer.Receive(ref client1);
                    string message = Encoding.UTF8.GetString(data);

                    string[] parts = message.Split(',');
                    if (parts.Count() == 3)
                    {
                        bot = true;
                    }
                    bool symbol = bool.Parse(parts[0]);
                    int cellNumber = int.Parse(parts[1]);

                    mas[cellNumber - 1] = symbol ? 1 : 2;
                    ArrayPrint(mas);
                    check(mas, udpServer, client1, client2);

                    if (bot == false)
                    {
                        byte[] resultData = Encoding.UTF8.GetBytes($"{symbol},{cellNumber}");
                        udpServer.Send(resultData, resultData.Length, client2);
                        flag = false;
                    }
                    else
                    {
                        Thread.Sleep(300);
                        BotStep();
                        byte[] resultData = Encoding.UTF8.GetBytes($"{false},{bplaceofhod + 1}");
                        udpServer.Send(resultData, resultData.Length, client1);
                        ArrayPrint(mas);
                        break;
                    }
                }
                else if (flag == false)
                {
                    byte[] data = udpServer.Receive(ref client2);
                    string message = Encoding.UTF8.GetString(data);

                    string[] parts = message.Split(',');
                    bool symbol = bool.Parse(parts[0]);
                    int cellNumber = int.Parse(parts[1]);

                    mas[cellNumber - 1] = symbol ? 1 : 2;
                    ArrayPrint(mas);
                    check(mas, udpServer, client1, client2);

                    byte[] resultData = Encoding.UTF8.GetBytes($"{symbol},{cellNumber}");
                    udpServer.Send(resultData, resultData.Length, client1);
                    flag = true;
                }
            }
            if (bot == true)
                while (true)
                {
                    byte[] data = udpServer.Receive(ref client1);
                    string message = Encoding.UTF8.GetString(data);

                    string[] parts = message.Split(',');
                    if (parts.Count() == 3)
                    {
                        bot = true;
                    }
                    bool symbol = bool.Parse(parts[0]);
                    int cellNumber = int.Parse(parts[1]);

                    mas[cellNumber - 1] = symbol ? 1 : 2;
                    ArrayPrint(mas);
                    check(mas, udpServer, client1, client2);
                    Thread.Sleep(800);
                    BotStep();
                    byte[] resultData = Encoding.UTF8.GetBytes($"{false},{bplaceofhod+1}");
                    udpServer.Send(resultData, resultData.Length, client1);
                    ArrayPrint(mas);
                    check(mas, udpServer, client1, client2);
                }
        }

        static void ArrayPrint(int[] mas)
        {
            int k = 0;
            for (int i = 0; i < 3; i++)
            {
                for (int j = 0; j < 3; j++)
                {
                    Console.Write(mas[k] + " ");
                    k++;
                }
                Console.WriteLine();
            }
            Console.WriteLine();
        }

        static void check(int[] mas, UdpClient udpServer, IPEndPoint client1, IPEndPoint client2)
        {
            int x = 3;
            bool xwin = false;
            bool owin = false;
            int countofzero = 9;
            for (int i = 0; i < mas.Length; i++)
            {
                if (i % 3 == 0 && mas[i] != 0 && mas[i] == mas[i + 1] && mas[i + 2] == mas[i + 1])
                {
                    if (mas[i] == 1)
                        xwin = true;
                    else if (mas[i] == 2)
                        owin = true;
                    break;
                }

                else if (i < 3 && mas[i] != 0 && mas[i] == mas[i + x] && mas[i + x] == mas[i + x * 2])
                {
                    if (mas[i] == 1)
                        xwin = true;
                    else if (mas[i] == 2)
                        owin = true;
                    break;
                }
                else if (i == 4 && mas[i] != 0 && ((mas[i] == mas[0] && mas[i] == mas[mas.Length - 1]) || (mas[i] == mas[x - 1] && mas[i] == mas[mas.Length - x])))
                {
                    if (mas[i] == 1)
                        xwin = true;
                    else if (mas[i] == 2)
                        owin = true;
                    break;
                }
                if (mas[i] != 0)
                    countofzero--;
            }
            if (bot == false)
            {
                if (countofzero == 0)
                {
                    Console.WriteLine("NOBODY WIN!");
                    byte[] xWinBytes = Encoding.UTF8.GetBytes("Win:N");
                    udpServer.Send(xWinBytes, xWinBytes.Length, client1);
                    udpServer.Send(xWinBytes, xWinBytes.Length, client2);
                }
                else if (xwin)
                {
                    Console.WriteLine("WIN X!");
                    byte[] xWinBytes = Encoding.UTF8.GetBytes("Win:X");
                    udpServer.Send(xWinBytes, xWinBytes.Length, client1);

                    byte[] oWinBytes = Encoding.UTF8.GetBytes("Win:O");
                    udpServer.Send(oWinBytes, oWinBytes.Length, client2);
                }
                else if (owin)
                {
                    Console.WriteLine("WIN O!");
                    byte[] oWinBytes = Encoding.UTF8.GetBytes("Win:O");
                    udpServer.Send(oWinBytes, oWinBytes.Length, client1);

                    byte[] xWinBytes = Encoding.UTF8.GetBytes("Win:X");
                    udpServer.Send(xWinBytes, xWinBytes.Length, client2);
                }
            }
            else
            {
                if(countofzero == 0)
                {
                    Console.WriteLine("NOBODY WIN!");
                    byte[] xWinBytes = Encoding.UTF8.GetBytes("Win:N");
                    udpServer.Send(xWinBytes, xWinBytes.Length, client1);
                }
                else if (xwin)
                {
                    Console.WriteLine("WIN X!");
                    byte[] xWinBytes = Encoding.UTF8.GetBytes("Win:X");
                    udpServer.Send(xWinBytes, xWinBytes.Length, client1);
                }
                else if (owin)
                {
                    Console.WriteLine("WIN O!");
                    byte[] oWinBytes = Encoding.UTF8.GetBytes("Win:O");
                    udpServer.Send(oWinBytes, oWinBytes.Length, client1);
                }
            }
            //else if(countofzero == 0)
            //{
            //    Console.WriteLine("NOBODY WINS!");
            //    byte[] xWinBytes = Encoding.UTF8.GetBytes("Win:NOBODY");
            //    udpServer.Send(xWinBytes, xWinBytes.Length, client1);
            //}
        }
        static private void BotStep()
        {
            if (bhods == 0)
            {
                Random random = new Random();
                for (int i = 0; i < 10;)
                {
                    int a = random.Next(9);
                    if (mas[a] == 0)
                    {
                        mas[a] = 2;
                        bplaceofhod = a;
                        break;
                    }
                }
                bhods++;
            }
            else
            {
               int a = CloseWin();
               mas[a] = 2;
               bplaceofhod = a;
            }
        }
        static private int CloseWin()
        {
            int x = 3;
            bool owin = false;
            int placeofhod = -1;
            Random random1 = new Random();
            int erorbot = random1.Next(101);
            if(erorbot!=50)
            for (int i = 0; i < mas.Length; i++)
            {
                if (i % 3 == 0 && (mas[i] == mas[i + 1] || mas[i + 2] == mas[i + 1] || mas[i] == mas[i + 2]) &&
                    (mas[i] == 0 || mas[i + 1] == 0 || mas[i + 2] == 0) && (mas[i] != 0 && mas[i + 1] != 0 || mas[i + 2] != 0 && mas[i + 1] != 0 || mas[i + 2] != 0 && mas[i] != 0))
                {
                    if (mas[i] == 2 || mas[i + 1] == 2)
                    {
                        if (mas[i] == 0)
                            placeofhod = i;
                        else if (mas[i + 1] == 0)
                            placeofhod = i + 1;
                        else if (mas[i + 2] == 0)
                            placeofhod = i + 2;
                        owin = true;
                        break;
                    }
                    else
                    {
                        if (mas[i] == 0)
                            placeofhod = i;
                        else if (mas[i + 1] == 0)
                            placeofhod = i + 1;
                        else if (mas[i + 2] == 0)
                            placeofhod = i + 2;
                    }
                }
                else if (i < 3 && (mas[i] == mas[i + x] || mas[i + x] == mas[i + x * 2]) &&
                    (mas[i] == 0 || mas[i + x] == 0 || mas[i + x * 2] == 0) && (mas[i] != 0 && mas[i + x] != 0 || mas[i + x * 2] != 0 && mas[i + x] != 0 || mas[i + x * 2] != 0 && mas[i] != 0))
                {
                    if (mas[i] == 2 || mas[i + x] == 2)
                    {
                        if (mas[i] == 0)
                            placeofhod = i;
                        else if (mas[i + x] == 0)
                            placeofhod = i + x;
                        else if (mas[i + x * 2] == 0)
                            placeofhod = i + x * 2;
                        owin = true;
                        break;
                    }
                    else
                    {
                        if (mas[i] == 0)
                            placeofhod = i;
                        else if (mas[i + x] == 0)
                            placeofhod = i + x;
                        else if (mas[i + x * 2] == 0)
                            placeofhod = i + x * 2;
                    }
                }
                else if (i == 4 && (((mas[i] == mas[0] || mas[i] == mas[mas.Length - 1] || mas[0] == mas[mas.Length - 1]) && (mas[i] == 0 || mas[0] == 0 || mas[mas.Length - 1] == 0) &&
                    ((mas[i] != 0 && mas[0] != 0) || (mas[i] != 0 && mas[mas.Length - 1] != 0) || (mas[mas.Length - 1] != 0 && mas[0] != 0))) ||

                    ((mas[i] == mas[x - 1] || mas[i] == mas[mas.Length - x] || mas[mas.Length - x] == mas[x - 1]) && (mas[i] == 0 || mas[x - 1] == 0 || mas[mas.Length - x] == 0) &&
                    ((mas[i] != 0 && mas[x - 1] != 0) || (mas[i] != 0 && mas[mas.Length - x] != 0) || (mas[mas.Length - x] != 0 && mas[x - 1] != 0)))))
                {
                    bool a = false;
                    if ((mas[i] == mas[0] || mas[i] == mas[mas.Length - 1] || mas[0] == mas[mas.Length-1]) && (mas[i] == 0 || mas[0] == 0 || mas[mas.Length - 1] == 0))
                        a = true;
                    if (a)
                    {
                        if (mas[i] == 2 || mas[0] == 2)
                        {
                            if (mas[i] == 0)
                                placeofhod = i;
                            else if (mas[mas.Length - 1] == 0)
                                placeofhod = mas.Length - 1;
                            else if (mas[0] == 0)
                                placeofhod = 0;
                            owin = true;
                            break;
                        }
                        else
                        {
                            if (mas[i] == 0)
                                placeofhod = i;
                            else if (mas[mas.Length - 1] == 0)
                                placeofhod = mas.Length - 1;
                            else if (mas[0] == 0)
                                placeofhod = 0;
                        }
                    }
                    else
                    {
                        if (mas[i] == 2 || mas[x - 1] == 2)
                        {
                            if (mas[i] == 0)
                                placeofhod = i;
                            else if (mas[mas.Length - x] == 0)
                                placeofhod = mas.Length - x;
                            else if (mas[x - 1] == 0)
                                placeofhod = x - 1;
                            owin = true;
                            break;
                        }
                        else
                        {
                            if (mas[i] == 0)
                                placeofhod = i;
                            else if (mas[mas.Length - x] == 0)
                                placeofhod = mas.Length - x;
                            else if (mas[x - 1] == 0)
                                placeofhod = x - 1;
                        }
                    }
                }
            }
            if (placeofhod == -1)
            {
                Random random = new Random();
                for (int i = 0; i < 10;)
                {
                    int a = random.Next(9);
                    if (mas[a] == 0)
                    {
                        return a;
                    }
                }
            }
            return placeofhod;
        }
    }
}