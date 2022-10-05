
using Client.Common;
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
        int myEendpoint = 8100 + new Random().Next(1000);
        string myHost = "localhost";

        readonly string URL = "http://localhost:60238/";

        private int completedJobsNum = 0;

        List<ClientInfo> aliveClients = new List<ClientInfo>();

        ScriptEngine python;
        ScriptScope scope;

        List<string> jobs;

        private JobServerInterface foob;

        private int myClientId;

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
            // for server side

            Console.WriteLine("hey so like welcome to my client");
            //This is the actual host service system
            ServiceHost host;
            //This represents a tcp/ip binding in the Windows network stack
            NetTcpBinding tcp = new NetTcpBinding();
            //Bind server to the implementation of DataServer
            host = new ServiceHost(typeof(JobServerImpl));
            /*Present the publicly accessible interface to the client. 0.0.0.0 tells .net to
            accept on any interface. :8100 means this will use port 8100. DataService is a name for the
            actual service, this can be any string.*/

            host.AddServiceEndpoint(typeof(JobServerInterface), tcp, "net.tcp://0.0.0.0:"+ myEendpoint + "/JobServer");
            //And open the host for business!
            host.Open();
            Console.WriteLine($"Client {host.Description} Online"); 
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
                ClientInfo ClientInfo = JsonConvert.DeserializeObject<ClientInfo>(restResponse.Content);
                myClientId = ClientInfo.Id;
                Console.WriteLine($"Register To Sever successfully! myClientId = {myClientId}");
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

                RestClient client = new RestClient(URL);
                RestRequest restRequest = new RestRequest("api/Clients", Method.Get);
                RestResponse restResponse = client.Execute(restRequest);
                if (restResponse.IsSuccessful)
                {
                    List<ClientInfo> ClientInfos = JsonConvert.DeserializeObject<List<ClientInfo>>(restResponse.Content);
                    // update aliveClients
                    aliveClients = ClientInfos.Where(x => 
                        (x.Host!=myHost) || (x.Host == myHost && x.Port != myEendpoint)
                    ).ToList();
                    Console.WriteLine(aliveClients);
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

                // refresh GUI
                Dispatcher.Invoke(() => {
                    // clears
                    jobListView.Items.Clear();
                    foreach (Job temp in MyJob.jobs)
                    {
                        int numCompleted = temp.ClientInfos.FindAll(x => x.Answered).Count();
                        int numTotal = temp.ClientInfos.Count();
                        // refresh
                        jobListView.Items.Add(new
                        {
                            Id = temp.Id,
                            Status = numCompleted == numTotal ? "Done" : "Working",
                            Total = numTotal,
                            Finished = numCompleted
                        });
                    }
                });
            }
        }

        private (int jobId, string script) dowmloadJob(ClientInfo clientInfo)
        {
            string script = null;
            int jobId = -1;
            // for client side
            ChannelFactory<JobServerInterface> foobFactory;
            NetTcpBinding netTcpBinding = new NetTcpBinding();
            //Set the URL and create the connection!
            string URL = "net.tcp://"+ clientInfo.Host+ ":" + clientInfo.Port + "/JobServer";
            Console.WriteLine($"dowmloadJob from {URL}");
            foobFactory = new ChannelFactory<JobServerInterface>(netTcpBinding, URL);
            foob = foobFactory.CreateChannel();
            foob.DownloadJob(out script, out jobId);
            foobFactory.Close();
            return (jobId: jobId, script: script);
        }

        private void uploadSolution(ClientInfo clientInfo, int myClientId, int jobId, dynamic dynamicResult)
        {
            // for client side
            ChannelFactory<JobServerInterface> foobFactory;
            NetTcpBinding netTcpBinding = new NetTcpBinding();
            //Set the URL and create the connection!
            string URL = "net.tcp://" + clientInfo.Host + ":" + clientInfo.Port + "/JobServer";
            Console.WriteLine($"dowmloadJob from {URL}");
            foobFactory = new ChannelFactory<JobServerInterface>(netTcpBinding, URL);
            foob = foobFactory.CreateChannel();
            foob.UploadSolution(jobId, myClientId,  dynamicResult);
            foobFactory.Close();
        }

        private void ServerAsync()
        {
            while (true)
            {
                UpdateMessage("Server is runing");
                Console.WriteLine("Server is runing");
                Thread.Sleep(1000);


                // onnect to the .NET Remoting server at the IP address and port in the list
                // query if any jobs exist and download them.
                foreach (ClientInfo clientInfo in aliveClients)
                {
                    var job = dowmloadJob(clientInfo);
                    if (job.script != null)
                    {
                        Console.WriteLine($"Execue script : {job.script}");
                        dynamic dynamicResult = python.Execute(job.script);
                        Console.WriteLine($"dynamic result: {dynamicResult}");
                        // upload solutions 
                        uploadSolution(clientInfo, myClientId, job.jobId, dynamicResult);
                    }
                }

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
            string script = ConvertRichTextBoxContentsToString(paythonRichText);  
            MyJob.jobs.Add(new Job(script, aliveClients));
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
