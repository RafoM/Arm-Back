using System.ComponentModel.DataAnnotations;

namespace TransactionCore.Models.RequestModels
{
    public class UploadIqonRequest
    {
        [Required]
        public int LanguageId { get; set; }

        [Required]
        public IFormFile FlagFile { get; set; }
    }
}
