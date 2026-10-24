namespace SubFlow.Api.Domain.Entities
{
    public class ProcessedWebhookEvent
    {
        public Guid Id { get; set; }
        public string EventId { get; set; } = string.Empty;
        public string EventType { get; set; } = string.Empty;
        public DateTime ProcessedAtUtc { get; set; }
    }
}