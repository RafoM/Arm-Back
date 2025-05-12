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
        private readonly IFileStorageService _fileStorageService;

        public CryptoService(TransactionCoreDbContext dbContext, IFileStorageService fileStorageService)
        {
            _dbContext = dbContext;
            _fileStorageService = fileStorageService;
        }

        public async Task<IEnumerable<CryptoResponseModel>> GetAllAsync()
        {
            var cryptos = await _dbContext.Cryptos.ToListAsync();
            return cryptos.Select(c => new CryptoResponseModel
            {
                Id = c.Id,
                Name = c.Name,
                IconUrl = c.IconUrl
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
                Name = crypto.Name,
                IconUrl = crypto.IconUrl
            };
        }

        public async Task<CryptoResponseModel> CreateAsync(CryptoRequestModel request)
        {
            var crypto = new Crypto
            {
                Name = request.Name,
                IconUrl = request.IconUrl
            };

            _dbContext.Cryptos.Add(crypto);
            await _dbContext.SaveChangesAsync();

            return new CryptoResponseModel
            {
                Id = crypto.Id,
                Name = crypto.Name,
                IconUrl = request.IconUrl
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

        public async Task<string> UploadIconAsync(int cryptoId, IFormFile iconFile)
        {
            var crypto = await _dbContext.Cryptos.FindAsync(cryptoId);
            if (crypto == null)
                throw new Exception("Crypto not found.");

            if (iconFile == null || iconFile.Length == 0)
                throw new Exception("Invalid icon file.");


            var iconUrl = await _fileStorageService.UploadFileAsync(iconFile, "crypto-icons");

            crypto.IconUrl = iconUrl;
            _dbContext.Cryptos.Update(crypto);
            await _dbContext.SaveChangesAsync();

            return iconUrl;
        }
    }
}
