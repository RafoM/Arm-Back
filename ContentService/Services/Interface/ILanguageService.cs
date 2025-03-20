using ContentService.Data.Entity;
using ContentService.Models.RequestModels;

namespace ContentService.Services.Interface
{
    public interface ILanguageService
    {
        Task<IEnumerable<Language>> GetAllLanguagesAsync();
        Task<Language> GetLanguageByIdAsync(int id);
        Task<Language> CreateLanguageAsync(LanguageRequestModel language);
        Task UpdateLanguageAsync(int id, LanguageRequestModel language);
        Task DeleteLanguageAsync(int id);
        Task<string> UploadFlagAsync(int languageId, IFormFile flagFile);
    }
}
