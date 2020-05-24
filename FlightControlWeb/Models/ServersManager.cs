using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FlightControlWeb.FlightObjects;

namespace FlightControlWeb.Models
{
    public class ServersManager : IServersManager
    {
        private IList<Server> externalServers;

        // Constructor.
        public ServersManager (IList<Server> servers)
        {
            externalServers = servers;
        }

        // Get a list of external servers from which the server synchronizes information.
        public IEnumerable<Server> GetExternalServers()
        {
            return externalServers;
        }

        // Add new server to synchronize flights.
        public void AddServer(Server server)
        {
            externalServers.Add(server);
        }

        // Delete server by server ID.
        public void DeleteServerById(string id)
        {
            // Get the server with the given ID.
            Server server = externalServers.Where(x => x.ServerId == id).FirstOrDefault();
            // If server doesn't exist.
            if (server == null)
            {
                throw new Exception("Error: Server to delete not found");
            }
            // If server exists but cannot be removed.
            else if (!externalServers.Remove(server))
            {
                throw new Exception("Error: Server cannot be removed");
            }
        }
    }
}
