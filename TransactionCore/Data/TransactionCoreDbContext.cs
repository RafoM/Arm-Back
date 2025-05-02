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
        public DbSet<UserInfo> UserInfos { get; set; }
        public DbSet<Promo> Promos { get; set; }
        public DbSet<PromoUsage> PromoUsages { get; set; }
        public DbSet<SubscriptionUsage> SubscriptionUsages { get; set; }
        public DbSet<ReferralRoleRewardConfig> ReferralRoleRewardConfigs { get; set; }
        public DbSet<RemainderInfo> RemainderInfos { get; set; }
        public DbSet<ReferralPayment> ReferralPayments { get; set; }
        public DbSet<ReferralActivity> ReferralActivities { get; set; }
        public DbSet<ReferralWithdrawal> ReferralWithdrawals { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<ReferralPayment>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(p => p.Id).HasDefaultValueSql("NEWID()");

                entity.Property(e => e.CreatedAt)
                    .IsRequired();

                entity.HasOne(e => e.Payment)
                    .WithMany()
                    .HasForeignKey(e => e.PaymentId)
                    .OnDelete(DeleteBehavior.Restrict);



                entity.HasOne(e => e.ReferrerUserInfo)
                    .WithMany()
                    .HasForeignKey(e => e.ReferrerUserInfoId)
                    .OnDelete(DeleteBehavior.Restrict);
            });

            modelBuilder.Entity<ReferralWithdrawal>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(p => p.Id).HasDefaultValueSql("NEWID()");
                entity.Property(p => p.Amount).HasPrecision(18, 4);
                entity.HasOne(e => e.ReferrerUserInfo)
                    .WithMany()
                    .HasForeignKey(e => e.ReferrerUserInfoId)
                    .OnDelete(DeleteBehavior.Restrict);
            });

            modelBuilder.Entity<ReferralActivity>(e =>
            {
                e.HasKey(r => r.Id);
                e.Property(p => p.Id).HasDefaultValueSql("NEWID()");

                e.Property(r => r.Action).IsRequired();

                e.HasOne(r => r.ReferredUserInfo)
                 .WithMany()
                 .HasForeignKey(r => r.ReferredUserInfoId)
                 .OnDelete(DeleteBehavior.Restrict);

                e.HasOne(r => r.Payment)
                 .WithMany()
                 .HasForeignKey(r => r.PaymentId)
                 .OnDelete(DeleteBehavior.SetNull);
            });

            modelBuilder.Entity<ReferralRoleRewardConfig>(p =>
            {
                p.HasKey(s => s.Id);
                p.Property(p => p.Id).HasDefaultValueSql("NEWID()");
            });

           

            modelBuilder.Entity<RemainderInfo>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Id).HasDefaultValueSql("NEWID()");
                entity.Property(p => p.Amount).HasPrecision(18, 4);
                entity.HasOne(e => e.Wallet)
                      .WithMany()
                      .HasForeignKey(e => e.WalletId)
                      .OnDelete(DeleteBehavior.Restrict);
            });


            modelBuilder.Entity<ReferralRoleRewardConfig>(p =>
            {
                p.HasKey(s => s.Id);
                p.Property(p => p.Id).HasDefaultValueSql("NEWID()");
            });
            

            modelBuilder.Entity<SubscriptionUsage>(p =>
            {
                p.HasKey(s => s.Id);
                p.Property(p => p.Id).HasDefaultValueSql("NEWID()");
            });


            modelBuilder.Entity<SubscriptionUsage>()
                .HasOne(s => s.UserInfo)
                .WithMany()
                .HasForeignKey(s => s.UserInfoId);

            modelBuilder.Entity<SubscriptionUsage>()
                .HasOne(s => s.SubscriptionPackage)
                .WithMany()
                .HasForeignKey(s => s.SubscriptionPackageId);


            modelBuilder.Entity<Promo>(entity =>
                {
                    entity.HasKey(p => p.Id);
                    entity.Property(p => p.Id).HasDefaultValueSql("NEWID()");
                    entity.HasIndex(p => p.Code).IsUnique();
                    entity.Property(p => p.Code).IsRequired();
                    entity.Property(p => p.DiscountPercent).HasPrecision(5, 2);
                    entity.Property(p => p.IsActive).HasDefaultValue(true);
                });

            modelBuilder.Entity<PromoUsage>(entity =>
            {
                entity.HasKey(u => u.Id);
                entity.Property(u => u.Id).HasDefaultValueSql("NEWID()");
                entity.HasOne(u => u.Promo)
                      .WithMany(p => p.Usages)
                      .HasForeignKey(u => u.PromoId)
                      .OnDelete(DeleteBehavior.Cascade);
            });

            modelBuilder.Entity<UserInfo>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Id).HasDefaultValueSql("NEWID()");
                entity.Property(p => p.ReferalBalance).HasPrecision(18, 4);
                entity.Property(p => p.Balance).HasPrecision(18, 4);

            });

            modelBuilder.Entity<Payment>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Id).HasDefaultValueSql("NEWID()");
                entity.Property(p => p.ExpectedFee).HasPrecision(18, 4);
                entity.HasOne(e => e.Wallet)
                      .WithMany()
                      .HasForeignKey(e => e.WalletId)
                      .OnDelete(DeleteBehavior.Restrict);
                entity.HasOne(r => r.UserInfo)
                 .WithMany()
                 .HasForeignKey(r => r.UserInfoId)
                 .OnDelete(DeleteBehavior.Cascade);
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
                entity.Property(p => p.Price).HasPrecision(18, 4);
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
                entity.Property(p => p.TransactionFee).HasPrecision(18, 4);
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
