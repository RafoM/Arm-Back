using Arbito.Shared.Contracts.ContentTranslation;
using ContentService.Common.Enums;
using ContentService.Services.Interface;
using MassTransit;

namespace ContentService.Consumers
{
    public class GetTranslationsConsumer : IConsumer<GetTranslationsRequest>
    {
        private readonly IContentTranslationService _translationService;

        public GetTranslationsConsumer(IContentTranslationService translationService)
        {
            _translationService = translationService;
        }

        public async Task Consume(ConsumeContext<GetTranslationsRequest> context)
        {
            if (!Enum.TryParse<ContentTypeEnum>(context.Message.ContentType, out var contentType))
            {
                throw new ArgumentException($"Invalid content type: {context.Message.ContentType}");
            }

            var translations = await _translationService.GetTranslationsAsync(context.Message.ContentId, contentType);

            var response = new GetTranslationsResponse
            {
                Translations = translations.Select(t => new TranslationDto
                {
                    Key = t.Key,
                    Value = t.Value,
                    LanguageId = t.LanguageId
                }).ToList()
            };

            await context.RespondAsync(response);
        }
    }
}
