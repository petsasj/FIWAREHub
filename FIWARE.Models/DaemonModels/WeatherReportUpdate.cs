using MongoDB.Bson.Serialization.Attributes;

namespace FIWAREHub.Models.DaemonModels
{
    [BsonIgnoreExtraElements]
    public class WeatherReportUpdate
    {
        [BsonElement("attrs.ReportTime")]
        public UpdateValue ReportTime { get; set; }

        [BsonElement("attrs.WeatherEvent")]
        public UpdateValue WeatherEvent { get; set; }

        [BsonElement("attrs.Severity")]
        public UpdateValue Severity { get; private set; }

        [BsonElement("attrs.OriginalWeatherConditionDescription")]
        public UpdateValue OriginalWeatherConditionDescription { get; set; }

        [BsonElement("attrs.Temperature")]
        public UpdateValue Temperature { get; set; }

        [BsonElement("attrs.WindChill")]
        public UpdateValue WindChill { get; set; }

        [BsonElement("attrs.Humidity")]
        public UpdateValue Humidity { get; set; }

        [BsonElement("attrs.Pressure")]
        public UpdateValue Pressure { get; set; }

        [BsonElement("attrs.Visibility")]
        public UpdateValue Visibility { get; set; }

        [BsonElement("attrs.CloudCoverage")]
        public UpdateValue CloudCoverage { get; set; }

        [BsonElement("attrs.WindDirection")]
        public UpdateValue WindDirection { get; set; }

        [BsonElement("attrs.WindSpeed")]
        public UpdateValue WindSpeed { get; set; }

        [BsonElement("attrs.Precipitation")]
        public UpdateValue Precipitation { get; set; }
    }

    [BsonIgnoreExtraElements]
    public class UpdateValue
    {
        [BsonElement("value")]
        public dynamic Value { get; set; }

        [BsonElement("type")]
        public string Type { get; set; }

        [BsonElement("creDate")]
        public double CreationDate { get; set; }

        [BsonElement("modDate")]
        public double ModifiedDate { get; set; }
    }
}
