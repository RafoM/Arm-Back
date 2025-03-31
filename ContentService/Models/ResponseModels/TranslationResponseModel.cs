namespace ContentService.Models.ResponseModels
{
    public class TranslationResponseModel
    {
        public int Id { get; set; }
        public int LocalizationId { get; set; }
        public string LocalizationKey { get; set; }
        public int LanguageId { get; set; }
        public string CultureCode { get; set; }
        public string Value { get; set; }
    }
}
