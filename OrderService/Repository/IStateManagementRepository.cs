using OrderService.Domain;

namespace OrderService.Repository;

public interface IStateManagementRepository
{
    Task SaveOrderAsync(Order order);
}