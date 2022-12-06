using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infra.ExternalServices.Translate
{
    public class TranslateResponse
    {
        public List<TranslationData> translations { get; set; }
    }

    public class TranslationData
    {
        public string translation { get; set; }
    }
}
