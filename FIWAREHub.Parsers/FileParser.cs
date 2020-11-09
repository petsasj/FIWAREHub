using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using FileHelpers;
using FIWAREHub.Models.ParserModels;
using FIWAREHub.Parsers.Extensions;


namespace FIWAREHub.Parsers
{
    public class FileParser
    {
        private const string ExtractLocationDebug = @"..\..\..\..\FIWAREHub.Datasets\extracted\";

        private const string ExtractLocationRelease = @"/ssapp/extracted";

        private const string AccidentZipDebug = @"..\..\..\..\FIWAREHub.Datasets\US_Accidents_Dataset_2016_2020.zip";

        private const string AccidentZipRelease = @"US_Accidents_Dataset_2016_2020.zip";

        private const string AccidentCsv = "US_Accidents_June20.csv";

        private const string MappingsLocationDebug = @"..\..\..\..\FIWAREHub.Parsers\WeatherMapping.json";

        private const string MappingsLocationRelease = @"WeatherMapping.json";

        private const string WeatherZipDebug = @"..\..\..\..\FIWAREHub.Datasets\US_Weather_Events.zip";

        private const string WeatherZipRelease = @"US_Weather_Events.zip";

        private const string WeatherCsv = "US_WeatherEvents_2016-2019.csv";

        public IEnumerable<DatasetWeatherEvent> ParseWeatherEvents()
        {
            var currentDirectory = System.IO.Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);

            // Nullity check, although this is known not to be null
            if (string.IsNullOrWhiteSpace(currentDirectory))
                throw new System.IO.FileNotFoundException("Current Directory is null");

            // Check if csv exists

#if DEBUG
            var weatherReportCsvFullLocation =
                System.IO.Path.GetFullPath(System.IO.Path.Combine(currentDirectory, ExtractLocationDebug, WeatherCsv));
#endif

#if !DEBUG
            var weatherReportCsvFullLocation =
                System.IO.Path.GetFullPath(System.IO.Path.Combine(currentDirectory, ExtractLocationRelease, WeatherCsv));
#endif
            
            var csvExists = System.IO.File.Exists(weatherReportCsvFullLocation);

            // if csv is missing, extract it from the zip
            if (!csvExists)
            {
                // Check if zip exists
#if DEBUG
                var weatherReportZipLocation =
                    System.IO.Path.GetFullPath(System.IO.Path.Combine(currentDirectory, WeatherZipDebug));
#endif

#if !DEBUG
                var weatherReportZipLocation =
                    System.IO.Path.GetFullPath(System.IO.Path.Combine(currentDirectory, WeatherZipRelease));
#endif

                var zipExists = System.IO.File.Exists(weatherReportZipLocation);

                if (!zipExists)
                    throw new System.IO.FileNotFoundException("Zip is missing from the folder FIWAREHub.Datasets");

#if DEBUG
                var extractLocation = System.IO.Path.GetFullPath(System.IO.Path.Combine(currentDirectory, ExtractLocationDebug));
#endif

#if !DEBUG
                var extractLocation = System.IO.Path.GetFullPath(System.IO.Path.Combine(currentDirectory, ExtractLocationRelease));
#endif
                System.IO.Compression.ZipFile.ExtractToDirectory(weatherReportZipLocation, extractLocation);

                csvExists = System.IO.File.Exists(weatherReportCsvFullLocation);
                if (!csvExists)
                    throw new ArgumentException("CSV Not found even after extraction attempt. Possible zip corruption. Please try downloading the datasets again.");
            }

            var engine = new FileHelperEngine<DatasetWeatherEvent>();

            var weatherEvents = engine
                .ReadFile(weatherReportCsvFullLocation)
                .ToList();

            var severityScale = weatherEvents.Select(we => we.Severity).DistinctBy(s => s).ToList();

            return weatherEvents;
        }

        public IEnumerable<FiwareCombinedReport> ParseAccidentsDataset()
        {
            var currentDirectory = System.IO.Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);

            // Nullity check, although this is known not to be null
            if (string.IsNullOrWhiteSpace(currentDirectory))
                throw new System.IO.FileNotFoundException("Current Directory is null");

            // Check if csv exists
#if DEBUG
            var accidentCsvFullLocation =
                System.IO.Path.GetFullPath(System.IO.Path.Combine(currentDirectory, ExtractLocationDebug, AccidentCsv));
#endif
#if !DEBUG
            var accidentCsvFullLocation =
      System.IO.Path.GetFullPath(System.IO.Path.Combine(currentDirectory, ExtractLocationRelease, AccidentCsv));
#endif
            var csvExists = System.IO.File.Exists(accidentCsvFullLocation);

            // if csv is missing, extract it from the zip
            if (!csvExists)
            {
                // Check if zip exists
#if DEBUG
                var accidentZipFullLocation =
    System.IO.Path.GetFullPath(System.IO.Path.Combine(currentDirectory, AccidentZipDebug));
#endif
#if !DEBUG
                var accidentZipFullLocation =
                    System.IO.Path.GetFullPath(System.IO.Path.Combine(currentDirectory, AccidentZipRelease));
#endif

                var zipExists = System.IO.File.Exists(accidentZipFullLocation);

                if (!zipExists)
                    throw new System.IO.FileNotFoundException("Zip is missing from the folder FIWAREHub.Datasets");

#if DEBUG
                var extractLocation = System.IO.Path.GetFullPath(System.IO.Path.Combine(currentDirectory, ExtractLocationDebug));
#endif
#if !DEBUG
                var extractLocation = System.IO.Path.GetFullPath(System.IO.Path.Combine(currentDirectory, ExtractLocationRelease));
#endif
                System.IO.Compression.ZipFile.ExtractToDirectory(accidentZipFullLocation, extractLocation);

                csvExists = System.IO.File.Exists(accidentCsvFullLocation);
                if (!csvExists)
                    throw new ArgumentException("CSV Not found even after extraction attempt. Possible zip corruption. Please try downloading the datasets again.");
            }

            // Deserialization of statistic-friendly weather mappings
#if DEBUG
            var mappingsFileLocation = MappingsLocationDebug;
#endif

#if !DEBUG
            var mappingsFileLocation = MappingsLocationRelease;
#endif
            var mappingsFullLocation = System.IO.Path.GetFullPath(System.IO.Path.Combine(currentDirectory, mappingsFileLocation));
            var mappingExists = System.IO.File.Exists(mappingsFullLocation);

            if (!mappingExists)
                throw new System.IO.FileNotFoundException("Weather mappings json is missing.");

            var fileAsString = System.IO.File.ReadAllText(mappingsFullLocation);
            var weatherMappings = Newtonsoft.Json.JsonConvert.DeserializeObject<WeatherMappings>(fileAsString);

            // File helper engine to deserialize CSV data set
            var engine = new FileHelperEngine<DatasetAccidentReport>();

            var accidents = engine
                .ReadFile(accidentCsvFullLocation)
                .ToList();


            // Two top states with most accidents
            var accidentsOfTwoTopStates = accidents
                // Year 2017, 2018
                .Where(a => new[] { 2017, 2018 }.Contains(a.StartTime.GetValueOrDefault().Year))
                // Omits accident reports without weather condition
                .Where(a => !string.IsNullOrWhiteSpace(a.WeatherCondition))
                .GroupBy(a => a.State)
                .OrderByDescending(g => g.Count())
                .Take(2)
                .SelectMany(g => g.ToList())
                .Select((ar, idx) => new FiwareCombinedReport
                {
                    FiwareWeatherReport = new FiwareWeatherReport(ar.WeatherCondition, weatherMappings, ar, idx + 1),
                    FiwareTrafficDataReport = new FiwareTrafficReport(ar, idx + 1)
                })
                .ToList();

            return accidentsOfTwoTopStates;
        }
    }
}
