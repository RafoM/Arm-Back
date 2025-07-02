using Arbito.Shared.Contracts.Transaction;
using MassTransit;
using TransactionCore.Models.RequestModels;
using TransactionCore.Services.Implementation;
using TransactionCore.Services.Interface;

namespace TransactionCore.Consumer
{
    public class CreateUserInfoConsumer : IConsumer<ICreateUserInfoRequest>
    {
        private readonly IUserInfoService _userInfoService;
        private readonly ILogger<CreateUserInfoConsumer> _logger;

        public CreateUserInfoConsumer(IUserInfoService userInfoService, ILogger<CreateUserInfoConsumer> logger)
        {
            _userInfoService = userInfoService;
            _logger = logger;
        }

        public async Task Consume(ConsumeContext<ICreateUserInfoRequest> context)
        {
            _logger.LogInformation("User info consume");

            var userId = context.Message.UserId;
            var referrerId = context.Message.ReferrerId;
            var promoCode = context.Message.PromoCode;
            var email = context.Message.Email;
            await _userInfoService.CreateUserinfoAsync(userId, email, promoCode, referrerId);
        }
    }
}
