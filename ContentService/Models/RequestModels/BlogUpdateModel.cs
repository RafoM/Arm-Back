using System.ComponentModel.DataAnnotations;

namespace ContentService.Models.RequestModels
{
    public class BlogUpdateModel
    {

        [Required]
        public int BlogId { get; set; }  

        [Required]
        public string Title { get; set; } = null!;

        public string? Subtitle { get; set; }

        public string? MainImage { get; set; }

        [Required]
        public string Content { get; set; } = null!;

        public List<int>? TagIds { get; set; }
        public int LanguageId { get; set; }

    }
}
