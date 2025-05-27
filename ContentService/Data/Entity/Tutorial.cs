using ContentService.Common.Enums;

namespace ContentService.Data.Entity
{
    public class Tutorial
    {

        public Guid Id { get; set; }

        public Guid DifficultyId { get; set; }
        public TutorialDifficultyLevel DifficultyLevel { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }

        public ICollection<Lesson> Lessons { get; set; } = new List<Lesson>();
    }
}
