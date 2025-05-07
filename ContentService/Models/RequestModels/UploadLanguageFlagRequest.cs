using Microsoft.AspNetCore.Mvc;

namespace ContentService.Models.RequestModels
{
    public class UploadLanguageFlagRequest
    {
        [FromForm]
        public int LanguageId { get; set; }

        [FromForm]
        public IFormFile FlagFile { get; set; }
    }
}
