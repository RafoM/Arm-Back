namespace ContentService.Models.RequestModels
{
    public class TranslationRequestModel
    {
        public int LocalizationId { get; set; }
        public int LanguageId { get; set; }
        public string Value { get; set; }
    }
}
