using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace FlightControlWeb.FlightObjects
{
    public class Segment
    {
        // Segment properties.
        [JsonProperty("longitude")]
        [JsonPropertyName("longitude")]
        public double Longitude { get; set; }

        [JsonProperty("latitude")]
        [JsonPropertyName("latitude")]
        public double Latitude { get; set; }

        [JsonProperty("timespan_seconds")]
        [JsonPropertyName("timespan_seconds")]
        [Range(0, Double.MaxValue - 1)]
        public double TimespanSeconds { get; set; } = -1;
    }
}
