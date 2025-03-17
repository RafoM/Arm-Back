using LanguageService.Data.Entity;
using LanguageService.Data;
using LanguageService.Services.Interface;
using Microsoft.EntityFrameworkCore;

namespace LanguageService.Services.Implementation
{
    public class TranslationService : ITranslationService
    {
        private readonly LocalizationDbContext _dbContext;

        public TranslationService(LocalizationDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<IEnumerable<Translation>> GetAllTranslationsAsync()
        {
            return await _dbContext.Translations
                .Include(t => t.Language)
                .ToListAsync();
        }

        public async Task<Translation> GetTranslationByIdAsync(int id)
        {
            return await _dbContext.Translations
                .Include(t => t.Language)
                .FirstOrDefaultAsync(t => t.Id == id);
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
        public async Task<IEnumerable<Translation>> GetTranslationsByLanguageIdAsync(int languageId)
        {
            return await _dbContext.Translations
                .Where(t => t.LanguageId == languageId)
                .Include(t => t.Language)
                .ToListAsync();
        }

    }
}
