using ContentService.Models.RequestModels;
using ContentService.Models.ResponseModels;

namespace ContentService.Services.Interface
{
    public interface ILocalizationService
    {
        Task<List<LocalizationResponseModel>> GetAllAsync();
        Task<LocalizationResponseModel?> GetByIdAsync(int id);
        Task<LocalizationResponseModel> CreateAsync(LocalizationRequestModel model);
        Task<LocalizationResponseModel?> UpdateAsync(LocalizationUpdateModel model);
        Task<bool> DeleteAsync(int id);
    }
}
