using ContentService.Data.Entity;
using ContentService.Data;
using ContentService.Services.Interface;
using Microsoft.EntityFrameworkCore;
using ContentService.Models.RequestModels;
using ContentService.Models.ResponseModels;
using ContentService.Common.Constants;
using ContentService.Common.Enums;

namespace ContentService.Services.Implementation
{
    public class CaseService : ICaseService
    {
        private readonly ContentDbContext _dbContext;
        private readonly IContentTranslationService _translationService;
        private readonly IFileStorageService _fileStorageService;

        public CaseService(
            ContentDbContext dbContext,
            IContentTranslationService translationService,
            IFileStorageService fileStorageService)
        {
            _dbContext = dbContext;
            _translationService = translationService;
            _fileStorageService = fileStorageService;
        }

        public async Task<CaseResponseModel> CreateAsync(CaseRequestModel request)
        {
            var Case = new Case
            {
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow,
            };

            if (request.TagIds != null && request.TagIds.Count > 0)
            {
                var tags = await _dbContext.CaseTags
                    .Where(t => request.TagIds.Contains(t.Id))
                    .ToListAsync();
                foreach (var tag in tags)
                {
                    Case.CaseTags.Add(tag);
                }
            }

            _dbContext.Cases.Add(Case);
            await _dbContext.SaveChangesAsync();

            var contentId = Case.Id;

            await _translationService.SetTranslationAsync(contentId, ContentTranslationKeys.Title, request.Title, request.LanguageId, ContentTypeEnum.Case);
            await _translationService.SetTranslationAsync(contentId, ContentTranslationKeys.Subtitle, request.Subtitle, request.LanguageId, ContentTypeEnum.Case);
            await _translationService.SetTranslationAsync(contentId, ContentTranslationKeys.Content, request.Content, request.LanguageId, ContentTypeEnum.Case);
            await _translationService.SetTranslationAsync(contentId, ContentTranslationKeys.MainImage, request.MainImage, request.LanguageId, ContentTypeEnum.Case);


            return await GetByIdAsync(contentId, request.LanguageId);
        }

        public async Task<CaseResponseModel> UpdateAsync(CaseUpdateModel request)
        {
            var Case = await _dbContext.Cases
                .Include(b => b.CaseTags)
                .FirstOrDefaultAsync(b => b.Id == request.CaseId);

            if (Case == null)
                throw new KeyNotFoundException($"Case with ID {request.CaseId} not found.");

            Case.UpdatedAt = DateTime.UtcNow;

            if (request.TagIds != null)
            {
                Case.CaseTags.Clear();
                var newTags = await _dbContext.CaseTags
                    .Where(t => request.TagIds.Contains(t.Id))
                    .ToListAsync();

                foreach (var tag in newTags)
                {
                    Case.CaseTags.Add(tag);
                }
            }

            await _translationService.SetTranslationAsync(request.CaseId, ContentTranslationKeys.Title, request.Title, request.LanguageId, ContentTypeEnum.Case);
            await _translationService.SetTranslationAsync(request.CaseId, ContentTranslationKeys.Subtitle, request.Subtitle, request.LanguageId, ContentTypeEnum.Case);
            await _translationService.SetTranslationAsync(request.CaseId, ContentTranslationKeys.Content, request.Content, request.LanguageId, ContentTypeEnum.Case);
            await _translationService.SetTranslationAsync(request.CaseId, ContentTranslationKeys.MainImage, request.MainImage, request.LanguageId, ContentTypeEnum.Case);

            await _dbContext.SaveChangesAsync();

            return await GetByIdAsync(Case.Id, request.LanguageId);
        }

        public async Task<CaseResponseModel> GetByIdAsync(Guid CaseId, int languageId)
        {
            var Case = await _dbContext.Cases
                .Include(b => b.CaseTags)
                .FirstOrDefaultAsync(b => b.Id == CaseId);

            if (Case == null)
                throw new KeyNotFoundException($"Case with ID {CaseId} not found.");

            return await ToCaseResponse(Case, languageId);
        }

        public async Task<IEnumerable<CaseResponseModel>> GetAllAsync(int languageId)
        {
            var Cases = await _dbContext.Cases
                .Include(b => b.CaseTags)
                .OrderByDescending(b => b.CreatedAt)
                .ToListAsync();

            var responseList = new List<CaseResponseModel>();
            foreach (var Case in Cases)
            {
                responseList.Add(await ToCaseResponse(Case, languageId));
            }

            return responseList;
        }

        public async Task<IEnumerable<CaseResponseModel>> GetByTagIdsAsync(List<Guid> tagIds, int languageId)
        {
            var Cases = await _dbContext.Cases
                .Include(b => b.CaseTags)
                .Where(b => b.CaseTags.Any(tag => tagIds.Contains(tag.Id)))
                .OrderByDescending(b => b.CreatedAt)
                .ToListAsync();

            var responseList = new List<CaseResponseModel>();
            foreach (var Case in Cases)
            {
                responseList.Add(await ToCaseResponse(Case, languageId));
            }

            return responseList;
        }

        public async Task DeleteAsync(Guid CaseId)
        {
            var Case = await _dbContext.Cases.FindAsync(CaseId);
            if (Case == null)
                throw new KeyNotFoundException($"Case with ID {CaseId} not found.");

            _dbContext.Cases.Remove(Case);
            await _dbContext.SaveChangesAsync();
        }

        private async Task<CaseResponseModel> ToCaseResponse(Case Case, int languageId)
        {
            var tagResponses = new List<CaseTagResponseModel>();

            foreach (var tag in Case.CaseTags)
            {
                var translatedTag = await _translationService.GetTranslationAsync(tag.Id, ContentTranslationKeys.Tag, languageId, ContentTypeEnum.CaseTag);
                tagResponses.Add(new CaseTagResponseModel
                {
                    TagId = tag.Id,
                    Tag = translatedTag
                });
            }

            return new CaseResponseModel
            {
                CaseId = Case.Id,
                Title = await _translationService.GetTranslationAsync(Case.Id, ContentTranslationKeys.Title, languageId, ContentTypeEnum.Case) ?? string.Empty,
                Subtitle = await _translationService.GetTranslationAsync(Case.Id, ContentTranslationKeys.Subtitle, languageId, ContentTypeEnum.Case),
                MainImage = await _translationService.GetTranslationAsync(Case.Id, ContentTranslationKeys.MainImage, languageId, ContentTypeEnum.Case),
                Content = await _translationService.GetTranslationAsync(Case.Id, ContentTranslationKeys.Content, languageId, ContentTypeEnum.Case) ?? string.Empty,
                CreatedAt = Case.CreatedAt,
                UpdatedAt = Case.UpdatedAt,
                LanguageId = languageId,
                Tags = tagResponses
            };
        }

        public async Task<string> UploadCaseMediaAsync(IFormFile mediaFile)
        {
            if (mediaFile == null || mediaFile.Length == 0)
                throw new Exception("Invalid media file.");

            var fileUrl = await _fileStorageService.UploadFileAsync(mediaFile, "Case-media");
            return fileUrl;
        }
    }
}
