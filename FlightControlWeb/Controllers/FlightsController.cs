using System;
using System.Collections.Generic;
using System.Linq;
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
        public IEnumerable<Flight> GetAllFlights(DateTime relative_to)
        {
            // Convert time to UTC
            DateTime time = TimeZoneInfo.ConvertTimeToUtc(relative_to);
            string parameters = Request.QueryString.Value;
            if (parameters.Contains("sync_all"))
            {
                // Return all active internal and external flights
                return flightsManager.GetAllFlights(time);
            }
            else
            {
                // Return all active internal flights
                return flightsManager.GetInternalFlights(time);
            }
        }

        // DELETE api/Flights/id
        [HttpDelete("{id}")]
        public void DeleteFlightById(string id)
        {
            flightsManager.DeleteFlightById(id);
        }
    }
}
