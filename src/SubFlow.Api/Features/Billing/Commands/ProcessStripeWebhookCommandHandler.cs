using MediatR;
using Microsoft.EntityFrameworkCore;
using SubFlow.Api.Domain.Entities;
using SubFlow.Api.Domain.Enums;
using SubFlow.Api.Infrastructure.Persistence;

namespace SubFlow.Api.Features.Billing.Commands;

public class ProcessStripeWebhookCommandHandler(AppDbContext context) : IRequestHandler<ProcessStripeWebhookCommand>
{
    public async Task Handle(ProcessStripeWebhookCommand command, CancellationToken cancellationToken)
    {
        // 1. Проверка идемпотентности: событие уже обрабатывалось?
        var alreadyProcessed = await context.ProcessedWebhookEvents
            .AnyAsync(e => e.EventId == command.EventId, cancellationToken);

        if (alreadyProcessed)
        {
            return; // Дубликат игнорируем
        }

        // 2. Обработка успешной оплаты подписки
        if (command.EventType == "invoice.payment_succeeded")
        {
            var subscription = await context.Subscriptions
                .FirstOrDefaultAsync(s => s.UserId == command.UserId, cancellationToken);

            if (subscription == null)
            {
                // Создаем новую активную подписку
                var newSubscription = new Subscription
                {
                    Id = Guid.NewGuid(),
                    UserId = command.UserId,
                    PlanId = command.PlanId,
                    Status = SubscriptionStatus.Active,
                    CurrentPeriodStartUtc = DateTime.UtcNow,
                    CurrentPeriodEndUtc = DateTime.UtcNow.AddDays(30)
                };

                context.Subscriptions.Add(newSubscription);
            }
            else
            {
                // Продлеваем существующую
                subscription.Status = SubscriptionStatus.Active;
                var baseDate = subscription.CurrentPeriodEndUtc > DateTime.UtcNow
                    ? subscription.CurrentPeriodEndUtc
                    : DateTime.UtcNow;

                subscription.CurrentPeriodEndUtc = baseDate.AddDays(30);
            }

            // 3. Фиксируем обработанный вебхук
            var webhookEvent = new ProcessedWebhookEvent
            {
                Id = Guid.NewGuid(),
                EventId = command.EventId,
                EventType = command.EventType,
                ProcessedAtUtc = DateTime.UtcNow
            };

            context.ProcessedWebhookEvents.Add(webhookEvent);

            // 4. Сохраняем всё в PostgreSQL
            await context.SaveChangesAsync(cancellationToken);
        }
    }
}