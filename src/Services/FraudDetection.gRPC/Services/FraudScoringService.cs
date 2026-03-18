// src/Services/FraudDetection.gRPC/Services/FraudScoringService.cs
using Grpc.Core;
using FraudDetection.gRPC;

namespace FraudDetection.gRPC.Services;

// Fíjate cómo heredamos de la clase base que .NET generó mágicamente a partir del archivo .proto
public class FraudScoringService : FraudScoring.FraudScoringBase
{
    private readonly ILogger<FraudScoringService> _logger;

    public FraudScoringService(ILogger<FraudScoringService> logger)
    {
        _logger = logger;
    }

    public override Task<FraudResponse> AnalyzeTransaction(TransactionRequest request, ServerCallContext context)
    {
        _logger.LogInformation($"Analyzing transaction for User: {request.UserId}, Amount: {request.Amount}");

        bool isSafe = true;
        double score = 0.1; // 0.0 es totalmente seguro, 1.0 es fraude seguro
        string reason = "Standard transaction";

        // Lógica matemática inicial de scoring (Mockup para la detección de anomalías)
        // Más adelante, aquí cargaremos el modelo entrenado con ML.NET
        if (request.Amount > 10000)
        {
            isSafe = false;
            score = 0.85;
            reason = "Amount exceeds normal standard deviation bounds";
        }
        else if (request.Amount > 5000)
        {
            score = 0.5;
            reason = "High amount, requires manual review";
        }

        return Task.FromResult(new FraudResponse
        {
            IsSafe = isSafe,
            RiskScore = score,
            RiskReason = reason
        });
    }
}