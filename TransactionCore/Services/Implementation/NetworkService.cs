using TransactionCore.Data.Entity;
using TransactionCore.Data;
using TransactionCore.Models.ResponseModels;
using TransactionCore.Models.RequestModels;
using Microsoft.EntityFrameworkCore;
using TransactionCore.Services.Interface;

namespace TransactionCore.Services.Implementation
{
    public class NetworkService : INetworkService
    {
        private readonly TransactionCoreDbContext _dbContext;
        private readonly IFileStorageService _fileStorageService;

        public NetworkService(TransactionCoreDbContext dbContext, IFileStorageService fileStorageService)
        {
            _dbContext = dbContext;
            _fileStorageService = fileStorageService;
        }

        public async Task<IEnumerable<NetworkResponseModel>> GetAllAsync()
        {
            var networks = await _dbContext.Networks.ToListAsync();
            return networks.Select(n => new NetworkResponseModel
            {
                Id = n.Id,
                Name = n.Name,
                IconUrl = n.IconUrl
            });
        }

        public async Task<NetworkResponseModel> GetByIdAsync(Guid id)
        {
            var network = await _dbContext.Networks.FindAsync(id);
            if (network == null)
                return null;

            return new NetworkResponseModel
            {
                Id = network.Id,
                Name = network.Name,
                IconUrl = network.IconUrl
            };
        }

        public async Task<NetworkResponseModel> CreateAsync(NetworkRequestModel request)
        {
            var network = new Network
            {
                Name = request.Name
            };

            _dbContext.Networks.Add(network);
            await _dbContext.SaveChangesAsync();

            return new NetworkResponseModel
            {
                Id = network.Id,
                Name = network.Name,
                IconUrl = network.IconUrl
            };
        }

        public async Task UpdateAsync(NetworkUpdateModel request)
        {
            var existing = await _dbContext.Networks.FindAsync(request.Id);
            if (existing == null)
                throw new Exception("Network not found");

            existing.Name = request.Name;
            _dbContext.Networks.Update(existing);
            await _dbContext.SaveChangesAsync();
        }

        public async Task DeleteAsync(Guid id)
        {
            var existing = await _dbContext.Networks.FindAsync(id);
            if (existing == null)
                throw new Exception("Network not found");

            _dbContext.Networks.Remove(existing);
            await _dbContext.SaveChangesAsync();
        }

        public async Task<string> UploadIconAsync(int networkId, IFormFile iconFile)
        {
            var network = await _dbContext.Networks.FindAsync(networkId);
            if (network == null)
                throw new Exception("Network not found.");

            if (iconFile == null || iconFile.Length == 0)
                throw new Exception("Invalid icon file.");


            var iconUrl = await _fileStorageService.UploadFileAsync(iconFile, "network-icons");

            network.IconUrl = iconUrl;
            _dbContext.Networks.Update(network);
            await _dbContext.SaveChangesAsync();

            return iconUrl;
        }
    }
}
