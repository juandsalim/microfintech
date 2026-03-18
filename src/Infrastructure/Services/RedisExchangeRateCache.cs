// src/Infrastructure/Services/RedisExchangeRateCache.cs
using Application.Interfaces;
using Microsoft.Extensions.Caching.Distributed;

namespace Infrastructure.Services;

public class RedisExchangeRateCache : IExchangeRateCache
{
    private readonly IDistributedCache _cache;

    public RedisExchangeRateCache(IDistributedCache cache)
    {
        _cache = cache;
    }

    public async Task<decimal?> GetRateAsync(string currencyPair)
    {
        var cachedRate = await _cache.GetStringAsync(currencyPair);
        
        if (string.IsNullOrEmpty(cachedRate)) 
            return null;

        return decimal.Parse(cachedRate);
    }

    public async Task SetRateAsync(string currencyPair, decimal rate, TimeSpan expiration)
    {
        var options = new DistributedCacheEntryOptions 
        { 
            AbsoluteExpirationRelativeToNow = expiration 
        };
        
        await _cache.SetStringAsync(currencyPair, rate.ToString(), options);
    }
}