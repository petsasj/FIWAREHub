using System.Collections.Generic;

namespace FIWAREHub.Models.ParserModels
{
    public class WeatherMappings
    {
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
