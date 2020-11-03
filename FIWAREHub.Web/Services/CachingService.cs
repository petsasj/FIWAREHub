using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Accord;
using DevExpress.Xpo;
using FIWAREHub.Models.Sql;

namespace FIWAREHub.Web.Services
{
    public class CachingService
    {
        public List<string> States { get; set; }

        public List<(string city, string state)> StateCities { get; set; }

        public List<(string street, string state)> StateStreets { get; set; }

        public List<string> WeatherEvents { get; set; }

        public List<string> WeatherSeverities { get; set; }

        public async Task InitializeAsync()
        {
            var uow = new UnitOfWork();
            await PopulateStates(uow);
            await PopulateCities(uow);
            await PopulateStreets(uow);
            await PopulateWeatherEvents(uow);
            await PopulateWeatherSeverities(uow);
        }

        private async Task PopulateStates(UnitOfWork uow)
        {
            var states = await uow.Query<RoadTrafficReport>()
                .Select(rtr => rtr.State)
                .Distinct()
                .ToListAsync();

            States = states;
        }

        private async Task PopulateCities(UnitOfWork uow)
        {
            var stateCities = await uow.Query<RoadTrafficReport>()
                .Select(rtr => new {rtr.City, rtr.State})
                .Distinct()
                .ToListAsync();

            var cities = stateCities.Select(sc => new ValueTuple<string, string>(sc.City, sc.State)).ToList();

            StateCities = cities;
        }

        private async Task PopulateStreets(UnitOfWork uow)
        {
            var stateStreets = await uow.Query<RoadTrafficReport>()
                .Select(rtr => new { rtr.Street, rtr.State})
                .Distinct()
                .ToListAsync();

            var streets = stateStreets.Select(sc => new ValueTuple<string, string>(sc.Street, sc.State)).ToList();

            StateStreets = streets;
        }

        private async Task PopulateWeatherEvents(UnitOfWork uow)
        {
            var weatherEvents = await uow.Query<WeatherReport>()
                .Select(rtr => rtr.WeatherEvent)
                .Distinct()
                .ToListAsync();

            WeatherEvents = weatherEvents;
        }

        private async Task PopulateWeatherSeverities(UnitOfWork uow)
        {
            var weatherSeverities = await uow.Query<WeatherReport>()
                .Select(rtr => rtr.Severity)
                .Distinct()
                .ToListAsync();

            WeatherSeverities = weatherSeverities;
        }

    }
}