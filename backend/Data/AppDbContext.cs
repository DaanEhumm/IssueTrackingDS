using Microsoft.EntityFrameworkCore;
using IssueTrackingDS.Models;

namespace IssueTrackingDS.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }

        public DbSet<User> Users { get; set; }
        public DbSet<Ticket> Tickets { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

    // Configure ENUMs als strings
    modelBuilder.Entity<User>()
        .Property(u => u.Role)
        .HasConversion<string>();

    modelBuilder.Entity<Ticket>()
        .Property(t => t.Status)
        .HasConversion<string>();

    modelBuilder.Entity<Ticket>()
        .Property(t => t.Priority)
        .HasConversion<string>();

    // 🔹 Configureer AssignedUser relatie
    modelBuilder.Entity<Ticket>()
        .HasOne(t => t.AssignedUser)
        .WithMany(u => u.AssignedTickets)
        .HasForeignKey(t => t.AssignedTo)
        .OnDelete(DeleteBehavior.SetNull);

    // 🔹 Configureer Creator relatie
    modelBuilder.Entity<Ticket>()
        .HasOne(t => t.Creator)
        .WithMany(u => u.CreatedTickets)
        .HasForeignKey(t => t.CreatedBy)
        .OnDelete(DeleteBehavior.Cascade);
}
    }
}