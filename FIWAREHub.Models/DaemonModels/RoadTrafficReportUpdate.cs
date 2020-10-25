using MongoDB.Bson.Serialization.Attributes;

namespace FIWAREHub.Models.DaemonModels
{
    [BsonIgnoreExtraElements]
    public class RoadTrafficReportUpdate
    {
        [BsonElement("attrs.Severity")]
        public UpdateValue Severity { get; set; }

        [BsonElement("attrs.StartTime")]
        public UpdateValue StartTime { get; set; }

        [BsonElement("attrs.Geolocation")]
        public UpdateValue GeoLocation { get; set; }

        [BsonElement("attrs.Distance")]
        public UpdateValue Distance { get; set; }

        [BsonElement("attrs.Description")]
        public UpdateValue Description { get; set; }

        [BsonElement("attrs.AddressNumber")]
        public UpdateValue AddressNumber { get; set; }

        [BsonElement("attrs.Street")]
        public UpdateValue Street { get; set; }

        [BsonElement("attrs.Side")]
        public UpdateValue Side { get; set; }

        [BsonElement("attrs.City")]
        public UpdateValue City { get; set; }

        [BsonElement("attrs.County")]
        public UpdateValue County { get; set; }

        [BsonElement("attrs.State")]
        public UpdateValue State { get; set; }

        [BsonElement("attrs.ZipCode")]
        public UpdateValue ZipCode { get; set; }

        [BsonElement("attrs.Country")]
        public UpdateValue Country { get; set; }

        [BsonElement("attrs.UID")]
        public UpdateValue UID { get; set; }
    }
}
