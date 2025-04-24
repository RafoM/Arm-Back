using System.ComponentModel.DataAnnotations;

namespace ContentService.Models.RequestModels
{
    public class CaseTagUpdateModel
    {
        [Required]
        public int TagId { get; set; }

        [Required]
        public string Tag { get; set; } = null!;
    }
}
