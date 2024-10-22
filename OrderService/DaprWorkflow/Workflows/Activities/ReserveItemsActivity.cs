using Dapr.Client;
using Dapr.Workflow;
using Shared.Dtos;
using Shared.IntegrationEvents;
using Shared.Queues;

namespace OrderService.DaprWorkflow.Workflows.Activities
{
    public class ReserveItemsActivity : WorkflowActivity<OrderItemDto, object?>
    {
        private readonly DaprClient _daprClient;
        private readonly ILogger<NotifyActivity> _logger;

        public ReserveItemsActivity(DaprClient daprClient, ILogger<NotifyActivity> logger)
        {
            _daprClient = daprClient;
            _logger = logger;
        }

        public override async Task<object?> RunAsync(WorkflowActivityContext context, OrderItemDto input)
        {
            _logger.LogInformation($"About to publish: {input}");

            var reservationRequestMessage = new ReserveItemsEvent { CorrelationId = context.InstanceId };

            await _daprClient.PublishEventAsync(WarehouseChannel.Channel,
                                                WarehouseChannel.Topics.Reservation,
                                                reservationRequestMessage);

            return null;
        }
    }
}
