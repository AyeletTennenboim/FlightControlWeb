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
        private static ConcurrentDictionary<string, FlightPlan> flightPlans =
            new ConcurrentDictionary<string, FlightPlan>();

        // Get all active internal flights
        public IEnumerable<Flight> GetInternalFlights(DateTime time)
        {
            string id;
            Tuple<double, double> location;
            Flight flight;
            FlightPlan flightPlan;
            DateTime startTime, endTime;
            List<Flight> activeFlights = new List<Flight>();

            // Go over all internal flights
            foreach (var plan in flightPlans)
            {
                id = plan.Key;
                if (!flightPlans.TryGetValue(id, out flightPlan))
                {
                    throw new Exception("Error getting active flights");
                }
                // Get the flight start and end time
                startTime = flightPlan.InitialLocation.DateTime;
                endTime = flightPlan.InitialLocation.DateTime.
                    AddSeconds(getFlightDuration(flightPlan));
                // If flight is active
                if (DateTime.Compare(startTime, time) <= 0 && DateTime.Compare(time, endTime) <= 0)
                {
                    location = getCurrentLocation(flightPlan, time);
                    // Create flight according to the flight plan
                    flight = new Flight
                    {
                        FlightId = id,
                        Longitude = location.Item1,
                        Latitude = location.Item2,
                        Passengers = flightPlan.Passengers,
                        CompanyName = flightPlan.CompanyName,
                        DateTime = flightPlan.InitialLocation.DateTime,
                        IsExternal = false
                    };
                    // Add the flight to the list
                    activeFlights.Add(flight);
                }
            }
            return activeFlights;
        }

        // Get all active internal and external flights
        public IEnumerable<Flight> GetAllFlights(DateTime time)
        {
            List<Flight> flightsList = new List<Flight>();
            // Get active internal flights
            flightsList.AddRange(GetInternalFlights(time));

            /*
             * Add external flights !
             */

            return flightsList;
        }

        // Add flight plan with unique flight ID
        public void AddFlightPlan(FlightPlan plan)
        {
            string id = RandomFlightId();
            flightPlans.TryAdd(id, plan);
        }

        // Get flight plan by flight ID
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

        // Delete flight plan by flight ID
        public void DeleteFlightById(string id)
        {
            if (flightPlans.ContainsKey(id))
            {
                FlightPlan removedPlan;
                flightPlans.TryRemove(id, out removedPlan);
            }
            else
            {
                throw new Exception("Flight not found");
            }        
        }

        // Generate a random unique flight ID
        private string RandomFlightId()
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

        // Calculate flight duration in seconds
        private double getFlightDuration(FlightPlan plan)
        {
            double duration = 0;
            foreach (Segment segment in plan.Segments)
            {
                duration += segment.TimespanSeconds;
            }
            return duration;
        }

        // Calculate the flight location according to the requested time
        private Tuple<double, double> getCurrentLocation(FlightPlan plan, DateTime currentTime)
        {
            int i, numOfSegments = plan.Segments.Length;
            double longitude = 0, latitude = 0, startLon, startLat, endLon, endLat;
            DateTime segmentStartTime = plan.InitialLocation.DateTime, segmentEndTime;

            for (i = 0; i <= numOfSegments; i++)
            {
                // Calculate segment end time
                segmentEndTime = segmentStartTime.AddSeconds(plan.Segments[i].TimespanSeconds);
                // If the flight is in this segement
                if (DateTime.Compare(segmentStartTime, currentTime) <= 0
                    && DateTime.Compare(currentTime, segmentEndTime) <= 0)
                {
                    // Get the endpoints of the segment
                    startLon = plan.Segments[i].Longitude;
                    startLat = plan.Segments[i].Latitude;
                    endLon = plan.Segments[i + 1].Longitude;
                    endLat = plan.Segments[i + 1].Latitude;




                    break;
                }
                else
                {
                    // Set start time for the next segment
                    segmentStartTime = segmentEndTime;
                }
            }
            // Return current location
            return new Tuple<double, double>(longitude, latitude);
        }
    }
}
