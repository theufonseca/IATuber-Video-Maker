using Domain.Dto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Interfaces
{
    public interface IVoiceService
    {
        Task<FileResponseDto> GenerateVoice(string text, int videoId);
    }
}
