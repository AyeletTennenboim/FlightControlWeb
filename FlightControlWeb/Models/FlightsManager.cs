using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using FlightControlWeb.FlightObjects;
using Newtonsoft.Json;

namespace FlightControlWeb.Models
{
    public class FlightsManager : IFlightsManager
    {
        private static ConcurrentDictionary<string, FlightPlan> flightPlans =
            new ConcurrentDictionary<string, FlightPlan>();
        private static IServersManager serversManager = new ServersManager();
        private static readonly HttpClient client = new HttpClient();
        private static readonly JsonSerializerOptions options =
            new JsonSerializerOptions() { PropertyNameCaseInsensitive = true };

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
        public async Task<IEnumerable<Flight>> GetAllFlights(DateTime time)
        {
            string request;
            IEnumerable<Server> externalServers = serversManager.GetExternalServers();
            List<Flight> flightsList = new List<Flight>(), externalFlights = new List<Flight>(),
                desFlights;

            // Get active internal flights
            flightsList.AddRange(GetInternalFlights(time));
            // Get active external flights
            foreach (Server server in externalServers)
            {
                request = server.ServerUrl + "/api/Flights?relative_to="
                    + time.ToString("yyyy-MM-ddTHH:mm:ssZ");
                HttpResponseMessage response = client.GetAsync(request).Result;
                if (response.IsSuccessStatusCode)
                {
                    // Get the response
                    string flightsJsonString = await response.Content.ReadAsStringAsync();
                    // Deserialize the data
                    desFlights = System.Text.Json.JsonSerializer.Deserialize<List<Flight>>
                        (flightsJsonString, options);
                    // Add the flights received from the server to the list of external flights
                    externalFlights.AddRange(desFlights);
                }
            }
            // Indicate flights as external and add them to general flights list
            foreach (Flight flight in externalFlights)
            {
                flight.IsExternal = true;
                flightsList.Add(flight);
            }
            return flightsList;
        }

        // Add flight plan with unique flight ID
        public void AddFlightPlan(FlightPlan plan)
        {
            string id = RandomFlightId();
            flightPlans.TryAdd(id, plan);
        }

        // Get flight plan by flight ID
        public async Task<FlightPlan> GetFlightPlanById(string id)
        {
            string request;
            FlightPlan plan;
            IEnumerable<Server> externalServers = serversManager.GetExternalServers();

            // If the ID is an internal flight ID
            if (flightPlans.TryGetValue(id, out plan))
            {
                return plan;
            }
            else
            {
                // Check if the ID is an external flight ID
                foreach (Server server in externalServers)
                {
                    request = server.ServerUrl + "/api/FlightPlan/" + id;
                    HttpResponseMessage response = client.GetAsync(request).Result;
                    // If the ID is an external flight ID
                    if (response.IsSuccessStatusCode)
                    {
                        // Get the response
                        string planJsonString = await response.Content.ReadAsStringAsync();
                        // Deserialize the data
                        plan = System.Text.Json.JsonSerializer.Deserialize<FlightPlan>
                            (planJsonString, options);
                        return plan;
                    }
                }
            }
            // If the ID does not exist for both internal and external flights - throw exception
            throw new Exception("Flight plan not found");
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
            double longitude = 0, latitude = 0, startLon, startLat, endLon, endLat,
                timeDifference, ratio;
            DateTime segmentStartTime = plan.InitialLocation.DateTime, segmentEndTime;

            for (i = 0; i <= numOfSegments; i++)
            {
                // Calculate segment end time
                segmentEndTime = segmentStartTime.AddSeconds(plan.Segments[i].TimespanSeconds);
                // If the flight is in this segement
                if (DateTime.Compare(segmentStartTime, currentTime) <= 0
                    && DateTime.Compare(currentTime, segmentEndTime) <= 0)
                {
                    // Calculate the time that has passed since the beginning of the segment
                    timeDifference = currentTime.Subtract(segmentStartTime).TotalSeconds;
                    // Calculate the ratio of time passed from the beginning of a segment
                    // to the entire segment time
                    ratio = timeDifference / plan.Segments[i].TimespanSeconds;

                    // Get the endpoints of the segment
                    if (i == 0)
                    {
                        startLon = plan.InitialLocation.Longitude;
                        startLat = plan.InitialLocation.Latitude;
                    }
                    else
                    {
                        startLon = plan.Segments[i - 1].Longitude;
                        startLat = plan.Segments[i - 1].Latitude;
                    }
                    endLon = plan.Segments[i].Longitude;
                    endLat = plan.Segments[i].Latitude;

                    // Calculate current longitude and latitude
                    longitude = ((1 - ratio) * startLon) + (ratio * endLon);
                    latitude = ((1 - ratio) * startLat) + (ratio * endLat);
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
