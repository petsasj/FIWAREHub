using System;
using System.Linq;
using Newtonsoft.Json;
using FIWAREHub.Models.ParserModels;

namespace FIWAREHub.Models.ParserModels
{
    public class FiwareWeatherReport
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
        public double? Temperature { get; set; }

        [JsonProperty("wch")]
        public double? WindChill { get; set; }

        [JsonProperty("hm")]
        public double? Humidity { get; set; }

        [JsonProperty("pr")]
        public double? Pressure { get; set; }

        [JsonProperty("vs")]
        public double? Visibility { get; set; }

        [JsonProperty("cc")]
        public string CloudCoverage { get; private set; }

        [JsonProperty("wd")]
        public string WindDirection { get; set; }

        [JsonProperty("ws")]
        public double? WindSpeed { get; set; }

        [JsonProperty("pcpt")]
        public double? Precipitation { get; set; }

        [JsonProperty("ct")]
        public string City { get; set; }

        [JsonProperty("cn")]
        public string County { get; set; }

        [JsonProperty("st")]
        public string State { get; set; }

        [JsonProperty("zc")]
        public string ZipCode { get; set; }

        [JsonProperty("c")]
        public string Country { get; set; }

        [JsonProperty("uid")]
        public long UID { get; set; }

        /// <summary>
        /// Used only for Test Actions
        /// </summary>
        [Obsolete]
        public FiwareWeatherReport(long uid)
        {
            UID = uid;
        }

        public FiwareWeatherReport(string weatherCondition, WeatherMappings weatherMappings, DatasetAccidentReport accidentReport, long uid)
        {
            if (string.IsNullOrWhiteSpace(weatherCondition))
                return;

            // Map weather condition to more statistical-friendly properties
            SetWeatherEventProperties(weatherCondition, weatherMappings);

            Humidity = accidentReport.Humidity;
            Precipitation = accidentReport.Precipitation;
            Pressure = accidentReport.Pressure;
            ReportTime = accidentReport.WeatherTimestamp;
            Temperature = accidentReport.Temperature;
            Visibility = accidentReport.Visibility;
            WindChill = accidentReport.WindChill;
            WindDirection = accidentReport.WindDirection;
            WindSpeed = accidentReport.WindSpeed;
            City = accidentReport.City;
            Country = accidentReport.Country;
            County = accidentReport.County;
            State = accidentReport.State;
            ZipCode = accidentReport.ZipCode;
            UID = uid;
        }


        /// <summary>
        /// This method maps the Original Weather Condition value
        /// to more statistical appropriate sub-properties
        /// </summary>
        /// <param name="weatherCondition"></param>
        private void SetWeatherEventProperties(string weatherCondition, WeatherMappings weatherMappings)
        {
            this.OriginalWeatherConditionDescription = weatherCondition;

            var weatherMapping = weatherMappings.Mappings
                .SingleOrDefault(wm => wm.Name.ToLower() == weatherCondition.ToLower());

            if (weatherMapping == null)
                throw new ArgumentException($"Weather mapping with name of {weatherCondition} not found.");

            this.Severity = weatherMapping.Severity;
            this.WeatherEvent = weatherMapping.WeatherEvent;
            this.CloudCoverage = weatherMapping.CloudCoverage;
        }
    }
}
