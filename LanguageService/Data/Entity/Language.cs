namespace LanguageService.Data.Entity
{
    public class Language
    {
        public int Id { get; set; }
        public string CultureCode { get; set; }
        public string Name { get; set; }
        public ICollection<Translation> Translations { get; set; }
    }

}
