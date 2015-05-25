using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using CcAcca.CacheAbstraction.DemoWeb.Models;
using Newtonsoft.Json;

namespace CcAcca.CacheAbstraction.DemoWeb.Repositories
{
    public class UserRepository
    {
        public static string TestUserJsonFile { get; set; }

        public UserRepository()
        {
            FakeDelay = 5000;
            Db = JsonConvert.DeserializeObject<List<User>>(File.ReadAllText(TestUserJsonFile));
        }

        /// <summary>
        ///     The number of milliseconds to delay the results returned by each method
        /// </summary>
        /// <remarks>
        ///     Simulates network latency
        /// </remarks>
        public int FakeDelay { get; set; }

        private static ICollection<User> Db { get; set; }

        public async Task<ICollection<UserBrief>> GetAllAsync()
        {
            await Delay();
            return Db.Select(u => new UserBrief { Avatar = u.Avatar, Name = u.Name}).ToList();
        }

        public async Task<User> GetOneByNameAsync(string name)
        {
            await Delay();
            return Db.FirstOrDefault(u => u.Name == name);
        }

        private Task Delay()
        {
            return Task.Delay(TimeSpan.FromMilliseconds(FakeDelay));
        }
    }
}