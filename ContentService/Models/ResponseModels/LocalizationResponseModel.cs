namespace ContentService.Models.ResponseModels
{
    public class LocalizationResponseModel
    {
        public int Id { get; set; }
        public string Key { get; set; }
        public int PageId { get; set; }
        public string? PageName { get; set; }
    }
}
