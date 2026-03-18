// src/Services/FinancialCore.API/Controllers/TransactionsController.cs
using FraudDetection.gRPC;
using Microsoft.AspNetCore.Mvc;
using StackExchange.Redis;
using System.Text.Json;

namespace FinancialCore.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class TransactionsController : ControllerBase
{
    private readonly FraudScoring.FraudScoringClient _fraudClient;
    private readonly IConnectionMultiplexer _redis; 

    public TransactionsController(FraudScoring.FraudScoringClient fraudClient,IConnectionMultiplexer redis)
    {
        _fraudClient = fraudClient;
        _redis = redis;
    }

    [HttpPost("process")]
    public async Task<IActionResult> ProcessTransaction([FromBody] TransactionRequestDto request)
    {
        var grpcRequest = new TransactionRequest
        {
            UserId = request.UserId,
            Amount = request.Amount,
            Currency = request.Currency
        };

        var fraudAnalysis = await _fraudClient.AnalyzeTransactionAsync(grpcRequest);

        if (!fraudAnalysis.IsSafe)
        {
            return BadRequest(new { Status = "Rejected", Reason = fraudAnalysis.RiskReason, Score = fraudAnalysis.RiskScore });
        }

        // --- ESTA ES LA PARTE CLAVE QUE TE FALTA ---
        var publisher = _redis.GetDatabase();
        var messagePayload = JsonSerializer.Serialize(new { 
            UserId = request.UserId, 
            Amount = request.Amount, 
            Timestamp = DateTime.UtcNow 
        });

        // Publicamos en Redis
        await publisher.PublishAsync(RedisChannel.Literal("approved_transactions"), messagePayload);

        // Devolvemos 202 Accepted en lugar de 200 OK
        return Accepted(new 
        { 
            Status = "Processing", 
            Message = "Transaction passed fraud checks and is queued for settlement.",
            Score = fraudAnalysis.RiskScore
        });
    }
}

// DTO simple para recibir el JSON desde Swagger
public class TransactionRequestDto
{
    public string UserId { get; set; } = string.Empty;
    public double Amount { get; set; }
    public string Currency { get; set; } = string.Empty;
}