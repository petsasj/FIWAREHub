﻿using System;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using NCrontab;

namespace FIWAREHub.SynchronizerDaemon
{
    public class Program
    {
        private static bool _keepWorking = true;
        private static Task<Task> _currentTask;

        static void Main(string[] args)
        {
            // Adding Dependency Injection if needed.
            //var serviceProvider = new ServiceCollection()
            //    .AddLogging()
            //    .AddSingleton<Daemon>()
            //    .BuildServiceProvider();

            Start();
            
            var command = "";
            while (!(command?.ToLower() == "exit" || command?.ToLower() == "quit"))
            {
                command = Console.ReadLine();
            }

            Stop();
        }

        /// <summary>
        /// Runs Daemon Synchronizer tool
        /// Checks every minute if it has exited
        /// </summary>
        public static void Start()
        {
            var cron = CrontabSchedule.Parse("* * * * *");
            var lastCheck = DateTime.UtcNow.AddYears(-1);

            while (_keepWorking)
            {
                try
                {

                    if (DateTime.UtcNow > cron.GetNextOccurrence(lastCheck))
                    {
                        lastCheck = DateTime.UtcNow;

                        if (!Daemon.Running)
                        {
                            _currentTask = Task.Factory.StartNew(async () => await Daemon.ListenForMongoDbChanges(),
                                TaskCreationOptions.LongRunning);
                        }
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }
                finally
                {
                    Thread.Sleep(1000);
                }
            }
        }

        /// <summary>
        /// Stop
        /// </summary>
        public static void Stop()
        {
            _keepWorking = false;
            if (!_currentTask.IsCompleted)
                Thread.Sleep(250);
            Console.WriteLine("Stopped scheduling");
        }
    }
}
