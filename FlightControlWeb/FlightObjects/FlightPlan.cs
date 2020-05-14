using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Text.Json.Serialization;
using Newtonsoft.Json;

namespace FlightControlWeb.FlightObjects
{
    public class FlightPlan
    {
        [JsonPropertyName("passengers")]
        public int Passengers { get; set; }

        [JsonPropertyName("company_name")]
        public string CompanyName { get; set; }

        [JsonPropertyName("initial_location")]
        public InitialLocation InitialLocation { get; set; }

        [JsonPropertyName("segments")]
        public Segment[] Segments { get; set; }
    }
}