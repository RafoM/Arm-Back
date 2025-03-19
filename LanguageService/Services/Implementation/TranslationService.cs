using LanguageService.Data.Entity;
using LanguageService.Data;
using LanguageService.Services.Interface;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;

namespace LanguageService.Services.Implementation
{
    public class TranslationService : ITranslationService
    {
        private readonly ContentDbContext _dbContext;
        private readonly IMemoryCache _cache;

        public TranslationService(ContentDbContext dbContext, IMemoryCache cache)
        {
            _dbContext = dbContext;
            _cache = cache;
        }

        public async Task<IEnumerable<Translation>> GetAllTranslationsAsync()
        {
            return await _dbContext.Translations.ToListAsync();
        }

        public async Task<Translation> GetTranslationByIdAsync(int id)
        {
            return await _dbContext.Translations.FindAsync(id);
        }

        public async Task<IEnumerable<Translation>> GetTranslationsAsync(string languageCode, string entityName = null, int? entityId = null, string group = null)
        {
            if (!string.IsNullOrEmpty(group) && group.ToLower() == "global")
            {
                var cacheKey = $"GlobalTranslations_{languageCode}";
                if (_cache.TryGetValue(cacheKey, out IEnumerable<Translation> cachedTranslations))
                {
                    return cachedTranslations;
                }

                var translations = await _dbContext.Translations
                    .Where(t => t.LanguageCode == languageCode && t.Group == group)
                    .ToListAsync();

                _cache.Set(cacheKey, translations, TimeSpan.FromMinutes(10));
                return translations;
            }
            else
            {
                var query = _dbContext.Translations.AsQueryable();

                if (!string.IsNullOrEmpty(languageCode))
                {
                    query = query.Where(t => t.LanguageCode == languageCode);
                }
                if (!string.IsNullOrEmpty(entityName))
                {
                    query = query.Where(t => t.EntityName == entityName);
                }
                if (entityId.HasValue)
                {
                    query = query.Where(t => t.EntityId == entityId.Value);
                }

                return await query.ToListAsync();
            }
        }

        public async Task<Translation> CreateTranslationAsync(Translation translation)
        {
            _dbContext.Translations.Add(translation);
            await _dbContext.SaveChangesAsync();
            return translation;
        }

        public async Task UpdateTranslationAsync(int id, Translation translation)
        {
            if (id != translation.Id)
                throw new Exception("ID mismatch");
            _dbContext.Entry(translation).State = EntityState.Modified;
            await _dbContext.SaveChangesAsync();
        }

        public async Task DeleteTranslationAsync(int id)
        {
            var translation = await _dbContext.Translations.FindAsync(id);
            if (translation == null)
                throw new Exception("Translation not found");
            _dbContext.Translations.Remove(translation);
            await _dbContext.SaveChangesAsync();
        }

    }
}
