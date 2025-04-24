using MassTransit;
using TransactionCore.Models.RequestModels;
using TransactionCore.Services.Interface;

namespace TransactionCore.Consumer
{
    public class CreateUserInfoConsumer : IConsumer<CreateUserInfo>
    {
        private readonly IUserInfoService _userInfoService;

        public CreateUserInfoConsumer(IUserInfoService userInfoService)
        {
            _userInfoService = userInfoService;
        }

        public async Task Consume(ConsumeContext<CreateUserInfo> context)
        {
            var userId = context.Message.UserId;
            var referrerId = context.Message.ReferrerId;
            var promoCode = context.Message.PromoCode;

            await _userInfoService.CreateUserinfoAsync(userId, promoCode, referrerId);
        }
    }
}
