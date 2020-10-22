using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using Newtonsoft.Json;

namespace FIWAREHub.Parsers.Models
{
    public class POSTWeatherReport
    {
        [JsonProperty("rt")]
        public DateTime? ReportTime { get; set; }

        [JsonProperty("we")]
        public string WeatherEvent { get; private set; }

        [JsonProperty("sv")]
        public string Severity { get; private set; }

        [JsonProperty("owcd")]
        public string OriginalWeatherConditionDescription { get; private set; }

        [JsonProperty("tmp")]
        public decimal? Temperature { get; set; }

        [JsonProperty("wch")]
        public decimal? WindChill { get; set; }

        [JsonProperty("hm")]
        public decimal? Humidity { get; set; }

        [JsonProperty("pr")]
        public decimal? Pressure { get; set; }

        [JsonProperty("vs")]
        public decimal? Visibility { get; set; }

        [JsonProperty("cc")]
        public string CloudCoverage { get; private set; }

        [JsonProperty("wd")]
        public string WindDirection { get; set; }

        [JsonProperty("ws")]
        public decimal? WindSpeed { get; set; }

        [JsonProperty("pcpt")]
        public decimal? Precipitation { get; set; }

        [Obsolete]
        public POSTWeatherReport()
        {
            
        }

        public POSTWeatherReport(string weatherCondition, WeatherMappingList weatherMappings)
        {
            if (string.IsNullOrWhiteSpace(weatherCondition))
                return;

            setWeatherEventProperties(weatherCondition, weatherMappings);
        }


        /// <summary>
        /// This method maps the Original Weather Condition value
        /// to more statistical appropriate sub-properties
        /// </summary>
        /// <param name="weatherCondition"></param>
        private void setWeatherEventProperties(string weatherCondition, WeatherMappingList weatherMappings)
        {
            this.OriginalWeatherConditionDescription = weatherCondition;

            var weatherMapping = weatherMappings.WeatherMappings
                .SingleOrDefault(wm => wm.Name.ToLower() == weatherCondition.ToLower());

            if (weatherMapping == null)
                throw new ArgumentException($"Weather mapping with name of {weatherCondition} not found.");

            this.Severity = weatherMapping.Severity;
            this.WeatherEvent = weatherMapping.WeatherEvent;
            this.CloudCoverage = weatherMapping.CloudCoverage;
        }
    }
}
