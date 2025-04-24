using ContentService.Models.RequestModels;
using ContentService.Models.ResponseModels;

namespace ContentService.Services.Interface
{
    public interface ICaseService
    {
        Task<CaseResponseModel> CreateAsync(CaseRequestModel request);
        Task<CaseResponseModel> UpdateAsync(CaseUpdateModel request);
        Task<CaseResponseModel> GetByIdAsync(int CaseId);
        Task<IEnumerable<CaseResponseModel>> GetAllAsync();
        Task DeleteAsync(int CaseId);
    }
}
