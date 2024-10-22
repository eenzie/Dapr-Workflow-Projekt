using Dapr.Workflow;
using OrderService.DaprWorkflow.Workflows.Activities;
using OrderService.DaprWorkflow.Workflows.Activities.CompensatingActivities;
using OrderService.DaprWorkflow.Workflows.External;
using OrderService.Domain;
using Shared.Dtos;
using Shared.IntegrationEvents;

namespace OrderService.DaprWorkflow.Workflows;

public class OrderWorkflow : Workflow<Order, OrderResult>
{
    public override async Task<OrderResult> RunAsync(WorkflowContext context, Order order)
    {
        #region Create order

        var newOrder = order with { Status = OrderStatus.Received, OrderId = context.InstanceId };

        await context.CallActivityAsync(
            nameof(NotifyActivity),
            new Notification($"Received order {order.ShortId} from {order.CustomerDto.Name}.", newOrder));

        #endregion

        #region Reserve Item(s)

        newOrder = newOrder with { Status = OrderStatus.CheckingInventory };

        // Auto conversion Fungere ikke med enum typer...
        //var itemsToReserve = new InventoryRequestDto(newOrder.OrderItems);

        // Inline conversion using LINQ for array of OrderItem to OrderItemDto
        var itemsToReserve = new InventoryRequestDto(
            newOrder.OrderItems.Select(orderItem => new OrderItemDto(
                (ItemTypeDto)orderItem.ItemType,  // Enum conversion
                orderItem.Quantity
            )).ToArray()
        );

        await context.CallActivityAsync(nameof(ReserveItemsActivity), itemsToReserve);

        await context.CallActivityAsync(
            nameof(NotifyActivity),
            new Notification($"Wating for reservation: Order {order.ShortId} from {order.CustomerDto.Name}.", newOrder));

        var reservationResult = await context.WaitForExternalEventAsync<ItemsReservedResultEvent>(
            ExternalEvents.ItemReservedEvent,
            TimeSpan.FromDays(3));

        if (reservationResult.State == ResultState.Failed)
        {
            newOrder = newOrder with { Status = OrderStatus.InsufficientInventory };

            await context.CallActivityAsync(
                nameof(NotifyActivity),
                new Notification(
                    $"Failed: Order {order.ShortId} " +
                    $"from {order.CustomerDto.Name}. " +
                    $"Reservation failed.",
                    newOrder));

            return new OrderResult(newOrder.Status, newOrder, "Reservation failed.");
        }

        newOrder = newOrder with { Status = OrderStatus.SufficientInventory };
        await context.CallActivityAsync(
            nameof(NotifyActivity),
            new Notification($"Reservation Completed: Order {order.ShortId} from {order.CustomerDto.Name}.", newOrder));

        #endregion

        #region Process Payment

        newOrder = newOrder with { Status = OrderStatus.CheckingPayment };

        var paymentDto = new PaymentDto(newOrder.TotalAmount);

        await context.CallActivityAsync(nameof(ProcessPaymentActivity), paymentDto);

        await context.CallActivityAsync(
            nameof(NotifyActivity),
            new Notification($"Wating for Payment: Order {order.ShortId} from {order.CustomerDto.Name}.", newOrder));

        var paymentResult = await context.WaitForExternalEventAsync<PaymentProcessedResultEvent>(
            ExternalEvents.PaymentEvent,
            TimeSpan.FromDays(3));

        if (paymentResult.State == ResultState.Failed)
        {
            newOrder = newOrder with { Status = OrderStatus.PaymentFailing };
            await context.CallActivityAsync(
                nameof(NotifyActivity),
                new Notification($"Failed: Order {order.ShortId} from {order.CustomerDto.Name}. Payment failed.",
                    newOrder));

            // Compensating transaction - Unreserve the items
            await context.CallActivityAsync(
                nameof(UnReserveItemsActivity),
                itemsToReserve // Pass the items to be unreserved
            );

            return new OrderResult(newOrder.Status, newOrder, "Payment failed.");
        }

        newOrder = newOrder with { Status = OrderStatus.PaymentSuccess };
        await context.CallActivityAsync(
            nameof(NotifyActivity),
            new Notification($"Payment Completed: Order {order.ShortId} from {order.CustomerDto.Name}.", newOrder));

        #endregion

        await context.CallActivityAsync(
            nameof(NotifyActivity),
            new Notification($"Completed: Order {order.ShortId} from {order.CustomerDto.Name}.", newOrder));

        return new OrderResult(newOrder.Status, newOrder);
    }
}