using ContentService.Data.Entity;
using ContentService.Data;
using ContentService.Services.Interface;
using ContentService.Models.ResponseModels;
using ContentService.Models.RequestModels;
using Microsoft.EntityFrameworkCore;
using ContentService.Common.Constants;
using ContentService.Common.Enums;
using Microsoft.Extensions.Caching.Memory;

namespace ContentService.Services.Implementation
{
    public class TutorialService : ITutorialService
    {

        private readonly ContentDbContext _context;
        private readonly IContentTranslationService _translationService;
        private readonly ITutorialSubjectService _tutorialSubjectService;
        private readonly IMemoryCache _cache;

        public TutorialService(ContentDbContext context, IContentTranslationService translationService, ITutorialSubjectService tutorialSubjectService, IMemoryCache cache)
        {
            _context = context;
            _translationService = translationService;
            _tutorialSubjectService = tutorialSubjectService;
            _cache = cache;
        }

        public async Task<TutorialResponseModel> CreateTutorialAsync(TutorialRequestModel request)
        {
            var tutorial = new Tutorial
            {
                Id = Guid.NewGuid(),
                DifficultyId = request.DifficultyId,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            _context.Tutorials.Add(tutorial);
            await _context.SaveChangesAsync();

            foreach (var subjectId in request.SubjectIds)
            {
                var subject = await _context.TutorialSubjects.FindAsync(subjectId);
                if (subject != null)
                {
                    tutorial.Subjects.Add(subject);
                }
            }
            await _context.SaveChangesAsync();

            return await MapTutorial(tutorial, request.LanguageId);
        }

        public async Task<List<TutorialResponseModel>> GetAllTutorialsAsync(int languageId, Guid? difficultyId = null, List<Guid>? subjectIds = null)
        {
            var subjectKey = subjectIds != null && subjectIds.Any()
                ? string.Join(",", subjectIds.OrderBy(id => id))
                : "all";

            var cacheKey = $"tutorials:lang={languageId}:difficulty={difficultyId?.ToString() ?? "all"}:subjects={subjectKey}";

            if (_cache.TryGetValue(cacheKey, out List<TutorialResponseModel> cachedResult))
                return cachedResult;

            var allSubjects = subjectIds is { Count: > 0 }
                ? await _context.TutorialSubjects.Where(s => subjectIds.Contains(s.Id)).ToListAsync()
                : await _context.TutorialSubjects.ToListAsync();

            var tutorialsQuery = _context.Tutorials
                .Include(t => t.DifficultyLevel)
                .Include(t => t.Subjects)
                .AsQueryable();

            if (difficultyId.HasValue)
                tutorialsQuery = tutorialsQuery.Where(t => t.DifficultyId == difficultyId.Value);

            var allTutorials = await tutorialsQuery.ToListAsync();

            var matchingTutorials = allTutorials
                .Where(t => t.Subjects.Any(s => allSubjects.Select(sub => sub.Id).Contains(s.Id)))
                .ToList();

            if (subjectIds is not { Count: > 0 })
                matchingTutorials = allTutorials;

            var result = new List<TutorialResponseModel>();
            foreach (var tutorial in matchingTutorials)
                result.Add(await MapTutorial(tutorial, languageId));

            _cache.Set(cacheKey, result, TimeSpan.FromMinutes(10));
            return result;
        }


        public async Task<TutorialResponseModel> GetTutorialByIdAsync(Guid id, int languageId)
        {
            var tutorial = await _context.Tutorials
                .Include(t => t.DifficultyLevel)
                .Include(t => t.Subjects)
                .FirstOrDefaultAsync(t => t.Id == id);

            if (tutorial == null) throw new KeyNotFoundException("Tutorial not found.");

            return await MapTutorial(tutorial, languageId);
        }

        public async Task UpdateTutorialAsync(TutorialUpdateModel request)
        {
            var tutorial = await _context.Tutorials
                .Include(t => t.Subjects)
                .FirstOrDefaultAsync(t => t.Id == request.Id);

            if (tutorial == null) throw new KeyNotFoundException("Tutorial not found.");

            tutorial.DifficultyId = request.DifficultyId;
            tutorial.UpdatedAt = DateTime.UtcNow;

            tutorial.Subjects.Clear();
            foreach (var subjectId in request.SubjectIds)
            {
                var subject = await _context.TutorialSubjects.FindAsync(subjectId);
                if (subject != null)
                {
                    tutorial.Subjects.Add(subject);
                }
            }

            await _context.SaveChangesAsync();
        }

        public async Task DeleteTutorialAsync(Guid id)
        {
            var tutorial = await _context.Tutorials.FindAsync(id);
            if (tutorial == null) throw new KeyNotFoundException("Tutorial not found.");

            _context.Tutorials.Remove(tutorial);
            await _context.SaveChangesAsync();

            await _translationService.DeleteTranslationsAsync(id, ContentTypeEnum.Tutorial);
        }

        private async Task<TutorialResponseModel> MapTutorial(Tutorial tutorial, int languageId)
        {
            var difficultyName = await _translationService.GetTranslationAsync(
                tutorial.DifficultyId,
                ContentTranslationKeys.Difficulty,
                languageId,
                ContentTypeEnum.DifficultyLevel
            );

            var subjectNames = new List<string>();
            foreach (var subject in tutorial.Subjects)
            {
                var subjectResponse = await _tutorialSubjectService.GetByIdAsync(subject.Id, languageId);
                var translation = subjectResponse.Translations
                    .FirstOrDefault(t => t.LanguageId == languageId)?.Value;

                if (!string.IsNullOrWhiteSpace(translation))
                {
                    subjectNames.Add(translation);
                }
            }

            return new TutorialResponseModel
            {
                Id = tutorial.Id,
                Subject = string.Join(", ", subjectNames),
                Difficulty = difficultyName ?? string.Empty,
                CreatedAt = tutorial.CreatedAt,
                UpdatedAt = tutorial.UpdatedAt
            };
        }
    }
}
