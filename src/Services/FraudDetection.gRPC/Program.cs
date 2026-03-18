using FraudDetection.gRPC.Services;

var builder = WebApplication.CreateBuilder(args);

// Agregamos el framework gRPC
builder.Services.AddGrpc();

var app = builder.Build();

// Mapeamos nuestro servicio a los endpoints
app.MapGrpcService<FraudScoringService>();

// Endpoint de prueba por si alguien entra con un navegador normal
app.MapGet("/", () => "Communication with gRPC endpoints must be made through a gRPC client. To learn how to create a client, visit: https://go.microsoft.com/fwlink/?linkid=2086909");

app.Run();