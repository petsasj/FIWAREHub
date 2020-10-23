using System.Collections.Generic;
using Newtonsoft.Json;

namespace FIWAREHub.Models.ParserModels
{
    public class WeatherMappings
    {
        [JsonProperty("weatherMappings")]
        public List<WeatherMapping> Mappings { get; set; }
    }

    public class WeatherMapping
    {
        public string Name { get; set; }

        public string WeatherEvent { get; set; }

        public string Severity { get;set; }

        public string CloudCoverage { get; set; }
    }
}
