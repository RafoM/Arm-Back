using ContentService.Data.Entity;
using Microsoft.EntityFrameworkCore;

namespace ContentService.Data
{
    public class ContentDbContext : DbContext
    {
        public ContentDbContext(DbContextOptions<ContentDbContext> options)
            : base(options) { }

        public DbSet<Language> Languages { get; set; }
        public DbSet<Localization> Localizations { get; set; }
        public DbSet<Translation> Translations { get; set; }
        public DbSet<Blog> Blogs { get; set; }
        public DbSet<BlogTag> BlogTags { get; set; }
        public DbSet<Case> Cases { get; set; }
        public DbSet<CaseTag> CaseTags { get; set; }
        public DbSet<Tutorial> Tutorials { get; set; }
        public DbSet<Lesson> Lessons { get; set; }
        //rolepermission
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {

            modelBuilder.Entity<Tutorial>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Id).ValueGeneratedOnAdd();
            });

            modelBuilder.Entity<Lesson>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Id).ValueGeneratedOnAdd();
            });

            modelBuilder.Entity<Lesson>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Content).IsRequired();
                entity.Property(e => e.LessonNumber).IsRequired();
                entity.HasIndex(e => new { e.TutorialId, e.LessonNumber }).IsUnique();

                entity.Property(e => e.CreatedAt).HasDefaultValueSql("GETUTCDATE()");
                entity.Property(e => e.UpdatedAt).HasDefaultValueSql("GETUTCDATE()");
            });


            modelBuilder.Entity<Case>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Id).ValueGeneratedOnAdd();
            });

            modelBuilder.Entity<Case>()
                .HasMany(b => b.CaseTags)
                .WithMany(t => t.Case)
                .UsingEntity<Dictionary<string, object>>(
                    "caseTagMap",
                    join => join
                        .HasOne<CaseTag>()
                        .WithMany()
                        .HasForeignKey("CaseTagId")
                        .HasConstraintName("FK_CaseTagMap_CaseTags_CaseTagId")
                        .OnDelete(DeleteBehavior.Cascade),

                    join => join
                        .HasOne<Case>()
                        .WithMany()
                        .HasForeignKey("CaseId")
                        .HasConstraintName("FK_CaseTagMap_Cases_CaseId")
                        .OnDelete(DeleteBehavior.Cascade)
                );

            modelBuilder.Entity<Blog>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Id).ValueGeneratedOnAdd();
            });

            modelBuilder.Entity<Blog>()
                .HasMany(b => b.BlogTags)
                .WithMany(t => t.Blogs)
                .UsingEntity<Dictionary<string, object>>(
                    "BlogTagMap",
                    join => join
                        .HasOne<BlogTag>()
                        .WithMany()
                        .HasForeignKey("BlogTagId")
                        .HasConstraintName("FK_BlogTagMap_BlogTags_BlogTagId")
                        .OnDelete(DeleteBehavior.Cascade),

                    join => join
                        .HasOne<Blog>()
                        .WithMany()
                        .HasForeignKey("BlogId")
                        .HasConstraintName("FK_BlogTagMap_Blogs_BlogId")
                        .OnDelete(DeleteBehavior.Cascade)
                );


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

            modelBuilder.Entity<Localization>(entity =>
            {
                entity.ToTable("LocalizationKeys");
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Id).ValueGeneratedOnAdd();

                entity.Property(e => e.Key)
                      .IsRequired()
                      .HasMaxLength(200);

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
