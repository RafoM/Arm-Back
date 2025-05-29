using ContentService.Common.Enums;

namespace ContentService.Models.RequestModels
{
    public class TutorialUpdateModel
    {
        public Guid Id { get; set; }
        public List<Guid> SubjectIds { get; set; }
        public Guid DifficultyId { get; set; }
        public int LanguageId { get; set; }
    }
}
