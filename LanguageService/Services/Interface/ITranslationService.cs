using LanguageService.Data.Entity;

namespace LanguageService.Services.Interface
{
    public interface ITranslationService
    {
        Task<IEnumerable<Translation>> GetAllTranslationsAsync();
        Task<Translation> GetTranslationByIdAsync(int id);
        Task<Translation> CreateTranslationAsync(Translation translation);
        Task UpdateTranslationAsync(int id, Translation translation);
        Task DeleteTranslationAsync(int id);
        Task<IEnumerable<Translation>> GetTranslationsByLanguageIdAsync(int languageId);
    }
}
