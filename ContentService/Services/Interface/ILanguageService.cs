using ContentService.Models.RequestModels;
using ContentService.Models.ResponseModels;

namespace ContentService.Services.Interface
{
    public interface ILanguageService
    {
        Task<IEnumerable<LanguageResponseModel>> GetAllAsync();
        Task<LanguageResponseModel> GetByIdAsync(int id);
        Task<int> CreateAsync(LanguageRequestModel model);
        Task<bool> UpdateAsync(LanguageUpdateModel model);
        Task<bool> DeleteAsync(int id);
        Task<string> UploadFlagAsync(int languageId, IFormFile flagFile);
    }
}
