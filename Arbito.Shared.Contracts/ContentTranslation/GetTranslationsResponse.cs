using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Arbito.Shared.Contracts.ContentTranslation
{
    public class GetTranslationsResponse
    {
        public List<TranslationDto> Translations { get; set; } = new();
    }
}
