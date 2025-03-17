using LanguageService.Data.Entity;
using LanguageService.Data;
using Microsoft.EntityFrameworkCore;
using LanguageService.Services.Interface;

namespace LanguageService.Services.Implementation
{
    public class LanguageService : ILanguageService
    {
        private readonly LocalizationDbContext _dbContext;

        public LanguageService(LocalizationDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<IEnumerable<Language>> GetAllLanguagesAsync()
        {
            return await _dbContext.Languages.ToListAsync();
        }

        public async Task<Language> GetLanguageByIdAsync(int id)
        {
            return await _dbContext.Languages.FindAsync(id);
        }

        public async Task<Language> CreateLanguageAsync(Language language)
        {
            _dbContext.Languages.Add(language);
            await _dbContext.SaveChangesAsync();
            return language;
        }

        public async Task UpdateLanguageAsync(int id, Language language)
        {
            if (id != language.Id)
                throw new Exception("ID mismatch");

            _dbContext.Entry(language).State = EntityState.Modified;
            await _dbContext.SaveChangesAsync();
        }

        public async Task DeleteLanguageAsync(int id)
        {
            var language = await _dbContext.Languages.FindAsync(id);
            if (language == null)
                throw new Exception("Language not found");

            _dbContext.Languages.Remove(language);
            await _dbContext.SaveChangesAsync();
        }
    }
}
