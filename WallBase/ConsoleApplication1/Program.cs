using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Hangfire;
using Hangfire.SqlServer;

namespace ConsoleApplication1
{
    class Program
    {
    public static void Main()
        {
          
            var sqlServerStorage = new SqlServerStorage(
               "DefaultConnection");

            JobStorage.Current = sqlServerStorage;

            RecurringJob.AddOrUpdate(() => Console.WriteLine("Hello, world!"), Cron.Minutely);
            RecurringJob.AddOrUpdate("hourly", () => Console.WriteLine("Hello"), "25 15 * * *");

            var options = new BackgroundJobServerOptions
            {
                Queues = new[] { "critical", "default" }
            };

            using (var server = new BackgroundJobServer(options))
            {
                var count = 1;

                while (true)
                {
                    var command = Console.ReadLine();

                    if (command == null || command.Equals("stop", StringComparison.OrdinalIgnoreCase))
                    {
                        break;
                    }

                    if (command.Equals("start", StringComparison.OrdinalIgnoreCase))
                    {
                        server.Start();
                    }

                    if (command.StartsWith("add", StringComparison.OrdinalIgnoreCase))
                    {
                        try
                        {
                            var workCount = int.Parse(command.Substring(4));
                            for (var i = 0; i < workCount; i++)
                            {
                                var number = i;
                                BackgroundJob.Enqueue<Services>(x => x.Random(number));
                            }
                            Console.WriteLine("Jobs enqueued.");
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine(ex.Message);
                        }
                    }

                    if (command.StartsWith("static", StringComparison.OrdinalIgnoreCase))
                    {
                        try
                        {
                            var workCount = int.Parse(command.Substring(7));
                            for (var i = 0; i < workCount; i++)
                            {
                                BackgroundJob.Enqueue(() => Console.WriteLine("Hello, {0}!", "world"));
                            }
                            Console.WriteLine("Jobs enqueued.");
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine(ex.Message);
                        }
                    }

                    if (command.StartsWith("error", StringComparison.OrdinalIgnoreCase))
                    {
                        var workCount = int.Parse(command.Substring(6));
                        for (var i = 0; i < workCount; i++)
                        {
                            BackgroundJob.Enqueue<Services>(x => x.Error());
                        }
                    }

                    if (command.StartsWith("args", StringComparison.OrdinalIgnoreCase))
                    {
                        var workCount = int.Parse(command.Substring(5));
                        for (var i = 0; i < workCount; i++)
                        {
                            BackgroundJob.Enqueue<Services>(x => x.Args(Guid.NewGuid().ToString(), 14442, DateTime.UtcNow));
                        }
                    }

                    if (command.StartsWith("custom", StringComparison.OrdinalIgnoreCase))
                    {
                        var workCount = int.Parse(command.Substring(7));
                        for (var i = 0; i < workCount; i++)
                        {
                            BackgroundJob.Enqueue<Services>(x => x.Custom(
                                new Random().Next(),
                                new []{ "Hello", "world!" },
                                new Services.CustomObject { Id = 123 },
                                DayOfWeek.Friday
                                ));
                        }
                    }

                    if (command.StartsWith("fullargs", StringComparison.OrdinalIgnoreCase))
                    {
                        var workCount = int.Parse(command.Substring(9));
                        for (var i = 0; i < workCount; i++)
                        {
                            BackgroundJob.Enqueue<Services>(x => x.FullArgs(
                                false,
                                123,
                                'c',
                                DayOfWeek.Monday,
                                "hello",
                                new TimeSpan(12, 13, 14),
                                new DateTime(2012, 11, 10),
                                new Services.CustomObject { Id = 123 },
                                new[] { "1", "2", "3" },
                                new[] { 4, 5, 6 },
                                new long[0],
                                null,
                                new List<string> { "7", "8", "9" }));
                        }
                    }

                    if (command.StartsWith("in", StringComparison.OrdinalIgnoreCase))
                    {
                        var seconds = int.Parse(command.Substring(2));
                        var number = count++;
                        BackgroundJob.Schedule<Services>(x => x.Random(number), TimeSpan.FromSeconds(seconds));
                    }

                    if (command.StartsWith("cancelable", StringComparison.OrdinalIgnoreCase))
                    {
                        var iterations = int.Parse(command.Substring(11));
                        BackgroundJob.Enqueue<Services>(x => x.Cancelable(iterations, JobCancellationToken.Null));
                    }

                    if (command.StartsWith("delete", StringComparison.OrdinalIgnoreCase))
                    {
                        var workCount = int.Parse(command.Substring(7));
                        for (var i = 0; i < workCount; i++)
                        {
                            var jobId = BackgroundJob.Enqueue<Services>(x => x.EmptyDefault());
                            BackgroundJob.Delete(jobId);
                        }
                    }

                    if (command.StartsWith("fast", StringComparison.OrdinalIgnoreCase))
                    {
                        try
                        {
                            var workCount = int.Parse(command.Substring(5));
                            Parallel.For(0, workCount, i =>
                            {
                                if (i % 2 == 0)
                                {
                                    BackgroundJob.Enqueue<Services>(x => x.EmptyCritical());
                                }
                                else
                                {
                                    BackgroundJob.Enqueue<Services>(x => x.EmptyDefault());
                                }
                            });
                            Console.WriteLine("Jobs enqueued.");
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine(ex.Message);
                        }
                    }

                    if (command.StartsWith("generic", StringComparison.OrdinalIgnoreCase))
                    {
                        BackgroundJob.Enqueue<GenericServices<string>>(x => x.Method("hello", 1));
                    }
                }
            }

            Console.WriteLine("Press Enter to exit...");
            Console.ReadLine();
        }
    }
    }
  public class GenericServices<TType>
    {
        public void Method<TMethod>(TType arg1, TMethod arg2)
        {
            Console.WriteLine("Arg1: {0}, Arg2: {1}", arg1, arg2);
        }
    }
 public class Services
    {
        private static readonly Random _random = new Random();

        public void EmptyDefault()
        {
        }

        [Queue("critical")]
        public void EmptyCritical()
        {
        }

        [AutomaticRetry(Attempts = 0)]
        public void Error()
        {
            Console.WriteLine("Beginning error task...");
            throw new InvalidOperationException(null, new FileLoadException());
        }

        [Queue("critical")]
        public void Random(int number)
        {
            int time;
            lock (_random)
            {
                time = _random.Next(10);
            }

            if (time < 5)
            {
                throw new Exception();
            }

            Thread.Sleep(TimeSpan.FromSeconds(5 + time));
            Console.WriteLine("Finished task: " + number);
        }

        public void Cancelable(int iterationCount, IJobCancellationToken token)
        {
            try
            {
                for (var i = 1; i <= iterationCount; i++)
                {
                    Thread.Sleep(1000);
                    Console.WriteLine("Performing step {0} of {1}...", i, iterationCount);

                    token.ThrowIfCancellationRequested();
                }
            }
            catch (OperationCanceledException)
            {
                Console.WriteLine("Cancellation requested, exiting...");
                throw;
            }
        }

        [DisplayName("Name: {0}")]
        public void Args(string name, int authorId, DateTime createdAt)
        {
            Console.WriteLine("{0}, {1}, {2}", name, authorId, createdAt);
        }

        public void Custom(int id, string[] values, CustomObject objects, DayOfWeek dayOfWeek)
        {
        }

        public void FullArgs(
            bool b,
            int i,
            char c,
            DayOfWeek e,
            string s,
            TimeSpan t,
            DateTime d,
            CustomObject o,
            string[] sa,
            int[] ia,
            long[] ea,
            object[] na,
            List<string> sl)
        {
        }

        public class CustomObject
        {
            public int Id { get; set; }
            public CustomObject[] Children { get; set; }
        }
    }
