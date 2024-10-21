namespace OrderService.DaprWorkflow.Workflows.External;

public class ExternalEvents
{
    public static readonly string PaymentEvent = "PaymentEvent";
    public static readonly string ItemReservedEvent = "ItemReservedEvent";
    public static readonly string ItemShippedEvent = "ItemShippedEvent";
}