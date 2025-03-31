namespace ContentService.Data.Entity
{
    public class Page
    {
        public int Id { get; set; }
        public string Name { get; set; }      
        public string DisplayName { get; set; } 

        public ICollection<Localization> LocalizationKeys { get; set; }
    }
}
