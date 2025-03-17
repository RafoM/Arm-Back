namespace LanguageService.Data.Entity
{
    public class Translation
    {
        public int Id { get; set; }
        public string Key { get; set; }  
        public string Value { get; set; }
        public int LanguageId { get; set; }
        public Language Language { get; set; }
    }
}
