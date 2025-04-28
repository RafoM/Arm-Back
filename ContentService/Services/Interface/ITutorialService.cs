using ContentService.Models.RequestModels;
using ContentService.Models.ResponseModels;

namespace ContentService.Services.Interface
{
    public interface ITutorialService
    {
        Task<TutorialResponseModel> CreateTutorialAsync(TutorialRequestModel request);
        Task<List<TutorialResponseModel>> GetAllTutorialsAsync();
        Task<TutorialResponseModel> GetTutorialByIdAsync(int id);
        Task UpdateTutorialAsync(TutorialUpdateModel request);
        Task DeleteTutorialAsync(int id);
    }
}
