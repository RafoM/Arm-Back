using System.ComponentModel.DataAnnotations;

namespace ContentService.Models.RequestModels
{
    public class UploadLanguageFlagRequest
    {
        [Required]
        public int LanguageId { get; set; }

        [Required]
        public IFormFile FlagFile { get; set; }
    }
}
