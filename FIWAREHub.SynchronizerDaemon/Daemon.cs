using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using DevExpress.Xpo;
using DevExpress.Xpo.DB;
using DevExpress.Xpo.Metadata;
using FIWAREHub.Models.DaemonModels;
using FIWAREHub.Models.Enums;
using FIWAREHub.Models.Sql;
using MongoDB.Bson.Serialization;
using MongoDB.Driver;

namespace FIWAREHub.SynchronizerDaemon
{
    public static class Daemon
    {
        // Informs whether service is running or not
        public static bool Running { get; private set; }

        // Timer Properties
        // Runs SQL Syncing Task every SyncingPeriod
        private static readonly TimeSpan Zero = TimeSpan.Zero;
        private static readonly TimeSpan SyncingPeriod = TimeSpan.FromMinutes(1);

        // Lock for thread-safety during SQL saving
        private static volatile bool _unitOfWorkLock;
        private static DateTime _dateUnitOfWorkInstantiated;
        // Unit of work for SQL Operations
        private static UnitOfWork _unitOfWork;

        // 2 Thread-Safe Collections for WeatherData and RoadTrafficData
        // that submit every minute
        public static ConcurrentQueue<WeatherReport> WeatherReports { get; } = new ConcurrentQueue<WeatherReport>();
        public static volatile int WeatherChangesReceived;
        public static ConcurrentQueue<RoadTrafficReport> RoadTrafficReports { get; } = new ConcurrentQueue<RoadTrafficReport>();
        public static volatile int RoadTrafficChangesReceived;

        public static async Task ListenForMongoDbChanges()
        {
            Report("Starting Listener");
            Running = true;

            // Task that runs SQL Syncing
            var timer = new System.Threading.Timer(async (e) =>
            {
                await SqlSyncing();   
            }, null, Zero, SyncingPeriod);


            // Helper class for connection to MongoDb
            var orionContext = new OrionContext();

            // Change Stream API of MongoDB (based on optlog)
            // This is an async task and creates a thread for each change
            using (var cursor = await orionContext.Entities.WatchAsync())
            {
                Report("Cursor Watching started");
                try
                {
                    foreach (var change in cursor.ToEnumerable())
                    {
                        Report("Cursor Watch received update");

                        // Getting id of device in safe manner
                        // Helps determine entity of receiving object
                        if (!(BsonSerializer.Deserialize<dynamic>(change.DocumentKey) is IDictionary<string, object> documentKey))
                            continue;

                        var hasId = documentKey.TryGetValue("_id", out var _id);
                        if (!hasId)
                            continue;
                        
                        if (!(_id is IDictionary<string, object> idRow))
                            continue;

                        var hasEntityType = idRow.TryGetValue("type", out var entityType);
                        if (!hasEntityType)
                            continue;

                        // Turns out this enum doesn't do all that much
                        EntityTypeEnum entityTypeEnum;
                        switch (entityType)
                        {
                            case "weatherReport":
                                entityTypeEnum = EntityTypeEnum.WeatherReport;
                                // Interlocked ensures atomicity of each incrementation
                                Interlocked.Increment(ref WeatherChangesReceived);
                                break;
                            case "roadTrafficReport":
                                entityTypeEnum = EntityTypeEnum.RoadTrafficReport;
                                // Interlocked ensures atomicity of each incrementation
                                Interlocked.Increment(ref RoadTrafficChangesReceived);
                                break;
                            default:
                                continue;
                        }

                        idRow.TryGetValue("id", out var deviceId);

                        // creates instace of DTO Object and adds to ConcurrentQueue
                        if (entityTypeEnum == EntityTypeEnum.WeatherReport)
                        {
                            Report($"Received Weather Report from device {deviceId}");
                            
                            // Deserialize to Typed object
                            var weatherUpdate = BsonSerializer.Deserialize<WeatherReportUpdate>(change.UpdateDescription.UpdatedFields);

                            if (weatherUpdate == null)
                                continue;

                            var uow = await getUnitOfWorkAsync();

                            // Delay to ensure nothing gets added to lists while SQL Save Operation is happening
                            while (_unitOfWorkLock || _unitOfWork?.IsObjectsSaving == true|| uow.IsObjectsSaving)
                            {
                                Report("Lists are locked, waiting");
                                await Task.Delay(50);
                            }

                            var weatherReport = new WeatherReport(uow, weatherUpdate) {DeviceId = deviceId?.ToString()};

                            WeatherReports.Enqueue(weatherReport);
                        }
                        else if (entityTypeEnum == EntityTypeEnum.RoadTrafficReport)
                        {
                            Report($"Received Road Traffic Report from device {deviceId}");

                            // Deserialize to Typed object
                            var roadTrafficUpdate = BsonSerializer.Deserialize<RoadTrafficReportUpdate>(change.UpdateDescription.UpdatedFields);

                            if (roadTrafficUpdate == null)
                                continue;

                            var uow = await getUnitOfWorkAsync();

                            // Delay to ensure nothing gets added to lists while SQL Save Operation is happening
                            while (_unitOfWorkLock || _unitOfWork?.IsObjectsSaving == true|| uow.IsObjectsSaving)
                            {
                                Report("Lists are locked, waiting");
                                await Task.Delay(50);
                            }

                            var roadTrafficReport = new RoadTrafficReport(uow, roadTrafficUpdate) {DeviceId = deviceId?.ToString()};

                            RoadTrafficReports.Enqueue(roadTrafficReport);
                        }
                    }
                }
                catch (Exception ex)
                {
                    Running = false;
                    Report($"EXCEPTION: {ex.Message}");
                    throw new Exception(ex.Message);
                }

                Running = false;
            }

            Report("Exiting Listener");
        }

        private static void Report(string message) => Console.WriteLine($"{DateTime.UtcNow:dd-MM hh:mm:ss}: {message}");

        /// <summary>
        /// Creation of ThreadSafe unit of work
        /// </summary>
        /// <returns></returns>
        private static async Task<UnitOfWork> getUnitOfWorkAsync(bool overRideLock = false)
        {
            // Delay to make sure no operations violate atomicity
            while (_unitOfWorkLock && !overRideLock)
            {
                await Task.Delay(50);
            }

            // Caching of unit of work
            if (_unitOfWork != null && _unitOfWork.IsConnected)
                return _unitOfWork;

            // Sets up thread safe datalayer all units of work
            // All units of work with default datalayer are thread safe
            var dictionary = PrepareDictionary();
            static XPDictionary PrepareDictionary()
            {
                var dict = new ReflectionDictionary();
                dict.GetDataStoreSchema(ConnectionHelper.GetPersistentTypes());
                return dict;
            }

            IDataStore store = XpoDefault.GetConnectionProvider(
                XpoDefault.GetConnectionPoolString(ConnectionHelper.ConnectionString, 5, 100),
                AutoCreateOption.DatabaseAndSchema);
            XpoDefault.DataLayer = new ThreadSafeDataLayer(dictionary, store);

            _unitOfWork = new UnitOfWork();
            _dateUnitOfWorkInstantiated = DateTime.UtcNow;

            return _unitOfWork;
        }

        private static async Task SqlSyncing()
        {
            Report("Locking Unit of Work");
            // Locking lists to prevent addition while SQL Saving
            _unitOfWorkLock = true;

            Report("SQL Syncing Started");

            if (WeatherReports.Any() || RoadTrafficReports.Any())
            {
                // Diagnostic stopwatch to inform elapsed time for update
                var stopwatch = new System.Diagnostics.Stopwatch();
                stopwatch.Start();

                var uow = await getUnitOfWorkAsync(true);

                // SQL Save
                Report("Saving items in SQL");
                await uow.CommitChangesAsync();

                stopwatch.Stop();
                var elapsedTime = stopwatch.ElapsedMilliseconds;
                var totalItemCount = WeatherReports.Count + RoadTrafficReports.Count;

                Report($"Database update took {elapsedTime}ms for {totalItemCount} items.");

                // Renews lists
                Report("Clearing Lists");
                WeatherReports.Clear();
                RoadTrafficReports.Clear();

                Report("Unlocking Unit of Work");
                _unitOfWorkLock = false;

                // Refresh unit of work due to large dataset
                // Clears Session Cache
                _unitOfWork = null;
            }
            else
            {
                Report("No SQL items for saving");
                _unitOfWorkLock = false;
            }

        }
    }
}
