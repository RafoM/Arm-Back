using ContentService.Models.RequestModels;
using ContentService.Models.ResponseModels;

namespace ContentService.Services.Interface
{
    public interface ITutorialDifficultyService
    {
        Task<TutorialDifficultyResponseModel> CreateAsync(TutorialDifficultyRequestModel request);
        Task<TutorialDifficultyResponseModel> UpdateAsync(TutorialDifficultyUpdateModel request);
        Task<TutorialDifficultyResponseModel> GetByIdAsync(Guid id, int languageId);
        Task<IEnumerable<TutorialDifficultyResponseModel>> GetAllAsync(int languageId);
        Task DeleteAsync(Guid id);
    }
}
