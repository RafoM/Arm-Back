using ContentService.Data;
using ContentService.Data.Entity;
using ContentService.Models.RequestModels;
using ContentService.Models.ResponseModels;
using ContentService.Services.Interface;
using Microsoft.EntityFrameworkCore;

namespace ContentService.Services.Implementation
{
    public class LocalizationService : ILocalizationService
    {
        private readonly ContentDbContext _dbContext;

        public LocalizationService(ContentDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<List<LocalizationResponseModel>> GetAllAsync()
        {
            return await _dbContext.Localizations
                .Select(l => new LocalizationResponseModel
                {
                    Id = l.Id,
                    Key = l.Key
                })
                .ToListAsync();
        }

        public async Task<LocalizationResponseModel?> GetByIdAsync(int id)
        {
            var entity = await _dbContext.Localizations.FirstOrDefaultAsync(l => l.Id == id);

            if (entity == null) return null;

            return new LocalizationResponseModel
            {
                Id = entity.Id,
                Key = entity.Key,
            };
        }

        public async Task<LocalizationResponseModel> CreateAsync(LocalizationRequestModel model)
        {
            var localization = new Localization
            {
                Key = model.Key.Trim(),
            };

            _dbContext.Localizations.Add(localization);
            await _dbContext.SaveChangesAsync();

            return new LocalizationResponseModel
            {
                Id = localization.Id,
                Key = localization.Key,
            };
        }

        public async Task<LocalizationResponseModel?> UpdateAsync(LocalizationUpdateModel model)
        {
            var entity = await _dbContext.Localizations.FindAsync(model.Id);
            if (entity == null) return null;

            entity.Key = model.Key.Trim();

            await _dbContext.SaveChangesAsync();

            return new LocalizationResponseModel
            {
                Id = entity.Id,
                Key = entity.Key
            };
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var entity = await _dbContext.Localizations.FindAsync(id);
            if (entity == null) return false;

            _dbContext.Localizations.Remove(entity);
            await _dbContext.SaveChangesAsync();
            return true;
        }
    }
}
