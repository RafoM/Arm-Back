using ContentService.Data.Entity;
using ContentService.Data;
using Microsoft.EntityFrameworkCore;
using ContentService.Services.Interface;
using ContentService.Models.RequestModels;

namespace ContentService.Services.Implementation
{
    public class LanguageService : ILanguageService
    {
        private readonly ContentDbContext _dbContext;
        private readonly IFileStorageService _fileStorageService;

        public LanguageService(ContentDbContext dbContext, IFileStorageService fileStorageService)
        {
            _dbContext = dbContext;
            _fileStorageService = fileStorageService;
        }

        public async Task<IEnumerable<Language>> GetAllLanguagesAsync()
        {
            return await _dbContext.Languages.ToListAsync();
        }

        public async Task<Language> GetLanguageByIdAsync(int id)
        {
            return await _dbContext.Languages.FindAsync(id);
        }

        public async Task<Language> CreateLanguageAsync(LanguageRequestModel request)
        {
            var language = new Language
            {
                Code = request.Code,
                Name = request.Name,
                IsActive = request.IsActive,
                Flag = request.Flag,
                Translations = request.Translations?.Select(t => new Translation
                {
                    EntityName = t.EntityName,
                    EntityId = t.EntityId,
                    FieldName = t.FieldName,
                    LanguageCode = t.LanguageCode,
                    Value = t.Value,
                    Group = t.Group
                }).ToList()
            };

            _dbContext.Languages.Add(language);
            await _dbContext.SaveChangesAsync();
            return language;
        }

        public async Task UpdateLanguageAsync(int id, LanguageRequestModel request)
        {
            var language = await _dbContext.Languages.FindAsync(id);
            if (language == null)
                throw new Exception("Language not found.");

            language.Code = request.Code;
            language.Name = request.Name;
            language.IsActive = request.IsActive;
            language.Flag = request.Flag;


            _dbContext.Languages.Update(language);
            await _dbContext.SaveChangesAsync();
        }

        public async Task DeleteLanguageAsync(int id)
        {
            var language = await _dbContext.Languages.FindAsync(id);
            if (language == null)
                throw new Exception("Language not found");
            _dbContext.Languages.Remove(language);
            await _dbContext.SaveChangesAsync();
        }

        public async Task<string> UploadFlagAsync(int languageId, IFormFile flagFile)
        {
            var language = await _dbContext.Languages.FindAsync(languageId);
            if (language == null)
                throw new Exception("Language not found.");

            if (flagFile == null || flagFile.Length == 0)
                throw new Exception("Invalid flag file.");

           
            var flagUrl = await _fileStorageService.UploadFileAsync(flagFile, "language-flags");

            language.Flag = flagUrl;
            _dbContext.Languages.Update(language);
            await _dbContext.SaveChangesAsync();

            return flagUrl;
        }
    }
}
