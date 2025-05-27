using TransactionCore.Data.Entity;
using TransactionCore.Data;
using TransactionCore.Models.RequestModels;
using TransactionCore.Models.ResponseModels;
using Microsoft.EntityFrameworkCore;
using TransactionCore.Services.Interface;
using Arbito.Shared.Contracts.ContentTranslation;
using MassTransit;
using TransactionCore.Common.Constants;

namespace TransactionCore.Services.Implementation
{

    public class SubscriptionService : ISubscriptionService
    {
        private readonly TransactionCoreDbContext _dbContext;
        private readonly IRequestClient<GetTranslationsRequest> _translationClient;

        public SubscriptionService(TransactionCoreDbContext dbContext, IRequestClient<GetTranslationsRequest> translationClient)
        {
            _dbContext = dbContext;
            _translationClient = translationClient;
        }

        public async Task<IEnumerable<SubscriptionResponseModel>> GetAllAsync(int? languageId)
        {
            if (languageId == null) languageId = 1;
            var packages = await _dbContext.SubscriptionPackages.ToListAsync();

            var response = new List<SubscriptionResponseModel>();

            foreach (var p in packages)
            {
                var (name, description) = await GetTranslationAsync(p.Id, languageId.Value);

                response.Add(new SubscriptionResponseModel
                {
                    Id = p.Id,
                    Name = name,
                    Description = description,
                    Duration = p.Duration,
                    Price = p.Price,
                    Discount = p.Discount,
                    Currency = p.Currency,
                    FinalPrice = (decimal)(p.Price * (decimal)p.Discount / 100)
                });
            }

            return response;
        }

        public async Task<SubscriptionResponseModel> GetByIdAsync(int id)
        {
            var p = await _dbContext.SubscriptionPackages.FindAsync(id);
            if (p == null) return null;

            var (name, description) = await GetTranslationAsync(p.Id, p.LanguageId);

            return new SubscriptionResponseModel
            {
                Id = p.Id,
                Name = name,
                Description = description,
                Duration = p.Duration,
                Price = p.Price,
                Discount = p.Discount,
                Currency = p.Currency,
                FinalPrice = (decimal)(p.Price * (decimal)p.Discount / 100)
            };
        }

        public async Task<SubscriptionResponseModel> CreateAsync(SubscriptionRequestModel request)
        {
            var package = new SubscriptionPackage
            {
                LanguageId = request.LanguageId,
                Duration = request.Duration,
                Price = request.Price,
                Discount = request.Discount,
                Currency = request.Currency,
                RoleId = request.RoleId,
            };

            _dbContext.SubscriptionPackages.Add(package);
            await _dbContext.SaveChangesAsync();

            return new SubscriptionResponseModel
            {
                Id = package.Id,
                Name = null,
                Description = null,
                Duration = package.Duration,
                Price = package.Price,
                Discount = package.Discount,
                Currency = package.Currency,
                FinalPrice = (decimal)(package.Price * (decimal)package.Discount / 100)
            };
        }

        public async Task UpdateAsync(SubscriptionUpdateModel request)
        {
            var existing = await _dbContext.SubscriptionPackages.FindAsync(request.Id);
            if (existing == null)
                throw new Exception("Subscription package not found");

            existing.Duration = request.Duration;
            existing.Price = request.Price;
            existing.Discount = request.Discount;
            existing.Currency = request.Currency;
            existing.RoleId = request.RoleId;

            _dbContext.SubscriptionPackages.Update(existing);
            await _dbContext.SaveChangesAsync();
        }

        public async Task DeleteAsync(int id)
        {
            var existing = await _dbContext.SubscriptionPackages.FindAsync(id);
            if (existing == null)
                throw new Exception("Subscription package not found");

            _dbContext.SubscriptionPackages.Remove(existing);
            await _dbContext.SaveChangesAsync();
        }

        public async Task<(string? Name, string? Description)> GetTranslationAsync(Guid packageId, int languageId)
        {
            var translations = await _translationClient.GetResponse<GetTranslationsResponse>(new GetTranslationsRequest
            {
                ContentId = packageId,
                ContentType = "SubscriptionPackage"
            });

            var name = translations.Message.Translations
                .FirstOrDefault(t => t.Key == TranslationKeys.Name && t.LanguageId == languageId)?.Value;

            var description = translations.Message.Translations
                .FirstOrDefault(t => t.Key == TranslationKeys.Description && t.LanguageId == languageId)?.Value;

            return (name, description);
        }
    }
}
