using ContentService.Data.Entity;
using ContentService.Data;
using ContentService.Services.Interface;
using ContentService.Models.ResponseModels;
using ContentService.Models.RequestModels;
using Microsoft.EntityFrameworkCore;
using ContentService.Common.Constants;
using ContentService.Common.Enums;

namespace ContentService.Services.Implementation
{
    public class CaseTagService : ICaseTagService
    {
        private readonly ContentDbContext _dbContext;
        private readonly IContentTranslationService _translationService;

        public CaseTagService(ContentDbContext dbContext, IContentTranslationService translationService)
        {
            _dbContext = dbContext;
            _translationService = translationService;
        }

        public async Task<CaseTagResponseModel> CreateAsync(CaseTagRequestModel request)
        {
            var newTag = new CaseTag
            {
                Id = Guid.NewGuid()
            };

            _dbContext.CaseTags.Add(newTag);
            await _dbContext.SaveChangesAsync();

            await _translationService.SetTranslationAsync(
                newTag.Id,
                ContentTranslationKeys.Tag,
                request.Tag,
                request.LanguageId,
                ContentTypeEnum.CaseTag);

            return new CaseTagResponseModel
            {
                TagId = newTag.Id,
                Tag = request.Tag
            };
        }

        public async Task<CaseTagResponseModel> UpdateAsync(CaseTagUpdateModel request)
        {
            var tag = await _dbContext.CaseTags.FindAsync(request.TagId);
            if (tag == null)
                throw new KeyNotFoundException($"Tag with ID {request.TagId} not found.");

            await _translationService.SetTranslationAsync(
                tag.Id,
                ContentTranslationKeys.Tag,
                request.Tag,
                request.LanguageId,
                ContentTypeEnum.CaseTag);

            return new CaseTagResponseModel
            {
                TagId = tag.Id,
                Tag = request.Tag
            };
        }

        public async Task<CaseTagResponseModel> GetByIdAsync(Guid tagId, int languageId)
        {
            var tag = await _dbContext.CaseTags.FindAsync(tagId);
            if (tag == null)
                throw new KeyNotFoundException($"Tag with ID {tagId} not found.");

            var translatedTag = await _translationService.GetTranslationAsync(
                tag.Id,
                ContentTranslationKeys.Tag,
                languageId,
                ContentTypeEnum.CaseTag);

            return new CaseTagResponseModel
            {
                TagId = tag.Id,
                Tag = translatedTag ?? "(untranslated)"
            };
        }

        public async Task<IEnumerable<CaseTagResponseModel>> GetAllAsync(int languageId)
        {
            var tags = await _dbContext.CaseTags.ToListAsync();
            var result = new List<CaseTagResponseModel>();

            foreach (var tag in tags)
            {
                var translatedTag = await _translationService.GetTranslationAsync(
                    tag.Id,
                    ContentTranslationKeys.Tag,
                    languageId,
                    ContentTypeEnum.CaseTag);

                result.Add(new CaseTagResponseModel
                {
                    TagId = tag.Id,
                    Tag = translatedTag ?? "(untranslated)"
                });
            }

            return result;
        }

        public async Task DeleteAsync(Guid tagId)
        {
            var tag = await _dbContext.CaseTags.FindAsync(tagId);
            if (tag == null)
                throw new KeyNotFoundException($"Tag with ID {tagId} not found.");

            _dbContext.CaseTags.Remove(tag);
            await _translationService.DeleteTranslationsAsync(tagId, ContentTypeEnum.CaseTag);
            await _dbContext.SaveChangesAsync();
        }
    }
}
