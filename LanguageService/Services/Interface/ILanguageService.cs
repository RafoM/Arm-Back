using LanguageService.Data.Entity;

namespace LanguageService.Services.Interface
{
    public interface ILanguageService
    {
        Task<IEnumerable<Language>> GetAllLanguagesAsync();
        Task<Language> GetLanguageByIdAsync(int id);
        Task<Language> CreateLanguageAsync(Language language);
        Task UpdateLanguageAsync(int id, Language language);
        Task DeleteLanguageAsync(int id);
    }
}
