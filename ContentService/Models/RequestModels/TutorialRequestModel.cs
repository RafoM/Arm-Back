using ContentService.Common.Enums;

namespace ContentService.Models.RequestModels
{
    public class TutorialRequestModel
    {
        public string Subject { get; set; }
        public DifficultyLevelEnum Difficulty { get; set; }
    }
}
