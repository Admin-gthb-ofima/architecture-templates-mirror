using Domain.Entities;
using Infraestructure.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Infraestructure.Data;

public class AppDbContext : IdentityDbContext<ApplicationUser>
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
    }

    public DbSet<Applicant> Applicants => Set<Applicant>();
    public DbSet<Project> Projects => Set<Project>();
    public DbSet<ChangeRequest> ChangeRequests => Set<ChangeRequest>();
    public DbSet<CommitteeMember> CommitteeMembers => Set<CommitteeMember>();
    public DbSet<ChangeRequestStage> ChangeRequestStages => Set<ChangeRequestStage>();
    public DbSet<ChangeRequestAttachment> ChangeRequestAttachments => Set<ChangeRequestAttachment>();

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        builder.Entity<Applicant>(e =>
        {
            e.Property(x => x.FirstName).HasMaxLength(100).IsRequired();
            e.Property(x => x.LastName).HasMaxLength(100).IsRequired();
            e.Property(x => x.Email).HasMaxLength(200).IsRequired();
            e.Property(x => x.Position).HasMaxLength(150);
            e.Property(x => x.Department).HasMaxLength(150);
            e.HasIndex(x => x.Email).IsUnique();
        });

        builder.Entity<Project>(e =>
        {
            e.Property(x => x.Name).HasMaxLength(200).IsRequired();
        });

        builder.Entity<ChangeRequest>(e =>
        {
            e.Property(x => x.TicketNumber).HasMaxLength(30).IsRequired();
            e.Property(x => x.RequestType).HasMaxLength(100).IsRequired();
            e.Property(x => x.Status).HasMaxLength(30).IsRequired();
            e.Property(x => x.Priority).HasMaxLength(30).IsRequired();
            e.HasIndex(x => x.TicketNumber).IsUnique();
            e.HasOne(x => x.Project)
                .WithMany(x => x.ChangeRequests)
                .HasForeignKey(x => x.ProjectId)
                .OnDelete(DeleteBehavior.Restrict);
            e.HasOne(x => x.Applicant)
                .WithMany(x => x.ChangeRequests)
                .HasForeignKey(x => x.ApplicantId)
                .OnDelete(DeleteBehavior.Restrict);
        });

        builder.Entity<CommitteeMember>(e =>
        {
            e.Property(x => x.Name).HasMaxLength(150).IsRequired();
            e.Property(x => x.Role).HasMaxLength(150);
            e.Property(x => x.Department).HasMaxLength(150);
            e.Property(x => x.Email).HasMaxLength(200);
            e.Property(x => x.AvatarUrl).HasMaxLength(400);
        });

        builder.Entity<ChangeRequestStage>(e =>
        {
            e.Property(x => x.StageName).HasMaxLength(80).IsRequired();
            e.Property(x => x.Status).HasMaxLength(30).IsRequired();
            e.Property(x => x.Note).HasMaxLength(1000);
            e.HasIndex(x => new { x.ChangeRequestId, x.Sequence }).IsUnique();
            e.HasOne(x => x.ChangeRequest)
                .WithMany(x => x.Stages)
                .HasForeignKey(x => x.ChangeRequestId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        builder.Entity<ChangeRequestAttachment>(e =>
        {
            e.Property(x => x.FileName).HasMaxLength(260).IsRequired();
            e.Property(x => x.ContentType).HasMaxLength(120).IsRequired();
            e.HasOne(x => x.ChangeRequest)
                .WithMany(x => x.Attachments)
                .HasForeignKey(x => x.ChangeRequestId)
                .OnDelete(DeleteBehavior.Cascade);
        });
    }
}
