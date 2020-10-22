﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using FileHelpers;
using FIWAREHub.Parsers.Extensions;
using FIWAREHub.Parsers.Models;
using FIWAREHub.Parsers.ViewModels;

namespace FIWAREHub.Parsers
{
    public class FileParser
    {
        private const string WeatherCsv = @"C:\Users\yiannis\Desktop\Thesis\US_Weather_Events\US_WeatherEvents_2016-2019.csv";

        private const string AccidentCsv = @"C:\Users\yiannis\Desktop\Thesis\US_Accidents_Dataset_2016_2020\US_Accidents_Dataset_2016_2020.csv";

        public IEnumerable<DatasetWeatherEvent> ParseWeatherEvents()
        {
            var engine = new FileHelperEngine<DatasetWeatherEvent>();

            var weatherEvents = engine
                .ReadFile(WeatherCsv)
                .ToList();

            var severityScale = weatherEvents.Select(we => we.Severity).DistinctBy(s => s).ToList();

            return weatherEvents;
        }

        public IEnumerable<IoTCombinedReport> ParseAccidentsDataset()
        {
            var currentDirectory = System.IO.Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            var mappingsFileLocation = @"..\..\..\..\FIWAREHub.Parsers\WeatherMapping.json";
            var combined = System.IO.Path.GetFullPath(System.IO.Path.Combine(currentDirectory, mappingsFileLocation));
            var fileExists = System.IO.File.Exists(combined);

            if (!fileExists)
                throw new ArgumentException("Weather mappings json is missing.");

            var fileAsString = System.IO.File.ReadAllText(combined);
            var weatherMappings = Newtonsoft.Json.JsonConvert.DeserializeObject<WeatherMappingList>(fileAsString);


            var engine = new FileHelperEngine<DatasetAccidentReport>();

            var accidents = engine
                .ReadFile(AccidentCsv)
                .ToList();


            // Two top states with most accidents
            var accidentsOfTwoTopStates = accidents
                // Year 2017, 2018
                .Where(a => new[] {2017, 2018}.Contains(a.StartTime.GetValueOrDefault().Year))
                // Omits accident reports without weather condition
                .Where(a => !string.IsNullOrWhiteSpace(a.WeatherCondition))
                .GroupBy(a => a.State)
                .OrderByDescending(g => g.Count())
                .Take(2)
                .SelectMany(g => g.ToList())
                .Select(ar => new IoTCombinedReport
                {
                    WeatherReport = new POSTWeatherReport(ar.WeatherCondition, weatherMappings)
                    {
                        Humidity = ar.Humidity,
                        Precipitation = ar.Precipitation,
                        Pressure = ar.Pressure,
                        ReportTime = ar.WeatherTimestamp,
                        Temperature = ar.Temperature,
                        Visibility = ar.Visibility,
                        WindChill = ar.WindChill,
                        WindDirection = ar.WindDirection,
                        WindSpeed = ar.WindSpeed
                    },
                    TrafficData = new POSTTrafficReport
                    {
                        StartTime = ar.StartTime,
                        AddressNumber = ar.AddressNumber,
                        City = ar.City,
                        Country = ar.Country,
                        County = ar.County,
                        Description = ar.Description,
                        Distance = ar.Distance,
                        Severity = ar.Severity,
                        State = ar.State,
                        Side = ar.Side,
                        StartLatitude = ar.StartLatitude,
                        StartLongitude = ar.StartLongitude,
                        Street = ar.Street,
                        ZipCode = ar.ZipCode

                    }
                })
                .ToList();


            return accidentsOfTwoTopStates;
        }
    }
}
