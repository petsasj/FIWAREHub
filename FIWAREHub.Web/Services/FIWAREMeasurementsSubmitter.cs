﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using DevExpress.Xpo;
using FIWAREHub.Models.Sql;
using FIWAREHub.Parsers;
using FIWAREHub.Web.Extensions;
using FIWAREHub.Web.Models;

namespace FIWAREHub.Web.Services
{
    public class FIWAREMeasurementsSubmitter
    {
        private Guid _uniqueId;
        // Timer Properties
        // Runs Progress Updates every 5 minutes
        private readonly TimeSpan _zero = TimeSpan.Zero;
        private readonly TimeSpan _syncingPeriod = TimeSpan.FromMinutes(5);

        /// <summary>
        /// Async Task that performs operations
        /// Reports to SQL Database for possible errors
        /// And Progress Percentage
        /// </summary>
        /// <param name="uniqueId"></param>
        /// <param name="delay"></param>
        /// <returns></returns>
        public async Task SubmitToAgentsAsyncTask(Guid uniqueId, int delay)
        {
            // Log start of procedure
            await ReportStartProcedure(uniqueId);

            // Instance of File parser 
            var fileParser = new FileParser();

            // Get predefined dataset results
            var accidentDataset = fileParser.ParseAccidentsDataset().ToList();

            // Split into chunks of 20 for easier use with weather devices
            var chunks = accidentDataset.Chunk(20);

            // Diagnostics
            var progress = 0;

            // Procedure to Perform Progress Updates
            var timer = new System.Threading.Timer(async (e) =>
            {
                await ReportProgress(accidentDataset.Count,progress);   
            }, null, _zero, _syncingPeriod);

            // Adds async Tasks to list for execution
            var taskList = new List<Task>();

            // POST measurements to IoT devices
            foreach (var chunk in chunks)
            {
                // each chunk uses its own fiware client
                using var fiwareClient = new FIWAREClient();

                // index of iteration to select correct device
                var index = 0;

                foreach (var report in chunk)
                {
                    // Adds pausing due to severe headover in node.js/async tasks line up
                    // This pause allows for built-up entities to get cleared from the async queue
                    if (progress % 25000 == 0 && progress > 0)
                    {
                        const int minutes = 1;
                        // wait 1 minute every 25000
                        const int time = 1000 * 60 * minutes;
                        Debug.WriteLine($"Progress at {progress}");
                        Debug.WriteLine($"{DateTime.UtcNow:dd-MM-yyyy HH:mm:ss}: Waiting for {minutes} minute(s)");
                        taskList = new List<Task>();
                        await Task.Delay(time);
                    }

                    try
                    {
                        // POST to JSON
                        taskList.Add(fiwareClient.SendJson(HttpMethod.Post,
                            FIWAREUrls.JsonMeasurementUrl(
                                FIWAREUrls.WeatherDeviceIds.Skip(index).FirstOrDefault()),
                            report.FiwareWeatherReport));

                        // Delay Task for operation completion
                        taskList.Add(Task.Delay(delay));

                        // POST to UL
                        taskList.Add(fiwareClient.SendUltraLight(HttpMethod.Post,
                            FIWAREUrls.UltraLightMeasurementUrl(
                                FIWAREUrls.RoadTrafficDeviceIds.Skip(index).FirstOrDefault()),
                            report.FiwareTrafficDataReport.ToUltraLightSyntax()));

                        // await Task execution
                        await Task.WhenAll(taskList);
                        taskList.Clear();
                        index++;
                        progress++;
                    }
                    catch (Exception ex)
                    {
                        await ReportException(ex, report.FiwareTrafficDataReport.UID);
                        continue;
                    }
                }
            }

            // Update sync operation with end values and attempt to stop timer
            await ReportEnd(timer);
        }

        /// <summary>
        /// Creates diagnostic entity in DB to trace progress of syncing operation
        /// </summary>
        /// <param name="uniqueId"></param>
        /// <param name="totalItemCount"></param>
        /// <returns></returns>
        private async Task ReportStartProcedure(Guid uniqueId)
        {
            _uniqueId = uniqueId;

            using var uow = new UnitOfWork();

            var syncOperation = new SyncOperation(uow)
            {
                UniqueId = _uniqueId,
                IsRunning = true
            };

            await uow.CommitChangesAsync();
            uow.Dispose();
        }

        /// <summary>
        /// Updates created diagnostic entity with values
        /// </summary>
        /// <param name="currentItem"></param>
        /// <param name="percentage"></param>
        /// <returns></returns>
        private async Task ReportProgress(long totalItemsCount, long currentItem)
        {
            using var uow = new UnitOfWork();

            var syncOperation = await GetSyncOperation(uow);

            syncOperation.CurrentItem = currentItem;
            syncOperation.DateModified = DateTime.UtcNow;
            syncOperation.TotalItemCount = totalItemsCount;

            await uow.CommitChangesAsync();
            uow.Dispose();
        }

        /// <summary>
        /// Adds Diagnostic logging if exception occurs
        /// </summary>
        /// <param name="ex"></param>
        /// <returns></returns>
        private async Task ReportException(Exception ex, long uid)
        {
            using var uow = new UnitOfWork();

            var syncOperation = await GetSyncOperation(uow);

            syncOperation.SyncOperationExceptions.Add(new SyncOperationException(uow)
            {
                Message = ex.Message,
                UID = uid
            });
            syncOperation.DateModified = DateTime.UtcNow;

            await uow.CommitChangesAsync();
            uow.Dispose();
        }

        /// <summary>
        /// Reports the end of the syncing operation
        /// </summary>
        /// <returns></returns>
        private async Task ReportEnd(System.Threading.Timer timer)
        {
            using var uow = new UnitOfWork();

            var syncOperation = await GetSyncOperation(uow);

            syncOperation.DateFinished = DateTime.UtcNow;
            syncOperation.IsRunning = false;

            // Stop Threading timer
            timer.Change(System.Threading.Timeout.Infinite, System.Threading.Timeout.Infinite);
            await timer.DisposeAsync();

            await uow.CommitChangesAsync();
            uow.Dispose();
        }

        /// <summary>
        /// Gets unique operation by unique id
        /// </summary>
        /// <param name="uow"></param>
        /// <returns></returns>
        private async Task<SyncOperation> GetSyncOperation(UnitOfWork uow)
        {
            var syncOperation = await uow.Query<SyncOperation>().SingleOrDefaultAsync(so => so.UniqueId == _uniqueId);

            if (syncOperation == null)
                throw new NullReferenceException($"Sync operation with Unique Id {_uniqueId} has not been created!");

            return syncOperation;
        }
    }
}
