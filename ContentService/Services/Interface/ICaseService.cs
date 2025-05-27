using ContentService.Models.RequestModels;
using ContentService.Models.ResponseModels;

namespace ContentService.Services.Interface
{
    public interface ICaseService
    {
        Task<CaseResponseModel> CreateAsync(CaseRequestModel request);
        Task<CaseResponseModel> UpdateAsync(CaseUpdateModel request);
        Task<CaseResponseModel> GetByIdAsync(Guid CaseId, int languageId);
        Task<IEnumerable<CaseResponseModel>> GetAllAsync(int languageId);
        Task DeleteAsync(Guid CaseId);
        Task<string> UploadCaseMediaAsync(IFormFile mediaFile);
        Task<IEnumerable<CaseResponseModel>> GetByTagIdsAsync(List<Guid> tagIds, int languageId);
    }
}
