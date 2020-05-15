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
        public FlightPlan GetFlightPlanById(string id)
        {
            return flightsManager.GetFlightPlanById(id);
        }

        // POST api/FlightPlan
        [HttpPost]
        public FlightPlan AddFlightPlan([FromBody]FlightPlan plan)
        {
            flightsManager.AddFlightPlan(plan);
            return plan;
        }
    }
}
