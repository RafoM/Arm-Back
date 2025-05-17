using ContentService.Data.Entity;
using System.ComponentModel.DataAnnotations;

namespace ContentService.Data.Entity
{
    public class Blog
    {
        [Key]
        public Guid Id { get; set; }

        public DateTime CreatedAt { get; set; }

        public DateTime UpdatedAt { get; set; }

        public ICollection<BlogTag> BlogTags { get; set; } = new List<BlogTag>();
    }
}
