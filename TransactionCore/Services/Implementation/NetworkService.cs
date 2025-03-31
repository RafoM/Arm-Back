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

        public NetworkService(TransactionCoreDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<IEnumerable<NetworkUpdateModel>> GetAllAsync()
        {
            var networks = await _dbContext.Networks.ToListAsync();
            return networks.Select(n => new NetworkUpdateModel
            {
                Id = n.Id,
                Name = n.Name
            });
        }

        public async Task<NetworkUpdateModel> GetByIdAsync(Guid id)
        {
            var network = await _dbContext.Networks.FindAsync(id);
            if (network == null)
                return null;

            return new NetworkUpdateModel
            {
                Id = network.Id,
                Name = network.Name
            };
        }

        public async Task<NetworkUpdateModel> CreateAsync(NetworkRequestModel request)
        {
            var network = new Network
            {
                Name = request.Name
            };

            _dbContext.Networks.Add(network);
            await _dbContext.SaveChangesAsync();

            return new NetworkUpdateModel
            {
                Id = network.Id,
                Name = network.Name
            };
        }

        public async Task UpdateAsync(NetworkResponseModel request)
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
    }
}
