namespace ContentService.Data.Entity
{
    public class Localization
    {
        public int Id { get; set; }
        public string Key { get; set; }      

        public int PageId { get; set; }
        public Page Page { get; set; }

        public ICollection<Translation> Translations { get; set; }
    }
}
