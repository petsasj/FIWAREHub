﻿using System;
using System.Collections.Generic;
using System.Linq;

namespace FIWAREHub.Models.ParserModels
{
    public class FiwareTrafficReport
    {
        // sv
        public string Severity { get; set; }
         
        // sdt
        public DateTime? StartTime { get; set; }

        public string StartLatitude { get; set; }

        public string StartLongitude { get; set; }

        // geo
        public string GeoLocation => $"{StartLatitude}, {StartLongitude}";

        // dis
        public decimal? Distance { get; set; }

        // desc
        public string Description { get; set; }

        // addrnr
        public string AddressNumber { get; set; }

        // str
        public string Street { get; set; }

        // sd
        public string Side { get; set; }

        // ct
        public string City { get; set; }

        // cn
        public string County { get; set; }

        // st
        public string State { get; set; }

        // zc
        public string ZipCode { get; set; }

        // c
        public string Country { get; set; }

        private static Dictionary<string, string> _ultraLightMappings = null;

        public string ToUltraLightSyntax()
        {
            if (_ultraLightMappings == null)
                PopulateMappings();

            var nonNullProperties = this.GetType().GetProperties()
                .Where(p => p.GetValue(this) != null && _ultraLightMappings.ContainsKey(p.Name)).ToList();

            var ulSyntax = string.Join("|", nonNullProperties.Select(nnp => $"{_ultraLightMappings[nnp.Name]}|{nnp.GetValue(this)}"));

            return ulSyntax;
        }

        private void PopulateMappings()
        {
            _ultraLightMappings = new Dictionary<string, string>
            {
                {nameof(Severity), "sv"},
                {nameof(StartTime), "sdt"},
                {nameof(GeoLocation), "geo"},
                {nameof(Distance), "dis"},
                {nameof(Description), "desc"},
                {nameof(AddressNumber), "addrnr"},
                {nameof(Street), "str"},
                {nameof(Side), "sd"},
                {nameof(City), "ct"},
                {nameof(County), "cn"},
                {nameof(State), "st"},
                {nameof(ZipCode), "zc"},
                {nameof(Country), "c"}
            };

        }
    }
}