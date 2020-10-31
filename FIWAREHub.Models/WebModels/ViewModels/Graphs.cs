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
}
