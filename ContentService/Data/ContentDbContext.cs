using ContentService.Data.Entity;
using Microsoft.EntityFrameworkCore;

namespace ContentService.Data
{
    public class ContentDbContext : DbContext
    {
        public ContentDbContext(DbContextOptions<ContentDbContext> options)
            : base(options) { }

        public DbSet<Language> Languages { get; set; }
        public DbSet<Page> Pages { get; set; }
        public DbSet<Localization> Localizations { get; set; }
        public DbSet<Translation> Translations { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Language>(entity =>
            {
                entity.ToTable("Languages");
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Id).ValueGeneratedOnAdd();

                entity.HasIndex(e => e.CultureCode).IsUnique();
                entity.Property(e => e.CultureCode)
                      .IsRequired()
                      .HasMaxLength(10);

                entity.Property(e => e.DisplayName)
                      .IsRequired()
                      .HasMaxLength(100);
            });

            modelBuilder.Entity<Page>(entity =>
            {
                entity.ToTable("Pages");
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Id).ValueGeneratedOnAdd();

                entity.HasIndex(e => e.Name).IsUnique();
                entity.Property(e => e.Name)
                      .IsRequired()
                      .HasMaxLength(100);

                entity.Property(e => e.DisplayName)
                      .HasMaxLength(150);
            });

            modelBuilder.Entity<Localization>(entity =>
            {
                entity.ToTable("LocalizationKeys");
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Id).ValueGeneratedOnAdd();

                entity.Property(e => e.Key)
                      .IsRequired()
                      .HasMaxLength(200);

                entity.HasIndex(e => new { e.PageId, e.Key }).IsUnique();

                entity.HasOne(e => e.Page)
                      .WithMany(p => p.LocalizationKeys)
                      .HasForeignKey(e => e.PageId)
                      .OnDelete(DeleteBehavior.Cascade);
            });

            modelBuilder.Entity<Translation>(entity =>
            {
                entity.ToTable("Translations");
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Id).ValueGeneratedOnAdd();

                entity.Property(e => e.Value)
                      .IsRequired();

                entity.HasIndex(e => new { e.LocalizationId, e.LanguageId }).IsUnique();

                entity.HasOne(e => e.Localization)
                      .WithMany(k => k.Translations)
                      .HasForeignKey(e => e.LocalizationId)
                      .OnDelete(DeleteBehavior.Cascade);

                entity.HasOne(e => e.Language)
                      .WithMany(l => l.Translations)
                      .HasForeignKey(e => e.LanguageId)
                      .OnDelete(DeleteBehavior.Cascade);
            });
        }
    }
}
