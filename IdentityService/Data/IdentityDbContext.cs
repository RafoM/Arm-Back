using IdentityService.Data.Entity;
using Microsoft.EntityFrameworkCore;

namespace IdentityService.Data
{
    public class IdentityDbContext : DbContext
    {
        public IdentityDbContext(DbContextOptions<IdentityDbContext> options)
           : base(options) { }

        public DbSet<User> Users { get; set; }
        public DbSet<Role> Roles { get; set; }
        public DbSet<RefreshToken> RefreshTokens { get; set; }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<User>(entity =>
            {
                entity.HasKey(x => x.Id);
                entity.Property(x => x.Id).HasDefaultValueSql("NEWID()");
                entity.HasIndex(x => x.Email).IsUnique();

                entity.HasOne(x => x.Role)
                      .WithMany()
                      .HasForeignKey(x => x.RoleId)
                      .IsRequired();
            });

            modelBuilder.Entity<RefreshToken>(entity =>
            {
                entity.HasKey(x => x.Id);
                entity.Property(x => x.Id).HasDefaultValueSql("NEWID()");
                entity.HasOne(x => x.User)
                      .WithMany()
                      .HasForeignKey(x => x.UserId);
            });


            modelBuilder.Entity<Role>(entity =>
            {
                entity.HasKey(x => x.Id);

                entity.Property(x => x.Name).IsRequired();
                entity.HasIndex(x => x.Name).IsUnique();

                entity.HasData(
                    new Role { Id = 1, Name = "Admin" },
                    new Role { Id = 2, Name = "User" },
                    new Role { Id = 3, Name = "Pro" },
                    new Role { Id = 4, Name = "EarlyAccess" }
                );
            });

            base.OnModelCreating(modelBuilder);
        }
    }
}
