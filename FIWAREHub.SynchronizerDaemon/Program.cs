using System;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using FIWAREHub.ContextBroker;
using NCrontab;

namespace FIWAREHub.SynchronizerDaemon
{
    public class Program
    {
        private static bool _keepWorking = true;
        private static TaskAwaiter<Task<Task>> currentTask;

        static void Main(string[] args)
        {
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
                    Daemon.ListenForMongoDBChanges().GetAwaiter().GetResult();

                    //if (DateTime.UtcNow > cron.GetNextOccurrence(lastCheck))
                    //{
                    //    lastCheck = DateTime.UtcNow;
                    //    //currentTask = Task.Factory.StartNew(async () => ScheduleAsync(await ServerTime()),TaskCreationOptions.LongRunning).GetAwaiter();
                    //    currentTask = Task.Factory.StartNew(async () => Daemon.ListenForMongoDBChanges(),
                    //        TaskCreationOptions.LongRunning).GetAwaiter();
                    //}
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
