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
        [JsonPropertyName("passengers")]
        [Range(0, Int32.MaxValue - 1)]
        public int Passengers { get; set; } = -1;

        [JsonPropertyName("company_name")]
        [Required]
        public string CompanyName { get; set; }

        [JsonPropertyName("initial_location")]
        [Required]
        public InitialLocation InitialLocation { get; set; }

        [JsonPropertyName("segments")]
        [Required]
        public Segment[] Segments { get; set; }
    }
}