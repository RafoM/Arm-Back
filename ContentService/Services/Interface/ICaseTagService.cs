using ContentService.Models.RequestModels;
using ContentService.Models.ResponseModels;

namespace ContentService.Services.Interface
{
    public interface ICaseTagService
    {
        Task<CaseTagResponseModel> CreateAsync(CaseTagRequestModel request);
        Task<CaseTagResponseModel> UpdateAsync(CaseTagUpdateModel request);
        Task<CaseTagResponseModel> GetByIdAsync(Guid tagId, int languageId);
        Task<IEnumerable<CaseTagResponseModel>> GetAllAsync(int languageId);
        Task DeleteAsync(Guid tagId);
    }
}
