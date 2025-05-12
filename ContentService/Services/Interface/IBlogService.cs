using ContentService.Data.Entity;
using ContentService.Models.RequestModels;
using ContentService.Models.ResponseModels;

namespace ContentService.Services.Interface
{
    public interface IBlogService
    {
        Task<BlogResponseModel> CreateAsync(BlogRequestModel request);
        Task<BlogResponseModel> UpdateAsync(BlogUpdateModel request);
        Task<BlogResponseModel> GetByIdAsync(int blogId);
        Task<IEnumerable<BlogResponseModel>> GetAllAsync();
        Task DeleteAsync(int blogId);
        Task<string> UploadBlogMediaAsync(IFormFile mediaFile);
    }
}
