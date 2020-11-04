using System;
using System.Collections.Generic;
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
        public List<string> States { get; set; }

        public List<(string city, string state)> StateCities { get; set; }

        public List<(string street, string state)> StateStreets { get; set; }

        public List<string> WeatherEvents { get; set; }

        public List<string> WeatherSeverities { get; set; }
    }
}
