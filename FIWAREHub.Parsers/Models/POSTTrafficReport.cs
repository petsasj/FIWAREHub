using System;
using System.Collections.Generic;
using System.Text;

namespace FIWAREHub.Parsers.Models
{
    public class POSTTrafficReport
    {
        public string Severity { get; set; }

        public DateTime? StartTime { get; set; }

        public string StartLatitude { get; set; }

        public string StartLongitude { get; set; }

        public string GeoLocation => $"{StartLatitude}, {StartLongitude}";

        public decimal? Distance { get; set; }

        public string Description { get; set; }

        public string AddressNumber { get; set; }

        public string Street { get; set; }

        public string Side { get; set; }

        public string City { get; set; }

        public string County { get; set; }

        public string State { get; set; }

        public string ZipCode { get; set; }

        public string Country { get; set; }
    }
}
