<a name="2.0.0"></a>
# 2.0.0 (2015-05-23)


## Bug Fixes

- **ObjectCacheWrapper:** hide private implementation
  ([b8a37ddb](https://github.com/christianacca/Cache-Abstraction/commit/b8a37ddb90461889620e44596e05932a6dd05c88))


## Features

- **CacheAbstration.Distributed:** support distributed caches (eg Redis)
  ([e6bd8b5e](https://github.com/christianacca/Cache-Abstraction/commit/e6bd8b5ee80ca1c7dc9c41ecf0ee94ce349f3d84))
- **All-Caches:**
  - make members virtual by default
  ([24953739](https://github.com/christianacca/Cache-Abstraction/commit/2495373929507479caebc1b29d4ec3d26b6809c9))
  - isolate caches that share a backing store
  ([b12a58e2](https://github.com/christianacca/Cache-Abstraction/commit/b12a58e24e5da14d400b38147ace76bb6f2d78ff))
- **ICache:** allow a cache to choose to whether to implement the Count property
  ([70532930](https://github.com/christianacca/Cache-Abstraction/commit/70532930dbef5f7d81d4363615d5233234b5c4ab))


## Breaking Changes

- **All-Caches:** due to [b12a58e2](https://github.com/christianacca/Cache-Abstraction/commit/b12a58e24e5da14d400b38147ace76bb6f2d78ff),
  `ICache.Flush` only removes items added by the specific `ICache` instance

Previously `Flush` would remove all items in the shared backing store, irrespective
of the `ICache` instance used to add the item to the store.

- **CacheAbstration.Distributed:** due to [e6bd8b5e](https://github.com/christianacca/Cache-Abstraction/commit/e6bd8b5ee80ca1c7dc9c41ecf0ee94ce349f3d84),
  ICache.GetData<T> method is replaced with a method named GetCacheItem<T>

GetData<T> is now an extension method. This means that consumers of ICache instances
will simply need to recompile against the new binaries.

Authors of subclasses will need to implement the new GetCacheItem<T> and delete their current
GetData<T> method

- **CacheBase:** due to [a479ac99](https://github.com/christianacca/Cache-Abstraction/commit/a479ac990b603fce179bea250e195ed0d4b0e3ce),
  removed CacheBase.InstanceName

This was property has been superseded by the Id property

- **ICache:** due to [70532930](https://github.com/christianacca/Cache-Abstraction/commit/70532930dbef5f7d81d4363615d5233234b5c4ab),
  ICache.Count changed to Nullable<int>


