using ContentService.Models.RequestModels;

namespace ContentService.Models.ResponseModels
{
    public class TutorialSubjectResponseModel
    {
        public Guid Id { get; set; }
        public string Code { get; set; } = null!;
        public List<TranslatedItem> Translations { get; set; } = new();
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
    }
}
