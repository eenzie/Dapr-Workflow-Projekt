using Dapr;
using Dapr.Client;
using Microsoft.AspNetCore.Mvc;
using Shared.IntegrationEvents;
using Shared.Queues;

namespace PaymentService.Controllers;

[Route("api/[controller]/[action]")]
[ApiController]
public class PaymentController : ControllerBase
{
	private readonly DaprClient _daprClient;
	private readonly ILogger<PaymentController> _logger;

	public PaymentController(DaprClient daprClient, ILogger<PaymentController> logger)
	{
		_daprClient = daprClient;
		_logger = logger;
	}

	[Topic(PaymentChannel.Channel, PaymentChannel.Topics.Payment)]
	[HttpPost]
	public async Task<IActionResult> DoPayment([FromBody] ProcessPaymentEvent paymentRequest)
	{
		_logger.LogInformation("Payment request received: {CorrelationId}, {Amount}", paymentRequest.CorrelationId,
			paymentRequest.Amount);

		var paymentResponse = new PaymentProcessedResultEvent
		{
			CorrelationId = paymentRequest.CorrelationId,
			Amount = paymentRequest.Amount,
			State = ResultState.Succeeded
		};

		await _daprClient.PublishEventAsync(WorkflowChannel.Channel, WorkflowChannel.Topics.PaymentResult,
			paymentResponse);

		_logger.LogInformation("Payment processed: {CorrelationId}, {Amount}, {State}", paymentResponse.CorrelationId,
			paymentResponse.Amount, paymentResponse.State);

		return Ok();
	}
}