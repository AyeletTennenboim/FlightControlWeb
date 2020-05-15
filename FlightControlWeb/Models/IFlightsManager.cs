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
        IEnumerable<Flight> GetAllFlights(DateTime time);
        void AddFlightPlan(FlightPlan plan);
        FlightPlan GetFlightPlanById(string id);
        void DeleteFlightById(string id);
    }
}
