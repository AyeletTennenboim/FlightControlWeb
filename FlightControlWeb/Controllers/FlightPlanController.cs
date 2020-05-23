using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FlightControlWeb.FlightObjects;
using FlightControlWeb.Models;
using Microsoft.AspNetCore.Mvc;

namespace FlightControlWeb.Controllers
{
    [Route("api/[controller]")]
    public class FlightPlanController : Controller
    {
        private IFlightsManager flightsManager = new FlightsManager();

        // GET api/FlightPlan/id
        [HttpGet("{id}")]
        public async Task<ActionResult<FlightPlan>> GetFlightPlanById(string id)
        {
            FlightPlan plan;
            try
            {
                plan = await flightsManager.GetFlightPlanById(id);
                // 200 status code (success) - The resource has been fetched and is transmitted
                // in the message body.
                return Ok(plan);
            }
            catch (Exception)
            {
                // 404 status code (error) - The server can not find the requested resource.
                return NotFound();
            }
        }

        // POST api/FlightPlan
        [HttpPost]
        public ActionResult<FlightPlan> AddFlightPlan([FromBody]FlightPlan plan)
        {
            // If client input is invalid
            if (plan == null)
            {
                // 400 status code (error) - The server cannot process the request.
                return BadRequest();
            }
            try
            {
                string randomId = flightsManager.AddFlightPlan(plan);
                // 201 status code (success) - The request has succeeded and a new resource has
                // been created as a result (or 400 status code in case of failure).
                return CreatedAtAction(nameof(GetFlightPlanById), new { id = randomId }, plan);
            }
            catch (Exception)
            {
                // 400 status code (error) - The server cannot process the request.
                return BadRequest();
            }
        }
    }
}
