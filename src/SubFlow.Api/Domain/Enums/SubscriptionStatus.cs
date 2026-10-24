namespace SubFlow.Api.Domain.Enums
{
    public enum SubscriptionStatus
    {
        Unknown = 0,
        Trialing = 1,
        Active = 2,
        PastDue = 3,
        Canceled = 4
    }
}