using Dapr;
using Dapr.Client;
using Microsoft.AspNetCore.Mvc;
using Shared.IntegrationEvents;
using Shared.Queues;

namespace WarehouseService.Controllers;

[Route("api/[controller]/[action]")]  // TODO: Ændrer maturity level fra 2 til 1. Burde være routing på HTTP
[ApiController]
public class ItemsController : ControllerBase
{
    private readonly DaprClient _daprClient;
    private readonly ILogger<ItemsController> _logger;

    public ItemsController(DaprClient daprClient, ILogger<ItemsController> logger)
    {
        _daprClient = daprClient;
        _logger = logger;
    }

    [Topic(WarehouseChannel.Channel, WarehouseChannel.Topics.Reservation)]
    [HttpPost]
    public async Task<IActionResult> DoReservation([FromBody] ReserveItemsEvent reserveItemsRequest)
    {
        _logger.LogInformation($"Inventory request received: {reserveItemsRequest.CorrelationId}");

        var itemsReservedResponse = new ItemsReservedResultEvent
        {
            CorrelationId = reserveItemsRequest.CorrelationId,
            State = ResultState.Succeeded
        };

        await _daprClient.PublishEventAsync(WorkflowChannel.Channel, WorkflowChannel.Topics.ItemsReserveResult,
            itemsReservedResponse);

        _logger.LogInformation(
            $"Item reserved: {itemsReservedResponse.CorrelationId}, {itemsReservedResponse.State}");

        return Ok();
    }

    [Topic(WarehouseChannel.Channel, WarehouseChannel.Topics.ReservationFailed)]
    [HttpPost]
    public async Task<IActionResult> DoUnreserve([FromBody] ItemsReservationFailedEvent unreserveItemsRequest)
    {
        _logger.LogInformation($"Unreserve request received: {unreserveItemsRequest.CorrelationId}");

        // TODO: Implement logic to undo the reservation here.
        // e.g., release the items back to the warehouse, update inventory, etc.
        // ???????????!!!!!!!!

        foreach (var item in unreserveItemsRequest.Items)
        {
            _logger.LogInformation($"Unreserving item: {item.ItemType}, Quantity: {item.Quantity}");
            // Your inventory release logic goes here
        }

        return Ok();
    }

    [Topic(WarehouseChannel.Channel, WarehouseChannel.Topics.Shipment)]
    [HttpPost]
    public async Task<IActionResult> DoShippment([FromBody] ShipItemsEvent shipItemsEventRequest)
    {
        _logger.LogInformation($"Payment request received: {shipItemsEventRequest.CorrelationId}");

        var itemsShippedResponse = new ItemsShippedResultEvent
        {
            CorrelationId = shipItemsEventRequest.CorrelationId,
            State = ResultState.Succeeded
        };

        await _daprClient.PublishEventAsync(WorkflowChannel.Channel, WorkflowChannel.Topics.ItemsShippedResult,
            itemsShippedResponse);

        _logger.LogInformation(
            $"Payment processed: {itemsShippedResponse.CorrelationId}, , {itemsShippedResponse.State}");

        return Ok();
    }
}