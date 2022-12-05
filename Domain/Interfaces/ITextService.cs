using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Interfaces
{
    public interface ITextService
    {
        Task<string> GenerateTitle(string phrase);
        Task<string> GenerateText(string phrase);
        Task<string> GenerateKeyWords(string phrase);
    }
}
