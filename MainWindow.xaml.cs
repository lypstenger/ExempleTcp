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
using System.Net.Sockets;
using System.Net;
using System.Threading;
using System.IO;

namespace ExTcp
{
    /// <summary>
    /// Logique d'interaction pour MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        List<Client> Lc = new List<Client>();
        public MainWindow()
        {
            InitializeComponent();
        }
        TcpListener tt = new TcpListener(IPAddress.Parse("127.0.0.1"), 9990);
        //      TcpListener tt = new TcpListener(IPAddress.Parse("192.168.20.63"), 9990);

        private void button_Click(object sender, RoutedEventArgs e)
        {
            if (!ecoute)
            {
                ecoute = !ecoute;
                Thread Thread_copy = new Thread(() => ecouteTsp(""));
                Thread_copy.Start();
                button.Content = "Stopper le listener";
            }
            else
            {
                dial = false;
                ecoute = !ecoute;
                cpt = 0;
                button.Content = "Lancer le listener";
                foreach (TcpClient tc in listtcp)
                {
                    if (tc.Connected)
                    {
                        tc.Close();
                    }
                }
                listtcp.Clear();
                

            }

        }
        private bool ecoute = false;
        private bool dial = false;

        TcpClient client = null;

        private void dialtcp(TcpClient str)
        {


            string mess = "";
            NetworkStream stream = str.GetStream();
            while (dial)
            {
                if (str.Available > 0)
                {
                    byte[] mytab = new byte[str.Available];
                    stream.Read(mytab, 0, str.Available);

                    mess = Encoding.ASCII.GetString(mytab);

                    this.Dispatcher.BeginInvoke(new Action(delegate
                    {
                        affich_reception(mess);
                    }));
                }
                Thread.Sleep(10);


            }
            stream.Close();
            str.Close();
        }

        int cptrecp = 0;
        private void affich_reception(string mess)
        {
            cptrecp++;
              
          //  button1_Copy1_Click(null, null);

            label1.Items.Insert(0, mess );
        }
        List<TcpClient> listtcp = new List<TcpClient>();
        private void ecouteTsp(string str)
        {
            //   client = null;
            tt.Start();
            while (ecoute)
            {
                //   client = null;
                if (tt.Pending())
                {
                    client = tt.AcceptTcpClient();
                    listtcp.Add(client);
                    if (client != null)
                    {
                        dial = true;
                        Thread Thread_copy = new Thread(() => dialtcp(client));
                        Thread_copy.Start();
                        cpt++;
                    }
                }




                Thread.Sleep(10);
                this.Dispatcher.BeginInvoke(new Action(delegate
                {
                    affich_reception();
                }));

            }

        }
        int cpt = 0;
        private void affich_reception()
        {
            label.Content = cpt.ToString();
        }

        private void button1_Click(object sender, RoutedEventArgs e)
        {
 
                byte[] tmp = Encoding.ASCII.GetBytes(textBox.Text + "\n");

                foreach(TcpClient tc in listtcp)
                {
                    if (tc.Connected)
                    {
                        tc.GetStream().Write(tmp, 0, tmp.Length);
                    }
                }

            
           
        }

          private void fin_upload()
        {
            label.Content = cpt.ToString();
        }
        List<Client> lesfebnetres = new List<Client>();
        private void button3_Click(object sender, RoutedEventArgs e)
        {
            Client tcpclient = new Client();
            tcpclient.Show();
            lesfebnetres.Add(tcpclient);

        }

         private void button4_Click(object sender, RoutedEventArgs e)
        {
            label1.Items.Clear();
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            dial = false;
            ecoute = false;
            foreach (TcpClient tc in listtcp)
            {
                if (tc.Connected)
                {
                    tc.Close();
                }
            }
            foreach (Client tc in lesfebnetres)
            {
                tc.Close();
                
            }

        }
    }
}
