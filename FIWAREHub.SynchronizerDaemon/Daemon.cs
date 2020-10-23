using System;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using FIWAREHub.SynchronizerDaemon;
using FIWAREHub.SynchronizerDaemon.Models;
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
            Console.WriteLine("Starting Listener");
            Running = true;

            // or use a connection string
            var orionContext = new OrionContext();

            using (var cursor = orionContext.Entities.Watch())
            {
                try
                {
                    foreach (var change in cursor.ToEnumerable())
                    {
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

            Console.WriteLine("Exiting Listener");
        }
    }
}
