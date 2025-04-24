using System.ComponentModel.DataAnnotations;

namespace ContentService.Models.RequestModels
{
    public class CaseUpdateModel
    {

        [Required]
        public int CaseId { get; set; }  

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
