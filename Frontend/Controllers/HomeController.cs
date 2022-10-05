using Client.Common;
using Frontend.Models;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using RestSharp;
using System.ServiceModel;

namespace Frontend.Controllers
{
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            List<ClientInfo> clientInfos = new List<ClientInfo>();
            clientInfos.Add(new ClientInfo(1, "localhost", 8080, 10));
            clientInfos.Add(new ClientInfo(2, "localhost", 8082, 2));
            //clientInfos = loadClientInfo();

            return View(clientInfos);
        }


        private List<ClientInfo> loadClientInfo()
        {
            List<ClientInfo> clientInfos = new List<ClientInfo>();

            // all clients
            RestClient restClient = new RestClient("http://localhost:60238/");
            RestRequest restRequest = new RestRequest("api/clients", Method.Get);
            RestResponse restResponse = restClient.Execute(restRequest);

            List<Backend.Models.Client> clients = JsonConvert.DeserializeObject<List<Backend.Models.Client>>(restResponse.Content);
            foreach (Backend.Models.Client client in clients)
            {
                //Set the URL and create the connection!
                string URL = "net.tcp://" + client.Host + ":" + client.Port + "/JobServer";
                try
                {
                    ChannelFactory<JobServerInterface> foobFactory;
                    NetTcpBinding netTcpBinding = new NetTcpBinding();
                    //Set the URL and create the connection!
                    Console.WriteLine($"dowmloadJob from {URL}");
                    foobFactory = new ChannelFactory<JobServerInterface>(netTcpBinding, URL);
                    JobServerInterface foob = foobFactory.CreateChannel();
                    int numCompletedJobs = 0;
                    foob.FetchJobInfo(out numCompletedJobs);
                    foobFactory.Close();

                    // add into clientInfos
                    clientInfos.Add(new ClientInfo(client.Id, client.Host, client.Port, numCompletedJobs));
                }
                catch (Exception e)
                {
                    Console.WriteLine($"Exception connect to {URL} failed : {e.Message}");
                }
            }
            return clientInfos;
        }

        [HttpGet]
        public IActionResult ClientInfo()
        {
            //return Ok(loadClientInfo());
            List<ClientInfo> clientInfos = new List<ClientInfo>();
            clientInfos.Add(new ClientInfo(1, "localhost", 8080, 10));
            clientInfos.Add(new ClientInfo(2, "localhost", 8082, 2));
            clientInfos.Add(new ClientInfo(3, "localhost", 8083, 5));
            return Ok(clientInfos);
        }


    }
}
