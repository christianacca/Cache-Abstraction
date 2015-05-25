// Copyright (c) 2014 Christian Crowhurst.  All rights reserved.
// see LICENSE

using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
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
            _process.WaitForExit(5000);
            _process.Dispose();
            _process = null;
        }

        private static void StartRedis()
        {
            CleanupOrphanedRedisProcesses();

            string toolsDir = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"..\..\..\tools");
            string redisExePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory,
                @"..\..\..\packages\Redis-64.2.8.19\redis-server.exe");

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

        private static void CleanupOrphanedRedisProcesses()
        {
            var orphanedRedisProcesses = Process.GetProcessesByName("redis-server").OrderBy(p => p.ProcessName).ToList();
            foreach (var orphan in orphanedRedisProcesses)
            {
                orphan.Kill();
            }
            if (orphanedRedisProcesses.Any())
            {
                Assert.Inconclusive(
                    "Existing redis-server processes found that had to be killed off. Test run aborted, please try running the Redis test suite again");
            }
        }
    }
}