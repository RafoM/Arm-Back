using LanguageService.Data.Entity;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Reflection.Emit;

namespace LanguageService.Data
{
    public class LocalizationDbContext : DbContext
    {
        public LocalizationDbContext(DbContextOptions<LocalizationDbContext> options)
        : base(options)
        {
        }

        public DbSet<Language> Languages { get; set; }
        public DbSet<Translation> Translations { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Language>()
                .HasIndex(l => l.CultureCode)
                .IsUnique();

            modelBuilder.Entity<Translation>()
                .HasOne(t => t.Language)
                .WithMany(l => l.Translations)
                .HasForeignKey(t => t.LanguageId);

            base.OnModelCreating(modelBuilder);
        }
    }
}
