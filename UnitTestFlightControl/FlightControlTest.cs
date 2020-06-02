using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using FlightControlWeb.Controllers;
using FlightControlWeb.FlightObjects;
using Microsoft.AspNetCore.Mvc;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Xunit;

namespace UnitTestFlightControl
{
    [TestClass]
    public class FlightControlTest
    {
        // Controllers.
        private FlightsController flightsController;
        private FlightPlanController flightPlanController;
        private ServersController serversController;

        // Constructor.
        public FlightControlTest()
        {
            IDictionary<string, FlightPlan> flightPlans =
                new ConcurrentDictionary<string, FlightPlan>();
            IDictionary<string, Server> flightsAndServers =
                new ConcurrentDictionary<string, Server>();
            IList<Server> externalServers = new List<Server>();
            flightsController = new FlightsController(flightPlans, externalServers,
                flightsAndServers);
            flightPlanController = new FlightPlanController(flightPlans, externalServers,
                flightsAndServers);
            serversController = new ServersController(externalServers);
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

        [TestMethod]
        // Test the flight plan controller "POST" method with valid flight plan.
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
        // Test the flight plan controller "POST" method with invalid flight plan.
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
    }
}
