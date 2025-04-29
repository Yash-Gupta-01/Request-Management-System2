using Microsoft.EntityFrameworkCore;
using RMS.Models;

namespace RMS.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<UserDetails> Users { get; set; } = null!;
        public DbSet<RequestDetails> Requests { get; set; } = null!;
        public DbSet<RequestStatusHistory> RequestStatusHistories { get; set; } = null!;

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<RequestDetails>()
                .HasOne(r => r.User)
                .WithMany()
                .HasForeignKey(r => r.Username)
                .IsRequired();
        }
    }
}
