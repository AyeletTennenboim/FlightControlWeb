using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FlightControlWeb.FlightObjects;

namespace FlightControlWeb.Models
{
    interface IFlightsManager
    {
        IEnumerable<Flight> GetInternalFlights(DateTime time);
        Task<IEnumerable<Flight>> GetAllFlights(DateTime time);
        string AddFlightPlan(FlightPlan plan);
        Task<FlightPlan> GetFlightPlanById(string id);
        void DeleteFlightById(string id);
    }
}
