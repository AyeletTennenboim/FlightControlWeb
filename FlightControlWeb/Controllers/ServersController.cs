using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FlightControlWeb.FlightObjects;
using FlightControlWeb.Models;
using Microsoft.AspNetCore.Mvc;

namespace FlightControlWeb.Controllers
{
    [Route("api/[controller]")]
    public class ServersController : Controller
    {
        private IServersManager serversManager = new ServersManager();

        // GET: api/Servers
        [HttpGet]
        public IEnumerable<Server> GetExternalServers()
        {
            return serversManager.GetExternalServers();
        }

        // POST api/Servers
        [HttpPost]
        public Server AddServer([FromBody]Server server)
        {
            serversManager.AddServer(server);
            return server;
        }

        // DELETE api/Servers/id
        [HttpDelete("{id}")]
        public void DeleteServerById(string id)
        {
            serversManager.DeleteServerById(id);
        }
    }
}
