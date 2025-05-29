namespace ContentService.Models.RequestModels
{
    public class TranslatedItem
    {
        public int LanguageId { get; set; }
        public string Value { get; set; } = null!;
    }
}
