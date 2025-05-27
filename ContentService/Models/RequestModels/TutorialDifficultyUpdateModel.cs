namespace ContentService.Models.RequestModels
{
    public class TutorialDifficultyUpdateModel
    {
        public Guid Id { get; set; }
        public string Code { get; set; } = null!;
        public string Name { get; set; } = null!;
        public int LanguageId { get; set; }
    }
}
