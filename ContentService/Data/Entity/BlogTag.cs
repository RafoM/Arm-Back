using System.ComponentModel.DataAnnotations;

namespace ContentService.Data.Entity
{
    public class BlogTag
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string Tag { get; set; } = null!;
        public ICollection<Blog> Blogs { get; set; } = new List<Blog>();

    }
}
