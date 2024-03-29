using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using FlightControlWeb.Controllers;
using FlightControlWeb.FlightObjects;
using Microsoft.AspNetCore.Mvc;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace UnitTestFlightControlWeb
{
    [TestClass]
    public class FlightPlanControllerTest
    {
        // Instance of the tested controller.
        private FlightPlanController flightPlanController;

        [TestInitialize]
        public void Initialize()
        {
            // Create collections for FlightPlanController.
            IDictionary<string, FlightPlan> flightPlans =
            new ConcurrentDictionary<string, FlightPlan>();
            IDictionary<string, Server> flightsAndServers =
                new ConcurrentDictionary<string, Server>();
            IList<Server> externalServers = new List<Server>();
            // Create external server and add it with fake flight ID to flightsAndServers list.
            Server server = new Server()
            {
                ServerId = "TestServer",
                ServerUrl = "http://www.TestServer.com/"
            };
            flightsAndServers.Add("AB1234", server);
            // Inject HttpMessageHandler into a new HttpClient.
            // It lets override the HttpClient's response method with a stub one.
            HttpClient httpClient = new HttpClient(new HttpMessageHandlerStub());
            // Create a new FlightPlanController with the objects created above.
            flightPlanController = new FlightPlanController(flightPlans, externalServers,
                flightsAndServers, httpClient);
        }

        [TestMethod]
        // Test the flight plan controller's "GET" (by ID) method with external flight ID.
        public async Task GetExternalFlightPlanByIdAsync()
        {
            // Arrange - Create FlightPlanController with injected HttpClient stub in
            // the Initialize() method.

            // Act - Request a flight plan by flight ID received from an external server.
            var response = await flightPlanController.GetFlightPlanById("AB1234")
                as ActionResult<FlightPlan>;
            var responseFlightPlan = response.Value as FlightPlan;

            // Assert - Check that the returned object is the correct flight plan.
            Assert.IsInstanceOfType(responseFlightPlan, typeof(FlightPlan));
            Assert.AreEqual("TestAir", responseFlightPlan.CompanyName);
            Assert.AreEqual(160, responseFlightPlan.Passengers);
            Assert.IsNotNull(responseFlightPlan.InitialLocation);
            Assert.IsNotNull(responseFlightPlan.Segments);
        }

        [TestMethod]
        // Test the flight plan controller's "POST" method with valid flight plan.
        public void AddValidFlightPlanReturnCreatedItem()
        {
            // Arrange - Create FlightPlan object.
            FlightPlan testFlightPlan = GetSampleFlightPlan();

            // Act - Post the flight plan and get the created response object.
            var createdResponse = flightPlanController.AddFlightPlan(testFlightPlan)
                as CreatedAtActionResult;
            var createdFlightPlan = createdResponse.Value as FlightPlan;

            // Assert - Check that the created object is a flight plan with correct details.
            Assert.IsInstanceOfType(createdFlightPlan, typeof(FlightPlan));
            Assert.AreEqual(200, createdFlightPlan.Passengers);
            Assert.AreEqual("EL-AL", createdFlightPlan.CompanyName);
            Assert.AreEqual(34.95, createdFlightPlan.InitialLocation.Longitude);
            Assert.AreEqual(29.55, createdFlightPlan.InitialLocation.Latitude);
            Assert.AreEqual("2020-05-31T23:30:00Z", TimeZoneInfo.ConvertTimeToUtc
                (createdFlightPlan.InitialLocation.DateTime).ToString("yyyy-MM-ddTHH:mm:ssZ"));
            Assert.AreEqual(35.21, createdFlightPlan.Segments[0].Longitude);
            Assert.AreEqual(31.76, createdFlightPlan.Segments[0].Latitude);
            Assert.AreEqual(1000, createdFlightPlan.Segments[0].TimespanSeconds);
            Assert.AreEqual(34.78, createdFlightPlan.Segments[1].Longitude);
            Assert.AreEqual(32.11, createdFlightPlan.Segments[1].Latitude);
            Assert.AreEqual(750, createdFlightPlan.Segments[1].TimespanSeconds);
        }

        [TestMethod]
        // Test the flight plan controller's "POST" method with invalid flight plan.
        public void AddInvalidFlightPlanReturnBadRequest()
        {
            // Arrange - Create invalid FlightPlan object.
            FlightPlan testFlightPlan = GetSampleFlightPlan();
            testFlightPlan.CompanyName = null;
            // Add the ModelError object explicitly to the ModelState to test invalid case.
            flightPlanController.ModelState.AddModelError("CompanyName", "Required");

            // Act - Post the flight plan and get the response.
            var badResponse = flightPlanController.AddFlightPlan(testFlightPlan);

            // Assert - Check that the returned object is a bad request object.
            Assert.IsInstanceOfType(badResponse, typeof(BadRequestResult));
        }

        // Create and return a FlightPlan object.
        private FlightPlan GetSampleFlightPlan()
        {
            // Create FlightPlan object.
            FlightPlan flightPlan = new FlightPlan()
            {
                Passengers = 200,
                CompanyName = "EL-AL",
                InitialLocation = new InitialLocation()
                {
                    Longitude = 34.95,
                    Latitude = 29.55,
                    DateTime = DateTime.Parse("2020-05-31T23:30:00Z")
                },
                Segments = new Segment[]
                {   new Segment()
                    {
                        Longitude = 35.21, Latitude = 31.76, TimespanSeconds = 1000
                    },
                    new Segment()
                    {
                        Longitude = 34.78, Latitude = 32.11, TimespanSeconds = 750
                    }
                }
            };
            return flightPlan;
        }
    }
}
