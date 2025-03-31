using ContentService.Data.Entity;

namespace ContentService.Models.RequestModels
{
    public class LanguageRequestModel
    {
        public string CultureCode { get; set; }
        public string DisplayName { get; set; }
    }
}
