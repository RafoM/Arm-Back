using System.ComponentModel.DataAnnotations;

namespace ContentService.Data.Entity
{
    public class TutorialSubject
    {
        [Key]
        public Guid Id { get; set; }

        [Required]
        public string Code { get; set; } = null!;

        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        public ICollection<Tutorial> Tutorials { get; set; } = new List<Tutorial>();
    }
}
