using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using UserTaskJWT.Web.Api.Users;

namespace UserTaskJWT.Web.Api.Data.EntityConfigurations
{
    public class UserEntityConfiguration : IEntityTypeConfiguration<User>
    {
        public void Configure(EntityTypeBuilder<User> builder)
        {
            ArgumentNullException.ThrowIfNull(builder);

            builder.HasKey(u => u.Id);

            builder.Property(u => u.Username).IsRequired();

            builder.Property(u => u.Email).IsRequired();

            builder.Property(u => u.PasswordHash).IsRequired();

            builder.Property(u => u.CreatedAt).IsRequired();

            builder.Property(u => u.UpdatedAt).IsRequired();

            builder.HasMany(u => u.Tasks)
                .WithOne(t => t.User)
                .HasForeignKey(t => t.UserId)
                .IsRequired();
        }
    }
}