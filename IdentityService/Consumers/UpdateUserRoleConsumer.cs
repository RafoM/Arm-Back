using IdentityService.Models.RequestModels;
using IdentityService.Services.Interface;
using MassTransit;

namespace IdentityService.Consumers
{
    public class UpdateUserRoleConsumer : IConsumer<UpdateUserRoleRequestModel>
    {
        private readonly IUserService _userService;

        public UpdateUserRoleConsumer(IUserService userService)
        {
            _userService = userService;
        }

        public async Task Consume(ConsumeContext<UpdateUserRoleRequestModel> context)
        {
            await _userService.UpdateUserRoleAsync(context.Message);
        }
    }
}
