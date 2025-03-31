namespace ContentService.Data.Entity
{
    public class Language
    {
        public int Id { get; set; }
        public string CultureCode { get; set; }   
        public string DisplayName { get; set; }   
        public string? FlagUrl { get; set; }
        public ICollection<Translation> Translations { get; set; }
    }

}
