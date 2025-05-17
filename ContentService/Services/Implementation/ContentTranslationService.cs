using ContentService.Common.Enums;
using ContentService.Data.Entity;
using ContentService.Data;
using ContentService.Services.Interface;
using Microsoft.EntityFrameworkCore;

namespace ContentService.Services.Implementation
{
    public class ContentTranslationService : IContentTranslationService
    {
        private readonly ContentDbContext _context;

        public ContentTranslationService(ContentDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<ContentTranslation>> GetTranslationsAsync(Guid contentId, ContentTypeEnum contentType)
        {
            return await _context.ContentTranslations
                .Where(ct => ct.ContentId == contentId && ct.ContentType == contentType)
                .ToListAsync();
        }

        public async Task<string?> GetTranslationAsync(Guid contentId, string key, int languageId, ContentTypeEnum contentType)
        {
            return await _context.ContentTranslations
                .Where(ct => ct.ContentId == contentId && ct.Key == key && ct.LanguageId == languageId && ct.ContentType == contentType)
                .Select(ct => ct.Value)
                .FirstOrDefaultAsync();
        }

        public async Task SetTranslationAsync(Guid contentId, string key, string value, int languageId, ContentTypeEnum contentType)
        {
            var existing = await _context.ContentTranslations
                .FirstOrDefaultAsync(ct => ct.ContentId == contentId && ct.Key == key && ct.LanguageId == languageId && ct.ContentType == contentType);

            if (existing != null)
            {
                existing.Value = value;
                _context.ContentTranslations.Update(existing);
            }
            else
            {
                var translation = new ContentTranslation
                {
                    Id = Guid.NewGuid(),
                    ContentId = contentId,
                    Key = key,
                    Value = value,
                    LanguageId = languageId,
                    ContentType = contentType
                };
                _context.ContentTranslations.Add(translation);
            }

            await _context.SaveChangesAsync();
        }

        public async Task DeleteTranslationsAsync(Guid contentId, ContentTypeEnum contentType)
        {
            var translations = await _context.ContentTranslations
                .Where(ct => ct.ContentId == contentId && ct.ContentType == contentType)
                .ToListAsync();

            _context.ContentTranslations.RemoveRange(translations);
            await _context.SaveChangesAsync();
        }
    }
}
