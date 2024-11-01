using Dapr.Workflow;
using OrderService.DaprWorkflow.Workflows;
using OrderService.DaprWorkflow.Workflows.Activities;
using OrderService.DaprWorkflow.Workflows.Activities.CompensatingActivities;

var builder = WebApplication.CreateBuilder(args);

builder.AddServiceDefaults();

#region Dapr setup
// Dapr uses a random port for gRPC by default. If we don't know what that port
// is (because this app was started separate from dapr), then assume 50001.
var daprGrpcPort = Environment.GetEnvironmentVariable("DAPR_GRPC_PORT");
if (string.IsNullOrEmpty(daprGrpcPort))
{
    Console.WriteLine("DAPR_GRPC_PORT not set. Assuming 50001.");
    daprGrpcPort = "50001";
    Environment.SetEnvironmentVariable("DAPR_GRPC_PORT", daprGrpcPort);
}

builder.Services.AddControllers()
    // Kun ved explicit pubsub 
    .AddDapr(config => config
    .UseGrpcEndpoint($"http://localhost:{daprGrpcPort}"));
#endregion

builder.Services.AddDaprWorkflow(options =>
{
    options.RegisterWorkflow<OrderWorkflow>();

    // "Success" Activity
    options.RegisterActivity<NotifyActivity>();
    options.RegisterActivity<ProcessPaymentActivity>();
    options.RegisterActivity<ReserveItemsActivity>();

    // Compensating Activity
    options.RegisterActivity<IssueRefundActivity>();
});

// Add services to the container.
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