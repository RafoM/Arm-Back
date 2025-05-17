using ContentService.Data.Entity;
using ContentService.Models.RequestModels;
using ContentService.Models.ResponseModels;
using System.Threading.Tasks;

namespace ContentService.Services.Interface
{
    public interface IBlogService
    {
        Task<BlogResponseModel> CreateAsync(BlogRequestModel request);
        Task<BlogResponseModel> UpdateAsync(BlogUpdateModel request);
        Task<BlogResponseModel> GetByIdAsync(Guid blogId, int languageId);
        Task<IEnumerable<BlogResponseModel>> GetAllAsync(int languageId);
        Task DeleteAsync(Guid blogId);
        Task<string> UploadBlogMediaAsync(IFormFile mediaFile);
        Task<IEnumerable<BlogResponseModel>> GetByTagIdsAsync(List<Guid> tagIds, int languageId);

    }
}
