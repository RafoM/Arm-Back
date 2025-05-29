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
        public DbSet<ContentTranslation> ContentTranslations { get; set; }
        public DbSet<TutorialDifficultyLevel> TutorialDifficultyLevels { get; set; }
        public DbSet<TutorialSubject> TutorialSubjects { get; set; }

        //rolepermission
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {

            modelBuilder.Entity<Tutorial>()
                        .HasMany(t => t.Subjects)
                        .WithMany(s => s.Tutorials)
                        .UsingEntity<Dictionary<string, object>>(
                            "TutorialTutorialSubject", 
                            j => j.HasOne<TutorialSubject>()
                                  .WithMany()
                                  .HasForeignKey("TutorialSubjectId")
                                  .OnDelete(DeleteBehavior.Cascade),
                            j => j.HasOne<Tutorial>()
                                  .WithMany()
                                  .HasForeignKey("TutorialId")
                                  .OnDelete(DeleteBehavior.Cascade),
                            j =>
                            {
                                j.HasKey("TutorialId", "TutorialSubjectId");
                                j.ToTable("TutorialTutorialSubject"); 
                                j.Property<DateTime>("CreatedAt").HasDefaultValueSql("GETUTCDATE()");
                            }
                        );

            modelBuilder.Entity<ContentTranslation>()
                        .HasIndex(ct => new { ct.ContentId, ct.Key, ct.LanguageId, ct.ContentType })
                        .IsUnique();

            modelBuilder.Entity<ContentTranslation>(entity =>
            {
                entity.HasKey(x => x.Id);
                entity.Property(e => e.Id).HasDefaultValueSql("NEWID()");
                entity.Property(x => x.ContentType).HasConversion<string>();
            });


            modelBuilder.Entity<Blog>(entity =>
            {
                entity.HasKey(x => x.Id);
                entity.Property(e => e.Id).HasDefaultValueSql("NEWID()");
            });

            modelBuilder.Entity<Case>(entity =>
            {
                entity.HasKey(x => x.Id);
                entity.Property(e => e.Id).HasDefaultValueSql("NEWID()");
            });
            modelBuilder.Entity<BlogTag>(entity =>
            {
                entity.HasKey(x => x.Id);
                entity.Property(e => e.Id).HasDefaultValueSql("NEWID()");
            });
            modelBuilder.Entity<CaseTag>(entity =>
            {
                entity.HasKey(x => x.Id);
                entity.Property(e => e.Id).HasDefaultValueSql("NEWID()");
            });
            modelBuilder.Entity<Tutorial>(entity =>
            {
                entity.HasKey(t => t.Id);
                entity.Property(t => t.Id).HasDefaultValueSql("NEWID()");

                entity.Property(t => t.CreatedAt).IsRequired();
                entity.Property(t => t.UpdatedAt).IsRequired();

                entity.HasMany(t => t.Lessons)
                      .WithOne(l => l.Tutorial)
                      .HasForeignKey(l => l.TutorialId)
                      .OnDelete(DeleteBehavior.Cascade);
            });


            modelBuilder.Entity<Lesson>(entity =>
            {
                entity.HasKey(l => l.Id);
                entity.Property(l => l.Id).HasDefaultValueSql("NEWID()");

                entity.Property(l => l.LessonNumber).IsRequired();
                entity.Property(l => l.CreatedAt).IsRequired();
                entity.Property(l => l.UpdatedAt).IsRequired();
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
                entity.Property(e => e.Id).HasDefaultValueSql("NEWID()");

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
