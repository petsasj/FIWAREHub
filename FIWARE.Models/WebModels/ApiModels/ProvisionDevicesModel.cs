using System.Collections.Generic;
using Newtonsoft.Json;

namespace FIWAREHub.Models.WebModels.ApiModels
{
    public class ProvisionDevicesModel
    {

        [JsonProperty("devices")]
        public IList<Device> Devices { get; set; }
    }

    public class Device
    {
        [JsonProperty("device_id")]
        public string DeviceId { get; set; }

        [JsonProperty("entity_name")]
        public string EntityName { get; set; }

        [JsonProperty("entity_type")]
        public string EntityType { get; set; }

        [JsonProperty("timezone")]
        public string Timezone { get; set; }

        [JsonProperty("attributes")]
        public IList<Attribute> Attributes { get; set; }

        [JsonProperty("static_attributes")]
        public IList<StaticAttribute> StaticAttributes { get; set; }
    }

    public class Attribute
    {

        [JsonProperty("object_id")]
        public string ObjectId { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("type")]
        public string Type { get; set; }
    }

    public class StaticAttribute
    {

        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("type")]
        public string Type { get; set; }

        [JsonProperty("value")]
        public string Value { get; set; }
    }
}
