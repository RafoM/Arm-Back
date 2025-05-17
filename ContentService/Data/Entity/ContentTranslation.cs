using ContentService.Common.Enums;

namespace ContentService.Data.Entity
{
    public class ContentTranslation
    {
        public Guid Id { get; set; }

        public Guid ContentId { get; set; }

        public ContentTypeEnum ContentType { get; set; }  // Stored as string in DB

        public string Key { get; set; } = null!;

        public string Value { get; set; } = null!;

        public int LanguageId { get; set; }
    }

}
