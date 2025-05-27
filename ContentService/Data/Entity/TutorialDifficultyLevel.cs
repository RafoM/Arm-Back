using System.ComponentModel.DataAnnotations;

namespace ContentService.Data.Entity
{
    public class TutorialDifficultyLevel
    {
        [Key]
        public Guid Id { get; set; }

        [Required]
        public string Code { get; set; } = null!;

        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
    }
}
