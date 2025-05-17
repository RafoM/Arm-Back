using System.ComponentModel.DataAnnotations;

namespace ContentService.Data.Entity
{
    public class BlogTag
    {
        [Key]
        public Guid Id { get; set; }
        public ICollection<Blog> Blogs { get; set; } = new List<Blog>();

    }
}
