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
    public class ServersController : Controller
    {
        private IServersManager serversManager;

        // Constructor uses dependency injection.
        public ServersController (IList<Server> servers)
        {
            serversManager = new ServersManager(servers);
        }

        // GET: api/Servers
        [HttpGet]
        public ActionResult<IEnumerable<Server>> GetExternalServers()
        {
            // 200 status code (success) - The resource has been fetched and is transmitted
            // in the message body.
            return Ok(serversManager.GetExternalServers());
        }

        // POST api/Servers
        [HttpPost]
        public ActionResult<Server> AddServer([FromBody]Server server)
        {
            // If client input is valid.
            if (ModelState.IsValid)
            {
                try
                {
                    serversManager.AddServer(server);
                    // 200 status code (success) - The resource describing the result of the action
                    // is transmitted in the message body.
                    return Ok(server);
                }
                catch (Exception)
                {
                    // 400 status code (error) - The server cannot process the request.
                    return BadRequest();
                }
            }
            // If client input is invalid.
            else
            {
                // 400 status code (error) - The server cannot process the request.
                return BadRequest();
            }
        }

        // DELETE api/Servers/id
        [HttpDelete("{id}")]
        public IActionResult DeleteServerById(string id)
        {
            try
            {
                serversManager.DeleteServerById(id);
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
