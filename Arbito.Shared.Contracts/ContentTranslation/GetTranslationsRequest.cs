using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Arbito.Shared.Contracts.ContentTranslation
{
    public class GetTranslationsRequest
    {
        public Guid ContentId { get; set; }
        public string ContentType { get; set; } = default!;
    }
}
