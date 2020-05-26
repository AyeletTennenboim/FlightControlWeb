using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Text.Json.Serialization;
using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations;

namespace FlightControlWeb.FlightObjects
{
    public class Server
    {
        // Server properties.
        [JsonPropertyName("ServerId")]
        [Required]
        public string ServerId { get; set; }

        [JsonPropertyName("ServerURL")]
        [Required]
        public string ServerUrl { get; set; }
    }
}
