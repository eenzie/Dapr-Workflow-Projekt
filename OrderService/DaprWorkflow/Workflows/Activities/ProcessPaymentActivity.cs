using Dapr.Client;
using Dapr.Workflow;
using Shared.Dtos;
using Shared.IntegrationEvents;
using Shared.Queues;

namespace OrderService.DaprWorkflow.Workflows.Activities;

// https://docs.dapr.io/developing-applications/building-blocks/workflow/workflow-patterns/
public class ProcessPaymentActivity : WorkflowActivity<PaymentDto, object?>
{
    private readonly DaprClient _daprClient;
    private readonly ILogger<NotifyActivity> _logger;

    public ProcessPaymentActivity(DaprClient daprClient, ILogger<NotifyActivity> logger)
    {
        _daprClient = daprClient;
        _logger = logger;
    }

    public override async Task<object?> RunAsync(WorkflowActivityContext context, PaymentDto input)
    {
        _logger.LogInformation($"About to publish: {input}");
        var paymentRequestMessage = new ProcessPaymentEvent { CorrelationId = context.InstanceId, Amount = 100 };
        await _daprClient.PublishEventAsync(PaymentChannel.Channel, PaymentChannel.Topics.Payment,
            paymentRequestMessage);

        return null;
    }
}