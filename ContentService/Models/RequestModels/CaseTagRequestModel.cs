using System.ComponentModel.DataAnnotations;

namespace ContentService.Models.RequestModels
{
    public class CaseTagRequestModel
    {
        [Required]
        public string Tag { get; set; } = null!;
    }
}
