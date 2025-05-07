namespace ContentService.Data.Entity
{
    public class Localization
    {
        public int Id { get; set; }
        public string Key { get; set; }      
        public ICollection<Translation> Translations { get; set; }
    }
}
