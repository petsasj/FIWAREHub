using System;
using DevExpress.Xpo;
using FIWAREHub.Models.DaemonModels;

namespace FIWAREHub.Models.Sql
{

    public partial class RoadTrafficReport
    {
        public RoadTrafficReport(Session session) : base(session) { }
        public override void AfterConstruction() { base.AfterConstruction(); }


        public RoadTrafficReport(Session session, RoadTrafficReportUpdate roadTrafficUpdate) : base(session)
        {
            
            var date = DateTime.Parse(roadTrafficUpdate.StartTime?.Value, null,
                System.Globalization.DateTimeStyles.RoundtripKind);


            // Ultralight devices have no types for attributes
            // All are treaded as strings, and must be parsed for their values
            double.TryParse(roadTrafficUpdate.Distance?.Value.ToString(), out double distance);
            int.TryParse(roadTrafficUpdate?.Severity.Value.ToString(), out int severityScale);
            double.TryParse(roadTrafficUpdate?.Latitude.Value.ToString(), out double latitude);
            double.TryParse(roadTrafficUpdate?.Longitude.Value.ToString(), out double longitude);

            StartTime = date;
            Severity = severityScale;
            Country = roadTrafficUpdate.Country?.Value;
            County = roadTrafficUpdate.County?.Value;
            City = roadTrafficUpdate.City?.Value;
            Street = roadTrafficUpdate.Street?.Value;
            Side = roadTrafficUpdate.Side?.Value;
            AddressNumber = roadTrafficUpdate.AddressNumber?.Value;
            State = roadTrafficUpdate.State?.Value;
            ZipCode = roadTrafficUpdate.ZipCode?.Value;
            GeoLocation = roadTrafficUpdate.GeoLocation?.Value;
            Latitude = latitude;
            Longitude = longitude;
            Distance = distance;
            Description = roadTrafficUpdate.Description?.Value;
            UID = long.Parse(roadTrafficUpdate.UID.Value.ToString());
        }
    }

}
