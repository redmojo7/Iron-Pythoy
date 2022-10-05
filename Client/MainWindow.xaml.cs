using IronPython.Hosting;
using Microsoft.Scripting.Hosting;
using Newtonsoft.Json;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
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
using static IronPython.Modules._ast;

namespace Client
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        int myEendpoint = 8200;
        string myHost = "localhost";

        readonly string URL = "http://localhost:60238/";

        private int completedJobsNum = 0;

        ScriptEngine python;
        ScriptScope scope;

        List<string> jobs;

        //private StudentBusinessServerInterface foob;

        public MainWindow()
        {
            InitializeComponent();

            this.python = Python.CreateEngine();
            this.scope = this.python.CreateScope();

            jobs = new List<string>();

            RegisterToSever();
            InitializeRemoting();

            //new Thread(NetworkingAsync).Start();
            //new Thread(Server).Start();
            //var eggsTask = NetworkingAsync();
            Task.Run(() => NetworkingAsync());
            Task.Run(() => ServerAsync());
        }

        private void InitializeRemoting()
        {
            Console.WriteLine("hey so like welcome to my client");
            //This is the actual host service system
            ServiceHost host;
            //This represents a tcp/ip binding in the Windows network stack
            NetTcpBinding tcp = new NetTcpBinding();
            //Bind server to the implementation of DataServer
            //host = new ServiceHost(typeof(StudentBusinessServerImpl));
            /*Present the publicly accessible interface to the client. 0.0.0.0 tells .net to
            accept on any interface. :8100 means this will use port 8100. DataService is a name for the
            actual service, this can be any string.*/

            //host.AddServiceEndpoint(typeof(StudentBusinessServerInterface), tcp, "net.tcp://0.0.0.0:8200/BusinessServer");
            //And open the host for business!
            //host.Open();
            //Console.WriteLine($"Client {host.Description} Online");
        }

        private void RegisterToSever()
        {
            RestClient client = new RestClient(URL);
            RestRequest restRequest = new RestRequest("api/Clients/", Method.Post);
            ClientInfo clientInfo = new ClientInfo(myHost, myEendpoint);
            restRequest.AddBody(clientInfo);
            RestResponse restResponse = client.Execute(restRequest);
            if (restResponse.IsSuccessful)
            {
                Console.WriteLine("Register To Sever successfully");
            }
            else
            {
                Console.WriteLine(restResponse.Content);
                MessageBox.Show("Get fail!", "Error", MessageBoxButton.OK);
            }
        }

        private Task NetworkingAsync()
        {
            while (true)
            {
                UpdateMessage("Networking is runing");
                Console.WriteLine("Networking is runing");
                Thread.Sleep(1000);

                // The first part needs to query the Web Service for a list of other clients
                // Look for new clients'

                List<ClientInfo> otherClients;
                RestClient client = new RestClient(URL);
                RestRequest restRequest = new RestRequest("api/Clients", Method.Get);
                RestResponse restResponse = client.Execute(restRequest);
                if (restResponse.IsSuccessful)
                {
                    List<ClientInfo> ClientInfos = JsonConvert.DeserializeObject<List<ClientInfo>>(restResponse.Content);
                    otherClients = ClientInfos.Where(x => x.Host!=myHost && x.Port!=myEendpoint).ToList();
                    Console.WriteLine(otherClients);
                }
                else if (restResponse.StatusCode == System.Net.HttpStatusCode.NotFound)
                {
                    MessageBox.Show("Get fail! NotFound!", "Message", MessageBoxButton.OK);
                }
                else
                {
                    Console.WriteLine(restResponse.Content);
                    MessageBox.Show("Get fail!", "Error", MessageBoxButton.OK);
                }
                
                // Check each client for jobs, and do them if it can. 
   
            }
        }

        private void ServerAsync()
        {
            while (true)
            {
                UpdateMessage("Server is runing");
                Console.WriteLine("Server is runing");
                Thread.Sleep(1000);

                if (jobs.Count != 0)
                {
                    // read from jobs
                    string script = jobs[0];
                    dynamic dynamicResult = python.Execute(script);
                    Console.WriteLine($"dynamic result: {dynamicResult}");
  
                    // remove from jobs
                    jobs.Remove(script);
                }
            }
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            jobs.Add(ConvertRichTextBoxContentsToString(paythonRichText));
            Console.WriteLine("Submitting");
        }

        private void UpdateMessage(string msg)
        {
            Dispatcher.Invoke(() => { txtMessage.Text = msg; });
        }

        private string ConvertRichTextBoxContentsToString(RichTextBox rtb)
        {
            TextRange textRange = new TextRange(rtb.Document.ContentStart, rtb.Document.ContentEnd);
            return textRange.Text;

        }
    }
}
