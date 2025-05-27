namespace ContentService.Data.Entity
{
    public class Lesson
    {
        public Guid Id { get; set; }

        public Guid TutorialId { get; set; }
        public Tutorial Tutorial { get; set; } = null!;

        public int LessonNumber { get; set; }

        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
    }
}
