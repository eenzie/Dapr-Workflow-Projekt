﻿using Dapr.Workflow;
using OrderService.Domain;
using OrderService.Repository;

namespace OrderService.DaprWorkflow.Workflows.Activities.CompensatingActivities;

public class IssueRefundActivity : WorkflowActivity<Order, OrderResult>
{
    private readonly IStateManagementRepository _stateManagement;

    public IssueRefundActivity(IStateManagementRepository stateManagement)
    {
        _stateManagement = stateManagement;
    }

    public override async Task<OrderResult> RunAsync(WorkflowActivityContext context, Order order)
    {
        await _stateManagement.SaveOrderAsync(order);

        return new OrderResult(OrderStatus.Received, order);
    }
}