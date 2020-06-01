using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Text.Json.Serialization;
using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations;

namespace FlightControlWeb.FlightObjects
{
    public class Flight
    {
        // Flight properties.
        [JsonProperty("flight_id")]
        [JsonPropertyName("flight_id")]
        [Required]
        public string FlightId { get; set; }

        [JsonProperty("longitude")]
        [JsonPropertyName("longitude")]
        [Range(-180.0, 180.0)]
        public double Longitude { get; set; }

        [JsonProperty("latitude")]
        [JsonPropertyName("latitude")]
        [Range(-90.0, 90.0)]
        public double Latitude { get; set; }

        [JsonProperty("passengers")]
        [JsonPropertyName("passengers")]
        [Range(0, Int32.MaxValue - 1)]
        public int Passengers { get; set; }

        [JsonProperty("company_name")]
        [JsonPropertyName("company_name")]
        [Required]
        public string CompanyName { get; set; }

        [JsonProperty("date_time")]
        [JsonPropertyName("date_time")]
        [Range(typeof(DateTime), "0001-01-01T00:00:00Z", "9999-12-31T11:59:59Z")]
        public DateTime DateTime { get; set; }

        [JsonProperty("is_external")]
        [JsonPropertyName("is_external")]
        public bool IsExternal { get; set; }
    }
}
