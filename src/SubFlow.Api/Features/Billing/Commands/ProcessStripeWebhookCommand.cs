using MediatR;

namespace SubFlow.Api.Features.Billing.Commands;

public record ProcessStripeWebhookCommand(
    string EventId,
    string EventType,
    Guid UserId,
    Guid PlanId
) : IRequest;