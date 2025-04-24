using System.ComponentModel.DataAnnotations;

namespace ContentService.Models.RequestModels
{
    public class BlogTagUpdateModel
    {
        [Required]
        public int TagId { get; set; }

        [Required]
        public string Tag { get; set; } = null!;
    }
}
