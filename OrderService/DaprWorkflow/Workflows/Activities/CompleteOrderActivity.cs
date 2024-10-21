namespace OrderService.DaprWorkflow.Workflows.Activities;

//TODO: Det er slutningen af flowet

//public class CompleteOrderActivity : WorkflowActivity<Order, object?>
//{
//    readonly IStateManagementRepository _stateManagement;

//    public CompleteOrderActivity(IStateManagementRepository stateManagement)
//    {
//        _stateManagement = stateManagement;
//    }

//    public override async Task<object?> RunAsync(WorkflowActivityContext context, Order order)
//    {
//        await _stateManagement.SaveOrderAsync(order);

//        return null;
//    }
//}