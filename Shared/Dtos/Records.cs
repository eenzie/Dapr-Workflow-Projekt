using System.Text.Json.Serialization;

namespace Shared.Dtos;

public record OrderItemDto(ItemTypeDto ItemType, int Quantity)
{
	public OrderItemDto() : this(default, 1)
	{
	}

	//// Implicit conversion from OrderItem to OrderItemDto
	//public static implicit operator OrderItemDto(OrderItem orderItem)
	//{
	//	return new OrderItemDto(new ItemTypeDto(orderItem.ItemType.Id, orderItem.ItemType.Name), orderItem.Quantity);
	//}
}

public record OrderDto(
	string OrderId,
	OrderItemDto[] OrderItems,
	DateTime OrderDate,
	CustomerDto CustomerDto,
	OrderStatusDto Status = OrderStatusDto.Received)
{
	public string ShortId => OrderId.Substring(0, 8);
}

public record CustomerDto(string Name, string Email);

public record PaymentDto(double Amount);

public record InventoryRequestDto(OrderItemDto[] ItemsRequested);



// TODO: Er et event, så skal nok ikke bruges. Gem for at være sikker...
public record InventoryResultDto(bool IsSufficientInventory, OrderItemDto[] ItemsInStock);


[JsonConverter(typeof(JsonStringEnumConverter))]
public enum ItemTypeDto
{
	Computer = 1,
	Monitor = 2,
	Keyboard = 3,
	Mouse = 4
}

[JsonConverter(typeof(JsonStringEnumConverter))]
public enum OrderStatusDto
{
	Received = 0,
	CheckingInventory = 1,
	SufficientInventory = 2,
	InsufficientInventory = 3,
	CheckingPayment = 4,
	PaymentFailing = 5,
	ShipingItems = 6,
	ShipingItemsFailing = 7,
	Error = 8
}