using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Arbito.Shared.Contracts.ContentTranslation
{
    public class TranslationDto
    {
        public string Key { get; set; } = default!;
        public string Value { get; set; } = default!;
        public int LanguageId { get; set; }
    }
}
