namespace SubFlow.Api.Domain.Entities
{
    public class Plan
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public int PriceInCents { get; set; }
        public string Currency { get; set; } = string.Empty;
        public string BillingInterval { get; set; } = string.Empty;
    }
}