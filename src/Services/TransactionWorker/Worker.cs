// src/Services/TransactionWorker/Worker.cs
using System.Text.Json;
using StackExchange.Redis;

namespace TransactionWorker;

public class Worker : BackgroundService
{
    private readonly ILogger<Worker> _logger;
    private readonly IConnectionMultiplexer _redis;
    private const string ChannelName = "approved_transactions";

    public Worker(ILogger<Worker> logger, IConfiguration configuration)
    {
        _logger = logger;
        // Nos conectamos al Redis que está en tu Docker
        var connectionString = configuration.GetConnectionString("RedisConnection") ?? "localhost:6379";
        _redis = ConnectionMultiplexer.Connect(connectionString);
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("Worker is starting and connecting to Redis Pub/Sub...");

        var subscriber = _redis.GetSubscriber();

        // Nos suscribimos a la "cola" de transacciones aprobadas
        await subscriber.SubscribeAsync(RedisChannel.Literal(ChannelName), (channel, message) =>
        {
            _logger.LogInformation($"[RECEIVED MESSAGE] New approved transaction incoming!");
            
            // Simulamos el procesamiento pesado (ej. guardar en DB, enviar email)
            ProcessTransactionAsync(message.ToString()).GetAwaiter().GetResult();
        });

        // Mantenemos el worker vivo hasta que lo detengas con Ctrl+C
        while (!stoppingToken.IsCancellationRequested)
        {
            await Task.Delay(1000, stoppingToken);
        }
    }

    private async Task ProcessTransactionAsync(string message)
    {
        _logger.LogInformation($"Processing payload: {message}");
        
        // Simulamos que el trabajo a la base de datos tarda 2 segundos
        await Task.Delay(2000); 
        
        _logger.LogInformation($"[SUCCESS] Transaction fully processed and settled in the ledger.\n---");
    }
}