using Microsoft.EntityFrameworkCore;
using Shopverse.Domain.Entities;

public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> opts) : base(opts) { }

    public DbSet<User> Users { get; set; } = null!;
    public DbSet<OtpVerification> OtpVerifications { get; set; } = null!;
    public DbSet<Role> Roles { get; set; } = null!;
    public DbSet<Attachment> Attachments { get; set; } = null!;
    public DbSet<Setting> Settings { get; set; } = null!;

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<User>(b =>
        {
            b.HasKey(x => x.Id);
            b.HasIndex(x => x.Email).IsUnique();
            b.HasIndex(x => x.Username).IsUnique();
            b.Property(x => x.Email).IsRequired().HasMaxLength(256);
            b.Property(x => x.Username).IsRequired().HasMaxLength(100);

            b.HasOne(u => u.Role)
             .WithMany()
             .HasForeignKey(u => u.RoleId)
             .OnDelete(DeleteBehavior.Cascade);  
        });

        modelBuilder.Entity<OtpVerification>(b =>
        {
            b.HasKey(x => x.Id);
            b.Property(x => x.Code).IsRequired().HasMaxLength(10);
            b.Property(x => x.Purpose).IsRequired();
            b.HasIndex(x => new { x.UserId, x.IsUsed, x.Expiry });
        });

        // Configure Attachment entity
        modelBuilder.Entity<Attachment>(b =>
        {
            b.HasKey(a => a.Id);
            b.Property(a => a.Name).IsRequired();
            b.Property(a => a.FilePath).IsRequired();
            b.Property(a => a.MimeType).IsRequired();
            b.Property(a => a.FileSize).IsRequired();
        });

        modelBuilder.Entity<Role>(b =>
        {
            b.HasKey(r => r.Id);
            b.Property(r => r.Name).IsRequired();
            b.Property(r => r.Description).IsRequired();
            b.HasIndex(r => r.Key).IsUnique(); 
        });

        modelBuilder.Entity<Setting>(b =>
        {
            b.HasKey(s => s.Id);
            b.Property(s => s.Key).IsRequired();
            b.Property(s => s.Value).IsRequired();
        });
    }
}
