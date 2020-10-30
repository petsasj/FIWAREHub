using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using FIWAREHub.Models.WebModels.ApiModels;
using FIWAREHub.Web.Models;
using FIWAREHub.Web.Services;
using Microsoft.AspNetCore.Mvc;
using Attribute = FIWAREHub.Models.WebModels.ApiModels.Attribute;

namespace FIWAREHub.Web.Controllers.Api
{
    public class ProvisionController : ControllerBase
    {
        public async Task<IActionResult> ProvisionJsonDevices()
        {
            // Helper method to verify that services groups are created
            var serviceGroupsExist =
                await CreateServiceGroups($"{FIWAREUrls.IoTAJsonNorth}{FIWAREUrls.ServiceGroupPath}");

            if (!serviceGroupsExist)
                return Conflict("Error creating service groups");

            // Entity attributes for given IoT Agent
            var entityAttributes = new List<(string name, string objectId, string type)>
            {
                ("ReportTime", "rt", "string"),
                ("WeatherEvent", "we", "string"),
                ("Severity", "sv", "string"),
                ("OriginalWeatherConditionDescription", "owcd", "string"),
                ("Temperature", "tmp", "double"),
                ("WindChill", "wch", "double"),
                ("Humidity", "hm", "double"),
                ("Pressure", "pr", "double"),
                ("Visibility", "vs", "double"),
                ("CloudCoverage", "cc", "string"),
                ("WindDirection", "wd", "string"),
                ("WindSpeed", "ws", "double"),
                ("Precipitation", "pcpt", "double"),
                ("City", "ct", "string"),
                ("County", "cn", "string"),
                ("State", "st", "string"),
                ("ZipCode", "zc", "string"),
                ("Country", "c", "string"),
                ("UID", "uid", "int")
            };

            // POST Model for Provision of multiple devices
            var json = new ProvisionDevicesModel
            {
                Devices = FIWAREUrls.WeatherDeviceIds.Select(id => new Device
                {
                    DeviceId = id,
                    EntityName = $"urn:ngsi-ld:{id}",
                    EntityType = "weatherReport",
                    Attributes = entityAttributes.Select(ea => new Attribute
                    {
                        Name = ea.name,
                        ObjectId = ea.objectId,
                        Type = ea.type
                    }).ToList()
                }).ToList()
                    
            };

            using var fiwareClient = new FIWAREClient();
            var response = await fiwareClient.SendJson(HttpMethod.Post, $"{FIWAREUrls.IoTAJsonNorth}{FIWAREUrls.DeviceProvisionPath}", json);
            var success = response.IsSuccessStatusCode;
            var returnMessage = success
                ? "Successfully provisioned JSON Devices"
                : $"There was an error during the execution of {nameof(ProvisionJsonDevices)}";

            if (!success)
                return BadRequest(returnMessage);

            return Ok(returnMessage);
        }

        public async Task<IActionResult> ProvisionUltraLightDevices()
        {
            // Helper method to verify that services groups are created
            var serviceGroupsExist =
                await CreateServiceGroups($"{FIWAREUrls.IoTUltraLightNorth}{FIWAREUrls.ServiceGroupPath}");

            if (!serviceGroupsExist)
                return Conflict("Error creating service groups");

            // Entity attributes for given IoT Agent
            var entityAttributes = new List<(string name, string objectId, string type)>
            {
                ("Severity", "sv", "int"),
                ("StartTime", "sdt", "string"),
                ("Latitude", "lat", "double"),
                ("Longitude", "lon", "double"),
                ("Geolocation", "geo", "string"),
                ("Distance", "dis", "double"),
                ("Description", "desc", "string"),
                ("AddressNumber", "addrnr", "string"),
                ("Street", "str", "string"),
                ("Side", "sd", "string"),
                ("City", "ct", "string"),
                ("County", "cn", "string"),
                ("State", "st", "string"),
                ("ZipCode", "zc", "string"),
                ("Country", "c", "string"),
                ("UID", "uid", "int")
            };

            // POST Model for Provision of multiple devices
            var json = new ProvisionDevicesModel
            {
                Devices = FIWAREUrls.RoadTrafficDeviceIds.Select(id => new Device
                {
                    DeviceId = id,
                    EntityName = $"urn:ngsi-ld:{id}",
                    EntityType = "roadTrafficReport",
                    Attributes = entityAttributes.Select(ea => new Attribute
                    {
                        Name = ea.name,
                        ObjectId = ea.objectId,
                        Type = ea.type
                    }).ToList()
                }).ToList()
                    
            };

            using var fiwareClient = new FIWAREClient();
            var response = await fiwareClient.SendJson(HttpMethod.Post, $"{FIWAREUrls.IoTUltraLightNorth}{FIWAREUrls.DeviceProvisionPath}", json);
            var success = response.IsSuccessStatusCode;
            var returnMessage = success
                ? "Successfully provisioned UltraLight Devices"
                : $"There was an error during the execution of {nameof(ProvisionUltraLightDevices)}";

            if (!success)
                return BadRequest(returnMessage);

            return Ok(returnMessage);
        }

        private async Task<bool> CreateServiceGroups(string url)
        {
            var jsonGroup = new Service
            {
                Apikey = FIWAREUrls.JsonKey,
                Resource = FIWAREUrls.JsonServiceGroupResource,
                Cbroker = FIWAREUrls.OrionUrl
            };

            var ulGroup = new Service
            {
                Apikey = FIWAREUrls.UltraLightKey,
                Resource = FIWAREUrls.UltraLightServiceGroupResource,
                Cbroker = FIWAREUrls.OrionUrl
            };

            var json = new ProvisionServiceGroupModel
            {
                Services = new List<Service>
                {
                    jsonGroup, 
                    ulGroup
                }
            };

            using var fiwareClient = new FIWAREClient();
            var response = await fiwareClient.SendJson(HttpMethod.Post, url, json);

            return response.IsSuccessStatusCode || response.StatusCode == HttpStatusCode.Conflict;
        }
    }
}
