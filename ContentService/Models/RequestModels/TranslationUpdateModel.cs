namespace ContentService.Models.RequestModels
{
    public class TranslationUpdateModel
    {
        public int Id { get; set; }
        public int LocalizationId { get; set; }
        public int LanguageId { get; set; }
        public string Value { get; set; }
    }
}
