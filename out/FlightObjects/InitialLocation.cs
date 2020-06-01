using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace FlightControlWeb.FlightObjects
{
    public class InitialLocation
    {
        // Initial Location properties.
        [JsonProperty("longitude")]
        [JsonPropertyName("longitude")]
        [Range(-180.0, 180.0)]
        public double Longitude { get; set; } = 200;

        [JsonProperty("latitude")]
        [JsonPropertyName("latitude")]
        [Range(-90.0, 90.0)]
        public double Latitude { get; set; } = 100;

        [JsonProperty("date_time")]
        [JsonPropertyName("date_time")]
        public DateTime DateTime { get; set; }
    }
}
