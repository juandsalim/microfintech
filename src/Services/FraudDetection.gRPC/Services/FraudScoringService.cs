using Grpc.Core;
using FraudDetection.gRPC;
using Microsoft.ML.OnnxRuntime;
using Microsoft.ML.OnnxRuntime.Tensors;

namespace FraudDetection.gRPC.Services;

public class FraudScoringService : FraudScoring.FraudScoringBase
{
    private readonly ILogger<FraudScoringService> _logger;
    private readonly InferenceSession _onnxSession;

    public FraudScoringService(ILogger<FraudScoringService> logger)
    {
        _logger = logger;
        // Cargamos el modelo matemático en memoria al iniciar el servicio
        var modelPath = Path.Combine(AppContext.BaseDirectory, "ML", "fraud_model.onnx");
        _onnxSession = new InferenceSession(modelPath);
    }

    // FÍJATE AQUÍ: Cambiamos a Task<FraudResponse>
    public override Task<FraudResponse> AnalyzeTransaction(TransactionRequest request, ServerCallContext context)
    {
        _logger.LogInformation($"[ML Engine] Analizando transacción de {request.Amount} {request.Currency}");

        // 1. PREPARACIÓN MATEMÁTICA
        float[] inputData = new float[] { (float)request.Amount, 1.0f };
        var tensor = new DenseTensor<float>(inputData, new int[] { 1, 2 });
        var inputs = new List<NamedOnnxValue>
        {
            NamedOnnxValue.CreateFromTensor("float_input", tensor)
        };

        // 2. INFERENCIA EN TIEMPO REAL
        using var results = _onnxSession.Run(inputs);

        // 3. EXTRACCIÓN DEL RESULTADO
        var prediction = results.First(r => r.Name == "output_label").AsEnumerable<long>().First();
        bool isFraud = prediction == 1;

        // FÍJATE AQUÍ: Instanciamos FraudResponse y usamos las propiedades en PascalCase
        var response = new FraudResponse
        {
            IsSafe = !isFraud,
            RiskScore = isFraud ? 0.99 : 0.05,
            RiskReason = isFraud ? "ML Model detected anomaly based on statistical patterns." : "Clear"
        };

        _logger.LogInformation($"[ML Engine] Resultado de Inferencia: {(isFraud ? "FRAUDE" : "SEGURO")}");

        return Task.FromResult(response);
    }
}