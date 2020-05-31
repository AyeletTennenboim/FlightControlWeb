using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Text.Json.Serialization;
using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations;

namespace FlightControlWeb.FlightObjects
{
    public class FlightPlan
    {
        // Flight Plan properties.
        [JsonProperty("passengers")]
        [JsonPropertyName("passengers")]
        [Range(0, Int32.MaxValue - 1)]
        public int Passengers { get; set; } = -1;

        [JsonProperty("company_name")]
        [JsonPropertyName("company_name")]
        [Required]
        public string CompanyName { get; set; }

        [JsonProperty("initial_location")]
        [JsonPropertyName("initial_location")]
        [Required]
        public InitialLocation InitialLocation { get; set; }

        [JsonProperty("segments")]
        [JsonPropertyName("segments")]
        [Required]
        public Segment[] Segments { get; set; }
    }
}