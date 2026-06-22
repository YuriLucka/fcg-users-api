using Users.Domain.Entities;
using Users.Domain.ValueObjects;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Users.Infrastructure.Data.Mappings
{
    public class UserMapping : IEntityTypeConfiguration<User>
    {
        public void Configure(EntityTypeBuilder<User> builder)
        {
            builder.ToTable("Users");

            builder.HasKey(u => u.Id);

            builder.Property(u => u.Id)
                .ValueGeneratedNever();

            builder.Property(u => u.Name)
                .IsRequired()
                .HasMaxLength(100);

            // Mapeia o Value Object Email como uma coluna string
            builder.Property(u => u.Email)
                .IsRequired()
                .HasMaxLength(200)
                .HasConversion(
                    email => email.Value,
                    value => new Email(value));

            builder.HasIndex(u => u.Email).IsUnique();

            builder.Property(u => u.PasswordHash)
                .IsRequired();

            builder.Property(u => u.Role)
                .IsRequired()
                .HasConversion<int>();

            builder.Property(u => u.CreatedAt)
                .IsRequired();
        }
    }
}
