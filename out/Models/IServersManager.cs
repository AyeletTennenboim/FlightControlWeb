using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FlightControlWeb.FlightObjects;

namespace FlightControlWeb.Models
{
    interface IServersManager
    {
        IEnumerable<Server> GetExternalServers();
        void AddServer(Server server);
        void DeleteServerById(string id);
    }
}
