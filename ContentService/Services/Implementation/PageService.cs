using ContentService.Data;
using ContentService.Data.Entity;
using ContentService.Models.RequestModels;
using ContentService.Models.ResponseModels;
using Microsoft.EntityFrameworkCore;

namespace ContentService.Services.Implementation
{
    public class PageService
    {
        private readonly ContentDbContext _dbContext;

        public PageService(ContentDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<List<PageResponseModel>> GetAllAsync()
        {
            return await _dbContext.Pages
                .Select(p => new PageResponseModel
                {
                    Id = p.Id,
                    Name = p.Name,
                    DisplayName = p.DisplayName
                })
                .ToListAsync();
        }

        public async Task<PageResponseModel?> GetByIdAsync(int id)
        {
            var entity = await _dbContext.Pages.FindAsync(id);
            if (entity == null) return null;

            return new PageResponseModel
            {
                Id = entity.Id,
                Name = entity.Name,
                DisplayName = entity.DisplayName
            };
        }

        public async Task<PageResponseModel> CreateAsync(PageRequestModel model)
        {
            var page = new Page
            {
                Name = model.Name.Trim(),
                DisplayName = model.DisplayName?.Trim()
            };

            _dbContext.Pages.Add(page);
            await _dbContext.SaveChangesAsync();

            return new PageResponseModel
            {
                Id = page.Id,
                Name = page.Name,
                DisplayName = page.DisplayName
            };
        }

        public async Task<PageResponseModel?> UpdateAsync(PageUpdateModel model)
        {
            var entity = await _dbContext.Pages.FindAsync(model.Id);
            if (entity == null) return null;

            entity.Name = model.Name.Trim();
            entity.DisplayName = model.DisplayName?.Trim();

            await _dbContext.SaveChangesAsync();

            return new PageResponseModel
            {
                Id = entity.Id,
                Name = entity.Name,
                DisplayName = entity.DisplayName
            };
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var entity = await _dbContext.Pages.FindAsync(id);
            if (entity == null) return false;

            _dbContext.Pages.Remove(entity);
            await _dbContext.SaveChangesAsync();
            return true;
        }
    }
}
