namespace ContentService.Models.RequestModels
{
    public class TutorialSubjectUpdateModel
    {
        public Guid Id { get; set; }
        public string Code { get; set; } = null!;
        public List<TranslatedItem> Translations { get; set; } = new();
    }
}
