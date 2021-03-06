﻿using System;
using FileHelpers;

namespace FIWAREHub.Models.ParserModels
{
    [DelimitedRecord(","), IgnoreFirst(1)]
    public class DatasetAccidentReport
    {
        public string Id { get; set; }

        public string Source { get; set; }

        public string Tmc { get; set; }

        public int Severity { get; set; }

        [FieldConverter(ConverterKind.Date, "yyyy-MM-dd HH:mm:ss")]
        public DateTime? StartTime { get; set; }

        [FieldConverter(ConverterKind.Date, "yyyy-MM-dd HH:mm:ss")]
        public DateTime? EndTime { get; set; }

        public double? StartLatitude { get; set; }

        public double? StartLongitude { get; set; }

        public string EndLatitude { get; set; }

        public string EndLongitude { get; set; }

        public double? Distance { get; set; }

        public string Description { get; set; }

        public string AddressNumber { get; set; }

        public string Street { get; set; }

        public string Side { get; set; }

        public string City { get; set; }

        public string County { get; set; }

        public string State { get; set; }

        public string ZipCode { get; set; }

        public string Country { get; set; }

        public string Timezone { get; set; }

        public string AirportCode { get; set; }

        [FieldConverter(ConverterKind.Date, "yyyy-MM-dd HH:mm:ss")]
        public DateTime? WeatherTimestamp { get; set; }

        public double? Temperature { get; set; }

        public double? WindChill { get; set; }

        public double? Humidity { get; set; }

        public double? Pressure { get; set; }

        public double? Visibility { get; set; }

        public string WindDirection { get; set; }

        public double? WindSpeed { get; set; }

        public double? Precipitation { get; set; }

        public string WeatherCondition { get; set; }

        [FieldConverter(ConverterKind.Boolean)]
        public bool Amenity { get; set; }

        [FieldConverter(ConverterKind.Boolean)]
        public bool Bump { get; set; }

        [FieldConverter(ConverterKind.Boolean)]
        public bool Crossing { get; set; }

        [FieldConverter(ConverterKind.Boolean)]
        public bool GiveWay { get; set; }

        [FieldConverter(ConverterKind.Boolean)]
        public bool Junction { get; set; }

        [FieldConverter(ConverterKind.Boolean)]
        public bool NoExit { get; set; }

        [FieldConverter(ConverterKind.Boolean)]
        public bool Railway { get; set; }

        [FieldConverter(ConverterKind.Boolean)]
        public bool RoundAbout { get; set; }

        [FieldConverter(ConverterKind.Boolean)]
        public bool Station { get; set; }

        [FieldConverter(ConverterKind.Boolean)]
        public bool Stop { get; set; }

        [FieldConverter(ConverterKind.Boolean)]
        public bool TrafficCalming { get; set; }

        [FieldConverter(ConverterKind.Boolean)]
        public bool TrafficSignal { get; set; }

        [FieldConverter(ConverterKind.Boolean)]
        public bool TurningLoop { get; set; }

        public string SunriseSunset { get; set; }

        public string CivilTwilight { get; set; }

        public string NauticalTwilight { get; set; }

        public string AstronomicalTwilight { get; set; }
    }
}
