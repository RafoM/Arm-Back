using ContentService.Common.Enums;

namespace ContentService.Models.RequestModels
{
    public class TutorialUpdateModel
    {
        public int Id { get; set; }
        public string Subject { get; set; }
        public DifficultyLevelEnum Difficulty { get; set; }
    }
}
