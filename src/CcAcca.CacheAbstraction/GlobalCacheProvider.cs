// Copyright (c) 2014 Christian Crowhurst.  All rights reserved.
// see LICENSE

using System;
using System.Collections.Concurrent;
using System.Runtime.Caching;

namespace CcAcca.CacheAbstraction
{
    /// <summary>
    /// Creates <see cref="ICache"/> instances that will be long lived within the application, and so therefore
    /// will be registered with any <see cref="CacheAdministator"/> that this provider has been configured with
    /// </summary>
    /// <remarks>
    /// By default, instances of this class will use the default cache constructors that have previously been
    /// registered using <see cref="RegisterDefaultConstructor{T}"/>
    /// </remarks>
    public class GlobalCacheProvider : ICacheProvider
    {
        private const string ExMsg = "A constructor has not been registed for the Cache service request; requested: {0}";

        private readonly ICache _caches = new MultiThreadProtectedDecorator(new SimpleInmemoryCache());
        private readonly ConcurrentDictionary<Type, Func<CacheIdentity, ICache>> _cacheConstructors;

        public GlobalCacheProvider()
        {
            _cacheConstructors = new ConcurrentDictionary<Type, Func<CacheIdentity, ICache>>(_defaultConstructors);
        }

        public virtual CacheAdministator CacheAdministator { get; set; }

        public virtual ConcurrentDictionary<Type, Func<CacheIdentity, ICache>> CacheConstructors
        {
            get { return new ConcurrentDictionary<Type, Func<CacheIdentity, ICache>>(_cacheConstructors); }
        }

        public virtual void ClearConstructors()
        {
            _cacheConstructors.Clear();
        }

        public virtual ICache Get<T>(CacheIdentity cacheId) where T : class
        {
            if (!_cacheConstructors.ContainsKey(typeof (T)))
            {
                throw new ArgumentException(string.Format(ExMsg, typeof (T).FullName));
            }
            if (cacheId == null)
            {
                throw new ArgumentNullException("cacheId");
            }

            ICache cache = _caches.GetOrAdd(cacheId.ToString(),
                                            id => {
                                                Func<CacheIdentity, ICache> ctor = _cacheConstructors[typeof (T)];
                                                ICache c = ctor(id);
                                                if (CacheAdministator != null)
                                                {
                                                    return CacheAdministator.Register(c);
                                                }
                                                return c;
                                            });

            return cache;
        }

        /// <summary>
        /// Register a delegate that will be responsible for creating an instance of the cache requested
        /// </summary>
        /// <typeparam name="T">The type of cache that to be created</typeparam>
        /// <param name="ctor">
        /// The delegate that will be called to constructor the cache of type <typeparamref name="T"/>. The delegate
        /// will be supplied with the <see cref="CacheIdentity"/> of the cache to be constructed. 
        /// The delegate must assign this id to the cache instance that it creates.
        /// </param>
        /// <remarks>
        /// Type <typeparamref name="T"/> can be the actual cache implementation class (eg. <see cref="MemoryCache"/>, 
        /// or an interface. It's up to the <paramref name="ctor"/> delegate to return an appropriate cache instance
        /// for the request type.
        /// </remarks>
        /// <example>
        /// <code>
        /// RegisterConstructor&lt;MemoryCache&gt;(
        ///     cacheId => new MemoryCache(cacheId.Name).WrapCache(CacheCreationOptions.DefaultsWith(cacheId)));
        /// </code>
        /// </example>
        public virtual void RegisterConstructor<T>(Func<CacheIdentity, ICache> ctor) where T : class
        {
            _cacheConstructors[typeof (T)] = ctor;
        }

        /// <summary>
        /// Safely removes the previously registered cache constructor for the type <typeparamref name="T"/>
        /// </summary>
        /// <typeparam name="T">The type paramater supplied when registering the constructor</typeparam>
        public virtual void RemoveConstructor<T>() where T : class
        {
            Func<CacheIdentity, ICache> ignored;
            _cacheConstructors.TryRemove(typeof (T), out ignored);
        }

        private static ConcurrentDictionary<Type, Func<CacheIdentity, ICache>> _defaultConstructors;
        private static Lazy<GlobalCacheProvider> _defaultInstance;

        static GlobalCacheProvider()
        {
            ResetDefaultConstructors();
            // defer creating the actual instance so as that default cache constructors can be registered
            // at the start of the executing application
            _defaultInstance = new Lazy<GlobalCacheProvider>(() => new GlobalCacheProvider { CacheAdministator = CacheAdministator.DefaultInstance });
        }

        public static ConcurrentDictionary<Type, Func<CacheIdentity, ICache>> CreateDefaultCacheCtors()
        {
            var ctors = new ConcurrentDictionary<Type, Func<CacheIdentity, ICache>>();

            DoRegisterDefaultConstructor<ICache>(ctors,
                cacheId => new ConcurrentDictionary<string, object>()
                    .WrapCache(CacheCreationOptions.DefaultsWith(cacheId)));
            DoRegisterDefaultConstructor<ObjectCache>(ctors,
                cacheId => new MemoryCache(cacheId.Name).WrapCache(CacheCreationOptions.DefaultsWith(cacheId)));
            DoRegisterDefaultConstructor<MemoryCache>(ctors,
                cacheId => new MemoryCache(cacheId.Name).WrapCache(CacheCreationOptions.DefaultsWith(cacheId)));

            return ctors;
        }

        /// <summary>
        /// A default, globally available, instance of a <see cref="GlobalCacheProvider"/> for use within an
        /// application
        /// </summary>
        /// <remarks>
        /// <para>
        /// Any caches created by this default instance will be:
        /// <list type="bullet">
        /// <item>registered with the default instance of the <see cref="CacheAdministator"/></item>
        /// <item>created using the default cache constructors registered at the start of the executing application</item>
        /// </list>
        /// </para>
        /// <para>
        /// To override the above behaviour, an application should assign it's own instance to this static property
        /// </para>
        /// </remarks>
        /// <seealso cref="RegisterDefaultConstructor{T}"/>
        public static GlobalCacheProvider DefaultInstance
        {
            get { return _defaultInstance.Value; }
            set { _defaultInstance = new Lazy<GlobalCacheProvider>(() => value); }
        }

        /// <summary>
        /// Removes all cache constructors that have been registered using <see cref="RegisterDefaultConstructor{T}"/>
        /// </summary>
        public static void ClearDefaultConstructors()
        {
            _defaultConstructors = new ConcurrentDictionary<Type, Func<CacheIdentity, ICache>>();
        }

        /// <summary>
        /// Register a default constructor that cache providers will use to create cache instances. A default
        /// constructor will be used unless another is specified for an individual cache provider.
        /// </summary>
        /// <remarks>
        /// For details see <see cref="RegisterConstructor{T}"/>
        /// </remarks>
        public static void RegisterDefaultConstructor<T>(Func<CacheIdentity, ICache> ctor) where T : class
        {
            DoRegisterDefaultConstructor<T>(_defaultConstructors, ctor);
        }

        /// <summary>
        /// see <see cref="RegisterDefaultConstructor{T}"/>
        /// </summary>
        private static void DoRegisterDefaultConstructor<T>(ConcurrentDictionary<Type, Func<CacheIdentity, ICache>> ctors, Func<CacheIdentity, ICache> ctor) where T : class
        {
            ctors[typeof (T)] = ctor;
        }

        /// <summary>
        /// Resets the list of default cache constructors to "factory defaults"
        /// </summary>
        public static void ResetDefaultConstructors()
        {
            _defaultConstructors = CreateDefaultCacheCtors();
        }
    }
}