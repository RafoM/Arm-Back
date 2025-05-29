using ContentService.Models.RequestModels;
using ContentService.Models.ResponseModels;

namespace ContentService.Services.Interface
{
    public interface ITutorialSubjectService
    {
        Task<TutorialSubjectResponseModel> CreateAsync(TutorialSubjectRequestModel request);
        Task<List<TutorialSubjectResponseModel>> GetAllAsync();
        Task<TutorialSubjectResponseModel> GetByIdAsync(Guid id, int languageId);
        Task UpdateAsync(TutorialSubjectUpdateModel request);
        Task DeleteAsync(Guid id);
    }
}
