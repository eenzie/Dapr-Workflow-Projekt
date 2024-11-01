using Shared.Dtos;

namespace Shared.IntegrationEvents;

public abstract record IntegrationEventIncoming
{
    public string CorrelationId { get; init; } = String.Empty;
    public ResultState State { get; init; } = ResultState.Failed;
}

public enum ResultState
{
    Succeeded,
    Failed
}

public record PaymentProcessedResultEvent : IntegrationEventIncoming
{
    public decimal Amount { get; init; }

}

public record ItemsReservedResultEvent : IntegrationEventIncoming
{
}

public record ItemsShippedResultEvent : IntegrationEventIncoming
{
}

public record OrderCompletedEvent : IntegrationEventIncoming
{
}

// Inherits intergration events properties and adds additional property
public abstract record FailedEvent : IntegrationEventIncoming
{
    public string Reason { get; init; } = string.Empty;
}

public record OrderFailedEvent : FailedEvent
{
}

public record PaymentFailedEvent : FailedEvent
{
}

public record ItemsReservationFailedEvent : FailedEvent
{
    public List<OrderItemDto> Items { get; init; } = new();
}

public record ItemsShipmentFailedEvent : FailedEvent
{
}