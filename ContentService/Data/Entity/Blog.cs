using ContentService.Data.Entity;
using System.ComponentModel.DataAnnotations;

namespace ContentService.Data.Entity
{
    public class Blog
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string Title { get; set; } = null!;

        public string? MainImage { get; set; }

        public string? Subtitle { get; set; }
        public int LanguageId { get; set; }
        [Required]
        public string Content { get; set; } = null!;

        public DateTime CreatedAt { get; set; }

        public DateTime UpdatedAt { get; set; }

        public ICollection<BlogTag> BlogTags { get; set; } = new List<BlogTag>();
    }
}
