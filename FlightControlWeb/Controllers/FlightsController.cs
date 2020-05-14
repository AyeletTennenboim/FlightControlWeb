using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FlightControlWeb.FlightObjects;
using FlightControlWeb.Models;
using Microsoft.AspNetCore.Mvc;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace FlightControlWeb.Controllers
{
    [Route("api/[controller]")]
    public class FlightsController : Controller
    {
        private IFlightsManager flightsManager = new FlightsManager();

        // GET: api/Flights
        [HttpGet]
        public IEnumerable<Flight> GetAllFlights()
        {
            return flightsManager.GetAllFlights();
        }

        // DELETE api/Flights/5
        [HttpDelete("{id}")]
        public void DeleteFlightById(string id)
        {
            flightsManager.DeleteFlightById(id);
        }
    }
}
