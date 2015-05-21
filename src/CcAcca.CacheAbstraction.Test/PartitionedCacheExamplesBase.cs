// Copyright (c) 2014 Christian Crowhurst.  All rights reserved.
// see LICENSE

using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;

namespace CcAcca.CacheAbstraction.Test
{
    [TestFixture]
    public abstract class PartitionedCacheExamplesBase
    {
        public ICollection<ICache> Caches { get; set; }


        #region Setup / Teardown

        [SetUp]
        public void TestInitialise()
        {
            Caches = CreateCachesWithSharedStorage();
        }

        protected abstract ICollection<ICache> CreateCachesWithSharedStorage();

        [TearDown]
        public void TestCleanup()
        {
            foreach (var cache in Caches)
            {
                cache.Flush();
            }
        }

        #endregion


        [Test]
        public void AddOrUpdate_CanAddItemsWithSameKeyToDifferentPartitions()
        {
            // given
            var cache1 = Caches.ElementAt(0);
            var cache2 = Caches.ElementAt(1);
            var cache3 = Caches.ElementAt(2);

            // when
            cache1.AddOrUpdate("key1", 1);
            cache1.AddOrUpdate("key2", 2);

            cache2.AddOrUpdate("key1", 3);
            cache2.AddOrUpdate("key2", 4);
            cache2.AddOrUpdate("key3", 5);

            // then
            Assert.That(cache1.GetData<int>("key1"), Is.EqualTo(1));
            Assert.That(cache1.GetData<int>("key2"), Is.EqualTo(2));
            Assert.That(cache2.GetData<int>("key1"), Is.EqualTo(3));
            Assert.That(cache2.GetData<int>("key2"), Is.EqualTo(4));
            Assert.That(cache2.GetData<int>("key3"), Is.EqualTo(5));

            Assert.That(cache1.Contains("key3"), Is.False);

            Assert.That(cache3.Contains("key1"), Is.False);
            Assert.That(cache3.Contains("key2"), Is.False);
            Assert.That(cache3.Contains("key3"), Is.False);
        }


        [Test]
        public void AddOrUpdate_UpdatesShouldNotAffectOtherPartition()
        {
            // given
            var cache1 = Caches.ElementAt(0);
            var cache2 = Caches.ElementAt(1);
            var cache3 = Caches.ElementAt(2);

            cache1.AddOrUpdate("key1", 1);
            cache2.AddOrUpdate("key1", 2);

            // when, then
            cache1.AddOrUpdate("key1", 10);
            Assert.That(cache1.GetData<int>("key1"), Is.EqualTo(10));
            Assert.That(cache2.GetData<int>("key1"), Is.EqualTo(2));
            Assert.That(cache3.Contains("key1"), Is.False);

            cache3.AddOrUpdate("key1", 3);
            Assert.That(cache1.GetData<int>("key1"), Is.EqualTo(10));
            Assert.That(cache2.GetData<int>("key1"), Is.EqualTo(2));
            Assert.That(cache3.GetData<int>("key1"), Is.EqualTo(3));

            cache2.AddOrUpdate("key1", 20);
            Assert.That(cache1.GetData<int>("key1"), Is.EqualTo(10));
            Assert.That(cache2.GetData<int>("key1"), Is.EqualTo(20));
            Assert.That(cache3.GetData<int>("key1"), Is.EqualTo(3));
        }

        [Test]
        public void Flush_ShouldFlushPartitionOnly()
        {
            // given
            var cache1 = Caches.ElementAt(0);
            var cache2 = Caches.ElementAt(1);

            cache1.AddOrUpdate("key1", 1);
            cache1.AddOrUpdate("key2", 2);

            cache2.AddOrUpdate("key1", 3);
            cache2.AddOrUpdate("key2", 4);

            // when
            cache1.Flush();

            // then
            Assert.That(cache1.Contains("key1"), Is.False);
            Assert.That(cache1.Contains("key2"), Is.False);
            Assert.That(cache2.Contains("key1"), Is.True);
            Assert.That(cache2.Contains("key2"), Is.True);
        }

        [Test]
        public void Remove_ShouldRemoveOnlyFromPartitionThatItemWasAdded()
        {
            // given
            var cache1 = Caches.ElementAt(0);
            var cache2 = Caches.ElementAt(1);

            cache1.AddOrUpdate("key1", 1);
            cache2.AddOrUpdate("key1", 2);

            // when, then
            cache1.Remove("key1");
            Assert.That(cache1.Contains("key1"), Is.False);
            Assert.That(cache2.Contains("key1"), Is.True);

            cache2.Remove("key1");
            Assert.That(cache2.Contains("key1"), Is.False);
        }

        [Test]
        public void Count_ShouldOnlyCountItemsInPartition()
        {
            // test only makes sense when ICache implementation supports the Count property
            if (Caches.ElementAt(0).Count == null) return;

            // given
            var cache1 = Caches.ElementAt(0);
            var cache2 = Caches.ElementAt(1);
            var cache3 = Caches.ElementAt(2);

            // when
            cache1.AddOrUpdate("key1", 1);

            cache2.AddOrUpdate("key1", 3);
            cache2.AddOrUpdate("key2", 4);
            cache2.AddOrUpdate("key3", 5);

            // then
            Assert.That(cache1.Count, Is.EqualTo(1));
            Assert.That(cache2.Count, Is.EqualTo(3));
            Assert.That(cache3.Count, Is.EqualTo(0));
            Assert.That(cache3.Contains("key3"), Is.False);
        }
    }
}