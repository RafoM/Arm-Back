using Microsoft.EntityFrameworkCore;
using TransactionCore.Data.Entity;

namespace TransactionCore.Data
{
    public class TransactionCoreDbContext : DbContext
    {
        public TransactionCoreDbContext(DbContextOptions<TransactionCoreDbContext> options)
        : base(options)
        {
        }

        public DbSet<SubscriptionPackage> SubscriptionPackages { get; set; }
        public DbSet<PaymentMethod> PaymentMethods { get; set; }
        public DbSet<Crypto> Cryptos { get; set; }
        public DbSet<Network> Networks { get; set; }
        public DbSet<Wallet> Wallets { get; set; }  
        public DbSet<Payment> Payments { get; set; }
        public DbSet<UserFinance> UserFinances { get; set; }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<UserFinance>(entity =>
            {
                entity.HasKey(e =>  e.Id);
                entity.Property(e => e.Id).HasDefaultValueSql("NEWID()");
            });

            modelBuilder.Entity<Payment>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Id).HasDefaultValueSql("NEWID()");
                entity.HasOne(e => e.Wallet)
                      .WithMany()
                      .HasForeignKey(e => e.WalletId)
                      .OnDelete(DeleteBehavior.Restrict);
            });

            modelBuilder.Entity<Wallet>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Id).HasDefaultValueSql("NEWID()");
                entity.HasOne(e => e.PaymentMethod)
                      .WithMany()
                      .HasForeignKey(e => e.PaymentMethodId)
                      .OnDelete(DeleteBehavior.Restrict);
            });

            modelBuilder.Entity<SubscriptionPackage>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Id).HasDefaultValueSql("NEWID()");
                entity.Property(e => e.Name)
                      .IsRequired();
            });
            modelBuilder.Entity<Crypto>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(x => x.Id).HasDefaultValueSql("NEWID()");
                entity.Property(e => e.Name).IsRequired();
            });

            
            modelBuilder.Entity<Network>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(x => x.Id).HasDefaultValueSql("NEWID()");
                entity.Property(e => e.Name).IsRequired();
            });

            
            modelBuilder.Entity<PaymentMethod>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(x => x.Id).HasDefaultValueSql("NEWID()");

                entity.HasOne(pm => pm.Crypto)
                      .WithMany() 
                      .HasForeignKey(pm => pm.CryptoId)
                      .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(pm => pm.Network)
                      .WithMany() 
                      .HasForeignKey(pm => pm.NetworkId)
                      .OnDelete(DeleteBehavior.Restrict);
            });
        }
    }
}
