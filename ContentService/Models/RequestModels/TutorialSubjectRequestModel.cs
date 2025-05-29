namespace ContentService.Models.RequestModels
{
    public class TutorialSubjectRequestModel
    {
        public string Code { get; set; } = null!;
        public List<TranslatedItem> Translations { get; set; } = new();
    }
}
