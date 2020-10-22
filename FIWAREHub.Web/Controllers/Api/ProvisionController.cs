using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using FIWAREHub.Web.Models;
using FIWAREHub.Web.Models.ApiModels;
using FIWAREHub.Web.Services;
using Microsoft.AspNetCore.Mvc;
using Attribute = FIWAREHub.Web.Models.ApiModels.Attribute;

namespace FIWAREHub.Web.Controllers.Api
{
    public class ProvisionController : ControllerBase
    {
        public async Task<IActionResult> ProvisionJsonDevices()
        {
            var serviceGroupsExist =
                await CreateServiceGroups($"{FIWAREUrls.IoTAJsonNorth}{FIWAREUrls.ServiceGroupPath}");

            if (!serviceGroupsExist)
                return Conflict("Error creating service groups");

            var entityAttributes = new List<(string name, string objectId, string type)>
            {
                ("ReportTime", "rt", "string"),
                ("WeatherEvent", "we", "string"),
                ("Severity", "sv", "string"),
                ("OriginalWeatherConditionDescription", "owcd", "string"),
                ("Temperature", "tmp", "decimal"),
                ("WindChill", "wch", "decimal"),
                ("Humidity", "hm", "decimal"),
                ("Pressure", "pr", "decimal"),
                ("Visibility", "vs", "decimal"),
                ("CloudCoverage", "cc", "string"),
                ("WindDirection", "wd", "string"),
                ("WindSpeed", "ws", "decimal"),
                ("Precipitation", "pcpt", "decimal")
            };

            var json = new POSTProvisionDevice
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
            var serviceGroupsExist =
                await CreateServiceGroups($"{FIWAREUrls.IoTUltraLightNorth}{FIWAREUrls.ServiceGroupPath}");

            if (!serviceGroupsExist)
                return Conflict("Error creating service groups");

            var entityAttributes = new List<(string name, string objectId, string type)>
            {
                ("Severity", "sv", "string"),
                ("StartTime", "sdt", "string"),
                ("Geolocation", "geo", "string"),
                ("Distance", "dis", "decimal"),
                ("Description", "desc", "string"),
                ("AddressNumber", "addrnr", "string"),
                ("Street", "str", "string"),
                ("Side", "sd", "string"),
                ("City", "ct", "string"),
                ("County", "cn", "string"),
                ("State", "st", "string"),
                ("ZipCode", "zc", "string"),
                ("Country", "c", "string")
            };

            var json = new POSTProvisionDevice
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

            var json = new POSTServiceGroupCreation
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
