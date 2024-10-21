namespace Shared.IntegrationEvents;

public record IntegrationEventOutgoing
{
    public string CorrelationId { get; init; } = String.Empty;
}

public record ProcessPaymentEvent : IntegrationEventOutgoing
{
    public decimal Amount { get; init; }
}

public record ReserveItemsEvent : IntegrationEventOutgoing
{
    public List<ItemDto> Items { get; init; } = new();
    public int Quantity { get; init; }
}

public record ItemDto
{
    public string ItemType { get; init; } = String.Empty;
    public int Quantity { get; init; }
}

public record ShipItemsEvent : IntegrationEventOutgoing
{
    public List<ItemDto> Items { get; init; } = new();
    public int Quantity { get; init; }
}