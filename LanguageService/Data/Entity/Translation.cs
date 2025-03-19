namespace LanguageService.Data.Entity
{
    public class Translation
    {
        public int Id { get; set; }
        public string EntityName { get; set; }
        public int? EntityId { get; set; }
        public string FieldName { get; set; }
        public string LanguageCode { get; set; }
        public string Value { get; set; }
        public string Group { get; set; }
    }
}
