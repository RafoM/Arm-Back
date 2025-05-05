namespace TransactionCore.Models.ResponseModels
{
    public class PageResultModel<T> where T : class
    {
        public int PageNumber { get; set; }
        public int PageSize { get; set; }
        public int TotalPages { get; set; }
        public int TotalCount { get; set; }
        public IEnumerable<T> Data { get; set; }
    }
}
