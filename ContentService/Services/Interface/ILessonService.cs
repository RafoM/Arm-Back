using ContentService.Models.RequestModels;
using ContentService.Models.ResponseModels;

namespace ContentService.Services.Interface
{
    public interface ILessonService
    {
        Task<LessonResponseModel> CreateLessonAsync(int tutorialId, LessonRequestModel request);
        Task<List<LessonResponseModel>> GetLessonsAsync(int tutorialId);
        Task<LessonResponseModel> GetLessonByNumberAsync(int tutorialId, int lessonNumber);
        Task UpdateLessonAsync(LessonUpdateModel request);
        Task DeleteLessonAsync(int tutorialId, int lessonNumber);
        Task<string> UploadMediaAsync(IFormFile mediaFile);
    }
}
