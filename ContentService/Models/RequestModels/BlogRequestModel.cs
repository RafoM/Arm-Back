using System.ComponentModel.DataAnnotations;

namespace ContentService.Models.RequestModels
{
    public class BlogRequestModel
    {
        [Required]
        public string Title { get; set; } = null!;

        public string? Subtitle { get; set; }

        public string? MainImage { get; set; }

        [Required]
        public string Content { get; set; } = null!;
        public int LanguageId { get; set; }

        public List<int>? TagIds { get; set; }
    }
}
