using ContentService.Common.Constants;
using ContentService.Common.Enums;
using ContentService.Data;
using ContentService.Data.Entity;
using ContentService.Models.RequestModels;
using ContentService.Models.ResponseModels;
using ContentService.Services.Interface;
using Microsoft.EntityFrameworkCore;

namespace ContentService.Services.Implementation
{
    public class BlogTagService : IBlogTagService
    {
        private readonly ContentDbContext _dbContext;
        private readonly IContentTranslationService _translationService;

        public BlogTagService(ContentDbContext dbContext, IContentTranslationService translationService)
        {
            _dbContext = dbContext;
            _translationService = translationService;
        }

        public async Task<BlogTagResponseModel> CreateAsync(BlogTagRequestModel request)
        {
            var newTag = new BlogTag
            {
                Id = Guid.NewGuid()
            };

            _dbContext.BlogTags.Add(newTag);
            await _dbContext.SaveChangesAsync();

            await _translationService.SetTranslationAsync(
                newTag.Id,
                ContentTranslationKeys.Tag,
                request.Tag,
                request.LanguageId,
                ContentTypeEnum.BlogTag);

            return new BlogTagResponseModel
            {
                TagId = newTag.Id,
                Tag = request.Tag
            };
        }

        public async Task<BlogTagResponseModel> UpdateAsync(BlogTagUpdateModel request)
        {
            var tag = await _dbContext.BlogTags.FindAsync(request.TagId);
            if (tag == null)
                throw new KeyNotFoundException($"Tag with ID {request.TagId} not found.");

            await _translationService.SetTranslationAsync(
                tag.Id,
                ContentTranslationKeys.Tag,
                request.Tag,
                request.LanguageId,
                ContentTypeEnum.BlogTag);

            return new BlogTagResponseModel
            {
                TagId = tag.Id,
                Tag = request.Tag
            };
        }

        public async Task<BlogTagResponseModel> GetByIdAsync(Guid tagId, int languageId)
        {
            var tag = await _dbContext.BlogTags.FindAsync(tagId);
            if (tag == null)
                throw new KeyNotFoundException($"Tag with ID {tagId} not found.");

            var translatedTag = await _translationService.GetTranslationAsync(
                tag.Id,
                ContentTranslationKeys.Tag,
                languageId,
                ContentTypeEnum.BlogTag);

            return new BlogTagResponseModel
            {
                TagId = tag.Id,
                Tag = translatedTag ?? "(untranslated)"
            };
        }

        public async Task<IEnumerable<BlogTagResponseModel>> GetAllAsync(int languageId)
        {
            var tags = await _dbContext.BlogTags.ToListAsync();
            var result = new List<BlogTagResponseModel>();

            foreach (var tag in tags)
            {
                var translatedTag = await _translationService.GetTranslationAsync(
                    tag.Id,
                    ContentTranslationKeys.Tag,
                    languageId,
                    ContentTypeEnum.BlogTag);

                result.Add(new BlogTagResponseModel
                {
                    TagId = tag.Id,
                    Tag = translatedTag ?? "(untranslated)"
                });
            }

            return result;
        }

        public async Task DeleteAsync(Guid tagId)
        {
            var tag = await _dbContext.BlogTags.FindAsync(tagId);
            if (tag == null)
                throw new KeyNotFoundException($"Tag with ID {tagId} not found.");

            _dbContext.BlogTags.Remove(tag);
            await _translationService.DeleteTranslationsAsync(tagId, ContentTypeEnum.BlogTag);
            await _dbContext.SaveChangesAsync();
        }
    }
}
