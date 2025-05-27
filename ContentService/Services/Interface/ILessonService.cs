using ContentService.Models.RequestModels;
using ContentService.Models.ResponseModels;

namespace ContentService.Services.Interface
{
    public interface ILessonService
    {
        Task<LessonResponseModel> CreateLessonAsync(Guid tutorialId, LessonRequestModel request);
        Task<List<LessonResponseModel>> GetLessonsAsync(Guid tutorialId, int languageId);
        Task<LessonResponseModel> GetLessonByNumberAsync(Guid tutorialId, int lessonNumber, int languageId);
        Task UpdateLessonAsync(LessonUpdateModel request);
        Task DeleteLessonAsync(Guid tutorialId, int lessonNumber);
        Task<string> UploadMediaAsync(IFormFile mediaFile);
    }
}
