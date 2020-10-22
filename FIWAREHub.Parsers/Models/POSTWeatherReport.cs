using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;

namespace FIWAREHub.Parsers.Models
{
    public class POSTWeatherReport
    {
        public DateTime? ReportTime { get; set; }

        public string WeatherEvent { get; private set; }

        public string Severity { get; private set; }

        public string OriginalWeatherConditionDescription { get; private set; }

        public decimal? Temperature { get; set; }

        public decimal? WindChill { get; set; }

        public decimal? Humidity { get; set; }

        public decimal? Pressure { get; set; }

        public decimal? Visibility { get; set; }

        public string CloudCoverage { get; private set; }

        public string WindDirection { get; set; }

        public decimal? WindSpeed { get; set; }

        public decimal? Precipitation { get; set; }

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
