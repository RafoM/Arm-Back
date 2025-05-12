using ContentService.Data.Entity;
using ContentService.Data;
using ContentService.Services.Interface;
using Microsoft.EntityFrameworkCore;
using ContentService.Models.RequestModels;
using ContentService.Models.ResponseModels;

namespace ContentService.Services.Implementation
{
    public class CaseService : ICaseService
    {
        private readonly ContentDbContext _dbContext;
        private readonly IFileStorageService _fileStorageService;

        public CaseService(ContentDbContext dbContext, IFileStorageService fileStorageService)
        {
            _dbContext = dbContext;
            _fileStorageService = fileStorageService;
        }

        public async Task<CaseResponseModel> CreateAsync(CaseRequestModel request)
        {
            var newCase = new Case
            {
                Title = request.Title,
                Subtitle = request.Subtitle,
                MainImage = request.MainImage,
                Content = request.Content,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow,
                LanguageId = request.LanguageId,
            };

            if (request.TagIds != null && request.TagIds.Count > 0)
            {
                var tags = await _dbContext.CaseTags
                    .Where(t => request.TagIds.Contains(t.Id))
                    .ToListAsync();
                foreach (var tag in tags)
                {
                    newCase.CaseTags.Add(tag);
                }
            }

            _dbContext.Cases.Add(newCase);
            await _dbContext.SaveChangesAsync();

            return await GetByIdAsync(newCase.Id);
        }

        public async Task<CaseResponseModel> UpdateAsync(CaseUpdateModel request)
        {
            var Case = await _dbContext.Cases
                .Include(b => b.CaseTags)
                .FirstOrDefaultAsync(b => b.Id == request.CaseId);

            if (Case == null)
                throw new KeyNotFoundException($"Case with ID {request.CaseId} not found.");

            Case.Title = request.Title;
            Case.Subtitle = request.Subtitle;
            Case.MainImage = request.MainImage;
            Case.Content = request.Content;
            Case.UpdatedAt = DateTime.UtcNow;
            Case.LanguageId = request.LanguageId;
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

            await _dbContext.SaveChangesAsync();

            return await GetByIdAsync(Case.Id);
        }

        public async Task<CaseResponseModel> GetByIdAsync(int CaseId)
        {
            var Case = await _dbContext.Cases
                .Include(b => b.CaseTags)
                .FirstOrDefaultAsync(b => b.Id == CaseId);

            if (Case == null)
                throw new KeyNotFoundException($"Case with ID {CaseId} not found.");

            return ToCaseResponse(Case);
        }

        public async Task<IEnumerable<CaseResponseModel>> GetAllAsync()
        {
            var Cases = await _dbContext.Cases
                .Include(b => b.CaseTags)
                .OrderByDescending(b => b.CreatedAt)
                .ToListAsync();

            return Cases.Select(ToCaseResponse);
        }

        public async Task DeleteAsync(int CaseId)
        {
            var Case = await _dbContext.Cases.FindAsync(CaseId);
            if (Case == null)
                throw new KeyNotFoundException($"Case with ID {CaseId} not found.");

            _dbContext.Cases.Remove(Case);
            await _dbContext.SaveChangesAsync();
        }

        private CaseResponseModel ToCaseResponse(Case Case)
        {
            return new CaseResponseModel
            {
                CaseId = Case.Id,
                Title = Case.Title,
                Subtitle = Case.Subtitle,
                MainImage = Case.MainImage,
                Content = Case.Content,
                CreatedAt = Case.CreatedAt,
                UpdatedAt = Case.UpdatedAt,
                LanguageId = Case.LanguageId,
                Tags = Case.CaseTags
                    .Select(t => new CaseTagResponseModel
                    {
                        TagId = t.Id,
                        Tag = t.Tag
                    })
                    .ToList()
            };
        }

        public async Task<string> UploadCaseMediaAsync(IFormFile mediaFile)
        {

            if (mediaFile == null || mediaFile.Length == 0)
                throw new Exception("Invalid media file.");

            var flagUrl = await _fileStorageService.UploadFileAsync(mediaFile, "case-media");

            return flagUrl;
        }
    }
}
