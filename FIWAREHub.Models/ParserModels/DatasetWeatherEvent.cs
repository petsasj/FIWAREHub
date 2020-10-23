using System;
using FileHelpers;

namespace FIWAREHub.Models.ParserModels
{
    [DelimitedRecord(","), IgnoreFirst(1)]
    public class DatasetWeatherEvent
    {
        public string EventId { get; set; }

        public string WeatherEventType { get; set; }

        public string Severity { get; set; }

        [FieldConverter(ConverterKind.Date, "yyyy-MM-dd HH:mm:ss")]
        public DateTime StartTime{ get; set; }

        [FieldConverter(ConverterKind.Date, "yyyy-MM-dd HH:mm:ss")]
        public DateTime EndTime { get; set; }

        public string TimeZone { get; set; }

        public string AirportCode { get; set; }

        public string LocationLatitude { get; set; }

        public string LocationLongitude { get; set; }

        public string City { get; set; }

        public string County { get; set; }

        public string State { get; set; }

        public string ZipCode { get; set; }
    }
}
