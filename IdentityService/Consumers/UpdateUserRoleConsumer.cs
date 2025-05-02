using Arbito.Shared.Contracts.Identity;
using IdentityService.Models.RequestModels;
using IdentityService.Services.Interface;
using MassTransit;

namespace IdentityService.Consumers
{
    public class UpdateUserRoleConsumer : IConsumer<IUpdateUserRoleRequest>
    {
        private readonly IUserService _userService;

        public UpdateUserRoleConsumer(IUserService userService)
        {
            _userService = userService;
        }

        public async Task Consume(ConsumeContext<IUpdateUserRoleRequest> context)
        {
            var request = new UpdateUserRoleRequestModel
            {
                RoleId = context.Message.NewRoleId,
                UserId = context.Message.UserId
            };
            await _userService.UpdateUserRoleAsync(request);
        }
    }
}
