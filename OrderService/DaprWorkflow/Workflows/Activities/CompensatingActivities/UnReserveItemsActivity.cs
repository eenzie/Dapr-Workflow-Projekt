using Dapr.Client;
using Dapr.Workflow;
using OrderService.Repository;
using Shared.Dtos;
using Shared.IntegrationEvents;
using Shared.Queues;

namespace OrderService.DaprWorkflow.Workflows.Activities.CompensatingActivities;

public class UnReserveItemsActivity : WorkflowActivity<OrderItemDto, object?>
{
    private readonly IStateManagementRepository _stateManagement;
    private readonly ILogger<NotifyActivity> _logger;
    private readonly DaprClient _daprClient;

    public UnReserveItemsActivity(IStateManagementRepository stateManagement, ILogger<NotifyActivity> logger, DaprClient daprClient)
    {
        _stateManagement = stateManagement;
        _logger = logger;
        _daprClient = daprClient;
    }

    public override async Task<object?> RunAsync(WorkflowActivityContext context, OrderItemDto input)
    {
        // TODO: IA Do we not need state management here?
        //await _stateManagement.SaveOrderAsync(order);
        //return new OrderResult(OrderStatus.Received, order);

        _logger.LogInformation($"Unreserving items for OrderId: {context.InstanceId}");

        var unreserveItemsEvent = new ItemsReservationFailedEvent
        {
            // TODO: IA fix this unreserve
            CorrelationId = context.InstanceId,
            Items = new List<OrderItemDto> { input },  // Include items that failed reservation
            Reason = "Reservation failed due to insufficient inventory"
        };

        await _daprClient.PublishEventAsync(WarehouseChannel.Channel,
                                            WarehouseChannel.Topics.ReservationFailed,
                                            unreserveItemsEvent);

        _logger.LogInformation($"Unreserve request published for OrderId: {context.InstanceId}");

        return null;
    }
}