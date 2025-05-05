namespace ContentService.Models.ResponseModels
{
    public class LessonResponseModel
    {
        public int Id { get; set; }
        public int LessonNumber { get; set; }
        public string Title { get; set; }
        public string Content { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
    }
}
