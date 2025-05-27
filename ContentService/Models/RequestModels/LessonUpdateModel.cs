namespace ContentService.Models.RequestModels
{
    public class LessonUpdateModel
    {
        public Guid TutorialId { get; set; }
        public int LessonNumber { get; set; }
        public string Title { get; set; }
        public string Content { get; set; }
        public int LanguageId { get; set; }
    }
}
