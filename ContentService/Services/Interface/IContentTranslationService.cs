using ContentService.Common.Enums;
using ContentService.Data.Entity;

namespace ContentService.Services.Interface
{
    public interface IContentTranslationService
    {
        Task<IEnumerable<ContentTranslation>> GetTranslationsAsync(Guid contentId, ContentTypeEnum contentType);
        Task<string?> GetTranslationAsync(Guid contentId, string key, int languageId, ContentTypeEnum contentType);
        Task SetTranslationAsync(Guid contentId, string key, string value, int languageId, ContentTypeEnum contentType);
        Task DeleteTranslationsAsync(Guid contentId, ContentTypeEnum contentType);
        Task<List<ContentTranslation>> GetAllTranslationsAsync(Guid contentId, string key, ContentTypeEnum contentType);
    }
}
