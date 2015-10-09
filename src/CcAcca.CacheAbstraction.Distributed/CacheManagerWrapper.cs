// Copyright (c) 2014 Christian Crowhurst.  All rights reserved.
// see LICENSE

using System;
using CacheManager.Core;

namespace CcAcca.CacheAbstraction.Distributed
{
    /// <summary>
    /// Adapts the ICacheManager from the https://github.com/MichaCo/CacheManager library to match
    /// <see cref="ICache"/>
    /// </summary>
    /// <remarks>
    /// ICacheManager implementations add support for various distributed caches such as Redis an Memcached
    /// </remarks>
    public class CacheManagerWrapper : CacheBase, ICache
    {
        public CacheManagerWrapper(CacheIdentity id, ICacheManager<object> impl) : base(id)
        {
            Impl = impl;
        }

        private ICacheManager<object> Impl { get; set; }

        public virtual int? Count
        {
            get { return null; }
        }

        public virtual void AddOrUpdate<T>(string key, T value, object cachePolicy = null)
        {
            Impl.Put(key, GetItemValue(value), Id.ToString());
        }

        private static object GetItemValue<T>(T value)
        {
            return ReferenceEquals(null, value) ? NullValue.ForType<T>() : value;
        }

        public virtual void AddOrUpdate<T>(string key, T addValue, Func<string, T, T> updateFactory, object cachePolicy = null)
        {
            Impl.AddOrUpdate(key, Id.ToString(), GetItemValue(addValue), existingValue => GetItemValue(updateFactory(key, (T)existingValue)));
        }

        public virtual bool Contains(string key)
        {
            return GetCacheItem<object>(key) != null;
        }

        public virtual void Flush()
        {
            Impl.ClearRegion(Id.ToString());
        }

        public virtual CacheItem<T> GetCacheItem<T>(string key)
        {
            var implItem = Impl.GetCacheItem(key, Id.ToString());
            if (implItem == null) return null;
            if (NullValue.IsNull<T>(implItem.Value))
            {
                return new CacheItem<T>(default(T));
            }

            return new CacheItem<T>((T)implItem.Value);
        }

        public virtual void Remove(string key)
        {
            Impl.Remove(key, Id.ToString());
        }

        /// <remarks>
        /// <para>
        /// Having to use speicifc values as nulls is a workaround to CacheManager not allowing null values to be stored
        /// </para>
        /// <para>
        /// The solution
        /// </para>
        /// </remarks>
        private static class NullValue
        {
            public static object ForType<T>()
            {
                return "{1C0017F4-01E3-4FB5-B1F6-4004D28F08B8}";
                // WARNING: returning minimum value as a way to indicate null is a very poor solution
                // for example what if the value you need to cache is a Int16.MinValue - this will be considered null
                // todo: Need to come up with a better solution than using minimum value

/*
                var type = Nullable.GetUnderlyingType(typeof(T)) ?? typeof(T);
                if (type == typeof(Int16))
                {
                    return Int16.MinValue;
                }
                if (type == typeof(Int32))
                {
                    return Int32.MinValue;
                }
                if (type == typeof(Int64))
                {
                    // needed to add 1 to avoid StackExchange.Redis.RedisValue.TryParseInt64 throwing a OverflowException
                    return Int64.MinValue + 1;
                }
                if (type == typeof(decimal))
                {
                    return Decimal.MinValue;
                }
                if (type == typeof(float))
                {
                    return float.MinValue;
                }
                if (type == typeof(double))
                {
                    return Double.MinValue;
                }
                if (type == typeof(String))
                {
                    return "";
                }
                if (type == typeof(DateTime))
                {
                    return DateTime.MinValue;
                }
                if (type == typeof(DateTimeOffset))
                {
                    return DateTimeOffset.MinValue;
                }
                if (type == typeof(Guid))
                {
                    return new Guid("{908604F5-D5CB-4515-A24E-881EA2E3630C}");
                }
                return "";
*/
            }

            public static bool IsNull<T>(object value)
            {
                return Equals(ForType<T>(), value);
            }
        }
    }
}