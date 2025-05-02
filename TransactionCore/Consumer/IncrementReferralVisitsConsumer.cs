using MassTransit;
using TransactionCore.Data;
using Arbito.Shared.Contracts.Transaction;
using Microsoft.EntityFrameworkCore;

namespace TransactionCore.Consumer
{
    public class IncrementReferralVisitsConsumer : IConsumer<IIncrementReferralVisits>
    {
        private readonly TransactionCoreDbContext _db;

        public IncrementReferralVisitsConsumer(TransactionCoreDbContext db)
        {
            _db = db;
        }

        public async Task Consume(ConsumeContext<IIncrementReferralVisits> context)
        {
            var userInfo = await _db.UserInfos.FirstOrDefaultAsync(u => u.UserId == context.Message.UserId);
            if (userInfo != null)
            {
                userInfo.Visits += 1;
                await _db.SaveChangesAsync();
            }
        }
    }

}
