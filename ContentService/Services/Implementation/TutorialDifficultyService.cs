using ContentService.Common.Constants;
using ContentService.Common.Enums;
using ContentService.Data.Entity;
using ContentService.Data;
using ContentService.Services.Interface;
using ContentService.Models.ResponseModels;
using ContentService.Models.RequestModels;
using Microsoft.EntityFrameworkCore;

namespace ContentService.Services.Implementation
{
    public class TutorialDifficultyService : ITutorialDifficultyService
    {
        private readonly ContentDbContext _context;
        private readonly IContentTranslationService _translationService;

        public TutorialDifficultyService(ContentDbContext context, IContentTranslationService translationService)
        {
            _context = context;
            _translationService = translationService;
        }

        public async Task<TutorialDifficultyResponseModel> CreateAsync(TutorialDifficultyRequestModel request)
        {
            var difficulty = new TutorialDifficultyLevel
            {
                Id = Guid.NewGuid(),
                Code = request.Code,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            _context.TutorialDifficultyLevels.Add(difficulty);
            await _context.SaveChangesAsync();

            await _translationService.SetTranslationAsync(
                difficulty.Id,
                ContentTranslationKeys.Difficulty,
                request.Name,
                request.LanguageId,
                ContentTypeEnum.DifficultyLevel);

            return await MapToResponseAsync(difficulty, request.LanguageId);
        }

        public async Task<TutorialDifficultyResponseModel> GetByIdAsync(Guid id, int languageId)
        {
            var difficulty = await _context.TutorialDifficultyLevels.FindAsync(id);
            if (difficulty == null)
                throw new KeyNotFoundException("Difficulty not found.");

            return await MapToResponseAsync(difficulty, languageId);
        }

        public async Task<IEnumerable<TutorialDifficultyResponseModel>> GetAllAsync(int languageId)
        {
            var list = await _context.TutorialDifficultyLevels.ToListAsync();
            var translated = await Task.WhenAll(list.Select(d => MapToResponseAsync(d, languageId)));
            return translated.AsEnumerable();
        }

        public async Task<TutorialDifficultyResponseModel> UpdateAsync(TutorialDifficultyUpdateModel request)
        {
            var difficulty = await _context.TutorialDifficultyLevels.FindAsync(request.Id);
            if (difficulty == null)
                throw new KeyNotFoundException("Difficulty not found.");

            difficulty.Code = request.Code;
            difficulty.UpdatedAt = DateTime.UtcNow;
            await _context.SaveChangesAsync();

            await _translationService.SetTranslationAsync(
                difficulty.Id,
                ContentTranslationKeys.Difficulty,
                request.Name,
                request.LanguageId,
                ContentTypeEnum.DifficultyLevel);

            return await MapToResponseAsync(difficulty, request.LanguageId);
        }

        public async Task DeleteAsync(Guid id)
        {
            var difficulty = await _context.TutorialDifficultyLevels.FindAsync(id);
            if (difficulty == null)
                throw new KeyNotFoundException("Difficulty not found.");

            _context.TutorialDifficultyLevels.Remove(difficulty);
            await _context.SaveChangesAsync();

            await _translationService.DeleteTranslationsAsync(id, ContentTypeEnum.DifficultyLevel);
        }

        private async Task<TutorialDifficultyResponseModel> MapToResponseAsync(TutorialDifficultyLevel entity, int languageId)
        {
            var name = await _translationService.GetTranslationAsync(
                entity.Id,
                ContentTranslationKeys.Difficulty,
                languageId,
                ContentTypeEnum.DifficultyLevel);

            return new TutorialDifficultyResponseModel
            {
                Id = entity.Id,
                Code = entity.Code,
                Name = name ?? string.Empty,
                CreatedAt = entity.CreatedAt,
                UpdatedAt = entity.UpdatedAt
            };
        }
    }
}
