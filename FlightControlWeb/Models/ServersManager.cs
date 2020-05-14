using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FlightControlWeb.FlightObjects;

namespace FlightControlWeb.Models
{
    public class ServersManager : IServersManager
    {
        private static List<Server> externalServers = new List<Server>();

        public IEnumerable<Server> GetExternalServers()
        {
            return externalServers;
        }

        public void AddServer(Server server)
        {
            externalServers.Add(server);
        }

        public void DeleteServerById(string id)
        {
            ///////////////////////////////////////// REMOVE !!!          
            externalServers.Add(new Server { ServerId = "1", ServerUrl = "url" });
            externalServers.Add(new Server { ServerId = "2", ServerUrl = "url2" });
            
            Server server = externalServers.Where(x => x.ServerId == id).FirstOrDefault();
            if (server == null)
            {
                throw new Exception("Server not found");
            }
            else
            {
                externalServers.Remove(server);
            }
        }
    }
}
