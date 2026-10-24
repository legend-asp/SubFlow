using MediatR;
using Microsoft.AspNetCore.Mvc;
using SubFlow.Api.Features.Billing.Commands;

namespace SubFlow.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class WebhooksController(IMediator mediator) : ControllerBase
    {
        [HttpPost("stripe")]
        public async Task<IActionResult> StripeWebhook([FromBody] ProcessStripeWebhookCommand command)
        {
            await mediator.Send(command);
            return Ok();
        }
    }
}