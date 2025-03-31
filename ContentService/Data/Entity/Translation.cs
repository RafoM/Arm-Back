namespace ContentService.Data.Entity
{
    public class Translation
    {
        public int Id { get; set; }

        public int LocalizationId { get; set; }
        public Localization Localization { get; set; }

        public int LanguageId { get; set; }
        public Language Language { get; set; }

        public string Value { get; set; }
    }
}
