namespace ContentService.Models.ResponseModels
{
    public class BlogResponseModel
    {
        public Guid BlogId { get; set; }
        public string Title { get; set; } = null!;
        public string? Subtitle { get; set; }
        public string? MainImage { get; set; }
        public string Content { get; set; } = null!;
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        public int LanguageId { get; set; }
        public List<BlogTagResponseModel>? Tags { get; set; }
    }
}
