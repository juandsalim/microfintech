// src/Application/Interfaces/IExchangeRateCache.cs
namespace Application.Interfaces;

public interface IExchangeRateCache
{
    Task<decimal?> GetRateAsync(string currencyPair);
    Task SetRateAsync(string currencyPair, decimal rate, TimeSpan expiration);
}