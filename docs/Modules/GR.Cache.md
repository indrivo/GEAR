# Cache module

## Description
This module is created for store some data in cache by key, now it has 2 implementations:
- `InMemory`
- `Distributed Cache (Redis)`

## Installation
This module is registered in core of GR,WebApplication, it has configurations, for details on configuration, consult WebApplication module docs

But it can easily installed in Services configuration:
1. `InMemory` provider
```csharp
services.AddCacheModule<InMemoryCacheService>();
```
2. `Distributed` cache
```csharp
services.AddDistributedMemoryCache()
                .AddCacheModule<DistributedCacheService>()
                .AddRedisCacheConfiguration<RedisConnection>(configuration.HostingEnvironment, configuration.Configuration);
```
## Usage

For use this module, you  need to inject the service, injection can be made using `IoC` helper or standard .net core injection:
Example:
```csharp
    [Authorize(Roles = GlobalResources.Roles.ADMINISTRATOR)]
    public class CacheController : Controller
    {
        /// <summary>
        /// Inject cache service
        /// </summary>
        private readonly ICacheService _cacheService;

        /// <summary>
        /// Cache options
        /// </summary>
        private readonly IWritableOptions<CacheConfiguration> _writableOptions;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="cacheService"></param>
        /// <param name="writableOptions"></param>
        public CacheController(ICacheService cacheService, IWritableOptions<CacheConfiguration> writableOptions)
        {
            _cacheService = cacheService;
            _writableOptions = writableOptions;
        }
```
Available methods:
- `Task<bool> SetAsync<TObject>(string key, TObject obj) where TObject : class;` -  set a value in cache with a key
Example:
```csharp
 string cacheKey = GenerateKey(s, key);
 await _cacheService.SetAsync(cacheKey, new {});
```
-  `Task<TObject> GetAsync<TObject>(string key) where TObject : class;` - get a value by saved previously in cache
Example:
```csharp
  var cacheData = await _cacheService.GetAsync<List<CountryInfoViewModel>>(Key);
```
- `Task RemoveAsync(string key);` - remove a value from cache
- `IEnumerable<CacheEntry> GetAllKeys();` - get all registered keys
- `IEnumerable<CacheEntry> GetAllByPatternFilter(string pattern);` - find keys by filter pattern 
- ` bool IsConnected();` - check if provider is available
-  `void FlushAll();` - remove all from cache
-  `string GetImplementationProviderName();` 
- `Task<ResultModel<T>> GetOrSetResponseAsync<T>(string key, Func<Task<ResultModel<T>>> func) where T : class;` - get value, if value is null then the resolver is called, result is saved in cache and returned. Note: the result is saved only if the result state is OK
Example: 
```csharp
 var request = await _cacheService.GetOrSetResponseAsync("coinbasePro_products", async ()
                => await SendServiceCall<IEnumerable<Product>>(HttpMethod.Get, "/products"));
```
-  `Task<T> GetOrSetResponseAsync<T>(string key, Func<Task<T>> func) where T : class;` - this method get or set the value, if value is null, then is resolver is called
- `Task<T> GetOrSetWithExpireTimeAsync<T>(string key, TimeSpan life, Func<Task<T>> func) where T : class;` - this method get and set values in cache, then the life argument is set for a specific time, the value from cache is available until the timer is not expired, if time expire, the value is renewed , renew process is not due automatically, is due in moment of call of method
Example:
```csharp
            var profit = _cacheService.GetOrSetWithExpireTimeAsync($"wallet_profit_{currency.Code}_{source.Id}", TimeSpan.FromMinutes(15), async () =>
                await CalculateWalletProfitAsync(source.Id, source.WalletType.Symbol, currency.Code)).GetAwaiter().GetResult();
```