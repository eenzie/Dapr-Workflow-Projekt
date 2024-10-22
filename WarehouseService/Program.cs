var builder = WebApplication.CreateBuilder(args);

builder.AddServiceDefaults();

// Add services to the container.
#region Dapr setup
// Dapr uses a random port for gRPC by default. If we don't know what that port
// is (because this app was started separate from dapr), then assume 50001.
var daprGrpcPort = Environment.GetEnvironmentVariable("DAPR_GRPC_PORT");
if (string.IsNullOrEmpty(daprGrpcPort))
{
    daprGrpcPort = "50001";
    Environment.SetEnvironmentVariable("DAPR_GRPC_PORT", daprGrpcPort);
}

// Kun ved explicit pubsub 
builder.Services.AddControllers()
    // Kun ved explicit pubsub 
    .AddDapr(config => config
        .UseGrpcEndpoint($"http://localhost:{daprGrpcPort}"));
#endregion

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

app.MapDefaultEndpoints();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

#region Dapr middelware setup
// app.UseHttpsRedirection(); // Dapr uses HTTP, so no need for HTTPS
app.UseCloudEvents();
app.MapSubscribeHandler(); // Kun ved explicit pubsub
#endregion

app.MapControllers();

app.Run();

