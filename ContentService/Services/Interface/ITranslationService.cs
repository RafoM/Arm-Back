using ContentService.Data.Entity;
using ContentService.Models.RequestModels;
using ContentService.Models.ResponseModels;

namespace ContentService.Services.Interface
{
    public interface ITranslationService
    {
        Task<List<TranslationResponseModel>> GetAllAsync();
        Task<TranslationResponseModel?> GetByIdAsync(int id);
        Task<TranslationResponseModel> CreateAsync(TranslationRequestModel model);
        Task<TranslationResponseModel?> UpdateAsync(TranslationUpdateModel model);
        Task<bool> DeleteAsync(int id);
        Task<Dictionary<string, string>> GetTranslationsByPageAsync(int pageId, int languageId);

    }
}
