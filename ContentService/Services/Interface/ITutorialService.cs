using ContentService.Models.RequestModels;
using ContentService.Models.ResponseModels;

namespace ContentService.Services.Interface
{
    public interface ITutorialService
    {
        Task<TutorialResponseModel> CreateTutorialAsync(TutorialRequestModel request);
        Task<List<TutorialResponseModel>> GetAllTutorialsAsync(int languageId, Guid? difficultyId = null, List<Guid>? subjectIds = null);
        Task<TutorialResponseModel> GetTutorialByIdAsync(Guid id, int languageId);
        Task UpdateTutorialAsync(TutorialUpdateModel request);
        Task DeleteTutorialAsync(Guid id);
    }
}
