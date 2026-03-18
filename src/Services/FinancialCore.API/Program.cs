using Application.Interfaces;
using Infrastructure.Data;
using Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;
using FraudDetection.gRPC;
using StackExchange.Redis; // Este namespace viene del archivo .proto

var builder = WebApplication.CreateBuilder(args);

// Agregar el DbContext configurado para usar PostgreSQL
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));
builder.Services.AddScoped<IAccountRepository, AccountRepository>();
// Configurar Redis
builder.Services.AddStackExchangeRedisCache(options =>
{
    options.Configuration = builder.Configuration.GetConnectionString("RedisConnection");
});

// Registrar nuestro servicio de caché
builder.Services.AddScoped<IExchangeRateCache, Infrastructure.Services.RedisExchangeRateCache>();

builder.Services.AddControllers();
// Registrar Cliente gRPC
builder.Services.AddGrpcClient<FraudScoring.FraudScoringClient>(options =>
{
    // Por ahora pondremos una URL estática, luego la moveremos al appsettings
    // Asumimos que el motor gRPC correrá en el puerto 5200 (lo configuraremos en un momento)
    options.Address = new Uri("http://localhost:5200"); 
});
// Registrar StackExchange.Redis (Pub/Sub)
builder.Services.AddSingleton<IConnectionMultiplexer>(sp => 
    ConnectionMultiplexer.Connect(builder.Configuration.GetConnectionString("RedisConnection") ?? "localhost:6379"));
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseAuthorization();
app.MapControllers();

app.Run();