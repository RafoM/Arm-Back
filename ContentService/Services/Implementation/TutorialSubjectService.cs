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
    public class TutorialSubjectService : ITutorialSubjectService
    {
        private readonly ContentDbContext _context;
        private readonly IContentTranslationService _translationService;

        public TutorialSubjectService(ContentDbContext context, IContentTranslationService translationService)
        {
            _context = context;
            _translationService = translationService;
        }

        public async Task<TutorialSubjectResponseModel> CreateAsync(TutorialSubjectRequestModel request)
        {
            var subject = new TutorialSubject
            {
                Id = Guid.NewGuid(),
                Code = request.Code,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            _context.TutorialSubjects.Add(subject);
            await _context.SaveChangesAsync();

            foreach (var translation in request.Translations)
            {
                await _translationService.SetTranslationAsync(
                    subject.Id,
                    ContentTranslationKeys.Subject,
                    translation.Value,
                    translation.LanguageId,
                    ContentTypeEnum.TutorialSubject);
            }

            return await MapSubject(subject);
        }

        public async Task<List<TutorialSubjectResponseModel>> GetAllAsync()
        {
            var subjects = await _context.TutorialSubjects.ToListAsync();
            var result = new List<TutorialSubjectResponseModel>();

            foreach (var subject in subjects)
            {
                result.Add(await MapSubject(subject));
            }

            return result;
        }

        public async Task<TutorialSubjectResponseModel> GetByIdAsync(Guid id, int languageId)
        {
            var subject = await _context.TutorialSubjects.FindAsync(id);
            if (subject == null)
                throw new KeyNotFoundException("Subject not found.");

            var translation = await _translationService.GetTranslationAsync(
                id,
                ContentTranslationKeys.Subject,
                languageId,
                ContentTypeEnum.TutorialSubject);

            return new TutorialSubjectResponseModel
            {
                Id = subject.Id,
                Code = subject.Code,
                Translations = new List<TranslatedItem>
        {
            new TranslatedItem
            {
                LanguageId = languageId,
                Value = translation ?? string.Empty
            }
        },
                CreatedAt = subject.CreatedAt,
                UpdatedAt = subject.UpdatedAt
            };
        }


        public async Task UpdateAsync(TutorialSubjectUpdateModel request)
        {
            var subject = await _context.TutorialSubjects.FindAsync(request.Id);
            if (subject == null) throw new KeyNotFoundException("Subject not found.");

            subject.Code = request.Code;
            subject.UpdatedAt = DateTime.UtcNow;
            await _context.SaveChangesAsync();

            foreach (var translation in request.Translations)
            {
                await _translationService.SetTranslationAsync(
                    subject.Id,
                    ContentTranslationKeys.Subject,
                    translation.Value,
                    translation.LanguageId,
                    ContentTypeEnum.TutorialSubject);
            }
        }

        public async Task DeleteAsync(Guid id)
        {
            var subject = await _context.TutorialSubjects.FindAsync(id);
            if (subject == null) throw new KeyNotFoundException("Subject not found.");

            _context.TutorialSubjects.Remove(subject);
            await _context.SaveChangesAsync();

            await _translationService.DeleteTranslationsAsync(id, ContentTypeEnum.TutorialSubject);
        }

        private async Task<TutorialSubjectResponseModel> MapSubject(TutorialSubject subject)
        {
            var translations = await _translationService.GetAllTranslationsAsync(
                subject.Id,
                ContentTranslationKeys.Subject,
                ContentTypeEnum.TutorialSubject);

            var translatedItems = translations.Select(t => new TranslatedItem
            {
                LanguageId = t.LanguageId,
                Value = t.Value
            }).ToList();

            return new TutorialSubjectResponseModel
            {
                Id = subject.Id,
                Code = subject.Code,
                Translations = translatedItems,
                CreatedAt = subject.CreatedAt,
                UpdatedAt = subject.UpdatedAt
            };
        }
    }
}
