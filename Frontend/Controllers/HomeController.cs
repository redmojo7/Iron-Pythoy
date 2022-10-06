using Backend.Models;
using Desktop.Common;
using Frontend.Models;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using RestSharp;

namespace Frontend.Controllers
{
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            List<ClientInfo> clientInfos = new List<ClientInfo>();
            /*clientInfos.Add(new ClientInfo(1, "localhost", 8080, 10));
            clientInfos.Add(new ClientInfo(2, "localhost", 8082, 2));
            clientInfos.Add(new ClientInfo(3, "localhost", 8083, 33));
            clientInfos.Add(new ClientInfo(4, "localhost", 8084, 23));*/
            clientInfos = loadClientInfo();

            return View(clientInfos);
        }


        private List<ClientInfo> loadClientInfo()
        {
            List<ClientInfo> clientInfos = new List<ClientInfo>();

            // all clients
            RestClient restClient = new RestClient("http://localhost:60238/");
            RestRequest restRequest = new RestRequest("api/clients/info", Method.Get);
            RestResponse restResponse = restClient.Execute(restRequest);
            if (restResponse.IsSuccessful)
            {
                clientInfos = JsonConvert.DeserializeObject<List<ClientInfo>>(restResponse.Content);
            }
            
            return clientInfos;
        }

        [HttpGet]
        public IActionResult ClientInfo()
        {
            return Ok(loadClientInfo());
            /*List<ClientInfo> clientInfos = new List<ClientInfo>();
            clientInfos.Add(new ClientInfo(1, "localhost", 8080, 10));
            clientInfos.Add(new ClientInfo(2, "localhost", 8082, 2));
            clientInfos.Add(new ClientInfo(3, "localhost", 8083, 5));
            return Ok(clientInfos);*/
        }


    }
}
