namespace ContentService.Models.RequestModels
{
    public class LanguageUpdateModel
    {
        public int Id { get; set; }
        public string CultureCode { get; set; }
        public string DisplayName { get; set; }
    }
}
