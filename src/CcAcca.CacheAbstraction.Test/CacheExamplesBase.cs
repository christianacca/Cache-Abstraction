// Copyright (c) 2014 Christian Crowhurst.  All rights reserved.
// see LICENSE

using System;
using System.Threading.Tasks;
using NUnit.Framework;
// ReSharper disable ExpressionIsAlwaysNull

namespace CcAcca.CacheAbstraction.Test
{
    /// <summary>
    /// Defines and asserts the detailed behaviour / contract that a <see cref="ICache"/> implementation is
    /// expected to implement
    /// </summary>
    /// <remarks>
    /// When writing a new <see cref="ICache"/> implementation, you should create a test class that inherits from 
    /// this class. Doing so ensures that your <see cref="ICache"/> implementation is checked for compliance to 
    /// the <see cref="ICache"/> contract
    /// </remarks>
    public abstract class CacheExamplesBase
    {
        #region Member Variables

        private ICache _cache;

        #endregion


        #region Properties

        public ICache Cache
        {
            get { return _cache; }
        }

        #endregion


        #region Setup / Teardown

        [SetUp]
        public void TestInitialise()
        {
            _cache = CreateCache();
        }

        [TearDown]
        public void TestCleanup()
        {
            if (_cache != null)
            {
                _cache.Flush();
            }
            _cache = null;
        }

        #endregion


        [Test]
        public void AddOrUpdate_Should_Add()
        {
            Cache.AddOrUpdate("key", 5);

            Assert.That(Cache.GetData<int>("key"), Is.EqualTo(5));
            Assert.That(Cache.Contains("key"), Is.True);
        }


        [Test]
        public void AddOrUpdate_ShouldUpdateExistingItem()
        {
            Cache.AddOrUpdate("key", 5);
            Cache.AddOrUpdate("key", 6);

            Assert.That(Cache.GetData<int>("key"), Is.EqualTo(6));
        }


        [Test]
        public void AddOrUpdate_UpdateShouldThrowWhenNullValueSupplied()
        {
            Cache.AddOrUpdate("key", 5);
            Assert.Throws<ArgumentNullException>(() => { Cache.AddOrUpdate<object>("key", null); });
        }


        [Test]
        public void AddOrUpdate_AddShouldThrowWhenNullSupplied()
        {
            Assert.Throws<ArgumentNullException>(() => { Cache.AddOrUpdate<object>("whatever", null); });
        }

        [Test]
        public void AddOrUpdateFactory_UpdateShouldThrowWhenNullValueSupplied()
        {
            // given
            Cache.AddOrUpdate("key", 5);

            // when, then
            Assert.Throws<ArgumentNullException>(() => { this.Cache.AddOrUpdate<int?>("key", 5, (k, v) => null); });
        }


        
        [Test]
        public void AddOrUpdateFactory_AddShouldThrowWhenNullSupplied()
        {
            int? value = null;
            Assert.Throws<ArgumentNullException>(() => { Cache.AddOrUpdate("whatever", value, (k, v) => 5); });
        }


        [Test]
        public void AddOrUpdateFactory_Should_Add()
        {
            Cache.AddOrUpdate("key", 5, (k, existingValue) => existingValue + 1);

            Assert.That(Cache.GetData<int>("key"), Is.EqualTo(5));
            Assert.That(Cache.Contains("key"), Is.True);
        }


        [Test]
        public void AddOrUpdateFactory_ShouldUpdateExistingItem()
        {
            Cache.AddOrUpdate("key", 5);
            Cache.AddOrUpdate("key", 5, (key, existingValue) => existingValue + 1);

            Assert.That(Cache.GetData<int>("key"), Is.EqualTo(6));
        }


        protected abstract ICache CreateCache();


        [Test]
        public void GetCacheItem_ShouldReturnNullIfItemMissing()
        {
            Assert.That(Cache.GetCacheItem<int>("missingkey"), Is.Null);
        }


        [Test]
        public void GetData_ShouldReturnDefaultValueIfItemMissing()
        {
            Assert.That(Cache.GetData<object>("missingkey"), Is.Null);
        }

        
        [Test]
        public void GetData_ShouldReturnDefaultValueIfItemMissing_NullableInt()
        {
            Assert.That(Cache.GetData<int?>("missingkey"), Is.Null);
        }


        [Test]
        public void GetData_ShouldReturnDefaultValueIfItemMissing_Int()
        {
            Assert.That(Cache.GetData<int>("missingkey"), Is.EqualTo(0));
        }

        
        [Test]
        public void GetOrAdd_ShouldThrowWhenNullAdded()
        {
            Assert.Throws<ArgumentNullException>(() => { Cache.GetOrAdd<object>("whatever", _ => null); });
        }


        [Test]
        public void GetOrAdd_ShouldReturnExistingCachedItem()
        {
            //given
            var v1 = new TestValue();
            var v2 = new TestValue();

            Cache.GetOrAdd("someKey", _ => v1);

            //when
            var itemFromCache = Cache.GetOrAdd("someKey", _ => v2);

            //then
            Assert.That(itemFromCache, Is.EqualTo(v1), "previously cached item not returned");
        }


        [Test]
        public void GetOrAdd_WhereItemAlreadyExistsInCache_ShouldNotExecuteConstructorDelegate()
        {
            //given
            Cache.GetOrAdd("someKey", _ => new object());

            //when
            bool constructorExecuted = false;
            Func<string, object> constructor = delegate {
                constructorExecuted = true;
                return new object();
            };
            Cache.GetOrAdd("someKey", constructor);

            //then
            Assert.That(constructorExecuted, Is.False);
        }


        [Test]
        public void GetOrAdd_WhereItemNotAlreadyCached_ShouldAddItemToCache()
        {
            var v1 = new TestValue();
            var cachedItem = Cache.GetOrAdd("someKey", _ => v1);

            //then
            Assert.That(cachedItem, Is.EqualTo(v1));
        }


        [Test]
        public async Task GetOrAdd_WhereItemNotAlreadyCached_UnderRaceCondition_ShouldReturnFirstValueAdded()
        {
            Task<int> t1 = Task.Run(async () => {
                await Task.Delay(TimeSpan.FromMilliseconds(100));
                return Cache.GetOrAdd("Key", _ => 1);
            });
            Task<int> t2 = Task.Run(async () => {
                await Task.Delay(TimeSpan.FromMilliseconds(5));
                return Cache.GetOrAdd("Key", _ => 2);
            });
            Task<int[]> ts = Task.WhenAll(t1, t2);

            Assert.That(await ts, Is.EqualTo(new[] {2, 2}));
        }


        [Test]
        public void Remove_ShouldSilentlyIgnoreMissingItem()
        {
            Assert.DoesNotThrow(() => Cache.Remove("missingkey"));
        }


        [Test]
        public void WhenItemExpiresFromCache_GetOrAdd_WillAddNewItemToCache()
        {
            //given
            var v1 = new TestValue();
            var v2 = new TestValue();
            var v3 = new TestValue();
            Cache.GetOrAdd("someKey", _ => v1);
            Cache.Flush(); //simulate item expiring from the cache

            //when, then
            Assert.That(Cache.GetOrAdd("someKey", _ => v2), Is.EqualTo(v2), "expired item was not replaced");

            Assert.That(Cache.GetOrAdd("someKey", _ => v3), Is.EqualTo(v2), "v2 still expected");
        }

        [Serializable]
        public class TestValue : IEquatable<TestValue>
        {
            private readonly Guid _id = Guid.NewGuid();

            public bool Equals(TestValue other)
            {
                if (ReferenceEquals(null, other)) return false;
                if (ReferenceEquals(this, other)) return true;
                return _id.Equals(other._id);
            }

            public override bool Equals(object obj)
            {
                if (ReferenceEquals(null, obj)) return false;
                if (ReferenceEquals(this, obj)) return true;
                if (obj.GetType() != GetType()) return false;
                return Equals((TestValue)obj);
            }

            public override int GetHashCode()
            {
                return _id.GetHashCode();
            }
        }
    }
}