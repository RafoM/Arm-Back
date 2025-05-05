using ContentService.Common.Enums;

namespace ContentService.Models.ResponseModels
{
    public class TutorialResponseModel
    {
        public int Id { get; set; }
        public string Subject { get; set; }
        public DifficultyLevelEnum Difficulty { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
    }
}
