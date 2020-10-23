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

            StartTime = date;
            Severity = roadTrafficUpdate.Severity?.Value;
            Country = roadTrafficUpdate.Country?.Value;
            County = roadTrafficUpdate.County?.Value;
            City = roadTrafficUpdate.City?.Value;
            Street = roadTrafficUpdate.Street?.Value;
            Side = roadTrafficUpdate.Side?.Value;
            AddressNumber = roadTrafficUpdate.AddressNumber?.Value;
            State = roadTrafficUpdate.State?.Value;
            ZipCode = roadTrafficUpdate.ZipCode?.Value;
            GeoLocation = roadTrafficUpdate.GeoLocation?.Value;
            Distance = (decimal?) roadTrafficUpdate?.Distance?.Value;
            Description = roadTrafficUpdate.Description?.Value;
        }
    }

}
