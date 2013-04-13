namespace RedisReplication
{
    using System;
    using System.ComponentModel;
    using System.Diagnostics;
    using System.IO;

    public class Program
    {
        static void Main(string[] args)
        {
            StartRedisMaster();

            Console.ReadLine();
        }

        private static void StartRedisMaster()
        {
            var redisMaster = new BackgroundWorker();

            redisMaster.DoWork += (sender, eventArgs) =>
            {
                var process = new ProcessStartInfo
                {
                    Arguments = Path.GetFullPath(@"..\..\..\..\tools\redis\redis.conf"),
                    FileName = Path.GetFullPath(@"..\..\..\..\tools\redis\redis-server.exe"),
                    WindowStyle = ProcessWindowStyle.Hidden,
                    CreateNoWindow = true
                };

                Process.Start(process).WaitForExit();
            };

            redisMaster.RunWorkerAsync();

            Console.WriteLine("Redis master iniciado");
        }
    }
}
