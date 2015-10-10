<a name="3.0.0"></a>
# 3.0.0 (2015-10-10)


## Features

- **ICache:** new overload to AddOrUpdate that accepts update delegate
  ([720311af](https://github.com/christianacca/Cache-Abstraction/commit/720311af9c578f2eee63fb3feca7fb74535ef81c))


## Breaking Changes

- **CacheDecoratorOptions:** due to [2e70c22f](https://github.com/christianacca/Cache-Abstraction/commit/2e70c22f4453c89d17501ecb7e9cd3416a76d4b1),
  caches no longer decorated by `MultiThreadProtectedDecorator` by default

To make this decorator apply by default you will need to set `CacheDecoratorOptions.Default` at the start of your application. Eg:

```csharp
CacheDecoratorOptions.Default = new CacheDecoratorOptions
{
    IsMultiThreadProtectionOn = true,
    IsPausableOn = true
};
```

- **ICache:** due to [8b61518e](https://github.com/christianacca/Cache-Abstraction/commit/8b61518e332e3c88decd236d9bc56da81d4c76fd),
  remove null support

This is in preparation to adding a new overload to AddOrUpdate.

This new AddOrUpdate overload will not work with CacheManager
library in conjunction with null support


<a name="2.1.0"></a>
# 2.1.0 (2015-05-25)


## Bug Fixes

- **NullCache:** `As<T>` should return a reference to self
  ([065750a2](https://github.com/christianacca/Cache-Abstraction/commit/065750a26b80d5639af8962274fe3277773d9401))


## Features

- **GlobalCacheProvider:** add DefaultInstance static property to return a well known instance
  ([ecf9ae93](https://github.com/christianacca/Cache-Abstraction/commit/ecf9ae9378c86ce2fda953ff34fe172aae13f90f))



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


