using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
namespace MvcProject.Models
{
    public class EntityTypeConfiguration:IEntityTypeConfiguration<User>
    {
        public void Configure(EntityTypeBuilder<User> builder)
        {
            builder.HasAlternateKey(user => user.Email);
            builder.HasKey(user => user.Id).HasName("PK_Users");
            builder.HasIndex(user => user.Email).HasDatabaseName("IDX_Users");
            builder.Property(user => user.Timestamp).IsRowVersion();
            builder.HasData(new User() { Email = "xamacoredevelopment@gmail.com",IsAlive=true, PasswordHash = SecureData.HashPassword(SecureData.AdminPassword), Id = 1 });
            builder.HasQueryFilter(user => user.IsAlive);
        }
    }
}
