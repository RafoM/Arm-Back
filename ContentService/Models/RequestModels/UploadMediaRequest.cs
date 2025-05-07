using System.ComponentModel.DataAnnotations;

namespace ContentService.Models.RequestModels
{
    public class UploadMediaRequest
    {
        [Required]
        public IFormFile MediaFile { get; set; }
    }
}
