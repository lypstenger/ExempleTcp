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
using System.Net.Sockets;
using System.Net;
using System.Threading;
using System.IO;

namespace ExTcp
{
    /// <summary>
    /// Logique d'interaction pour Client.xaml
    /// </summary>
    public partial class Client : Window
    {
        public Client()
        {
            InitializeComponent();
        }
        TcpClient client = null;
        private void button_Click(object sender, RoutedEventArgs e)
        {
            if (!ecoute)
            {
                // client = new TcpClient("192.168.20.63", 9990);
                client = new TcpClient();
                client.ExclusiveAddressUse = false;
                client.Connect("127.0.0.1", 9990);
                 ecoute = true;
                Thread Thread_copy = new Thread(() => ecouteTsp(client));
                Thread_copy.Start();
                button.Content = "Connected";
            }
            else
            {
                button.Content = "Diconnected";
                ecoute = !ecoute;
            }
         }


        private bool ecoute = false;
        private void ecouteTsp(TcpClient str)
        {
            string mess = "";
            NetworkStream stream = str.GetStream();
            while (ecoute)
            {
                if (str.Connected)
                {
                        if (str.Available>0)
                    {
                        byte[] mytab = new byte[str.Available];
                        stream.Read(mytab, 0, mytab.Length);

                        mess = Encoding.ASCII.GetString(mytab);
                          this.Dispatcher.BeginInvoke(new Action(delegate
                        {
                            affich_reception(mess);
                        }));
                    }
                }
                else
                {
                    ecoute = false;
                }
                Thread.Sleep(1000);

            }
         }
        private void affich_reception(string mess)
        {
            label.Text = mess;
        }
        private void envoi(string mes)
        {
            if (client?.Connected == true)
            {

                byte[] tmp = Encoding.ASCII.GetBytes(mes);
                try { 
                client.GetStream().Write(tmp, 0, tmp.Length);
                }
                catch { }
            }
        }

        private void button1_Click(object sender, RoutedEventArgs e)
        {
            if (client?.Connected == true)
            {
                byte[] tmp = Encoding.ASCII.GetBytes(textBox.Text);
                try
                {
                    client.GetStream().Write(tmp, 0, tmp.Length);
                }
                catch { }
            }
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            ecoute = false;
        }
    }
}
