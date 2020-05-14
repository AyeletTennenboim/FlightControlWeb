using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FlightControlWeb.FlightObjects;

namespace FlightControlWeb.Models
{
    public class FlightsManager : IFlightsManager
    {
        private static ConcurrentDictionary<string, Flight> flights =
            new ConcurrentDictionary<string, Flight>();
        private static ConcurrentDictionary<string, FlightPlan> flightPlans =
            new ConcurrentDictionary<string, FlightPlan>();

        public IEnumerable<Flight> GetInternalFlights()
        {
            List<Flight> flightsList = new List<Flight>();
            flightsList.AddRange(flights.Values);
            return flightsList;
        }

        public IEnumerable<Flight> GetAllFlights()
        {
            List<Flight> flightsList = new List<Flight>();
            flightsList.AddRange(flights.Values);
            return flightsList;
        }

        public void AddFlightPlan(FlightPlan plan)
        {
            string id = RandomFlightId();
            Flight flight = new Flight
            {
                FlightId = id,
                Longitude = plan.InitialLocation.Longitude,
                Latitude = plan.InitialLocation.Latitude,
                Passengers = plan.Passengers,
                CompanyName = plan.CompanyName,
                DateTime = plan.InitialLocation.DateTime,
                IsExternal = false
            };
            flights.TryAdd(id, flight);
            flightPlans.TryAdd(id, plan);
        }

        public FlightPlan GetFlightPlanById(string id)
        {
            FlightPlan plan;
            if (flightPlans.TryGetValue(id, out plan))
            {
                return plan;
            }
            else
            {
                throw new Exception("Flight plan not found");
            }
        }

        public void DeleteFlightById(string id)
        {
            ///////////////////////////////////////// REMOVE !!!
            /*
            flights.TryAdd("1", new Flight { FlightId = "AB1234", Longitude = 33.244, Latitude = 31.12, Passengers = 216, CompanyName = "SwissAir", DateTime = "2020-12-26T23:56:21Z", IsExternal = false });
            flights.TryAdd("2", new Flight { FlightId = "ZY9876", Longitude = 43.244, Latitude = 41.12, Passengers = 416, CompanyName = "ElAl", DateTime = "2020-05-13T23:56:21Z", IsExternal = true });
            */

            if (flights.ContainsKey(id))
            {
                Flight removedFlight;
                FlightPlan removedPlan;
                flights.TryRemove(id, out removedFlight);
                flightPlans.TryRemove(id, out removedPlan);
            }
            else
            {
                throw new Exception("Flight not found");
            }        
        }

        // Generate a random unique flight ID
        public string RandomFlightId()
        {
            StringBuilder builder = new StringBuilder();
            Random random = new Random();
            char ch;
            for (int i = 0; i < 2; i++)
            {
                ch = Convert.ToChar(Convert.ToInt32(Math.Floor(26 * random.NextDouble() + 65)));
                builder.Append(ch);
            }
            builder.Append(random.Next(1000, 9999));
            return builder.ToString();
        }
    }
}
