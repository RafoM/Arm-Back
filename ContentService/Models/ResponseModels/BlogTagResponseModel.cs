namespace ContentService.Models.ResponseModels
{
    public class BlogTagResponseModel
    {
        public Guid TagId { get; set; }
        public string Tag { get; set; } = null!;
    }
}
