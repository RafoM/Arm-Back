namespace ContentService.Models.RequestModels
{
    public class TutorialDifficultyRequestModel
    {
        public string Code { get; set; } = null!;
        public string Name { get; set; } = null!;
        public int LanguageId { get; set; }
    }
}
