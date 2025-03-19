namespace LanguageService.Data.Entity
{
    public class Language
    {
        public int Id { get; set; }
        public string Code { get; set; }
        public string Name { get; set; }
        public bool IsActive { get; set; }
        public string Flag { get; set; }
        public ICollection<Translation> Translations { get; set; }
    }

}
