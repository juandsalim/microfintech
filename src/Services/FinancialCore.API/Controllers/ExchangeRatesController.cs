// src/Services/FinancialCore.API/Controllers/ExchangeRatesController.cs
using Application.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace FinancialCore.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ExchangeRatesController : ControllerBase
{
    private readonly IExchangeRateCache _cache;

    public ExchangeRatesController(IExchangeRateCache cache)
    {
        _cache = cache;
    }

    [HttpPost("{pair}")]
    public async Task<IActionResult> SetRate(string pair, [FromBody] decimal rate)
    {
        // Guardamos la tasa por 5 minutos
        await _cache.SetRateAsync(pair.ToUpper(), rate, TimeSpan.FromMinutes(5));
        return Ok(new { Message = $"Rate {rate} for {pair.ToUpper()} cached successfully." });
    }

    [HttpGet("{pair}")]
    public async Task<IActionResult> GetRate(string pair)
    {
        var rate = await _cache.GetRateAsync(pair.ToUpper());
        
        if (rate == null)
            return NotFound(new { Message = "Rate not found in cache or expired." });

        return Ok(new { Pair = pair.ToUpper(), Rate = rate });
    }
}