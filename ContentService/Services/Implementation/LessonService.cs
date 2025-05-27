using ContentService.Data.Entity;
using ContentService.Data;
using ContentService.Services.Interface;
using Microsoft.EntityFrameworkCore;
using ContentService.Models.ResponseModels;
using ContentService.Models.RequestModels;
using ContentService.Common.Constants;
using ContentService.Common.Enums;

namespace ContentService.Services.Implementation
{
    public class LessonService : ILessonService
    {
        private readonly ContentDbContext _context;
        private readonly IFileStorageService _fileStorageService;
        private readonly IContentTranslationService _translationService;

        public LessonService(ContentDbContext context, IFileStorageService fileStorageService, IContentTranslationService translationService)
        {
            _context = context;
            _fileStorageService = fileStorageService;
            _translationService = translationService;
        }

        public async Task<LessonResponseModel> CreateLessonAsync(Guid tutorialId, LessonRequestModel request)
        {
            var lesson = new Lesson
            {
                Id = Guid.NewGuid(),
                TutorialId = tutorialId,
                LessonNumber = request.LessonNumber,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            _context.Lessons.Add(lesson);
            await _context.SaveChangesAsync();

            await SaveTranslations(lesson.Id, request.Title, request.Title, request.LanguageId);
            await SaveTranslations(lesson.Id, request.Content, request.Content, request.LanguageId);
            return await MapLesson(lesson, request.LanguageId);
        }

        public async Task<string> UploadMediaAsync(IFormFile mediaFile)
        {
            if (mediaFile == null || mediaFile.Length == 0)
                throw new Exception("Invalid media file.");

            return await _fileStorageService.UploadFileAsync(mediaFile, "lesson-medias");
        }

        public async Task<List<LessonResponseModel>> GetLessonsAsync(Guid tutorialId, int languageId)
        {
            var lessons = await _context.Lessons
                .Where(l => l.TutorialId == tutorialId)
                .OrderBy(l => l.LessonNumber)
                .ToListAsync();

            var results = new List<LessonResponseModel>();
            foreach (var lesson in lessons)
                results.Add(await MapLesson(lesson, languageId));

            return results;
        }

        public async Task<LessonResponseModel> GetLessonByNumberAsync(Guid tutorialId, int lessonNumber, int languageId)
        {
            var lesson = await _context.Lessons
                .FirstOrDefaultAsync(l => l.TutorialId == tutorialId && l.LessonNumber == lessonNumber);

            if (lesson == null) throw new KeyNotFoundException("Lesson not found.");
            return await MapLesson(lesson, languageId);
        }

        public async Task UpdateLessonAsync(LessonUpdateModel request)
        {
            var lesson = await _context.Lessons
                .FirstOrDefaultAsync(l => l.TutorialId == request.TutorialId && l.LessonNumber == request.LessonNumber);

            if (lesson == null) throw new KeyNotFoundException("Lesson not found.");

            lesson.UpdatedAt = DateTime.UtcNow;
            await _context.SaveChangesAsync();

            await SaveTranslations(lesson.Id, request.Title, request.Title, request.LanguageId);
            await SaveTranslations(lesson.Id, request.Content, request.Content, request.LanguageId);
        }

        public async Task DeleteLessonAsync(Guid tutorialId, int lessonNumber)
        {
            var lesson = await _context.Lessons
                .FirstOrDefaultAsync(l => l.TutorialId == tutorialId && l.LessonNumber == lessonNumber);

            if (lesson == null) throw new KeyNotFoundException("Lesson not found.");

            _context.Lessons.Remove(lesson);
            await _context.SaveChangesAsync();

            await _translationService.DeleteTranslationsAsync(lesson.Id, ContentTypeEnum.Lesson);
        }

        private async Task SaveTranslations(Guid lessonId, string title, string content, int languageId)
        {
            await _translationService.SetTranslationAsync(lessonId, ContentTranslationKeys.Title, title, languageId, ContentTypeEnum.Lesson);
            await _translationService.SetTranslationAsync(lessonId, ContentTranslationKeys.Content, content, languageId, ContentTypeEnum.Lesson);
        }

        private async Task<LessonResponseModel> MapLesson(Lesson lesson, int languageId)
        {
            var title = await _translationService.GetTranslationAsync(lesson.Id, ContentTranslationKeys.Title, languageId, ContentTypeEnum.Lesson);
            var content = await _translationService.GetTranslationAsync(lesson.Id, ContentTranslationKeys.Content, languageId, ContentTypeEnum.Lesson);

            return new LessonResponseModel
            {
                Id = lesson.Id,
                LessonNumber = lesson.LessonNumber,
                Title = title ?? string.Empty,
                Content = content ?? string.Empty,
                CreatedAt = lesson.CreatedAt,
                UpdatedAt = lesson.UpdatedAt
            };
        }
    }
}
