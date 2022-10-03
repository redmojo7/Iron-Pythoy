using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
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

namespace Client
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            new Thread(Networking).Start();
            new Thread(Server).Start();

        }

        private void Networking()
        {
            while (true)
            {
                Thread.Sleep(5000);
                // The first part needs to query the Web Service for a list of other clients
                // Look for new clients


                // Check each client for jobs, and do them if it can. 
                UpdateMessage("Networking msg");
                Console.WriteLine("Networking");
            }
        }

        private void Server()
        {
            while (true)
            {
                Thread.Sleep(5000);
                UpdateMessage("Server msg");
                Console.WriteLine("Server");
            }
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            Console.WriteLine("Main");
        }

        private void UpdateMessage(string msg)
        {
            Dispatcher.Invoke(() => { txtMessage.Text = msg; });
        }

    }
}
