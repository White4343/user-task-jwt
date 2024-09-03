using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace UserTaskJWT.Web.Api.Data.EntityConfigurations
{
    public class TaskEntityConfiguration : IEntityTypeConfiguration<Tasks.Task>
    {
        public void Configure(EntityTypeBuilder<Tasks.Task> builder)
        {
            ArgumentNullException.ThrowIfNull(builder);

            builder.HasKey(t => t.Id);

            builder.Property(t => t.Title).IsRequired();

            builder.Property(t => t.Status).IsRequired();

            builder.Property(t => t.Priority).IsRequired();

            builder.Property(t => t.CreatedAt).IsRequired();

            builder.Property(t => t.UpdatedAt).IsRequired();

            builder.HasOne(t => t.User)
                .WithMany(u => u.Tasks)
                .HasForeignKey(t => t.UserId)
                .IsRequired();
        }
    }
}