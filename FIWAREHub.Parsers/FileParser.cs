using System;
using System.Collections.Generic;
using System.Linq;
using FileHelpers;
using FIWAREHub.Parsers.Extensions;
using FIWAREHub.Parsers.Models;

namespace FIWAREHub.Parsers
{
    public class FileParser
    {
        public IEnumerable<WeatherEvent> ParseWeatherEvents()
        {
            var engine = new FileHelperEngine<WeatherEvent>();
            var weatherEvents = engine
                .ReadFile(@"C:\Users\yiannis\Desktop\Thesis\US_Weather_Events\US_WeatherEvents_2016-2019.csv")
                .ToList();

            return weatherEvents;
        }

        public IEnumerable<AccidentReport> ParseAccidents()
        {
            var engine = new FileHelperEngine<AccidentReport>();
            var accidents = engine
                .ReadFile(
                    @"C:\Users\yiannis\Desktop\Thesis\US_Accidents_Dataset_2016_2020\US_Accidents_Dataset_2016_2020.csv")
                .ToList();

            // Two top states with most accidents
            var accidentsOfTwoTopStates = accidents
                // Year 2017, 2018
                .Where(a => new[] {2017, 2018}.Contains(a.StartTime.GetValueOrDefault().Year))
                .GroupBy(a => a.State)
                .OrderByDescending(g => g.Count())
                .Take(2)
                .SelectMany(g => g.ToList())
                //
                .ToList();

            // Distinct list of weather events in 
            var weatherEvents = accidentsOfTwoTopStates.Select(a => a.WeatherCondition).ToList().DistinctBy(a => a);

            var firstAccident = accidentsOfTwoTopStates.FirstOrDefault();

            return accidentsOfTwoTopStates;
        }
    }
}
