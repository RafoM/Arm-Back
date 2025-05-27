namespace ContentService.Models.ResponseModels
{
    public class CaseResponseModel
    {
        public Guid CaseId { get; set; }
        public string Title { get; set; } = null!;
        public string? Subtitle { get; set; }
        public string? MainImage { get; set; }
        public string Content { get; set; } = null!;
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        public int LanguageId { get; set; }
        public List<CaseTagResponseModel>? Tags { get; set; }
    }
}
