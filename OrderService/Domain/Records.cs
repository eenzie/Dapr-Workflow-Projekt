using System.Text.Json.Serialization;

namespace OrderService.Domain;

public record OrderItem(ItemType ItemType, int Quantity)
{
	public OrderItem() : this(default, 1)
	{
	}
}

public record Customer(string Name, string Email);

public record OrderResult(OrderStatus Status, Order Order, string? Message = null);

public record Notification(string Message, Order Order);

[JsonConverter(typeof(JsonStringEnumConverter))]
public enum ItemType
{
	Computer = 1,
	Monitor = 2,
	Keyboard = 3,
	Mouse = 4
}