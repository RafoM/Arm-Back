using ContentService.Data.Entity;
using ContentService.Data;
using ContentService.Services.Interface;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using ContentService.Models.RequestModels;
using ContentService.Models.ResponseModels;

namespace ContentService.Services.Implementation
{
    public class TranslationService : ITranslationService
    {
        private readonly IMemoryCache _cache;
        private readonly ContentDbContext _dbContext;

        public TranslationService(ContentDbContext dbContext, IMemoryCache cache)
        {
            _dbContext = dbContext;
            _cache = cache;
        }

        public async Task<Dictionary<string, string>> GetTranslationsByPageAsync(int pageId, int languageId)
        {
            var cacheKey = $"translations:page:{pageId}:lang:{languageId}";

            if (_cache.TryGetValue(cacheKey, out Dictionary<string, string> cached))
                return cached;

            var translations = await _dbContext.Translations
                .Include(t => t.Localization)
                .Where(t => t.Localization.PageId == pageId && t.LanguageId == languageId)
                .ToDictionaryAsync(t => t.Localization.Key, t => t.Value);

            _cache.Set(cacheKey, translations, TimeSpan.FromMinutes(10));

            return translations;
        }



        public async Task<List<TranslationResponseModel>> GetAllAsync()
        {
            return await _dbContext.Translations
                .Include(t => t.Localization)
                .Include(t => t.Language)
                .Select(t => new TranslationResponseModel
                {
                    Id = t.Id,
                    LocalizationId = t.LocalizationId,
                    LocalizationKey = t.Localization.Key,
                    LanguageId = t.LanguageId,
                    CultureCode = t.Language.CultureCode,
                    Value = t.Value
                })
                .ToListAsync();
        }

        public async Task<TranslationResponseModel?> GetByIdAsync(int id)
        {
            var t = await _dbContext.Translations
                .Include(x => x.Localization)
                .Include(x => x.Language)
                .FirstOrDefaultAsync(x => x.Id == id);

            if (t == null) return null;

            return new TranslationResponseModel
            {
                Id = t.Id,
                LocalizationId = t.LocalizationId,
                LocalizationKey = t.Localization.Key,
                LanguageId = t.LanguageId,
                CultureCode = t.Language.CultureCode,
                Value = t.Value
            };
        }

        public async Task<TranslationResponseModel> CreateAsync(TranslationRequestModel model)
        {
            var entity = new Translation
            {
                LocalizationId = model.LocalizationId,
                LanguageId = model.LanguageId,
                Value = model.Value.Trim()
            };

            _dbContext.Translations.Add(entity);
            await _dbContext.SaveChangesAsync();

            var localization = await _dbContext.Localizations.FindAsync(entity.LocalizationId);
            var language = await _dbContext.Languages.FindAsync(entity.LanguageId);

            return new TranslationResponseModel
            {
                Id = entity.Id,
                LocalizationId = entity.LocalizationId,
                LocalizationKey = localization?.Key ?? string.Empty,
                LanguageId = entity.LanguageId,
                CultureCode = language?.CultureCode ?? string.Empty,
                Value = entity.Value
            };
        }

        public async Task<TranslationResponseModel?> UpdateAsync(TranslationUpdateModel model)
        {
            var entity = await _dbContext.Translations.FindAsync(model.Id);
            if (entity == null) return null;

            entity.LocalizationId = model.LocalizationId;
            entity.LanguageId = model.LanguageId;
            entity.Value = model.Value.Trim();

            await _dbContext.SaveChangesAsync();

            var localization = await _dbContext.Localizations.FindAsync(entity.LocalizationId);
            var language = await _dbContext.Languages.FindAsync(entity.LanguageId);

            return new TranslationResponseModel
            {
                Id = entity.Id,
                LocalizationId = entity.LocalizationId,
                LocalizationKey = localization?.Key ?? string.Empty,
                LanguageId = entity.LanguageId,
                CultureCode = language?.CultureCode ?? string.Empty,
                Value = entity.Value
            };
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var entity = await _dbContext.Translations.FindAsync(id);
            if (entity == null) return false;

            _dbContext.Translations.Remove(entity);
            await _dbContext.SaveChangesAsync();
            return true;
        }

    }
}
