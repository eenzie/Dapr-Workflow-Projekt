using Dapr.Workflow;
using OrderService.Domain;
using OrderService.Repository;

namespace OrderService.DaprWorkflow.Workflows.Activities
{
    public class CreateOrderActivity : WorkflowActivity<Order, object?>
    {
        readonly IStateManagementRepository _stateManagement;

        public CreateOrderActivity(IStateManagementRepository stateManagement)
        {
            _stateManagement = stateManagement;
        }

        public override async Task<object?> RunAsync(WorkflowActivityContext context, Order order)
        {
            await _stateManagement.SaveOrderAsync(order);

            return null;
        }
    }
}
