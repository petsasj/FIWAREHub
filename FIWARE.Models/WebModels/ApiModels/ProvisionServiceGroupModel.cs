using System.Collections.Generic;
using Newtonsoft.Json;

namespace FIWAREHub.Models.WebModels.ApiModels
{
    public class ProvisionServiceGroupModel
    {

        [JsonProperty("services")]
        public IList<Service> Services { get; set; }
    }

    public class Service
    {

        [JsonProperty("apikey")]
        public string Apikey { get; set; }

        [JsonProperty("cbroker")]
        public string Cbroker { get; set; }

        [JsonProperty("resource")]
        public string Resource { get; set; }
    }


}
