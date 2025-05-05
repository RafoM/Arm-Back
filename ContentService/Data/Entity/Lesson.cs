namespace ContentService.Data.Entity
{
    public class Lesson
    {
        public int Id { get; set; }
        public int TutorialId { get; set; }
        public Tutorial Tutorial { get; set; }

        public int LessonNumber { get; set; }
        public string Title { get; set; }
        public string Content { get; set; } 
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
    }
}
