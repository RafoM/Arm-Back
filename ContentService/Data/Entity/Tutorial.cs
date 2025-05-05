using ContentService.Common.Enums;

namespace ContentService.Data.Entity
{
    public class Tutorial
    {
        public int Id { get; set; }
        public string Subject { get; set; }
        public DifficultyLevelEnum Difficulty { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }

        public ICollection<Lesson> Lessons { get; set; }
    }
}
