using System;
using System.Collections.Generic;
using System.Text;

namespace FIWAREHub.Parsers.Models
{
    public class WeatherMappingList
    {
        public List<WeatherMapping> WeatherMappings { get; set; }
    }

    public class WeatherMapping
    {
        public string Name { get; set; }

        public string WeatherEvent { get; set; }

        public string Severity { get;set; }

        public string CloudCoverage { get; set; }
    }
}
