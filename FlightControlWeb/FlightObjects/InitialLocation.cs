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
        [Range(typeof(DateTime), "0001-01-01T00:00:00Z", "9999-12-31T11:59:59Z")]
        public DateTime DateTime { get; set; }
    }
}
