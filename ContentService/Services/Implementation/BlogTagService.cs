using ContentService.Data.Entity;
using ContentService.Data;
using ContentService.Services.Interface;
using ContentService.Models.ResponseModels;
using ContentService.Models.RequestModels;
using Microsoft.EntityFrameworkCore;

namespace ContentService.Services.Implementation
{
    public class BlogTagService : IBlogTagService
    {
        private readonly ContentDbContext _dbContext;

        public BlogTagService(ContentDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<BlogTagResponseModel> CreateAsync(BlogTagRequestModel request)
        {
            var newTag = new BlogTag
            {
                Tag = request.Tag
            };

            _dbContext.BlogTags.Add(newTag);
            await _dbContext.SaveChangesAsync();

            return ToTagResponse(newTag);
        }

        public async Task<BlogTagResponseModel> UpdateAsync(BlogTagUpdateModel request)
        {
            var tag = await _dbContext.BlogTags.FindAsync(request.TagId);
            if (tag == null)
                throw new KeyNotFoundException($"Tag with ID {request.TagId} not found.");

            tag.Tag = request.Tag;
            await _dbContext.SaveChangesAsync();

            return ToTagResponse(tag);
        }

        public async Task<BlogTagResponseModel> GetByIdAsync(int tagId)
        {
            var tag = await _dbContext.BlogTags.FindAsync(tagId);
            if (tag == null)
                throw new KeyNotFoundException($"Tag with ID {tagId} not found.");

            return ToTagResponse(tag);
        }

        public async Task<IEnumerable<BlogTagResponseModel>> GetAllAsync()
        {
            var tags = await _dbContext.BlogTags.ToListAsync();
            return tags.Select(ToTagResponse);
        }

        public async Task DeleteAsync(int tagId)
        {
            var tag = await _dbContext.BlogTags.FindAsync(tagId);
            if (tag == null)
                throw new KeyNotFoundException($"Tag with ID {tagId} not found.");

            _dbContext.BlogTags.Remove(tag);
            await _dbContext.SaveChangesAsync();
        }

        private BlogTagResponseModel ToTagResponse(BlogTag tag)
        {
            return new BlogTagResponseModel
            {
                TagId = tag.Id,
                Tag = tag.Tag
            };
        }
    }
}
