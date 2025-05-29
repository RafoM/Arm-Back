using ContentService.Common.Enums;

namespace ContentService.Models.RequestModels
{
    public class TutorialRequestModel
    {
        public List<Guid> SubjectIds { get; set; }
        public Guid DifficultyId { get; set; }
        public int LanguageId { get; set; }

    }
}
