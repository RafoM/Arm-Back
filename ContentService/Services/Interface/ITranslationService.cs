using ContentService.Data.Entity;
using ContentService.Models.RequestModels;

namespace ContentService.Services.Interface
{
    public interface ITranslationService
    {
        Task<IEnumerable<Translation>> GetAllTranslationsAsync();
        Task<Translation> GetTranslationByIdAsync(int id);
        Task<IEnumerable<Translation>> GetTranslationsAsync(string languageCode, string entityName = null, int? entityId = null, string group = null);
        Task<Translation> CreateTranslationAsync(TranslationRequestModel translation);
        Task UpdateTranslationAsync(int id, TranslationRequestModel translation);
        Task DeleteTranslationAsync(int id);
    }
}
