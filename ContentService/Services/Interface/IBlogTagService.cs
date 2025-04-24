using ContentService.Models.RequestModels;
using ContentService.Models.ResponseModels;

namespace ContentService.Services.Interface
{
    public interface IBlogTagService
    {
        Task<BlogTagResponseModel> CreateAsync(BlogTagRequestModel request);
        Task<BlogTagResponseModel> UpdateAsync(BlogTagUpdateModel request);
        Task<BlogTagResponseModel> GetByIdAsync(int tagId);
        Task<IEnumerable<BlogTagResponseModel>> GetAllAsync();
        Task DeleteAsync(int tagId);
    }
}
