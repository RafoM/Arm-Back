using ContentService.Data.Entity;

namespace ContentService.Models.RequestModels
{
    public class LanguageRequestModel
    {
        public string Code { get; set; }
        public string Name { get; set; }
        public bool IsActive { get; set; }
        public string Flag { get; set; }
        public ICollection<TranslationRequestModel> Translations { get; set; }
    }
}
