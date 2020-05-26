using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FlightControlWeb.FlightObjects;
using FlightControlWeb.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace FlightControlWeb.Controllers
{
    [Route("api/[controller]")]
    public class FlightsController : Controller
    {
        private IFlightsManager flightsManager = new FlightsManager();

        // GET: api/Flights?relative_to=<DATE_TIME>
        [HttpGet]
        public async Task<IEnumerable<Flight>> GetAllFlights(DateTime relative_to)
        {
            IEnumerable<Flight> flights = new List<Flight>();
            // Convert time to UTC
            //DateTime time = TimeZoneInfo.ConvertTimeToUtc(relative_to);
            string parameters = Request.QueryString.Value;

            if (parameters.Contains("sync_all"))
            {
                // Return all active internal and external flights
                flights = await flightsManager.GetAllFlights(relative_to);
            }
            else
            {
                // Return all active internal flights
                flights = flightsManager.GetInternalFlights(relative_to);
            }
            return flights;
        }

        // DELETE api/Flights/id
        [HttpDelete("{id}")]
        public void DeleteFlightById(string id)
        {
            flightsManager.DeleteFlightById(id);
        }
    }
}
