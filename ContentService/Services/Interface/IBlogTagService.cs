using ContentService.Models.RequestModels;
using ContentService.Models.ResponseModels;

namespace ContentService.Services.Interface
{
    public interface IBlogTagService
    {
        Task<BlogTagResponseModel> CreateAsync(BlogTagRequestModel request);
        Task<BlogTagResponseModel> UpdateAsync(BlogTagUpdateModel request);
        Task<BlogTagResponseModel> GetByIdAsync(Guid tagId, int languageId);
        Task<IEnumerable<BlogTagResponseModel>> GetAllAsync(int languageId);
        Task DeleteAsync(Guid tagId);
    }
}
