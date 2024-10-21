using Dapr.Workflow;
using OrderService.Domain;

namespace OrderService.DaprWorkflow.Workflows.Activities;

public class NotifyActivity : WorkflowActivity<Notification, object?>
{
    private readonly ILogger _logger;

    public NotifyActivity(ILogger<NotifyActivity> logger)
    {
        _logger = logger;
    }

    public override async Task<object?> RunAsync(WorkflowActivityContext context, Notification notificationDto)
    {
        _logger.LogInformation(notificationDto.Message);
        //string channelName = $"pizza-notifications:{notificationDto.OrderDto.OrderId}";
        //var _channel = _realtimeClient.Channels.Get(channelName);
        //await _channel.PublishAsync(notificationDto.OrderDto.Status.ToString(), notificationDto);

        await Task.CompletedTask;
        return null;
    }
}