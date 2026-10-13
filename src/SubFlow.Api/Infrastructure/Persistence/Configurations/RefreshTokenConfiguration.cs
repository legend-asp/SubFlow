using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SubFlow.Api.Domain.Entities;

namespace SubFlow.Api.Infrastructure.Persistence.Configurations
{
    public class RefreshTokenConfiguration : IEntityTypeConfiguration<RefreshToken>
    {
        public void Configure(EntityTypeBuilder<RefreshToken> builder)
        {
            builder.HasKey(rt => rt.Id);

            builder.Property(u => u.Token)
            .IsRequired()
            .HasMaxLength(500);
            
            builder.HasIndex(rt => rt.Token);
            
            builder.Property(i => i.ReplacedByToken)
                .IsRequired(false)
                .HasMaxLength(500);
        }
    }
}
