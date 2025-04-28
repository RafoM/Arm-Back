using ContentService.Data.Entity;
using ContentService.Data;
using ContentService.Services.Interface;
using Microsoft.EntityFrameworkCore;
using ContentService.Models.ResponseModels;
using ContentService.Models.RequestModels;

namespace ContentService.Services.Implementation
{
    public class LessonService : ILessonService
    {
        private readonly ContentDbContext _context;
        private readonly IFileStorageService _fileStorageService;
        public LessonService(ContentDbContext context, IFileStorageService fileStorageService)
        {
            _context = context;
            _fileStorageService = fileStorageService;
        }

        public async Task<LessonResponseModel> CreateLessonAsync(int tutorialId, LessonRequestModel request)
        {
            var lesson = new Lesson
            {
                TutorialId = tutorialId,
                LessonNumber = request.LessonNumber,
                Title = request.Title,
                Content = request.Content,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            _context.Lessons.Add(lesson);
            await _context.SaveChangesAsync();

            return MapLesson(lesson);
        }
        public async Task<string> UploadMediaAsync(IFormFile mediaFile)
        {
            if (mediaFile == null || mediaFile.Length == 0)
                throw new Exception("Invalid media file.");

            var flagUrl = await _fileStorageService.UploadFileAsync(mediaFile, "lesson-medias");

            return flagUrl;
        }
        public async Task<List<LessonResponseModel>> GetLessonsAsync(int tutorialId)
        {
            return await _context.Lessons
                .Where(l => l.TutorialId == tutorialId)
                .OrderBy(l => l.LessonNumber)
                .Select(l => MapLesson(l))
                .ToListAsync();
        }

        public async Task<LessonResponseModel> GetLessonByNumberAsync(int tutorialId, int lessonNumber)
        {
            var lesson = await _context.Lessons
                .FirstOrDefaultAsync(l => l.TutorialId == tutorialId && l.LessonNumber == lessonNumber);

            if (lesson == null) throw new KeyNotFoundException("Lesson not found.");

            return MapLesson(lesson);
        }

        public async Task UpdateLessonAsync(LessonUpdateModel request)
        {
            var lesson = await _context.Lessons
                .FirstOrDefaultAsync(l => l.TutorialId == request.TutorialId && l.LessonNumber == request.LessonNumber);

            if (lesson == null) throw new KeyNotFoundException("Lesson not found.");

            lesson.Title = request.Title;
            lesson.Content = request.Content;
            lesson.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();
        }

        public async Task DeleteLessonAsync(int tutorialId, int lessonNumber)
        {
            var lesson = await _context.Lessons
                .FirstOrDefaultAsync(l => l.TutorialId == tutorialId && l.LessonNumber == lessonNumber);

            if (lesson == null) throw new KeyNotFoundException("Lesson not found.");

            _context.Lessons.Remove(lesson);
            await _context.SaveChangesAsync();
        }

        private static LessonResponseModel MapLesson(Lesson lesson) => new LessonResponseModel
        {
            Id = lesson.Id,
            LessonNumber = lesson.LessonNumber,
            Title = lesson.Title,
            Content = lesson.Content,
            CreatedAt = lesson.CreatedAt,
            UpdatedAt = lesson.UpdatedAt
        };
    }
}
