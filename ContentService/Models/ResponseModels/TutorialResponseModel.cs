using ContentService.Common.Enums;

namespace ContentService.Models.ResponseModels
{
    public class TutorialResponseModel
    {
        public Guid Id { get; set; }
        public string Subject { get; set; }
        public string Difficulty { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
    }
}
