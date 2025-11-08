using Microsoft.EntityFrameworkCore;
using Shopverse.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Net.Mail;
using System.Reflection.Emit;
using System.Reflection.Metadata;
using System.Security;
using System.Text;
using System.Threading.Tasks;
using System.Transactions;
using static System.Runtime.InteropServices.JavaScript.JSType;
using Attachment = Shopverse.Domain.Entities.Attachment;
namespace Shopverse.Infrastructure.Context
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> opts) : base(opts) { }

        public DbSet<User> Users { get; set; } = null!;
        public DbSet<OtpVerification> OtpVerifications { get; set; } = null!;
        public DbSet<Role> Roles { get; set; }
        public DbSet<UserRole> UserRoles { get; set; }
        public DbSet<Attachment> Attachments { get; set; }

        public DbSet<Setting> Settings { get; set; }
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
                b.Property(x => x.Name).IsRequired().HasMaxLength(200);
            });

            modelBuilder.Entity<OtpVerification>(b =>
            {
                b.HasKey(x => x.Id);
                b.Property(x => x.Code).IsRequired().HasMaxLength(10);
                b.Property(x => x.Purpose).IsRequired();
                b.HasIndex(x => new { x.UserId, x.IsUsed, x.Expiry });
            });

            modelBuilder.Entity<UserRole>()
          .HasKey(ur => new { ur.UserId, ur.RoleId });

            modelBuilder.Entity<UserRole>()
                .HasOne(ur => ur.User)
                .WithMany(u => u.UserRoles)
                .HasForeignKey(ur => ur.UserId);

            modelBuilder.Entity<UserRole>()
                .HasOne(ur => ur.Role)
                .WithMany(r => r.UserRoles)
                .HasForeignKey(ur => ur.RoleId);

            modelBuilder.Entity<Domain.Entities.Attachment>(b =>
            {
                b.HasKey(a => a.Id);
                b.Property(a => a.Name).IsRequired();
                b.Property(a => a.FilePath).IsRequired();
            });


        }
    }
}
