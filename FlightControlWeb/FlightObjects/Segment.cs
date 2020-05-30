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
        [Range(-180.0, 180.0)]
        public double Longitude { get; set; } = 200;

        [JsonProperty("latitude")]
        [JsonPropertyName("latitude")]
        [Range(-90.0, 90.0)]
        public double Latitude { get; set; } = 100;

        [JsonProperty("timespan_seconds")]
        [JsonPropertyName("timespan_seconds")]
        [Range(0, Double.MaxValue - 1)]
        public double TimespanSeconds { get; set; } = -1;
    }
}
