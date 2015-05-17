using System;
using System.Diagnostics;
using System.IO;
using NUnit.Framework;

namespace CcAcca.CacheAbstraction.Test.Redis
{
    [SetUpFixture]
    public class RedisSetup
    {
        private static Process _process;

        [SetUp]
        public void OneTimeSetup()
        {
            StartRedis();
        }

        [TearDown]
        public void OneTimeTeardown()
        {
            _process.Kill();
            _process.WaitForExit();
            _process.Dispose();
            _process = null;
        }

        private static void StartRedis()
        {
            string toolsDir = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"..\..\..\tools");
            string redisExePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"..\..\..\src\packages\Redis-64.2.8.19\redis-server.exe");

            _process = new Process
            {
                StartInfo =
                {
                    FileName = redisExePath,
                    CreateNoWindow = true,
                    WindowStyle = ProcessWindowStyle.Hidden,
                    UseShellExecute = false,
                    WorkingDirectory = toolsDir,
                    RedirectStandardOutput = true,
                    RedirectStandardError = true,
                    Arguments = "master.conf"
                }
            };
            _process.Start();
        }
    }
}