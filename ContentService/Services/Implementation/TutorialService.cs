using ContentService.Data.Entity;
using ContentService.Data;
using ContentService.Services.Interface;
using ContentService.Models.ResponseModels;
using ContentService.Models.RequestModels;
using Microsoft.EntityFrameworkCore;

namespace ContentService.Services.Implementation
{
    public class TutorialService : ITutorialService
    {
        private readonly ContentDbContext _context;

        public TutorialService(ContentDbContext context)
        {
            _context = context;
        }

        public async Task<TutorialResponseModel> CreateTutorialAsync(TutorialRequestModel request)
        {
            var tutorial = new Tutorial
            {
                Subject = request.Subject,
                Difficulty = request.Difficulty,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            _context.Tutorials.Add(tutorial);
            await _context.SaveChangesAsync();

            return MapTutorial(tutorial);
        }

        public async Task<List<TutorialResponseModel>> GetAllTutorialsAsync()
        {
            return await _context.Tutorials
                .Select(t => MapTutorial(t))
                .ToListAsync();
        }

        public async Task<TutorialResponseModel> GetTutorialByIdAsync(int id)
        {
            var tutorial = await _context.Tutorials.FindAsync(id);
            if (tutorial == null) throw new KeyNotFoundException("Tutorial not found.");

            return MapTutorial(tutorial);
        }

        public async Task UpdateTutorialAsync(TutorialUpdateModel request)
        {
            var tutorial = await _context.Tutorials.FindAsync(request.Id);
            if (tutorial == null) throw new KeyNotFoundException("Tutorial not found.");

            tutorial.Subject = request.Subject;
            tutorial.Difficulty = request.Difficulty;
            tutorial.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();
        }

        public async Task DeleteTutorialAsync(int id)
        {
            var tutorial = await _context.Tutorials.FindAsync(id);
            if (tutorial == null) throw new KeyNotFoundException("Tutorial not found.");

            _context.Tutorials.Remove(tutorial);
            await _context.SaveChangesAsync();
        }

        private static TutorialResponseModel MapTutorial(Tutorial tutorial) => new TutorialResponseModel
        {
            Id = tutorial.Id,
            Subject = tutorial.Subject,
            Difficulty = tutorial.Difficulty,
            CreatedAt = tutorial.CreatedAt,
            UpdatedAt = tutorial.UpdatedAt
        };
    }
}
