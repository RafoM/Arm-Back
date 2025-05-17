using System.ComponentModel.DataAnnotations;

namespace ContentService.Models.RequestModels
{
    public class BlogTagRequestModel
    {
        [Required]
        public string Tag { get; set; } = null!;
        public int LanguageId { get; set; }
    }
}
