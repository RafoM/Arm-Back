using TransactionCore.Data.Entity;
using TransactionCore.Data;
using TransactionCore.Models.ResponseModels;
using Microsoft.EntityFrameworkCore;
using TransactionCore.Models.RequestModels;
using TransactionCore.Services.Interface;

namespace TransactionCore.Services.Implementation
{
    public class CryptoService : ICryptoService
    {
        private readonly TransactionCoreDbContext _dbContext;

        public CryptoService(TransactionCoreDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<IEnumerable<CryptoResponseModel>> GetAllAsync()
        {
            var cryptos = await _dbContext.Cryptos.ToListAsync();
            return cryptos.Select(c => new CryptoResponseModel
            {
                Id = c.Id,
                Name = c.Name
            });
        }

        public async Task<CryptoResponseModel> GetByIdAsync(Guid id)
        {
            var crypto = await _dbContext.Cryptos.FindAsync(id);
            if (crypto == null)
                return null;

            return new CryptoResponseModel
            {
                Id = crypto.Id,
                Name = crypto.Name
            };
        }

        public async Task<CryptoResponseModel> CreateAsync(CryptoRequestModel request)
        {
            var crypto = new Crypto
            {
                Name = request.Name
            };

            _dbContext.Cryptos.Add(crypto);
            await _dbContext.SaveChangesAsync();

            return new CryptoResponseModel
            {
                Id = crypto.Id,
                Name = crypto.Name
            };
        }

        public async Task UpdateAsync(CryptoUpdateModel request)
        {
            var existing = await _dbContext.Cryptos.FindAsync(request.Id);
            if (existing == null)
                throw new Exception("Crypto not found");

            existing.Name = request.Name;
            _dbContext.Cryptos.Update(existing);
            await _dbContext.SaveChangesAsync();
        }

        public async Task DeleteAsync(Guid id)
        {
            var existing = await _dbContext.Cryptos.FindAsync(id);
            if (existing == null)
                throw new Exception("Crypto not found");

            _dbContext.Cryptos.Remove(existing);
            await _dbContext.SaveChangesAsync();
        }
    }
}
