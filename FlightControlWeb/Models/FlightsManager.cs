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
        private IDictionary<string, FlightPlan> flightPlans;
        private IDictionary<string, Server> flightsAndServers;
        private IList<Server> externalServers;
        private HttpClient client;

        // Constructor.
        public FlightsManager (IDictionary<string, FlightPlan> flightPlansDict,
            IList<Server> servers, IDictionary<string, Server> flightsAndServersDic,
            HttpClient httpClient)
        {
            flightPlans = flightPlansDict;
            flightsAndServers = flightsAndServersDic;
            externalServers = servers;
            client = httpClient;
        }

        // Get all active internal flights.
        public IEnumerable<Flight> GetInternalFlights(DateTime time)
        {
            string id;
            Tuple<double, double> location;
            Flight flight;
            FlightPlan flightPlan;
            DateTime startTime, endTime;
            List<Flight> activeFlights = new List<Flight>();

            // Go over all internal flights.
            foreach (var plan in flightPlans)
            {
                id = plan.Key;
                if (!flightPlans.TryGetValue(id, out flightPlan))
                {
                    throw new Exception("Error getting active internal flights");
                }
                // Get the flight start and end time.
                startTime = flightPlan.InitialLocation.DateTime;
                endTime = flightPlan.InitialLocation.DateTime.
                    AddSeconds(getFlightDuration(flightPlan));
                // If flight is active.
                if (DateTime.Compare(startTime, time) <= 0 && DateTime.Compare(time, endTime) <= 0)
                {
                    location = getCurrentLocation(flightPlan, time);
                    // Create flight according to the flight plan.
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
                    // Add the flight to the list.
                    activeFlights.Add(flight);
                }
            }
            return activeFlights;
        }

        // Get all active internal and external flights.
        public async Task<IEnumerable<Flight>> GetAllFlights(DateTime time)
        {
            List<Flight> flightsList = new List<Flight>(), externalFlights = new List<Flight>();

            // Get active internal flights.
            flightsList.AddRange(GetInternalFlights(time));
            // Get active external flights.
            foreach (Server server in externalServers)
            {
                try
                {
                    flightsList.AddRange(await GetFlightsFromExternalServer(time, server));
                }
                catch (Exception)
                {
                    // Ignore external server issues and continue to the next iteration.
                }
            }
            return flightsList;
        }

        // Get external flights from a specific server.
        private async Task<List<Flight>> GetFlightsFromExternalServer(DateTime time, Server server)
        {
            string request, serverUrl;
            List<Flight> externalFlights = new List<Flight>();

            // If server Url ends with "/".
            if (server.ServerUrl.EndsWith("/"))
            {
                // Remove the last "/".
                serverUrl = server.ServerUrl.Remove(server.ServerUrl.Length - 1);
            }
            else
            {
                serverUrl = server.ServerUrl;
            }
            // Send a request to the server to get all its active flights.
            request = serverUrl + "/api/Flights?relative_to="
                + time.ToString("yyyy-MM-ddTHH:mm:ssZ");
            using (HttpClient client = new HttpClient())
            {
                HttpResponseMessage response = await client.GetAsync(request);
                // If the HTTP response is not successful.
                if (!response.IsSuccessStatusCode)
                {
                    throw new Exception("Error getting active external flights");
                }
                // Get the response.
                string flightsJsonString = await response.Content.ReadAsStringAsync();
                // Deserialize the data.
                externalFlights.AddRange(JsonConvert.
                    DeserializeObject<List<Flight>>(flightsJsonString));
                foreach (Flight flight in externalFlights)
                {
                    // Save the flight ID with the server from which it was received.
                    flightsAndServers.TryAdd(flight.FlightId, server);
                    // Indicate flight as external flight.
                    flight.IsExternal = true;
                }
            }
            return externalFlights;
        }

        // Add flight plan with unique flight ID.
        public string AddFlightPlan(FlightPlan plan)
        {
            string id = RandomFlightId();
            // If flight ID already exists in flight plans dictionary.
            if (!flightPlans.TryAdd(id, plan))
            {
                throw new Exception("Error: Use of an existing flight ID");
            }
            return id;
        }

        // Get flight plan by flight ID.
        public async Task<FlightPlan> GetFlightPlanById(string id)
        {
            string request, serverUrl;
            FlightPlan plan;
            Server server;

            // If the ID is an internal flight ID.
            if (flightPlans.TryGetValue(id, out plan))
            {
                return plan;
            }
            // If the ID is an external flight ID.
            else if (flightsAndServers.TryGetValue(id, out server))
            {
                // If server Url ends with "/".
                if (server.ServerUrl.EndsWith("/"))
                {
                    // Remove the last "/".
                    serverUrl = server.ServerUrl.Remove(server.ServerUrl.Length - 1);
                }
                else
                {
                    serverUrl = server.ServerUrl;
                }
                // Send a request to the server to get a specific flight.
                request = serverUrl + "/api/FlightPlan/" + id;
                using (client)
                {
                    HttpResponseMessage response = await client.GetAsync(request);
                    // If the HTTP response is successful.
                    if (response.IsSuccessStatusCode)
                    {
                        // Get the response.
                        string planJsonString = await response.Content.ReadAsStringAsync();
                        // Deserialize the data.
                        plan = JsonConvert.DeserializeObject<FlightPlan>(planJsonString);
                        return plan;
                    }
                    else
                    {
                        throw new Exception("Error: Flight plan not found");
                    }
                }
            }
            // If the ID doesn't exist in both internal and external flights - throw exception.
            throw new Exception("Error: Flight plan not found");
        }

        // Delete flight plan by flight ID.
        public void DeleteFlightById(string id)
        {
            if (flightPlans.ContainsKey(id))
            {
                if (!flightPlans.Remove(id))
                {
                    throw new Exception("Error: Flight cannot be removed");
                }
            }
            else
            {
                throw new Exception("Error: Flight to delete not found");
            }        
        }

        // Generate a random unique flight ID.
        private string RandomFlightId()
        {
            StringBuilder builder = new StringBuilder();
            Random random = new Random();
            char ch;
            // Choose two random letters.
            for (int i = 0; i < 2; i++)
            {
                ch = Convert.ToChar(Convert.ToInt32(Math.Floor(26 * random.NextDouble() + 65)));
                builder.Append(ch);
            }
            // Choose four random numbers.
            builder.Append(random.Next(1000, 9999));
            return builder.ToString();
        }

        // Calculate flight duration in seconds.
        private double getFlightDuration(FlightPlan plan)
        {
            double duration = 0;
            // Sum the duration of all segments.
            foreach (Segment segment in plan.Segments)
            {
                duration += segment.TimespanSeconds;
            }
            return duration;
        }

        // Calculate the flight location according to the requested time.
        private Tuple<double, double> getCurrentLocation(FlightPlan plan, DateTime currentTime)
        {
            int i, numOfSegments = plan.Segments.Length;
            double longitude = 0, latitude = 0, startLon, startLat, endLon, endLat,
                timeDifference, ratio;
            DateTime segmentStartTime = plan.InitialLocation.DateTime, segmentEndTime;

            for (i = 0; i <= numOfSegments; i++)
            {
                // Calculate segment end time.
                segmentEndTime = segmentStartTime.AddSeconds(plan.Segments[i].TimespanSeconds);
                // If the flight is in this segement.
                if (DateTime.Compare(segmentStartTime, currentTime) <= 0
                    && DateTime.Compare(currentTime, segmentEndTime) <= 0)
                {
                    // Calculate the time that has passed since the beginning of the segment.
                    timeDifference = currentTime.Subtract(segmentStartTime).TotalSeconds;
                    // Calculate the ratio of time passed from the beginning of a segment
                    // to the entire segment time.
                    ratio = timeDifference / plan.Segments[i].TimespanSeconds;

                    // Get the endpoints of the segment.
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

                    // Calculate current longitude and latitude.
                    longitude = ((1 - ratio) * startLon) + (ratio * endLon);
                    latitude = ((1 - ratio) * startLat) + (ratio * endLat);
                    break;
                }
                else
                {
                    // Set start time for the next segment.
                    segmentStartTime = segmentEndTime;
                }
            }
            // Return current location.
            return new Tuple<double, double>(longitude, latitude);
        }
    }
}
