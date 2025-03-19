using LanguageService.Data.Entity;

namespace ContentService.Data.Entity
{
    public class Tutorial
    {

        public int Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public string VideoUrl { get; set; }
        public string ThumbnailUrl { get; set; }
        public int LanguageId { get; set; }
        public Language Language { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime? UpdatedDate { get; set; }
    }
}
