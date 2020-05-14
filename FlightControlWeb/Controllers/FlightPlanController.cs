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
    public class FlightPlanController : Controller
    {
        private IFlightsManager flightsManager = new FlightsManager();

        // GET api/FlightPlan/5
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
