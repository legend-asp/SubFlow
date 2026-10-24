using SubFlow.Api.Domain.Enums;

namespace SubFlow.Api.Domain.Entities
{
    public class Subscription
    {
        public Guid Id { get; set; }
        public Guid UserId { get; set; }
        public User User { get; set; } = null!;
        public Guid PlanId { get; set; }
        public Plan Plan { get; set; } = null!;
        public SubscriptionStatus Status { get; set; }
        public DateTime CurrentPeriodStartUtc { get; set; }
        public DateTime CurrentPeriodEndUtc  { get; set; }
        public bool CancelAtPeriodEnd { get; set; }
    }
}
