using ContentService.Models.RequestModels;
using ContentService.Models.ResponseModels;

namespace ContentService.Services.Interface
{
    public interface ICaseTagService
    {
        Task<CaseTagResponseModel> CreateAsync(CaseTagRequestModel request);
        Task<CaseTagResponseModel> UpdateAsync(CaseTagUpdateModel request);
        Task<CaseTagResponseModel> GetByIdAsync(int tagId);
        Task<IEnumerable<CaseTagResponseModel>> GetAllAsync();
        Task DeleteAsync(int tagId);
    }
}
