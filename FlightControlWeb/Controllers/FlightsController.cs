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
        public async Task<ActionResult<IEnumerable<Flight>>> GetAllFlights
            ([FromQuery(Name = "relative_to")] DateTime time)
        {
            IEnumerable<Flight> flights = new List<Flight>();
            string parameters = Request.QueryString.Value;
            try
            {
                if (parameters.Contains("sync_all"))
                {
                    // Return all active internal and external flights.
                    flights = await flightsManager.GetAllFlights(time);
                }
                else
                {
                    // Return all active internal flights.
                    flights = flightsManager.GetInternalFlights(time);
                }
                // 200 status code (success) - The resource has been fetched and is transmitted
                // in the message body.
                return Ok(flights);
            }
            catch (Exception)
            {
                // 404 status code (error) - The server can not find the requested resource.
                return NotFound();
            }
        }

        // DELETE api/Flights/id
        [HttpDelete("{id}")]
        public IActionResult DeleteFlightById(string id)
        {
            try
            {
                flightsManager.DeleteFlightById(id);
                // 204 status code (success) - There is no content to send for this request.
                return NoContent();
            }
            catch (Exception)
            {
                // 404 status code (error) - The server can not find the requested resource.
                return NotFound();
            }
        }
    }
}
