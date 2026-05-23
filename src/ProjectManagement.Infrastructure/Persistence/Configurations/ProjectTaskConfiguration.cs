using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ProjectManagement.Domain.Projects;
using ProjectManagement.Infrastructure.Identity;

namespace ProjectManagement.Infrastructure.Persistence.Configurations;

public sealed class ProjectTaskConfiguration : IEntityTypeConfiguration<ProjectTask>
{
    public void Configure(EntityTypeBuilder<ProjectTask> builder)
    {
        builder.ToTable("ProjectTasks");

        builder.HasKey(task => task.Id);

        builder.Property(task => task.Id)
            .ValueGeneratedNever();

        builder.Property(task => task.Title)
            .HasMaxLength(160)
            .IsRequired();

        builder.Property(task => task.Description)
            .HasMaxLength(1000);

        builder.Property(task => task.AssignedEmployeeId)
            .HasMaxLength(450)
            .IsRequired();

        builder.Property(task => task.AssignedEmployeeName)
            .HasMaxLength(200)
            .IsRequired();

        builder.Property(task => task.Status)
            .HasConversion<int>()
            .IsRequired();

        builder.Property(task => task.CreatedAt)
            .IsRequired();

        builder.Property(task => task.RowVersion)
            .IsRowVersion();

        builder.HasIndex(task => task.AssignedEmployeeId)
            .HasDatabaseName("IX_ProjectTasks_AssignedEmployeeId");

        builder.HasOne<ApplicationUser>()
            .WithMany()
            .HasForeignKey(task => task.AssignedEmployeeId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
