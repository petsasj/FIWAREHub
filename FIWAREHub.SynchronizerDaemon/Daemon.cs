using System;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using DevExpress.Xpo;
using DevExpress.Xpo.DB;
using FIWAREHub.Models.DaemonModels;
using FIWAREHub.Models.Sql;
using FIWAREHub.SynchronizerDaemon;
using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using MongoDB.Driver;

namespace FIWAREHub.ContextBroker
{
    public class Daemon
    {
        public static bool Running { get; private set; } = false;

        // 2 Lists for WeatherData and RoadTrafficData
        // that submit every once in a while to an xpo connection db

        public static async Task ListenForMongoDBChanges()
        {
            var uow = new UnitOfWork(ConnectionHelper.GetDataLayer(AutoCreateOption.DatabaseAndSchema));

            Report("Starting Listener");
            Running = true;

            // or use a connection string
            var orionContext = new OrionContext();

            using (var cursor = orionContext.Entities.Watch())
            {
                Report("Cursor Watching started");
                try
                {
                    foreach (var change in cursor.ToEnumerable())
                    {
                        Report("Cursor Watch received update");
                        // Do Stuff
                        // Find way to distinguish between weather and roadtraffic data 
                        var weatherUpdates = BsonSerializer.Deserialize<WeatherReportUpdate>(change.UpdateDescription.UpdatedFields);

                        
                    }

                }
                catch (Exception ex)
                {
                    throw new Exception(ex.Message);
                }

                Running = false;
            }

            Report("Exiting Listener");
        }

        private static void Report(string message) => Console.WriteLine($"{DateTime.UtcNow:dd-MM hh:mm:ss}: {message}");
    }
}
