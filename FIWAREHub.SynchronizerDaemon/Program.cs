using System;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using FIWAREHub.ContextBroker;
using Microsoft.Extensions.DependencyInjection;
using NCrontab;

namespace FIWAREHub.SynchronizerDaemon
{
    public class Program
    {
        private static bool _keepWorking = true;
        private static TaskAwaiter<Task<Task>> currentTask;

        static void Main(string[] args)
        {
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
                            currentTask = Task.Factory.StartNew(async () => Daemon.ListenForMongoDBChanges(),
                                TaskCreationOptions.LongRunning).GetAwaiter();
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

        public static void Stop()
        {
            _keepWorking = false;
            if (!currentTask.IsCompleted)
                Thread.Sleep(250);
            Console.WriteLine("Stopped scheduling");
        }
    }
}
