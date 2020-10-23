using System;
using System.Collections.Generic;
using System.Text;
using MongoDB.Bson.Serialization.Attributes;

namespace FIWAREHub.Models.DaemonModels
{
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
