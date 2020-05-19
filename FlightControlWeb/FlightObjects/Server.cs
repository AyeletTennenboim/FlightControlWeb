using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Text.Json.Serialization;
using Newtonsoft.Json;

namespace FlightControlWeb.FlightObjects
{
    public class Server
    {
        [JsonPropertyName("ServerId")]
        public string ServerId { get; set; }

        [JsonPropertyName("ServerURL")]
        public string ServerUrl { get; set; }
    }
}
