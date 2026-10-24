using SubFlow.Api.Domain.Enums;

namespace SubFlow.Api.Domain.Entities
{
    public class User
    {
        public Guid Id { get; set; }
        public UserRole Role { get; set; } = UserRole.Customer;
        public string Email { get; set; } = string.Empty;
        public string PasswordHash { get; set; } = string.Empty;
        public DateTime CreatedAtUtc { get; set; }

        public virtual ICollection<RefreshToken> RefreshTokens { get; set; } = new List<RefreshToken>();
        public virtual ICollection<Subscription> Subscriptions { get; set; } = new List<Subscription>();
    }
}