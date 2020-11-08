using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace FIWAREHub.Models.WebModels.ViewModels
{
    public class GeoClusterViewModel
    {
        public List<Position> AccidentLocations { get; set; }

        public List<Position> ClusterLocations { get; set; }

        public int Year { get; set; }

        public int Quarter { get; set; }

        public string State { get; set; }
    }

    public class Position
    {
        public double Latitude { get; set; }

        public double Longitude { get; set; }
    }

    public class ProbabilitiesViewModel
    {
        [Display(Name = "State")]
        public List<string> States { get; set; }

        [Display(Name = "City")]
        public List<(string city, string state)> StateCities { get; set; }

        [Display(Name = "Street")]
        public List<(string street, string state)> StateStreets { get; set; }

        [Display(Name = "Weather Event")]
        public List<string> WeatherEvents { get; set; }

        [Display(Name = "Weather Severity")]
        public List<string> WeatherSeverities { get; set; }

        [Display(Name = "Match Weather Conditions in Total Count")]
        public bool MatchWeatherConditions { get; set; }
    }

    public class ProbabilitiesResultsViewModel
    {
        public List<string> Labels { get; set; }

        public List<double> Percentages { get; set; }
    }

    public class ProbabilityStatistic
    {
        public string Street { get; set; }

        public double Percentage { get; set; }

        public int Count { get; set; }
    }
}
