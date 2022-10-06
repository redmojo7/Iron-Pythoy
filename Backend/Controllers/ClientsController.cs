using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.ServiceModel;
using System.Web.Http;
using System.Web.Http.Description;
using Backend.Models;
using Desktop.Common;

namespace Backend.Controllers
{
    public class ClientsController : ApiController
    {
        private ClientDatabaseEntities db = new ClientDatabaseEntities();

        // GET: api/Clients
        public IQueryable<Client> GetClients()
        {
            return db.Clients;
        }

        [HttpGet]
        [Route("api/Clients/info")]
        public IHttpActionResult GetClientsInfo()
        {
            List<ClientInfo> clientInfos = new List<ClientInfo>();
            List<Client> clients = db.Clients.ToList();
            foreach (Client client in clients)
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
            return Ok(clientInfos);
        }

        // GET: api/Clients/5
        [ResponseType(typeof(Client))]
        public IHttpActionResult GetClient(int id)
        {
            Client client = db.Clients.Find(id);
            if (client == null)
            {
                return NotFound();
            }

            return Ok(client);
        }

        // PUT: api/Clients/5
        [ResponseType(typeof(void))]
        public IHttpActionResult PutClient(int id, Client client)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != client.Id)
            {
                return BadRequest();
            }

            db.Entry(client).State = EntityState.Modified;

            try
            {
                db.SaveChanges();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ClientExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return StatusCode(HttpStatusCode.NoContent);
        }

        // POST: api/Clients
        [ResponseType(typeof(Client))]
        public IHttpActionResult PostClient(Client client)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            db.Clients.Add(client);
            db.SaveChanges();

            return CreatedAtRoute("DefaultApi", new { id = client.Id }, client);
        }

        // DELETE: api/Clients/5
        [ResponseType(typeof(Client))]
        public IHttpActionResult DeleteClient(int id)
        {
            Client client = db.Clients.Find(id);
            if (client == null)
            {
                return NotFound();
            }

            db.Clients.Remove(client);
            db.SaveChanges();

            return Ok(client);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        private bool ClientExists(int id)
        {
            return db.Clients.Count(e => e.Id == id) > 0;
        }
    }
}