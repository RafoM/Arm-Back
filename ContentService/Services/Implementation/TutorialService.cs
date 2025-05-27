using ContentService.Data.Entity;
using ContentService.Data;
using ContentService.Services.Interface;
using ContentService.Models.ResponseModels;
using ContentService.Models.RequestModels;
using Microsoft.EntityFrameworkCore;
using ContentService.Common.Constants;
using ContentService.Common.Enums;

namespace ContentService.Services.Implementation
{
    public class TutorialService : ITutorialService
    {
        private readonly ContentDbContext _context;
        private readonly IContentTranslationService _translationService;

        public TutorialService(ContentDbContext context, IContentTranslationService translationService)
        {
            _context = context;
            _translationService = translationService;
        }

        public async Task<TutorialResponseModel> CreateTutorialAsync(TutorialRequestModel request)
        {
            var tutorial = new Tutorial
            {
                Id = Guid.NewGuid(),
                DifficultyId = request.DifficultyId,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            _context.Tutorials.Add(tutorial);
            await _context.SaveChangesAsync();

            await _translationService.SetTranslationAsync(
                tutorial.Id,
                ContentTranslationKeys.Subject,
                request.Subject,
                request.LanguageId,
                ContentTypeEnum.Tutorial);

            return await MapTutorial(tutorial, request.LanguageId);
        }

        public async Task<List<TutorialResponseModel>> GetAllTutorialsAsync(int languageId)
        {
            var tutorials = await _context.Tutorials.Include(t => t.DifficultyLevel).ToListAsync();
            var results = new List<TutorialResponseModel>();

            foreach (var tutorial in tutorials)
                results.Add(await MapTutorial(tutorial, languageId));

            return results;
        }

        public async Task<TutorialResponseModel> GetTutorialByIdAsync(Guid id, int languageId)
        {
            var tutorial = await _context.Tutorials.Include(t => t.DifficultyLevel).FirstOrDefaultAsync(t => t.Id == id);
            if (tutorial == null) throw new KeyNotFoundException("Tutorial not found.");

            return await MapTutorial(tutorial, languageId);
        }

        public async Task UpdateTutorialAsync(TutorialUpdateModel request)
        {
            var tutorial = await _context.Tutorials.FindAsync(request.Id);
            if (tutorial == null) throw new KeyNotFoundException("Tutorial not found.");

            tutorial.DifficultyId = request.DifficultyId;
            tutorial.UpdatedAt = DateTime.UtcNow;
            await _context.SaveChangesAsync();

            await _translationService.SetTranslationAsync(
                tutorial.Id,
                ContentTranslationKeys.Subject,
                request.Subject,
                request.LanguageId,
                ContentTypeEnum.Tutorial);
        }

        public async Task DeleteTutorialAsync(Guid id)
        {
            var tutorial = await _context.Tutorials.FindAsync(id);
            if (tutorial == null) throw new KeyNotFoundException("Tutorial not found.");

            _context.Tutorials.Remove(tutorial);
            await _context.SaveChangesAsync();

            await _translationService.DeleteTranslationsAsync(id, ContentTypeEnum.Tutorial);
        }

        private async Task<TutorialResponseModel> MapTutorial(Tutorial tutorial, int languageId)
        {
            var subject = await _translationService.GetTranslationAsync(
                tutorial.Id, ContentTranslationKeys.Subject, languageId, ContentTypeEnum.Tutorial);

            var difficultyName = await _translationService.GetTranslationAsync(
                tutorial.DifficultyId, ContentTranslationKeys.Difficulty, languageId, ContentTypeEnum.DifficultyLevel);

            return new TutorialResponseModel
            {
                Id = tutorial.Id,
                Subject = subject ?? string.Empty,
                Difficulty = difficultyName ?? string.Empty,
                CreatedAt = tutorial.CreatedAt,
                UpdatedAt = tutorial.UpdatedAt
            };
        }
    }
}
