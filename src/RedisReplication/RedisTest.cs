namespace RedisReplication
{
    using System.Diagnostics;
    using System.IO;
    using System.Threading;
    using BookSleeve;
    using NBehave.Spec.NUnit;
    using NUnit.Framework;

    [TestFixture]
    public class RedisTest
    {
        private Process redisMaster;
        private Process redisSlave;

        [SetUp]
        public void Setup()
        {
            this.redisMaster = this.StartRedis("redis.conf");
            this.redisSlave = this.StartRedis("redis-replica.conf");
        }

        [TearDown]
        public void Teardown()
        {
            if (this.redisMaster.HasExited == false) this.redisMaster.Kill();
            if (this.redisSlave.HasExited == false) this.redisSlave.Kill();
        }

        [Test]
        public void Test()
        {
            Debug.WriteLine("Iniciando");

            using (var conn = new RedisConnection("localhost"))
            {
                conn.Open();
                conn.Strings.Set(0, "nome", "joão");
            }

            Thread.Sleep(2000);

            using (var conn = new RedisConnection("localhost", 6380))
            {
                conn.Open();
                conn.Strings.GetString(0, "nome").Result.ShouldEqual("joão");
            }

            this.redisMaster.Kill();

            Thread.Sleep(2000);

            using (var conn = new RedisConnection("localhost", 6380))
            {
                conn.Open();
                conn.Strings.GetString(0, "nome").Result.ShouldEqual("joão");

                conn.Strings.Set(0, "idade", "27");
                conn.Strings.GetString(0, "idade").Result.ShouldEqual("27");
            }

            Debug.WriteLine("Fim");
        }

        private Process StartRedis(string conf)
        {
            return Process.Start(new ProcessStartInfo
            {
                Arguments = conf,
                FileName = Path.GetFullPath(@"..\..\..\..\tools\redis\redis-server.exe"),
                WorkingDirectory = Path.GetFullPath(@"..\..\..\..\tools\redis"),
                WindowStyle = ProcessWindowStyle.Hidden,
                CreateNoWindow = true,
                RedirectStandardOutput = true,
                UseShellExecute = false,
            });
        }
    }
}
