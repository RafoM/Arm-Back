using TransactionCore.Data.Entity;
using TransactionCore.Data;
using TransactionCore.Models.ResponseModels;
using Microsoft.EntityFrameworkCore;
using TransactionCore.Models.RequestModels;
using TransactionCore.Services.Interface;
using TransactionCore.Common.Enums;

namespace TransactionCore.Services.Implementation
{
    public class PaymentMethodService : IPaymentMethodService
    {
        private readonly TransactionCoreDbContext _dbContext;

        public PaymentMethodService(TransactionCoreDbContext dbContext)
        {
            _dbContext = dbContext;
        }


        public async Task<IEnumerable<PaymentMethodResponseModel>> GetAllAsync()
        {
            var methods = await _dbContext.PaymentMethods
                .Include(pm => pm.Crypto)
                .Include(pm => pm.Network)
                .ToListAsync();

            return methods.Select(pm => new PaymentMethodResponseModel
            {
                Id = pm.Id,
                TransactionFee = pm.TransactionFee,
                Note = pm.Note,
                Crypto = new CryptoResponseModel 
                {
                    Id = pm.Crypto.Id,
                    Name = pm.Crypto.Name,
                    IconUrl = pm.Crypto.IconUrl
                },
                Network = new NetworkResponseModel
                {
                    Id = pm.Network.Id,
                    Name=pm.Network.Name,
                    IconUrl = pm.Network.IconUrl
                }
            });
        }

        public async Task<IEnumerable<PaymentMethodResponseModel>> GetAllPaymentMethodsByCryptoIdAsync(Guid cryptoId)
        {
            var methods = await _dbContext.PaymentMethods
                .Include(pm => pm.Crypto)
                .Include(pm => pm.Network)
                .Where(pm => pm.CryptoId == cryptoId && pm.Status == PaymentMethodStatusEnum.Active)
                .ToListAsync();

            return methods.Select(pm => new PaymentMethodResponseModel
            {
                Id = pm.Id,
                TransactionFee = pm.TransactionFee,
                Note = pm.Note,
                Crypto = new CryptoResponseModel
                {
                    Id = pm.Crypto.Id,
                    Name = pm.Crypto.Name,
                    IconUrl = pm.Crypto.IconUrl
                },
                Network = new NetworkResponseModel
                {
                    Id = pm.Network.Id,
                    Name = pm.Network.Name,
                    IconUrl = pm.Network.IconUrl
                }
            }).ToList();
        }


        public async Task<PaymentMethodResponseModel> GetByIdAsync(Guid id)
        {
            var pm = await _dbContext.PaymentMethods
                .Include(p => p.Crypto)
                .Include(p => p.Network)
                .FirstOrDefaultAsync(p => p.Id == id);

            if (pm == null)
                return null;

            return new PaymentMethodResponseModel
            {
                Id = pm.Id,
                TransactionFee = pm.TransactionFee,
                Note = pm.Note,
                Crypto = new CryptoResponseModel
                {
                    Id = pm.Crypto.Id,
                    Name = pm.Crypto.Name,
                    IconUrl = pm.Crypto.IconUrl
                },
                Network = new NetworkResponseModel
                {
                    Id = pm.Network.Id,
                    Name = pm.Network.Name,
                    IconUrl = pm.Network.IconUrl
                }
            };
        }

        public async Task<PaymentMethodResponseModel> CreateAsync(PaymentMethodRequestModel request)
        {
          
            var paymentMethod = new PaymentMethod
            {
                CryptoId = request.CryptoId,
                NetworkId = request.NetworkId,
                TransactionFee = request.TransactionFee,
                Note = request.Note,
                Status =PaymentMethodStatusEnum.Deactive
            };
            if (request.Status)
            {
                paymentMethod.Status = PaymentMethodStatusEnum.Active;
            }
            _dbContext.PaymentMethods.Add(paymentMethod);
            await _dbContext.SaveChangesAsync();

            await _dbContext.Entry(paymentMethod).Reference(pm => pm.Crypto).LoadAsync();
            await _dbContext.Entry(paymentMethod).Reference(pm => pm.Network).LoadAsync();

            return new PaymentMethodResponseModel
            {
                Id = paymentMethod.Id,
                TransactionFee = paymentMethod.TransactionFee,
                Note = paymentMethod.Note,
                Crypto = new CryptoResponseModel
                {
                    Id = paymentMethod.Crypto.Id,
                    Name = paymentMethod.Crypto.Name,
                    IconUrl = paymentMethod.Crypto.IconUrl
                },
                Network = new NetworkResponseModel
                {
                    Id = paymentMethod.Network.Id,
                    Name = paymentMethod.Network.Name,
                    IconUrl = paymentMethod.Network.IconUrl
                }
            };
        }

        public async Task UpdateAsync(PaymentMethodUpdateModel request)
        {
            var existing = await _dbContext.PaymentMethods.FindAsync(request.Id);
            if (existing == null)
                throw new Exception("Payment method not found");

            existing.CryptoId = request.CryptoId;
            existing.NetworkId = request.NetworkId;
            existing.TransactionFee = request.TransactionFee;
            existing.Note = request.Note;
            existing.Status = PaymentMethodStatusEnum.Deactive;

            if (request.Status)
            {
                existing.Status = PaymentMethodStatusEnum.Active;
            }
            _dbContext.PaymentMethods.Update(existing);
            await _dbContext.SaveChangesAsync();
        }

        public async Task DeleteAsync(Guid id)
        {
            var existing = await _dbContext.PaymentMethods.FindAsync(id);
            if (existing == null)
                throw new Exception("Payment method not found");

            _dbContext.PaymentMethods.Remove(existing);
            await _dbContext.SaveChangesAsync();
        }
    }
}
