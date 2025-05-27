using Arbito.Shared.Contracts.ContentTranslation;
using ContentService.Common.Enums;
using ContentService.Services.Interface;
using MassTransit;

namespace ContentService.Consumers
{
    public class SetTranslationConsumer : IConsumer<SetTranslationRequest>
    {
        private readonly IContentTranslationService _translationService;

        public SetTranslationConsumer(IContentTranslationService translationService)
        {
            _translationService = translationService;
        }

        public async Task Consume(ConsumeContext<SetTranslationRequest> context)
        {
            if (!Enum.TryParse<ContentTypeEnum>(context.Message.ContentType, out var contentType))
            {
                throw new ArgumentException($"Invalid content type: {context.Message.ContentType}");
            }

            await _translationService.SetTranslationAsync(
                context.Message.ContentId,
                context.Message.Key,
                context.Message.Value,
                context.Message.LanguageId,
                contentType
            );

            await context.RespondAsync(new SetTranslationResponse { Success = true });
        }
    }

}
