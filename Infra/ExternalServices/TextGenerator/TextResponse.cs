using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infra.ExternalServices.TextService.TextGenerator
{
    public class CompletionsResponse
    {
        public List<Choice> Choices { get; set; }
    }

    public class Choice
    {
        public string Text { get; set; }
    }
}
