using Arbito.Shared.Contracts.Identity;
using Google;
using IdentityService.Data;
using MassTransit;
using Microsoft.EntityFrameworkCore;

namespace IdentityService.Consumers
{
    public class GetUserRoleConsumer : IConsumer<IGetUserRoleRequest>
    {
        private readonly IdentityDbContext _dbContext;

        public GetUserRoleConsumer(IdentityDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task Consume(ConsumeContext<IGetUserRoleRequest> context)
        {
            var user = await _dbContext.Users
                .Include(u => u.Role)
                .FirstOrDefaultAsync(u => u.Id == context.Message.UserId);

            await context.RespondAsync<IGetUserRoleResponse>(new
            {
                Role = user?.Role?.Name ?? "Unknown"
            });
        }
    }

}
