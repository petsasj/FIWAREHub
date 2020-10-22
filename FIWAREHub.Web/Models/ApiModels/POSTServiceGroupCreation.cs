using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace FIWAREHub.Web.Models.ApiModels
{
    public class POSTServiceGroupCreation
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
