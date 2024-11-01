using Dapr;
using Dapr.Client;
using Microsoft.AspNetCore.Mvc;
using OrderService.DaprWorkflow.Workflows.External;
using Shared.IntegrationEvents;
using Shared.Queues;

namespace WorkflowSample.Controllers;

[Route("api/[controller]/[action]")]
[ApiController]
public class WorkflowChannelController : ControllerBase
{
    private readonly DaprClient _daprClient;
    private readonly ILogger<WorkflowChannelController> _logger;
    private readonly string _workflowComponentName = "dapr";
    // private readonly string _workflowName = nameof(OrderWorkflow);

    public WorkflowChannelController(DaprClient daprClient, ILogger<WorkflowChannelController> logger)
    {
        _daprClient = daprClient;
        _logger = logger;
    }

    [Topic(WorkflowChannel.Channel, WorkflowChannel.Topics.PaymentResult)]
    [HttpPost]
    public async Task<IActionResult> PaymentResult([FromBody] PaymentProcessedResultEvent paymentResponse)
    {
        _logger.LogInformation(
            $"Payment response received: Id: {paymentResponse.CorrelationId}, Amount: {paymentResponse.Amount}, State: {paymentResponse.State}");

        await _daprClient.RaiseWorkflowEventAsync(
            paymentResponse.CorrelationId,
            _workflowComponentName,
            ExternalEvents.PaymentEvent,
            paymentResponse);

        _logger.LogInformation("Payment response send to workflow");
        return Ok();
    }

    [Topic(WorkflowChannel.Channel, WorkflowChannel.Topics.ItemsReserveResult)]
    [HttpPost]
    // TODO: Skal den hedde ItemsReservedResult eller ReservationResult?
    public async Task<IActionResult> ReservationResult([FromBody] ItemsReservedResultEvent reservationReponse)
    {
        _logger.LogInformation(
            $"Reservation response received: ID: {reservationReponse.CorrelationId}");

        await _daprClient.RaiseWorkflowEventAsync(
            reservationReponse.CorrelationId,
            _workflowComponentName,
            ExternalEvents.ItemReservedEvent,
            reservationReponse);

        _logger.LogInformation("Reservation response sent to workflow");

        return Ok();
    }

    //[Topic(WorkflowChannel.Channel, WorkflowChannel.Topics.ItemsReserveResult)]
    //[HttpPost]
    //public async Task<IActionResult> ItemsReservedResult([FromBody] ItemsReservedResultEvent itemsReservedResponse)
    //{
    //	_logger.LogInformation(
    //		$"Payment response received: Id: {itemsReservedResponse.CorrelationId}, State: {itemsReservedResponse.State}");

    //	await _daprClient.RaiseWorkflowEventAsync(itemsReservedResponse.CorrelationId, _workflowComponentName,
    //		ExternalEvents.ItemReservedEvent,
    //		itemsReservedResponse);

    //	_logger.LogInformation("Payment response send to workflow");
    //	return Ok();
    //}

    //[Topic(WorkflowChannel.Channel, WorkflowChannel.Topics.ItemsReserveResult)]
    //[HttpPost]
    //public async Task<IActionResult> ItemsShippedResult([FromBody] ItemsShippedResultEvent itemsShippedResponse)
    //{
    //    _logger.LogInformation(
    //        $"Payment response received: Id: {itemsShippedResponse.CorrelationId}, State: {itemsShippedResponse.State}");

    //    await _daprClient.RaiseWorkflowEventAsync(itemsShippedResponse.CorrelationId, _workflowComponentName,
    //        ExternalEvents.ItemShippedEvent,
    //        itemsShippedResponse);

    //    _logger.LogInformation("Payment response send to workflow");
    //    return Ok();
    //}
}