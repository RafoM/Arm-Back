using ContentService.Data.Entity;
using ContentService.Data;
using Microsoft.EntityFrameworkCore;
using ContentService.Services.Interface;
using ContentService.Models.RequestModels;
using ContentService.Models.ResponseModels;

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

        public async Task<IEnumerable<LanguageResponseModel>> GetAllAsync()
        {
            return await _dbContext.Languages
                .Select(l => new LanguageResponseModel
                {
                    Id = l.Id,
                    CultureCode = l.CultureCode,
                    DisplayName = l.DisplayName
                }).ToListAsync();
        }

        public async Task<LanguageResponseModel> GetByIdAsync(int id)
        {
            var lang = await _dbContext.Languages.FindAsync(id);
            if (lang == null) return null;

            return new LanguageResponseModel
            {
                Id = lang.Id,
                CultureCode = lang.CultureCode,
                DisplayName = lang.DisplayName
            };
        }

        public async Task<int> CreateAsync(LanguageRequestModel model)
        {
            var lang = new Language
            {
                CultureCode = model.CultureCode,
                DisplayName = model.DisplayName
            };

            _dbContext.Languages.Add(lang);
            await _dbContext.SaveChangesAsync();

            return lang.Id;
        }

        public async Task<bool> UpdateAsync(LanguageUpdateModel model)
        {
            var lang = await _dbContext.Languages.FindAsync(model.Id);
            if (lang == null) throw new InvalidOperationException($"Language with ID {model.Id} was not found.");

            lang.CultureCode = model.CultureCode;
            lang.DisplayName = model.DisplayName;

            await _dbContext.SaveChangesAsync();
            return true;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var lang = await _dbContext.Languages.FindAsync(id);
            if (lang == null) return false;

            _dbContext.Languages.Remove(lang);
            await _dbContext.SaveChangesAsync();
            return true;
        }

        public async Task<string> UploadFlagAsync(int languageId, IFormFile flagFile)
        {
            var language = await _dbContext.Languages.FindAsync(languageId);
            if (language == null)
                throw new Exception("Language not found.");

            if (flagFile == null || flagFile.Length == 0)
                throw new Exception("Invalid flag file.");

           
            var flagUrl = await _fileStorageService.UploadFileAsync(flagFile, "language-flags");

            language.FlagUrl = flagUrl;
            _dbContext.Languages.Update(language);
            await _dbContext.SaveChangesAsync();

            return flagUrl;
        }
    }
}
