using ContentService.Data.Entity;
using ContentService.Data;
using ContentService.Services.Interface;
using ContentService.Models.ResponseModels;
using ContentService.Models.RequestModels;
using Microsoft.EntityFrameworkCore;

namespace ContentService.Services.Implementation
{
    public class CaseTagService : ICaseTagService
    {
        private readonly ContentDbContext _dbContext;

        public CaseTagService(ContentDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<CaseTagResponseModel> CreateAsync(CaseTagRequestModel request)
        {
            var newTag = new CaseTag
            {
                Tag = request.Tag
            };

            _dbContext.CaseTags.Add(newTag);
            await _dbContext.SaveChangesAsync();

            return ToTagResponse(newTag);
        }

        public async Task<CaseTagResponseModel> UpdateAsync(CaseTagUpdateModel request)
        {
            var tag = await _dbContext.CaseTags.FindAsync(request.TagId);
            if (tag == null)
                throw new KeyNotFoundException($"Tag with ID {request.TagId} not found.");

            tag.Tag = request.Tag;
            await _dbContext.SaveChangesAsync();

            return ToTagResponse(tag);
        }

        public async Task<CaseTagResponseModel> GetByIdAsync(int tagId)
        {
            var tag = await _dbContext.CaseTags.FindAsync(tagId);
            if (tag == null)
                throw new KeyNotFoundException($"Tag with ID {tagId} not found.");

            return ToTagResponse(tag);
        }

        public async Task<IEnumerable<CaseTagResponseModel>> GetAllAsync()
        {
            var tags = await _dbContext.CaseTags.ToListAsync();
            return tags.Select(ToTagResponse);
        }

        public async Task DeleteAsync(int tagId)
        {
            var tag = await _dbContext.CaseTags.FindAsync(tagId);
            if (tag == null)
                throw new KeyNotFoundException($"Tag with ID {tagId} not found.");

            _dbContext.CaseTags.Remove(tag);
            await _dbContext.SaveChangesAsync();
        }

        private CaseTagResponseModel ToTagResponse(CaseTag tag)
        {
            return new CaseTagResponseModel
            {
                TagId = tag.Id,
                Tag = tag.Tag
            };
        }
    }
}
