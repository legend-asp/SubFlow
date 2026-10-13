using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SubFlow.Api.Domain.Entities;
using System.ComponentModel.DataAnnotations;
using System.Reflection.Emit;

namespace SubFlow.Api.Infrastructure.Persistence.Configurations
{
    public class UserConfiguration : IEntityTypeConfiguration<User>
    {
        public void Configure(EntityTypeBuilder<User> builder)
        {
            builder.HasKey(u => u.Id);
              
                builder.Property(u => u.Email)
                .IsRequired()
                .HasMaxLength(256);

                builder.HasIndex(u => u.Email)
                .IsUnique();

                builder.Property(u => u.PasswordHash)
                .IsRequired()
                .HasMaxLength(256);

                builder.Property(u => u.Role)
                .HasConversion<string>()
                .HasMaxLength(50);

            builder.HasMany(u => u.RefreshTokens)
                .WithOne(t => t.User)
                .HasForeignKey(i => i.UserId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}