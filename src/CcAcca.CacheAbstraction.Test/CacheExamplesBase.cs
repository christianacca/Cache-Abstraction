// Copyright (c) 2014 Christian Crowhurst.  All rights reserved.
// see LICENSE

using System;
using System.Threading.Tasks;
using NUnit.Framework;

namespace CcAcca.CacheAbstraction.Test
{
    using System.ComponentModel;

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
        public void AddOrUpdate_ShouldUpdateExistingItem_NullValueSupplied()
        {
            Cache.AddOrUpdate("key", 5);
            Cache.AddOrUpdate<int?>("key", null);

            Assert.That(Cache.GetData<int?>("key"), Is.Null);
        }

        [Test]
        public void AddOrUpdate_ShouldUpdateExistingItem_NullIntSupplied()
        {
            Cache.AddOrUpdate("key", 5);
            Cache.AddOrUpdate<int?>("key", null);

            Assert.That(Cache.GetData<int?>("key"), Is.Null);
        }

        [Test]
        public void AddOrUpdate_ShouldUpdateExistingItem_NullDateTimeSupplied()
        {
            Cache.AddOrUpdate("key", 5);
            Cache.AddOrUpdate<DateTime?>("key", null);

            Assert.That(Cache.GetData<DateTime?>("key"), Is.Null);
        }

        [Test]
        public void AddOrUpdate_ShouldUpdateExistingItem_NullGuidSupplied()
        {
            Cache.AddOrUpdate("key", 5);
            Cache.AddOrUpdate<Guid?>("key", null);

            Assert.That(Cache.GetData<Guid?>("key"), Is.Null);
        }


        [Test]
        public void AddOrUpdate_ShouldNotIgnoreAttemptsToAddNullItem()
        {
            Cache.AddOrUpdate<object>("whatever", null);

            Assert.That(Cache.Contains("whatever"), Is.True);
        }

        [Test]
        public void AddOrUpdate_ShouldNotIgnoreAttemptsToAddNullCustomObject()
        {
            Cache.AddOrUpdate<TestValue>("whatever", null);

            Assert.That(Cache.Contains("whatever"), Is.True);
        }

        [Test]
        public void AddOrUpdate_ShouldNotIgnoreAttemptsToAddNullInt()
        {
            Cache.AddOrUpdate<int?>("whatever", null);

            Assert.That(Cache.Contains("whatever"), Is.True);
        }

        [Test]
        public void AddOrUpdate_ShouldNotIgnoreAttemptsToAddNullDateTime()
        {
            Cache.AddOrUpdate<DateTime?>("whatever", null);

            Assert.That(Cache.Contains("whatever"), Is.True);
        }

        [Test]
        public void AddOrUpdateFactory_Should_Add()
        {
            Cache.AddOrUpdate("key", 5, (key, existingValue) => existingValue + 1);

            Assert.That(Cache.Contains("key"), Is.True);
        }


        [Test]
        public void AddOrUpdateFactory_ShouldUpdateExistingItem()
        {
            Cache.AddOrUpdate("key", 5);
            Cache.AddOrUpdate("key", 5, (key, existingValue) => existingValue + 1);

            Assert.That(Cache.GetData<int>("key"), Is.EqualTo(6));
        }


        [Test]
        public void AddOrUpdateFactory_ShouldUpdateExistingItem_NullStringSupplied()
        {
            Cache.AddOrUpdate("key", "crap");
            Cache.AddOrUpdate("key", "crap 2", (key, existingValue) => null);

            Assert.That(Cache.GetData<string>("key"), Is.Null);
        }

        [Test]
        public void AddOrUpdateFactory_ShouldUpdateExistingItem_NullInt16Supplied()
        {
            Cache.AddOrUpdate<short?>("key", 10);
            Cache.AddOrUpdate<short?>("key", 11, (key, existingValue) => null);

            Assert.That(Cache.GetData<short?>("key"), Is.Null);
        }

        [Test]
        public void AddOrUpdateFactory_ShouldUpdateExistingItem_NullIntSupplied()
        {
            Cache.AddOrUpdate("key", 10);
            Cache.AddOrUpdate<int?>("key", 11, (key, existingValue) => null);

            Assert.That(Cache.GetData<int?>("key"), Is.Null);
        }

        [Test]
        public void AddOrUpdateFactory_ShouldUpdateExistingItem_NullInt64Supplied()
        {
            Cache.AddOrUpdate<long?>("key", 10);
            Cache.AddOrUpdate<long?>("key", 11, (key, existingValue) => null);

            Assert.That(Cache.GetData<long?>("key"), Is.Null);
        }

        [Test]
        public void AddOrUpdateFactory_ShouldUpdateExistingItem_NullFloatSupplied()
        {
            Cache.AddOrUpdate<float?>("key", 10F);
            Cache.AddOrUpdate<float?>("key", 11F, (key, existingValue) => null);

            Assert.That(Cache.GetData<float?>("key"), Is.Null);
        }

        [Test]
        public void AddOrUpdateFactory_ShouldUpdateExistingItem_NullDoubleSupplied()
        {
            Cache.AddOrUpdate<double?>("key", 10D);
            Cache.AddOrUpdate<double?>("key", 11D, (key, existingValue) => null);

            Assert.That(Cache.GetData<double?>("key"), Is.Null);
        }

        [Test]
        public void AddOrUpdateFactory_ShouldUpdateExistingItem_NullDecimalSupplied()
        {
            Cache.AddOrUpdate<decimal?>("key", 10m);
            Cache.AddOrUpdate<decimal?>("key", 11m, (key, existingValue) => null);

            Assert.That(Cache.GetData<decimal?>("key"), Is.Null);
        }


        [Test]
        public void AddOrUpdateFactory_ShouldUpdateExistingItem_NullGuidSupplied()
        {
            Cache.AddOrUpdate("key", Guid.NewGuid());
            Cache.AddOrUpdate<Guid?>("key", Guid.NewGuid(), (key, existingValue) => null);

            Assert.That(Cache.GetData<Guid?>("key"), Is.Null);
        }

        [Test]
        public void AddOrUpdateFactory_ShouldUpdateExistingItem_NullByteArraySupplied()
        {
            Cache.AddOrUpdate("key", new Byte[0]);
            Cache.AddOrUpdate("key", new Byte[0], (key, existingValue) => null);

            Assert.That(Cache.GetData<Byte[]>("key"), Is.Null);
        }

        [Test]
        public void AddOrUpdateFactory_ShouldUpdateExistingItem_NullDateTimeSupplied()
        {
            Cache.AddOrUpdate("key", DateTime.Now);
            Cache.AddOrUpdate<DateTime?>("key", DateTime.Now, (key, existingValue) => null);

            Assert.That(Cache.GetData<DateTime?>("key"), Is.Null);
        }

        [Test]
        public void AddOrUpdateFactory_ShouldUpdateExistingItem_NullDateTimeOffsetSupplied()
        {
            Cache.AddOrUpdate("key", DateTimeOffset.Now);
            Cache.AddOrUpdate<DateTimeOffset?>("key", DateTimeOffset.Now, (key, existingValue) => null);

            Assert.That(Cache.GetData<DateTimeOffset?>("key"), Is.Null);
        }

        [Test]
        public void AddOrUpdateFactory_ShouldUpdateExistingItem_NullCustomObjectSupplied()
        {
            Cache.AddOrUpdate("key", new TestValue());
            Cache.AddOrUpdate("key", new TestValue(), (key, existingValue) => null);

            Assert.That(Cache.GetData<TestValue>("key"), Is.Null);
        }

        [Test]
        public void AddOrUpdateFactory_ShouldNotIgnoreAttemptsToAddNullItem()
        {
            Cache.AddOrUpdate<int?>("whatever", null, (key, existingValue) => 1);

            Assert.That(Cache.Contains("whatever"), Is.True);
            Assert.That(Cache.GetData<int?>("whatever"), Is.Null);
        }

        [Test]
        public void AddOrUpdateFactory_ShouldNotIgnoreAttemptsToAddNullCustomValue()
        {
            Cache.AddOrUpdate<TestValue>("whatever", null, (key, existingValue) => new TestValue());

            Assert.That(Cache.Contains("whatever"), Is.True);
            Assert.That(Cache.GetData<TestValue>("whatever"), Is.Null);
        }

        [Test]
        public void AddOrUpdateFactory_ShouldNotIgnoreAttemptsToUpdateNullInt()
        {
            // given
            Cache.AddOrUpdate<int?>("whatever", null, (key, existingValue) => 10);

            // when
            Cache.AddOrUpdate<int?>("whatever", null, (key, existingValue) => 11);

            Assert.That(Cache.Contains("whatever"), Is.True);
            Assert.That(Cache.GetData<int?>("whatever"), Is.EqualTo(11));
        }

        [Test]
        public void AddOrUpdateFactory_ShouldNotIgnoreAttemptsToUpdateNullCustomValue()
        {
            // given
            Cache.AddOrUpdate<TestValue>("whatever", null, (key, existingValue) => new TestValue());

            // when
            var update = new TestValue();
            Cache.AddOrUpdate<TestValue>("whatever", null, (key, existingValue) => update);

            Assert.That(Cache.Contains("whatever"), Is.True);
            Assert.That(Cache.GetData<TestValue>("whatever"), Is.EqualTo(update));
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
        public void GetOrAdd_ShouldNotIgnoreAttemptsToAddNullItem()
        {
            Cache.GetOrAdd<object>("whatever", _ => null);

            Assert.That(Cache.Contains("whatever"), Is.True);
        }

        [Test]
        public void GetOrAdd_ShouldNotIgnoreAttemptsToAddNullCustomObject()
        {
            Cache.GetOrAdd<TestValue>("whatever", _ => null);

            Assert.That(Cache.Contains("whatever"), Is.True);
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