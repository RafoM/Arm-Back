using ContentService.Data.Entity;
using Microsoft.EntityFrameworkCore;

namespace ContentService.Data
{
    public class ContentDbContext : DbContext
    {
        public ContentDbContext(DbContextOptions<ContentDbContext> options)
        : base(options)
        {
        }

        public DbSet<Language> Languages { get; set; }
        public DbSet<Translation> Translations { get; set; }
        //public DbSet<Blog> Blogs { get; set; }
        //public DbSet<Tutorial> Tutorials { get; set; }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Language>(entity =>
            {
                entity.HasKey(l => l.Id);
                entity.Property(l => l.Id)
                      .ValueGeneratedOnAdd();
                entity.HasIndex(l => l.Code).IsUnique();
            });

            modelBuilder.Entity<Translation>(entity =>
            {
                entity.HasKey(t => t.Id);
                entity.Property(t => t.Id)
                      .ValueGeneratedOnAdd();
            });

            //modelBuilder.Entity<Blog>(entity =>
            //{
            //    entity.HasKey(b => b.Id);
            //    entity.Property(b => b.Id)
            //          .ValueGeneratedOnAdd();
            //    entity.HasOne(b => b.Language)
            //          .WithMany()
            //          .HasForeignKey(b => b.LanguageId);
            //});

            //modelBuilder.Entity<Tutorial>(entity =>
            //{
            //    entity.HasKey(t => t.Id);
            //    entity.Property(t => t.Id)
            //          .ValueGeneratedOnAdd();
            //    entity.HasOne(t => t.Language)
            //          .WithMany()
            //          .HasForeignKey(t => t.LanguageId);
            //});

            base.OnModelCreating(modelBuilder);
        }
    }
}
