using Arbito.Shared.Contracts.Transaction;
using MassTransit;
using TransactionCore.Models.RequestModels;
using TransactionCore.Services.Interface;

namespace TransactionCore.Consumer
{
    public class CreateUserInfoConsumer : IConsumer<ICreateUserInfoRequest>
    {
        private readonly IUserInfoService _userInfoService;

        public CreateUserInfoConsumer(IUserInfoService userInfoService)
        {
            _userInfoService = userInfoService;
        }

        public async Task Consume(ConsumeContext<ICreateUserInfoRequest> context)
        {
            var userId = context.Message.UserId;
            var referrerId = context.Message.ReferrerId;
            var promoCode = context.Message.PromoCode;
            var email = context.Message.Email;
            await _userInfoService.CreateUserinfoAsync(userId, email, promoCode, referrerId);
        }
    }
}
