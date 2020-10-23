using System;
using DevExpress.Xpo;
using FIWAREHub.Models.DaemonModels;

namespace FIWAREHub.Models.Sql
{

    public partial class WeatherReport
    {
        public WeatherReport(Session session) : base(session) { }
        public override void AfterConstruction() { base.AfterConstruction(); }

        public WeatherReport(Session session, WeatherReportUpdate weatherUpdate) : base(session)
        {
            var date = DateTime.Parse(weatherUpdate.ReportTime?.Value, null,
                System.Globalization.DateTimeStyles.RoundtripKind);

            Severity = weatherUpdate.Severity?.Value;
            CloudCoverage = weatherUpdate.CloudCoverage?.Value;
            OriginalWeatherDescription = weatherUpdate.OriginalWeatherConditionDescription?.Value;
            Humidity = (decimal?) weatherUpdate.Humidity?.Value;
            Precipitation = (decimal?) weatherUpdate.Precipitation?.Value;
            Pressure = (decimal?) weatherUpdate.Pressure?.Value;
            ReportTime = date;
            Visibility = (decimal?) weatherUpdate.Visibility?.Value;
            WindDirection = weatherUpdate.WindDirection?.Value;
            WindChill = (decimal?) weatherUpdate.WindChill?.Value;
            WindSpeed = (decimal?) weatherUpdate.WindSpeed?.Value;
            Temperature = (decimal?) weatherUpdate.Temperature?.Value;
            WeatherEvent = weatherUpdate.WeatherEvent?.Value;
        }
    }

}
