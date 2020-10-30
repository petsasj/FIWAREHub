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
            Humidity = (double?) weatherUpdate.Humidity?.Value;
            Precipitation = (double?) weatherUpdate.Precipitation?.Value;
            Pressure = (double?) weatherUpdate.Pressure?.Value;
            ReportTime = date;
            Visibility = (double?) weatherUpdate.Visibility?.Value;
            WindDirection = weatherUpdate.WindDirection?.Value;
            WindChill = (double?) weatherUpdate.WindChill?.Value;
            WindSpeed = (double?) weatherUpdate.WindSpeed?.Value;
            Temperature = (double?) weatherUpdate.Temperature?.Value;
            WeatherEvent = weatherUpdate.WeatherEvent?.Value;
            Country = weatherUpdate.Country?.Value;
            County = weatherUpdate.County?.Value;
            City = weatherUpdate.City?.Value;
            State = weatherUpdate.State?.Value;
            ZipCode = weatherUpdate.ZipCode?.Value;
            UID = long.Parse(weatherUpdate.UID.Value.ToString());
        }
    }

}
