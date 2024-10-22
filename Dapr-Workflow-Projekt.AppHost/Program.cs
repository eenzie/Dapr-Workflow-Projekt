var builder = DistributedApplication.CreateBuilder(args);

// Dapr Components
var stateStore = builder.AddDaprStateStore("statestore");
var workflowChannel = builder.AddDaprPubSub("workflowChannel");
var paymentChannel = builder.AddDaprPubSub("paymentchannel");
var warehouseChannel = builder.AddDaprPubSub("warehousechannel");

// Dapr projects and references
builder.AddProject<Projects.OrderService>("orderservice")
    .WithDaprSidecar()
    .WithReference(stateStore)
    .WithReference(workflowChannel)  // De skal alle have workflow channel, da det er her de skal svare tilbage til
    .WithReference(paymentChannel)
    .WithReference(warehouseChannel);

builder.AddProject<Projects.PaymentService>("paymentservice")
    .WithDaprSidecar()
    //.WithReference(stateStore)  // Har den brug for state store eller er det kun orchestrator (ordre?)
    .WithReference(workflowChannel)  // Sends response return
    .WithReference(paymentChannel);  // Needs its own channel, as it 'reads' from here.

builder.AddProject<Projects.WarehouseService>("warehouseservice")
    .WithDaprSidecar()
    //.WithReference(stateStore)
    .WithReference(workflowChannel)
    .WithReference(warehouseChannel);

// TODO: Har vi brug for Shared projekt?
//builder.AddProject<Projects.Shared>("shared");

builder.Build().Run();
