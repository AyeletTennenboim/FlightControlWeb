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

        // Get a list of external servers from which the server synchronizes information
        public IEnumerable<Server> GetExternalServers()
        {
            return externalServers;
        }

        // Add new server to synchronize flights
        public void AddServer(Server server)
        {
            externalServers.Add(server);
        }

        // Delete server by server ID
        public void DeleteServerById(string id)
        {
            // Get the server with the given ID
            Server server = externalServers.Where(x => x.ServerId == id).FirstOrDefault();
            // If server doesn't exist
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
