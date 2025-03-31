using ContentService.Models.RequestModels;
using ContentService.Models.ResponseModels;

namespace ContentService.Services.Interface
{
    public interface IPageService
    {
        Task<List<PageResponseModel>> GetAllAsync();
        Task<PageResponseModel?> GetByIdAsync(int id);
        Task<PageResponseModel> CreateAsync(PageRequestModel model);
        Task<PageResponseModel?> UpdateAsync(PageUpdateModel model);
        Task<bool> DeleteAsync(int id);
    }
}
